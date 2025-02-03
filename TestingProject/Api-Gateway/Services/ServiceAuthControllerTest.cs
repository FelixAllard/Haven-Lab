using System.Net;
using Api_Gateway.Models;
using Api_Gateway.Services;
using Moq;
using Moq.Protected;
using Microsoft.AspNetCore.Mvc;

namespace TestingProject.Api_Gateway.Services;

[TestFixture]
public class ServiceAuthControllerTest
{
    private Mock<IHttpClientFactory> _httpClientFactoryMock;
    private Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private ServiceAuthController _serviceAuthController;

    [SetUp]
    public void SetUp()
    {
        // Create a mock of the HttpMessageHandler to mock the HTTP response.
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        // Mock IHttpClientFactory to return an HttpClient using the mocked handler.
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _httpClientFactoryMock
            .Setup(factory => factory.CreateClient(It.IsAny<string>()))
            .Returns(new HttpClient(_httpMessageHandlerMock.Object));

        // Initialize the ServiceAuthController with the mocked IHttpClientFactory.
        _serviceAuthController = new ServiceAuthController(_httpClientFactoryMock.Object);
    }

        
        
}
    
