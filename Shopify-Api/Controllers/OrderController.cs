using System.Diagnostics;
using ShopifySharp;
using ShopifySharp.Factories;

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
            return StatusCode(500, new { message = "Error fetching products", details = ex.Message });
        }
    }
    
    //Add your functions here! Check the ProductController to see how to use the shopify service
}
