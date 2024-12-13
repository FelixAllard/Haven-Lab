using Microsoft.AspNetCore.Mvc;
using Moq;
using Shopify_Api;
using Shopify_Api.Controllers;
using Shopify_Api.Exceptions;
using ShopifySharp;
using ShopifySharp.Factories;

namespace TestingProject.Shopify_Api.Controllers;

[TestFixture]
public class DraftOrderControllerTest
{
    private Mock<IDraftOrderService> _mockShopifyService;
    private Mock<IDraftOrderServiceFactory> _mockServiceFactory;
    private ShopifyRestApiCredentials _falseCredentials;
    private DraftOrderController _controller;

    [SetUp]
    public void Setup()
    {
        _mockShopifyService = new Mock<IDraftOrderService>();
        _mockServiceFactory = new Mock<IDraftOrderServiceFactory>();
        _falseCredentials = new ShopifyRestApiCredentials("NotARealURL","NotARealToken");

        // Mock the creation of the service
        _mockServiceFactory.Setup(x => x.Create(It.IsAny<ShopifySharp.Credentials.ShopifyApiCredentials>()))
                           .Returns(_mockShopifyService.Object);

        // Setup the controller
        _controller = new DraftOrderController(_mockServiceFactory.Object, _falseCredentials);
    }

    [Test]
    public async Task PostProduct_ShouldReturnInvoiceUrl_WhenSuccessful()
    {
        // Arrange
        var order = new DraftOrder(); // Setup the order object as per your model

        var expectedInvoiceUrl = "https://invoiceurl.com";
        _mockShopifyService.Setup(s => s.CreateAsync(order,default)).ReturnsAsync(new ShopifySharp.DraftOrder
        {
            InvoiceUrl = expectedInvoiceUrl
        });

        // Act
        var result = await _controller.PostDraftOrder(order);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.AreEqual(expectedInvoiceUrl, okResult.Value);
    }

    [Test]
    public async Task PostProduct_ShouldReturnBadRequest_WhenInputExceptionOccurs()
    {
        // Arrange
        var order = new DraftOrder(); // Setup the order object as per your model
        _mockShopifyService.Setup(s => s.CreateAsync(order,default)).ThrowsAsync(new InputException("Invalid Input"));

        // Act
        var result = await _controller.PostDraftOrder(order);

        // Assert
        var badRequestResult = result as ObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
        Assert.That(badRequestResult.Value.ToString(), Is.EqualTo("{ message = Invalid Input }"));
    }

    [Test]
    public async Task PostProduct_ShouldReturnNotFound_WhenShopifyExceptionOccurs_With404Message()
    {
        // Arrange
        var order = new DraftOrder(); // Setup the order object as per your model
        _mockShopifyService.Setup(s => s.CreateAsync(order,default)).ThrowsAsync(new ShopifyException("(404 Not Found) Not Found"));

        // Act
        var result = await _controller.PostDraftOrder(order);

        // Assert
        var notFoundResult = result as ObjectResult;
        Assert.IsNotNull(notFoundResult);
        Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));
        Assert.That(notFoundResult.Value.ToString(), Is.EqualTo("{ message = (404 Not Found) Not Found }"));
    }

    [Test]
    public async Task PostProduct_ShouldReturnBadRequest_WhenShopifyExceptionOccurs_With400Message()
    {
        // Arrange
        var order = new DraftOrder(); // Setup the order object as per your model
        _mockShopifyService.Setup(s => s.CreateAsync(order, default)).ThrowsAsync(new ShopifyException("(400 Bad Request) draft_order: Required parameter missing or invalid"));

        // Act
        var result = await _controller.PostDraftOrder(order);

        // Assert
        var badRequestResult = result as ObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
        Assert.That(badRequestResult.Value.ToString(), Is.EqualTo("{ message = (400 Bad Request) draft_order: Required parameter missing or invalid }"));
    }
    [Test]
    public async Task PostProduct_ShouldReturnBadRequest_WhenShopifyExceptionOccurs_With500Message()
    {
        // Arrange
        var order = new DraftOrder(); // Setup the order object as per your model
        _mockShopifyService.Setup(s => s.CreateAsync(order, default)).ThrowsAsync(new ShopifyException("UnknownMessage"));

        // Act
        var result = await _controller.PostDraftOrder(order);

        // Assert
        var badRequestResult = result as ObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.That(badRequestResult.StatusCode, Is.EqualTo(500));
        Assert.That(badRequestResult.Value.ToString(), Is.EqualTo("{ message = Error fetching products, details = UnknownMessage }"));
    }

    [Test]
    public async Task PostProduct_ShouldReturnInternalServerError_WhenUnexpectedExceptionOccurs()
    {
        // Arrange
        var order = new DraftOrder(); // Setup the order object as per your model
        _mockShopifyService.Setup(s => s.CreateAsync(order, default)).ThrowsAsync(new System.Exception("Unexpected error"));

        // Act
        var result = await _controller.PostDraftOrder(order);

        // Assert
        var internalServerErrorResult = result as ObjectResult;
        Assert.IsNotNull(internalServerErrorResult);
        Assert.That(internalServerErrorResult.StatusCode, Is.EqualTo(500));
        Assert.That(internalServerErrorResult.Value.ToString(), Is.EqualTo("{ message = Error creating product Unexpected error }"));
    }
}
