using System.Net;
using Api_Gateway.Models;
using Api_Gateway.Services;
using Moq;
using Moq.Protected;

namespace TestingProject.Api_Gateway.Services;


   
[TestFixture]
public class ServiceEmailApiControllerTest
{
    private Mock<IHttpClientFactory> _mockHttpClientFactory;
    private ServiceEmailApiController _serviceEmailApiController;

    [SetUp]
    public void SetUp()
    {
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
    }

     [Test]
        public async Task PostEmail_ReturnsSuccess_WhenApiCallIsSuccessful()
        {
            // Arrange: Mock HttpClient behavior
            var directEmailModel = new DirectEmailModel
            {
                // Setup your DirectEmailModel properties here
                // Example:
                EmailToSendTo = "test@example.com",
                EmailTitle = "Test Email",
                Body = "This is a test email."
            };

            var mockResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"status\":\"Success\"}")
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

            _serviceEmailApiController = new ServiceEmailApiController(_mockHttpClientFactory.Object);

            // Act: Call the method
            var result = await _serviceEmailApiController .PostEmail(directEmailModel);

            // Assert: Validate the result
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(result.Content, Is.EqualTo("{\"status\":\"Success\"}"));
        }

        [Test]
        public async Task PostEmail_ReturnsError_WhenApiCallFails()
        {
            // Arrange: Mock HttpClient behavior for failure case
            var directEmailModel = new DirectEmailModel
            {
                EmailToSendTo = "test@example.com",
                EmailTitle = "Test Email",
                Body = "This is a test email."
            };

            var mockResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("Error: Bad Request")
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

            _serviceEmailApiController = new ServiceEmailApiController(_mockHttpClientFactory.Object);

            // Act: Call the method
            var result = await _serviceEmailApiController.PostEmail(directEmailModel);

            // Assert: Validate the result
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
            Assert.That(result.Content, Does.Contain("Error: Bad Request"));

        }

        [Test]
        public async Task PostEmail_ReturnsServiceUnavailable_WhenExceptionOccurs()
        {
            // Arrange: Mock HttpClient behavior to simulate an exception
            var directEmailModel = new DirectEmailModel
            {
                EmailToSendTo = "test@example.com",
                EmailTitle = "Test Email",
                Body = "This is a test email."
            };

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Network Error"));

            var client = new HttpClient(handlerMock.Object);
            _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);

            _serviceEmailApiController = new ServiceEmailApiController(_mockHttpClientFactory.Object);

            // Act: Call the method
            var result = await _serviceEmailApiController.PostEmail(directEmailModel);

            // Assert: Validate the result
            Assert.That(result.StatusCode, Is.EqualTo(503));
            Assert.That(result.Content, Is.EqualTo("Error: Network Error"));
        }

    
}
