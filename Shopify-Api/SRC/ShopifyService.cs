namespace Shopify_Api;

using ShopifySharp;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ShopifyService
{
    private readonly string _shopUrl;
    private readonly string _accessToken;

    public static string shopUrl;
    public static string accessToken;

    public ShopifyService(string shopUrl, string accessToken)
    {
        _shopUrl = shopUrl;
        _accessToken = accessToken;
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        var productService = new ProductService(_shopUrl, _accessToken);
        // Fetch products and access the Items property to return the actual list
        var products = await productService.ListAsync();
        return products.Items;
    }

}
