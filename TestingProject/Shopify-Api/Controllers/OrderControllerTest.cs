using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shopify_Api;
using Shopify_Api.Controllers;
using ShopifySharp;
using ShopifySharp.Credentials;
using ShopifySharp.Factories;

namespace TestingProject.Shopify_Api.Controllers;

[TestFixture]
public class OrderControllerTest
{
        private Mock<IOrderServiceFactory> _mockOrderServiceFactory;
        private Mock<IOrderService> _mockOrderService;
        private ShopifyRestApiCredentials _falseCredentials;
        private OrderController _controller;

        [SetUp]
        public void Setup()
        {
            _mockOrderServiceFactory = new Mock<IOrderServiceFactory>();
            _mockOrderService = new Mock<IOrderService>();
            _falseCredentials = new ShopifyRestApiCredentials("NotARealURL","NotARealToken");

            // Set up the mock to return the mock IProductService when Create is called.
            _mockOrderServiceFactory
                .Setup(x => x.Create(It.IsAny<ShopifyApiCredentials>()))
                .Returns(_mockOrderService.Object);

            _controller = new OrderController(_mockOrderServiceFactory.Object, _falseCredentials);
        }

        [Test]
        public async Task GetAllOrders_ReturnsOk_WhenOrdersAreFetchedSuccessfully()
        {
            // Arrange
            var orderList = new List<Order> 
            { 
                new Order { Id = 1, AppId = 1234567 },
                new Order { Id = 2, AppId = 1234560 }
            };

            // Create a ListResult<Product> containing the product list
            var listResult = new ShopifySharp.Lists.ListResult<Order>(orderList, default);

            // Mock the ListAsync method to return the ListResult object
            _mockOrderService.Setup(x => x.ListAsync(null, default)).ReturnsAsync(listResult);

            // Act
            var result = await _controller.GetAllOrders();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));

            // Extract the ListResult<Order> from okResult.Value
            var listResultValue = okResult.Value as ShopifySharp.Lists.ListResult<Order>;
            Assert.IsNotNull(listResultValue);

            // Compare the Items list
            var returnedOrders = listResultValue.Items;
            Assert.IsNotNull(returnedOrders);
            Assert.That(returnedOrders, Is.EqualTo(orderList));
        }




        [Test]
        public async Task GetAllOrders_ReturnsInternalServerError_WhenShopifyExceptionIsThrown()
        {
            // Arrange
            _mockOrderService.Setup(x => x.ListAsync(null, default))
                .ThrowsAsync(new ShopifyException("Shopify error"));

            // Act
            var result = await _controller.GetAllOrders();

            // Assert
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));

            var value = JObject.FromObject(objectResult.Value);
            Assert.AreEqual("Error fetching orders", value["message"]?.ToString());
        }


}