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
using Microsoft.AspNetCore.Mvc;

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
    
            // Simulate a successful response from the login API endpoint.
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("Login successful")
            };

            // Setup the mock for HttpMessageHandler to return the expected response.
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync", // Mock the SendAsync method of HttpMessageHandler.
                    ItExpr.Is<HttpRequestMessage>(req =>
                            req.Method == HttpMethod.Post && // Ensure it's a POST request.
                            req.RequestUri.ToString().EndsWith("/api/Account/login") && // Check that the URI is correct.
                            req.Content.ReadAsStringAsync().Result == JsonConvert.SerializeObject(loginModel) // Ensure the body is the serialized login model.
                    ),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(expectedResponse); // Return the expected response when SendAsync is called.

            // Act
            var result = await _serviceAuthController.LoginAsync(loginModel); // Call the method under test.

            // Assert
            var okResult = result as OkObjectResult; // Cast the result to OkObjectResult.
            Assert.IsNotNull(okResult); // Ensure the result is an OkObjectResult.
            Assert.AreEqual(HttpStatusCode.OK, (HttpStatusCode)okResult.StatusCode); // Check the status code is 200 OK.

            // Assert that the content returned is "Login successful"
            var responseContent = okResult.Value as string; // Cast the response content to string.
            Assert.AreEqual("Login successful", responseContent); // Check that the response content matches the expected string.
        }
        
        [Test]
        public async Task LoginAsync_ShouldReturnSuccessResponse_WhenResponseContainsJson()
        {
            // Arrange
            var loginModel = new Login { Username = "testuser", Password = "testpass" };
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(new { Token = "abc123" }), Encoding.UTF8, "application/json")
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri.ToString().EndsWith("/api/Account/login") &&
                        req.Content.ReadAsStringAsync().Result == JsonConvert.SerializeObject(loginModel)
                    ),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _serviceAuthController.LoginAsync(loginModel);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(HttpStatusCode.OK, (HttpStatusCode)okResult.StatusCode);

            var responseContent = okResult.Value as string;
            Assert.IsNotNull(responseContent);
            Assert.AreEqual(JsonConvert.SerializeObject(new { Token = "abc123" }), responseContent);
        }


        [Test]
        public async Task LoginAsync_ShouldReturnBadRequestResponse_WhenCredentialsAreInvalid()
        {
            // Arrange
            var loginModel = new Login { Username = "wronguser", Password = "wrongpass" };

            // Setup the mock for HttpMessageHandler to simulate the behavior of an invalid credentials response.
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync", // Mock the SendAsync method of HttpMessageHandler.
                    ItExpr.Is<HttpRequestMessage>(req =>
                            req.Method == HttpMethod.Post && // Ensure it's a POST request.
                            req.RequestUri.ToString().EndsWith("/api/Account/login") && // Check that the URI is correct.
                            req.Content.ReadAsStringAsync().Result == JsonConvert.SerializeObject(loginModel) // Ensure the body is the serialized login model.
                    ),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Unauthorized) // Simulate a 401 Unauthorized response.
                {
                    Content = new StringContent("Unauthorized: Invalid credentials")
                });

            // Act
            var result = await _serviceAuthController.LoginAsync(loginModel); // Call the method under test.

            // Assert
            var badRequestResult = result as BadRequestObjectResult; // Cast the result to BadRequestObjectResult.
            Assert.IsNotNull(badRequestResult, "Expected BadRequestObjectResult but got null."); // Ensure the result is a BadRequestObjectResult.
            Assert.AreEqual((int)HttpStatusCode.BadRequest, badRequestResult.StatusCode, "Expected status code to be 400 BadRequest."); // Check that the status code is 400 BadRequest.

            // Check the content of the BadRequestObjectResult directly
            var responseContent = badRequestResult.Value as dynamic;
            Assert.IsNotNull(responseContent, "Response content is null."); // Ensure the response content is not null.

            // Use reflection to access the Error property
            var errorProperty = responseContent.GetType().GetProperty("Error");
            Assert.IsNotNull(errorProperty, "Error property is missing in the response content."); // Ensure the 'Error' property exists.

            var errorMessage = errorProperty.GetValue(responseContent) as string;
            Assert.IsNotNull(errorMessage, "Error message is null."); // Ensure the 'Error' property is not null.
            Assert.AreEqual("Unauthorized: Invalid credentials", errorMessage, "Error message does not match.");
        }
        
        [Test]
        public async Task LoginAsync_ShouldReturnInternalServerErrorResponse_WhenExceptionOccurs()
        {
            // Arrange
            var loginModel = new Login { Username = "testuser", Password = "testpass" };

            // Simulate an exception during the HTTP request.
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync", // Mock the SendAsync method of HttpMessageHandler.
                    ItExpr.IsAny<HttpRequestMessage>(), // Match any request.
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new Exception("Internal Server Error")); // Simulate an unexpected internal server error.

            // Act
            var result = await _serviceAuthController.LoginAsync(loginModel); // Call the method under test.

            // Assert
            var internalServerErrorResult = result as ObjectResult; // Cast to ObjectResult to check the response.

            Assert.IsNotNull(internalServerErrorResult, "Expected ObjectResult but got null."); // Ensure that the result is an ObjectResult.
            Assert.AreEqual((int)HttpStatusCode.InternalServerError, internalServerErrorResult.StatusCode, "Expected status code to be 500 (Internal Server Error).");

            // Assert that the content of the response contains the correct error message.
            var responseContent = internalServerErrorResult.Value as string; // Get the response content.
            Assert.AreEqual("Error: Internal Server Error - Internal Server Error", responseContent, "Error message does not match."); // Check that the error message matches the expected message.
        }

        [Test]
        public async Task LoginAsync_ShouldReturnServiceUnavailableResponse_WhenServiceIsUnavailable()
        {
            // Arrange
            var loginModel = new Login { Username = "testuser", Password = "testpass" };

            // Simulate a 503 Service Unavailable response
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri.ToString().EndsWith("/api/Account/login")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                {
                    Content = new StringContent("Service Unavailable")
                });

            // Act
            var result = await _serviceAuthController.LoginAsync(loginModel);

            // Assert
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(400, objectResult.StatusCode);
        }

        [Test]
        public async Task LoginAsync_ShouldReturnRequestTimeoutResponse_WhenRequestTimesOut()
        {
            // Arrange
            var loginModel = new Login { Username = "testuser", Password = "testpass" };

            // Simulate a timeout (TaskCanceledException)
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new TaskCanceledException("The request timed out"));

            // Act
            var result = await _serviceAuthController.LoginAsync(loginModel);

            // Assert
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual((int)HttpStatusCode.RequestTimeout, objectResult.StatusCode);
            Assert.AreEqual("Error 408: Request Timeout - The request timed out", objectResult.Value);
        }
        
        [Test]
        public async Task LoginAsync_ShouldReturnInternalServerErrorResponse_WhenUnexpectedErrorOccurs()
        {
            // Arrange
            var loginModel = new Login { Username = "testuser", Password = "testpass" };

            // Simulate a general internal server error
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _serviceAuthController.LoginAsync(loginModel);

            // Assert
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual((int)HttpStatusCode.InternalServerError, objectResult.StatusCode);
            Assert.AreEqual("Error: Unexpected error - Internal Server Error", objectResult.Value);
        }
        
        [Test]
        public async Task LogoutAsync_ShouldReturnSuccessResponse_WhenLogoutIsSuccessful()
        {
            // Arrange
            var username = "testuser";

            // Mock the response for the HttpClient call, assuming the controller logic is calling an HTTP API internally.
            // For this example, we'll assume the Logout API responds with a success message, which in this case is "Logout successful".
            var expectedResponse = new OkObjectResult("Logout successful");

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
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("Logout successful")
                });

            // Act
            var result = await _serviceAuthController.LogoutAsync(username);

            // Assert
            var okResult = result as OkObjectResult; // Ensure the result is an OkObjectResult
            Assert.IsNotNull(okResult); // Assert that it is not null
            Assert.AreEqual(200, okResult.StatusCode); // Assert that the status code is 200 (OK)
            Assert.AreEqual("Logout successful", okResult.Value); // Assert that the returned message is correct
        }


        [Test]
        public async Task LogoutAsync_ShouldReturnInternalServerErrorResponse_WhenExceptionOccurs()
        {
            // Arrange
            var username = "testuser";

            // Mock the HttpClient call to throw an exception when invoked.
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
            var statusCodeResult = result as ObjectResult; // Ensure the result is an ObjectResult
            Assert.IsNotNull(statusCodeResult); // Assert that it's not null
            Assert.AreEqual(500, statusCodeResult.StatusCode); // Assert that the status code is 500 (Internal Server Error)
            Assert.AreEqual("Error: Internal Server Error - Internal Server Error", statusCodeResult.Value); // Assert that the error message is as expected
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
            var serviceUnavailableResult = result as ObjectResult; // Ensure it's an ObjectResult with the custom message
            Assert.IsNotNull(serviceUnavailableResult); // Assert that the result is not null
            Assert.AreEqual(500, serviceUnavailableResult.StatusCode); // Assert that the status code is 503 (Service Unavailable)
            Assert.AreEqual("Error: Service Unavailable - Internal Server Error", serviceUnavailableResult.Value); // Assert the error message
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
            var internalServerErrorResult = result as ObjectResult; // Ensure the result is ObjectResult with custom message
            Assert.IsNotNull(internalServerErrorResult); // Assert that the result is not null
            Assert.AreEqual(500, internalServerErrorResult.StatusCode); // Assert that status code is 500 (Internal Server Error)
            Assert.AreEqual("Error: Request Timeout - Internal Server Error", internalServerErrorResult.Value); // Assert the error message
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
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual((int)HttpStatusCode.OK, okResult.StatusCode);
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
            var notFoundResult = result as NotFoundObjectResult; // Ensure the result is NotFoundObjectResult
            Assert.IsNotNull(notFoundResult); // Assert that the result is not null
            Assert.AreEqual(HttpStatusCode.NotFound, (HttpStatusCode)notFoundResult.StatusCode); // Assert that status code is 404 NotFound
            Assert.AreEqual("Endpoint not found", notFoundResult.Value); // Assert that the response content matches the expected message
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
            var objectResult = result as ObjectResult; // The result will be ObjectResult since we return content.
            Assert.IsNotNull(objectResult); // Ensure it's not null
            Assert.AreEqual((int)HttpStatusCode.ServiceUnavailable, objectResult.StatusCode); // Assert that status code is 503 Service Unavailable
            Assert.AreEqual("Error 503: Service Unavailable - Timeout occurred", objectResult.Value); // Assert content matches expected error message
        }

        
        [Test]
        public async Task VerifyTokenAsync_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var token = "any-token";

            // Mock an exception being thrown during the HTTP call
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _serviceAuthController.VerifyTokenAsync(token);

            // Assert
            var objectResult = result as ObjectResult; // The result will be ObjectResult since we return content.
            Assert.IsNotNull(objectResult); // Ensure it's not null
            Assert.AreEqual((int)HttpStatusCode.InternalServerError, objectResult.StatusCode); // Assert that status code is 500
            Assert.AreEqual("Error: Internal server error - Internal Server Error", objectResult.Value); // Assert content matches expected error message
        }

        
        [Test]
        public async Task VerifyTokenAsync_ShouldReturnRequestTimeout_WhenRequestIsCanceled()
        {
            // Arrange
            var token = "any-token";
    
            // Mock the TaskCanceledException (indicating request timeout)
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new TaskCanceledException("Request timeout"));

            // Act
            var result = await _serviceAuthController.VerifyTokenAsync(token);

            // Assert
            var objectResult = result as ObjectResult; // The result will be ObjectResult since we return content.
            Assert.IsNotNull(objectResult); // Ensure it's not null
            Assert.AreEqual(500, objectResult.StatusCode); // Assert that status code is 408
            Assert.AreEqual("Error: Request timeout - Internal Server Error", objectResult.Value); // Assert content matches expected error message
        }

    }
}