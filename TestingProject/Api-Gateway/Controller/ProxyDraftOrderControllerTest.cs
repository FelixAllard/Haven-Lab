using System.Net;
using System.Text;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Api_Gateway.Controller;
using Api_Gateway.Services;
using ShopifySharp;
using Microsoft.AspNetCore.Mvc;

namespace TestingProject.Api_Gateway.Controller
{
    [TestFixture]
    public class ProxyDraftOrderControllerTests
    {
        private Mock<ServiceDraftOrderController> _mockServiceDraftOrderController;
        private ProxyDraftOrderController _proxyDraftOrderController;

        [SetUp]
        public void SetUp()
        {
            // Mock the ServiceDraftOrderController
            _mockServiceDraftOrderController = new Mock<ServiceDraftOrderController>(Mock.Of<IHttpClientFactory>());
            _proxyDraftOrderController = new ProxyDraftOrderController(_mockServiceDraftOrderController.Object);
        }

        [Test]
        public async Task PostDraftOrder_ReturnsOkResult_WhenApiCallIsSuccessful()
        {
            // Arrange
            var draftOrder = new DraftOrder { Id = 1, Name = "Test Order" };
            var expectedResponse = "{\"id\":1,\"name\":\"Test Order\"}"; // Expected response as a JSON string

            // Mock the successful response from ServiceDraftOrderController
            _mockServiceDraftOrderController
                .Setup(service => service.PostDraftOrder(It.IsAny<DraftOrder>()))
                .ReturnsAsync((200, expectedResponse));  // Return tuple with status code and string response

            // Act
            var result = await _proxyDraftOrderController.PostDraftOrder(draftOrder);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult, "Expected OkObjectResult");
            Assert.AreEqual(200, okResult.StatusCode);

            // Assert that the response content matches the expected string
            Assert.AreEqual(expectedResponse, okResult.Value);
        }



        [Test]
        public async Task PostDraftOrder_ReturnsErrorResult_WhenApiCallFails()
        {
            // Arrange
            var draftOrder = new DraftOrder { Id = 1, Name = "Test Order" };
            var errorMessage = "Error: Invalid Data";

            // Mock the failed response from ServiceDraftOrderController
            _mockServiceDraftOrderController
                .Setup(service => service.PostDraftOrder(It.IsAny<DraftOrder>()))
                .ReturnsAsync((400, errorMessage));

            // Act
            var result = await _proxyDraftOrderController.PostDraftOrder(draftOrder);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.IsNotNull(statusCodeResult, "Expected ObjectResult");
            Assert.AreEqual(400, statusCodeResult.StatusCode);
            Assert.AreEqual("{ Message = Error: Invalid Data }", statusCodeResult.Value.ToString());
        }

        [Test]
        public async Task PostDraftOrder_ReturnsExceptionResult_WhenExceptionOccurs()
        {
            // Arrange
            var draftOrder = new DraftOrder { Id = 1, Name = "Test Order" };
            var exceptionMessage = "Test Exception";

            // Mock the exception from ServiceDraftOrderController
            _mockServiceDraftOrderController
                .Setup(service => service.PostDraftOrder(It.IsAny<DraftOrder>()))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _proxyDraftOrderController.PostDraftOrder(draftOrder);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.IsNotNull(statusCodeResult, "Expected ObjectResult");
            Assert.AreEqual(503, statusCodeResult.StatusCode);
            Assert.AreEqual("{ Message = Test Exception }", statusCodeResult.Value.ToString());
        }
    }
}
