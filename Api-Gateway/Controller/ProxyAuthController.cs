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

            // Ensure result is an ObjectResult to access StatusCode and Value
            if (result is ObjectResult objectResult)
            {
                switch (objectResult.StatusCode)
                {
                    case 401:
                        return Unauthorized(new { Message = "Invalid credentials. Please try again." });

                    case 404:
                        return NotFound(new { Message = "404 Not Found: Endpoint not found" });

                    case 503:
                        return StatusCode(503, new { Message = "Error 503: Service Unavailable" });

                    case 200:
                        // Ensure the response contains valid JSON data
                        if (objectResult.Value is string jsonString)
                        {
                            try
                            {
                                var tokenResponse = JsonConvert.DeserializeObject<JObject>(jsonString);
                                var token = tokenResponse?["token"]?.ToString(); // Extract the token value

                                if (!string.IsNullOrEmpty(token))
                                {
                                    return Ok(new { Token = token });
                                }
                            }
                            catch (JsonException)
                            {
                                return StatusCode(500, new { Message = "Invalid response format from authentication service." });
                            }
                        }
                        return StatusCode(500, new { Message = "Unexpected response from authentication service." });

                    default:
                        return StatusCode(objectResult.StatusCode ?? 500, 
                            new { Message = objectResult.Value?.ToString() ?? "Unknown error occurred." });
                }
            }

            // If result is not an ObjectResult, return a generic 500 error
            return StatusCode(500, new { Message = "Unexpected response from authentication service." });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex); // Log the exception
            return StatusCode(500, new { Message = "Internal server error", Details = ex.Message });
        }
    }


    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] string username)
    {
        try
        {
            var result = await _serviceAuthController.LogoutAsync(username);

            // Ensure result is an ObjectResult to access StatusCode
            if (result is ObjectResult objectResult)
            {
                switch (objectResult.StatusCode)
                {
                    case 200:
                        return Ok(new { Message = "Logged out successfully." });

                    case 401:
                        return Unauthorized(new { Message = "Unauthorized to perform logout." });

                    case 404:
                        return NotFound(new { Message = "User not found." });

                    default:
                        return StatusCode(objectResult.StatusCode ?? 500, 
                            new { Message = objectResult.Value?.ToString() ?? "Unknown error occurred." });
                }
            }

            // If result is not an ObjectResult, return a generic 500 error
            return StatusCode(500, new { Message = "Unexpected response from logout service." });
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
            
            if (result is ObjectResult objectResult)
            {
                switch (objectResult.StatusCode)
                {
                    case 200:
                        return Ok(new { Message = "Token is valid." });

                    case 401:
                        return Unauthorized(new { Message = "Token is invalid or expired." });

                    case 404:
                        return NotFound(new { Message = "Token verification service not found." });

                    default:
                        return StatusCode(objectResult.StatusCode ?? 500, 
                            new { Message = objectResult.Value?.ToString() ?? "Unknown error occurred." });
                }
            }
            
            return StatusCode(500, new { Message = "Unexpected response from token verification service." });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return StatusCode(500, new { Message = "Internal server error", Details = ex.Message });
        }
    }

}
