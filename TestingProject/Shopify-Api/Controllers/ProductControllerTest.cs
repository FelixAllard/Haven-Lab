using System.Collections;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using Shopify_Api;
using Shopify_Api.Controllers;
using ShopifySharp;
using ShopifySharp.Credentials;
using ShopifySharp.Factories;
using Newtonsoft.Json.Linq;
using Shopify_Api.Exceptions;
using Shopify_Api.Model;
using ShopifySharp.Lists;
using TestingProject.Utility;

namespace TestingProject.Shopify_Api.Controllers;

public class ProductControllerTest
{
    private Mock<IProductServiceFactory> _mockProductServiceFactory;
    private Mock<IProductService> _mockProductService;
    private Mock<IMetaFieldServiceFactory> _mockMetaFieldServiceFactory;
    private Mock<IMetaFieldService> _mockMetaFieldService;
    private ShopifyRestApiCredentials _falseCredentials;
    private ProductsController _controller;
    private ProductValidator _productValidator;

    [SetUp]
    public void Setup()
    {
        _mockProductServiceFactory = new Mock<IProductServiceFactory>();
        _mockProductService = new Mock<IProductService>();
        _mockMetaFieldServiceFactory = new Mock<IMetaFieldServiceFactory>();
        _mockMetaFieldService = new Mock<IMetaFieldService>();

        _falseCredentials = new ShopifyRestApiCredentials("NotARealURL", "NotARealToken");
        _productValidator = new ProductValidator();

        // Set up the mock to return the mock IProductService when Create is called.
        _mockProductServiceFactory
            .Setup(x => x.Create(It.IsAny<ShopifyApiCredentials>()))
            .Returns(_mockProductService.Object);

        // Set up the mock to return the mock IMetaFieldService when Create is called.
        _mockMetaFieldServiceFactory
            .Setup(x => x.Create(It.IsAny<ShopifyApiCredentials>()))
            .Returns(_mockMetaFieldService.Object);

        _controller = new ProductsController(
            _mockProductServiceFactory.Object,
            _falseCredentials,
            _productValidator,
            _mockMetaFieldServiceFactory.Object
        );
    }

    //================================  PRODUCT ENDPOINTS ==================================

