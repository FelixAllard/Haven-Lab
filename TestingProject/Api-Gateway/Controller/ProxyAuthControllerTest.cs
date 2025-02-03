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
    
    

}

