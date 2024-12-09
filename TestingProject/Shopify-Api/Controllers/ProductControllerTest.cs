using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using Shopify_Api;
using Shopify_Api.Controllers;
using ShopifySharp;
using ShopifySharp.Credentials;
using ShopifySharp.Factories;
using Newtonsoft.Json.Linq;
using Shopify_Api.Exceptions;
using TestingProject.Utility;

namespace TestingProject.Shopify_Api.Controllers;

public class ProductControllerTest
{
    private Mock<IProductServiceFactory> _mockProductServiceFactory;
    private Mock<IProductService> _mockProductService;
    private ShopifyRestApiCredentials _falseCredentials;
    private ProductsController _controller;
    private ProductValidator _productValidator;

    [SetUp]
    public void Setup()
    {
        _mockProductServiceFactory = new Mock<IProductServiceFactory>();
        _mockProductService = new Mock<IProductService>();
        _falseCredentials = new ShopifyRestApiCredentials("NotARealURL","NotARealToken");
        _productValidator = new ProductValidator();

        // Set up the mock to return the mock IProductService when Create is called.
        _mockProductServiceFactory
            .Setup(x => x.Create(It.IsAny<ShopifyApiCredentials>()))
            .Returns(_mockProductService.Object);

        _controller = new ProductsController(
            _mockProductServiceFactory.Object, 
            _falseCredentials,
            _productValidator
        );
    }

