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
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var loginModel = new Login { Username = "wrongUser", Password = "wrongPassword" };

        _mockServiceAuthController
            .Setup(s => s.LoginAsync(It.IsAny<Login>()))
            .ReturnsAsync(new ObjectResult(new { Message = "Invalid credentials" }) { StatusCode = 401 });

        // Act
        var result = await _proxyAuthController.Login(loginModel);

        // Assert
        var statusCodeResult = result as ObjectResult;
        Assert.IsNotNull(statusCodeResult);
        Assert.AreEqual(401, statusCodeResult.StatusCode);
    }

    [Test]
    public async Task Login_ServiceUnavailable_ReturnsServiceUnavailable()
    {
        // Arrange
        var loginModel = new Login { Username = "user", Password = "password" };

        _mockServiceAuthController
            .Setup(s => s.LoginAsync(It.IsAny<Login>()))
            .ReturnsAsync(new ObjectResult(new { Message = "Service Unavailable" }) { StatusCode = 503 });

        // Act
        var result = await _proxyAuthController.Login(loginModel);

        // Assert
        var statusCodeResult = result as ObjectResult;
        Assert.IsNotNull(statusCodeResult);
        Assert.AreEqual(503, statusCodeResult.StatusCode);
    }

    [Test]
    public async Task Logout_Success_ReturnsSuccessMessage()
    {
        // Arrange
        var username = "user123";

        _mockServiceAuthController
            .Setup(s => s.LogoutAsync(It.IsAny<string>()))
            .ReturnsAsync(new ObjectResult(new { Message = "Logged out successfully" }) { StatusCode = 200 });

        // Act
        var result = await _proxyAuthController.Logout(username);

        // Assert
        var statusCodeResult = result as ObjectResult;
        Assert.IsNotNull(statusCodeResult);
        Assert.AreEqual(200, statusCodeResult.StatusCode);
        var response = statusCodeResult.Value as JObject;
    }
    
    [Test]
    public async Task Logout_UserNotFound_ReturnsNotFound()
    {
        // Arrange
        var username = "nonExistentUser";

        _mockServiceAuthController
            .Setup(s => s.LogoutAsync(It.IsAny<string>()))
            .ReturnsAsync(new ObjectResult(new { Message = "User not found" }) { StatusCode = 404 });

        // Act
        var result = await _proxyAuthController.Logout(username);

        // Assert
        var statusCodeResult = result as ObjectResult;
        Assert.IsNotNull(statusCodeResult);
        Assert.AreEqual(404, statusCodeResult.StatusCode);
    }

    [Test]
    public async Task Logout_Unauthorized_ReturnsUnauthorized()
    {
        // Arrange
        var username = "user123";

        _mockServiceAuthController
            .Setup(s => s.LogoutAsync(It.IsAny<string>()))
            .ReturnsAsync(new ObjectResult(new { Message = "Unauthorized to perform logout" }) { StatusCode = 401 });

        // Act
        var result = await _proxyAuthController.Logout(username);

        // Assert
        var statusCodeResult = result as ObjectResult;
        Assert.IsNotNull(statusCodeResult);
        Assert.AreEqual(401, statusCodeResult.StatusCode);
    }
    
    [Test]
    public async Task VerifyToken_ValidToken_ReturnsValidMessage()
    {
        // Arrange
        var token = "validToken123";

        _mockServiceAuthController
            .Setup(s => s.VerifyTokenAsync(It.IsAny<string>()))
            .ReturnsAsync(new ObjectResult(new { Message = "Token is valid." }) { StatusCode = 200 });

        // Act
        var result = await _proxyAuthController.VerifyToken(token);

        // Assert
        var statusCodeResult = result as ObjectResult;
        Assert.IsNotNull(statusCodeResult);
        Assert.AreEqual(200, statusCodeResult.StatusCode);
    }
    
    [Test]
    public async Task VerifyToken_InvalidToken_ReturnsUnauthorized()
    {
        // Arrange
        var token = "invalidToken123";

        _mockServiceAuthController
            .Setup(s => s.VerifyTokenAsync(It.IsAny<string>()))
            .ReturnsAsync(new ObjectResult(new { Message = "Token is invalid or expired." }) { StatusCode = 401 });

        // Act
        var result = await _proxyAuthController.VerifyToken(token);

        // Assert
        var statusCodeResult = result as ObjectResult;
        Assert.IsNotNull(statusCodeResult);
        Assert.AreEqual(401, statusCodeResult.StatusCode);
    }

    [Test]
    public async Task VerifyToken_ServiceUnavailable_ReturnsServiceUnavailable()
    {
        // Arrange
        var token = "someToken123";

        _mockServiceAuthController
            .Setup(s => s.VerifyTokenAsync(It.IsAny<string>()))
            .ReturnsAsync(new ObjectResult(new { Message = "Service Unavailable" }) { StatusCode = 503 });

        // Act
        var result = await _proxyAuthController.VerifyToken(token);

        // Assert
        var statusCodeResult = result as ObjectResult;
        Assert.IsNotNull(statusCodeResult);
        Assert.AreEqual(503, statusCodeResult.StatusCode);
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

