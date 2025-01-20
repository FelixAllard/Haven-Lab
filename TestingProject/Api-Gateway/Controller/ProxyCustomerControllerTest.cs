using Api_Gateway.Controller;
using Api_Gateway.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shopify_Api.Controllers;

namespace TestingProject.Api_Gateway.Controller;

[TestFixture]
public class ProxyCustomerControllerTest
{
    private Mock<ServiceCustomerController> _mockServiceCustomerController;
    private ProxyCustomerController _controller;

    [SetUp]
    public void SetUp()
    {
        // Create a mock of the ServiceOrderController
        _mockServiceCustomerController = new Mock<ServiceCustomerController>(null);

        // Inject the mock into the ProxyOrderController
        _controller = new ProxyCustomerController(_mockServiceCustomerController.Object);
    }

    [Test]
    public async Task Subscribe_Success_ShouldReturnOk()
    {
        // Arrange
        var email = "newuser@example.com";
        var subscriptionResult = "Subscription successful";

        // Setup mock to return a success message
        _mockServiceCustomerController.Setup(service => service.Subscribe(It.IsAny<string>()))
            .ReturnsAsync(subscriptionResult);

        // Act
        var result = await _controller.Subscribe(email);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));

        var returnValue = okResult.Value as dynamic;
        Assert.IsNotNull(returnValue);

        // Verify Subscribe method is called once with the correct email
        _mockServiceCustomerController.Verify(service => service.Subscribe(It.IsAny<string>()), Times.Once);
    }
    
    [Test]
    public async Task Subscribe_ErrorMessage_ShouldReturn500()
    {
        // Arrange
        var email = "existinguser@example.com";
        var errorMessage = "Error: Something went wrong";

        // Setup mock to return an error message
        _mockServiceCustomerController.Setup(service => service.Subscribe(It.IsAny<string>()))
            .ReturnsAsync(errorMessage);

        // Act
        var result = await _controller.Subscribe(email);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));

        var returnValue = objectResult.Value as dynamic;
        Assert.IsNotNull(returnValue);

        // Verify Subscribe method is called once with the correct email
        _mockServiceCustomerController.Verify(service => service.Subscribe(It.IsAny<string>()), Times.Once);
    }
}
