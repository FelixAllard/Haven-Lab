namespace Shopify_Api;

public class ShopifyRestApiCredentials
{
    public string ShopUrl { get; set; }
    public string AccessToken { get; set; }

    public ShopifyRestApiCredentials(string shopUrl, string accessToken)
    {
        ShopUrl = shopUrl;
        AccessToken = accessToken;
    }
}
