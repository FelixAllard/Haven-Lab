using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shopify_Api;
using Shopify_Api.Controllers;
using Shopify_Api.Exceptions;
using Shopify_Api.Model;
using ShopifySharp;
using ShopifySharp.Credentials;
using ShopifySharp.Factories;
using ShopifySharp.Lists;

namespace TestingProject.Shopify_Api.Controllers;

[TestFixture]
public class OrderControllerTest
{
        private Mock<IOrderServiceFactory> _mockOrderServiceFactory;
        private Mock<IOrderService> _mockOrderService;
        private ShopifyRestApiCredentials _falseCredentials;
        private OrderController _controller;

        [SetUp]
        public void Setup()
        {
            _mockOrderServiceFactory = new Mock<IOrderServiceFactory>();
            _mockOrderService = new Mock<IOrderService>();
            _falseCredentials = new ShopifyRestApiCredentials("NotARealURL","NotARealToken");

            // Set up the mock to return the mock IProductService when Create is called.
            _mockOrderServiceFactory
                .Setup(x => x.Create(It.IsAny<ShopifyApiCredentials>()))
                .Returns(_mockOrderService.Object);

            _controller = new OrderController(_mockOrderServiceFactory.Object, _falseCredentials);
        }

        [Test]
        public async Task GetAllOrders_ReturnsOk_WhenOrdersAreFetchedSuccessfully()
        {
            // Arrange
            var orderList = new List<Order> 
            { 
                new Order { Id = 1, AppId = 1234567 },
                new Order { Id = 2, AppId = 1234560 } 
            };

            // Create a ListResult<Product> containing the product list
            var listResult = new ShopifySharp.Lists.ListResult<Order>(orderList, default);

            // Mock the ListAsync method to return the ListResult object
            _mockOrderService.Setup(x => x.ListAsync(null, default)).ReturnsAsync(listResult);

            // Act
            var result = await _controller.GetAllOrders();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));

            // Extract the ListResult<Order> from okResult.Value
            var listResultValue = okResult.Value as ShopifySharp.Lists.ListResult<Order>;
            Assert.IsNotNull(listResultValue);

            // Compare the Items list
            var returnedOrders = listResultValue.Items;
            Assert.IsNotNull(returnedOrders);
            Assert.That(returnedOrders, Is.EqualTo(orderList));
        }

        [TestCase("Customer1")]
        [TestCase("Customer2")]
        [TestCase("Customer3")]

        [Test]
        public async Task GetAllOrders_FiltersByCustomerName_ReturnsMatchingOrders(string searchTerm)
        {
            // Arrange: Create a list of orders with different customer names
            IEnumerable<Order> orders = new List<Order>
            {
                new Order
                {
                    Id = 1,
                    ShippingAddress = new Address { Name = "Customer1" },
                    FinancialStatus = "paid",
                    CreatedAt = DateTime.Now
                },
                new Order
                {
                    Id = 2,
                    ShippingAddress = new Address { Name = "Customer2" },
                    FinancialStatus = "voided",
                    CreatedAt = DateTime.Now
                },
                new Order
                {
                    Id = 3,
                    ShippingAddress = new Address { Name = "Customer3" },
                    FinancialStatus = "paid",
                    CreatedAt = DateTime.Now
                }
            };

            var finalOrders = new ShopifySharp.Lists.ListResult<Order>(orders, default);

            // Mock the service to return a list of orders
            _mockOrderService.Setup(s => s.ListAsync(null, default)).ReturnsAsync(finalOrders);

            var searchArguments = new OrderSearchArgument { CustomerName = searchTerm };

            // Act: Call the controller method
            var result = await _controller.GetAllOrders(searchArguments) as OkObjectResult;
            var filteredOrders = (result.Value as ListResult<Order>).Items;

            // Assert: Verify the filtered results
            Assert.That(filteredOrders.Count, Is.EqualTo(1));  // Only one order should match the search term
            Assert.That(filteredOrders.First().ShippingAddress.Name, Is.EqualTo(searchTerm));
        }
        
        [TestCase("paid")]
 
        [Test]
        public async Task GetAllOrders_FiltersByStatus_ReturnsMatchingOrders(string status)
        {
            // Arrange: Create a list of orders with different statuses
            IEnumerable<Order> orders = new List<Order>
            {
                new Order
                {
                    Id = 1,
                    ShippingAddress = new Address { Name = "Customer1" },
                    FinancialStatus = "paid",
                    CreatedAt = DateTime.Now
                },
                new Order
                {
                    Id = 2,
                    ShippingAddress = new Address { Name = "Customer2" },
                    FinancialStatus = "voided",
                    CreatedAt = DateTime.Now
                },
                new Order
                {
                    Id = 3,
                    ShippingAddress = new Address { Name = "Customer3" },
                    FinancialStatus = "paid",
                    CreatedAt = DateTime.Now
                }
            };

            var finalOrders = new ShopifySharp.Lists.ListResult<Order>(orders, default);

            // Mock the service to return the list of orders
            _mockOrderService.Setup(s => s.ListAsync(null, default)).ReturnsAsync(finalOrders);

            var searchArguments = new OrderSearchArgument { Status = status };

            // Act: Call the controller method
            var result = await _controller.GetAllOrders(searchArguments) as OkObjectResult;
            var filteredOrders = (result.Value as ListResult<Order>).Items;

            // Assert: Verify the filtered results
            Assert.That(filteredOrders.Count, Is.EqualTo(2));  // Two orders should match the status
            Assert.That(filteredOrders.All(o => o.FinancialStatus == status), Is.True);
        }
        
        [TestCase("2024-01-02")]
        public async Task GetAllOrders_FiltersByDateBefore_ReturnsMatchingOrders(DateTime dateBefore)
        {
            // Arrange: Create a list of orders with different dates
            IEnumerable<Order> orders = new List<Order>
            {
                new Order
                {
                    Id = 1,
                    ShippingAddress = new Address { Name = "Customer1" },
                    FinancialStatus = "paid",
                    CreatedAt = new DateTime(2024, 01, 01)
                },
                new Order
                {
                    Id = 2,
                    ShippingAddress = new Address { Name = "Customer2" },
                    FinancialStatus = "voided",
                    CreatedAt = new DateTime(2024, 01, 02)
                },
                new Order
                {
                    Id = 3,
                    ShippingAddress = new Address { Name = "Customer3" },
                    FinancialStatus = "paid",
                    CreatedAt = new DateTime(2024, 01, 03)
                }
            };

            var finalOrders = new ShopifySharp.Lists.ListResult<Order>(orders, default);

            // Mock the service to return a list of orders
            _mockOrderService.Setup(s => s.ListAsync(null, default)).ReturnsAsync(finalOrders);

            var searchArguments = new OrderSearchArgument { DateBefore = dateBefore };

            // Act: Call the controller method
            var result = await _controller.GetAllOrders(searchArguments) as OkObjectResult;
            var filteredOrders = (result.Value as ListResult<Order>).Items;

            // Assert: Verify the filtered results
            Assert.That(filteredOrders.Count, Is.EqualTo(2));  // Two orders should be before the given date
            Assert.That(filteredOrders.All(o => o.CreatedAt <= dateBefore), Is.True);
        }
        
        [TestCase("2024-01-02")]
        public async Task GetAllOrders_FiltersByDateAfter_ReturnsMatchingOrders(DateTime dateAfter)
        {
            // Arrange: Create a list of orders with different dates
            IEnumerable<Order> orders = new List<Order>
            {
                new Order
                {
                    Id = 1,
                    ShippingAddress = new Address { Name = "Customer1" },
                    FinancialStatus = "paid",
                    CreatedAt = new DateTime(2024, 01, 01)
                },
                new Order
                {
                    Id = 2,
                    ShippingAddress = new Address { Name = "Customer2" },
                    FinancialStatus = "unpaid",
                    CreatedAt = new DateTime(2024, 01, 02)
                },
                new Order
                {
                    Id = 3,
                    ShippingAddress = new Address { Name = "Customer3" },
                    FinancialStatus = "paid",
                    CreatedAt = new DateTime(2024, 01, 03)
                }
            };

            var finalOrders = new ShopifySharp.Lists.ListResult<Order>(orders, default);

            // Mock the service to return a list of orders
            _mockOrderService.Setup(s => s.ListAsync(null, default)).ReturnsAsync(finalOrders);
            
            var searchArguments = new OrderSearchArgument { DateAfter = dateAfter };

            // Act: Call the controller method
            var result = await _controller.GetAllOrders(searchArguments) as OkObjectResult;
            var filteredOrders = (result.Value as ListResult<Order>).Items;

            // Assert: Verify the filtered results
            Assert.That(filteredOrders.Count, Is.EqualTo(2));  // Two orders should be after the given date
            Assert.That(filteredOrders.All(o => o.CreatedAt >= dateAfter), Is.True);
        }

        [Test]
        public async Task GetAllOrders_ReturnsInternalServerError_WhenShopifyExceptionIsThrown()
        {
            // Arrange
            _mockOrderService.Setup(x => x.ListAsync(null, default))
                .ThrowsAsync(new ShopifyException("Shopify error"));

            // Act
            var result = await _controller.GetAllOrders();

            // Assert
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));

            var value = JObject.FromObject(objectResult.Value);
            Assert.AreEqual("Error fetching orders", value["message"]?.ToString());
        }
        
        
        [Test]
        public async Task GetOrderByIdAsync_ReturnsOk_WhenOrderExists()
        {
            // Arrange
            long orderId = 123;
            var mockOrder = new Order { Id = orderId, Name = "Sample Order" };
            _mockOrderService
                .Setup(service => service.GetAsync(orderId, default, default))
                .ReturnsAsync(mockOrder);

            // Act
            var result = await _controller.GetOrderByIdAsync(orderId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(mockOrder, okResult.Value);
        }

        [Test]
        public async Task GetOrderByIdAsync_ReturnsNotFound_WhenOrderDoesNotExist()
        {
            // Arrange
            long orderId = 123;
            _mockOrderService
                .Setup(service => service.GetAsync(orderId, default, default))
                .ReturnsAsync((ShopifySharp.Order)null); // Return null of the expected type

            // Act
            var result = await _controller.GetOrderByIdAsync(orderId);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result); // Check result type
            var notFoundResult = result as NotFoundObjectResult;

            Assert.IsNotNull($"Order with ID {orderId} not found", notFoundResult.Value?.ToString());
        }


        [Test]
        public async Task GetOrderByIdAsync_ThrowsException_WhenServiceThrowsShopifyException()
        {
            // Arrange
            long orderId = 123;
            var exceptionMessage = "Service error";
            _mockOrderService
                .Setup(service => service.GetAsync(orderId, default, default))
                .ThrowsAsync(new ShopifyException(exceptionMessage));

            // Act & Assert
            var exception = Assert.ThrowsAsync<Exception>(async () => await _controller.GetOrderByIdAsync(orderId));
            Assert.IsNotNull(exception);
            Assert.IsTrue(exception.Message.Contains($"Failed to retrieve order with ID {orderId}"));
            Assert.IsTrue(exception.InnerException?.Message.Contains(exceptionMessage));
        }

