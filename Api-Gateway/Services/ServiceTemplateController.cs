using System.Text;
using Api_Gateway.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Api_Gateway.Services;

public class ServiceTemplateController
{
    private readonly IHttpClientFactory _httpClientFactory; // Use IHttpClientFactory instead of HttpClient directly
    private readonly string BASE_URL; // Your base URL for Shopify API

    public ServiceTemplateController()
    {
        BASE_URL = Environment.GetEnvironmentVariable("BASE_URL_EMAIL_API")??"http://localhost:5092";
    }

    // Constructor that takes in IHttpClientFactory via Dependency Injection
    public ServiceTemplateController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        BASE_URL = Environment.GetEnvironmentVariable("BASE_URL_EMAIL_API")??"http://localhost:5092";
    }
    
    public virtual async Task<string> GetAllTemplateNames()
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var requestUrl = $"{BASE_URL}/api/Template/names";

            var response = await client.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                return $"Error fetching price rules: {response.ReasonPhrase}";
            }
        }
        catch (Exception ex)
        {
            return $"Exception: {ex.Message}";
        }
    }
    public virtual async Task<string> GetAllTemplate()
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var requestUrl = $"{BASE_URL}/api/Template";

            var response = await client.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                return $"Error fetching price rules: {response.ReasonPhrase}";
            }
        }
        catch (Exception ex)
        {
            return $"Exception: {ex.Message}";
        }
    }
    public virtual async Task<string> GetTemplateByName(string name)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var requestUrl = $"{BASE_URL}/api/Template/{name}";

            var response = await client.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                return $"Error fetching price rules: {response.ReasonPhrase}";
            }
        }
        catch (Exception ex)
        {
            return $"Exception: {ex.Message}";
        }
    }
    public virtual async Task<HttpResponseMessage> PostTemplate(Template template)
    {
        try
        {
            var client = _httpClientFactory.CreateClient(); // Uses default HttpClient configuration
            var requestUrl = $"{BASE_URL}/api/Template"; // Shopify endpoint for product creation

            // Serialize the product to JSON with camelCase using Newtonsoft.Json
            var jsonSettings = new Newtonsoft.Json.JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(), // Ensures camelCase for property names
                Formatting = Formatting.None // Optional: Compact JSON without extra spaces
            };

            // Serialize the product object to JSON with camelCase
            var jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(template, jsonSettings);
            

            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Send the POST request to Shopify API
            var response = await client.PostAsync(requestUrl, content);
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
    public virtual async Task<HttpResponseMessage> PutTemplate(string name,Template template)
    {
        try
        {
            var client = _httpClientFactory.CreateClient(); // Uses default HttpClient configuration
            var requestUrl = $"{BASE_URL}/api/Template/{name}"; // Shopify endpoint for product creation

            // Serialize the product to JSON with camelCase using Newtonsoft.Json
            var jsonSettings = new Newtonsoft.Json.JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(), // Ensures camelCase for property names
                Formatting = Formatting.None // Optional: Compact JSON without extra spaces
            };

            // Serialize the product object to JSON with camelCase
            var jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(template, jsonSettings);
            

            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Send the POST request to Shopify API
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
    public virtual async Task<string> DeleteTemplate(string name)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var requestUrl = $"{BASE_URL}/api/Template/{name}";

            var response = await client.DeleteAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                return $"Error fetching price rules: {response.ReasonPhrase}";
            }
        }
        catch (Exception ex)
        {
            return $"Exception: {ex.Message}";
        }
    }
    
}