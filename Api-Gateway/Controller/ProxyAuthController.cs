using ApiWebAuth.Models;
using Api_Gateway.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api_Gateway.Controller;
[Route("gateway/api/[controller]")]
[ApiController]
public class ProxyAuthController : ControllerBase
{
    private readonly ServiceAuthController _serviceAuthController;
    
    public ProxyAuthController(ServiceAuthController serviceAuthController)
    {
        _serviceAuthController = serviceAuthController;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] Login model)
    {
        try
        {
            // Call the LoginAsync method in ServiceAuthController
            var result = await _serviceAuthController.LoginAsync(model);

            if (result.StartsWith("404")) // Endpoint not found or similar issues
            {
                return NotFound(new { Message = result });
            }
            else if (result.StartsWith("Error")) // Handle general errors
            {
                return BadRequest(new { Message = result });
            }
            else if (result.StartsWith("Unauthorized")) // Handle invalid credentials
            {
                return Unauthorized(new { Message = "Invalid username or password" });
            }

            // Return 200 OK with the result as JSON (assuming it's a token)
            return Ok(new { Token = result });
        }
        catch (Exception e)
        {
            // Log the exception to the console
            Console.WriteLine(e);
            // Return 500 Internal Server Error with exception details
            return StatusCode(500, new { Message = e.Message });
        }
    }


}