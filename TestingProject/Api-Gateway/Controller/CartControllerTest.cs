using System.Reflection;
using Api_Gateway.Controller;
using Microsoft.AspNetCore.Http;

namespace TestingProject.Api_Gateway.Controller;

using System.Net;
using System.Text;
using Api_Gateway.Controller;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

[TestFixture]
public class CartControllerTests
{
    private Mock<IHttpClientFactory> _mockHttpClientFactory;
    private CartController _cartController;

    [SetUp]
    public void SetUp()
    {
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _cartController = new CartController(); // Adjust constructor as needed
    }

    [Test]
    public void GetCart_ReturnsOkResultWithCartItems()
    {
        // Arrange
        var expectedCart = new Dictionary<long, int>
        {
            { 1, 2 },
            { 2, 3 }
        };
        var cartJson = JsonConvert.SerializeObject(expectedCart);

        // Setup HttpContext with cookies
        var httpContext = new DefaultHttpContext();
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        };
        httpContext.Response.Cookies.Append("Cart", cartJson, cookieOptions);
        
        _cartController.ControllerContext.HttpContext = httpContext;

        // Act
        IActionResult actionResult = _cartController.GetCart();
        
        // Assert
        var okResult = actionResult as OkObjectResult;
        Assert.IsNotNull(okResult, "Expected OkObjectResult");
        Assert.AreEqual(200, okResult.StatusCode);
    }
    //
    // [Test]
    // public async Task AddToCart_AddsProductSuccessfully()
    // {
    //     // Arrange
    //     var productId = 1;
    //     var expectedCart = new Dictionary<long, int> { { productId, 1 } };
    //     _cartController.ControllerContext.HttpContext = new DefaultHttpContext();
    //
    //     // Mock IHttpContextAccessor and HttpContext
    //     var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
    //     var httpContext = new DefaultHttpContext();
    //     httpContextAccessorMock.SetupGet(x => x.HttpContext).Returns(httpContext);
    //     _cartController.ControllerContext.HttpContext = httpContext;
    //
    //     // Mock the IsValidProductId method
    //     var mockIsValidProductId = new Mock<Func<long, Task<bool>>>();
    //     mockIsValidProductId.Setup(m => m(It.IsAny<long>())).ReturnsAsync(true); // Simulating valid product ID
    //     _cartController.IsValidProductId = mockIsValidProductId.Object; // Assign mocked method to the controller
    //
    //     // Act
    //     var result = await _cartController.AddToCart(productId);
    //
    //     // Assert
    //     var okResult = result as OkObjectResult;
    //     Assert.IsNotNull(okResult);
    //     Assert.AreEqual(200, okResult.StatusCode);
    //
    //     // Check if cart is saved properly
    //     var savedCartJson = httpContext.Request.Cookies["Cart"];
    //     var savedCart = JsonConvert.DeserializeObject<Dictionary<long, int>>(savedCartJson);
    //     Assert.AreEqual(expectedCart, savedCart);
    // }
    //
    //
    //
    // [Test]
    // public async Task AddToCart_ReturnsBadRequest_WhenProductIdIsInvalid()
    // {
    //     // Arrange
    //     var productId = -1; // Invalid product ID
    //
    //     // Mock the IsValidProductId method
    //     var mockIsValidProductId = new Mock<Func<long, Task<bool>>>();
    //     mockIsValidProductId.Setup(m => m(It.IsAny<long>())).ReturnsAsync(false); // Simulating invalid product ID check
    //     _cartController.IsValidProductId = mockIsValidProductId.Object; // Assign mocked method to the controller
    //
    //     // Act
    //     var result = await _cartController.AddToCart(productId);
    //
    //     // Assert
    //     var badRequestResult = result as BadRequestObjectResult;
    //     Assert.IsNotNull(badRequestResult);
    //     Assert.AreEqual(400, badRequestResult.StatusCode);
    //     Assert.AreEqual("Invalid product ID format.", ((dynamic)badRequestResult.Value).Message);
    // }

}

