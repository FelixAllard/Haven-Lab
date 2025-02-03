namespace Api_Gateway.Services;

public class ServiceEmailLogController
{
    private readonly IHttpClientFactory _httpClientFactory; // Use IHttpClientFactory instead of HttpClient directly
    private readonly string BASE_URL; // Your base URL for Shopify API

    // Constructor that takes in IHttpClientFactory via Dependency Injection
    public ServiceEmailLogController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        BASE_URL = Environment.GetEnvironmentVariable("BASE_URL_EMAIL_API")??"http://localhost:5092";
    }
    public virtual async Task<string> GetAllEmailLogs()
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var requestUrl = $"{BASE_URL}/api/EmailLog";

            var response = await client.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                return $"Error fetching email Logs: {response.ReasonPhrase}";
            }
        }
        catch (Exception ex)
        {
            return $"Exception: {ex.Message}";
        }
    }
}