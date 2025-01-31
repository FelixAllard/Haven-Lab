using System.Net;
using System.Text;
using Api_Gateway.Models;
using Api_Gateway.Services;
using Moq;
using Moq.Protected;

namespace TestingProject.Api_Gateway.Services;

[TestFixture]
public class ServiceTemplateControllerTests
{
    private Mock<IHttpClientFactory> _httpClientFactoryMock;
    private ServiceTemplateController _controller;
    private Mock<HttpMessageHandler> _handlerMock;

    [SetUp]
    public void SetUp()
    {
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _handlerMock = new Mock<HttpMessageHandler>();

        var client = new HttpClient(_handlerMock.Object)
        {
            BaseAddress = new Uri("http://localhost:5092")
        };

        _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
        _controller = new ServiceTemplateController(_httpClientFactoryMock.Object);
    }

    [Test]
    public async Task GetAllTemplateNames_ReturnsSuccess()
    {
        var expectedResponse = "[\"Template1\", \"Template2\"]";
        SetupHttpResponse(HttpStatusCode.OK, expectedResponse);

        var result = await _controller.GetAllTemplateNames();

        Assert.AreEqual(expectedResponse, result);
    }

    [Test]
    public async Task GetAllTemplateNames_ReturnsError()
    {
        SetupHttpResponse(HttpStatusCode.BadRequest, "Bad Request");

        var result = await _controller.GetAllTemplateNames();

        Assert.AreEqual("Error fetching price rules: Bad Request", result);
    }
    

    [Test]
    public async Task GetAllTemplateNames_ThrowsException()
    {
        SetupHttpException();

        var result = await _controller.GetAllTemplateNames();

        Assert.IsTrue(result.StartsWith("Exception:"));
    }

    [Test]
    public async Task PostTemplate_ReturnsSuccess()
    {
        var template = new Template { TemplateName = "TestTemplate" };
        SetupHttpResponse(HttpStatusCode.Created, "Created");

        var response = await _controller.PostTemplate(template);

        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
    }

    [Test]
    public async Task PostTemplate_ReturnsHttpRequestException()
    {
        SetupHttpException();

        var template = new Template { TemplateName = "TestTemplate" };
        var response = await _controller.PostTemplate(template);

        Assert.AreEqual(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Test]
    public async Task PutTemplate_ReturnsSuccess()
    {
        var template = new Template { TemplateName = "UpdatedTemplate" };
        SetupHttpResponse(HttpStatusCode.OK, "Updated");

        var response = await _controller.PutTemplate("UpdatedTemplate", template);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [Test]
    public async Task PutTemplate_ThrowsException()
    {
        SetupHttpException();

        var template = new Template { TemplateName = "UpdatedTemplate" };
        var response = await _controller.PutTemplate("UpdatedTemplate", template);

        Assert.AreEqual(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Test]
    public async Task DeleteTemplate_ReturnsSuccess()
    {
        SetupHttpResponse(HttpStatusCode.OK, "Deleted");

        var result = await _controller.DeleteTemplate("TestTemplate");

        Assert.AreEqual("Deleted", result);
    }

    [Test]
    public async Task DeleteTemplate_ReturnsError()
    {
        SetupHttpResponse(HttpStatusCode.NotFound, "Not Found");

        var result = await _controller.DeleteTemplate("TestTemplate");

        Assert.AreEqual("Error fetching price rules: Not Found", result);
    }

    [Test]
    public async Task DeleteTemplate_ThrowsException()
    {
        SetupHttpException();

        var result = await _controller.DeleteTemplate("TestTemplate");

        Assert.IsTrue(result.StartsWith("Exception:"));
    }

    [Test]
    public async Task GetAllTemplate_ReturnsSuccess()
    {
        var expectedResponse = "[{\"TemplateName\":\"Template1\"}, {\"TemplateName\":\"Template2\"}]";
        SetupHttpResponse(HttpStatusCode.OK, expectedResponse);

        var result = await _controller.GetAllTemplate();

        Assert.AreEqual(expectedResponse, result);
    }

    [Test]
    public async Task GetAllTemplate_ReturnsError()
    {
        SetupHttpResponse(HttpStatusCode.BadRequest, "Bad Request");

        var result = await _controller.GetAllTemplate();

        Assert.AreEqual("Error fetching price rules: Bad Request", result);
    }

    [Test]
    public async Task GetAllTemplate_ThrowsException()
    {
        SetupHttpException();

        var result = await _controller.GetAllTemplate();

        Assert.IsTrue(result.StartsWith("Exception:"));
    }

    [Test]
    public async Task GetTemplateByName_ReturnsSuccess()
    {
        var expectedResponse = "{\"TemplateName\":\"Template1\"}";
        SetupHttpResponse(HttpStatusCode.OK, expectedResponse);

        var result = await _controller.GetTemplateByName("Template1");

        Assert.AreEqual(expectedResponse, result);
    }

    [Test]
    public async Task GetTemplateByName_ReturnsError()
    {
        SetupHttpResponse(HttpStatusCode.NotFound, "Not Found");

        var result = await _controller.GetTemplateByName("NonExistingTemplate");

        Assert.AreEqual("Error fetching price rules: Not Found", result);
    }

    [Test]
    public async Task GetTemplateByName_ThrowsException()
    {
        SetupHttpException();

        var result = await _controller.GetTemplateByName("Template1");

        Assert.IsTrue(result.StartsWith("Exception:"));
    }
    [Test]
    public async Task PostTemplate_ThrowsException()
    {
        SetupOtherException();

        var template = new Template { TemplateName = "TestTemplate" };
        var response = await _controller.PostTemplate(template);

        Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.IsTrue(content.StartsWith("Error:"));
    }

    [Test]
    public async Task PutTemplate2_ThrowsException()
    {
        SetupOtherException();

        var template = new Template { TemplateName = "UpdatedTemplate" };
        var response = await _controller.PutTemplate("UpdatedTemplate", template);

        Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.IsTrue(content.StartsWith("Error:"));
    }


    private void SetupHttpResponse(HttpStatusCode statusCode, string content)
    {
        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            });
    }
    

    private void SetupHttpException()
    {
        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new HttpRequestException("Network error"));
    }
    private void SetupOtherException()
    {
        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new Exception("Network error"));
    }
}
