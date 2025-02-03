using System.Reflection;
using Api_Gateway.Controller;
using Api_Gateway.Models;
using Api_Gateway.Services;
using Microsoft.AspNetCore.Http;

namespace TestingProject.Api_Gateway.Controller;

using System.Net;
using System.Text;
using Api_Gateway.Controller;
using Api_Gateway.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

[TestFixture]
public class ProxyCartControllerTest
{
    private Mock<IHttpClientFactory> _mockHttpClientFactory;
    private Mock<ServiceProductController> _mockServiceProductController;
    private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private Mock<ServiceCartController> _mockServiceCartController;
    private ProxyCartController _proxyCartController;

    [SetUp]
    public void SetUp()
    {
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockServiceProductController = new Mock<ServiceProductController>(_mockHttpClientFactory.Object);
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _mockServiceCartController = new Mock<ServiceCartController>(_mockHttpContextAccessor.Object);

        // Initialize ProxyCartController with mocked dependencies
        _proxyCartController = new ProxyCartController(
            _mockServiceProductController.Object, 
            _mockServiceCartController.Object
        );
    }
    
    ////////////////// GET CART //////////////////
    [Test]
    public void GetCart_ShouldReturnOkResult_WithCartData()
    {
        // Arrange
        var mockCart = new List<CartItem>
        {
            new CartItem { ProductId = 1, ProductTitle = "Test Product", VariantId = 1001, Price = 10.99m, Quantity = 2 }
        };
        _mockServiceCartController.Setup(s => s.GetCartFromCookies()).Returns(mockCart);

        // Act
        var result = _proxyCartController.GetCart() as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual(mockCart, result.Value);
    }

    [Test]
    public void GetCart_ShouldReturnOkResult_WithEmptyCart_WhenNoCartExists()
    {
        // Arrange
        _mockServiceCartController.Setup(s => s.GetCartFromCookies()).Returns(new List<CartItem>());

        // Act
        var result = _proxyCartController.GetCart() as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        Assert.IsEmpty((List<CartItem>)result.Value);
    }
    
    ////////////////// ADD TO CART //////////////////
    
    [Test]
    public async Task AddToCart_ShouldReturnOk_WhenItemIsAddedSuccessfully()
    {
        // Arrange
        var productId = 1;
        var productData = "{\"title\":\"Test Product\", \"variants\":[{\"id\":1001, \"price\":10.99, \"inventory_quantity\":5}]}";
        _mockServiceProductController.Setup(s => s.GetProductByIdAsync(productId)).ReturnsAsync(productData);
        _mockServiceCartController.Setup(s => s.GetCartFromCookies()).Returns(new List<CartItem>());

        // Act
        var result = await _proxyCartController.AddToCart(productId) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
    }

    [Test]
    public async Task AddToCart_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        var productId = 1;
        _mockServiceProductController.Setup(s => s.GetProductByIdAsync(productId)).ReturnsAsync("404 Not Found");