    [Test]
    public async Task GetAllProducts_ReturnsOk_WhenProductsAreFetchedSuccessfully()
    {
        // Arrange
        var productList = new List<Product> 
        { 
            new Product { Id = 1, Title = "Product 1" },
            new Product { Id = 2, Title = "Product 2" }
        };

        // Create a ListResult<Product> containing the product list
        var listResult = new ShopifySharp.Lists.ListResult<Product>(productList, default);

        // Mock the ListAsync method to return the ListResult object
        _mockProductService.Setup(x => x.ListAsync(null, false, default)).ReturnsAsync(listResult);

        // Act
        var result = await _controller.GetAllProducts();

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));

        // Extract the ListResult<Product> from okResult.Value
        var listResultValue = okResult.Value as ShopifySharp.Lists.ListResult<Product>;
        Assert.IsNotNull(listResultValue);

        // Compare the Items list
        var returnedProducts = listResultValue.Items;
        Assert.IsNotNull(returnedProducts);
        Assert.That(returnedProducts, Is.EqualTo(productList).Using(new ProductComparer()));
    }




    [Test]
    public async Task GetAllProducts_ReturnsInternalServerError_WhenShopifyExceptionIsThrown()
    {
        // Arrange
        _mockProductService.Setup(x => x.ListAsync(null, false, default))
            .ThrowsAsync(new ShopifyException("Shopify error"));

        // Act
        var result = await _controller.GetAllProducts();

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));

        var value = JObject.FromObject(objectResult.Value);
        Assert.AreEqual("Error fetching products", value["message"]?.ToString());
    }

    [TestCase("Input1")]
    [TestCase("Input2")]
    [TestCase("Input3")]
    [Test]
    
    public async Task GetAllProducts_ReturnsInternalServerError_WhenUnexpectedExceptionOccurs(string testCase)
    {
        
        // Arrange
        _mockProductService.Setup(x => x.ListAsync(null, false, default)).ThrowsAsync(new System.Exception(testCase));

        // Act
        var result = await _controller.GetAllProducts();

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));

        // Deserialize the response body to a JObject
        var responseBody = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(objectResult.Value));

        // Assert the message property
        Assert.That(responseBody["message"]?.ToString(), Is.EqualTo($"Error fetching products{testCase}"));
    }
        
        
        ///----------------------------------POST METHOD
        ///

    [Test]
    public async Task PostProduct_ReturnsOk_WhenProductIsCreatedSuccessfully()
    {
        // Arrange: Create a valid product
        var validProduct = new Product
        {
            Title = "Example Product",
            BodyHtml = "<p>A good example product</p>",
            CreatedAt = DateTime.Parse("2024-12-08T23:40:19-05:00"),
            UpdatedAt = DateTime.Parse("2024-12-08T23:40:19-05:00"),
            PublishedAt = DateTime.Parse("2024-12-08T22:17:59-05:00"),
            Vendor = "New Vendor",
            ProductType = "",
            Handle = "haven-lab",
            PublishedScope = "global",
            Status = "active",
            Variants = new List<ProductVariant>
            {
                new ProductVariant
                {
                    ProductId = 8073575366701,
                    Title = "Default Title",
                    SKU = null,
                    Position = 1,
                    Grams = 0,
                    InventoryPolicy = "deny",
                    FulfillmentService = "manual",
                    Taxable = true,
                    Weight = 0,
                    InventoryItemId = 45205286223917,
                    Price = 19.99M,
                    RequiresShipping = true,
                    InventoryQuantity = 5,
                    WeightUnit = "kg"
                }
            }
        };

        // Mock the service method CreateAsync to return the valid product
        _mockProductService.Setup(x => x.CreateAsync(It.IsAny<Product>(), default, default))
            .ReturnsAsync(validProduct);

        // Act: Call the controller method to create a product
        var result = await _controller.PostProduct(validProduct);

        // Assert: Verify the result
        Assert.IsNotNull(result);
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult, "Expected OkObjectResult but got null.");
        Assert.AreEqual(200, okResult.StatusCode, "Status code should be 200 OK.");

        // Verify that the returned product matches the created product
        var createdProduct = okResult.Value as Product;
        Assert.IsNotNull(createdProduct, "Created product should not be null.");
        Assert.AreEqual(validProduct.Title, createdProduct?.Title, "Product titles should match.");
        Assert.AreEqual(validProduct.Vendor, createdProduct?.Vendor, "Product vendors should match.");
        Assert.AreEqual(validProduct.Variants.Count(), createdProduct?.Variants.Count(), "Product variants count should match.");

        // Verify that CreateAsync was called exactly once
        _mockProductService.Verify(x => x.CreateAsync(It.IsAny<Product>(), default, default), Times.Once, "CreateAsync was not called exactly once.");
    }








    [Test]
    public async Task PostProduct_ReturnsBadRequest_WhenInputExceptionIsThrown()
    {
        // Arrange
        var product = new Product
        {
            Title = "Example Product",
            BodyHtml = "<p>A good example product</p>",
            CreatedAt = DateTime.Parse("2024-12-08T23:40:19-05:00"),
            UpdatedAt = DateTime.Parse("2024-12-08T23:40:19-05:00"),
            PublishedAt = DateTime.Parse("2024-12-08T22:17:59-05:00"),
            Vendor = "New Vendor",
            ProductType = "",
            Handle = "haven-lab",
            PublishedScope = "global",
            Status = "active",
            Variants = new List<ProductVariant>
            {
                new ProductVariant
                {
                    ProductId = 8073575366701,
                    Title = "Default Title",
                    SKU = null,
                    Position = 1,
                    Grams = 0,
                    InventoryPolicy = "deny",
                    FulfillmentService = "manual",
                    InventoryItemId = 45205286223917,
                    Price = 19.99M,
                    RequiresShipping = true,
                    Taxable = true,
                    InventoryQuantity = 5,
                    Weight = 54,
                    WeightUnit = "kg"
                }
            }
        };

        _mockProductService.Setup(x => x.CreateAsync(It.IsAny<Product>(), default,default))
            .ThrowsAsync(new InputException("Invalid input"));
        

        // Act
        var result = await _controller.PostProduct(product);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.That(objectResult.StatusCode, Is.EqualTo(400));

        var value = JObject.FromObject(objectResult.Value);
        Assert.AreEqual("Invalid input", value["message"]?.ToString());
    }

    [Test]
    public async Task PostProduct_ReturnsInternalServerError_WhenShopifyExceptionIsThrown()
    {
        // Arrange
        var product = new Product
        {
            Title = "Example Product",
            BodyHtml = "<p>A good example product</p>",
            CreatedAt = DateTime.Parse("2024-12-08T23:40:19-05:00"),
            UpdatedAt = DateTime.Parse("2024-12-08T23:40:19-05:00"),
            PublishedAt = DateTime.Parse("2024-12-08T22:17:59-05:00"),
            Vendor = "New Vendor",
            ProductType = "",
            Handle = "haven-lab",
            PublishedScope = "global",
            Status = "active",
            Variants = new List<ProductVariant>
            {
                new ProductVariant
                {
                    ProductId = 8073575366701,
                    Title = "Default Title",
                    SKU = null,
                    Position = 1,
                    Grams = 0,
                    InventoryPolicy = "deny",
                    FulfillmentService = "manual",
                    InventoryItemId = 45205286223917,
                    Price = 19.99M,
                    RequiresShipping = true,
                    Taxable = true,
                    InventoryQuantity = 5,
                    Weight = 54,
                    WeightUnit = "kg"
                }
            }
        };

        _mockProductService.Setup(x => x.CreateAsync(It.IsAny<Product>(), default, default))
            .ThrowsAsync(new ShopifyException("Shopify error"));

        // Act
        var result = await _controller.PostProduct(product);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));

        var value = JObject.FromObject(objectResult.Value);
        Assert.AreEqual("Error fetching products", value["message"]?.ToString());
    }

    [TestCase("Unexpected error 1")]
    [TestCase("Unexpected error 2")]
    [TestCase("Unexpected error 3")]
    [Test]
    public async Task PostProduct_ReturnsInternalServerError_WhenUnexpectedExceptionOccurs(string testCase)
    {
        // Arrange
        var product = new Product
        {
            Title = "Example Product",
            BodyHtml = "<p>A good example product</p>",
            CreatedAt = DateTime.Parse("2024-12-08T23:40:19-05:00"),
            UpdatedAt = DateTime.Parse("2024-12-08T23:40:19-05:00"),
            PublishedAt = DateTime.Parse("2024-12-08T22:17:59-05:00"),
            Vendor = "New Vendor",
            ProductType = "",
            Handle = "haven-lab",
            PublishedScope = "global",
            Status = "active",
            Variants = new List<ProductVariant>
            {
                new ProductVariant
                {
                    ProductId = 8073575366701,
                    Title = "Default Title",
                    SKU = null,
                    Position = 1,
                    Grams = 0,
                    InventoryPolicy = "deny",
                    FulfillmentService = "manual",
                    InventoryItemId = 45205286223917,
                    Price = 19.99M,
                    RequiresShipping = true,
                    Taxable = true,
                    InventoryQuantity = 5,
                    Weight = 54,
                    WeightUnit = "kg"
                }
            }
        };

        _mockProductService.Setup(
                x => x.CreateAsync(
                    It.IsAny<Product>(), 
                    default,
                    default
                    )
                )
            .ThrowsAsync(new System.Exception(testCase));

        // Act
        var result = await _controller.PostProduct(product);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));

        var responseBody = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(objectResult.Value));
        Assert.That(responseBody["message"]?.ToString(), Is.EqualTo($"Error creating product {testCase}"));
    }
}