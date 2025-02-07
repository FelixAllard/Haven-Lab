using System.Net;
using System.Text;
using Api_Gateway.Models;
using Newtonsoft.Json;

namespace Api_Gateway.Services;

public class ServiceAuthController
{
    private readonly IHttpClientFactory _httpClientFactory; 
    private readonly string BASE_URL; 

    public ServiceAuthController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        BASE_URL = Environment.GetEnvironmentVariable("BASE_URL_APIWEBAUTH_API") ?? "http://localhost:5113";
    }
    
    // Login method
    public virtual async Task<HttpResponseMessage> LoginAsync(Login model)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var requestUrl = $"{BASE_URL}/api/Account/login";
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUrl) { Content = content };

            var response = await client.SendAsync(requestMessage);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("Error 401: Unauthorized - Invalid credentials.")
                };
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return new HttpResponseMessage(response.StatusCode)
                {
                    Content = new StringContent($"Error {response.StatusCode}: {errorContent}")
                };
            }

            return await HandleResponse(response);
        }
        catch (HttpRequestException httpEx)
        {
            return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
            {
                Content = new StringContent($"Error 503: Service Unavailable - {httpEx.Message}")
            };
        }
        catch (TaskCanceledException timeoutEx)
        {
            return new HttpResponseMessage(HttpStatusCode.RequestTimeout)
            {
                Content = new StringContent($"Error 408: Request Timeout - {timeoutEx.Message}")
            };
        }
        catch (Exception ex)
        {
            return new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent($"Error 500: Internal Server Error - {ex.Message}")
            };
        }
    }



    // Logout method
    public virtual async Task<HttpResponseMessage> LogoutAsync(string username)
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
            return new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent($"Error 500: Internal Server Error - {ex.Message}")
            };
        }
    }

    // Verify Token method
    public virtual async Task<HttpResponseMessage> VerifyTokenAsync(string token)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var requestUrl = $"{BASE_URL}/api/Account/verify-token";
            var content = new StringContent(JsonConvert.SerializeObject(token), Encoding.UTF8, "application/json");
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUrl) { Content = content };

            var response = await client.SendAsync(requestMessage);
            return await HandleResponse(response);
        }
        catch (HttpRequestException httpEx)
        {
            return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
            {
                Content = new StringContent($"Error 503: Service Unavailable - {httpEx.Message}")
            };
        }
        catch (TimeoutException timeoutEx)
        {
            return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
            {
                Content = new StringContent($"Error 503: Service Unavailable - Timeout occurred: {timeoutEx.Message}")
            };
        }
        catch (Exception ex)
        {
            return new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent($"Error 500: Internal Server Error - {ex.Message}")
            };
        }
    }

    // Helper method to handle API responses and status codes
    private async Task<HttpResponseMessage> HandleResponse(HttpResponseMessage response)
    {
        // Check for successful response
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(content)
            };
        }

        // Handle different HTTP status codes
        switch (response.StatusCode)
        {
            case HttpStatusCode.Unauthorized:
                return new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("Unauthorized: Invalid credentials or token")
                };

            case HttpStatusCode.NotFound:
                return new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("404 Not Found: Endpoint not found")
                };

            case HttpStatusCode.ServiceUnavailable:
                return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                {
                    Content = new StringContent($"Error 503: Service Unavailable - {response.ReasonPhrase}")
                };

            default:
                // Return a generic error message with the reason phrase from the response
                return new HttpResponseMessage(response.StatusCode)
                {
                    Content = new StringContent($"Error: {response.ReasonPhrase}")
                };
        }
    }
}
