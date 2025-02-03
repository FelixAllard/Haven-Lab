using Api_Gateway.Models;
using Api_Gateway.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            var result = await _serviceAuthController.LoginAsync(model);
            
            if (result.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Unauthorized(new { Message = "Invalid credentials. Please try again." });
            }
            if (result.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound(new { Message = "404 Not Found: Endpoint not found" });
            }
            if (result.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                return StatusCode(503, new { Message = "Error 503: Service Unavailable" });
            }

            // Read the raw content from the response
            var responseContent = await result.Content.ReadAsStringAsync();

            // Try to parse the response as a JSON object and get the token from it
            var tokenResponse = JsonConvert.DeserializeObject<JObject>(responseContent);
            var token = tokenResponse["token"]?.ToString(); // Extract the token value

            if (!string.IsNullOrEmpty(token))
            {
                // Return the token directly
                return Ok(new { Token = token });
            }
            else
            {
                return StatusCode(500, new { Message = "Internal server error, status code: " + result.StatusCode });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex); // Log the exception
            return StatusCode(500, new { Message = "Internal server error", Details = ex.Message });
        }
    }


    // Logout endpoint
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] string username)
    {
        try
        {
            var result = await _serviceAuthController.LogoutAsync(username);

            // Use switch to handle the response based on status code
            switch (result.StatusCode)
            {
                case HttpStatusCode.OK:
                    return Ok(new { Message = "Logged out successfully." });

                case HttpStatusCode.Unauthorized:
                    return Unauthorized(new { Message = "Unauthorized to perform logout." });

                case HttpStatusCode.NotFound:
                    return NotFound(new { Message = "User not found." });

                default:
                    return StatusCode((int)result.StatusCode, new { Message = result.ReasonPhrase });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex); // Log the exception
            return StatusCode(500, new { Message = "Internal server error", Details = ex.Message });
        }
    }

    // Verify Token endpoint
    [HttpPost("verify-token")]
    public async Task<IActionResult> VerifyToken([FromBody] string token)
    {
        try
        {
            var result = await _serviceAuthController.VerifyTokenAsync(token);

            // Use switch to handle the response based on status code
            switch (result.StatusCode)
            {
                case HttpStatusCode.OK:
                    return Ok(new { Message = "Token is valid." });

                case HttpStatusCode.Unauthorized:
                    return Unauthorized(new { Message = "Token is invalid or expired." });

                case HttpStatusCode.NotFound:
                    return NotFound(new { Message = "Token verification service not found." });

                default:
                    return StatusCode((int)result.StatusCode, new { Message = result.ReasonPhrase });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex); // Log the exception
            return StatusCode(500, new { Message = "Internal server error", Details = ex.Message });
        }
    }
}
