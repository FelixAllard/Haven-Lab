using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
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

        try
        {
            // Call VerifyTokenAsync to check the token validity
            var result = await serviceAuthController.VerifyTokenAsync(token);

            // Cast result to ObjectResult to access StatusCode
            if (result is ObjectResult objectResult)
            {
                switch (objectResult.StatusCode)
                {
                    case 200:
                        return;

                    case 401:
                        context.Result = new UnauthorizedObjectResult(new { Message = "Unauthorized" });
                        return;

                    case 503:
                        context.Result = new StatusCodeResult(503);
                        return;

                    default:
                        context.Result = new StatusCodeResult(objectResult.StatusCode ?? 500);
                        return;
                }
            }
            else
            {
                context.Result = new StatusCodeResult(500);
            }
        }
        catch (HttpRequestException httpEx)
        {
            Console.WriteLine($"Error: Auth service unavailable - {httpEx.Message}");
            context.Result = new ObjectResult(new { Message = "Authentication service is currently unavailable" })
            {
                StatusCode = 503
            };
        }
        catch (TimeoutException timeoutEx) // Catch timeout issues
        {
            Console.WriteLine($"Error: Auth service timeout - {timeoutEx.Message}");
            context.Result = new ObjectResult(new { Message = "Authentication service timed out" })
            {
                StatusCode = 503
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: Unexpected error - {ex.Message}");
            context.Result = new ObjectResult(new { Message = $"Token validation failed: {ex.Message}" })
            {
                StatusCode = 500
            };
        }
    }
    }
}

