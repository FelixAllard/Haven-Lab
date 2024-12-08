namespace Api_Gateway.Services;

public class ServiceProductController
{
    private readonly HttpClient _httpClient;
    private readonly string BASE_URL = "http://localhost:5106";

    // Constructor that takes in HttpClient via Dependency Injection
    public ServiceProductController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Method to make the API call to Shopify and return the result
    public async Task<string> GetAllProductsAsync()
    {
        try
        {
            var requestUrl = $"{BASE_URL+"/api/Products"}"; // Shopify endpoint for products

            // Create the request message
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUrl)
            {
                
            };

            // Send the request and get the response
            var response = await _httpClient.SendAsync(requestMessage);

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