using Microsoft.AspNetCore.Mvc;
using Moq;
using Shopify_Api;
using Shopify_Api.Controllers;
using ShopifySharp;
using ShopifySharp.Credentials;
using ShopifySharp.Factories;
using ShopifySharp.Filters;

namespace TestingProject.Shopify_Api.Controllers;

[TestFixture]
public class CustomerControllerTests
{
    private Mock<ICustomerServiceFactory> _mockCustomerServiceFactory;
    private Mock<ICustomerService> _mockCustomerService;
    private ShopifyRestApiCredentials _falseCredentials;
    private CustomerController _controller;

    [SetUp]
    public void Setup()
    {
        _mockCustomerServiceFactory = new Mock<ICustomerServiceFactory>();
        _mockCustomerService = new Mock<ICustomerService>();
        _falseCredentials = new ShopifyRestApiCredentials("NotARealURL","NotARealToken");

        // Set up the mock to return the mock IProductService when Create is called.
        _mockCustomerServiceFactory
            .Setup(x => x.Create(It.IsAny<ShopifyApiCredentials>()))
            .Returns(_mockCustomerService.Object);

        _controller = new CustomerController(_mockCustomerServiceFactory.Object, _falseCredentials);
    }
    
    }