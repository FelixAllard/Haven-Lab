namespace Api_Gateway.Controller;
using Api_Gateway.Services;
using Microsoft.AspNetCore.Mvc;
using ShopifySharp;

[Route("gateway/api/[controller]")]
[ApiController]
public class ProxyCustomerController : ControllerBase
{
    private readonly ServiceCustomerController _serviceCustomerController;

    // Constructor injects the ShopifyApiService
    public ProxyCustomerController(ServiceCustomerController shopifyApiService)
    {
        _serviceCustomerController = shopifyApiService;
    }
    
    [HttpPost("subscribe")]
    public async Task<IActionResult> Subscribe([FromBody] string email)
    {
        try
        {
            // Call the service method to subscribe the email
            var result = await _serviceCustomerController.Subscribe(email);

            // Check if the result indicates an error
            if (result.StartsWith("Error") || result.StartsWith("Exception"))
            {
                // Return 500 Internal Server Error with the error message
                return StatusCode(500, new { Message = result });
            }

            // Return 200 OK with the result
            return Ok(new { Message = result }); // Result is serialized as JSON automatically
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            // Return 500 Internal Server Error with the exception message
            return StatusCode(500, new { Message = e.Message });
        }
    }
    
    [HttpPost("unsubscribe")]
    public async Task<IActionResult> Unsubscribe([FromBody] string email)
    {
        try
        {
            // Call the service method to subscribe the email
            var result = await _serviceCustomerController.Unsubscribe(email);

            // Check if the result indicates an error
            if (result.StartsWith("Error") || result.StartsWith("Exception"))
            {
                // Return 500 Internal Server Error with the error message
                return StatusCode(500, new { Message = result });
            }

            // Return 200 OK with the result
            return Ok(new { Message = result }); // Result is serialized as JSON automatically
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            // Return 500 Internal Server Error with the exception message
            return StatusCode(500, new { Message = e.Message });
        }
    }

}