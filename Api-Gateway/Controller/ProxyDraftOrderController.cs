using Api_Gateway.Services;
using Microsoft.AspNetCore.Mvc;
using ShopifySharp;

namespace Api_Gateway.Controller;
[Route("gateway/api/[controller]")]
[ApiController]
public class ProxyDraftOrderController : ControllerBase{
    
    private readonly ServiceDraftOrderController _serviceOrderController;

    // Constructor injects the ShopifyApiService
    public ProxyDraftOrderController(ServiceDraftOrderController shopifyApiService)
    {
        _serviceOrderController = shopifyApiService;
    }

    [HttpPost]
    public async Task<IActionResult> PostDraftOrder([FromBody] DraftOrder order)
    {
        try
        {
            var (statusCode, result) = await _serviceOrderController.PostDraftOrder(order);

            if (statusCode == 200)
            {
                // Return 200 OK with the result as JSON
                return Ok(result);
            }
            else
            {
                // Return the received status code with the result message
                return StatusCode(statusCode, new { Message = result });
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            // Return 503 Service Unavailable for unexpected exceptions
            return StatusCode(503, new { Message = e.Message });
        }
    }

    
    
}