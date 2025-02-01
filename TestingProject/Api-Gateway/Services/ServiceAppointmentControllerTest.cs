using Moq;
using NUnit.Framework;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Api_Gateway.Services;
using Newtonsoft.Json;
using Api_Gateway.Models;
using Microsoft.AspNetCore.Mvc;
using Moq.Protected;

namespace TestingProject.AppointmentsService.Tests
{
[TestFixture]
public class ServiceAppointmentsControllerTests
{
    private Mock<IHttpClientFactory> _mockHttpClientFactory;
    private ServiceAppointmentsController _controller;

    [SetUp]
    public void SetUp()
    {
        // Initialize the mock HttpClientFactory
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();

        // Create a mock HttpMessageHandler that will mock the SendAsync method
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

        // Create HttpClient using the mocked HttpMessageHandler
        var mockHttpClient = new HttpClient(mockHttpMessageHandler.Object);

        // Setup the factory to return the mocked HttpClient
        _mockHttpClientFactory.Setup(factory => factory.CreateClient(It.IsAny<string>()))
            .Returns(mockHttpClient);

        // Create the controller with the mocked HttpClientFactory
        _controller = new ServiceAppointmentsController(_mockHttpClientFactory.Object);
    }

    [Test]
    public async Task GetAllAppointmentsAsync_Success_ReturnsAppointmentsList()
    {
        // Arrange
        var appointmentsResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("[{\"AppointmentId\":\"12345\", \"Title\":\"Test Appointment\", \"Description\":\"Test Description\", \"AppointmentDate\":\"2025-01-20T10:00:00\", \"CustomerName\":\"John Doe\", \"CustomerEmail\":\"john.doe@example.com\", \"Status\":\"Upcoming\", \"CreatedAt\":\"2025-01-19T12:00:00\"}]")
        };

        _mockHttpClientFactory.Setup(client => client.CreateClient(It.IsAny<string>())).Returns(new HttpClient(new FakeHttpMessageHandler(appointmentsResponse)));

        // Act
        var result = await _controller.GetAllAppointmentsAsync();

        // Assert
        // Cast the result to OkObjectResult
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult); // Ensure the result is OkObjectResult

        // Extract the list of appointments
        var appointments = okResult.Value as List<Appointment>;
        Assert.IsNotNull(appointments); // Ensure the value is a list of appointments

