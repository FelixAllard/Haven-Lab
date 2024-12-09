using System.Text;
using Api_Gateway.Services;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> GetAllOrders()
    {
        try
        {
            var result = await _serviceOrderController.GetAllOrdersAsync();
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