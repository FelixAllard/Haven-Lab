using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json.Linq;
using Shopify_Api;
using Shopify_Api.Controllers;
using Shopify_Api.Exceptions;
using ShopifySharp;
using ShopifySharp.Credentials;
using ShopifySharp.Factories;
using ShopifySharp.Lists;

namespace TestingProject.Shopify_Api.Controllers;

[TestFixture]
public class PromoControllerTest
{
    private Mock<IPriceRuleServiceFactory> _mockPriceRuleServiceFactory;
    private Mock<IPriceRuleService> _mockPriceRuleService;
    private Mock<IDiscountCodeServiceFactory> _mockDiscountCodeServiceFactory;
    private Mock<IDiscountCodeService> _mockDiscountCodeService;
    private ShopifyRestApiCredentials _falseCredentials;
    private PromoController _controller;
    
    [SetUp]
    public void Setup()
    {
        _mockPriceRuleServiceFactory = new Mock<IPriceRuleServiceFactory>();
        _mockPriceRuleService = new Mock<IPriceRuleService>();
        _mockDiscountCodeServiceFactory = new Mock<IDiscountCodeServiceFactory>();
        _mockDiscountCodeService = new Mock<IDiscountCodeService>();
        _falseCredentials = new ShopifyRestApiCredentials("NotARealURL", "NotARealToken");

        // Set up mocks to return services when Create is called
        _mockPriceRuleServiceFactory.Setup(x => x.Create(It.IsAny<ShopifyApiCredentials>())).Returns(_mockPriceRuleService.Object);
        _mockDiscountCodeServiceFactory.Setup(x => x.Create(It.IsAny<ShopifyApiCredentials>())).Returns(_mockDiscountCodeService.Object);

        _controller = new PromoController(
            _mockPriceRuleServiceFactory.Object,
            _mockDiscountCodeServiceFactory.Object,
            _falseCredentials
        );
    }
    
    //=============== PRICE RULES =================
    //===============GET ALL PRICE RULES=================
    [Test]
    public async Task GetAllPriceRules_ReturnsOk_WhenPriceRulesAreFetchedSuccessfully()
    {
        // Arrange
        var priceRuleList = new List<PriceRule>
        {
            new PriceRule { Id = 1, Title = "Discount 1" },
            new PriceRule { Id = 2, Title = "Discount 2" }
        };
        
        var listResult = new ShopifySharp.Lists.ListResult<PriceRule>(priceRuleList, default);
        
        // Mock the ListAsync method to return the list of price rules
        _mockPriceRuleService.Setup(x => x.ListAsync(null, default)).ReturnsAsync(listResult);

        // Act
        var result = await _controller.GetAllPriceRules();

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        
        var returnedPriceRules = okResult.Value as ShopifySharp.Lists.ListResult<PriceRule>;
        Console.WriteLine("Returned prices" + returnedPriceRules);
        Assert.IsNotNull(returnedPriceRules);
        
        var returnedOrders = returnedPriceRules.Items;
        Assert.That(returnedOrders, Is.EqualTo(priceRuleList));
    }

    [Test]
    public async Task GetAllPriceRules_ReturnsNotFound_WhenShopifyExceptionIsThrown()
    {
        // Arrange
        _mockPriceRuleService.Setup(x => x.ListAsync(null, default)).ThrowsAsync(new ShopifyException("Shopify error"));

        // Act
        var result = await _controller.GetAllPriceRules();

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.That(objectResult.StatusCode, Is.EqualTo(404));

        var value = JObject.FromObject(objectResult.Value);
        Assert.That(value["message"]?.ToString(), Is.EqualTo("Error fetching price rules"));
    }

