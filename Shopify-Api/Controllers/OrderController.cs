using System.Diagnostics;
using Shopify_Api.Exceptions;
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

    [HttpPut("{orderId}")]
    public virtual async Task<IActionResult> PutProduct([FromRoute] long orderId,[FromBody] Order order)
    {
        try
        {
            // Serialize the order object to JSON for better readability in the console
            string orderJson = System.Text.Json.JsonSerializer.Serialize(order);
            Console.WriteLine($"Order being updated: {orderJson}");
            var returnedOrder = await _shopifyService.UpdateAsync(orderId, order);
            
            string returnedOrderJson = System.Text.Json.JsonSerializer.Serialize(returnedOrder);
            Console.WriteLine($"Order being updated: {returnedOrderJson}");
            
            return Ok(returnedOrder);
        }
        catch (InputException ex)
        {
            return StatusCode(400, new { message = ex.Message });
        }
        catch (ShopifyException ex)
        {
            return StatusCode(500, new { message = "Error updating order", details = ex.Message });
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, new { message = "Error updating order " + ex.Message });
        }
    }
}
