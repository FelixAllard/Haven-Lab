namespace Api_Gateway.Services;

public class ServiceProductController
{
    private readonly IHttpClientFactory _httpClientFactory; // Use IHttpClientFactory instead of HttpClient directly
    private readonly string BASE_URL = "http://localhost:5106"; // Your base URL for Shopify API

    // Constructor that takes in IHttpClientFactory via Dependency Injection
    public ServiceProductController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    // Method to make the API call to Shopify and return the result
    public virtual async Task<string> GetAllProductsAsync()
    {
        try
        {
            // Create the HttpClient instance using the factory
            var client = _httpClientFactory.CreateClient(); // Uses default HttpClient configuration

            var requestUrl = $"{BASE_URL}/api/Products"; // Shopify endpoint for products

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
}