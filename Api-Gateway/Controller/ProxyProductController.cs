using System.Net.Sockets;
using System.Text;
using Api_Gateway.Services;
using Microsoft.AspNetCore.Mvc;
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
}