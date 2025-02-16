using System.Net;
using System.Text;
using Api_Gateway.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Api_Gateway.Services;

public class ServiceAuthController
{
    private readonly IHttpClientFactory _httpClientFactory; 
    private readonly string BASE_URL; 

    public ServiceAuthController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        BASE_URL = Environment.GetEnvironmentVariable("BASE_URL_APIWEBAUTH_API") ?? Environment.GetEnvironmentVariable("ENV_BASE_URL_APIWEBAUTH_API") ??"http://localhost:5113";
    }
    
public virtual async Task<IActionResult> LoginAsync(Login model)
{
    try
    {
        var client = _httpClientFactory.CreateClient();
        var requestUrl = $"{BASE_URL}/api/Account/login";
        var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUrl) { Content = content };

        var response = await client.SendAsync(requestMessage);

        if (!response.IsSuccessStatusCode) // Check for unsuccessful status codes here 
        {
            string errorMessage;

            switch ((int)response.StatusCode)
            {
                case 401:
                    errorMessage = "Unauthorized: Invalid credentials";
                    break;

                case 503:
                    errorMessage = $"Error 503: Service Unavailable - {response.ReasonPhrase}";
                    break;

                default:
                    errorMessage = $"{(int)response.StatusCode}: {response.ReasonPhrase}";
                    break;
            }

            return new BadRequestObjectResult(new { Error = errorMessage }); // Return a 400 with an object containing the error message
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        return new OkObjectResult(responseContent);   // returns a OK status with the string content of the HTTP response
    }
    catch (TaskCanceledException ex)
    {
        // Handle request timeout specifically
        Console.WriteLine("Error: " + ex.Message);  // Log error message to console
        return new ObjectResult($"Error 408: Request Timeout - {ex.Message}")
        {
            StatusCode = (int)HttpStatusCode.RequestTimeout
        };
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error: " + ex.Message);  // Log error message to console
        return new ObjectResult($"Error: {ex.Message} - Internal Server Error")
        {
            StatusCode = (int)HttpStatusCode.InternalServerError
        };
    }
}

    // Logout method
    public virtual async Task<IActionResult> LogoutAsync(string username)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var requestUrl = $"{BASE_URL}/api/Account/logout";
            var content = new StringContent(JsonConvert.SerializeObject(username), Encoding.UTF8, "application/json");
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUrl) { Content = content };

            var response = await client.SendAsync(requestMessage);
            return await HandleResponse(response);
        }
        catch (Exception ex)
        {
            return new ObjectResult($"Error: {ex.Message} - Internal Server Error") 
                { StatusCode = (int)HttpStatusCode.InternalServerError };
        }
    }

    // Verify Token method
    public virtual async Task<IActionResult> VerifyTokenAsync(string token)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var requestUrl = $"{BASE_URL}/api/Account/verify-token";
            var content = new StringContent(JsonConvert.SerializeObject(token), Encoding.UTF8, "application/json");
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUrl) { Content = content };

            var response = await client.SendAsync(requestMessage);

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    return new UnauthorizedResult();

                else if (response.StatusCode == HttpStatusCode.NotFound)
                    return new NotFoundObjectResult("Endpoint not found");

                else if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
                    return new ObjectResult($"Error {(int)HttpStatusCode.ServiceUnavailable}: Service Unavailable - Timeout occurred")
                        { StatusCode = (int)HttpStatusCode.ServiceUnavailable };
                
                return new ObjectResult($"Error: {(int)response.StatusCode} - Internal Server Error") 
                    { StatusCode = (int)HttpStatusCode.InternalServerError };
            }
            else
                return new OkObjectResult(new { });
        }
        catch (Exception ex)
        {
            return new ObjectResult($"Error: {ex.Message} - Internal Server Error") 
                { StatusCode = (int)HttpStatusCode.InternalServerError };
        }
    }


    private async Task<IActionResult> HandleResponse(HttpResponseMessage response)
    {
        // Check for successful response
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return new OkObjectResult(content);  // returns a OK status with the string content of the HTTP response
        }
    
        // Handle different HTTP status codes
        switch (response.StatusCode)
        {
            case HttpStatusCode.Unauthorized:
                return new UnauthorizedObjectResult("Unauthorized: Invalid credentials or token");  // returns an Unauthorized result with a message
            
            case HttpStatusCode.NotFound:
                return new NotFoundObjectResult("404 Not Found: Endpoint not found");  // returns a NotFound status with a message
          
            case HttpStatusCode.ServiceUnavailable:
                return new ObjectResult($"Error 503: Service Unavailable - {response.ReasonPhrase}"){ StatusCode = (int)HttpStatusCode.ServiceUnavailable }; // returns an object result with a status code and the message
        
            default:
                // Return a generic error message with the reason phrase from the response
                return new ObjectResult($"Error: {response.ReasonPhrase}"){ StatusCode = (int)response.StatusCode };  // returns an object result with a status code and the message
        }
    }
}
