using Api_Gateway.Controller;
using Api_Gateway.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace TestingProject.Api_Gateway.Controller;
[TestFixture]
public class ProxyEmailLogControllerTest
{
    private Mock<ServiceEmailLogController> _mockServiceEmailLog;
    private ProxyEmailLogController _controller;

    [SetUp]
    public void SetUp()
    {
        _mockServiceEmailLog = new Mock<ServiceEmailLogController>(Mock.Of<IHttpClientFactory>());
        _controller = new ProxyEmailLogController(_mockServiceEmailLog.Object);
    }

    [Test]
    public async Task GetAllTemplatesNames_ReturnsNotFound_WhenServiceReturns404()
    {
        _mockServiceEmailLog.Setup(s => s.GetAllEmailLogs()).ReturnsAsync("404 Not Found");

        var result = await _controller.GetAllTemplatesNames();

        Assert.IsInstanceOf<NotFoundObjectResult>(result);
    }

    [Test]
    public async Task GetAllTemplatesNames_ReturnsUnauthorized_WhenServiceReturns401()
    {
        _mockServiceEmailLog.Setup(s => s.GetAllEmailLogs()).ReturnsAsync("401 Unauthorized");

        var result = await _controller.GetAllTemplatesNames();

        Assert.IsInstanceOf<UnauthorizedObjectResult>(result);
    }

    [Test]
    public async Task GetAllTemplatesNames_ReturnsInternalServerError_WhenServiceReturnsError()
    {
        _mockServiceEmailLog.Setup(s => s.GetAllEmailLogs()).ReturnsAsync("Error: Something went wrong");

        var result = await _controller.GetAllTemplatesNames();

        Assert.IsInstanceOf<ObjectResult>(result);
        var objectResult = (ObjectResult)result;
        Assert.AreEqual(500, objectResult.StatusCode);
    }

    [Test]
    public async Task GetAllTemplatesNames_ReturnsOk_WhenServiceReturnsValidData()
    {
        _mockServiceEmailLog.Setup(s => s.GetAllEmailLogs()).ReturnsAsync("Valid log data");

        var result = await _controller.GetAllTemplatesNames();

        Assert.IsInstanceOf<OkObjectResult>(result);
    }

    [Test]
    public async Task GetAllTemplatesNames_ReturnsInternalServerError_OnException()
    {
        _mockServiceEmailLog.Setup(s => s.GetAllEmailLogs()).ThrowsAsync(new Exception("Unexpected error"));

        var result = await _controller.GetAllTemplatesNames();

        Assert.IsInstanceOf<ObjectResult>(result);
        var objectResult = (ObjectResult)result;
        Assert.AreEqual(500, objectResult.StatusCode);
    }
    
}