using System.Net;
using System.Net.Sockets;
using System.Text;
using Api_Gateway.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ShopifySharp;

namespace Api_Gateway.Services;

public class ServiceProductController
{
    private readonly IHttpClientFactory _httpClientFactory; // Use IHttpClientFactory instead of HttpClient directly
    private readonly string BASE_URL; // Your base URL for Shopify API

    // Constructor that takes in IHttpClientFactory via Dependency Injection
    public ServiceProductController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        BASE_URL = Environment.GetEnvironmentVariable("BASE_URL_SHOPIFY_API")??"http://localhost:5106";
    }

    // Method to make the API call to Shopify and return the result
    public virtual async Task<string> GetAllProductsAsync(SearchArguments searchArguments = null)
    {
        try
        {
            // Create the HttpClient instance using the factory
            var client = _httpClientFactory.CreateClient(); // Uses default HttpClient configuration

            // Build the request URL with optional query parameters
            var queryString = searchArguments != null ? SearchArguments.BuildQueryString(searchArguments) : string.Empty;
            var requestUrl = $"{BASE_URL}/api/Products{queryString}";

            // Create the request message
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUrl);

            // Send the request and get the response
            var response = await client.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                // Read and return the response content as string
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                // If the API call fails, return an error message
                return $"Error fetching products: {response.ReasonPhrase}";
            }
        }
        catch (Exception ex)
        {
            // Return error details in case of an exception
            return $"Exception: {ex.Message}";
        }
    }

    public virtual async Task<string> GetProductByIdAsync(long id)
    {
        try
        {
            // Create the HttpClient instance using the factory
            var client = _httpClientFactory.CreateClient(); // Uses default HttpClient configuration

            var requestUrl = $"{BASE_URL}/api/Products/{id}"; // Shopify endpoint for fetching a single product by ID

            // Create the request message
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUrl);

            // Send the request and get the response
            var response = await client.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                // Read and return the response content as string
                return await response.Content.ReadAsStringAsync();
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return "404 Not Found: Product not found";
            }
            else
            {
                // If the API call fails, return an error message
                return $"Error fetching product by ID: {response.ReasonPhrase}";
            }
        }
        catch (Exception ex)
        {
            // Return error details in case of an exception
            return $"Exception: {ex.Message}";
        }
    }

    public virtual async Task<HttpResponseMessage> CreateProductAsync(Product product)
    {
        try
        {
            var client = _httpClientFactory.CreateClient(); // Uses default HttpClient configuration
            var requestUrl = $"{BASE_URL}/api/Products"; // Shopify endpoint for product creation

            // Serialize the product to JSON with camelCase using Newtonsoft.Json
            var jsonSettings = new Newtonsoft.Json.JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(), // Ensures camelCase for property names
                Formatting = Formatting.None // Optional: Compact JSON without extra spaces
            };

            // Serialize the product object to JSON with camelCase
            var jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(product, jsonSettings);
            

            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Send the POST request to Shopify API
            var response = await client.PostAsync(requestUrl, content);
            return response;
        }
        catch (System.Net.Http.HttpRequestException ex)
        {
            return new HttpResponseMessage(System.Net.HttpStatusCode.ServiceUnavailable)
            {
                Content = new StringContent($"Exception: {ex.Message}")
            };
        }
        catch (Exception ex)
        {
            // Return a response with an exception message if something goes wrong
            return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError)
            {
                Content = new StringContent($"Exception: {ex.Message}")
            };
        }
    }

    
    public virtual async Task<HttpResponseMessage> PutProductAsync(long id, Product product)
    {
        try
        {
            var client = _httpClientFactory.CreateClient(); // Uses default HttpClient configuration
            var requestUrl = $"{BASE_URL}/api/Products/{id}"; // Shopify endpoint for product creation

            // Serialize the product to JSON with camelCase using Newtonsoft.Json
            var jsonSettings = new Newtonsoft.Json.JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(), // Ensures camelCase for property names
                Formatting = Formatting.None // Optional: Compact JSON without extra spaces
            };

            // Serialize the product object to JSON with camelCase
            var jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(product, jsonSettings);
            

            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Send the POST request to Shopify API
            var response = await client.PutAsync(requestUrl, content);
            return response;
        }
        catch (System.Net.Http.HttpRequestException ex)
        {
            return new HttpResponseMessage(System.Net.HttpStatusCode.ServiceUnavailable)
            {
                Content = new StringContent($"Exception: {ex.Message}")
            };
        }
        catch (Exception ex)
        {
            // Return a response with an exception message if something goes wrong
            return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError)
            {
                Content = new StringContent($"Exception: {ex.Message}")
            };
        }
    }
    
    
    public virtual async Task<string> DeleteProductAsync(long id)
    {
        try
        {
            // Create the HttpClient instance using the factory
            var client = _httpClientFactory.CreateClient(); // Uses default HttpClient configuration

            var requestUrl = $"{BASE_URL}/api/Products/{id}"; // Shopify endpoint for fetching a single product by ID

            // Create the request message
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, requestUrl);

            // Send the request and get the response
            var response = await client.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                // Read and return the response content as string
                return await response.Content.ReadAsStringAsync();
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return "404 Not Found: Product not found";
            }
            else
            {
                // If the API call fails, return an error message
                return $"Error deleting product by ID: {response.ReasonPhrase}";
            }
        }
        catch (Exception ex)
        {
            // Return error details in case of an exception
            return $"Exception: {ex.Message}";
        }
    }
}