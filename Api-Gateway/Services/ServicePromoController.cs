namespace Api_Gateway.Services;

public class ServicePromoController
{
    private readonly IHttpClientFactory _httpClientFactory; 
    private readonly string BASE_URL; 

    public ServicePromoController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        BASE_URL = Environment.GetEnvironmentVariable("BASE_URL_SHOPIFY_API")??"http://localhost:5106";
    }
    
    public virtual async Task<string> GetAllPriceRulesAsync()
    {
        try
        {
            // Create the HttpClient instance using the factory
            var client = _httpClientFactory.CreateClient(); // Uses default HttpClient configuration

            var requestUrl = $"{BASE_URL}/api/pricerules"; // Shopify endpoint for orders

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
            return $"Exception: {ex.Message}";
        } 
    }

}