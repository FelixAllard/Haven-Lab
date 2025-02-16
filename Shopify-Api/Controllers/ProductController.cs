using Shopify_Api.Exceptions;
using Shopify_Api.Model;
using ShopifySharp;
using ShopifySharp.Factories;
using ShopifySharp.Lists;
using Product = ShopifySharp.Product;

namespace Shopify_Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _shopifyService;
    private readonly ProductValidator _productValidator;
    private readonly IMetaFieldService _metaFieldService;

    public ProductsController(
        IProductServiceFactory productServiceFactory,
        ShopifyRestApiCredentials credentials,
        ProductValidator productValidator,
        IMetaFieldServiceFactory metaFieldServiceFactory
    )
    {
        _shopifyService = productServiceFactory.Create(new ShopifySharp.Credentials.ShopifyApiCredentials(
                credentials.ShopUrl,
                credentials.AccessToken
            )
        );
        
        _metaFieldService = metaFieldServiceFactory.Create(new ShopifySharp.Credentials.ShopifyApiCredentials(
                credentials.ShopUrl,
                credentials.AccessToken
            )
        );
        
        _productValidator = productValidator;
    }
    
    //================================ PRODUCT ENDPOINTS ==================================
    
    [HttpGet("")]
    public async Task<IActionResult> GetAllProducts([FromQuery] SearchArguments searchArguments = null)
    {
        try
        {
            var products = await _shopifyService.ListAsync();

            if (searchArguments == null)
            {
                return Ok(products);
            }

            var filteredItems = products.Items.AsQueryable();

            // Filter by name if provided
            if (!string.IsNullOrWhiteSpace(searchArguments.Name))
            {
                filteredItems = filteredItems.Where(x => x.Title.ToLower().Contains(searchArguments.Name.ToLower()));
            }

            // Filter by minimum price if provided
            if (searchArguments.MinimumPrice > 0)
            {
                filteredItems = filteredItems.Where(x => x.Variants.FirstOrDefault().Price >= searchArguments.MinimumPrice);
            }

            // Filter by maximum price if provided
            if (searchArguments.MaximumPrice > 0)
            {
                filteredItems = filteredItems.Where(x => x.Variants.FirstOrDefault().Price <= searchArguments.MaximumPrice);
            }

            // Filter by availability if specified
            if (searchArguments.Available)
            {
                filteredItems = filteredItems.Where(x => x.Variants.FirstOrDefault().InventoryQuantity > 0);
            }

            return Ok(new ListResult<Product>(filteredItems.ToList(), null));
        }
        catch (ShopifyException ex)
        {
            return StatusCode(404, new { message = "Error fetching product", details = ex.Message });
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, new { message = "Error fetching product", details = ex.Message });
        }
    }

    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById([FromRoute]long id)
    {
        try
        {
            Console.WriteLine(id);
            var products = await _shopifyService.GetAsync(id);
            return Ok(products);
        }
        catch (ShopifyException ex)
        {
            return StatusCode(404, new { message = "Error fetching products", details = ex.Message });
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, new { message = "Error fetching products" + ex.Message });
        }
    }
    
    [HttpPost("")]
    public virtual async Task<IActionResult> PostProduct([FromBody] Product product)
    {
        try
        {
            Product tempProduct = _productValidator.FormatPostProduct(product);
            Console.Write("We formatted!");
            var products = await _shopifyService.CreateAsync(tempProduct);
            return Ok(products);
        }
        catch (InputException ex)
        {
            return StatusCode(400, new { message = ex.Message });
        }
        catch (ShopifyException ex)
        {
            return StatusCode(500, new { message = "Error fetching products", details = ex.Message });
        }
        catch (System.Exception ex)
        {
            // Log the exception if necessary
            return StatusCode(500, new { message = "Error creating product " + ex.Message });
        }
    }
    
    [HttpPut("{id}")]
    public virtual async Task<IActionResult> PutProduct([FromRoute] long id,[FromBody] Product product)
    {
        try
        {
            //Product tempProduct = _productValidator.FormatPostProduct(product);
            //Console.Write("We formatted!");
            var products = await _shopifyService.UpdateAsync(id, product);
            return Ok(products);
        }
        catch (InputException ex)
        {
            return StatusCode(400, new { message = ex.Message });
        }
        catch (ShopifyException ex)
        {
            return StatusCode(500, new { message = "Error fetching products", details = ex.Message });
        }
        catch (System.Exception ex)
        {
            // Log the exception if necessary
            return StatusCode(500, new { message = "Error updating product " + ex.Message });
        }
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct([FromRoute]long id)
    {
        try
        {
            Console.WriteLine(id);
            await _shopifyService.DeleteAsync(id);
            return Ok("Product deleted");
        }
        catch (ShopifyException ex)
        {
            return StatusCode(404, new { message = "No product found", details = ex.Message });
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, new { message = "Error deleting products" + ex.Message });
        }
    }
    
    [HttpGet("variant/{productId}")]
    public async Task<IActionResult> GetFirstVariantByProductId([FromRoute] long productId)
    {
        try
        {
            //fetch product using the Shopify service
            var product = await _shopifyService.GetAsync(productId);

            if (product == null || product.Variants == null || !product.Variants.Any())
            {
                return NotFound(new { message = "Product not found or no variants available." });
            }
            
            var firstVariant = product.Variants.FirstOrDefault();

            long firstVariantId = firstVariant.Id.Value;
            
            return Ok(new { VariantId = firstVariantId });
        }

        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error fetching product variants", details = ex.Message });
        }
    }

    //================================ TRANSLATED METAFIELD ENDPOINTS ==================================
    
    [HttpGet("{id}/translation")]
    public async Task<IActionResult> GetTranslatedProduct([FromRoute] long id, [FromQuery] string lang = "fr")
    {
        try
        {
            var metafields = await _metaFieldService.ListAsync(id, "products");

            var titleMetafield = metafields?.Items?.FirstOrDefault(m => m.Key == $"title_{lang}" && m.Namespace == "translations");
            var descriptionMetafield = metafields?.Items?.FirstOrDefault(m => m.Key == $"description_{lang}" && m.Namespace == "translations");

            string translatedTitle = titleMetafield?.Value?.ToString() ?? "N/A";
            string translatedDescription = descriptionMetafield?.Value?.ToString() ?? "N/A";

            return Ok(new
            {
                ProductId = id,
                Title = translatedTitle,
                Description = translatedDescription
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error fetching translations", details = ex.Message });
        }
    }

    [HttpPost("{id}/translation")]
    public async Task<IActionResult> AddProductTranslation([FromRoute] long id, [FromBody] TranslationRequest request)
    {
        try
        {
            var metafieldTitle = new MetaField()
            {
                Namespace = "translations",
                Key = $"title_{request.Locale}",
                Value = request.Title,
                Type = "string",
                OwnerId = id,
                OwnerResource = "product"
            };

            var metafieldDescription = new MetaField()
            {
                Namespace = "translations",
                Key = $"description_{request.Locale}",
                Value = request.Description,
                Type = "string",
                OwnerId = id,
                OwnerResource = "product"
            };

            await _metaFieldService.CreateAsync(metafieldTitle, id, "products");
            await _metaFieldService.CreateAsync(metafieldDescription, id, "products");

            return Ok(new { message = "Translation added!", productId = id });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error saving translation", details = ex.Message });
        }
    }
}
