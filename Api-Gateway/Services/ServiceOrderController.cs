using System.Text;
using Api_Gateway.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ShopifySharp;

namespace Api_Gateway.Services;

public class ServiceOrderController
{
    private readonly IHttpClientFactory _httpClientFactory; // Use IHttpClientFactory instead of HttpClient directly
    private readonly string BASE_URL ; // Your base URL for Shopify API

    // Constructor that takes in IHttpClientFactory via Dependency Injection
    public ServiceOrderController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        BASE_URL = Environment.GetEnvironmentVariable("BASE_URL_SHOPIFY_API")??"http://localhost:5106";
    }

    // Method to make the API call to Shopify and return the result
    public virtual async Task<string> GetAllOrdersAsync(OrderSearchArgument searchArgument=null)
    {
        try
        {
            // Create the HttpClient instance using the factory
            var client = _httpClientFactory.CreateClient(); // Uses default HttpClient configuration

            var queryString = searchArgument != null ? OrderSearchArgument.BuildQueryString(searchArgument) : string.Empty;
            var requestUrl = $"{BASE_URL}/api/Order{queryString}"; // Shopify endpoint for orders

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
                return $"Error fetching orders: {response.ReasonPhrase}";
            }
        }
        catch (Exception ex)
        {
            // Return error details in case of an exception
            return $"Error: {ex.Message}";
        } 
    }
    
    public virtual async Task<string> GetOrderByIdAsync(long orderId)
    {
        try
        {
            // Create the HttpClient instance using the factory
            var client = _httpClientFactory.CreateClient(); // Uses default HttpClient configuration

            var requestUrl = $"{BASE_URL}/api/Order/{orderId}"; // Shopify endpoint for a specific order

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
                return $"Error fetching order with ID {orderId}: {response.ReasonPhrase}";
            }
        }
        catch (Exception ex)
        {
            // Return error details in case of an exception
            return $"Error: {ex.Message}";
        }
    }
    
    public virtual async Task<HttpResponseMessage> PutOrderAsync(long id, Order order)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var requestUrl = $"{BASE_URL}/api/Order/{id}"; 
            
            var jsonSettings = new Newtonsoft.Json.JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(), 
                Formatting = Formatting.None 
            };
            
            var jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(order, jsonSettings);
            
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            
            var response = await client.PutAsync(requestUrl, content);
            return response;
        }
        catch (System.Net.Http.HttpRequestException ex)
        {
            return new HttpResponseMessage(System.Net.HttpStatusCode.ServiceUnavailable)
            {
                Content = new StringContent($"Error: {ex.Message}")
            };
        }
        catch (Exception ex)
        {
            // Return a response with an exception message if something goes wrong
            return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError)
            {
                Content = new StringContent($"Error: {ex.Message}")
            };
        }
    }

}