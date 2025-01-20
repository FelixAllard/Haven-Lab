using System.Net;
using System.Text;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;
using Api_Gateway.Services;
using ShopifySharp;

namespace TestingProject.Api_Gateway.Services
{
    [TestFixture]
    public class ServiceDraftOrderControllerTest
    {
        private Mock<IHttpClientFactory> _mockHttpClientFactory;
        private ServiceDraftOrderController _serviceDraftOrderController;

        [SetUp]
        public void SetUp()
        {
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        }

        [Test]
        public async Task PostDraftOrder_ReturnsSuccess_WhenApiCallIsSuccessful()
        {
            // Arrange: Mock HttpClient behavior
            var draftOrder = new DraftOrder { Id = 1, Name = "Test Order" };  // Example draft order
            var mockResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"id\": 1, \"name\": \"Test Order\"}")
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

            _serviceDraftOrderController = new ServiceDraftOrderController(_mockHttpClientFactory.Object);

            // Act: Call the method
            var result = await _serviceDraftOrderController.PostDraftOrder(draftOrder);

            // Assert: Validate the result
            Assert.AreEqual((int)HttpStatusCode.OK, result.StatusCode);
            Assert.AreEqual("{\"id\": 1, \"name\": \"Test Order\"}", result.Content);
        }

        [Test]
        public async Task PostDraftOrder_ReturnsError_WhenApiCallFails()
        {
            // Arrange: Mock HttpClient to return an error response
            var draftOrder = new DraftOrder { Id = 1, Name = "Test Order" };
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

            _serviceDraftOrderController = new ServiceDraftOrderController(_mockHttpClientFactory.Object);

            // Act: Call the method
            var result = await _serviceDraftOrderController.PostDraftOrder(draftOrder);

            // Assert: Validate the error message
            Assert.AreEqual((int)HttpStatusCode.InternalServerError, result.StatusCode);
            Assert.AreEqual("Error: Internal Server Error", result.Content);
        }

        [Test]
        public async Task PostDraftOrder_ReturnsExceptionMessage_WhenAnExceptionOccurs()
        {
            // Arrange: Mock HttpClient to throw an exception
            var draftOrder = new DraftOrder { Id = 1, Name = "Test Order" };

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

            _serviceDraftOrderController = new ServiceDraftOrderController(_mockHttpClientFactory.Object);

            // Act: Call the method
            var result = await _serviceDraftOrderController.PostDraftOrder(draftOrder);

            // Assert: Validate the exception message
            Assert.AreEqual(503, result.StatusCode);
            Assert.AreEqual("Error: Test Exception", result.Content);
        }
        
    }
    
}