        // Act
        var result = await _proxyCartController.AddToCart(productId) as NotFoundObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(404, result.StatusCode);
    }

    [Test]
    public async Task AddToCart_ShouldReturnBadRequest_WhenProductIsOutOfStock()
    {
        // Arrange
        var productId = 1;
        var productData = "{\"title\":\"Test Product\", \"variants\":[{\"id\":1001, \"price\":10.99, \"inventory_quantity\":0}]}";
        _mockServiceProductController.Setup(s => s.GetProductByIdAsync(productId)).ReturnsAsync(productData);

        // Act
        var result = await _proxyCartController.AddToCart(productId) as BadRequestObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
    }
    
    [Test]
    public async Task AddToCart_ShouldIncreaseQuantity_WhenItemAlreadyExistsAndStockIsAvailable()
    {
        // Arrange
        var productId = 1;
        var variantId = 1001;
        var productData = "{\"title\":\"Test Product\", \"variants\":[{\"id\":1001, \"price\":10.99, \"inventory_quantity\":5}]}";
        var existingCart = new List<CartItem>
        {
            new CartItem { ProductId = productId, ProductTitle = "Test Product", VariantId = variantId, Price = 10.99m, Quantity = 2 }
        };
        _mockServiceProductController.Setup(s => s.GetProductByIdAsync(productId)).ReturnsAsync(productData);
        _mockServiceCartController.Setup(s => s.GetCartFromCookies()).Returns(existingCart);

        // Act
        var result = await _proxyCartController.AddToCart(productId) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual(3, existingCart.First().Quantity);
    }
    
    ////////////////// REMOVE CART ITEM //////////////////
    [Test]
    public void RemoveFromCart_ShouldReturnOk_WhenItemExists()
    {
        // Arrange
        var variantId = 1001;
        var mockCart = new List<CartItem>
        {
            new CartItem { ProductId = 1, ProductTitle = "Test Product", VariantId = variantId, Price = 10.99m, Quantity = 2 }
        };
        _mockServiceCartController.Setup(s => s.GetCartFromCookies()).Returns(mockCart);

        // Act
        var result = _proxyCartController.RemoveFromCart(variantId) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
    }

    [Test]
    public void RemoveFromCart_ShouldReturnNotFound_WhenItemDoesNotExist()
    {
        // Arrange
        _mockServiceCartController.Setup(s => s.GetCartFromCookies()).Returns(new List<CartItem>());
        var variantId = 1001;

        // Act
        var result = _proxyCartController.RemoveFromCart(variantId) as NotFoundObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(404, result.StatusCode);
    }
    
    ////////////////// ADD BY ONE //////////////////
    [Test]
    public async Task AddByOne_ShouldReturnOk_WhenStockIsAvailable()
    {
        // Arrange
        var variantId = 1001;
        var productId = 1;
        var productData = "{\"title\":\"Test Product\", \"variants\":[{\"id\":1001, \"inventory_quantity\":5}]}";
        var mockCart = new List<CartItem>
        {
            new CartItem { ProductId = productId, ProductTitle = "Test Product", VariantId = variantId, Price = 10.99m, Quantity = 2 }
        };
        _mockServiceCartController.Setup(s => s.GetCartFromCookies()).Returns(mockCart);
        _mockServiceProductController.Setup(s => s.GetProductByIdAsync(productId)).ReturnsAsync(productData);

        // Act
        var result = await _proxyCartController.AddByOne(variantId) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
    }

    [Test]
    public async Task AddByOne_ShouldReturnBadRequest_WhenNotEnoughStock()
    {
        // Arrange
        var variantId = 1001;
        var productId = 1;
        var productData = "{\"title\":\"Test Product\", \"variants\":[{\"id\":1001, \"inventory_quantity\":2}]}";
        var mockCart = new List<CartItem>
        {
            new CartItem { ProductId = productId, ProductTitle = "Test Product", VariantId = variantId, Price = 10.99m, Quantity = 2 }
        };
        _mockServiceCartController.Setup(s => s.GetCartFromCookies()).Returns(mockCart);
        _mockServiceProductController.Setup(s => s.GetProductByIdAsync(productId)).ReturnsAsync(productData);

        // Act
        var result = await _proxyCartController.AddByOne(variantId) as BadRequestObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
    }
    
    ////////////////// REMOVE BY ONE //////////////////
    [Test]
    public void RemoveByOne_ShouldReturnOk_WhenQuantityIsReduced()
    {
        // Arrange
        var variantId = 1001;
        var mockCart = new List<CartItem>
        {
            new CartItem { ProductId = 1, ProductTitle = "Test Product", VariantId = variantId, Price = 10.99m, Quantity = 2 }
        };
        _mockServiceCartController.Setup(s => s.GetCartFromCookies()).Returns(mockCart);

        // Act
        var result = _proxyCartController.RemoveByOne(variantId) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual(1, mockCart.First().Quantity);
    }

    [Test]
    public void RemoveByOne_ShouldRemoveItem_WhenQuantityReachesZero()
    {
        // Arrange
        var variantId = 1001;
        var mockCart = new List<CartItem>
        {
            new CartItem { ProductId = 1, ProductTitle = "Test Product", VariantId = variantId, Price = 10.99m, Quantity = 1 }
        };
        _mockServiceCartController.Setup(s => s.GetCartFromCookies()).Returns(mockCart);

        // Act
        var result = _proxyCartController.RemoveByOne(variantId) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        Assert.IsEmpty(mockCart);
    }
}

