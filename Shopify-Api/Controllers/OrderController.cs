using System.Diagnostics;
using ShopifySharp;
using ShopifySharp.Factories;

namespace Shopify_Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _shopifyService;

    public OrderController(
        IOrderServiceFactory productServiceFactory,
        Shopify_Api.ShopifyRestApiCredentials credentials
        )
    {

        
        _shopifyService = productServiceFactory.Create(new ShopifySharp.Credentials.ShopifyApiCredentials(
                credentials.ShopUrl,
                credentials.AccessToken
            )
        );
        
        
        //_shopifyService = new ShopifyService(shopUrl, accessToken);
    }
    
    //Add your functions here! Check the ProductController to see how to use the shopify service
}
