using System.Diagnostics;
using Shopify_Api.Exceptions;
using ShopifySharp;
using ShopifySharp.Factories;

namespace Shopify_Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _shopifyService;
    private readonly ProductValidator _productValidator;

    public ProductsController(
        IProductServiceFactory productServiceFactory,
        Shopify_Api.ShopifyRestApiCredentials credentials,
        ProductValidator productValidator
        )
    {
        _shopifyService = productServiceFactory.Create(new ShopifySharp.Credentials.ShopifyApiCredentials(
                credentials.ShopUrl,
                credentials.AccessToken
            )
        );
        _productValidator = productValidator;
        
        
        //_shopifyService = new ShopifyService(shopUrl, accessToken);
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllProducts()
    {
        try
        {
            var products = await _shopifyService.ListAsync();
            return Ok(products);
        }
        catch (ShopifyException ex)
        {
            return StatusCode(500, new { message = "Error fetching product", details = ex.Message });
        }
        catch (System.Exception ex)
        {
            // Log the exception if necessary
            return StatusCode(500, new { message = "Error fetching product" + ex.Message });
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
}