    [Test]
    public async Task GetAllProducts_ReturnsOk_WhenProductsAreFetchedSuccessfully()
    {
        // Arrange
        var productList = new List<Product> 
        { 
            new Product { Id = 1, Title = "Product 1" },
            new Product { Id = 2, Title = "Product 2" }
        };

        // Create a ListResult<Product> containing the product list
        var listResult = new ShopifySharp.Lists.ListResult<Product>(productList, default);

        // Mock the ListAsync method to return the ListResult object
        _mockProductService.Setup(x => x.ListAsync(null, false, default)).ReturnsAsync(listResult);

        // Act
        var result = await _controller.GetAllProducts();

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));

        // Extract the ListResult<Product> from okResult.Value
        var listResultValue = okResult.Value as ShopifySharp.Lists.ListResult<Product>;
        Assert.IsNotNull(listResultValue);

        // Compare the Items list
        var returnedProducts = listResultValue.Items;
        Assert.IsNotNull(returnedProducts);
        Assert.That(returnedProducts, Is.EqualTo(productList).Using(new ProductComparer()));
    }




    [Test]
    public async Task GetAllProducts_ReturnsInternalServerError_WhenShopifyExceptionIsThrown()
    {
        // Arrange
        _mockProductService.Setup(x => x.ListAsync(null, false, default))
            .ThrowsAsync(new ShopifyException("Shopify error"));

        // Act
        var result = await _controller.GetAllProducts();

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.That(objectResult.StatusCode, Is.EqualTo(404));

        var value = JObject.FromObject(objectResult.Value);
        Assert.That(value["message"]?.ToString(), Is.EqualTo("Error fetching product"));
    }


    [Test]
    
    public async Task GetAllProducts_ReturnsInternalServerError_WhenUnexpectedExceptionOccurs()
    {
        
        // Arrange
        _mockProductService.Setup(x => x.ListAsync(null, false, default)).ThrowsAsync(new System.Exception("ExampleException"));

        // Act
        var result = await _controller.GetAllProducts();

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));

        // Deserialize the response body to a JObject
        var responseBody = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(objectResult.Value));

        // Assert the message property
        Assert.That(responseBody["message"]?.ToString(), Is.EqualTo($"Error fetching product"));
    }
    
    
    
    [TestCase("I")]
    [TestCase("In")]
    [TestCase("IN")]
    [TestCase("STOCK")]
    [TestCase("stock")]
    [TestCase("In Stock")]
    
    [Test]
    public async Task GetAllProducts_FiltersByName_ReturnsMatchingProducts(string searchTerm)
    {
        // Arrange
        // Arrange
        IEnumerable<Product> products = new List<Product>
        {
            new Product
            {
                Title = "In Stock", Variants = new List<ProductVariant>
                {
                    new ProductVariant
                    {
                        Price = 200,
                        Title = "Laptop",
                        InventoryQuantity = 100
                    }
                }
            },
            new Product
            {
                Title = "In Stock2",
                Variants = new List<ProductVariant>
                {
                    new ProductVariant
                    {
                        Price = 500,
                        Title = "Phone",
                        InventoryQuantity = 0

                    }
                }
            },
            new Product
            {
                Title = "What",
                Variants = new List<ProductVariant>
                {
                    new ProductVariant
                    {
                        Price = 0,
                        Title = "Phone",
                        InventoryQuantity = 0

                    }
                }
            }
        };
        var finalProduct = new ShopifySharp.Lists.ListResult<Product>(products, default);

        _mockProductService.Setup(s => s.ListAsync(null,false, default))
            .ReturnsAsync(finalProduct);


        var searchArguments = new SearchArguments { Name = searchTerm };

        // Act
        var result = await _controller.GetAllProducts(searchArguments) as OkObjectResult;
        var filteredProducts = (result.Value as ListResult<Product>).Items;

        // Assert
        Assert.That(filteredProducts.Count, Is.EqualTo(2));
        Assert.That(filteredProducts.First().Title, Is.EqualTo("In Stock"));
        Assert.That(filteredProducts.Last().Title, Is.EqualTo("In Stock2"));
    }

    [Test]
    public async Task GetAllProducts_FiltersByMinimumPrice_ReturnsMatchingProducts()
    {
        // Arrange
        IEnumerable<Product> products = new List<Product>
        {
            new Product
            {
                Title = "In Stock", Variants = new List<ProductVariant>
                {
                    new ProductVariant
                    {
                        Price = 200,
                        Title = "Laptop",
                        InventoryQuantity = 100
                    }
                }
            },
            new Product
            {
                Title = "In Stock2",
                Variants = new List<ProductVariant>
                {
                    new ProductVariant
                    {
                        Price = 500,
                        Title = "Phone",
                        InventoryQuantity = 0

                    }
                }
            },
            new Product
            {
                Title = "Not In Stock",
                Variants = new List<ProductVariant>
                {
                    new ProductVariant
                    {
                        Price = 0,
                        Title = "Phone",
                        InventoryQuantity = 0

                    }
                }
            }
        };
        var finalProduct = new ShopifySharp.Lists.ListResult<Product>(products, default);

        _mockProductService.Setup(s => s.ListAsync(null,false, default))
            .ReturnsAsync(finalProduct);

        var searchArguments = new SearchArguments { MinimumPrice = 100 };

        // Act
        var result = await _controller.GetAllProducts(searchArguments) as OkObjectResult;
        var filteredProducts = (result.Value as ListResult<Product>).Items;

        // Assert
        Assert.That(filteredProducts.Count, Is.EqualTo(2));
        Assert.That(filteredProducts.First().Title, Is.EqualTo("In Stock"));
        Assert.That(filteredProducts.Last().Title, Is.EqualTo("In Stock2"));
    }

    [Test]
    public async Task GetAllProducts_FiltersByMaximumPrice_ReturnsMatchingProducts()
    {
        IEnumerable<Product> products = new List<Product>
        {
            new Product
            {
                Title = "In Stock", Variants = new List<ProductVariant>
                {
                    new ProductVariant
                    {
                        Price = 200,
                        Title = "Laptop",
                        InventoryQuantity = 100
                    }
                }
            },
            new Product
            {
                Title = "In Stock2",
                Variants = new List<ProductVariant>
                {
                    new ProductVariant
                    {
                        Price = 500,
                        Title = "Phone",
                        InventoryQuantity = 0

                    }
                }
            },
            new Product
            {
                Title = "Not In Stock",
                Variants = new List<ProductVariant>
                {
                    new ProductVariant
                    {
                        Price = 700,
                        Title = "Phone",
                        InventoryQuantity = 0

                    }
                }
            }
        };
        var finalProduct = new ShopifySharp.Lists.ListResult<Product>(products, default);
        // Arrange
        _mockProductService.Setup(s => s.ListAsync(null, false, default))
            .ReturnsAsync(finalProduct);

        var searchArguments = new SearchArguments { MaximumPrice = 500 };

        // Act
        var result = await _controller.GetAllProducts(searchArguments) as OkObjectResult;
        
        var filteredProducts = (result.Value as ListResult<Product>).Items;

        // Assert
        Assert.That(filteredProducts.Count, Is.EqualTo(2));
        Assert.That(filteredProducts.First().Title, Is.EqualTo("In Stock"));
        Assert.That(filteredProducts.Last().Title, Is.EqualTo("In Stock2"));
    }

    [Test]
    public async Task GetAllProducts_FiltersByAvailability_ReturnsInStockProducts()
    {

        IEnumerable<Product> products = new List<Product>
        {
            new Product
            {
                Title = "In Stock", Variants = new List<ProductVariant>
                {
                    new ProductVariant
                    {
                        Price = 100023,
                        Title = "Laptop",
                        InventoryQuantity = 100
                    }
                }
            },
            new Product
            {
                Title = "Not In Stock",
                Variants = new List<ProductVariant>
                {
                    new ProductVariant
                    {
                        Price = 500,
                        Title = "Phone",
                        InventoryQuantity = 0

                    }
                }
            }
        };
        var finalProduct = new ShopifySharp.Lists.ListResult<Product>(products, default);


    // Arrange

        //ListResult<Product>

        _mockProductService.Setup(s => s.ListAsync(null,false,default))
            .ReturnsAsync(finalProduct);

        var searchArguments = new SearchArguments { Available = true };

        // Act
        var result = await _controller.GetAllProducts(searchArguments) as OkObjectResult;
        var filteredProducts = (result.Value as ListResult<Product>).Items;

        // Assert
        Assert.That(filteredProducts.Count, Is.EqualTo(1));
        Assert.That(filteredProducts.First().Title, Is.EqualTo("In Stock"));
    }
    //-------------------------------------Get BY Id Methods
    [Test]
    public async Task GetProductById_ReturnsOk_WhenProductIsFetchedSuccessfully()
    {
        // Arrange
        long productId = 1;
        var product = new Product
        {
            Id = productId,
            Title = "Product 1",
            BodyHtml = "<p>A good example product</p>",
            CreatedAt = DateTime.Parse("2024-12-08T23:40:19-05:00"),
            UpdatedAt = DateTime.Parse("2024-12-08T23:40:19-05:00"),
            PublishedAt = DateTime.Parse("2024-12-08T22:17:59-05:00"),
            Vendor = "New Vendor",
            ProductType = "",
            Handle = "haven-lab",
            PublishedScope = "global",
            Status = "active",
            Variants = new List<ProductVariant>
            {
                new ProductVariant
                {
                    ProductId = 8073575366701,
                    Title = "Default Title",
                    Price = 19.99M,
                    InventoryQuantity = 5,
                    Weight = 54,
                    WeightUnit = "kg"
                }
            }
        };

        // Mock the GetAsync method to return the product
        _mockProductService.Setup(x => x.GetAsync(productId, null, false,default)).ReturnsAsync(product);

        // Act
        var result = await _controller.GetProductById(productId);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));

        // Verify the returned product
        var returnedProduct = okResult.Value as Product;
        Assert.IsNotNull(returnedProduct);
        Assert.That(returnedProduct?.Title, Is.EqualTo(product.Title));
        Assert.That(returnedProduct?.Vendor, Is.EqualTo(product.Vendor));
        Assert.That(returnedProduct?.Variants.Count(), Is.EqualTo(product.Variants.Count()));
    }

    [Test]
    public async Task GetProductById_ReturnsInternalServerError_WhenShopifyExceptionIsThrown()
    {
        // Arrange
        long productId = 1;
        _mockProductService.Setup(x => x.GetAsync(productId, null, false,default))
            .ThrowsAsync(new ShopifyException("Shopify error"));

        // Act
        var result = await _controller.GetProductById(productId);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.That(objectResult.StatusCode, Is.EqualTo(404));

        var value = JObject.FromObject(objectResult.Value);
        Assert.That(value["message"]?.ToString(), Is.EqualTo("Error fetching products"));
    }

    [TestCase("Input1")]
    [TestCase("Input2")]
    [TestCase("Input3")]
    [Test]
    public async Task GetProductById_ReturnsInternalServerError_WhenUnexpectedExceptionOccurs(string testCase)
    {
        // Arrange
        long productId = 1;
        _mockProductService.Setup(x => x.GetAsync(productId, null, false,default))
            .ThrowsAsync(new System.Exception(testCase));

        // Act
        var result = await _controller.GetProductById(productId);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));

        var responseBody = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(objectResult.Value));
        Assert.That(responseBody["message"]?.ToString(), Is.EqualTo($"Error fetching products{testCase}"));
    }

        
        
        ///----------------------------------POST METHOD
        ///

    [Test]
    public async Task PostProduct_ReturnsOk_WhenProductIsCreatedSuccessfully()
    {
        // Arrange: Create a valid product
        var validProduct = new Product
        {
            Title = "Example Product",
            BodyHtml = "<p>A good example product</p>",
            CreatedAt = DateTime.Parse("2024-12-08T23:40:19-05:00"),
            UpdatedAt = DateTime.Parse("2024-12-08T23:40:19-05:00"),
            PublishedAt = DateTime.Parse("2024-12-08T22:17:59-05:00"),
            Vendor = "New Vendor",
            ProductType = "",
            Handle = "haven-lab",
            PublishedScope = "global",
            Status = "active",
            Variants = new List<ProductVariant>
            {
                new ProductVariant
                {
                    ProductId = 8073575366701,
                    Title = "Default Title",
                    SKU = null,
                    Position = 1,
                    Grams = 0,
                    InventoryPolicy = "deny",
                    FulfillmentService = "manual",
                    Taxable = true,
                    Weight = 0,
                    InventoryItemId = 45205286223917,
                    Price = 19.99M,
                    RequiresShipping = true,
                    InventoryQuantity = 5,
                    WeightUnit = "kg"
                }
            }
        };

        // Mock the service method CreateAsync to return the valid product
        _mockProductService.Setup(x => x.CreateAsync(It.IsAny<Product>(), default, default))
            .ReturnsAsync(validProduct);

        // Act: Call the controller method to create a product
        var result = await _controller.PostProduct(validProduct);

        // Assert: Verify the result
        Assert.IsNotNull(result);
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult, "Expected OkObjectResult but got null.");
        Assert.That(okResult.StatusCode, Is.EqualTo(200), "Status code should be 200 OK.");

        // Verify that the returned product matches the created product
        var createdProduct = okResult.Value as Product;
        Assert.IsNotNull(createdProduct, "Created product should not be null.");
        Assert.That(createdProduct?.Title, Is.EqualTo(validProduct.Title), "Product titles should match.");
        Assert.That(createdProduct?.Vendor, Is.EqualTo(validProduct.Vendor), "Product vendors should match.");
        Assert.That(createdProduct?.Variants.Count(), Is.EqualTo(validProduct.Variants.Count()), "Product variants count should match.");

        // Verify that CreateAsync was called exactly once
        _mockProductService.Verify(x => x.CreateAsync(It.IsAny<Product>(), default, default), Times.Once, "CreateAsync was not called exactly once.");
    }




        


    [Test]
    public async Task PostProduct_ReturnsBadRequest_WhenInputExceptionIsThrown()
    {
        // Arrange
        var product = new Product
        {
            Title = "Example Product",
            BodyHtml = "<p>A good example product</p>",
            CreatedAt = DateTime.Parse("2024-12-08T23:40:19-05:00"),
            UpdatedAt = DateTime.Parse("2024-12-08T23:40:19-05:00"),
            PublishedAt = DateTime.Parse("2024-12-08T22:17:59-05:00"),
            Vendor = "New Vendor",
            ProductType = "",
            Handle = "haven-lab",
            PublishedScope = "global",
            Status = "active",
            Variants = new List<ProductVariant>
            {
                new ProductVariant
                {
                    ProductId = 8073575366701,
                    Title = "Default Title",
                    SKU = null,
                    Position = 1,
                    Grams = 0,
                    InventoryPolicy = "deny",
                    FulfillmentService = "manual",
                    InventoryItemId = 45205286223917,
                    Price = 19.99M,
                    RequiresShipping = true,
                    Taxable = true,
                    InventoryQuantity = 5,
                    Weight = 54,
                    WeightUnit = "kg"
                }
            }
        };

        _mockProductService.Setup(x => x.CreateAsync(It.IsAny<Product>(), default,default))
            .ThrowsAsync(new InputException("Invalid input"));
        

        // Act
        var result = await _controller.PostProduct(product);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.That(objectResult.StatusCode, Is.EqualTo(400));

        var value = JObject.FromObject(objectResult.Value);
        Assert.That(value["message"]?.ToString(), Is.EqualTo("Invalid input"));
    }

    [Test]
    public async Task PostProduct_ReturnsInternalServerError_WhenShopifyExceptionIsThrown()
    {
        // Arrange
        var product = new Product
        {
            Title = "Example Product",
            BodyHtml = "<p>A good example product</p>",
            CreatedAt = DateTime.Parse("2024-12-08T23:40:19-05:00"),
            UpdatedAt = DateTime.Parse("2024-12-08T23:40:19-05:00"),
            PublishedAt = DateTime.Parse("2024-12-08T22:17:59-05:00"),
            Vendor = "New Vendor",
            ProductType = "",
            Handle = "haven-lab",
            PublishedScope = "global",
            Status = "active",
            Variants = new List<ProductVariant>
            {
                new ProductVariant
                {
                    ProductId = 8073575366701,
                    Title = "Default Title",
                    SKU = null,
                    Position = 1,
                    Grams = 0,
                    InventoryPolicy = "deny",
                    FulfillmentService = "manual",
                    InventoryItemId = 45205286223917,
                    Price = 19.99M,
                    RequiresShipping = true,
                    Taxable = true,
                    InventoryQuantity = 5,
                    Weight = 54,
                    WeightUnit = "kg"
                }
            }
        };

        _mockProductService.Setup(x => x.CreateAsync(It.IsAny<Product>(), default, default))
            .ThrowsAsync(new ShopifyException("Shopify error"));

        // Act
        var result = await _controller.PostProduct(product);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));

        var value = JObject.FromObject(objectResult.Value);
        Assert.That(value["message"]?.ToString(), Is.EqualTo("Error fetching products"));
    }

    [TestCase("Unexpected error 1")]
    [TestCase("Unexpected error 2")]
    [TestCase("Unexpected error 3")]
    [Test]
    public async Task PostProduct_ReturnsInternalServerError_WhenUnexpectedExceptionOccurs(string testCase)
    {
        // Arrange
        var product = new Product
        {
            Title = "Example Product",
            BodyHtml = "<p>A good example product</p>",
            CreatedAt = DateTime.Parse("2024-12-08T23:40:19-05:00"),
            UpdatedAt = DateTime.Parse("2024-12-08T23:40:19-05:00"),
            PublishedAt = DateTime.Parse("2024-12-08T22:17:59-05:00"),
            Vendor = "New Vendor",
            ProductType = "",
            Handle = "haven-lab",
            PublishedScope = "global",
            Status = "active",
            Variants = new List<ProductVariant>
            {
                new ProductVariant
                {
                    ProductId = 8073575366701,
                    Title = "Default Title",
                    SKU = null,
                    Position = 1,
                    Grams = 0,
                    InventoryPolicy = "deny",
                    FulfillmentService = "manual",
                    InventoryItemId = 45205286223917,
                    Price = 19.99M,
                    RequiresShipping = true,
                    Taxable = true,
                    InventoryQuantity = 5,
                    Weight = 54,
                    WeightUnit = "kg"
                }
            }
        };

        _mockProductService.Setup(
                x => x.CreateAsync(
                    It.IsAny<Product>(), 
                    default,
                    default
                    )
                )
            .ThrowsAsync(new System.Exception(testCase));

        // Act
        var result = await _controller.PostProduct(product);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));

        var responseBody = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(objectResult.Value));
        Assert.That(responseBody["message"]?.ToString(), Is.EqualTo($"Error creating product {testCase}"));
    }
    
    ///----------------------------------PUT METHOD
    ///
    
    [Test]
