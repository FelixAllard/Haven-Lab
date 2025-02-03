using System.Net;
using System.Text;
using Api_Gateway.Models;
using Api_Gateway.Services;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;

namespace TestingProject.Api_Gateway.Services
{
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
        public async Task LoginAsync_ShouldReturnSuccessResponse_WhenCredentialsAreValid()
        {
            // Arrange
            var loginModel = new Login { Username = "testuser", Password = "testpass" };
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("Login successful")
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri.ToString().EndsWith("/api/Account/login") &&
                        req.Content.ReadAsStringAsync().Result == JsonConvert.SerializeObject(loginModel)),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _serviceAuthController.LoginAsync(loginModel);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
            Assert.AreEqual("Login successful", await result.Content.ReadAsStringAsync());
        }

        [Test]
        public async Task LoginAsync_ShouldReturnUnauthorizedResponse_WhenCredentialsAreInvalid()
        {
            // Arrange
            var loginModel = new Login { Username = "wronguser", Password = "wrongpass" };
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                Content = new StringContent("Error 401: Unauthorized - Invalid credentials.")
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri.ToString().EndsWith("/api/Account/login") &&
                        req.Content.ReadAsStringAsync().Result == JsonConvert.SerializeObject(loginModel)),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _serviceAuthController.LoginAsync(loginModel);

            // Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
            Assert.AreEqual("Error 401: Unauthorized - Invalid credentials.", await result.Content.ReadAsStringAsync());
        }

        [Test]
        public async Task LoginAsync_ShouldReturnServiceUnavailableResponse_WhenServiceIsUnavailable()
        {
            // Arrange
            var loginModel = new Login { Username = "testuser", Password = "testpass" };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException("Service Unavailable"));

            // Act
            var result = await _serviceAuthController.LoginAsync(loginModel);

            // Assert
            Assert.AreEqual(HttpStatusCode.ServiceUnavailable, result.StatusCode);
            Assert.AreEqual("Error 503: Service Unavailable - Service Unavailable", await result.Content.ReadAsStringAsync());
        }

        [Test]
        public async Task LoginAsync_ShouldReturnRequestTimeoutResponse_WhenRequestTimesOut()
        {
            // Arrange
            var loginModel = new Login { Username = "testuser", Password = "testpass" };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new TaskCanceledException("Request Timeout"));

            // Act
            var result = await _serviceAuthController.LoginAsync(loginModel);

            // Assert
            Assert.AreEqual(HttpStatusCode.RequestTimeout, result.StatusCode);
            Assert.AreEqual("Error 408: Request Timeout - Request Timeout", await result.Content.ReadAsStringAsync());
        }

        [Test]
        public async Task LoginAsync_ShouldReturnInternalServerErrorResponse_WhenExceptionOccurs()
        {
            // Arrange
            var loginModel = new Login { Username = "testuser", Password = "testpass" };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new Exception("Internal Server Error"));

            // Act
            var result = await _serviceAuthController.LoginAsync(loginModel);

            // Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, result.StatusCode);
            Assert.AreEqual("Error 500: Internal Server Error - Internal Server Error", await result.Content.ReadAsStringAsync());
        }
        
        [Test]
        public async Task LogoutAsync_ShouldReturnSuccessResponse_WhenLogoutIsSuccessful()
        {
            // Arrange
            var username = "testuser";
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("Logout successful")
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri.ToString().EndsWith("/api/Account/logout") &&
                        req.Content.ReadAsStringAsync().Result == JsonConvert.SerializeObject(username)),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _serviceAuthController.LogoutAsync(username);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
            Assert.AreEqual("Logout successful", await result.Content.ReadAsStringAsync());
        }

        [Test]
        public async Task LogoutAsync_ShouldReturnInternalServerErrorResponse_WhenExceptionOccurs()
        {
            // Arrange
            var username = "testuser";

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new Exception("Internal Server Error"));

            // Act
            var result = await _serviceAuthController.LogoutAsync(username);

            // Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, result.StatusCode);
            Assert.AreEqual("Error 500: Internal Server Error - Internal Server Error", await result.Content.ReadAsStringAsync());
        }

        [Test]
        public async Task LogoutAsync_ShouldReturnBadRequestResponse_WhenUsernameIsInvalid()
        {
            // Arrange
            var username = ""; // Invalid username
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("Error 400: Bad Request - Invalid username")
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri.ToString().EndsWith("/api/Account/logout") &&
                        req.Content.ReadAsStringAsync().Result == JsonConvert.SerializeObject(username)),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _serviceAuthController.LogoutAsync(username);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.AreEqual("Error: Bad Request", await result.Content.ReadAsStringAsync());
        }

        [Test]
        public async Task LogoutAsync_ShouldReturnUnauthorizedResponse_WhenUserIsNotLoggedIn()
        {
            // Arrange
            var username = "testuser";
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                Content = new StringContent("Error 401: Unauthorized - User is not logged in")
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri.ToString().EndsWith("/api/Account/logout") &&
                        req.Content.ReadAsStringAsync().Result == JsonConvert.SerializeObject(username)),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _serviceAuthController.LogoutAsync(username);

            // Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
            Assert.AreEqual("Unauthorized: Invalid credentials or token", await result.Content.ReadAsStringAsync());
        }

        [Test]
        public async Task LogoutAsync_ShouldReturnServiceUnavailableResponse_WhenServiceIsUnavailable()
        {
            // Arrange
            var username = "testuser";

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException("Service Unavailable"));

            // Act
            var result = await _serviceAuthController.LogoutAsync(username);

            // Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, result.StatusCode);
            Assert.AreEqual("Error 500: Internal Server Error - Service Unavailable", await result.Content.ReadAsStringAsync());
        }

        [Test]
        public async Task LogoutAsync_ShouldReturnRequestTimesOutErrorResponse_WhenRequestTimesOut()
        {
            // Arrange
            var username = "testuser";

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new TaskCanceledException("Request Timeout"));

            // Act
            var result = await _serviceAuthController.LogoutAsync(username);

            // Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, result.StatusCode);
            Assert.AreEqual("Error 500: Internal Server Error - Request Timeout", await result.Content.ReadAsStringAsync());
        }
        
        [Test]
        public async Task VerifyTokenAsync_ShouldReturnOk_WhenResponseIsSuccessful()
        {
            // Arrange
            var token = "valid-token";
            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{ \"status\": \"valid\" }")
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(mockResponse)
                .Verifiable();

            // Act
            var result = await _serviceAuthController.VerifyTokenAsync(token);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
            Assert.AreEqual("{ \"status\": \"valid\" }", await result.Content.ReadAsStringAsync());
        }
        
        [Test]
        public async Task VerifyTokenAsync_ShouldReturnUnauthorized_WhenTokenIsInvalid()
        {
            // Arrange
            var token = "invalid-token";
            var mockResponse = new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                Content = new StringContent("Unauthorized: Invalid credentials or token")
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(mockResponse)
                .Verifiable();

            // Act
            var result = await _serviceAuthController.VerifyTokenAsync(token);

            // Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
            Assert.AreEqual("Unauthorized: Invalid credentials or token", await result.Content.ReadAsStringAsync());
        }

        [Test]
        public async Task VerifyTokenAsync_ShouldReturnNotFound_WhenEndpointIsNotFound()
        {
            // Arrange
            var token = "any-token";
            var mockResponse = new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("404 Not Found: Endpoint not found")
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(mockResponse)
                .Verifiable();

            // Act
            var result = await _serviceAuthController.VerifyTokenAsync(token);

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
            Assert.AreEqual("404 Not Found: Endpoint not found", await result.Content.ReadAsStringAsync());
        }

        [Test]
        public async Task VerifyTokenAsync_ShouldReturnServiceUnavailable_WhenServiceIsDown()
        {
            // Arrange
            var token = "any-token";
            var mockResponse = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
            {
                Content = new StringContent("Error 503: Service Unavailable - Service Unavailable")
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(mockResponse)
                .Verifiable();

            // Act
            var result = await _serviceAuthController.VerifyTokenAsync(token);

            // Assert
            Assert.AreEqual(HttpStatusCode.ServiceUnavailable, result.StatusCode);
            Assert.AreEqual("Error 503: Service Unavailable - Service Unavailable", await result.Content.ReadAsStringAsync());
        }
        
        [Test]
        public async Task VerifyTokenAsync_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var token = "any-token";
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _serviceAuthController.VerifyTokenAsync(token);

            // Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, result.StatusCode);
            Assert.AreEqual("Error 500: Internal Server Error - Internal server error", await result.Content.ReadAsStringAsync());
        }
        
        [Test]
        public async Task VerifyTokenAsync_ShouldReturnRequestTimeout_WhenRequestIsCanceled()
        {
            // Arrange
            var token = "any-token";
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new TaskCanceledException("Request timeout"));

            // Act
            var result = await _serviceAuthController.VerifyTokenAsync(token);

            // Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, result.StatusCode);
            Assert.AreEqual("Error 500: Internal Server Error - Request timeout", await result.Content.ReadAsStringAsync());
        }


    }
}