    [Test]
    public async Task GetAllPriceRules_ReturnsInternalServerError_WhenUnexpectedExceptionOccurs()
    {
        // Arrange
        _mockPriceRuleService.Setup(x => x.ListAsync(null, default)).ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await _controller.GetAllPriceRules();

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));

        var responseBody = JObject.FromObject(objectResult.Value);
        Assert.That(responseBody["message"]?.ToString(), Is.EqualTo("Error fetching price rules"));
    }
    
    //===============GET PRICE RULES BY ID=================

    [Test]
    public async Task GetPriceRulesById_ReturnsOk_WhenPriceRuleExists()
    {
        // Arrange
        long priceRuleId = 123;
        var mockPriceRule = new PriceRule { Id = priceRuleId, Title = "Discount 1" };
        _mockPriceRuleService
            .Setup(service => service.GetAsync(priceRuleId, default, default))
            .ReturnsAsync(mockPriceRule);

        // Act
        var result = await _controller.GetPriceRulesById(priceRuleId);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.AreEqual(mockPriceRule, okResult.Value);
    }
    
    [Test]
    public async Task GetPriceRulesById_ThrowsException_WhenShopifyExceptionIsThrown()
    {
        // Arrange
        long priceRuleId = 123;
        var exceptionMessage = "Shopify API error";
        _mockPriceRuleService
            .Setup(service => service.GetAsync(priceRuleId, default, default))
            .ThrowsAsync(new ShopifyException(exceptionMessage));

        // Act
        var result = await _controller.GetPriceRulesById(priceRuleId);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.AreEqual(404, objectResult.StatusCode);

        var responseBody = JObject.FromObject(objectResult.Value);
        Assert.That(responseBody["message"]?.ToString(), Is.EqualTo("Error fetching price rule"));
        Assert.That(responseBody["details"]?.ToString(), Is.EqualTo(exceptionMessage));
    }

    [Test]
    public async Task GetPriceRulesById_ThrowsException_WhenUnexpectedExceptionOccurs()
    {
        // Arrange
        long priceRuleId = 123;
        var exceptionMessage = "Unexpected error";
        _mockPriceRuleService
            .Setup(service => service.GetAsync(priceRuleId, default, default))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _controller.GetPriceRulesById(priceRuleId);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.AreEqual(500, objectResult.StatusCode);

        var responseBody = JObject.FromObject(objectResult.Value);
        Assert.That(responseBody["message"]?.ToString(), Is.EqualTo("Error fetching price rule"));
        Assert.That(responseBody["details"]?.ToString(), Is.EqualTo(exceptionMessage));
    }

    //=============== DELETE PRICE RULES =================
    [Test]
    public async Task DeletePriceRule_ReturnsOk_WhenPriceRuleDeletedSuccessfully()
    {
        // Arrange
        long priceRuleId = 123;
        _mockPriceRuleService
            .Setup(service => service.DeleteAsync(priceRuleId, default))
            .Returns(Task.CompletedTask); // Simulate successful deletion

        // Act
        var result = await _controller.DeletePriceRule(priceRuleId);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.AreEqual("Price rule deleted", okResult.Value);
    }

    [Test]
    public async Task DeletePriceRule_ReturnsNotFound_WhenPriceRuleDoesNotExist()
    {
        // Arrange
        long priceRuleId = 123;
        _mockPriceRuleService
            .Setup(service => service.DeleteAsync(priceRuleId, default))
            .ThrowsAsync(new ShopifyException("Price rule not found")); // Simulate error when price rule is not found

        // Act
        var result = await _controller.DeletePriceRule(priceRuleId);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.AreEqual(404, objectResult.StatusCode);

        var responseBody = JObject.FromObject(objectResult.Value);
        Assert.That(responseBody["message"]?.ToString(), Is.EqualTo("No price rule found"));
    }

    [Test]
    public async Task DeletePriceRule_ThrowsException_WhenUnexpectedExceptionOccurs()
    {
        // Arrange
        long priceRuleId = 123;
        var exceptionMessage = "Unexpected error";
        _mockPriceRuleService
            .Setup(service => service.DeleteAsync(priceRuleId, default))
            .ThrowsAsync(new Exception(exceptionMessage)); // Simulate an unexpected error during deletion

        // Act
        var result = await _controller.DeletePriceRule(priceRuleId);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.AreEqual(500, objectResult.StatusCode);

        var responseBody = JObject.FromObject(objectResult.Value);
        Assert.That(responseBody["message"]?.ToString(), Is.EqualTo("Error deleting price rules"));
    }

    //=============== DISCOUNT CODES =================
    //=============== GET ALL DISCOUNTS =================
    [Test]
    public async Task GetAllDiscountsByRule_ReturnsOk_WhenDiscountsAreFetchedSuccessfully()
    {
        // Arrange
        long priceRuleId = 123;
        var mockDiscounts = new List<PriceRuleDiscountCode>
        {
            new PriceRuleDiscountCode { Code = "DISCOUNT1"},
            new PriceRuleDiscountCode { Code = "DISCOUNT2"}
        };
        
        var listResult = new ShopifySharp.Lists.ListResult<PriceRuleDiscountCode>(mockDiscounts, default);
        
        _mockDiscountCodeService
            .Setup(service => service.ListAsync(priceRuleId, default, default))
            .ReturnsAsync(listResult); // Simulate fetching discounts successfully

        // Act
        var result = await _controller.GetAllDiscountsByRule(priceRuleId);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
    }

    [Test]
    public async Task GetAllDiscountsByRule_ThrowsException_WhenShopifyExceptionIsThrown()
    {
        // Arrange
        long priceRuleId = 123;
        var exceptionMessage = "Shopify API error";
        _mockDiscountCodeService
            .Setup(service => service.ListAsync(priceRuleId, default, default))
            .ThrowsAsync(new ShopifyException(exceptionMessage)); // Simulate ShopifyException

        // Act
        var result = await _controller.GetAllDiscountsByRule(priceRuleId);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.AreEqual(404, objectResult.StatusCode);

        var responseBody = JObject.FromObject(objectResult.Value);
        Assert.That(responseBody["message"]?.ToString(), Is.EqualTo("Error fetching discounts"));
        Assert.That(responseBody["details"]?.ToString(), Is.EqualTo(exceptionMessage));
    }

    [Test]
    public async Task GetAllDiscountsByRule_ThrowsException_WhenUnexpectedExceptionOccurs()
    {
        // Arrange
        long priceRuleId = 123;
        var exceptionMessage = "Unexpected error";
        _mockDiscountCodeService
            .Setup(service => service.ListAsync(priceRuleId, default, default))
            .ThrowsAsync(new Exception(exceptionMessage)); // Simulate an unexpected error

        // Act
        var result = await _controller.GetAllDiscountsByRule(priceRuleId);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.AreEqual(500, objectResult.StatusCode);

        var responseBody = JObject.FromObject(objectResult.Value);
        Assert.That(responseBody["message"]?.ToString(), Is.EqualTo("Error fetching product"));
        Assert.That(responseBody["details"]?.ToString(), Is.EqualTo(exceptionMessage));
    }

    //=============== POST DISCOUNTS =================

    [Test]
    public async Task PostDiscount_ReturnsOk_WhenDiscountIsCreatedSuccessfully()
    {
        // Arrange
        long priceRuleId = 123;
        var request = new PriceRuleDiscountCode { Code = "DISCOUNT10" };
        var createdDiscount = new PriceRuleDiscountCode { Code = "DISCOUNT10" };
    
        _mockDiscountCodeService
            .Setup(service => service.CreateAsync(priceRuleId, request, default))
            .ReturnsAsync(createdDiscount); // Simulate successful creation

        // Act
        var result = await _controller.PostDiscount(priceRuleId, request);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.AreEqual(createdDiscount, okResult.Value);
    }
    
    [Test]
    public async Task PostDiscount_ReturnsBadRequest_WhenInputExceptionIsThrown()
    {
        // Arrange
        long priceRuleId = 123;
        var request = new PriceRuleDiscountCode { Code = "INVALID" };
        var exceptionMessage = "Invalid input data";

        _mockDiscountCodeService
            .Setup(service => service.CreateAsync(priceRuleId, request, default))
            .ThrowsAsync(new InputException(exceptionMessage)); // Simulate input validation failure

        // Act
        var result = await _controller.PostDiscount(priceRuleId, request);

        // Assert
        var badRequestResult = result as ObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);

        var responseBody = JObject.FromObject(badRequestResult.Value);
        Assert.That(responseBody["message"]?.ToString(), Is.EqualTo(exceptionMessage));
    }
    
    [Test]
    public async Task PostDiscount_ReturnsServerError_WhenShopifyExceptionIsThrown()
    {
        // Arrange
        long priceRuleId = 123;
        var request = new PriceRuleDiscountCode { Code = "DISCOUNT10" };
        var exceptionMessage = "Shopify API error";

        _mockDiscountCodeService
            .Setup(service => service.CreateAsync(priceRuleId, request, default))
            .ThrowsAsync(new ShopifyException(exceptionMessage)); // Simulate Shopify exception

        // Act
        var result = await _controller.PostDiscount(priceRuleId, request);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.AreEqual(500, objectResult.StatusCode);

        var responseBody = JObject.FromObject(objectResult.Value);
        Assert.That(responseBody["message"]?.ToString(), Is.EqualTo("Error creating discounts"));
        Assert.That(responseBody["details"]?.ToString(), Is.EqualTo(exceptionMessage));
    }
    
    [Test]
    public async Task PostDiscount_ReturnsServerError_WhenUnexpectedExceptionIsThrown()
    {
        // Arrange
        long priceRuleId = 123;
        var request = new PriceRuleDiscountCode { Code = "DISCOUNT10" };
        var exceptionMessage = "Unexpected error";

        _mockDiscountCodeService
            .Setup(service => service.CreateAsync(priceRuleId, request, default))
            .ThrowsAsync(new Exception(exceptionMessage)); // Simulate unexpected error

        // Act
        var result = await _controller.PostDiscount(priceRuleId, request);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.AreEqual(500, objectResult.StatusCode);

        var responseBody = JObject.FromObject(objectResult.Value);
        Assert.That(responseBody["message"]?.ToString(), Is.EqualTo("Error creating discount " + exceptionMessage));
    }
    
    //=============== DELETE DISCOUNTS =================
    [Test]
    public async Task DeleteDiscount_ReturnsOk_WhenDiscountIsDeletedSuccessfully()
    {
        // Arrange
        long priceRuleId = 123;
        long discountId = 456;

        _mockDiscountCodeService
            .Setup(service => service.DeleteAsync(priceRuleId, discountId, default))
            .Returns(Task.CompletedTask); 

        // Act
        var result = await _controller.DeleteDiscount(priceRuleId, discountId);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.AreEqual("Discount deleted", okResult.Value);
    }
    
    [Test]
    public async Task DeleteDiscount_ReturnsNotFound_WhenShopifyExceptionIsThrown()
    {
        // Arrange
        long priceRuleId = 123;
        long discountId = 456;
        var exceptionMessage = "Discount not found";

        _mockDiscountCodeService
            .Setup(service => service.DeleteAsync(priceRuleId, discountId, default))
            .ThrowsAsync(new ShopifyException(exceptionMessage)); // Simulate Shopify exception for not found

        // Act
        var result = await _controller.DeleteDiscount(priceRuleId, discountId);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.AreEqual(404, objectResult.StatusCode);

        var responseBody = JObject.FromObject(objectResult.Value);
        Assert.That(responseBody["message"]?.ToString(), Is.EqualTo("No discount found"));
        Assert.That(responseBody["details"]?.ToString(), Is.EqualTo(exceptionMessage));
    }
    
    [Test]
    public async Task DeleteDiscount_ReturnsServerError_WhenUnexpectedExceptionIsThrown()
    {
        // Arrange
        long priceRuleId = 123;
        long discountId = 456;
        var exceptionMessage = "Unexpected error";

        _mockDiscountCodeService
            .Setup(service => service.DeleteAsync(priceRuleId, discountId, default))
            .ThrowsAsync(new Exception(exceptionMessage)); // Simulate unexpected exception

        // Act
        var result = await _controller.DeleteDiscount(priceRuleId, discountId);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.AreEqual(500, objectResult.StatusCode);

        var responseBody = JObject.FromObject(objectResult.Value);
        Assert.That(responseBody["message"]?.ToString(), Is.EqualTo("Error deleting Discount" + exceptionMessage));
    }

}
