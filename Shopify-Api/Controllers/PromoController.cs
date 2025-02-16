using Microsoft.AspNetCore.Mvc;
using Shopify_Api.Exceptions;
using Shopify_Api.Model;
using ShopifySharp;
using ShopifySharp.Factories;

namespace Shopify_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PromoController : ControllerBase
{
    private readonly IPriceRuleService _priceRuleService;
    private readonly IDiscountCodeService _discountCodeService;


    public PromoController(
        IPriceRuleServiceFactory priceServiceFactory,
        IDiscountCodeServiceFactory discountServiceFactory,
        Shopify_Api.ShopifyRestApiCredentials credentials
    )
    {
        _priceRuleService = priceServiceFactory.Create(new ShopifySharp.Credentials.ShopifyApiCredentials(
                credentials.ShopUrl,
                credentials.AccessToken
            )
        );
        
        _discountCodeService = discountServiceFactory.Create(new ShopifySharp.Credentials.ShopifyApiCredentials(
                credentials.ShopUrl,
                credentials.AccessToken
            )
        );
    }

    //================================ PRICE RULES ==================================
    [HttpGet("PriceRules")] 
    public async Task<IActionResult> GetAllPriceRules()
    {
        try
        {
            var pricerules = await _priceRuleService.ListAsync();
            
            return Ok(pricerules);
        }
        catch (ShopifyException ex)
        {
            return StatusCode(404, new { message = "Error fetching price rules", details = ex.Message });
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, new { message = "Error fetching price rules", details = ex.Message });
        }
    }
    
    [HttpGet("PriceRules/{id}")] 
    public async Task<IActionResult> GetPriceRulesById([FromRoute]long id)
    {
        try
        {
            var pricerule = await _priceRuleService.GetAsync(id);
            
            return Ok(pricerule);
        }
        catch (ShopifyException ex)
        {
            return StatusCode(404, new { message = "Error fetching price rule", details = ex.Message });
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, new { message = "Error fetching price rule", details = ex.Message });
        }
    }
    
    [HttpPost("PriceRules")]
    public virtual async Task<IActionResult> PostPriceRule([FromBody] PriceRule request)
    {
        try
        {
            PriceRule tempPriceRule = request;
            var pricerule = await _priceRuleService.CreateAsync(tempPriceRule);
            return Ok(pricerule);
        }
        catch (InputException ex)
        {
            return StatusCode(400, new { message = ex.Message });
        }
        catch (ShopifyException ex)
        {
            return StatusCode(500, new { message = "Error fetching PriceRules", details = ex.Message });
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, new { message = "Error creating PriceRule " + ex.Message });
        }
    }
    
    [HttpPut("PriceRules/{id}")]
    public virtual async Task<IActionResult> PutProduct([FromRoute] long id,[FromBody] PriceRule priceRule)
    {
        try
        {
            var priceRules = await _priceRuleService.UpdateAsync(id, priceRule);
            return Ok(priceRules);
        }
        catch (InputException ex)
        {
            return StatusCode(400, new { message = ex.Message });
        }
        catch (ShopifyException ex)
        {
            return StatusCode(500, new { message = "Error fetching PriceRules", details = ex.Message });
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, new { message = "Error updating PriceRule " + ex.Message });
        }
    }
    
    [HttpDelete("PriceRules/{id}")]
    public async Task<IActionResult> DeletePriceRule([FromRoute]long id)
    {
        try
        {
            await _priceRuleService.DeleteAsync(id);
            return Ok("Price rule deleted");
        }
        catch (ShopifyException ex)
        {
            return StatusCode(404, new { message = "No price rule found", details = ex.Message });
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, new { message = "Error deleting price rules"});
        }
    }
    
    //================================ DISCOUNT CODES ==================================
    [HttpGet("Discounts/{priceRuleId}")] 
    public async Task<IActionResult> GetAllDiscountsByRule([FromRoute]long priceRuleId)
    {
        try
        {
            var discounts = await _discountCodeService.ListAsync(priceRuleId);
            
            return Ok(discounts);
        }
        catch (ShopifyException ex)
        {
            return StatusCode(404, new { message = "Error fetching discounts", details = ex.Message });
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, new { message = "Error fetching product", details = ex.Message });
        }
    }
    
    [HttpPost("Discounts/{priceRuleId}")]
    public virtual async Task<IActionResult> PostDiscount([FromRoute]long priceRuleId, [FromBody] PriceRuleDiscountCode request)
    {
        try
        {
            PriceRuleDiscountCode tempPriceRuleDiscountCode = request;
            //Product tempProduct = _productValidator.FormatPostProduct(product);
            var discountCode = await _discountCodeService.CreateAsync(priceRuleId, request);
            return Ok(discountCode);
        }
        catch (InputException ex)
        {
            return StatusCode(400, new { message = ex.Message });
        }
        catch (ShopifyException ex)
        {
            return StatusCode(500, new { message = "Error creating discounts", details = ex.Message });
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, new { message = "Error creating discount " + ex.Message });
        }
    }
    
    [HttpDelete("Discounts/{priceRuleId}/{discountId}")]
    public async Task<IActionResult> DeleteDiscount([FromRoute]long priceRuleId, [FromRoute] long discountId)
    {
        try
        {
            await _discountCodeService.DeleteAsync(priceRuleId, discountId);
            return Ok("Discount deleted");
        }
        catch (ShopifyException ex)
        {
            return StatusCode(404, new { message = "No discount found", details = ex.Message });
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, new { message = "Error deleting Discount" + ex.Message });
        }
    }


}