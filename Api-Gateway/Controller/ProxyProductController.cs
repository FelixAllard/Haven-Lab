using Api_Gateway.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api_Gateway.Controller;
[Route("gateway/api/[controller]")]
[ApiController]
public class ProxyProductController : ControllerBase
{
    private readonly ServiceProductController _serviceProductController;

    // Constructor injects the ShopifyApiService
    public ProxyProductController(ServiceProductController shopifyApiService)
    {
        _serviceProductController = shopifyApiService;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllProducts()
    {
        // Call the service to get products
        var result = await _serviceProductController.GetAllProductsAsync();

        // Return the result from the Shopify API
        return Content(result, "application/json");
    }
    
}