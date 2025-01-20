using System.Net;
using Api_Gateway.Services;
using Moq;
using Moq.Protected;
using ShopifySharp;

namespace TestingProject.Api_Gateway.Services;

[TestFixture]
public class ServicePromoControllerTest
{
    private Mock<IHttpClientFactory> _mockHttpClientFactory;
    private ServicePromoController _servicePromoController;

    [SetUp]
    public void SetUp()
    {
        // Mock the HttpClientFactory
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
    }

    [Test]
    public async Task GetAllPriceRulesAsync_ReturnsData_WhenApiCallIsSuccessful()
    {
        // Arrange: Mock HttpClient behavior
        var mockResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("[{\"id\":1,\"title\":\"Promo1\"},{\"id\":2,\"title\":\"Promo2\"}]")
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

        _servicePromoController = new ServicePromoController(_mockHttpClientFactory.Object);

        // Act: Call the method
        var result = await _servicePromoController.GetAllPriceRulesAsync();

        // Assert: Validate the result
        Assert.AreEqual("[{\"id\":1,\"title\":\"Promo1\"},{\"id\":2,\"title\":\"Promo2\"}]", result);
    }
    
    [Test]
    public async Task GetAllPriceRulesAsync_ReturnsErrorMessage_WhenApiCallFails()
    {
        // Arrange: Mock HttpClient to return an error
        var mockResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.BadRequest,
            ReasonPhrase = "Bad Request"
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

        _servicePromoController = new ServicePromoController(_mockHttpClientFactory.Object);

        // Act: Call the method
        var result = await _servicePromoController.GetAllPriceRulesAsync();

        // Assert: Validate the error message
        Assert.AreEqual("Error fetching price rules: Bad Request", result);
    }

    [Test]
    public async Task GetAllPriceRulesAsync_ReturnsExceptionMessage_WhenAnExceptionOccurs()
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

        _servicePromoController = new ServicePromoController(_mockHttpClientFactory.Object);

        // Act: Call the method
        var result = await _servicePromoController.GetAllPriceRulesAsync();

