using System.Net;
using Api_Gateway.Services;
using Moq;
using Moq.Protected;

namespace TestingProject.Api_Gateway.Services;
[TestFixture]
public class ServiceProductControllerTest
{
     private Mock<IHttpClientFactory> _mockHttpClientFactory;
    private Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private ServiceProductController _serviceProductController;

    [SetUp]
    public void SetUp()
    {
        // Mock HttpMessageHandler
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();

        // Create HttpClient using the mocked handler
        var httpClient = new HttpClient(_mockHttpMessageHandler.Object);

        // Mock IHttpClientFactory to return the mocked HttpClient
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockHttpClientFactory.Setup(factory => factory.CreateClient(It.IsAny<string>())).Returns(httpClient);

        // Instantiate ServiceProductController with the mocked IHttpClientFactory
        _serviceProductController = new ServiceProductController(_mockHttpClientFactory.Object);
    }

    [Test]
    public async Task GetAllProductsAsync_ReturnsProducts_WhenApiCallIsSuccessful()
    {
        // Arrange
        var expectedResponse = "{\"products\": [{\"id\": 1, \"name\": \"Product1\"}]}";
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(expectedResponse)
        };

        // Setup the mock HttpMessageHandler to return a successful response
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.ToString() == "http://localhost:5106/api/Products"),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(responseMessage);

        // Act
        var result = await _serviceProductController.GetAllProductsAsync();

        // Assert
        Assert.That(result, Is.EqualTo(expectedResponse));
    }

    [Test]
    public async Task GetAllProductsAsync_ReturnsError_WhenApiCallFails()
    {
        // Arrange
        var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("Error fetching products: Bad Request")
        };

        // Setup the mock HttpMessageHandler to return a failure response
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.ToString() == "http://localhost:5106/api/Products"),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(responseMessage);

        // Act
        var result = await _serviceProductController.GetAllProductsAsync();

        // Assert
        Assert.That(result, Is.EqualTo("Error fetching products: Bad Request"));
    }

    [Test]
    public async Task GetAllProductsAsync_ReturnsExceptionMessage_WhenExceptionOccurs()
    {
        // Arrange
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new System.Exception("Unexpected error"));

        // Act
        var result = await _serviceProductController.GetAllProductsAsync();

        // Assert
        Assert.That(result, Is.EqualTo("Exception: Unexpected error"));
    }
}