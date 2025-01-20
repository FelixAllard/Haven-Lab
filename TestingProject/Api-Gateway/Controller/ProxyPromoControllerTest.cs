using System.Net;
using Api_Gateway.Controller;
using Api_Gateway.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json.Linq;
using ShopifySharp;

namespace TestingProject.Api_Gateway.Controller;

[TestFixture]
public class ProxyPromoControllerTests
{
    private Mock<ServicePromoController> _mockServicePromoController;
    private ProxyPromoController _controller;

    [SetUp]
    public void SetUp()
    {
        // Create a mock of the ServicePromoController
        _mockServicePromoController = new Mock<ServicePromoController>(null);

        // Inject the mock into the ProxyPromoController
        _controller = new ProxyPromoController(_mockServicePromoController.Object);
    }

    [Test]
    public async Task GetAllPriceRules_ReturnsOkResult_WhenServiceReturnsData()
    {
        // Arrange: Mock the service to return a valid result
        var mockResult = "[{\"id\":1,\"name\":\"PriceRule1\"},{\"id\":2,\"name\":\"PriceRule2\"}]";
        _mockServicePromoController.Setup(service => service.GetAllPriceRulesAsync())
                                   .ReturnsAsync(mockResult);

        // Act: Call the controller method
        var result = await _controller.GetAllPriceRules();

        // Assert: Verify the result is OkObjectResult with correct data
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult); // Ensure the result is of type OkObjectResult
        Assert.That(okResult.Value, Is.EqualTo(mockResult)); // The content should match the expected result
        Assert.That(okResult.StatusCode, Is.EqualTo(200)); // The status code should be 200
    }

    [Test]
    public async Task GetAllPriceRules_ReturnsInternalServerError_WhenServiceReturnsError()
    {
        // Arrange: Mock the service to return an error message
        var mockError = "Error fetching price rules: Some error occurred";
        _mockServicePromoController.Setup(service => service.GetAllPriceRulesAsync())
                                   .ReturnsAsync(mockError);

        // Act: Call the controller method
        var result = await _controller.GetAllPriceRules();

        // Assert: Verify the result is StatusCodeResult with 500
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult); // Ensure the result is of type ObjectResult
        Assert.AreEqual(500, objectResult.StatusCode); // Ensure status code is 500
        Assert.IsNotNull(objectResult.Value); // Ensure there is a message in the response
    }

    [Test]
    public async Task GetAllPriceRules_ReturnsInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange: Mock the service to throw an exception
        _mockServicePromoController.Setup(service => service.GetAllPriceRulesAsync())
                                   .ThrowsAsync(new System.Exception("Test Exception"));

        // Act: Call the controller method
        var result = await _controller.GetAllPriceRules();

        // Assert: Verify the result is StatusCodeResult with 500
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult); // Ensure the result is of type ObjectResult
        Assert.AreEqual(500, objectResult.StatusCode); // Ensure status code is 500
        Assert.IsNotNull(objectResult.Value); // Ensure there is a message in the response
    }
    
    [Test]
    public async Task GetPriceRuleById_ReturnsOkResult_WhenPriceRuleExists()
    {
        // Arrange: Mock the service to return a valid price rule
        var mockResult = "{\"id\":1,\"name\":\"PriceRule1\"}";
        _mockServicePromoController.Setup(service => service.GetPriceRuleByIdAsync(1))
            .ReturnsAsync(mockResult);

        // Act: Call the controller method
        var result = await _controller.GetPriceRuleById(1);

        // Assert: Verify the result is OkObjectResult with correct data
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult); // Ensure the result is of type OkObjectResult
        Assert.That(okResult.Value, Is.EqualTo(mockResult)); // The content should match the expected result
        Assert.That(okResult.StatusCode, Is.EqualTo(200)); // The status code should be 200
    }

    [Test]
    public async Task GetPriceRuleById_ReturnsNotFound_WhenPriceRuleDoesNotExist()
    {
        // Arrange: Mock the service to return a "404 Not Found" message (price rule not found)
        _mockServicePromoController.Setup(service => service.GetPriceRuleByIdAsync(999))
            .ReturnsAsync("404 Not Found: Price rule not found");

        // Act: Call the controller method
        var result = await _controller.GetPriceRuleById(999);

        // Assert: Verify the result is NotFound with the correct message
        var notFoundResult = result as NotFoundObjectResult;
        Assert.IsNotNull(notFoundResult); // Ensure the result is of type NotFoundObjectResult
        
        Assert.That(notFoundResult.StatusCode, Is.EqualTo(404)); // The status code should be 404
    }

    [Test]
    public async Task GetPriceRuleById_ReturnsInternalServerError_WhenServiceReturnsError()
    {
        // Arrange: Mock the service to return an error message
        var mockError = "Error fetching price rule with ID 1: Some error occurred";
        _mockServicePromoController.Setup(service => service.GetPriceRuleByIdAsync(1))
            .ReturnsAsync(mockError);

        // Act: Call the controller method
        var result = await _controller.GetPriceRuleById(1);

        // Assert: Verify the result is StatusCodeResult with 500
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult); // Ensure the result is of type ObjectResult
        Assert.AreEqual(500, objectResult.StatusCode); // Ensure status code is 500
        Assert.IsNotNull(objectResult.Value); // Ensure there is a message in the response
    }

    [Test]
    public async Task GetPriceRuleById_ReturnsInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange: Mock the service to throw an exception
        _mockServicePromoController.Setup(service => service.GetPriceRuleByIdAsync(1))
            .ThrowsAsync(new System.Exception("Test Exception"));

        // Act: Call the controller method
        var result = await _controller.GetPriceRuleById(1);

        // Assert: Verify the result is StatusCodeResult with 500
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult); // Ensure the result is of type ObjectResult
        Assert.AreEqual(500, objectResult.StatusCode); // Ensure status code is 500
        Assert.IsNotNull(objectResult.Value); // Ensure there is a message in the response
    }
    
    [Test]
    public async Task DeletePriceRule_ReturnsOkResult_WhenPriceRuleDeleted()
    {
        // Arrange: Mock the service to return a success message
        var mockResult = "Price rule deleted successfully.";
        _mockServicePromoController.Setup(service => service.DeletePriceRuleAsync(1))
            .ReturnsAsync(mockResult);

        // Act: Call the controller method
        var result = await _controller.DeletePriceRule(1);

        // Assert: Verify the result is OkObjectResult with correct data
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult); // Ensure the result is of type OkObjectResult
        Assert.That(okResult.Value, Is.EqualTo(mockResult)); // The content should match the expected result
        Assert.That(okResult.StatusCode, Is.EqualTo(200)); // The status code should be 200
    }

    [Test]
    public async Task DeletePriceRule_ReturnsNotFound_WhenPriceRuleDoesNotExist()
    {
        // Arrange: Mock the service to return a "404 Not Found" message (price rule not found)
        _mockServicePromoController.Setup(service => service.DeletePriceRuleAsync(999))
            .ReturnsAsync("404 Not Found: Price rule not found");

        // Act: Call the controller method
        var result = await _controller.DeletePriceRule(999);

        // Assert: Verify the result is NotFound with the correct message
        var notFoundResult = result as NotFoundObjectResult;
        Assert.IsNotNull(notFoundResult); // Ensure the result is of type NotFoundObjectResult

        Assert.That(notFoundResult.StatusCode, Is.EqualTo(404)); // The status code should be 404
    }

    [Test]
    public async Task DeletePriceRule_ReturnsInternalServerError_WhenServiceReturnsError()
    {
        // Arrange: Mock the service to return an error message
        var mockError = "Error deleting price rule with ID 1: Some error occurred";
        _mockServicePromoController.Setup(service => service.DeletePriceRuleAsync(1))
            .ReturnsAsync(mockError);

        // Act: Call the controller method
        var result = await _controller.DeletePriceRule(1);

        // Assert: Verify the result is StatusCodeResult with 500
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult); // Ensure the result is of type ObjectResult
        Assert.AreEqual(500, objectResult.StatusCode); // Ensure status code is 500
        Assert.IsNotNull(objectResult.Value); // Ensure there is a message in the response
    }

    [Test]
    public async Task DeletePriceRule_ReturnsInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange: Mock the service to throw an exception
        _mockServicePromoController.Setup(service => service.DeletePriceRuleAsync(1))
            .ThrowsAsync(new System.Exception("Test Exception"));

        // Act: Call the controller method
        var result = await _controller.DeletePriceRule(1);

        // Assert: Verify the result is StatusCodeResult with 500
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult); // Ensure the result is of type ObjectResult
        Assert.AreEqual(500, objectResult.StatusCode); // Ensure status code is 500
        Assert.IsNotNull(objectResult.Value); // Ensure there is a message in the response
    }
    
    [Test]
    public async Task GetAllDiscountsByRule_ReturnsOkResult_WhenDiscountsExist()
    {
        // Arrange: Mock the service to return valid discounts
        var mockResult = "[{\"id\":1,\"name\":\"Discount1\"},{\"id\":2,\"name\":\"Discount2\"}]";
        _mockServicePromoController.Setup(service => service.GetAllDiscountsByRuleAsync(1))
            .ReturnsAsync(mockResult);

        // Act: Call the controller method
        var result = await _controller.GetAllDiscountsByRule(1);

        // Assert: Verify the result is OkObjectResult with correct data
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult); // Ensure the result is of type OkObjectResult
        Assert.That(okResult.Value, Is.EqualTo(mockResult)); // The content should match the expected result
        Assert.That(okResult.StatusCode, Is.EqualTo(200)); // The status code should be 200
    }

    [Test]
    public async Task GetAllDiscountsByRule_ReturnsInternalServerError_WhenServiceReturnsError()
    {
        // Arrange: Mock the service to return an error message
        var mockError = "Error fetching discounts for price rule with ID 1: Some error occurred";
        _mockServicePromoController.Setup(service => service.GetAllDiscountsByRuleAsync(1))
            .ReturnsAsync(mockError);

        // Act: Call the controller method
        var result = await _controller.GetAllDiscountsByRule(1);

        // Assert: Verify the result is StatusCodeResult with 500
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult); // Ensure the result is of type ObjectResult
        Assert.AreEqual(500, objectResult.StatusCode); // Ensure status code is 500
        Assert.IsNotNull(objectResult.Value); // Ensure there is a message in the response
    }

    [Test]
    public async Task GetAllDiscountsByRule_ReturnsInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange: Mock the service to throw an exception
        _mockServicePromoController.Setup(service => service.GetAllDiscountsByRuleAsync(1))
            .ThrowsAsync(new System.Exception("Test Exception"));

        // Act: Call the controller method
        var result = await _controller.GetAllDiscountsByRule(1);

        // Assert: Verify the result is StatusCodeResult with 500
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult); // Ensure the result is of type ObjectResult
        Assert.AreEqual(500, objectResult.StatusCode); // Ensure status code is 500
        Assert.IsNotNull(objectResult.Value); // Ensure there is a message in the response
    }
    
    
    [Test]
    public async Task CreateDiscount_ReturnsCreatedResponse_WhenDiscountIsCreated()
    {
        // Arrange: Mock a successful response from the service
        var mockDiscountCode = new PriceRuleDiscountCode { Code = "DISCOUNT10" };
        var mockResponse = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent("{\"id\":1,\"message\":\"Discount created successfully.\"}")
        };
        _mockServicePromoController.Setup(service => service.CreateDiscountAsync(1, mockDiscountCode))
            .ReturnsAsync(mockResponse);

        // Act: Call the controller method
        var result = await _controller.CreateDiscount(1, mockDiscountCode);

        // Assert: Verify the result is CreatedAtActionResult with the correct status code and content
        var createdResult = result as ObjectResult;
        Assert.IsNotNull(createdResult); // Ensure the result is of type ObjectResult
        Assert.AreEqual(201, createdResult.StatusCode); // The status code should be 201
        Assert.IsNotNull(createdResult.Value); // Ensure there is content in the response
    }

    [Test]
    public async Task CreateDiscount_ReturnsServiceUnavailable_WhenServiceIsUnavailable()
    {
        // Arrange: Mock the service to return a ServiceUnavailable status code
        var mockDiscountCode = new PriceRuleDiscountCode { Code = "DISCOUNT10"};
        var mockResponse = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
        {
            Content = new StringContent("Service is currently unavailable")
        };
        _mockServicePromoController.Setup(service => service.CreateDiscountAsync(1, mockDiscountCode))
            .ReturnsAsync(mockResponse);

        // Act: Call the controller method
        var result = await _controller.CreateDiscount(1, mockDiscountCode);

        // Assert: Verify the result is StatusCodeResult with 503
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult); // Ensure the result is of type ObjectResult
        Assert.AreEqual(503, objectResult.StatusCode); // The status code should be 503
        Assert.IsNotNull(objectResult.Value); // Ensure there is a message in the response
    }

    [Test]
    public async Task CreateDiscount_ReturnsInternalServerError_WhenServiceReturnsError()
    {
        // Arrange: Mock the service to return an error response
        var mockDiscountCode = new PriceRuleDiscountCode { Code = "DISCOUNT10"};
        var mockResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent("Error creating discount")
        };
        _mockServicePromoController.Setup(service => service.CreateDiscountAsync(1, mockDiscountCode))
            .ReturnsAsync(mockResponse);

        // Act: Call the controller method
        var result = await _controller.CreateDiscount(1, mockDiscountCode);

        // Assert: Verify the result is StatusCodeResult with 500
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult); // Ensure the result is of type ObjectResult
        Assert.AreEqual(500, objectResult.StatusCode); // Ensure status code is 500
        Assert.IsNotNull(objectResult.Value); // Ensure there is a message in the response
    }

    [Test]
    public async Task CreateDiscount_ReturnsInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange: Mock the service to throw an exception
        var mockDiscountCode = new PriceRuleDiscountCode { Code = "DISCOUNT10"};
        _mockServicePromoController.Setup(service => service.CreateDiscountAsync(1, mockDiscountCode))
            .ThrowsAsync(new System.Exception("Test Exception"));

        // Act: Call the controller method
        var result = await _controller.CreateDiscount(1, mockDiscountCode);

        // Assert: Verify the result is StatusCodeResult with 500
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult); // Ensure the result is of type ObjectResult
        Assert.AreEqual(500, objectResult.StatusCode); // Ensure status code is 500
        Assert.IsNotNull(objectResult.Value); // Ensure there is a message in the response
    }
    
    [Test]
    public async Task DeleteDiscount_ReturnsOkResponse_WhenDiscountIsDeleted()
    {
        // Arrange: Mock a successful response from the service
        var mockResult = "Discount deleted successfully."; // Use string directly if method expects string
        _mockServicePromoController.Setup(service => service.DeleteDiscountAsync(1, 1))
            .ReturnsAsync(mockResult); // Return string directly instead of HttpResponseMessage

        // Act: Call the controller method
        var result = await _controller.DeleteDiscount(1, 1);

        // Assert: Verify the result is OkObjectResult with correct status code
        var okResult = result as ObjectResult;
        Assert.IsNotNull(okResult); // Ensure the result is of type ObjectResult
        Assert.AreEqual(200, okResult.StatusCode); // The status code should be 200
        Assert.IsNotNull(okResult.Value); // Ensure there is content in the response
    }
    
    [Test]
    public async Task DeleteDiscount_ReturnsInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange: Mock the service to throw an exception
        _mockServicePromoController.Setup(service => service.DeleteDiscountAsync(1, 1))
            .ThrowsAsync(new System.Exception("Test Exception"));

        // Act: Call the controller method
        var result = await _controller.DeleteDiscount(1, 1);

        // Assert: Verify the result is StatusCodeResult with 500
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult); // Ensure the result is of type ObjectResult
        Assert.AreEqual(500, objectResult.StatusCode); // Ensure status code is 500
        Assert.IsNotNull(objectResult.Value); // Ensure there is a message in the response
    }
}

