using System.Net;
using Api_Gateway.Controller;
using Api_Gateway.Models;
using Api_Gateway.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace TestingProject.Api_Gateway.Controller;

[TestFixture]
public class ProxyTemplateControllerTest
{
    private ProxyTemplateController _controller;
    private Mock<ServiceTemplateController> _mockService;

    [SetUp]
    public void Setup()
    {
        _mockService = new Mock<ServiceTemplateController>();
        _controller = new ProxyTemplateController(_mockService.Object);
    }

    [Test]
    public async Task GetAllTemplatesNames_ReturnsOk()
    {
        _mockService.Setup(s => s.GetAllTemplateNames()).ReturnsAsync("Template1, Template2");
        var result = await _controller.GetAllTemplatesNames();
        Assert.IsInstanceOf<OkObjectResult>(result);
    }

    [Test]
    public async Task GetAllTemplatesNames_ReturnsNotFound()
    {
        _mockService.Setup(s => s.GetAllTemplateNames()).ReturnsAsync("404 Not Found");
        var result = await _controller.GetAllTemplatesNames();
        Assert.IsInstanceOf<NotFoundObjectResult>(result);
    }
    [Test]
    public async Task GetAllTemplatesNames_Unauthorized_ReturnsUnauthorized()
    {
        // Arrange
        var unauthorizedResult = "401 Unauthorized: Access is denied due to invalid credentials";
        _mockService.Setup(s => s.GetAllTemplateNames()).ReturnsAsync(unauthorizedResult);

        // Act
        var result = await _controller.GetAllTemplatesNames();

        // Assert
        var actionResult = result as ObjectResult;
        Assert.AreEqual(401, actionResult.StatusCode);
    }

    [Test]
    public async Task GetAllTemplatesNames_Error_ReturnsInternalServerError()
    {
        // Arrange
        var errorResult = "Error: Something went wrong!";
        _mockService.Setup(s => s.GetAllTemplateNames()).ReturnsAsync(errorResult);

        // Act
        var result = await _controller.GetAllTemplatesNames();

        // Assert
        var actionResult = result as ObjectResult;
        Assert.AreEqual(500, actionResult.StatusCode);
    }

    [Test]
    public async Task GetAllTemplatesNames_Exception_ReturnsInternalServerError()
    {
        // Arrange
        _mockService.Setup(s => s.GetAllTemplateNames()).ThrowsAsync(new Exception("An unexpected error occurred"));

        // Act
        var result = await _controller.GetAllTemplatesNames();

        // Assert
        var actionResult = result as ObjectResult;
        Assert.AreEqual(500, actionResult.StatusCode);
    }
    //Get All Templates
    [Test]
    public async Task GetAllTemplates_NotFound_ReturnsNotFound()
    {
        // Arrange
        var notFoundResult = "404 Not Found: Templates not found";
        _mockService.Setup(s => s.GetAllTemplate()).ReturnsAsync(notFoundResult);

        // Act
        var result = await _controller.GetAllTemplates();

        // Assert
        var actionResult = result as ObjectResult;
        Assert.AreEqual(404, actionResult.StatusCode);
    }

    [Test]
    public async Task GetAllTemplates_Unauthorized_ReturnsUnauthorized()
    {
        // Arrange
        var unauthorizedResult = "401 Unauthorized: Access is denied due to invalid credentials";
        _mockService.Setup(s => s.GetAllTemplate()).ReturnsAsync(unauthorizedResult);

        // Act
        var result = await _controller.GetAllTemplates();

        // Assert
        var actionResult = result as ObjectResult;
        Assert.AreEqual(401, actionResult.StatusCode);
    }

    [Test]
    public async Task GetAllTemplates_Error_ReturnsInternalServerError()
    {
        // Arrange
        var errorResult = "Error: Something went wrong!";
        _mockService.Setup(s => s.GetAllTemplate()).ReturnsAsync(errorResult);

        // Act
        var result = await _controller.GetAllTemplates();

        // Assert
        var actionResult = result as ObjectResult;
        Assert.AreEqual(500, actionResult.StatusCode);

    }

