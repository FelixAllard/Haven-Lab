using Api_Gateway.Controller;
using Api_Gateway.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace TestingProject.Api_Gateway.Controller;

[TestFixture]
public class ProxyProductControllerTest
{
    private Mock<IHttpClientFactory> _mockHttpClientFactory; // Mock any dependencies
    private Mock<ServiceProductController> _mockServiceProductController; // Mock ServiceProductController
    private ProxyProductController _proxyProductController; // Controller under test

    [SetUp]
    public void SetUp()
    {
        // Mock IHttpClientFactory as a dependency for ServiceProductController
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();

        // Mock ServiceProductController
        _mockServiceProductController = new Mock<ServiceProductController>(_mockHttpClientFactory.Object);

        // Create the ProxyProductController, passing the mocked ServiceProductController
        _proxyProductController = new ProxyProductController(_mockServiceProductController.Object);
    }

    [Test]
    public async Task GetAllProducts_ReturnsCorrectResult()
    {
        // Arrange: Set up the expected result (as if returned by the real API)
        var expectedResult = "{\"products\": [{\"id\": 1, \"name\": \"Product1\"}]}";  // Example response

        // Set up the mock to return the expected result for GetAllProductsAsync
        _mockServiceProductController
            .Setup(controller => controller.GetAllProductsAsync())
            .ReturnsAsync(expectedResult);

        // Act: Call the GetAllProducts method of ProxyProductController
        var result = await _proxyProductController.GetAllProducts();
        Console.WriteLine($"Result: {result}");

        // Assert: Check that the result is an OkObjectResult (for 200 OK response)
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult); // Ensure the result is of type OkObjectResult
        Assert.That(okResult.Value, Is.EqualTo(expectedResult)); // The content should match the expected result
        Assert.That(okResult.StatusCode, Is.EqualTo(200)); // The status code should be 200
    }

    [Test]
    public async Task GetAllProducts_ReturnsInternalServerError_WhenExceptionOccurs()
    {
        // Arrange: Set up ServiceProductController to throw an exception
        _mockServiceProductController
            .Setup(controller => controller.GetAllProductsAsync())
            .ThrowsAsync(new System.Exception("Unexpected error"));

        // Act: Call the GetAllProducts method of ProxyProductController
        var result = await _proxyProductController.GetAllProducts();

        // Assert: Check that the result is an ObjectResult
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult); // Ensure the result is of type ObjectResult
        Assert.AreEqual(500, objectResult.StatusCode); // Ensure status code is 500
        Assert.IsNotNull(objectResult.Value); // Ensure there is a message in the response
    }
    [Test]
    public async Task GetAllProducts_ReturnsInternalServerError_WhenResultStartsWithError()
    {
        // Arrange: Set up the ServiceProductController to return a string starting with "Error"
        var errorMessage = "Error: Unable to fetch products";
        _mockServiceProductController
            .Setup(controller => controller.GetAllProductsAsync())
            .ReturnsAsync(errorMessage);

        // Act: Call the GetAllProducts method of ProxyProductController
        var result = await _proxyProductController.GetAllProducts();

        // Assert: Check that the result is an ObjectResult 
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult); // Ensure the result is of type ObjectResult
        Assert.AreEqual(500, objectResult.StatusCode); // Ensure status code is 500
        Assert.IsNotNull(objectResult.Value); // Ensure there is a message in the response
    }

}