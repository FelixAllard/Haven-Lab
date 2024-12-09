using Api_Gateway.Controller;
using Api_Gateway.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace TestingProject.Controller;

[TestFixture]
    public class ProxyOrderControllerTests
    {
        private Mock<ServiceOrderController> _mockServiceOrderController;
        private ProxyOrderController _controller;

        [SetUp]
        public void SetUp()
        {
            // Create a mock of the ServiceOrderController
            _mockServiceOrderController = new Mock<ServiceOrderController>(null);

            // Inject the mock into the ProxyOrderController
            _controller = new ProxyOrderController(_mockServiceOrderController.Object);
        }

        [Test]
        public async Task GetAllOrders_ReturnsOkResult_WhenServiceReturnsData()
        {
            // Arrange: Mock the service to return a valid result
            var mockResult = "[{\"id\":1,\"name\":\"Order1\"},{\"id\":2,\"name\":\"Order2\"}]";
            _mockServiceOrderController.Setup(service => service.GetAllOrdersAsync())
                                       .ReturnsAsync(mockResult);

            // Act: Call the controller method
            var result = await _controller.GetAllOrders();

            // Assert: Verify the result is OkObjectResult with correct data
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult); // Ensure the result is of type OkObjectResult
            Assert.That(okResult.Value, Is.EqualTo(mockResult)); // The content should match the expected result
            Assert.That(okResult.StatusCode, Is.EqualTo(200)); // The status code should be 200
        }

        [Test]
        public async Task GetAllOrders_ReturnsInternalServerError_WhenServiceReturnsError()
        {
            // Arrange: Mock the service to return an error message
            var mockError = "Error fetching orders: Some error occurred";
            _mockServiceOrderController.Setup(service => service.GetAllOrdersAsync())
                                       .ReturnsAsync(mockError);

            // Act: Call the controller method
            var result = await _controller.GetAllOrders();

            // Assert: Verify the result is StatusCodeResult with 500
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult); // Ensure the result is of type ObjectResult
            Assert.AreEqual(500, objectResult.StatusCode); // Ensure status code is 500
            Assert.IsNotNull(objectResult.Value); // Ensure there is a message in the response
        }

        [Test]
        public async Task GetAllOrders_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange: Mock the service to throw an exception
            _mockServiceOrderController.Setup(service => service.GetAllOrdersAsync())
                                       .ThrowsAsync(new System.Exception("Test Exception"));

            // Act: Call the controller method
            var result = await _controller.GetAllOrders();

            // Assert: Verify the result is StatusCodeResult with 500
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult); // Ensure the result is of type ObjectResult
            Assert.AreEqual(500, objectResult.StatusCode); // Ensure status code is 500
            Assert.IsNotNull(objectResult.Value); // Ensure there is a message in the respons
        }
    }
