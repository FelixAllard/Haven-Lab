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
    
    [Test]
    public async Task AddToCart_ValidProductId_AddsToCart()
    {
        // Arrange
        long productId = 8073898131501;
        long variantId = 43165387784237;
        var cartJson = JsonConvert.SerializeObject(new Dictionary<long, int>());

        var httpContext = new DefaultHttpContext();
        httpContext.Response.Cookies.Append("Cart", cartJson, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        });

        _cartController.ControllerContext.HttpContext = httpContext;

        _mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5158")
        });

        // Act
        IActionResult actionResult = await _cartController.AddToCart(productId);

        // Assert
        var okResult = actionResult as OkObjectResult;
        Assert.IsNotNull(okResult, "Expected OkObjectResult");
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.AreEqual(1, ((Dictionary<long, int>)okResult.Value)[variantId]);
    }
    
    [Test]
    public void RemoveFromCart_InvalidVariantId()
    {
        // Arrange
        var cartJson = JsonConvert.SerializeObject(new Dictionary<long, int>
        {
            { 43165387784237, 2 },
            { 43164630745133, 3 }
        });

        var httpContext = new DefaultHttpContext();
        httpContext.Response.Cookies.Append("Cart", cartJson, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        });

        _cartController.ControllerContext.HttpContext = httpContext;
        
        // Act
        IActionResult actionResult = _cartController.RemoveFromCart(1);
        Console.WriteLine($"Result: {actionResult}");
        
        var cookieValue = httpContext.Request.Cookies["Cart"];
        Console.WriteLine("Cookie set in HttpContext: " + cookieValue);

        // Assert
        var notFoundResult = actionResult as NotFoundObjectResult;
        Assert.IsNotNull(notFoundResult, "Expected NotFoundObjectResult");

        // Check status code
        Assert.AreEqual(404, notFoundResult.StatusCode);

        // Check the error message
        var jsonResponse = JsonConvert.SerializeObject(notFoundResult.Value);
        Assert.IsNotNull(jsonResponse);
        Assert.That(jsonResponse, Is.EqualTo("{\"Message\":\"Variant not found in cart.\"}"));
    }
    
    [Test]
    public void AddByOne_InvalidVariantId()
    {
        // Arrange
        var cartJson = JsonConvert.SerializeObject(new Dictionary<long, int>
        {
            { 1, 2 }
        });

        var httpContext = new DefaultHttpContext();
        httpContext.Response.Cookies.Append("Cart", cartJson, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        });

        _cartController.ControllerContext.HttpContext = httpContext;

        // Act
        IActionResult actionResult = _cartController.AddByOne(1);
        Console.WriteLine($"Result: {actionResult}");
        
        // Assert
        var notFoundResult = actionResult as NotFoundObjectResult;
        Assert.IsNotNull(notFoundResult, "Expected OkObjectResult");
        Assert.AreEqual(404, notFoundResult.StatusCode);
    }

    [Test]
    public void RemoveByOne_ValidVariantId_DecreasesQuantity()
    {
        // Arrange
        var cartJson = JsonConvert.SerializeObject(new Dictionary<long, int>
        {
            { 1, 2 }
        });

        var httpContext = new DefaultHttpContext();
        httpContext.Response.Cookies.Append("Cart", cartJson, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        });

        _cartController.ControllerContext.HttpContext = httpContext;

        // Act
        IActionResult actionResult = _cartController.RemoveByOne(1);
        
        // Assert
        var notFoundResult = actionResult as NotFoundObjectResult;
        Assert.IsNotNull(notFoundResult, "Expected OkObjectResult");
        Assert.AreEqual(404, notFoundResult.StatusCode);
    }
    
    [Test]
    public async Task AddToCart_InvalidProductId_ReturnsBadRequest()
    {
        // Arrange
        long invalidProductId = -1; // An invalid product ID

        // Act
        IActionResult actionResult = await _cartController.AddToCart(invalidProductId);

        // Assert
        var badRequestResult = actionResult as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult, "Expected BadRequestObjectResult");
        Assert.AreEqual(400, badRequestResult.StatusCode);

        var jsonResponse = JsonConvert.SerializeObject(badRequestResult.Value);
        Assert.IsNotNull(jsonResponse);
        Assert.That(jsonResponse, Is.EqualTo("{\"Message\":\"Invalid product ID format.\"}"));
    }
    
}

