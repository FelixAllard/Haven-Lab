using System.Net;
using System.Text;
using Api_Gateway.Controller;
using Api_Gateway.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
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
            .Setup(controller => controller.GetAllProductsAsync())
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
            .Setup(controller => controller.GetAllProductsAsync())
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
            .Setup(controller => controller.GetAllProductsAsync())
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

}