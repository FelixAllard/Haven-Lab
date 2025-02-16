using System.Text;
using Api_Gateway.Services;
using Microsoft.AspNetCore.Mvc;
using Api_Gateway.Models;
using ShopifySharp;
using Api_Gateway.Annotations;

namespace Api_Gateway.Controller;
[Route("gateway/api/[controller]")]
[ApiController]
public class ProxyOrderController : ControllerBase
{
    private readonly ServiceOrderController _serviceOrderController;

    // Constructor injects the ShopifyApiService
    public ProxyOrderController(ServiceOrderController shopifyApiService)
    {
        _serviceOrderController = shopifyApiService;
    }

    [HttpGet("")]
    [RequireAuth]
    public async Task<IActionResult> GetAllOrders([FromQuery] OrderSearchArgument searchArguments = null)
    {
        try 
        {
            var result = await _serviceOrderController.GetAllOrdersAsync(searchArguments);
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
    
    [HttpGet("{orderId}")]
    [RequireAuth]
    public async Task<IActionResult> GetOrderById(long orderId)
    {
        try
        {
            var result = await _serviceOrderController.GetOrderByIdAsync(orderId);

            if (result == null)
            {
                // Return 404 Not Found if no order is found with the given ID
                return NotFound(new { Message = $"Order with ID {orderId} not found" });
            }

            if (result.StartsWith("Error"))
            {
                // Return 500 Internal Server Error with error message
                return StatusCode(500, new { Message = result });
            }

            // Return 200 OK with the result as JSON
            return Ok(result); // The result will be serialized as JSON automatically
        }
        catch (Exception e)
        {
            // Log the exception and return 500 Internal Server Error
            Console.WriteLine(e);
            return StatusCode(500, new { Message = e.Message });
        }
    }
    
    [HttpPut("{id}")]
    [RequireAuth]
    public async Task<IActionResult> PutOrder([FromRoute]long id, [FromBody] Order order)
    {
        try
        {
            var response = await _serviceOrderController.PutOrderAsync(id, order);
            if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
            {
                return StatusCode(503, new { message = "Service is currently unavailable, please try again later." });
            }

            var content = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, content);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred", details = ex.Message });
        }
    }

}