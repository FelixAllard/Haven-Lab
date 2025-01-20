using System.Text;
using Api_Gateway.Models;
using Newtonsoft.Json;

namespace Api_Gateway.Services;

public class ServiceEmailApiController
{
     private readonly IHttpClientFactory _httpClientFactory; // Use IHttpClientFactory instead of HttpClient directly
    private readonly string BASE_URL; // Your base URL for Shopify API

    // Constructor that takes in IHttpClientFactory via Dependency Injection
    public ServiceEmailApiController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        BASE_URL = Environment.GetEnvironmentVariable("BASE_URL_EMAIL_API")??"http://localhost:5092";
    }

    // Method to make the API call to Shopify and return the result
    public virtual async Task<(int StatusCode, string Content)> PostDraftOrder(DirectEmailModel directEmailModel)
    {
        try
        {
            // Create the HttpClient instance using the factory
            var client = _httpClientFactory.CreateClient(); // Uses default HttpClient configuration

            var requestUrl = $"{BASE_URL}/api/Email/sendwithformat"; // Shopify endpoint for orders

            // Serialize the draftOrder to JSON and set the request content
            var content = new StringContent(JsonConvert.SerializeObject(directEmailModel), Encoding.UTF8, "application/json");

            // Create the request message
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUrl)
            {
                Content = content
            };

            // Send the request and get the response
            var response = await client.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                // Return the response content and the 200 status code
                return ((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }
            else
            {
                // Return the error status code and error message
                return ((int)response.StatusCode, $"Error: {response.ReasonPhrase}  {response.Content}");
            }
        }
        catch (Exception ex)
        {
            // Return 503 Service Unavailable if an exception occurs
            return (503, $"Error: {ex.Message}");
        }
    }
}