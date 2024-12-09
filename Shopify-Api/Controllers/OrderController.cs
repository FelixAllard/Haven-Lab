using System.Diagnostics;
using ShopifySharp;
using ShopifySharp.Factories;
using ShopifySharp.Filters;

namespace Shopify_Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _shopifyService;

    public OrderController(
        IOrderServiceFactory productServiceFactory,
        Shopify_Api.ShopifyRestApiCredentials credentials
        )
    {

        
        _shopifyService = productServiceFactory.Create(new ShopifySharp.Credentials.ShopifyApiCredentials(
                credentials.ShopUrl,
                credentials.AccessToken
            )
        );
        
        
        //_shopifyService = new ShopifyService(shopUrl, accessToken);
    }
    [HttpGet("")]
    public async Task<IActionResult> GetAllOrders()
    {
        try
        {
            var products = await _shopifyService.ListAsync();
            return Ok(products);
        }
        catch (ShopifyException ex)
        {
            return StatusCode(500, new { message = "Error fetching orders", details = ex.Message });
        }
    }
    
    [HttpGet("{orderId}")]
    public async Task<IActionResult> GetOrderByIdAsync(long orderId)
    {
        try
        {
            // Retrieve the order using the service
            var result = await _shopifyService.GetAsync(orderId);

            if (result == null)
            {
                // Return 404 Not Found if no order is found
                return NotFound(new { Message = $"Order with ID {orderId} not found" });
            }

            // Return 200 OK with the order as JSON
            return Ok(result);
        }
        catch (ShopifyException ex)
        {
            // Handle Shopify-specific exceptions
            throw new Exception($"Failed to retrieve order with ID {orderId}: {ex.Message}", ex);
        }
    }

    
}
