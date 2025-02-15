using System.Net;
using Api_Gateway.Services;
using Microsoft.AspNetCore.Mvc;
using ShopifySharp;
using Api_Gateway.Annotations;

namespace Api_Gateway.Controller;

[Route("gateway/api/[controller]")]
[ApiController]
public class ProxyPromoController : ControllerBase
{
    private readonly ServicePromoController _servicePromoController;

    // Constructor injects the ServicePromoController
    public ProxyPromoController(ServicePromoController servicePromoController)
    {
        _servicePromoController = servicePromoController;
    }

    [HttpGet("PriceRules")]
    [RequireAuth]
    public async Task<IActionResult> GetAllPriceRules()
    {
        try
        {
            var result = await _servicePromoController.GetAllPriceRulesAsync();

            if (result.StartsWith("Error"))
            {
                return StatusCode(500, new { Message = result });
            }

            return Ok(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, new { Message = e.Message });
        }
    }

    [HttpGet("PriceRules/{id}")]
    [RequireAuth]
    public async Task<IActionResult> GetPriceRuleById([FromRoute] long id)
    {
        try
        {
            var result = await _servicePromoController.GetPriceRuleByIdAsync(id);

            if (result.Contains("404 Not Found"))
            {
                return NotFound(new { message = result });
            }

            if (result.StartsWith("Error"))
            {
                return StatusCode(500, new { Message = result });
            }

            return Ok(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, new { Message = e.Message });
        }
    }
    
    [HttpPost("PriceRules")]
    [RequireAuth]
    public async Task<IActionResult> PostPriceRule([FromBody] PriceRule priceRule)
    {
        try
        {
            var response = await _servicePromoController.CreatePriceRuleAsync(priceRule);
            if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
            {
                return StatusCode(503, new { message = "Service is currently unavailable, please try again later." });
            }

            var content = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, content);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred", details = ex.Message });
        }
    }
    
    [HttpPut("PriceRules/{id}")]
    [RequireAuth]
    public async Task<IActionResult> PutPriceRule([FromRoute]long id, [FromBody] PriceRule priceRule)
    {
        try
        {
            var response = await _servicePromoController.PutPriceRuleAsync(id, priceRule);
            if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
            {
                return StatusCode(503, new { message = "Service is currently unavailable, please try again later." });
            }

            var content = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, content);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred", details = ex.Message });
        }
    }

    [HttpDelete("PriceRules/{id}")]
    [RequireAuth]
    public async Task<IActionResult> DeletePriceRule([FromRoute] long id)
    {
        try
        {
            var result = await _servicePromoController.DeletePriceRuleAsync(id);

            if (result.Contains("404 Not Found"))
            {
                return NotFound(new { message = result });
            }

            if (result.StartsWith("Error"))
            {
                return StatusCode(500, new { Message = result });
            }

            return Ok(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, new { Message = e.Message });
        }
    }

    [HttpGet("Discounts/{priceRuleId}")]
    [RequireAuth]
    public async Task<IActionResult> GetAllDiscountsByRule([FromRoute] long priceRuleId)
    {
        try
        {
            var result = await _servicePromoController.GetAllDiscountsByRuleAsync(priceRuleId);

            if (result.StartsWith("Error"))
            {
                return StatusCode(500, new { Message = result });
            }

            return Ok(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, new { Message = e.Message });
        }
    }

    [HttpPost("Discounts/{priceRuleId}")]
    [RequireAuth]
    public async Task<IActionResult> CreateDiscount([FromRoute] long priceRuleId, [FromBody] PriceRuleDiscountCode discountCode)
    {
        try
        {
            var response = await _servicePromoController.CreateDiscountAsync(priceRuleId, discountCode);

            if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                return StatusCode(503, new { message = "Service is currently unavailable, please try again later." });
            }

            var content = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, content);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred", details = ex.Message });
        }
    }

    [HttpDelete("Discounts/{priceRuleId}/{discountId}")]
    [RequireAuth]
    public async Task<IActionResult> DeleteDiscount([FromRoute] long priceRuleId, [FromRoute] long discountId)
    {
        try
        {
            var result = await _servicePromoController.DeleteDiscountAsync(priceRuleId, discountId);

            if (result.Contains("404 Not Found"))
            {
                return NotFound(new { message = result });
            }

            if (result.StartsWith("Error"))
            {
                return StatusCode(500, new { Message = result });
            }

            return Ok(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, new { Message = e.Message });
        }
    }
}
