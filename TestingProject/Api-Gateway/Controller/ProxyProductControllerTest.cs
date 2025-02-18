using System.Net;
using System.Text;
using Api_Gateway.Controller;
using Api_Gateway.Models;
using Api_Gateway.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ShopifySharp;

namespace TestingProject.Api_Gateway.Controller;

[TestFixture]
public class ProxyProductControllerTest
{
    private Mock<IHttpClientFactory> _mockHttpClientFactory; // Mock any dependencies
    private Mock<ServiceProductController> _mockServiceProductController; // Mock ServiceProductController
    private ProxyProductController _proxyProductController; // Controller under test

    [SetUp]
    public void SetUp()
    {
        // Mock IHttpClientFactory as a dependency for ServiceProductController
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();

        // Mock ServiceProductController
        _mockServiceProductController = new Mock<ServiceProductController>(_mockHttpClientFactory.Object);

        // Create the ProxyProductController, passing the mocked ServiceProductController
        _proxyProductController = new ProxyProductController(_mockServiceProductController.Object);
    }

    [Test]
    public async Task GetAllProducts_ReturnsCorrectResult()
    {
        // Arrange: Set up the expected result (as if returned by the real API)
        var expectedResult = "{\"products\": [{\"id\": 1, \"name\": \"Product1\"}]}";  // Example response

        // Set up the mock to return the expected result for GetAllProductsAsync
        _mockServiceProductController
            .Setup(controller => controller.GetAllProductsAsync(null))
            .ReturnsAsync(expectedResult);

        // Act: Call the GetAllProducts method of ProxyProductController
        var result = await _proxyProductController.GetAllProducts();
        Console.WriteLine($"Result: {result}");

        // Assert: Check that the result is an OkObjectResult (for 200 OK response)
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult); // Ensure the result is of type OkObjectResult
        Assert.That(okResult.Value, Is.EqualTo(expectedResult)); // The content should match the expected result
        Assert.That(okResult.StatusCode, Is.EqualTo(200)); // The status code should be 200
    }

    [Test]
    public async Task GetAllProducts_ReturnsInternalServerError_WhenExceptionOccurs()
    {
        // Arrange: Set up ServiceProductController to throw an exception
        _mockServiceProductController
            .Setup(controller => controller.GetAllProductsAsync(null))
            .ThrowsAsync(new System.Exception("Unexpected error"));

        // Act: Call the GetAllProducts method of ProxyProductController
        var result = await _proxyProductController.GetAllProducts();

        // Assert: Check that the result is an ObjectResult
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult); // Ensure the result is of type ObjectResult
        Assert.AreEqual(500, objectResult.StatusCode); // Ensure status code is 500
        Assert.IsNotNull(objectResult.Value); // Ensure there is a message in the response
    }
    [Test]
    public async Task GetAllProducts_ReturnsInternalServerError_WhenResultStartsWithError()
    {
        // Arrange: Set up the ServiceProductController to return a string starting with "Error"
        var errorMessage = "Error: Unable to fetch products";
        _mockServiceProductController
            .Setup(controller => controller.GetAllProductsAsync(null))
            .ReturnsAsync(errorMessage);

        // Act: Call the GetAllProducts method of ProxyProductController
        var result = await _proxyProductController.GetAllProducts();

        // Assert: Check that the result is an ObjectResult
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult); // Ensure the result is of type ObjectResult
        Assert.AreEqual(500, objectResult.StatusCode); // Ensure status code is 500
        Assert.IsNotNull(objectResult.Value); // Ensure there is a message in the response
    }
    // ------------------------- GET BY ID PRODUCT
    [Test]
    public async Task GetProductById_ReturnsOk_WhenProductIsFound()
    {
        // Arrange
        var productId = 1L; // The product ID to look for
        var expectedProduct = "{\"id\": 1, \"name\": \"Product1\"}"; // Example product data

        // Mock the GetProductByIdAsync to return a product as JSON
        _mockServiceProductController
            .Setup(controller => controller.GetProductByIdAsync(productId))
            .ReturnsAsync(expectedProduct);

        // Act: Call the GetProductById method of ProxyProductController
        var result = await _proxyProductController.GetProductById(productId);

        // Assert: Check that the result is OkObjectResult (200 OK)
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult); // Ensure the result is of type OkObjectResult
        Assert.That(okResult.Value, Is.EqualTo(expectedProduct)); // The content should match the expected result
        Assert.That(okResult.StatusCode, Is.EqualTo(200)); // The status code should be 200
    }
    
    [Test]
    public async Task GetProductById_ReturnsNotFound_WhenProductIsNotFound()
    {
        // Arrange: Set up the mock to return a 404 Not Found response
        var errorMessage = "404 Not Found: Product not found";
        _mockServiceProductController
            .Setup(controller => controller.GetProductByIdAsync(It.IsAny<long>()))
            .ReturnsAsync(errorMessage);

        // Act: Call the GetProductById method of ProxyProductController
        var result = await _proxyProductController.GetProductById(1);

        // Assert: Check that the result is a NotFoundObjectResult
        var notFoundResult = result as NotFoundObjectResult;
        Assert.IsNotNull(notFoundResult); // Ensure the result is of type NotFoundObjectResult

        // Deserialize the response using Newtonsoft.Json
        var responseMessage = JsonConvert.SerializeObject(notFoundResult.Value);

        // Now compare the message (make sure to extract the value from JValue)
        dynamic deserializedResponse = JsonConvert.DeserializeObject(responseMessage);
        Assert.AreEqual("404 Not Found: Product not found", (string)deserializedResponse.message); // Compare message content
    }



    [Test]
    public async Task GetProductById_ReturnsInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var productId = 1L; // Example product ID
        var exceptionMessage = "Unexpected error occurred";

        // Mock the GetProductByIdAsync to throw an exception
        _mockServiceProductController
            .Setup(controller => controller.GetProductByIdAsync(productId))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act: Call the GetProductById method of ProxyProductController
        var result = await _proxyProductController.GetProductById(productId);

        // Assert: Check that the result is ObjectResult (500 Internal Server Error)
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult); // Ensure the result is of type ObjectResult
        Assert.AreEqual(500, objectResult.StatusCode); // Ensure the status code is 500
        Assert.IsNotNull(objectResult.Value); // Ensure there is an error message in the response
    }


    
    // ----------------------- POST PRODUCT
    [Test]
    public async Task PostProduct_ReturnsCreated_WhenProductIsSuccessfullyCreated()
    {
        // Arrange
        var product = new Product { Title = "New Product" };
        var expectedResponse = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent("{\"id\": 1, \"title\": \"New Product\"}", Encoding.UTF8, "application/json")
        };
        
        _mockServiceProductController
            .Setup(controller => controller.CreateProductAsync(product))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _proxyProductController.PostProduct(product);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.AreEqual(201, objectResult.StatusCode);
        Assert.AreEqual("{\"id\": 1, \"title\": \"New Product\"}", objectResult.Value);
    }
    [Test]
    public async Task PostProduct_ReturnsServiceUnavailable_WhenServiceIsUnavailable()
    {
        // Arrange
        var product = new Product { Title = "New Product" };
        var expectedResponse = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);

        _mockServiceProductController
            .Setup(controller => controller.CreateProductAsync(product))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _proxyProductController.PostProduct(product);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.That(objectResult.StatusCode, Is.EqualTo(503));

        // Deserialize the value to a dynamic object using Newtonsoft.Json
        var responseContent = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(objectResult.Value));

        Assert.That((string)responseContent.message, Is.EqualTo("Service is currently unavailable, please try again later."));
    }
    [Test]
    public async Task PostProduct_ReturnsBadRequest_WhenRequestIsInvalid()
    {
        // Arrange
        var product = new Product(); // Invalid product
        var expectedResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("Invalid product data", Encoding.UTF8, "application/json")
        };

        _mockServiceProductController
            .Setup(controller => controller.CreateProductAsync(product))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _proxyProductController.PostProduct(product);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.AreEqual(400, objectResult.StatusCode);
        Assert.AreEqual("Invalid product data", objectResult.Value);
    }

    [Test]
    public async Task PostProduct_ReturnsInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var product = new Product { Title = "New Product" };

        _mockServiceProductController
            .Setup(controller => controller.CreateProductAsync(product))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await _proxyProductController.PostProduct(product);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.AreEqual(500, objectResult.StatusCode);

        // Deserialize the value to a dynamic object using Newtonsoft.Json
        var responseContent = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(objectResult.Value));

        Assert.AreEqual("An error occurred", (string)responseContent.message);
        Assert.AreEqual("Unexpected error", (string)responseContent.details);
    }
    [Test]
    public async Task PostProduct_ReturnsRequestTimeout_WhenRequestTimesOut()
    {
        // Arrange
        var product = new Product { Title = "New Product" };
        var expectedResponse = new HttpResponseMessage(HttpStatusCode.RequestTimeout)
        {
            Content = new StringContent("Request timed out", Encoding.UTF8, "application/json")
        };

        _mockServiceProductController
            .Setup(controller => controller.CreateProductAsync(product))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _proxyProductController.PostProduct(product);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.AreEqual(408, objectResult.StatusCode);
        Assert.AreEqual("Request timed out", objectResult.Value);
    }

    
    // ----------------------- PUT PRODUCT
    
    [Test]
    public async Task PutProduct_ReturnsOk_WhenProductIsSuccessfullyUpdated()
    {
        // Arrange
        var productId = 1;
        var product = new Product { Id = productId, Title = "Updated Product" };
        var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"id\": 1, \"title\": \"Updated Product\"}", Encoding.UTF8, "application/json")
        };

        _mockServiceProductController
            .Setup(controller => controller.PutProductAsync(productId, product))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _proxyProductController.PutProduct(productId, product);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.AreEqual(200, objectResult.StatusCode);
        Assert.AreEqual("{\"id\": 1, \"title\": \"Updated Product\"}", objectResult.Value);
    }

    [Test]
    public async Task PutProduct_ReturnsServiceUnavailable_WhenServiceIsUnavailable()
    {
        // Arrange
        var productId = 1;
        var product = new Product { Id = productId, Title = "Updated Product" };
        var expectedResponse = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);

        _mockServiceProductController
            .Setup(controller => controller.PutProductAsync(productId, product))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _proxyProductController.PutProduct(productId, product);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.AreEqual(503, objectResult.StatusCode);

        // Deserialize the value to a dynamic object using Newtonsoft.Json
        var responseContent = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(objectResult.Value));

        Assert.AreEqual("Service is currently unavailable, please try again later.", (string)responseContent.message);
    }
    
    [Test]
    public async Task PutProduct_ReturnsBadRequest_WhenRequestIsInvalid()
    {
        // Arrange
        var productId = 1;
        var product = new Product(); // Invalid product
        var expectedResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("Invalid product data", Encoding.UTF8, "application/json")
        };

        _mockServiceProductController
            .Setup(controller => controller.PutProductAsync(productId, product))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _proxyProductController.PutProduct(productId, product);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.AreEqual(400, objectResult.StatusCode);
        Assert.AreEqual("Invalid product data", objectResult.Value);
    }

    [Test]
    public async Task PutProduct_ReturnsInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var productId = 1;
        var product = new Product { Id = productId, Title = "Updated Product" };

        _mockServiceProductController
            .Setup(controller => controller.PutProductAsync(productId, product))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await _proxyProductController.PutProduct(productId, product);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.AreEqual(500, objectResult.StatusCode);

        // Deserialize the value to a dynamic object using Newtonsoft.Json
        var responseContent = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(objectResult.Value));

        Assert.AreEqual("An error occurred", (string)responseContent.message);
        Assert.AreEqual("Unexpected error", (string)responseContent.details);
    }

    [Test]
    public async Task PutProduct_ReturnsRequestTimeout_WhenRequestTimesOut()
    {
        // Arrange
        var productId = 1;
        var product = new Product { Id = productId, Title = "Updated Product" };
        var expectedResponse = new HttpResponseMessage(HttpStatusCode.RequestTimeout)
        {
            Content = new StringContent("Request timed out", Encoding.UTF8, "application/json")
        };

        _mockServiceProductController
            .Setup(controller => controller.PutProductAsync(productId, product))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _proxyProductController.PutProduct(productId, product);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.AreEqual(408, objectResult.StatusCode);
        Assert.AreEqual("Request timed out", objectResult.Value);
    }


    //---------------DELETE---------------
    [Test]
    public async Task DeleteProduct_ReturnsOk_WhenProductIsFound()
    {
        // Arrange
        var productId = 1L; // The product ID to look for
        var expectedProduct = "{\"id\": 1, \"name\": \"Product1\"}"; // Example product data

        // Mock the DeleteProductByIdAsync to return a product as JSON
        _mockServiceProductController
            .Setup(controller => controller.DeleteProductAsync(productId))
            .ReturnsAsync(expectedProduct);

        // Act: Call the DeleteProduct method of ProxyProductController
        var result = await _proxyProductController.DeleteProduct(productId);

        // Assert: Check that the result is OkObjectResult (200 OK)
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult); // Ensure the result is of type OkObjectResult
        Assert.That(okResult.Value, Is.EqualTo(expectedProduct)); // The content should match the expected result
        Assert.That(okResult.StatusCode, Is.EqualTo(200)); // The status code should be 200
    }

    
    [Test]
    public async Task DeleteProduct_ReturnsNotFound_WhenProductIsNotFound()
    {
        // Arrange: Set up the mock to return a 404 Not Found response
        var errorMessage = "404 Not Found: Product not found";
        _mockServiceProductController
            .Setup(controller => controller.DeleteProductAsync(It.IsAny<long>()))
            .ReturnsAsync(errorMessage);

        // Act: Call the DeleteProduct method of ProxyProductController
        var result = await _proxyProductController.DeleteProduct(1);

        // Assert: Check that the result is a NotFoundObjectResult
        var notFoundResult = result as NotFoundObjectResult;
        Assert.IsNotNull(notFoundResult); // Ensure the result is of type NotFoundObjectResult

        // Deserialize the response using Newtonsoft.Json
        var responseMessage = JsonConvert.SerializeObject(notFoundResult.Value);

        // Now compare the message (make sure to extract the value from JValue)
        dynamic deserializedResponse = JsonConvert.DeserializeObject(responseMessage);
        Assert.AreEqual("404 Not Found: Product not found", (string)deserializedResponse.message); // Compare message content
    }

    
    [Test]
    public async Task DeleteProduct_ReturnsInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var productId = 1L; // Example product ID
        var exceptionMessage = "Unexpected error occurred";

        // Mock the DeleteProductByIdAsync to throw an exception
        _mockServiceProductController
            .Setup(controller => controller.DeleteProductAsync(productId))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act: Call the DeleteProduct method of ProxyProductController
        var result = await _proxyProductController.DeleteProduct(productId);

        // Assert: Check that the result is ObjectResult (500 Internal Server Error)
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult); // Ensure the result is of type ObjectResult
        Assert.AreEqual(500, objectResult.StatusCode); // Ensure the status code is 500
        Assert.IsNotNull(objectResult.Value); // Ensure there is an error message in the response
    }
    
    //---------------GET VARIANT---------------

    [Test]
    public async Task GetFirstVariantByProductId_ReturnsOk_WhenProductWithVariantsIsFetchedSuccessfully()
    {
        // Arrange
        long productId = 1;
        long variantId = 45205286223917;
        var productJson = "{\"id\": " + productId + ", \"variants\": [{\"id\": " + variantId + "}]}";

        _mockServiceProductController.Setup(controller => controller.GetProductByIdAsync(productId))
            .ReturnsAsync(productJson);

        // Act
        var result = await _proxyProductController.GetFirstVariantByProductId(productId);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult, "Expected OkObjectResult.");
        Assert.AreEqual(200, okResult.StatusCode);
        
        var variantData = JsonConvert.SerializeObject(okResult.Value);

        Assert.IsNotNull(variantData, "Expected variant data to be not null.");
    }

    [Test]
    public async Task GetFirstVariantByProductId_ReturnsNotFound_WhenProductNotFound()
    {
        // Arrange
        long productId = 1;
        _mockServiceProductController.Setup(controller => controller.GetProductByIdAsync(productId))
            .ReturnsAsync("404 Not Found: Product not found");

        // Act
        var result = await _proxyProductController.GetFirstVariantByProductId(productId);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        Assert.IsNotNull(notFoundResult, "Expected NotFoundObjectResult.");
        Assert.AreEqual(404, notFoundResult.StatusCode);

        var responseMessage = JsonConvert.SerializeObject(notFoundResult.Value);
        dynamic deserializedResponse = JsonConvert.DeserializeObject(responseMessage);
        Assert.AreEqual("Product not found.", (string)deserializedResponse.message);
    }

    [Test]
    public async Task GetFirstVariantByProductId_ReturnsNotFound_WhenNoVariantsAvailable()
    {
        // Arrange
        long productId = 1;
        var productJson = "{\"id\": " + productId + ", \"variants\": []}";

        _mockServiceProductController.Setup(controller => controller.GetProductByIdAsync(productId))
            .ReturnsAsync(productJson);

        // Act
        var result = await _proxyProductController.GetFirstVariantByProductId(productId);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        Assert.IsNotNull(notFoundResult, "Expected NotFoundObjectResult.");
        Assert.AreEqual(404, notFoundResult.StatusCode);

        var responseMessage = JsonConvert.SerializeObject(notFoundResult.Value);
        dynamic deserializedResponse = JsonConvert.DeserializeObject(responseMessage);
        Assert.AreEqual("No variants available for the specified product.", (string)deserializedResponse.message);
    }

    [Test]
    public async Task GetFirstVariantByProductId_ReturnsNotFound_WhenFirstVariantIdIsNull()
    {
        // Arrange
        long productId = 1;
        var productJson = "{\"id\": " + productId + ", \"variants\": [{}]}"; // Variant with no ID

        _mockServiceProductController.Setup(controller => controller.GetProductByIdAsync(productId))
            .ReturnsAsync(productJson);

        // Act
        var result = await _proxyProductController.GetFirstVariantByProductId(productId);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        Assert.IsNotNull(notFoundResult, "Expected NotFoundObjectResult.");
        Assert.AreEqual(404, notFoundResult.StatusCode);

        var responseMessage = JsonConvert.SerializeObject(notFoundResult.Value);
        dynamic deserializedResponse = JsonConvert.DeserializeObject(responseMessage);
        Assert.AreEqual("First variant ID is null or unavailable.", (string)deserializedResponse.message);
    }
    
    [Test]
    public async Task GetFirstVariantByProductId_ReturnsInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var productId = 1L; // Example product ID
        var exceptionMessage = "Unexpected error occurred";

        // Mock the GetProductByIdAsync to throw an exception
        _mockServiceProductController
            .Setup(controller => controller.GetProductByIdAsync(productId))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act: Call the GetFirstVariantByProductId method of ProxyProductController
        var result = await _proxyProductController.GetFirstVariantByProductId(productId);

        // Assert: Check that the result is ObjectResult (500 Internal Server Error)
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult); // Ensure the result is of type ObjectResult
        Assert.AreEqual(500, objectResult.StatusCode); // Ensure the status code is 500
        Assert.IsNotNull(objectResult.Value); // Ensure there is an error message in the response

        // Deserialize the error message using Newtonsoft.Json
        var responseMessage = JsonConvert.SerializeObject(objectResult.Value);
        dynamic deserializedResponse = JsonConvert.DeserializeObject(responseMessage);
        Assert.AreEqual("Error fetching product variants", (string)deserializedResponse.message); // Compare message content
        Assert.AreEqual(exceptionMessage, (string)deserializedResponse.details); // Compare details content
    }

    //================================ TRANSLATED METAFIELD ENDPOINTS ==================================

    [Test]
    public async Task GetTranslatedProduct_ReturnsOk_WhenTranslationExists()
    {
        // Arrange
        long productId = 1;
        string lang = "fr";
        var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"productId\":1,\"title\":\"Produit Exemple\",\"description\":\"Ceci est un produit d'exemple\"}", Encoding.UTF8, "application/json")
        };

        _mockServiceProductController
            .Setup(x => x.GetTranslatedProductAsync(productId, lang))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _proxyProductController.GetTranslatedProduct(productId, lang);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        Assert.That(okResult.Value, Is.EqualTo(await expectedResponse.Content.ReadAsStringAsync()));
    }
    
    [Test]
    public async Task GetTranslatedProduct_ReturnsError_WhenApiCallFails()
    {
        // Arrange
        long productId = 1;
        string lang = "fr";
        var expectedResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("Error fetching translation")
        };

        _mockServiceProductController
            .Setup(x => x.GetTranslatedProductAsync(productId, lang))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _proxyProductController.GetTranslatedProduct(productId, lang);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.That(objectResult.StatusCode, Is.EqualTo(400));
        Assert.That(objectResult.Value, Is.EqualTo("Error fetching translation"));
    }
    
    [Test]
    public async Task GetTranslatedProduct_ReturnsInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        long productId = 1;
        string lang = "fr";

        _mockServiceProductController
            .Setup(x => x.GetTranslatedProductAsync(productId, lang))
            .ThrowsAsync(new Exception("Unexpected error occurred"));

        // Act
        var result = await _proxyProductController.GetTranslatedProduct(productId, lang);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));

        var responseContent = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(objectResult.Value));
        Assert.AreEqual("Unexpected error occurred", (string)responseContent.Message);
    }
    
    [Test]
    public async Task AddProductTranslation_ReturnsOk_WhenTranslationIsAdded()
    {
        // Arrange
        long productId = 1;
        var translationRequest = new TranslationRequest
        {
            Locale = "fr",
            Title = "Produit Exemple",
            Description = "Ceci est un produit d'exemple"
        };

        var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"message\": \"Translation added!\", \"productId\": 1}", Encoding.UTF8, "application/json")
        };

        _mockServiceProductController
            .Setup(x => x.AddProductTranslationAsync(productId, translationRequest))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _proxyProductController.AddProductTranslation(productId, translationRequest);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        Assert.That(okResult.Value, Is.EqualTo(await expectedResponse.Content.ReadAsStringAsync()));
    }
    
    [Test]
    public async Task AddProductTranslation_ReturnsError_WhenApiCallFails()
    {
        // Arrange
        long productId = 1;
        var translationRequest = new TranslationRequest { Locale = "fr", Title = "Produit Exemple", Description = "Ceci est un produit d'exemple" };
        
        var expectedResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("Error saving translation")
        };

        _mockServiceProductController
            .Setup(x => x.AddProductTranslationAsync(productId, translationRequest))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _proxyProductController.AddProductTranslation(productId, translationRequest);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.That(objectResult.StatusCode, Is.EqualTo(400));
        Assert.That(objectResult.Value, Is.EqualTo("Error saving translation"));
    }
    
    [Test]
    public async Task AddProductTranslation_ReturnsInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        long productId = 1;
        var translationRequest = new TranslationRequest { Locale = "fr", Title = "Produit Exemple", Description = "Ceci est un produit d'exemple" };

        _mockServiceProductController
            .Setup(x => x.AddProductTranslationAsync(productId, translationRequest))
            .ThrowsAsync(new Exception("Unexpected error occurred"));

        // Act
        var result = await _proxyProductController.AddProductTranslation(productId, translationRequest);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));

        var responseContent = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(objectResult.Value));
        Assert.AreEqual("Unexpected error occurred", (string)responseContent.Message);
    }
    
    [Test]
    public async Task PutProductImage_ReturnsOk_WhenImageIsValidAndServiceReturnsSuccess()
    {
        // Arrange
        var validBase64Image = Convert.ToBase64String(Encoding.UTF8.GetBytes("valid-image-data"));
        var request = new ProxyProductController.ImageUploadRequest { ImageData = validBase64Image };

        // Mock the HTTP response from Shopify
        var expectedResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("{\"image_url\": \"https://example.com/image.jpg\"}", Encoding.UTF8, "application/json")
        };

        // Mock the HttpClient to return the expected response
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(expectedResponse);

        var httpClient = new HttpClient(handlerMock.Object);
        _mockHttpClientFactory
            .Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(httpClient);

        // Act
        var result = await _proxyProductController.PutProductImage(request);

        // Assert
        Assert.That(result, Is.InstanceOf<ObjectResult>());
        var objectResult = result as ObjectResult;
        Assert.That(objectResult.StatusCode, Is.EqualTo(200));

        var responseContent = JsonConvert.DeserializeObject<dynamic>(objectResult.Value.ToString());
        Assert.That(responseContent.image_url.ToString(), Is.EqualTo("https://example.com/image.jpg"));
    }
    
    [Test]
    public async Task PutProductImage_ReturnsBadRequest_WhenImageDataIsInvalid()
    {
        // Arrange
        var invalidBase64Image = "invalid-base64-data";
        var request = new ProxyProductController.ImageUploadRequest { ImageData = invalidBase64Image };

        // Act
        var result = await _proxyProductController.PutProductImage(request);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));

        // Deserialize the response to a dynamic object
        var response = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(badRequestResult.Value));
        Assert.That(response.message.ToString(), Is.EqualTo("Invalid base64 image data."));
    }
    
    [Test]
    public async Task PutProductImage_ReturnsServiceUnavailable_WhenServiceReturns503()
    {
        // Arrange
        var validBase64Image = Convert.ToBase64String(Encoding.UTF8.GetBytes("valid-image-data"));
        var request = new ProxyProductController.ImageUploadRequest { ImageData = validBase64Image };

        var expectedResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.ServiceUnavailable
        };

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(expectedResponse);

        var httpClient = new HttpClient(handlerMock.Object);
        _mockHttpClientFactory
            .Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(httpClient);

        // Act
        var result = await _proxyProductController.PutProductImage(request);

        // Assert
        Assert.That(result, Is.InstanceOf<ObjectResult>());
        var objectResult = result as ObjectResult;
        Assert.That(objectResult.StatusCode, Is.EqualTo(400));
        Assert.That(objectResult.Value, Is.EqualTo( "Error uploading image: Service Unavailable"));
    }
    
    

}