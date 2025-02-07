using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Api_Gateway.Services;

namespace Api_Gateway.Annotations
{
    public class RequireAuthAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private const string AUTH_HEADER_NAME = "Authorization";
        private const string BEARER_PREFIX = "Bearer ";

        public RequireAuthAttribute() { }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var httpContext = context.HttpContext;
            var serviceAuthController = httpContext.RequestServices.GetRequiredService<ServiceAuthController>();

            // Check if Authorization header exists
            if (!httpContext.Request.Headers.TryGetValue(AUTH_HEADER_NAME, out var authorizationHeader))
            {
                context.Result = new UnauthorizedObjectResult(new { Message = "Missing Authorization Header" });
                return;
            }

            Console.WriteLine($"Authorization Header: {authorizationHeader}");

            // Extract token (expecting "Bearer <token>")
            var token = authorizationHeader.ToString();
            
            if (token.StartsWith("Authorization: Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                // Remove "Authorization: " from the start and then check for "Bearer "
                token = token.Substring("Authorization: ".Length).Trim();
            }

            if (!token.StartsWith(BEARER_PREFIX))
            {
                context.Result = new UnauthorizedObjectResult(new { Message = "Invalid Authorization format. Expected 'Bearer <token>'" });
                return;
            }

            token = token.Substring(BEARER_PREFIX.Length); // Remove "Bearer " prefix

            Console.WriteLine($"Just Token: {token}");

            try
            {
                // Call VerifyTokenAsync to check the token validity
                var result = await serviceAuthController.VerifyTokenAsync(token);

                // Handle response
                switch (result.StatusCode)
                {
                    case HttpStatusCode.OK:
                        return; // Token is valid, continue request
                    case HttpStatusCode.Unauthorized:
                        // Handle specific errors as before
                        var errorMessage = await result.Content.ReadAsStringAsync();
                        if (errorMessage.Contains("Invalid token format"))
                        {
                            context.Result = new UnauthorizedObjectResult(new { Message = "Invalid token format" });
                        }
                        else if (errorMessage.Contains("Invalid token claims"))
                        {
                            context.Result = new UnauthorizedObjectResult(new { Message = "Invalid token claims" });
                        }
                        else if (errorMessage.Contains("User not found"))
                        {
                            context.Result = new UnauthorizedObjectResult(new { Message = "User not found" });
                        }
                        else if (errorMessage.Contains("Token mismatch"))
                        {
                            context.Result = new UnauthorizedObjectResult(new { Message = "Token mismatch" });
                        }
                        else if (errorMessage.Contains("Token has expired"))
                        {
                            context.Result = new UnauthorizedObjectResult(new { Message = "Token has expired" });
                        }
                        else
                        {
                            context.Result = new UnauthorizedObjectResult(new { Message = "Token is invalid or expired" });
                        }
                        return;
                    case HttpStatusCode.ServiceUnavailable:
                        context.Result = new StatusCodeResult(503);
                        return;
                    default:
                        Console.WriteLine($"HMMMM");
                        context.Result = new StatusCodeResult((int)result.StatusCode);
                        return;
                }
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"Error: Auth service unavailable - {httpEx.Message}");
                context.Result = new ObjectResult(new { Message = "Authentication service is currently unavailable" })
                {
                    StatusCode = (int)HttpStatusCode.ServiceUnavailable
                };
            }
            catch (TimeoutException timeoutEx) // Catch timeout issues
            {
                Console.WriteLine($"Error: Auth service timeout - {timeoutEx.Message}");
                context.Result = new ObjectResult(new { Message = "Authentication service timed out" })
                {
                    StatusCode = (int)HttpStatusCode.ServiceUnavailable
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: Unexpected error - {ex.Message}");
                context.Result = new ObjectResult(new { Message = $"Token validation failed: {ex.Message}" })
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError // 500 Internal Server Error
                };
            }
        } 
    }
}

