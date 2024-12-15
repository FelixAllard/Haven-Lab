using System.Net.Sockets;
using System.Text;
using Api_Gateway.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShopifySharp;

namespace Api_Gateway.Controller;
[Route("gateway/api/[controller]")]
[ApiController]
public class ProxyProductController : ControllerBase
{
    private readonly ServiceProductController _serviceProductController;

    // Constructor injects the ShopifyApiService
    public ProxyProductController(ServiceProductController shopifyApiService)
    {
        _serviceProductController = shopifyApiService;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllProducts()
    {
        try
        {
            var result = await _serviceProductController.GetAllProductsAsync();
            if (result.StartsWith("Error"))
            {
                // Return 500 Internal Server Error with error message
                return StatusCode(500, new { Message = result });
            }
    
            // Return 200 OK with the result as JSON
            return Ok(result);  // The result will be serialized as JSON automatically
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, new { Message = e.Message });
        }
        
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById([FromRoute]long id)
    {
        try
        {
            var result = await _serviceProductController.GetProductByIdAsync(id);
            if (result.Contains("404 Not Found"))
            {
                return NotFound(new { message = result });
            }
            if (result.StartsWith("Error"))
            {
                // Return 500 Internal Server Error with error message
                return StatusCode(500, new { Message = result });
            }
    
            // Return 200 OK with the result as JSON
            return Ok(result);  // The result will be serialized as JSON automatically
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, new { Message = e.Message });
        }
    }

    [HttpPost("")]
    public async Task<IActionResult> PostProduct([FromBody] Product product)
    {
        try
        {
            var response = await _serviceProductController.CreateProductAsync(product);
            if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
            {
                return StatusCode(503, new { message = "Service is currently unavailable, please try again later." });
            }

            var content = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, content); // Ensure status code matches
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred", details = ex.Message });
        }
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> PutProduct([FromRoute]long id, [FromBody] Product product)
    {
        try
        {
            var response = await _serviceProductController.PutProductAsync(id, product);
            if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
            {
                return StatusCode(503, new { message = "Service is currently unavailable, please try again later." });
            }

            var content = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, content); // Ensure status code matches
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred", details = ex.Message });
        }
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct([FromRoute]long id)
    {
        try
        {
            var result = await _serviceProductController.DeleteProductAsync(id);
            if (result.Contains("404 Not Found"))
            {
                return NotFound(new { message = result });
            }
            if (result.StartsWith("Error"))
            {
                // Return 500 Internal Server Error with error message
                return StatusCode(500, new { Message = result });
            }
    
            // Return 200 OK with the result as JSON
            return Ok(result);  // The result will be serialized as JSON automatically
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, new { Message = e.Message });
        }
    }
    
    [HttpGet("variant/{productId}")]
    public async Task<IActionResult> GetFirstVariantByProductId([FromRoute] long productId)
    {
        try
        {
            // Fetch product data using the service
            var productJson = await _serviceProductController.GetProductByIdAsync(productId);

            if (string.IsNullOrEmpty(productJson) || productJson.Contains("404 Not Found"))
            {
                return NotFound(new { message = "Product not found." });
            }

            // Deserialize the product JSON to a dynamic object
            var product = JsonConvert.DeserializeObject<dynamic>(productJson);

            // Check if Variants exist
            if (product?.variants == null || product.variants.Count == 0)
            {
                return NotFound(new { message = "No variants available for the specified product." });
            }

            // Get the first variant ID
            long? firstVariantId = product.variants[0].id;

            if (firstVariantId == null)
            {
                return NotFound(new { message = "First variant ID is null or unavailable." });
            }

            // Return the first variant ID
            return Ok(new { VariantId = firstVariantId.Value });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return StatusCode(500, new { message = "Error fetching product variants", details = ex.Message });
        }
    }
}