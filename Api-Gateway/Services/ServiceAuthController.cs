using System.Net;
using System.Net.Sockets;
using System.Text;
using Api_Gateway.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ShopifySharp;

namespace Api_Gateway.Services;

public class ServiceAuthController
{
    private readonly IHttpClientFactory _httpClientFactory; // Use IHttpClientFactory instead of HttpClient directly
    private readonly string BASE_URL ; // Your base URL for Shopify API

    // Constructor that takes in IHttpClientFactory via Dependency Injection
    public ServiceAuthController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        BASE_URL = Environment.GetEnvironmentVariable("BASE_URL_APIWEBAUTH_API")??"http://localhost:5113";
    }
    
    public virtual async Task<string> LoginAsync(Login model)
    {
        try
        {
            // Create the HttpClient instance using the factory
            var client = _httpClientFactory.CreateClient();

            // Define the request URL for the remote login endpoint
            var requestUrl = $"{BASE_URL}/api/Account/login";

            // Serialize the login model into JSON
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            // Create the request message
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUrl)
            {
                Content = content
            };

            // Send the request and get the response
            var response = await client.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                // Read and return the response content as a string
                return await response.Content.ReadAsStringAsync();
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return "Unauthorized: Invalid credentials";
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return "404 Not Found: Login endpoint not found";
            }
            else if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                // If the API call fails, return an error message
                return $"Error 503: Service Unavailable - {response.ReasonPhrase}";
            }
            else
            {
                // If the API call fails, return an error message
                return $"Error logging in: {response.ReasonPhrase}";
            }
        }
        catch (Exception ex)
        {
            // Return error details in case of an exception
            return $"Error 500: Internal Server Error - {ex.Message}";
        }
    }


}