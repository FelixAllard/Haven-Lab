using ShopifySharp;

namespace Shopify_Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ShopifyService _shopifyService;

    public ProductsController()
    {
        // Replace these with your Shopify store credentials
        string shopUrl = "vc-shopz.myshopify.com";
        string accessToken = "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";
        
        _shopifyService = new ShopifyService(shopUrl, accessToken);
    }

    [HttpGet("get-all-products")]
    public async Task<IActionResult> GetAllProducts()
    {
        try
        {
            var products = await _shopifyService.GetAllProductsAsync();
            return Ok(products);
        }
        catch (ShopifyException ex)
        {
            return StatusCode(500, new { message = "Error fetching products", details = ex.Message });
        }
    }
}