    [Test]
    public async Task GetAllTemplates_Exception_ReturnsInternalServerError()
    {
        // Arrange
        _mockService.Setup(s => s.GetAllTemplate()).ThrowsAsync(new Exception("An unexpected error occurred"));

        // Act
        var result = await _controller.GetAllTemplates();

        // Assert
        var actionResult = result as ObjectResult;
        Assert.AreEqual(500, actionResult.StatusCode);
    }
    [Test]
    public async Task PostTemplate_ServiceUnavailable_ReturnsServiceUnavailable()
    {
        // Arrange
        var template = new Template { /* Set properties here */ };
        var httpResponse = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
        {
            Content = new StringContent("Service is currently unavailable")
        };
        _mockService.Setup(s => s.PostTemplate(It.IsAny<Template>())).ReturnsAsync(httpResponse);

        // Act
        var result = await _controller.PostTemplate(template);

        // Assert
        var actionResult = result as ObjectResult;
        Assert.AreEqual(503, actionResult.StatusCode);
    }

    [Test]
    public async Task PostTemplate_Exception_ReturnsInternalServerError()
    {
        // Arrange
        var template = new Template { /* Set properties here */ };
        _mockService.Setup(s => s.PostTemplate(It.IsAny<Template>())).ThrowsAsync(new Exception("An unexpected error occurred"));

        // Act
        var result = await _controller.PostTemplate(template);

        // Assert
        var actionResult = result as ObjectResult;
        Assert.AreEqual(500, actionResult.StatusCode);
    }
    [Test]
    public async Task PutTemplate_ServiceUnavailable_ReturnsServiceUnavailable()
    {
        // Arrange
        var template = new Template { /* Set properties here */ };
        var httpResponse = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
        {
            Content = new StringContent("Service is currently unavailable")
        };
        _mockService.Setup(s => s.PutTemplate(It.IsAny<string>(), It.IsAny<Template>())).ReturnsAsync(httpResponse);

        // Act
        var result = await _controller.PutTemplate("templateName", template);

        // Assert
        var actionResult = result as ObjectResult;
        Assert.AreEqual(503, actionResult.StatusCode);
    }

    [Test]
    public async Task PutTemplate_Exception_ReturnsInternalServerError()
    {
        // Arrange
        var template = new Template { /* Set properties here */ };
        _mockService.Setup(s => s.PutTemplate(It.IsAny<string>(), It.IsAny<Template>())).ThrowsAsync(new Exception("An unexpected error occurred"));

        // Act
        var result = await _controller.PutTemplate("templateName", template);

        // Assert
        var actionResult = result as ObjectResult;
        Assert.AreEqual(500, actionResult.StatusCode);
    }


    

    [Test]
    public async Task GetAllTemplates_ReturnsOk()
    {
        _mockService.Setup(s => s.GetAllTemplate()).ReturnsAsync("Template Data");
        var result = await _controller.GetAllTemplates();
        Assert.IsInstanceOf<OkObjectResult>(result);
    }
    
    [Test]
    public async Task GetTemplateByName_NotFound_ReturnsNotFound()
    {
        // Arrange
        var notFoundResult = "404 Not Found: Template not found";
        _mockService.Setup(s => s.GetTemplateByName(It.IsAny<string>())).ReturnsAsync(notFoundResult);

        // Act
        var result = await _controller.GetTemplateByName("templateName");

        // Assert
        var actionResult = result as ObjectResult;
        Assert.AreEqual(404, actionResult.StatusCode);
    }

    [Test]
    public async Task GetTemplateByName_Unauthorized_ReturnsUnauthorized()
    {
        // Arrange
        var unauthorizedResult = "401 Unauthorized: Access is denied due to invalid credentials";
        _mockService.Setup(s => s.GetTemplateByName(It.IsAny<string>())).ReturnsAsync(unauthorizedResult);

        // Act
        var result = await _controller.GetTemplateByName("templateName");

        // Assert
        var actionResult = result as ObjectResult;
        Assert.AreEqual(401, actionResult.StatusCode);
    }

    [Test]
    public async Task GetTemplateByName_Error_ReturnsInternalServerError()
    {
        // Arrange
        var errorResult = "Error: Something went wrong!";
        _mockService.Setup(s => s.GetTemplateByName(It.IsAny<string>())).ReturnsAsync(errorResult);

        // Act
        var result = await _controller.GetTemplateByName("templateName");

        // Assert
        var actionResult = result as ObjectResult;
        Assert.AreEqual(500, actionResult.StatusCode);
    }

