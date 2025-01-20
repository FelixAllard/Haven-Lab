using Microsoft.AspNetCore.Mvc;

namespace Api_Gateway.Services;

using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ShopifySharp;

public class ServiceCustomerController
{
    private readonly IHttpClientFactory _httpClientFactory; // Use IHttpClientFactory instead of HttpClient directly
    private readonly string BASE_URL ; // Your base URL for Shopify API

    // Constructor that takes in IHttpClientFactory via Dependency Injection
    public ServiceCustomerController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        BASE_URL = Environment.GetEnvironmentVariable("BASE_URL_SHOPIFY_API")??"http://localhost:5106";
    }

    public virtual async Task<string> Subscribe([FromBody] String email)
    {
        try
        {
            var client = _httpClientFactory.CreateClient(); // Uses default HttpClient configuration
            var requestUrl = $"{BASE_URL}/api/Customer/subscribe"; // Shopify endpoint for customer subscription

            // Serialize the email to JSON
            var jsonSettings = new Newtonsoft.Json.JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(), // Ensures camelCase for property names
                Formatting = Formatting.None
            };

            var jsonContent =
                Newtonsoft.Json.JsonConvert.SerializeObject(email, jsonSettings); // Wrap email in an object
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Send the POST request to Shopify API
            var response = await client.PostAsync(requestUrl, content);

            if (response.IsSuccessStatusCode)
            {
                // Read and return the response content as a string
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                // Return an error message if the response indicates failure
                Console.WriteLine($"Request URL: {requestUrl}");
                return $"Error: {response.StatusCode}, {response.ReasonPhrase}";

            }
        }
        catch (HttpRequestException ex)
        {
            // Return a message for HttpRequestException
            return $"Service Unavailable: {ex.Message}";
        }
        catch (Exception ex)
        {
            // Return a generic error message
            return $"Exception: {ex.Message}";
        }
    }


    public virtual async Task<string> Unsubscribe([FromBody] String email)
    {
        try
        {
            var client = _httpClientFactory.CreateClient(); // Uses default HttpClient configuration
            var requestUrl = $"{BASE_URL}/api/Customer/unsubscribe"; // Shopify endpoint for customer subscription

            // Serialize the email to JSON
            var jsonSettings = new Newtonsoft.Json.JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(), // Ensures camelCase for property names
                Formatting = Formatting.None 
            };

            var jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject( email , jsonSettings); // Wrap email in an object
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Send the POST request to Shopify API
            var response = await client.PostAsync(requestUrl, content);

            if (response.IsSuccessStatusCode)
            {
                // Read and return the response content as a string
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                // Return an error message if the response indicates failure
                Console.WriteLine($"Request URL: {requestUrl}");
                return $"Error: {response.StatusCode}, {response.ReasonPhrase}";

            }
        }
        catch (HttpRequestException ex)
        {
            // Return a message for HttpRequestException
            return $"Service Unavailable: {ex.Message}";
        }
        catch (Exception ex)
        {
            // Return a generic error message
            return $"Exception: {ex.Message}";
        }
    }
}