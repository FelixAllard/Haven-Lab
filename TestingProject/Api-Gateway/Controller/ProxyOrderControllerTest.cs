using Api_Gateway.Controller;
using Api_Gateway.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json.Linq;
using ShopifySharp;

namespace TestingProject.Api_Gateway.Controller;

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
            _mockServiceOrderController.Setup(service => service.GetAllOrdersAsync(null))
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
            _mockServiceOrderController.Setup(service => service.GetAllOrdersAsync(null))
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
            _mockServiceOrderController.Setup(service => service.GetAllOrdersAsync(null))
                                       .ThrowsAsync(new System.Exception("Test Exception"));

            // Act: Call the controller method
            var result = await _controller.GetAllOrders();

            // Assert: Verify the result is StatusCodeResult with 500
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult); // Ensure the result is of type ObjectResult
            Assert.AreEqual(500, objectResult.StatusCode); // Ensure status code is 500
            Assert.IsNotNull(objectResult.Value); // Ensure there is a message in the respons
        }
        
        [Test]
        public async Task GetOrderById_ReturnsOkResult_WhenOrderExists()
        {
            // Arrange: Mock the service to return a valid order
            var mockResult = "{\"id\":1,\"name\":\"Order1\"}";
            _mockServiceOrderController.Setup(service => service.GetOrderByIdAsync(1))
                .ReturnsAsync(mockResult);

            // Act: Call the controller method
            var result = await _controller.GetOrderById(1);

            // Assert: Verify the result is OkObjectResult with correct data
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult); // Ensure the result is of type OkObjectResult
            Assert.That(okResult.Value, Is.EqualTo(mockResult)); // The content should match the expected result
            Assert.That(okResult.StatusCode, Is.EqualTo(200)); // The status code should be 200
        }
        

        [Test]
        public async Task GetOrderById_ReturnsNotFound_WhenOrderDoesNotExist()
        {
            // Arrange: Mock the service to return null (order not found)
            _mockServiceOrderController.Setup(service => service.GetOrderByIdAsync(999))
                .ReturnsAsync((string)null);

            // Act: Call the controller method
            var result = await _controller.GetOrderById(999);

            // Assert: Verify the result is NotFound with the correct message
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult); // Ensure the result is of type NotFoundObjectResult

            // Use JObject to parse the value and extract the Message property
            var message = JObject.FromObject(notFoundResult.Value)["Message"].ToString();
            Assert.AreEqual("Order with ID 999 not found", message); // Compare the message value

            Assert.That(notFoundResult.StatusCode, Is.EqualTo(404)); // The status code should be 404
        }

        
        [Test]
        public async Task GetOrderById_ReturnsInternalServerError_WhenServiceReturnsError()
        {
            // Arrange: Mock the service to return an error message
            var mockError = "Error fetching order with ID 1: Some error occurred";
            _mockServiceOrderController.Setup(service => service.GetOrderByIdAsync(1))
                .ReturnsAsync(mockError);

            // Act: Call the controller method
            var result = await _controller.GetOrderById(1);

            // Assert: Verify the result is StatusCodeResult with 500
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult); // Ensure the result is of type ObjectResult
            Assert.AreEqual(500, objectResult.StatusCode); // Ensure status code is 500
            Assert.IsNotNull(objectResult.Value); // Ensure there is a message in the response
        }

        [Test]
        public async Task GetOrderById_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange: Mock the service to throw an exception
            _mockServiceOrderController.Setup(service => service.GetOrderByIdAsync(1))
                .ThrowsAsync(new System.Exception("Test Exception"));

            // Act: Call the controller method
            var result = await _controller.GetOrderById(1);

            // Assert: Verify the result is StatusCodeResult with 500
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult); // Ensure the result is of type ObjectResult
            Assert.AreEqual(500, objectResult.StatusCode); // Ensure status code is 500
            Assert.IsNotNull(objectResult.Value); // Ensure there is a message in the response
        }
        
        [Test]
        public async Task PutProduct_ReturnsOkResult_WhenUpdateIsSuccessful()
        {
            // Arrange
            long orderId = 1;
            var order = new Order
            {
                Id = 1,
                AppId = 1234567,
                Name = "Test Order"
            };
            var mockResponse = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent("{\"id\":1,\"name\":\"Test Order\"}")
            };

            _mockServiceOrderController
                .Setup(service => service.PutOrderAsync(orderId, order))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.PutOrder(orderId, order);

            // Assert
            var okResult = result as ObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value.ToString(), Does.Contain("Test Order"));
        }

        [Test]
        public async Task PutProduct_ReturnsServiceUnavailable_WhenServiceIsUnavailable()
        {
            // Arrange
            long orderId = 1;
            var order = new Order
            {
                Id = 1,
                AppId = 1234567,
                Name = "Test Order"
            };
            var mockResponse = new HttpResponseMessage(System.Net.HttpStatusCode.ServiceUnavailable);

            _mockServiceOrderController
                .Setup(service => service.PutOrderAsync(orderId, order))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.PutOrder(orderId, order);

            // Assert
            var objectResult = result as ObjectResult; 
            Assert.IsNotNull(objectResult); 
            Assert.That(objectResult.StatusCode, Is.EqualTo(503)); 
        
            var value = JObject.FromObject(objectResult.Value); 
            Assert.AreEqual("Service is currently unavailable, please try again later.", value["message"]?.ToString());
        }
        
        [Test]
        public async Task PutProduct_ReturnsServerError_WhenUnexpectedExceptionIsThrown()
        {
            // Arrange
            long orderId = 1;
            var order = new Order
            {
                Id = 1,
                AppId = 1234567,
                Name = "Test Order"
            };

            _mockServiceOrderController
                .Setup(service => service.PutOrderAsync(orderId, order))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.PutOrder(orderId, order);

            // Assert
            var objectResult = result as ObjectResult; 
            Assert.IsNotNull(objectResult); 
            Assert.That(objectResult.StatusCode, Is.EqualTo(500)); 
        
            var value = JObject.FromObject(objectResult.Value); 
            Assert.AreEqual("An error occurred", value["message"]?.ToString());
        
        }

    }
