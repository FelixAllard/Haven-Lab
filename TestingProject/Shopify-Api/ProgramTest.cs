using Microsoft.Extensions.DependencyInjection;
using Shopify_Api;

namespace TestingProject.Shopify_Api;

    
[TestFixture]
public class ProgramTests
{
    [Test]
    public void Test_ShopifyRestApiCredentials_Are_Registered()
    {
        // Arrange: Build the service provider with the necessary services
        var serviceProvider = new ServiceCollection()
            .AddSingleton<ShopifyRestApiCredentials>(sp =>
                new ShopifyRestApiCredentials(
                    "https://example.myshopify.com", 
                    "access_token"))
            .BuildServiceProvider();

        // Act: Try to resolve the service
        var credentials = serviceProvider.GetService<ShopifyRestApiCredentials>();

        // Assert: Ensure the service is resolved correctly
        Assert.NotNull(credentials);
    }
}


