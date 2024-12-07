namespace Shopify_Api;

using ShopifySharp;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ShopifyService
{
    private readonly string _shopUrl;
    private readonly string _accessToken;
    private readonly ProductService _productService;

    // Constructor for injecting ProductService (useful for testing)
    public ShopifyService(string shopUrl, string accessToken, ProductService productService = null)
    {
        _shopUrl = shopUrl;
        _accessToken = accessToken;
        _productService = productService ?? new ProductService(_shopUrl, _accessToken);
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        var products = await _productService.ListAsync();
        return products.Items;
    }
}