    [Test]
    public async Task GetTemplateByName_Exception_ReturnsInternalServerError()
    {
        // Arrange
        _mockService.Setup(s => s.GetTemplateByName(It.IsAny<string>())).ThrowsAsync(new Exception("An unexpected error occurred"));

        // Act
        var result = await _controller.GetTemplateByName("templateName");

        // Assert
        var actionResult = result as ObjectResult;
        Assert.AreEqual(500, actionResult.StatusCode);
    }
    [Test]
public async Task DeleteTemplate_Exception_ReturnsInternalServerError()
{
    // Arrange
    _mockService.Setup(s => s.DeleteTemplate(It.IsAny<string>())).ThrowsAsync(new Exception("An unexpected error occurred"));

    // Act
    var result = await _controller.DeleteTemplate("templateName");

    // Assert
    var actionResult = result as ObjectResult;
    Assert.AreEqual(500, actionResult.StatusCode);
}

[Test]
public async Task DeleteTemplate_ErrorResponse_ReturnsInternalServerError()
{
    // Arrange
    var errorMessage = "Error: Template not found";
    _mockService.Setup(s => s.DeleteTemplate(It.IsAny<string>())).ReturnsAsync(errorMessage);

    // Act
    var result = await _controller.DeleteTemplate("templateName");

    // Assert
    var actionResult = result as ObjectResult;
    Assert.AreEqual(500, actionResult.StatusCode);
}

[Test]
public async Task DeleteTemplate_NotFoundResponse_ReturnsNotFound()
{
    // Arrange
    var notFoundMessage = "404 Not Found: Template not found";
    _mockService.Setup(s => s.DeleteTemplate(It.IsAny<string>())).ReturnsAsync(notFoundMessage);

    // Act
    var result = await _controller.DeleteTemplate("templateName");

    // Assert
    var actionResult = result as ObjectResult;
    Assert.AreEqual(404, actionResult.StatusCode);
}

[Test]
public async Task DeleteTemplate_UnauthorizedResponse_ReturnsUnauthorized()
{
    // Arrange
    var unauthorizedMessage = "401 Unauthorized: Access denied";
    _mockService.Setup(s => s.DeleteTemplate(It.IsAny<string>())).ReturnsAsync(unauthorizedMessage);

    // Act
    var result = await _controller.DeleteTemplate("templateName");

    // Assert
    var actionResult = result as ObjectResult;
    Assert.AreEqual(401, actionResult.StatusCode);
}


    [Test]
    public async Task GetTemplateByName_ReturnsOk()
    {
        _mockService.Setup(s => s.GetTemplateByName("TestTemplate")).ReturnsAsync("Template Data");
        var result = await _controller.GetTemplateByName("TestTemplate");
        Assert.IsInstanceOf<OkObjectResult>(result);
    }

    [Test]
    public async Task PostTemplate_ReturnsStatusCode()
    {
        var template = new Template();
        var response = new HttpResponseMessage(HttpStatusCode.Created);
        _mockService.Setup(s => s.PostTemplate(template)).ReturnsAsync(response);
        var result = await _controller.PostTemplate(template);
        Assert.IsInstanceOf<ObjectResult>(result);
    }

    [Test]
    public async Task PutTemplate_ReturnsStatusCode()
    {
        var template = new Template();
        var response = new HttpResponseMessage(HttpStatusCode.OK);
        _mockService.Setup(s => s.PutTemplate("TestTemplate", template)).ReturnsAsync(response);
        var result = await _controller.PutTemplate("TestTemplate", template);
        Assert.IsInstanceOf<ObjectResult>(result);
    }

    [Test]
    public async Task DeleteTemplate_ReturnsOk()
    {
        _mockService.Setup(s => s.DeleteTemplate("TestTemplate")).ReturnsAsync("Deleted Successfully");
        var result = await _controller.DeleteTemplate("TestTemplate");
        Assert.IsInstanceOf<OkObjectResult>(result);
    }
    
}