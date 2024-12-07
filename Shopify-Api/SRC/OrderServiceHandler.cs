namespace Shopify_Api;

using ShopifySharp;

public class OrderServiceHandler
{
    private readonly string _shopUrl;
    private readonly string _accessToken;

    public OrderServiceHandler(string shopUrl, string accessToken)
    {
        _shopUrl = shopUrl;
        _accessToken = accessToken;
    }

    public async Task ListOrdersAsync()
    {
        var orderService = new OrderService(_shopUrl, _accessToken);
        var orders = await orderService.ListAsync();

        foreach (var order in orders.Items)
        {
            Console.WriteLine($"Order ID: {order.Id}, Total: {order.TotalPrice}");
        }
    }
}