        // Assert: Validate the exception message
        Assert.AreEqual("Exception: Test Exception", result);
    }
    
    [Test]
    public async Task GetPriceRuleByIdAsync_ReturnsData_WhenApiCallIsSuccessful()
    {
        // Arrange: Mock HttpClient behavior
        var mockResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("{\"id\":1,\"title\":\"PriceRule1\"}")
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

        _servicePromoController = new ServicePromoController(_mockHttpClientFactory.Object);

        // Act: Call the method
        var result = await _servicePromoController.GetPriceRuleByIdAsync(1);

        // Assert: Validate the result
        Assert.AreEqual("{\"id\":1,\"title\":\"PriceRule1\"}", result);
    }
    
      [Test]
    public async Task GetPriceRuleByIdAsync_ReturnsNotFoundMessage_WhenApiCallReturns404()
    {
        // Arrange: Mock HttpClient to return 404
        var mockResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NotFound,
            Content = new StringContent("404 Not Found: Price rule not found")
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

        _servicePromoController = new ServicePromoController(_mockHttpClientFactory.Object);

        // Act: Call the method
        var result = await _servicePromoController.GetPriceRuleByIdAsync(1);

        // Assert: Validate the not found message
        Assert.AreEqual("404 Not Found: Price rule not found", result);
    }

    [Test]
    public async Task GetPriceRuleByIdAsync_ReturnsErrorMessage_WhenApiCallFails()
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

        _servicePromoController = new ServicePromoController(_mockHttpClientFactory.Object);

        // Act: Call the method
        var result = await _servicePromoController.GetPriceRuleByIdAsync(1);

        // Assert: Validate the error message
        Assert.AreEqual("Error fetching price rule by ID: Internal Server Error", result);
    }
    
    [Test]
    public async Task GetPriceRuleByIdAsync_ReturnsExceptionMessage_WhenAnExceptionOccurs()
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

        _servicePromoController = new ServicePromoController(_mockHttpClientFactory.Object);

        // Act: Call the method
        var result = await _servicePromoController.GetPriceRuleByIdAsync(1);

        // Assert: Validate the exception message
        Assert.AreEqual("Exception: Test Exception", result);
    }
    
     [Test]
    public async Task DeletePriceRuleAsync_ReturnsSuccessMessage_WhenApiCallIsSuccessful()
    {
        // Arrange: Mock HttpClient behavior for a successful deletion
        var mockResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("Price rule deleted successfully.")
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

        _servicePromoController = new ServicePromoController(_mockHttpClientFactory.Object);

        // Act: Call the method
        var result = await _servicePromoController.DeletePriceRuleAsync(1);

        // Assert: Validate the result
        Assert.AreEqual("Price rule deleted successfully.", result);
    }

    [Test]
    public async Task DeletePriceRuleAsync_ReturnsNotFoundMessage_WhenPriceRuleDoesNotExist()
    {
        // Arrange: Mock HttpClient behavior for a 404 response
        var mockResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NotFound,
            Content = new StringContent("404 Not Found: Price rule not found")
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

        _servicePromoController = new ServicePromoController(_mockHttpClientFactory.Object);

        // Act: Call the method
        var result = await _servicePromoController.DeletePriceRuleAsync(1);

        // Assert: Validate the not found message
        Assert.AreEqual("404 Not Found: Price rule not found", result);
    }

    [Test]
    public async Task DeletePriceRuleAsync_ReturnsErrorMessage_WhenApiCallFails()
    {
        // Arrange: Mock HttpClient behavior for a failed response
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

        _servicePromoController = new ServicePromoController(_mockHttpClientFactory.Object);

        // Act: Call the method
        var result = await _servicePromoController.DeletePriceRuleAsync(1);

        // Assert: Validate the error message
        Assert.AreEqual("Error deleting price rule by ID: Internal Server Error", result);
    }

    [Test]
    public async Task DeletePriceRuleAsync_ReturnsExceptionMessage_WhenAnExceptionOccurs()
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

        _servicePromoController = new ServicePromoController(_mockHttpClientFactory.Object);

        // Act: Call the method
        var result = await _servicePromoController.DeletePriceRuleAsync(1);

        // Assert: Validate the exception message
        Assert.AreEqual("Exception: Test Exception", result);
    }

    [Test]
    public async Task GetAllDiscountsByRuleAsync_ReturnsData_WhenApiCallIsSuccessful()
    {
        // Arrange: Mock HttpClient behavior for a successful response
        var mockResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("[{\"id\":1,\"code\":\"DISCOUNT1\"},{\"id\":2,\"code\":\"DISCOUNT2\"}]")
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

        _servicePromoController = new ServicePromoController(_mockHttpClientFactory.Object);

        // Act: Call the method
        var result = await _servicePromoController.GetAllDiscountsByRuleAsync(1);

        // Assert: Validate the result
        Assert.AreEqual("[{\"id\":1,\"code\":\"DISCOUNT1\"},{\"id\":2,\"code\":\"DISCOUNT2\"}]", result);
    }
    
    [Test]
    public async Task GetAllDiscountsByRuleAsync_ReturnsErrorMessage_WhenApiCallFails()
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

        _servicePromoController = new ServicePromoController(_mockHttpClientFactory.Object);

        // Act: Call the method
        var result = await _servicePromoController.GetAllDiscountsByRuleAsync(1);

        // Assert: Validate the error message
        Assert.AreEqual("Error fetching discounts: Internal Server Error", result);
    }
    
    [Test]
    public async Task GetAllDiscountsByRuleAsync_ReturnsExceptionMessage_WhenAnExceptionOccurs()
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

        _servicePromoController = new ServicePromoController(_mockHttpClientFactory.Object);

        // Act: Call the method
        var result = await _servicePromoController.GetAllDiscountsByRuleAsync(1);

        // Assert: Validate the exception message
        Assert.AreEqual("Exception: Test Exception", result);
    }
    
     [Test]
    public async Task CreateDiscountAsync_ReturnsHttpResponseMessage_WhenApiCallIsSuccessful()
    {
        // Arrange: Mock HttpClient behavior for a successful response
        var mockResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.Created,
            Content = new StringContent("{\"code\":\"DISCOUNT123\"}")
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

        _servicePromoController = new ServicePromoController(_mockHttpClientFactory.Object);

        var discountCode = new PriceRuleDiscountCode { Code = "DISCOUNT123" };

        // Act: Call the method
        var result = await _servicePromoController.CreateDiscountAsync(1, discountCode);

        // Assert: Validate the result
        Assert.AreEqual(HttpStatusCode.Created, result.StatusCode);
        var responseContent = await result.Content.ReadAsStringAsync();
        Assert.AreEqual("{\"code\":\"DISCOUNT123\"}", responseContent);
    }

    [Test]
    public async Task CreateDiscountAsync_ReturnsHttpResponseMessage_WhenApiCallFails()
    {
        // Arrange: Mock HttpClient to return an error
        var mockResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.BadRequest,
            ReasonPhrase = "Bad Request"
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

        _servicePromoController = new ServicePromoController(_mockHttpClientFactory.Object);

        var discountCode = new PriceRuleDiscountCode { Code = "INVALID" };

        // Act: Call the method
        var result = await _servicePromoController.CreateDiscountAsync(1, discountCode);

        // Assert: Validate the result
        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.AreEqual("Bad Request", result.ReasonPhrase);
    }

    [Test]
    public async Task CreateDiscountAsync_ReturnsHttpResponseMessage_WithExceptionMessage_WhenAnExceptionOccurs()
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

        _servicePromoController = new ServicePromoController(_mockHttpClientFactory.Object);

        var discountCode = new PriceRuleDiscountCode { Code = "DISCOUNT123" };

        // Act: Call the method
        var result = await _servicePromoController.CreateDiscountAsync(1, discountCode);

        // Assert: Validate the exception handling
        Assert.AreEqual(HttpStatusCode.InternalServerError, result.StatusCode);
        var responseContent = await result.Content.ReadAsStringAsync();
        Assert.AreEqual("Exception: Test Exception", responseContent);
    }
    
    [Test]
    public async Task DeleteDiscountAsync_ReturnsSuccessMessage_WhenApiCallIsSuccessful()
    {
        // Arrange: Mock HttpClient behavior for a successful delete
        var mockResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("Discount code deleted successfully.")
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

        _servicePromoController = new ServicePromoController(_mockHttpClientFactory.Object);

        // Act: Call the method
        var result = await _servicePromoController.DeleteDiscountAsync(1, 1);

        // Assert: Validate the result
        Assert.AreEqual("Discount code deleted successfully.", result);
    }

    [Test]
    public async Task DeleteDiscountAsync_ReturnsNotFoundMessage_WhenDiscountDoesNotExist()
    {
        // Arrange: Mock HttpClient to return a 404 Not Found
        var mockResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NotFound,
            Content = new StringContent("404 Not Found: Discount code not found")
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

        _servicePromoController = new ServicePromoController(_mockHttpClientFactory.Object);

        // Act: Call the method
        var result = await _servicePromoController.DeleteDiscountAsync(1, 999);

        // Assert: Validate the result
        Assert.AreEqual("404 Not Found: Discount code not found", result);
    }
    
     [Test]
    public async Task DeleteDiscountAsync_ReturnsErrorMessage_WhenApiCallFails()
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

        _servicePromoController = new ServicePromoController(_mockHttpClientFactory.Object);

        // Act: Call the method
        var result = await _servicePromoController.DeleteDiscountAsync(1, 2);

        // Assert: Validate the result
        Assert.AreEqual("Error deleting discount code: Internal Server Error", result);
    }

    [Test]
    public async Task DeleteDiscountAsync_ReturnsExceptionMessage_WhenAnExceptionOccurs()
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

        _servicePromoController = new ServicePromoController(_mockHttpClientFactory.Object);

        // Act: Call the method
        var result = await _servicePromoController.DeleteDiscountAsync(1, 1);

        // Assert: Validate the result
        Assert.AreEqual("Exception: Test Exception", result);
    }
}