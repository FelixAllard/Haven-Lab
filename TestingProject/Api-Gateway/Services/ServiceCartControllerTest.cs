using Api_Gateway.Models;
using Api_Gateway.Services;
using Microsoft.AspNetCore.Http;
using Moq;

namespace TestingProject.Api_Gateway.Services;
 
[TestFixture]
public class ServiceCartControllerTest
{
    private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private Mock<HttpContext> _mockHttpContext;
    private Mock<HttpRequest> _mockRequest;
    private Mock<HttpResponse> _mockResponse;
    private ServiceCartController _serviceCartController;

    [SetUp]
    public void SetUp()
    {
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _mockHttpContext = new Mock<HttpContext>();
        _mockRequest = new Mock<HttpRequest>();
        _mockResponse = new Mock<HttpResponse>();

        _mockHttpContext.Setup(ctx => ctx.Request).Returns(_mockRequest.Object);
        _mockHttpContext.Setup(ctx => ctx.Response).Returns(_mockResponse.Object);
        _mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(_mockHttpContext.Object);

        _serviceCartController = new ServiceCartController(_mockHttpContextAccessor.Object);
    }

    [Test]
    public void GetCartFromCookies_ShouldReturnEmptyList_WhenNoCartExists()
    {
        // Arrange
        _mockRequest.Setup(req => req.Cookies.ContainsKey("Cart")).Returns(false);

        // Act
        var result = _serviceCartController.GetCartFromCookies();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsEmpty(result);
    }

    [Test]
    public void GetCartFromCookies_ShouldReturnDeserializedCart_WhenValidCartExists()
    {
        // Arrange
        var cartJson = "[{\"ProductId\":1,\"ProductTitle\":\"Test Product\",\"VariantId\":1001,\"Price\":10.99,\"Quantity\":2}]";
        _mockRequest.Setup(req => req.Cookies.ContainsKey("Cart")).Returns(true);
        _mockRequest.Setup(req => req.Cookies["Cart"]).Returns(cartJson);

        // Act
        var result = _serviceCartController.GetCartFromCookies();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(1, result.First().ProductId);
    }

    [Test]
    public void GetCartFromCookies_ShouldReturnEmptyList_WhenDeserializationFails()
    {
        // Arrange
        var invalidCartJson = "{";
        _mockRequest.Setup(req => req.Cookies.ContainsKey("Cart")).Returns(true);
        _mockRequest.Setup(req => req.Cookies["Cart"]).Returns(invalidCartJson);

        // Act
        var result = _serviceCartController.GetCartFromCookies();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsEmpty(result);
    }
    
    [Test]
    public void SaveCartToCookies_ShouldSerializeAndSaveCart()
    {
        // Arrange
        var cart = new List<CartItem>
        {
            new CartItem { ProductId = 1, ProductTitle = "Test Product", VariantId = 1001, Price = 10.99m, Quantity = 2 }
        };
        var cookieOptions = new CookieOptions();
        _mockResponse.Setup(res => res.Cookies.Append("Cart", It.IsAny<string>(), It.IsAny<CookieOptions>()));

        // Act
        _serviceCartController.SaveCartToCookies(cart);

        // Assert
        _mockResponse.Verify(res => res.Cookies.Append("Cart", It.IsAny<string>(), It.IsAny<CookieOptions>()), Times.Once);
    }
}