public async Task PutProduct_ReturnsOk_WhenProductIsUpdatedSuccessfully()
{
    // Arrange: Create a valid product
    var validProduct = new Product
    {
        Id = 1,
        Title = "Updated Product",
        BodyHtml = "<p>Updated product details</p>",
        CreatedAt = DateTime.Parse("2024-12-08T23:40:19-05:00"),
        UpdatedAt = DateTime.Parse("2024-12-08T23:40:19-05:00"),
        PublishedAt = DateTime.Parse("2024-12-08T22:17:59-05:00"),
        Vendor = "Updated Vendor",
        ProductType = "",
        Handle = "updated-handle",
        PublishedScope = "global",
        Status = "active",
        Variants = new List<ProductVariant>
        {
            new ProductVariant
            {
                ProductId = 8073575366701,
                Title = "Updated Title",
                SKU = null,
                Position = 1,
                Grams = 0,
                InventoryPolicy = "deny",
                FulfillmentService = "manual",
                Taxable = true,
                Weight = 0,
                InventoryQuantity = 5,
                WeightUnit = "kg"
            }
        }
    };

    // Mock the service method UpdateAsync to return the updated product
    _mockProductService.Setup(x => x.UpdateAsync(It.IsAny<long>(), It.IsAny<Product>(), default))
        .ReturnsAsync(validProduct);
    
    // Act: Call the controller method to update the product
    var result = await _controller.PutProduct(1, validProduct);

    // Assert: Verify the result
    Assert.IsNotNull(result);
    var okResult = result as OkObjectResult;
    Assert.IsNotNull(okResult, "Expected OkObjectResult but got null.");
    Assert.That(okResult.StatusCode, Is.EqualTo(200), "Status code should be 200 OK.");

    // Verify that the returned product matches the updated product
    var updatedProduct = okResult.Value as Product;
    Assert.IsNotNull(updatedProduct, "Updated product should not be null.");
    Assert.That(updatedProduct?.Title, Is.EqualTo(validProduct.Title), "Product titles should match.");
    Assert.That(updatedProduct?.Vendor, Is.EqualTo(validProduct.Vendor), "Product vendors should match.");
    Assert.That(updatedProduct?.Variants.Count(), Is.EqualTo(validProduct.Variants.Count()), "Product variants count should match.");

    // Verify that UpdateAsync was called exactly once
    _mockProductService.Verify(x => x.UpdateAsync(It.IsAny<long>(), It.IsAny<Product>(), default), Times.Once, "UpdateAsync was not called exactly once.");
}

    [Test]
    public async Task PutProduct_ReturnsBadRequest_WhenInputExceptionIsThrown()
    {
        // Arrange
        var product = new Product
        {
            Id = 1,
            Title = "Updated Product",
            BodyHtml = "<p>Updated product details</p>",
            CreatedAt = DateTime.Parse("2024-12-08T23:40:19-05:00"),
            UpdatedAt = DateTime.Parse("2024-12-08T23:40:19-05:00"),
            PublishedAt = DateTime.Parse("2024-12-08T22:17:59-05:00"),
            Vendor = "Updated Vendor",
            ProductType = "",
            Handle = "updated-handle",
            PublishedScope = "global",
            Status = "active",
            Variants = new List<ProductVariant>
            {
                new ProductVariant
                {
                    ProductId = 8073575366701,
                    Title = "Updated Title",
                    SKU = null,
                    Position = 1,
                    Grams = 0,
                    InventoryPolicy = "deny",
                    FulfillmentService = "manual",
                    Taxable = true,
                    Weight = 0,
                    InventoryQuantity = 5,
                    WeightUnit = "kg"
                }
            }
        };

        _mockProductService.Setup(x => x.UpdateAsync(It.IsAny<long>(), It.IsAny<Product>(), default))
            .ThrowsAsync(new InputException("Invalid input"));

        // Act
        var result = await _controller.PutProduct(1, product);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.That(objectResult.StatusCode, Is.EqualTo(400));

        var value = JObject.FromObject(objectResult.Value);
        Assert.That(value["message"]?.ToString(), Is.EqualTo("Invalid input"));
    }

    [Test]
    public async Task PutProduct_ReturnsInternalServerError_WhenShopifyExceptionIsThrown()
    {
        // Arrange
        var product = new Product
        {
            Id = 1,
            Title = "Updated Product",
            BodyHtml = "<p>Updated product details</p>",
            CreatedAt = DateTime.Parse("2024-12-08T23:40:19-05:00"),
            UpdatedAt = DateTime.Parse("2024-12-08T23:40:19-05:00"),
            PublishedAt = DateTime.Parse("2024-12-08T22:17:59-05:00"),
            Vendor = "Updated Vendor",
            ProductType = "",
            Handle = "updated-handle",
            PublishedScope = "global",
            Status = "active",
            Variants = new List<ProductVariant>
            {
                new ProductVariant
                {
                    ProductId = 8073575366701,
                    Title = "Updated Title",
                    SKU = null,
                    Position = 1,
                    Grams = 0,
                    InventoryPolicy = "deny",
                    FulfillmentService = "manual",
                    Taxable = true,
                    Weight = 0,
                    InventoryQuantity = 5,
                    WeightUnit = "kg"
                }
            }
        };

        _mockProductService.Setup(x => x.UpdateAsync(It.IsAny<long>(), It.IsAny<Product>(), default))
            .ThrowsAsync(new ShopifyException("Shopify error"));

        // Act
        var result = await _controller.PutProduct(1, product);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));

        var value = JObject.FromObject(objectResult.Value);
        Assert.That(value["message"]?.ToString(), Is.EqualTo("Error fetching products"));
    }

    [TestCase("Unexpected error 1")]
    [TestCase("Unexpected error 2")]
    [TestCase("Unexpected error 3")]
    [Test]
    public async Task PutProduct_ReturnsInternalServerError_WhenUnexpectedExceptionOccurs(string testCase)
    {
        // Arrange
        var product = new Product
        {
            Id = 1,
            Title = "Updated Product",
            BodyHtml = "<p>Updated product details</p>",
            CreatedAt = DateTime.Parse("2024-12-08T23:40:19-05:00"),
            UpdatedAt = DateTime.Parse("2024-12-08T23:40:19-05:00"),
            PublishedAt = DateTime.Parse("2024-12-08T22:17:59-05:00"),
            Vendor = "Updated Vendor",
            ProductType = "",
            Handle = "updated-handle",
            PublishedScope = "global",
            Status = "active",
            Variants = new List<ProductVariant>
            {
                new ProductVariant
                {
                    ProductId = 8073575366701,
                    Title = "Updated Title",
                    SKU = null,
                    Position = 1,
                    Grams = 0,
                    InventoryPolicy = "deny",
                    FulfillmentService = "manual",
                    Taxable = true,
                    Weight = 0,
                    InventoryQuantity = 5,
                    WeightUnit = "kg"
                }
            }
        };

        _mockProductService.Setup(
                x => x.UpdateAsync(It.IsAny<long>(), It.IsAny<Product>(), default)
            )
            .ThrowsAsync(new Exception(testCase));

        // Act
        var result = await _controller.PutProduct(1, product);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));

        var responseBody = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(objectResult.Value));
        Assert.That(responseBody["message"]?.ToString(), Is.EqualTo($"Error updating product {testCase}"));
    }

    
    
    //---------------------------------DELETE PRODUCT-----------------------------------
    
    [Test]
    public async Task DeleteProductById_ReturnsOk_WhenProductIsDeleteSuccessfully()
    {
        // Arrange
        long productId = 1;
        var product = new Product
        {
            Id = productId,
            Title = "Product 1",
            BodyHtml = "<p>A good example product</p>",
            CreatedAt = DateTime.Parse("2024-12-08T23:40:19-05:00"),
            UpdatedAt = DateTime.Parse("2024-12-08T23:40:19-05:00"),
            PublishedAt = DateTime.Parse("2024-12-08T22:17:59-05:00"),
            Vendor = "New Vendor",
            ProductType = "",
            Handle = "haven-lab",
            PublishedScope = "global",
            Status = "active",
            Variants = new List<ProductVariant>
            {
                new ProductVariant
                {
                    ProductId = 8073575366701,
                    Title = "Default Title",
                    Price = 19.99M,
                    InventoryQuantity = 5,
                    Weight = 54,
                    WeightUnit = "kg"
                }
            }
        };

        // Mock the DeleteAsync method to return the product
        _mockProductService.Setup(x => x.DeleteAsync(productId, default)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteProduct(productId);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));

        // Verify the returned product
        var returnedProduct = okResult.Value as Product;
        Assert.IsNull(returnedProduct);
    }

    [Test]
    public async Task DeleteProductById_ReturnsNotFound_WhenShopifyExceptionIsThrown()
    {
        // Arrange
        long productId = 1;
        _mockProductService.Setup(x => x.DeleteAsync(productId, default))
            .ThrowsAsync(new ShopifyException("Shopify error"));

        // Act
        var result = await _controller.DeleteProduct(productId);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult, "Expected ObjectResult but got null.");
        Assert.That(objectResult.StatusCode, Is.EqualTo(404), "Expected status code 404 but got a different code.");

        // Assert message
        var value = JObject.FromObject(objectResult.Value);
        Assert.That(value["message"]?.ToString(), Is.EqualTo("No product found"), "Expected 'No product found' but got a different message.");
    }


    [TestCase("Input1")]
    [TestCase("Input2")]
    [TestCase("Input3")]
    [Test]
    public async Task DeleteProductById_ReturnsInternalServerError_WhenUnexpectedExceptionOccurs(string testCase)
    {
        // Arrange
        long productId = 1;
        _mockProductService.Setup(x => x.DeleteAsync(productId, default))
            .ThrowsAsync(new System.Exception(testCase));

        // Act
        var result = await _controller.DeleteProduct(productId);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult, "Expected ObjectResult but got null.");
        Assert.That(objectResult.StatusCode, Is.EqualTo(500), "Expected status code 500 but got a different code.");

        var responseBody = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(objectResult.Value));
        Assert.That(responseBody["message"]?.ToString(), Is.EqualTo($"Error deleting products{testCase}"), "Error message mismatch.");
    }
    
    [Test]
    public async Task GetFirstVariantByProductId_ReturnsOk_WhenProductWithVariantsIsFetchedSuccessfully()
    {
        // Arrange
        long productId = 1;
        long variantId = 45205286223917;
        var product = new Product
        {
            Id = productId,
            Title = "Product 1",
            BodyHtml = "<p>A good example product</p>",
            CreatedAt = DateTime.Parse("2024-12-08T23:40:19-05:00"),
            UpdatedAt = DateTime.Parse("2024-12-08T23:40:19-05:00"),
            PublishedAt = DateTime.Parse("2024-12-08T22:17:59-05:00"),
            Vendor = "New Vendor",
            ProductType = "",
            Handle = "haven-lab",
            PublishedScope = "global",
            Status = "active",
            Variants = new List<ProductVariant>
            {
                new ProductVariant
                {
                    ProductId = productId,
                    Title = "Default Title",
                    Price = 19.99M,
                    InventoryQuantity = 5,
                    Weight = 54,
                    WeightUnit = "kg",
                    Id = variantId
                }
            }
        };

        _mockProductService.Setup(x => x.GetAsync(productId, null, false, default))
            .ReturnsAsync(product);

        // Act
        var result = await _controller.GetFirstVariantByProductId(productId);
        // Print result to console
        Console.WriteLine($"Result: {result}");

        // Assert
        Assert.IsNotNull(result, "Expected result to be not null.");

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult, "Expected OkObjectResult.");
        Assert.AreEqual(200, okResult.StatusCode);

        var jsonResponse = JsonConvert.SerializeObject(okResult.Value);
        Console.WriteLine($"Json: {jsonResponse}");
        Assert.IsNotNull(jsonResponse, "Expected variant data to be not null.");

        
    }
    
    [Test]
    public async Task GetFirstVariantByProductId_ReturnsNotFound_WhenProductIsNotFound()
    {
        // Arrange
        long productId = 9999; // Non-existing product ID
        _mockProductService.Setup(x => x.GetAsync(productId, null, false,default))
            .ReturnsAsync((Product)null);

        // Act
        var result = await _controller.GetFirstVariantByProductId(productId);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        Assert.IsNotNull(notFoundResult);
        Assert.AreEqual(404, notFoundResult.StatusCode);

        var jsonResponse = JsonConvert.SerializeObject(notFoundResult.Value);
        Assert.IsNotNull(jsonResponse);
        Assert.That(jsonResponse, Is.EqualTo("{\"message\":\"Product not found or no variants available.\"}"));

    }

    [Test]
    public async Task GetFirstVariantByProductId_ReturnsNotFound_WhenNoVariantsAvailable()
    {
        // Arrange
        long productId = 2;
        var product = new Product
        {
            Id = productId,
            Title = "Product 2",
            Variants = new List<ProductVariant>()
        };

        _mockProductService.Setup(x => x.GetAsync(productId, null, false,default))
            .ReturnsAsync(product);

        // Act
        var result = await _controller.GetFirstVariantByProductId(productId);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        Assert.IsNotNull(notFoundResult);
        Assert.AreEqual(404, notFoundResult.StatusCode);

        var jsonResponse = JsonConvert.SerializeObject(notFoundResult.Value);
        Assert.IsNotNull(jsonResponse);
        Assert.That(jsonResponse, Is.EqualTo("{\"message\":\"Product not found or no variants available.\"}"));
    }

    [Test]
    public async Task GetFirstVariantByProductId_ReturnsInternalServerError_WhenUnexpectedExceptionOccurs()
    {
        // Arrange
        long productId = 1;
        _mockProductService.Setup(x => x.GetAsync(productId, null, false,default))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await _controller.GetFirstVariantByProductId(productId);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.AreEqual(500, objectResult.StatusCode);

        var response = JObject.FromObject(objectResult.Value);
        Assert.AreEqual("Error fetching product variants", response["message"]?.ToString());
    }
    
    [Test]
    public async Task GetFirstVariantByProductId_ReturnsNotFound_WhenFirstVariantIdIsNull()
    {
        // Arrange
        long productId = 1;
        var product = new Product
        {
            Id = productId,
            Title = "Product 1",
            BodyHtml = "<p>A good example product</p>",
            CreatedAt = DateTime.Parse("2024-12-08T23:40:19-05:00"),
            UpdatedAt = DateTime.Parse("2024-12-08T23:40:19-05:00"),
            PublishedAt = DateTime.Parse("2024-12-08T22:17:59-05:00"),
            Vendor = "New Vendor",
            ProductType = "",
            Handle = "haven-lab",
            PublishedScope = "global",
            Status = "active",
            Variants = new List<ProductVariant>{} // No variants added here to simulate the null condition.
        };

        _mockProductService.Setup(x => x.GetAsync(productId, null, false, default))
            .ReturnsAsync(product);

        // Act
        var result = await _controller.GetFirstVariantByProductId(productId);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        Assert.IsNotNull(notFoundResult, "Expected NotFoundObjectResult.");
        Assert.AreEqual(404, notFoundResult.StatusCode);

        var response = JObject.FromObject(notFoundResult.Value);
        Assert.IsNotNull(response, "Expected message object.");
        Assert.AreEqual("Product not found or no variants available.", (string)response["message"]);
    }

    
    //================================ TRANSLATED METAFIELD ENDPOINTS ==================================
    
    [Test]
    public async Task GetTranslatedProduct_ReturnsOk_WhenTranslationExists()
    {
        // Arrange
        long productId = 1;
        string lang = "fr";

        var metafields = new ShopifySharp.Lists.ListResult<MetaField>(new List<MetaField>
        {
            new MetaField { Namespace = "translations", Key = "title_fr", Value = "Produit Exemple" },
            new MetaField { Namespace = "translations", Key = "description_fr", Value = "Ceci est un produit d'exemple" }
        }, default);

        _mockMetaFieldService
            .Setup(x => x.ListAsync(productId, "products", null, default))
            .ReturnsAsync(metafields);

        // Act
        var result = await _controller.GetTranslatedProduct(productId, lang);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));

        var response = JObject.FromObject(okResult.Value);
        Assert.That(response["ProductId"]?.ToString(), Is.EqualTo(productId.ToString()));
        Assert.That(response["Title"]?.ToString(), Is.EqualTo("Produit Exemple"));
        Assert.That(response["Description"]?.ToString(), Is.EqualTo("Ceci est un produit d'exemple"));
    }

    [Test]
    public async Task GetTranslatedProduct_ReturnsOk_WithDefaultValues_WhenNoTranslationExists()
    {
        // Arrange
        long productId = 1;
        string lang = "fr";

        var metafields = new ShopifySharp.Lists.ListResult<MetaField>(new List<MetaField>(), default);
        
        _mockMetaFieldService
            .Setup(x => x.ListAsync(productId, "products", null, default))
            .ReturnsAsync(metafields);

        // Act
        var result = await _controller.GetTranslatedProduct(productId, lang);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));

        var response = JObject.FromObject(okResult.Value);
        Assert.That(response["ProductId"]?.ToString(), Is.EqualTo(productId.ToString()));
        Assert.That(response["Title"]?.ToString(), Is.EqualTo("N/A"));
        Assert.That(response["Description"]?.ToString(), Is.EqualTo("N/A"));
    }
    
    [TestCase("UnexpectedError1")]
    [TestCase("UnexpectedError2")]
    [TestCase("UnexpectedError3")]
    [Test]
    public async Task GetTranslatedProduct_ReturnsInternalServerError_WhenExceptionOccurs(string testCase)
    {
        // Arrange
        long productId = 1;
        string lang = "fr";
        
        _mockMetaFieldService
            .Setup(x => x.ListAsync(productId, "products", null, default))
            .ThrowsAsync(new System.Exception(testCase));

        // Act
        var result = await _controller.GetTranslatedProduct(productId, lang);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));

        var response = JObject.FromObject(objectResult.Value);
        Assert.That(response["message"]?.ToString(), Is.EqualTo("Error fetching translations"));
        Assert.That(response["details"]?.ToString(), Is.EqualTo(testCase));
    }
    
    //----------------------------------POST METHOD-------------------------------------------
    
     [Test]
    public async Task AddProductTranslation_ReturnsOk_WhenTranslationIsCreatedSuccessfully()
    {
        // Arrange
        long productId = 1;
        var translationRequest = new TranslationRequest
        {
            Locale = "fr",
            Title = "Produit Exemple",
            Description = "Ceci est un produit d'exemple"
        };

        _mockMetaFieldService
            .Setup(x => x.CreateAsync(It.IsAny<MetaField>(), It.IsAny<long>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MetaField { Id = 123456, Namespace = "translations", Key = "title_fr", Value = "Produit Exemple" });

        // Act
        var result = await _controller.AddProductTranslation(productId, translationRequest);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));

        var response = JObject.FromObject(okResult.Value);
        Assert.That(response["message"]?.ToString(), Is.EqualTo("Translation added!"));
        Assert.That(response["productId"]?.ToString(), Is.EqualTo(productId.ToString()));

        // Verify that CreateAsync was called exactly twice (title & description)
        _mockMetaFieldService.Verify(
            x => x.CreateAsync(It.IsAny<MetaField>(), It.IsAny<long>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Exactly(2));
    }

    // 🔹 Test: Returns Internal Server Error When Exception Is Thrown
    [TestCase("UnexpectedError1")]
    [TestCase("UnexpectedError2")]
    [TestCase("UnexpectedError3")]
    [Test]
    public async Task AddProductTranslation_ReturnsInternalServerError_WhenExceptionOccurs(string testCase)
    {
        // Arrange
        long productId = 1;
        var translationRequest = new TranslationRequest
        {
            Locale = "fr",
            Title = "Produit Exemple",
            Description = "Ceci est un produit d'exemple"
        };

        _mockMetaFieldService
            .Setup(x => x.CreateAsync(It.IsAny<MetaField>(), It.IsAny<long>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new System.Exception(testCase));

        // Act
        var result = await _controller.AddProductTranslation(productId, translationRequest);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));

        var response = JObject.FromObject(objectResult.Value);
        Assert.That(response["message"]?.ToString(), Is.EqualTo("Error saving translation"));
        Assert.That(response["details"]?.ToString(), Is.EqualTo(testCase));

        // Verify that CreateAsync was attempted
        _mockMetaFieldService.Verify(
            x => x.CreateAsync(It.IsAny<MetaField>(), It.IsAny<long>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Exactly(1));
    }
    
}
    