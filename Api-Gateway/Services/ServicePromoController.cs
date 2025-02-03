using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ShopifySharp;

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
            var client = _httpClientFactory.CreateClient();
            var requestUrl = $"{BASE_URL}/api/Promo/PriceRules";

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
    
    public virtual async Task<string> GetPriceRuleByIdAsync(long id)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var requestUrl = $"{BASE_URL}/api/Promo/PriceRules/{id}";

            var response = await client.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return "404 Not Found: Price rule not found";
            }
            else
            {
                return $"Error fetching price rule by ID: {response.ReasonPhrase}";
            }
        }
        catch (Exception ex)
        {
            return $"Exception: {ex.Message}";
        }
    }
    
    public virtual async Task<HttpResponseMessage> CreatePriceRuleAsync(PriceRule priceRule)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var requestUrl = $"{BASE_URL}/api/Promo/PriceRules";
            
            var jsonSettings = new Newtonsoft.Json.JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(), 
                Formatting = Formatting.None 
            };
            
            var jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(priceRule, jsonSettings);

            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            
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
            return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError)
            {
                Content = new StringContent($"Error: {ex.Message}")
            };
        }
    }
    
    public virtual async Task<HttpResponseMessage> PutPriceRuleAsync(long id, PriceRule priceRule)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var requestUrl = $"{BASE_URL}/api/Promo/PriceRules/{id}";

            var jsonSettings = new Newtonsoft.Json.JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(), 
                Formatting = Formatting.None 
            };

            var jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(priceRule, jsonSettings);

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
            return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError)
            {
                Content = new StringContent($"Error: {ex.Message}")
            };
        }
    }
    
    public virtual async Task<string> DeletePriceRuleAsync(long id)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var requestUrl = $"{BASE_URL}/api/Promo/PriceRules/{id}";

            var response = await client.DeleteAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                return "Price rule deleted successfully.";
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return "404 Not Found: Price rule not found";
            }
            else
            {
                return $"Error deleting price rule by ID: {response.ReasonPhrase}";
            }
        }
        catch (Exception ex)
        {
            return $"Exception: {ex.Message}";
        }
    }
    
    public virtual async Task<string> GetAllDiscountsByRuleAsync(long priceRuleId)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var requestUrl = $"{BASE_URL}/api/Promo/Discounts/{priceRuleId}";

            var response = await client.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                return $"Error fetching discounts: {response.ReasonPhrase}";
            }
        }
        catch (Exception ex)
        {
            return $"Exception: {ex.Message}";
        }
    }
    
    public virtual async Task<HttpResponseMessage> CreateDiscountAsync(long priceRuleId, PriceRuleDiscountCode discountCode)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var requestUrl = $"{BASE_URL}/api/Promo/Discounts/{priceRuleId}";

            var jsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.None
            };

            var jsonContent = JsonConvert.SerializeObject(discountCode, jsonSettings);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            return await client.PostAsync(requestUrl, content);
        }
        catch (Exception ex)
        {
            return new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent($"Exception: {ex.Message}")
            };
        }
    }
    
    public virtual async Task<string> DeleteDiscountAsync(long priceRuleId, long discountId)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var requestUrl = $"{BASE_URL}/api/Promo/Discounts/{priceRuleId}/{discountId}";

            var response = await client.DeleteAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                return "Discount code deleted successfully.";
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return "404 Not Found: Discount code not found";
            }
            else
            {
                return $"Error deleting discount code: {response.ReasonPhrase}";
            }
        }
        catch (Exception ex)
        {
            return $"Exception: {ex.Message}";
        }
    }

}