        // Check if the list contains the expected appointment
        Assert.IsTrue(appointments.Any(a => a.Title == "Test Appointment"));
    }

    [Test]
    public async Task GetAllAppointmentsAsync_Failure_ReturnsErrorMessage()
    {
        // Arrange
        var errorResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.BadRequest,
            Content = new StringContent("{\"Message\": \"Client Error: Bad Request\"}", Encoding.UTF8, "application/json")
        };

        _mockHttpClientFactory
            .Setup(client => client.CreateClient(It.IsAny<string>()))
            .Returns(new HttpClient(new FakeHttpMessageHandler(errorResponse)));

        // Act
        var result = await _controller.GetAllAppointmentsAsync();

        // Assert
        var objectResult = result as BadRequestObjectResult;
        Assert.IsNotNull(objectResult);
    
        var responseContent = objectResult.Value as dynamic;
        Assert.IsNotNull(responseContent);
        Assert.AreEqual("Client Error: Bad Request", responseContent.Message);
    }
    
    [Test]
    public async Task GetAllAppointmentsAsync_InternalServerError_ReturnsErrorMessage()
    {
        // Arrange
        var errorResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.InternalServerError,
            Content = new StringContent("{\"Message\": \"Internal Server Error\"}", Encoding.UTF8, "application/json")
        };

        _mockHttpClientFactory
            .Setup(client => client.CreateClient(It.IsAny<string>()))
            .Returns(new HttpClient(new FakeHttpMessageHandler(errorResponse)));

        // Act
        var result = await _controller.GetAllAppointmentsAsync();

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.AreEqual(500, objectResult.StatusCode); // Ensure it's an Internal Server Error

        var responseContent = objectResult.Value as dynamic;
        Assert.IsNotNull(responseContent);
        Assert.AreEqual("Internal Server Error", responseContent.Message);
    }
    
    [Test]
    public async Task GetAllAppointmentsAsync_Exception_ReturnsErrorMessage()
    {
        // Arrange
        _mockHttpClientFactory
            .Setup(client => client.CreateClient(It.IsAny<string>()))
            .Throws(new Exception("Network error"));

        // Act
        var result = await _controller.GetAllAppointmentsAsync();

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.AreEqual(500, objectResult.StatusCode); // Ensure it's an Internal Server Error

        var responseContent = objectResult.Value as dynamic;
        Assert.IsNotNull(responseContent);
        Assert.AreEqual("Internal Server Error", responseContent.Message);
        Assert.AreEqual("Network error", responseContent.Details);
    }

    [Test]
    public async Task GetAppointmentByIdAsync_Success_ReturnsAppointmentDetails()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var appointmentResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("{\"AppointmentId\":\"12345\", \"Title\":\"Test Appointment\", \"Description\":\"Test Description\", \"AppointmentDate\":\"2025-01-20T10:00:00\", \"CustomerName\":\"John Doe\", \"CustomerEmail\":\"john.doe@example.com\", \"Status\":\"Upcoming\", \"CreatedAt\":\"2025-01-19T12:00:00\"}")
        };

        _mockHttpClientFactory.Setup(client => client.CreateClient(It.IsAny<string>())).Returns(new HttpClient(new FakeHttpMessageHandler(appointmentResponse)));

        // Act
        var result = await _controller.GetAppointmentByIdAsync(appointmentId);

        // Assert
        Assert.IsTrue(result.Contains("Test Appointment"));
    }
    
    [Test]
    public async Task GetAppointmentByIdAsync_NotFound_ReturnsErrorMessage()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var errorResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NotFound,
            Content = new StringContent("Not Found")
        };

        _mockHttpClientFactory.Setup(client => client.CreateClient(It.IsAny<string>())).Returns(new HttpClient(new FakeHttpMessageHandler(errorResponse)));

        // Act
        var result = await _controller.GetAppointmentByIdAsync(appointmentId);

        // Assert
        Assert.AreEqual(result, "Error fetching appointment: Not Found");
    }
    
    [Test]
    public async Task GetAppointmentByIdAsync_InternalServerError_ReturnsErrorMessage()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var errorResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.InternalServerError,
            Content = new StringContent("Internal Server Error")
        };

        _mockHttpClientFactory.Setup(client => client.CreateClient(It.IsAny<string>())).Returns(new HttpClient(new FakeHttpMessageHandler(errorResponse)));

        // Act
        var result = await _controller.GetAppointmentByIdAsync(appointmentId);

        // Assert
        Assert.AreEqual(result, "Error fetching appointment: Internal Server Error");
    }
    

    [Test]
    public async Task CreateAppointmentAsync_Success_ReturnsAppointmentCreatedMessage()
    {
        // Arrange
        var appointment = new Appointment
        {
            AppointmentId = Guid.NewGuid(),
            Title = "New Appointment",
            Description = "Test description",
            AppointmentDate = DateTime.UtcNow.AddDays(1),
            CustomerName = "Jane Doe",
            CustomerEmail = "jane.doe@example.com",
            Status = "Upcoming",
            CreatedAt = DateTime.UtcNow
        };

        var appointmentResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.Created,
            Content = new StringContent("{\"AppointmentId\":\"12345\", \"Title\":\"New Appointment\", \"Description\":\"Test description\", \"AppointmentDate\":\"2025-01-20T10:00:00\", \"CustomerName\":\"Jane Doe\", \"CustomerEmail\":\"jane.doe@example.com\", \"Status\":\"Upcoming\", \"CreatedAt\":\"2025-01-19T12:00:00\"}")
        };

        _mockHttpClientFactory.Setup(client => client.CreateClient(It.IsAny<string>())).Returns(new HttpClient(new FakeHttpMessageHandler(appointmentResponse)));

        // Act
        var result = await _controller.CreateAppointmentAsync(appointment);

        // Assert
        Assert.IsTrue(result.Contains("AppointmentId"));
    }

    [Test]
    public async Task CreateAppointmentAsync_BadRequest_ReturnsErrorMessage()
    {
        // Arrange
        var appointment = new Appointment
        {
            AppointmentId = Guid.NewGuid(),
            Title = "New Appointment",
            Description = "Test description",
            AppointmentDate = DateTime.UtcNow.AddDays(1),
            CustomerName = "Jane Doe",
            CustomerEmail = "jane.doe@example.com",
            Status = "InvalidStatus",  // Invalid status
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var result = await _controller.CreateAppointmentAsync(appointment);

        // Assert
        Assert.AreEqual(result, "Error 400: Status must be 'Cancelled', 'Upcoming', or 'Finished'");
    }
    
    [Test]
    public async Task CreateAppointmentAsync_InvalidStatus_ReturnsErrorMessage()
    {
        // Arrange
        var appointment = new Appointment
        {
            AppointmentId = Guid.NewGuid(),
            Title = "New Appointment",
            Description = "Test description",
            AppointmentDate = DateTime.UtcNow.AddDays(1),
            CustomerName = "Jane Doe",
            CustomerEmail = "jane.doe@example.com",
            Status = "InvalidStatus", // Invalid status
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var result = await _controller.CreateAppointmentAsync(appointment);

        // Assert
        Assert.AreEqual(result, "Error 400: Status must be 'Cancelled', 'Upcoming', or 'Finished'");
    }
    
    [Test]
    public async Task CreateAppointmentAsync_InternalServerError_ReturnsErrorMessage()
    {
        // Arrange
        var appointment = new Appointment
        {
            AppointmentId = Guid.NewGuid(),
            Title = "New Appointment",
            Description = "Test description",
            AppointmentDate = DateTime.UtcNow.AddDays(1),
            CustomerName = "Jane Doe",
            CustomerEmail = "jane.doe@example.com",
            Status = "Upcoming",
            CreatedAt = DateTime.UtcNow
        };

        var errorResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.InternalServerError,
            Content = new StringContent("Internal Server Error")
        };

        _mockHttpClientFactory.Setup(client => client.CreateClient(It.IsAny<string>())).Returns(new HttpClient(new FakeHttpMessageHandler(errorResponse)));

        // Act
        var result = await _controller.CreateAppointmentAsync(appointment);

        // Assert
        Assert.AreEqual(result, "Error creating appointment: Internal Server Error");
    }
    



    [Test]
    public async Task UpdateAppointmentAsync_Success_ReturnsSuccessMessage()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var appointment = new Appointment
        {
            AppointmentId = appointmentId,
            Title = "Updated Appointment",
            Description = "Updated Description",
            AppointmentDate = DateTime.UtcNow.AddDays(2),
            CustomerName = "John Doe",
            CustomerEmail = "john.doe@example.com",
            Status = "Finished",
            CreatedAt = DateTime.UtcNow
        };

        var appointmentResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("Appointment updated successfully")
        };

        _mockHttpClientFactory.Setup(client => client.CreateClient(It.IsAny<string>())).Returns(new HttpClient(new FakeHttpMessageHandler(appointmentResponse)));

        // Act
        var result = await _controller.UpdateAppointmentAsync(appointmentId, appointment);

        // Assert
        Assert.AreEqual(result, "Appointment updated successfully");
    }

    [Test]
    public async Task UpdateAppointmentAsync_BadRequest_ReturnsErrorMessage()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var appointment = new Appointment
        {
            AppointmentId = appointmentId,
            Title = "Updated Appointment",
            Description = "Updated Description",
            AppointmentDate = DateTime.UtcNow.AddDays(2),
            CustomerName = "John Doe",
            CustomerEmail = "john.doe@example.com",
            Status = "InvalidStatus",  // Invalid status
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var result = await _controller.UpdateAppointmentAsync(appointmentId, appointment);

        // Assert
        Assert.AreEqual(result, "Error 400: Status must be 'Cancelled', 'Upcoming', or 'Finished'");
    }
    
    [Test]
    public async Task UpdateAppointmentAsync_InvalidStatus_ReturnsErrorMessage()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var appointment = new Appointment
        {
            AppointmentId = appointmentId,
            Title = "Updated Appointment",
            Description = "Updated Description",
            AppointmentDate = DateTime.UtcNow.AddDays(2),
            CustomerName = "John Doe",
            CustomerEmail = "john.doe@example.com",
            Status = "InvalidStatus",  // Invalid status
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var result = await _controller.UpdateAppointmentAsync(appointmentId, appointment);

        // Assert
        Assert.AreEqual(result, "Error 400: Status must be 'Cancelled', 'Upcoming', or 'Finished'");
    }

    [Test]
    public async Task UpdateAppointmentAsync_InternalServerError_ReturnsErrorMessage()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var appointment = new Appointment
        {
            AppointmentId = appointmentId,
            Title = "Updated Appointment",
            Description = "Updated Description",
            AppointmentDate = DateTime.UtcNow.AddDays(2),
            CustomerName = "John Doe",
            CustomerEmail = "john.doe@example.com",
            Status = "Upcoming",
            CreatedAt = DateTime.UtcNow
        };

        var errorResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.InternalServerError,
            Content = new StringContent("Internal Server Error")
        };

        _mockHttpClientFactory.Setup(client => client.CreateClient(It.IsAny<string>())).Returns(new HttpClient(new FakeHttpMessageHandler(errorResponse)));

        // Act
        var result = await _controller.UpdateAppointmentAsync(appointmentId, appointment);

        // Assert
        Assert.AreEqual(result, "Error updating appointment: Internal Server Error");
    }


    [Test]
    public async Task DeleteAppointmentAsync_Success_ReturnsSuccessMessage()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var deleteResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NoContent
        };

        _mockHttpClientFactory.Setup(client => client.CreateClient(It.IsAny<string>())).Returns(new HttpClient(new FakeHttpMessageHandler(deleteResponse)));

        // Act
        var result = await _controller.DeleteAppointmentAsync(appointmentId);

        // Assert
        Assert.AreEqual(result, "Appointment deleted successfully");
    }

    [Test]
    public async Task DeleteAppointmentAsync_Failure_ReturnsErrorMessage()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var errorResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NotFound,
            Content = new StringContent("Not Found")
        };

        _mockHttpClientFactory.Setup(client => client.CreateClient(It.IsAny<string>())).Returns(new HttpClient(new FakeHttpMessageHandler(errorResponse)));

        // Act
        var result = await _controller.DeleteAppointmentAsync(appointmentId);

        // Assert
        Assert.AreEqual(result, "Error deleting appointment: Not Found");
    }
    
    [Test]
    public async Task DeleteAppointmentAsync_NotFound_ReturnsErrorMessage()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var errorResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NotFound,
            Content = new StringContent("Not Found")
        };

        _mockHttpClientFactory.Setup(client => client.CreateClient(It.IsAny<string>())).Returns(new HttpClient(new FakeHttpMessageHandler(errorResponse)));

        // Act
        var result = await _controller.DeleteAppointmentAsync(appointmentId);

        // Assert
        Assert.AreEqual(result, "Error deleting appointment: Not Found");
    }
    
    [Test]
    public async Task DeleteAppointmentAsync_InternalServerError_ReturnsErrorMessage()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var errorResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.InternalServerError,
            Content = new StringContent("Internal Server Error")
        };

        _mockHttpClientFactory.Setup(client => client.CreateClient(It.IsAny<string>())).Returns(new HttpClient(new FakeHttpMessageHandler(errorResponse)));

        // Act
        var result = await _controller.DeleteAppointmentAsync(appointmentId);

        // Assert
        Assert.AreEqual(result, "Error deleting appointment: Internal Server Error");
    }



    // Helper class to mock HttpMessageHandler
    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage _response;

        public FakeHttpMessageHandler(HttpResponseMessage response)
        {
            _response = response;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            return Task.FromResult(_response);
        }
    }
}

}
