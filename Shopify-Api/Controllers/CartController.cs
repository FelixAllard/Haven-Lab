using Microsoft.AspNetCore.Mvc;
using ShopifySharp;
using ShopifySharp.Factories;

namespace Shopify_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : Controller
{
    private readonly IProductService _productService;
    
    public CartController(
        IProductServiceFactory productServiceFactory,
        Shopify_Api.ShopifyRestApiCredentials credentials
    )
    {
        _productService = productServiceFactory.Create(new ShopifySharp.Credentials.ShopifyApiCredentials(
                credentials.ShopUrl,
                credentials.AccessToken
            )
        );
    }
    
    [HttpPost("get-first-variants")]
    public async Task<IActionResult> GetFirstVariants([FromBody] Dictionary<long, int> productIdQuantities)
    {
        if (productIdQuantities == null || productIdQuantities.Count == 0)
        {
            return BadRequest("Invalid or empty input.");
        }

        var result = new Dictionary<long?, int>();

        foreach (var entry in productIdQuantities)
        {
            long productId = entry.Key;
            int quantity = entry.Value;

            try
            {
                // Get the product by its ID
                var product = await _productService.GetAsync(productId);

                // Check if the product has variants
                if (product.Variants != null && product.Variants.Count() > 0)
                {
                    long? firstVariantId = product.Variants.First().Id;
                    if(firstVariantId != null)
                        result[firstVariantId] = quantity;
                }
                else
                {
                    return NotFound($"No variants found for product ID: {productId}");
                }
            }
            catch (ShopifyException ex)
            {
                return StatusCode(500, $"Shopify API error for product ID {productId}: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error processing product ID {productId}: {ex.Message}");
            }
        }
        return Ok(result);
    }
}