using System.Net;
using Api_Gateway.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.Protected;
using ShopifySharp;

namespace TestingProject.Api_Gateway.Services;

[TestFixture]
public class ServiceOrderControllerTest
{
    private Mock<IHttpClientFactory> _mockHttpClientFactory;
    private ServiceOrderController _serviceOrderController;

    [SetUp]
    public void SetUp()
    {
        // Mock the HttpClientFactory
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
    }

    [Test]
    public async Task GetAllOrdersAsync_ReturnsData_WhenApiCallIsSuccessful()
    {
        // Arrange: Mock HttpClient behavior
        var mockResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("[{\"id\":1,\"name\":\"Order1\"},{\"id\":2,\"name\":\"Order2\"}]")
        };

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(mockResponseMessage);

        var client = new HttpClient(handlerMock.Object);
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);

        _serviceOrderController = new ServiceOrderController(_mockHttpClientFactory.Object);

        // Act: Call the method
        var result = await _serviceOrderController.GetAllOrdersAsync();

        // Assert: Validate the result
        Assert.AreEqual("[{\"id\":1,\"name\":\"Order1\"},{\"id\":2,\"name\":\"Order2\"}]", result);
    }

    [Test]
    public async Task GetAllOrdersAsync_ReturnsErrorMessage_WhenApiCallFails()
    {
        // Arrange: Mock HttpClient to return an error
        var mockResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.InternalServerError,
            ReasonPhrase = "Internal Server Error"
        };

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(mockResponseMessage);

        var client = new HttpClient(handlerMock.Object);
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);

        _serviceOrderController = new ServiceOrderController(_mockHttpClientFactory.Object);

        // Act: Call the method
        var result = await _serviceOrderController.GetAllOrdersAsync();

        // Assert: Validate the error message
        Assert.AreEqual("Error fetching orders: Internal Server Error", result);
    }

    [Test]
    public async Task GetAllOrdersAsync_ReturnsExceptionMessage_WhenAnExceptionOccurs()
    {
        // Arrange: Mock HttpClient to throw an exception
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new Exception("Test Exception"));

        var client = new HttpClient(handlerMock.Object);
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);

        _serviceOrderController = new ServiceOrderController(_mockHttpClientFactory.Object);

        // Act: Call the method
        var result = await _serviceOrderController.GetAllOrdersAsync();

        // Assert: Validate the exception message
        Assert.AreEqual("Exception: Test Exception", result);
    }

    [Test]
    public async Task GetOrderByIdAsync_ReturnsData_WhenApiCallIsSuccessful()
    {
        // Arrange: Mock HttpClient behavior
        var mockResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("{\"id\":1,\"name\":\"Order1\"}")
        };

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(mockResponseMessage);

        var client = new HttpClient(handlerMock.Object);
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);

        _serviceOrderController = new ServiceOrderController(_mockHttpClientFactory.Object);

        // Act: Call the method
        var result = await _serviceOrderController.GetOrderByIdAsync(1);

        // Assert: Validate the result
        Assert.AreEqual("{\"id\":1,\"name\":\"Order1\"}", result);
    }
    
    [Test]
    public async Task GetOrderByIdAsync_ReturnsErrorMessage_WhenApiCallFails()
    {
        // Arrange: Mock HttpClient to return an error
        var mockResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.InternalServerError,
            ReasonPhrase = "Internal Server Error"
        };

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(mockResponseMessage);

        var client = new HttpClient(handlerMock.Object);
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);

        _serviceOrderController = new ServiceOrderController(_mockHttpClientFactory.Object);

        // Act: Call the method
        var result = await _serviceOrderController.GetOrderByIdAsync(1);

        // Assert: Validate the error message
        Assert.AreEqual("Error fetching order with ID 1: Internal Server Error", result);
    }
    
    [Test]
    public async Task GetOrderByIdAsync_ReturnsExceptionMessage_WhenAnExceptionOccurs()
    {
        // Arrange: Mock HttpClient to throw an exception
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new Exception("Test Exception"));

        var client = new HttpClient(handlerMock.Object);
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);

        _serviceOrderController = new ServiceOrderController(_mockHttpClientFactory.Object);

        // Act: Call the method
        var result = await _serviceOrderController.GetOrderByIdAsync(1);

        // Assert: Validate the exception message
        Assert.AreEqual("Exception: Test Exception", result);
    }
    
    [Test]
    public async Task PutOrderAsync_ReturnsServiceUnavailable_WhenHttpRequestExceptionOccurs()
    {
        // Arrange: Mock HttpClient to throw an HttpRequestException
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network error"));

        var client = new HttpClient(handlerMock.Object);
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);

        _serviceOrderController = new ServiceOrderController(_mockHttpClientFactory.Object);

        var order = new Order { /* Set necessary properties for order */ };

        // Act: Call the method
        var result = await _serviceOrderController.PutOrderAsync(1, order);

        // Assert: Validate the exception message and status code
        Assert.AreEqual(System.Net.HttpStatusCode.ServiceUnavailable, result.StatusCode);
        Assert.AreEqual("Exception: Network error", await result.Content.ReadAsStringAsync());
    }
    
    [Test]
    public async Task PutOrderAsync_ReturnsInternalServerError_WhenGeneralExceptionOccurs()
    {
        // Arrange: Mock HttpClient to throw a general exception
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new Exception("Unexpected error"));

        var client = new HttpClient(handlerMock.Object);
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);

        _serviceOrderController = new ServiceOrderController(_mockHttpClientFactory.Object);

        var order = new Order { /* Set necessary properties for order */ };

        // Act: Call the method
        var result = await _serviceOrderController.PutOrderAsync(1, order);

        // Assert: Validate the exception message and status code
        Assert.AreEqual(System.Net.HttpStatusCode.InternalServerError, result.StatusCode);
        Assert.AreEqual("Exception: Unexpected error", await result.Content.ReadAsStringAsync());
    }
    
    [Test]
    public async Task PutOrderAsync_ReturnsSuccess_WhenApiCallIsSuccessful()
    {
        // Arrange: Mock HttpClient to return a successful response
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent("Order updated successfully")
            });

        var client = new HttpClient(handlerMock.Object);
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);

        _serviceOrderController = new ServiceOrderController(_mockHttpClientFactory.Object);

        var order = new Order { /* Set necessary properties for order */ };

        // Act: Call the method
        var result = await _serviceOrderController.PutOrderAsync(1, order);

        // Assert: Validate the response status code and message
        Assert.AreEqual(System.Net.HttpStatusCode.OK, result.StatusCode);
        Assert.AreEqual("Order updated successfully", await result.Content.ReadAsStringAsync());
    }
    
} 