using System.Net;
using Api_Gateway.Services;
using Moq;
using Moq.Protected;
using ShopifySharp;

namespace TestingProject.Api_Gateway.Services;
[TestFixture]
public class ServiceProductControllerTest
{
    private Mock<IHttpClientFactory> _mockHttpClientFactory;
    private Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private ServiceProductController _serviceProductController;

    [SetUp]
    public void SetUp()
    {
        // Mock HttpMessageHandler
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();

        // Create HttpClient using the mocked handler
        var httpClient = new HttpClient(_mockHttpMessageHandler.Object);

        // Mock IHttpClientFactory to return the mocked HttpClient
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockHttpClientFactory.Setup(factory => factory.CreateClient(It.IsAny<string>())).Returns(httpClient);

        // Instantiate ServiceProductController with the mocked IHttpClientFactory
        _serviceProductController = new ServiceProductController(_mockHttpClientFactory.Object);
    }

    [Test]
    public async Task GetAllProductsAsync_ReturnsProducts_WhenApiCallIsSuccessful()
    {
        // Arrange
        var expectedResponse = "{\"products\": [{\"id\": 1, \"name\": \"Product1\"}]}";
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(expectedResponse)
        };

        // Setup the mock HttpMessageHandler to return a successful response
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.ToString() == "http://localhost:5106/api/Products"),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(responseMessage);

        // Act
        var result = await _serviceProductController.GetAllProductsAsync();

        // Assert
        Assert.That(result, Is.EqualTo(expectedResponse));
    }

    [Test]
    public async Task GetAllProductsAsync_ReturnsError_WhenApiCallFails()
    {
        // Arrange
        var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("Error fetching products: Bad Request")
        };

        // Setup the mock HttpMessageHandler to return a failure response
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.ToString() == "http://localhost:5106/api/Products"),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(responseMessage);

        // Act
        var result = await _serviceProductController.GetAllProductsAsync();

        // Assert
        Assert.That(result, Is.EqualTo("Error fetching products: Bad Request"));
    }

    [Test]
    public async Task GetAllProductsAsync_ReturnsExceptionMessage_WhenExceptionOccurs()
    {
        // Arrange
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new System.Exception("Unexpected error"));

        // Act
        var result = await _serviceProductController.GetAllProductsAsync();

        // Assert
        Assert.That(result, Is.EqualTo("Error: Unexpected error"));
    }
    // ------- GET Get By ID
    [Test]
    public async Task GetProductByIdAsync_ReturnsProduct_WhenApiCallIsSuccessful()
    {
        // Arrange
        long productId = 1;
        var expectedResponse = "{\"id\": 1, \"name\": \"Product1\"}";
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(expectedResponse)
        };

        // Setup the mock HttpMessageHandler to return a successful response
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.ToString() == $"http://localhost:5106/api/Products/{productId}"),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(responseMessage);

        // Act
        var result = await _serviceProductController.GetProductByIdAsync(productId);

        // Assert
        Assert.That(result, Is.EqualTo(expectedResponse));
    }

    [Test]
    public async Task GetProductByIdAsync_ReturnsError_WhenApiCallFails()
    {
        // Arrange
        long productId = 1;
        var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("Error fetching product by ID: Bad Request")
        };

        // Setup the mock HttpMessageHandler to return a failure response
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.ToString() == $"http://localhost:5106/api/Products/{productId}"),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(responseMessage);

        // Act
        var result = await _serviceProductController.GetProductByIdAsync(productId);

        // Assert
        Assert.That(result, Is.EqualTo("Error fetching product by ID: Bad Request"));
    }

    [Test]
    public async Task GetProductByIdAsync_ReturnsNotFound_WhenProductNotFound()
    {
        // Arrange
        long productId = 999; // Simulate a non-existing product
        var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent("404 Not Found: Product not found")
        };

        // Setup the mock HttpMessageHandler to return a NotFound response
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.ToString() == $"http://localhost:5106/api/Products/{productId}"),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(responseMessage);

        // Act
        var result = await _serviceProductController.GetProductByIdAsync(productId);

        // Assert
        Assert.That(result, Is.EqualTo("404 Not Found: Product not found"));
    }

    [Test]
    public async Task GetProductByIdAsync_ReturnsExceptionMessage_WhenExceptionOccurs()
    {
        // Arrange
        long productId = 1;
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new System.Exception("Unexpected error"));

        // Act
        var result = await _serviceProductController.GetProductByIdAsync(productId);

        // Assert
        Assert.That(result, Is.EqualTo("Error: Unexpected error"));
    }

    
    // ------ POST CreateProductAsync
    [Test]
    public async Task CreateProductAsync_ReturnsSuccessResponse_WhenApiCallIsSuccessful()
    {
        // Arrange
        var product = new Product
        {
            Title = "Test Product",
            BodyHtml = "Test Description"
        };

        var expectedResponseMessage = new HttpResponseMessage(HttpStatusCode.Created);

        // Setup the mock HttpMessageHandler to return a successful response
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri.ToString() == "http://localhost:5106/api/Products" &&
                    req.Content.Headers.ContentType.MediaType == "application/json"),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(expectedResponseMessage);

        // Act
        var response = await _serviceProductController.CreateProductAsync(product);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }
    [Test]
    public async Task CreateProductAsync_ReturnsServiceUnavailable_WhenHttpRequestExceptionOccurs()
    {
        // Arrange
        var product = new Product
        {
            Title = "Test Product",
            BodyHtml = "Test Description"
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new HttpRequestException("Network error"));

        // Act
        var response = await _serviceProductController.CreateProductAsync(product);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.ServiceUnavailable));
        Assert.That(await response.Content.ReadAsStringAsync(), Is.EqualTo("Error: Network error"));
    }
    [Test]
    public async Task CreateProductAsync_ReturnsInternalServerError_WhenGeneralExceptionOccurs()
    {
        // Arrange
        var product = new Product
        {
            Title = "Test Product",
            BodyHtml = "Test Description"
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var response = await _serviceProductController.CreateProductAsync(product);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        Assert.That(await response.Content.ReadAsStringAsync(), Is.EqualTo("Error: Unexpected error"));
    }
    [Test]
    public async Task CreateProductAsync_ReturnsBadRequest_WhenApiCallFails()
    {
        // Arrange
        var product = new Product
        {
            Title = "Test Product",
            BodyHtml = "Test Description"
        };

        var expectedResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("Invalid product data")
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(expectedResponseMessage);

        // Act
        var response = await _serviceProductController.CreateProductAsync(product);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        Assert.That(await response.Content.ReadAsStringAsync(), Is.EqualTo("Invalid product data"));
    }
    
    
    // ------ DELETE

    [Test]
    public async Task DeleteProductByIdAsync_DeletesProduct_WhenApiCallIsSuccessful()
    {
        // Arrange
        long productId = 1;
        var expectedResponseMessage = "Product deleted successfully.";
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(expectedResponseMessage)
        };

        // Setup the mock HttpMessageHandler to return a successful response for DELETE
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Delete && 
                    req.RequestUri.ToString() == $"http://localhost:5106/api/Products/{productId}"),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(responseMessage);

        // Act
        var result = await _serviceProductController.DeleteProductAsync(productId);

        // Assert
        Assert.That(result, Is.EqualTo(expectedResponseMessage));
    }


    [Test]
    public async Task DeleteProductByIdAsync_ReturnsError_WhenApiCallFails()
    {
        // Arrange
        long productId = 1;
        var expectedErrorMessage = "Error deleting product by ID: Bad Request";
        var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent(expectedErrorMessage)
        };

        // Setup the mock HttpMessageHandler to return a failure response for DELETE
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Delete && 
                    req.RequestUri.ToString() == $"http://localhost:5106/api/Products/{productId}"),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(responseMessage);

        // Act
        var result = await _serviceProductController.DeleteProductAsync(productId);

        // Assert
        Assert.That(result, Is.EqualTo(expectedErrorMessage));
    }


    [Test]
    public async Task DeleteProductByIdAsync_ReturnsNotFound_WhenProductNotFound()
    {
        // Arrange
        long productId = 999; // Simulate a non-existing product
        var expectedErrorMessage = "404 Not Found: Product not found";
        var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent(expectedErrorMessage)
        };

        // Setup the mock HttpMessageHandler to return a NotFound response for DELETE
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Delete && 
                    req.RequestUri.ToString() == $"http://localhost:5106/api/Products/{productId}"),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(responseMessage);

        // Act
        var result = await _serviceProductController.DeleteProductAsync(productId);

        // Assert
        Assert.That(result, Is.EqualTo(expectedErrorMessage));
    }


    [Test]
    public async Task DeleteProductByIdAsync_ReturnsExceptionMessage_WhenExceptionOccurs()
    {
        // Arrange
        long productId = 1;
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new System.Exception("Unexpected error"));

        // Act
        var result = await _serviceProductController.DeleteProductAsync(productId);

        // Assert
        Assert.That(result, Is.EqualTo("Error: Unexpected error"));
    }


    // ------ PUT
    
    [Test]
    public async Task UpdateProductAsync_ReturnsSuccessResponse_WhenApiCallIsSuccessful()
    {
        // Arrange: Create a product with updated details
        var product = new Product
        {
            Id = 1, // Assuming product with ID 1 exists
            Title = "Updated Product",
            BodyHtml = "Updated Description"
        };
    
        var expectedResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
    
        // Setup the mock HttpMessageHandler to return a successful response for a PUT request
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Put && // Changed to PUT
                    req.RequestUri.ToString() == "http://localhost:5106/api/Products/1" && // Assuming you're updating product with ID 1
                    req.Content.Headers.ContentType.MediaType == "application/json"),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(expectedResponseMessage);
    
        // Act: Call the controller method to update the product
        var response = await _serviceProductController.PutProductAsync(1, product); // Assuming the method is named UpdateProductAsync
    
        // Assert: Verify the response is OK
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task UpdateProductAsync_ReturnsServiceUnavailable_WhenHttpRequestExceptionOccurs()
    {
        // Arrange
        var product = new Product
        {
            Id = 1, // Assuming product with ID 1 exists
            Title = "Updated Product",
            BodyHtml = "Updated Description"
        };

        // Setup the mock HttpMessageHandler to simulate an HttpRequestException during a PUT request
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new HttpRequestException("Network error"));

        // Act: Try updating the product
        var response = await _serviceProductController.PutProductAsync(1, product);

        // Assert: Verify the response status and message
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.ServiceUnavailable));
        Assert.That(await response.Content.ReadAsStringAsync(), Is.EqualTo("Error: Network error"));
    }
    
    [Test]
    public async Task UpdateProductAsync_ReturnsInternalServerError_WhenGeneralExceptionOccurs()
    {
        // Arrange
        var product = new Product
        {
            Id = 1, // Assuming product with ID 1 exists
            Title = "Updated Product",
            BodyHtml = "Updated Description"
        };

        // Setup the mock HttpMessageHandler to simulate a general exception during a PUT request
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act: Try updating the product
        var response = await _serviceProductController.PutProductAsync(1, product);

        // Assert: Verify the response status and message
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        Assert.That(await response.Content.ReadAsStringAsync(), Is.EqualTo("Error: Unexpected error"));
    }

    [Test]
    public async Task UpdateProductAsync_ReturnsBadRequest_WhenApiCallFails()
    {
        // Arrange: Create a product with updated details
        var product = new Product
        {
            Id = 1, // Assuming product with ID 1 exists
            Title = "Updated Product",
            BodyHtml = "Updated Description"
        };

        var expectedResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("Invalid product data")
        };

        // Setup the mock HttpMessageHandler to simulate a bad request response for a PUT request
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(expectedResponseMessage);

        // Act: Try updating the product
        var response = await _serviceProductController.PutProductAsync(1, product);

        // Assert: Verify the response status and message
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        Assert.That(await response.Content.ReadAsStringAsync(), Is.EqualTo("Invalid product data"));
    }

    // ------ GET VARIANT 


}