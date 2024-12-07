using Microsoft.AspNetCore.Mvc;
using Moq;
using Shopify_Api;
using Shopify_Api.Controllers;
using ShopifySharp;
using ShopifySharp.Credentials;
using ShopifySharp.Factories;
using ShopifySharp.Filters;
using ShopifySharp.Lists;

namespace TestingProject.Shopify_Api.SRC
{
    [TestFixture]
    public class ProductsControllerTests
    {
        private Mock<IProductService> _mockProductService;
        private Mock<IProductServiceFactory> _mockProductServiceFactory;
        private ShopifyRestApiCredentials _mockCredentials;
        private ProductsController _controller;

        [SetUp]
        public void Setup()
        {
            // Initialize mocks
            _mockProductService = new Mock<IProductService>();
            _mockProductServiceFactory = new Mock<IProductServiceFactory>();
            _mockCredentials = new ShopifyRestApiCredentials("https://my-shop.myshopify.com", "valid-access-token");

            // Set up the factory to return the mocked service
            _mockProductServiceFactory
                .Setup(factory => factory.Create(It.IsAny<ShopifyApiCredentials>()))
                .Returns(_mockProductService.Object);

            // Set up the credentials mock

            // Create the controller with the mocks
            _controller = new ProductsController(_mockProductServiceFactory.Object, _mockCredentials);
        }

        [Test]
        public async Task GetAllProducts_ReturnsOkResult_WhenServiceSucceeds()
        {
            // Arrange
            var expectedProducts = new List<Product>
            {
                new Product { Id = 1, Title = "Product 1" },
                new Product { Id = 2, Title = "Product 2" }
            };

            // Mock LinkHeaderParseResult (customize if needed)
            var mockLinkHeader = new Mock<LinkHeaderParseResult<Product>>();

            // Create a ListResult using the constructor with the two parameters
            var listResult = new ListResult<Product>(expectedProducts, mockLinkHeader.Object);

            // Set up ListAsync to return the ListResult
            _mockProductService.Setup(service => service.ListAsync(null, false, default))
                .ReturnsAsync(listResult);

            // Act: Call the method
            var result = await _controller.GetAllProducts();

            // Assert: Check if the result is OK with the correct products
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(expectedProducts, okResult.Value);
        }

        [Test]
        public async Task GetAllProducts_ReturnsStatusCode500_WhenServiceThrowsShopifyException()
        {
            // Arrange: Setup the mock to throw a ShopifyException
            var expectedErrorMessage = "Error fetching products";
            _mockProductService.Setup(service => service.ListAsync(null,false, default)).ThrowsAsync(new ShopifyException(expectedErrorMessage));

            // Act: Call the method
            var result = await _controller.GetAllProducts();

            // Assert: Check if the result is a status code 500 with the error message
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
            var response = objectResult.Value as dynamic;
            Assert.AreEqual(expectedErrorMessage, response.message);
            Assert.AreEqual(expectedErrorMessage, response.details);
        }

        [Test]
        public async Task GetAllProducts_ReturnsStatusCode500_WhenUnexpectedExceptionOccurs()
        {
            // Arrange: Setup the mock to throw an unexpected exception
            _mockProductService.Setup(service => service.ListAsync(null,false, default)).ThrowsAsync(new System.Exception("Unexpected error"));

            // Act: Call the method
            var result = await _controller.GetAllProducts();

            // Assert: Check if the result is a status code 500 with a generic error message
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
            var response = objectResult.Value as dynamic;
            Assert.AreEqual("Error fetching products", response.message);
            Assert.AreEqual("Unexpected error", response.details);
        }
    }
}
