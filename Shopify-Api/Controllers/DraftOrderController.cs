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
public class DraftOrderController : ControllerBase
{
    private readonly IDraftOrderService _shopifyDraftOrderService;

    public DraftOrderController(
        IDraftOrderServiceFactory draftOrderServiceFactory,
        Shopify_Api.ShopifyRestApiCredentials credentials
        )
    {
        _shopifyDraftOrderService = draftOrderServiceFactory.Create(new ShopifySharp.Credentials.ShopifyApiCredentials(
                credentials.ShopUrl,
                credentials.AccessToken
            )
        );
    }
    [HttpPost("")]
    public virtual async Task<IActionResult> PostDraftOrder([FromBody] DraftOrder order)
    {
        try
        { 
            var products = await _shopifyDraftOrderService.CreateAsync(order);
            string invoiceUrl = products.InvoiceUrl;
            return Ok(invoiceUrl);
        }
        catch (InputException ex)
        {
            return StatusCode(400, new { message = ex.Message });
        }
        catch (ShopifyException ex)
        {
            if(ex.Message=="(404 Not Found) Not Found")
                return StatusCode(404, new { message = ex.Message });
            if(ex.Message=="(400 Bad Request) draft_order: Required parameter missing or invalid")
                return StatusCode(400, new { message = ex.Message});
            
            
            return StatusCode(500, new { message = "Error fetching products", details = ex.Message });
        }
        catch (System.Exception ex)
        {
            // Log the exception if necessary
            return StatusCode(500, new { message = "Error creating product " + ex.Message });
        }
    }
    
}