[Test]
    public async Task PutProduct_ReturnsOk_WhenOrderIsUpdatedSuccessfully()
    {
        // Arrange
        long orderId = 123;
        var order = new Order
        {
            Id = 1,
            AppId = 1234567,
            Name = "John Doe",
            ShippingAddress = new Address
            {
                FirstName = "John",
                LastName = "Doe",
                Address1 = "123 Main Street",
                City = "Springfield",
                Province = "IL",
                Zip = "62704",
                Country = "USA"
            },
            TotalPrice = 219.97m
        };

        var updatedOrder = new Order
        {
            Id = 1,
            AppId = 1234567,
            Name = "Regine Wang",
            ShippingAddress = order.ShippingAddress,
            TotalPrice = 219.97m 
        };

        _mockOrderService
            .Setup(service => service.UpdateAsync(orderId, order, default))
            .ReturnsAsync(updatedOrder);

        // Act
        var result = await _controller.PutProduct(orderId, order);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.AreEqual(updatedOrder, okResult.Value);
    }

    [Test]
    public async Task PutProduct_ReturnsBadRequest_WhenInputExceptionIsThrown()
    {
        // Arrange
        long orderId = 123;
        var order = new Order
        {
            Id = 1,
            AppId = 1234567,
            Name = "John Doe",
            ShippingAddress = new Address { FirstName = "John", LastName = "Doe" }
        };

        _mockOrderService
            .Setup(service => service.UpdateAsync(orderId, order, default))
            .ThrowsAsync(new InputException("Invalid input"));

        // Act
        var result = await _controller.PutProduct(orderId, order);

        // Assert
        var objectResult = result as ObjectResult; 
        Assert.IsNotNull(objectResult); 
        Assert.That(objectResult.StatusCode, Is.EqualTo(400)); 
        
        var value = JObject.FromObject(objectResult.Value); 
        Assert.AreEqual("Invalid input", value["message"]?.ToString());
    }

    [Test]
    public async Task PutProduct_ReturnsServerError_WhenShopifyExceptionIsThrown()
    {
        // Arrange
        long orderId = 123;
        var order = new Order
        {
            Id = 1,
            AppId = 1234567,
            Name = "John Doe"
        };

        _mockOrderService
            .Setup(service => service.UpdateAsync(orderId, order, default))
            .ThrowsAsync(new ShopifyException("Shopify service error"));

        // Act
        var result = await _controller.PutProduct(orderId, order);

        // Assert
        var objectResult = result as ObjectResult; 
        Assert.IsNotNull(objectResult); 
        Assert.That(objectResult.StatusCode, Is.EqualTo(500)); 
        
        var value = JObject.FromObject(objectResult.Value); 
        Assert.AreEqual("Error updating order", value["message"]?.ToString());
    }
}