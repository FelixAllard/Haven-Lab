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
}

