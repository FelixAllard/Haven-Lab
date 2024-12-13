using Api_Gateway.Controller;
using Api_Gateway.Models;
using Api_Gateway.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace TestingProject.Api_Gateway.Controller;

[TestFixture]
public class ProxyAuthControllerTest
{
    private Mock<IHttpClientFactory> _mockHttpClientFactory; // Mock any dependencies
    private Mock<ServiceAuthController> _mockServiceAuthController;
    private ProxyAuthController _proxyAuthController; // Controller under test

    [SetUp]
    public void SetUp()
    {
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        
        _mockServiceAuthController = new Mock<ServiceAuthController>(_mockHttpClientFactory.Object);
        
        _proxyAuthController = new ProxyAuthController(_mockServiceAuthController.Object);
    }

    [Test]
    public async Task Login_ReturnsOk_WhenLoginIsSuccessful()
    {
        // Arrange
        var loginModel = new Login { Username = "user", Password = "password" };
        var token = "valid_token"; // Example of a successful token response

        // Mock the LoginAsync method to return a valid token
        _mockServiceAuthController
            .Setup(service => service.LoginAsync(It.IsAny<Login>()))
            .ReturnsAsync(token);

        // Act
        var result = await _proxyAuthController.Login(loginModel);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        // Serialize to JSON and compare
        var expectedJson = "{\"Token\":\"valid_token\"}";
        var actualJson = System.Text.Json.JsonSerializer.Serialize(okResult.Value);
        Assert.AreEqual(expectedJson, actualJson);
    }


    [Test]
    public async Task Login_ReturnsNotFound_WhenResultStartsWith404()
    {
        // Arrange
        var loginModel = new Login { Username = "user", Password = "password" };
        var errorResponse = "404 Endpoint not found"; // Example of a 404 error message

        // Mock the LoginAsync method to return a 404 error message
        _mockServiceAuthController
            .Setup(service => service.LoginAsync(It.IsAny<Login>()))
            .ReturnsAsync(errorResponse);

        // Act
        var result = await _proxyAuthController.Login(loginModel);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        Assert.IsNotNull(notFoundResult);
        Assert.AreEqual(404, notFoundResult.StatusCode);

        // Serialize to JSON and compare
        var expectedJson = "{\"Message\":\"404 Endpoint not found\"}";
        var actualJson = System.Text.Json.JsonSerializer.Serialize(notFoundResult.Value);
        Assert.AreEqual(expectedJson, actualJson);
    }

    [Test]
    public async Task Login_ReturnsBadRequest_WhenResultStartsWithError()
    {
        // Arrange
        var loginModel = new Login { Username = "user", Password = "password" };
        var errorResponse = "Error: Invalid credentials"; // Example of a generic error message

        // Mock the LoginAsync method to return an error response
        _mockServiceAuthController
            .Setup(service => service.LoginAsync(It.IsAny<Login>()))
            .ReturnsAsync(errorResponse);

        // Act
        var result = await _proxyAuthController.Login(loginModel);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);

        // Serialize to JSON and compare
        var expectedJson = "{\"Message\":\"Error: Invalid credentials\"}";
        var actualJson = System.Text.Json.JsonSerializer.Serialize(badRequestResult.Value);
        Assert.AreEqual(expectedJson, actualJson);
    }

    [Test]
    public async Task Login_ReturnsUnauthorized_WhenResultStartsWithUnauthorized()
    {
        // Arrange
        var loginModel = new Login { Username = "user", Password = "password" };
        var unauthorizedResponse = "Unauthorized: Invalid username or password"; // Example of unauthorized response

        // Mock the LoginAsync method to return an unauthorized response
        _mockServiceAuthController
            .Setup(service => service.LoginAsync(It.IsAny<Login>()))
            .ReturnsAsync(unauthorizedResponse);

        // Act
        var result = await _proxyAuthController.Login(loginModel);

        // Assert
        var unauthorizedResult = result as UnauthorizedObjectResult;
        Assert.IsNotNull(unauthorizedResult);
        Assert.AreEqual(401, unauthorizedResult.StatusCode);

        // Serialize to JSON and compare
        var expectedJson = "{\"Message\":\"Invalid username or password\"}";
        var actualJson = System.Text.Json.JsonSerializer.Serialize(unauthorizedResult.Value);
        Assert.AreEqual(expectedJson, actualJson);
    }

    [Test]
    public async Task Login_ReturnsInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var loginModel = new Login { Username = "user", Password = "password" };
        var exceptionMessage = "Unexpected error occurred";

        // Mock the LoginAsync method to throw an exception
        _mockServiceAuthController
            .Setup(service => service.LoginAsync(It.IsAny<Login>()))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _proxyAuthController.Login(loginModel);

        // Assert
        var internalServerErrorResult = result as ObjectResult;
        Assert.IsNotNull(internalServerErrorResult);
        Assert.AreEqual(500, internalServerErrorResult.StatusCode);

        // Serialize to JSON and compare
        var expectedJson = $"{{\"Message\":\"{exceptionMessage}\"}}";
        var actualJson = System.Text.Json.JsonSerializer.Serialize(internalServerErrorResult.Value);
        Assert.AreEqual(expectedJson, actualJson);
    }
}

