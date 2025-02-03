using System.Net;
using Api_Gateway.Services;
using Moq;
using Moq.Protected;

namespace TestingProject.Api_Gateway.Services;
[TestFixture]
public class ServiceEmailLogControllerTest
{
    private Mock<IHttpClientFactory> _mockHttpClientFactory;
    private Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private ServiceEmailLogController _service;

    [SetUp]
    public void SetUp()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        var client = new HttpClient(_mockHttpMessageHandler.Object);

        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);

        _service = new ServiceEmailLogController(_mockHttpClientFactory.Object);
    }

    [Test]
    public async Task GetAllEmailLogs_ReturnsLogs_OnSuccess()
    {
        var expectedContent = "Valid log data";
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(expectedContent)
        };

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        var result = await _service.GetAllEmailLogs();

        Assert.AreEqual(expectedContent, result);
    }

    [Test]
    public async Task GetAllEmailLogs_ReturnsErrorMessage_OnFailure()
    {
        var response = new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            ReasonPhrase = "Not Found"
        };

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        var result = await _service.GetAllEmailLogs();

        Assert.AreEqual("Error fetching email Logs: Not Found", result);
    }

    [Test]
    public async Task GetAllEmailLogs_ReturnsExceptionMessage_OnException()
    {
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new Exception("Network failure"));

        var result = await _service.GetAllEmailLogs();

        Assert.AreEqual("Exception: Network failure", result);
    }
}