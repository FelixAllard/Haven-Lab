using System.Net;
using Api_Gateway.Models;
using Api_Gateway.Services;
using Moq;
using Moq.Protected;
using Microsoft.AspNetCore.Mvc;

namespace TestingProject.Api_Gateway.Services;

[TestFixture]
public class ServiceAuthControllerTest
{
    private Mock<IHttpClientFactory> _httpClientFactoryMock;
    private Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private ServiceAuthController _serviceAuthController;

    [SetUp]
    public void SetUp()
    {
        // Create a mock of the HttpMessageHandler to mock the HTTP response.
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        // Mock IHttpClientFactory to return an HttpClient using the mocked handler.
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _httpClientFactoryMock
            .Setup(factory => factory.CreateClient(It.IsAny<string>()))
            .Returns(new HttpClient(_httpMessageHandlerMock.Object));

        // Initialize the ServiceAuthController with the mocked IHttpClientFactory.
        _serviceAuthController = new ServiceAuthController(_httpClientFactoryMock.Object);
    }

    [Test]
    public async Task LoginAsync_Success_ReturnsResponseContent()
    {
        // Arrange
        var loginModel = new Login { Username = "user", Password = "password" };
        var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("Success response content")
        };

        // Mock the SendAsync method and handle null arguments properly
        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync", 
                ItExpr.IsAny<HttpRequestMessage>(), // Use ItExpr.IsAny() for any HttpRequestMessage
                ItExpr.IsAny<System.Threading.CancellationToken>() // Use ItExpr.IsAny() for cancellation token
            )
            .ReturnsAsync(mockResponse);

        // Act
        var result = await _serviceAuthController.LoginAsync(loginModel);

        // Assert
        Assert.AreEqual("Success response content", result);
    }
    
    [Test]
        public async Task LoginAsync_Unauthorized_ReturnsUnauthorizedMessage()
        {
            // Arrange
            var loginModel = new Login { Username = "user", Password = "wrongpassword" };
            var mockResponse = new HttpResponseMessage(HttpStatusCode.Unauthorized);

            // Mock the SendAsync method
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync", 
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<System.Threading.CancellationToken>()
                )
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _serviceAuthController.LoginAsync(loginModel);

            // Assert
            Assert.AreEqual("Unauthorized: Invalid credentials", result);
        }

        [Test]
        public async Task LoginAsync_NotFound_ReturnsNotFoundMessage()
        {
            // Arrange
            var loginModel = new Login { Username = "user", Password = "password" };
            var mockResponse = new HttpResponseMessage(HttpStatusCode.NotFound);

            // Mock the SendAsync method
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync", 
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<System.Threading.CancellationToken>()
                )
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _serviceAuthController.LoginAsync(loginModel);

            // Assert
            Assert.AreEqual("404 Not Found: Login endpoint not found", result);
        }

        [Test]
        public async Task LoginAsync_Exception_ReturnsErrorMessage()
        {
            // Arrange
            var loginModel = new Login { Username = "user", Password = "password" };

            // Mock the SendAsync method to throw an exception
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync", 
                    ItExpr.IsAny<HttpRequestMessage>(), 
                    ItExpr.IsAny<System.Threading.CancellationToken>()
                )
                .ThrowsAsync(new System.Exception("Some error occurred"));

            // Act
            var result = await _serviceAuthController.LoginAsync(loginModel);

            // Assert
            Assert.AreEqual("Exception: Some error occurred", result);
        }
        
        [Test]
        public async Task LoginAsync_ApiCallFails_ReturnsErrorMessage()
        {
            // Arrange
            var loginModel = new Login { Username = "user", Password = "password" };

            // Create a mock response with a non-success status code (e.g., BadRequest) and a custom ReasonPhrase
            var mockResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                ReasonPhrase = "Bad Request Error"
            };

            // Mock the SendAsync method to return the mock response
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync", 
                    ItExpr.IsAny<HttpRequestMessage>(), 
                    ItExpr.IsAny<System.Threading.CancellationToken>()
                )
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _serviceAuthController.LoginAsync(loginModel);

            // Assert
            // Assert that the error message returned contains the ReasonPhrase from the mock response
            Assert.AreEqual("Error logging in: Bad Request Error", result);
        }
        
        
}
    
