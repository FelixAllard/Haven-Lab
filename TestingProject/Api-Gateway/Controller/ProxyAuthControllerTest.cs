using Api_Gateway.Controller;
using Api_Gateway.Models;
using Api_Gateway.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.Net;
using System.Threading.Tasks;

namespace TestingProject.Api_Gateway.Controller;

[TestFixture]
public class ProxyAuthControllerTest
{
    private Mock<IHttpClientFactory> _mockHttpClientFactory;
    private Mock<ServiceAuthController> _mockServiceAuthController;
    private ProxyAuthController _proxyAuthController;

    [SetUp]
    public void SetUp()
    {
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockServiceAuthController = new Mock<ServiceAuthController>(_mockHttpClientFactory.Object);
        _proxyAuthController = new ProxyAuthController(_mockServiceAuthController.Object);
    }

    [Test]
    public async Task Login_ValidCredentials_ReturnsToken()
    {
        // Arrange
        var loginModel = new Login { Username = "user", Password = "password" };

        var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{ \"token\": \"validToken123\" }")
        };
        
        _mockServiceAuthController
            .Setup(s => s.LoginAsync(It.IsAny<Login>()))
            .ReturnsAsync(mockResponse);

        // Act
        var result = await _proxyAuthController.Login(loginModel);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
    }

    [Test]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var loginModel = new Login { Username = "user", Password = "wrongpassword" };

        var mockResponse = new HttpResponseMessage(HttpStatusCode.Unauthorized);
        _mockServiceAuthController
            .Setup(s => s.LoginAsync(It.IsAny<Login>()))
            .ReturnsAsync(mockResponse);

        // Act
        var result = await _proxyAuthController.Login(loginModel);

        // Assert
        var unauthorizedResult = result as UnauthorizedObjectResult;
        Assert.IsNotNull(unauthorizedResult);
        Assert.AreEqual(401, unauthorizedResult.StatusCode);
    }

    [Test]
    public async Task Login_ServiceUnavailable_Returns503()
    {
        // Arrange
        var loginModel = new Login { Username = "user", Password = "password" };

        var mockResponse = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
        _mockServiceAuthController
            .Setup(s => s.LoginAsync(It.IsAny<Login>()))
            .ReturnsAsync(mockResponse);

        // Act
        var result = await _proxyAuthController.Login(loginModel);

        // Assert
        var statusCodeResult = result as ObjectResult;
        Assert.IsNotNull(statusCodeResult);
        Assert.AreEqual(503, statusCodeResult.StatusCode);
    }

    [Test]
    public async Task Login_NotFound_Returns404()
    {
        // Arrange
        var loginModel = new Login { Username = "user", Password = "password" };

        var mockResponse = new HttpResponseMessage(HttpStatusCode.NotFound);
        _mockServiceAuthController
            .Setup(s => s.LoginAsync(It.IsAny<Login>()))
            .ReturnsAsync(mockResponse);

        // Act
        var result = await _proxyAuthController.Login(loginModel);

        // Assert
        var statusCodeResult = result as ObjectResult;
        Assert.IsNotNull(statusCodeResult);
        Assert.AreEqual(404, statusCodeResult.StatusCode);
    }
    
    [Test]
    public async Task Login_ExceptionThrown_Returns500()
    {
        // Arrange
        var loginModel = new Login { Username = "user", Password = "password" };

        _mockServiceAuthController
            .Setup(s => s.LoginAsync(It.IsAny<Login>()))
            .ThrowsAsync(new System.Exception("Some error"));

        // Act
        var result = await _proxyAuthController.Login(loginModel);

        // Assert
        var statusCodeResult = result as ObjectResult;
        Assert.IsNotNull(statusCodeResult);
        Assert.AreEqual(500, statusCodeResult.StatusCode);
    }
    
    [Test]
    public async Task Logout_ValidUser_ReturnsOk()
    {
        // Arrange
        var username = "user123";

        var mockResponse = new HttpResponseMessage(HttpStatusCode.OK);
        _mockServiceAuthController
            .Setup(s => s.LogoutAsync(It.IsAny<string>()))
            .ReturnsAsync(mockResponse);

        // Act
        var result = await _proxyAuthController.Logout(username);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
    }

    // Positive Test: User not found during logout
    [Test]
    public async Task Logout_UserNotFound_ReturnsNotFound()
    {
        // Arrange
        var username = "nonexistentUser";

        var mockResponse = new HttpResponseMessage(HttpStatusCode.NotFound);
        _mockServiceAuthController
            .Setup(s => s.LogoutAsync(It.IsAny<string>()))
            .ReturnsAsync(mockResponse);

        // Act
        var result = await _proxyAuthController.Logout(username);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        Assert.IsNotNull(notFoundResult);
        Assert.AreEqual(404, notFoundResult.StatusCode);
    }

    // Positive Test: Unauthorized logout
    [Test]
    public async Task Logout_Unauthorized_ReturnsUnauthorized()
    {
        // Arrange
        var username = "user123";

        var mockResponse = new HttpResponseMessage(HttpStatusCode.Unauthorized);
        _mockServiceAuthController
            .Setup(s => s.LogoutAsync(It.IsAny<string>()))
            .ReturnsAsync(mockResponse);

        // Act
        var result = await _proxyAuthController.Logout(username);

        // Assert
        var unauthorizedResult = result as UnauthorizedObjectResult;
        Assert.IsNotNull(unauthorizedResult);
        Assert.AreEqual(401, unauthorizedResult.StatusCode);
    }

    // Negative Test: Exception handling
    [Test]
    public async Task Logout_ExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        var username = "user123";

        _mockServiceAuthController
            .Setup(s => s.LogoutAsync(It.IsAny<string>()))
            .ThrowsAsync(new System.Exception("Some error"));

        // Act
        var result = await _proxyAuthController.Logout(username);

        // Assert
        var statusCodeResult = result as ObjectResult;
        Assert.IsNotNull(statusCodeResult);
        Assert.AreEqual(500, statusCodeResult.StatusCode);
    }
    
    [Test]
    public async Task VerifyToken_ValidToken_ReturnsOk()
    {
        // Arrange
        var token = "validToken123";

        var mockResponse = new HttpResponseMessage(HttpStatusCode.OK);
        _mockServiceAuthController
            .Setup(s => s.VerifyTokenAsync(It.IsAny<string>()))
            .ReturnsAsync(mockResponse);

        // Act
        var result = await _proxyAuthController.VerifyToken(token);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
    }

    // Positive Test: Token is invalid or expired
    [Test]
    public async Task VerifyToken_InvalidToken_ReturnsUnauthorized()
    {
        // Arrange
        var token = "invalidToken123";

        var mockResponse = new HttpResponseMessage(HttpStatusCode.Unauthorized);
        _mockServiceAuthController
            .Setup(s => s.VerifyTokenAsync(It.IsAny<string>()))
            .ReturnsAsync(mockResponse);

        // Act
        var result = await _proxyAuthController.VerifyToken(token);

        // Assert
        var unauthorizedResult = result as UnauthorizedObjectResult;
        Assert.IsNotNull(unauthorizedResult);
        Assert.AreEqual(401, unauthorizedResult.StatusCode);
    }

    // Positive Test: Token verification service not found
    [Test]
    public async Task VerifyToken_ServiceNotFound_ReturnsNotFound()
    {
        // Arrange
        var token = "someToken123";

        var mockResponse = new HttpResponseMessage(HttpStatusCode.NotFound);
        _mockServiceAuthController
            .Setup(s => s.VerifyTokenAsync(It.IsAny<string>()))
            .ReturnsAsync(mockResponse);

        // Act
        var result = await _proxyAuthController.VerifyToken(token);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        Assert.IsNotNull(notFoundResult);
        Assert.AreEqual(404, notFoundResult.StatusCode);
    }

    // Negative Test: Exception handling
    [Test]
    public async Task VerifyToken_ExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        var token = "someToken123";

        _mockServiceAuthController
            .Setup(s => s.VerifyTokenAsync(It.IsAny<string>()))
            .ThrowsAsync(new System.Exception("Some error"));

        // Act
        var result = await _proxyAuthController.VerifyToken(token);

        // Assert
        var statusCodeResult = result as ObjectResult;
        Assert.IsNotNull(statusCodeResult);
        Assert.AreEqual(500, statusCodeResult.StatusCode);
    }
}

