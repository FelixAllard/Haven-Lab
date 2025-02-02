using Api_Gateway.Models;
using Api_Gateway.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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

    // Login endpoint
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] Login model)
    {
        try
        {
            var result = await _serviceAuthController.LoginAsync(model);

            // Use switch to handle the response based on status code
            switch (result.StatusCode)
            {
                case HttpStatusCode.OK:
                    return Ok(new { Token = await result.Content.ReadAsStringAsync() });

                case HttpStatusCode.Unauthorized:
                    return Unauthorized(new { Message = "Invalid username or password" });

                case HttpStatusCode.NotFound:
                    return NotFound(new { Message = "Login endpoint not found." });

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
