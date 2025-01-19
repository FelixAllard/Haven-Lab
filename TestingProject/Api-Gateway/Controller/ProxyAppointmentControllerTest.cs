using Api_Gateway.Controller;
using Api_Gateway.Models;
using Api_Gateway.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace TestingProject.Api_Gateway.Controller;

[TestFixture]
public class ProxyAppointmentControllerTest
{
    private Mock<ServiceAppointmentsController> _mockServiceAppointmentsController;
    private ProxyAppointmentController _proxyAppointmentController;

    [SetUp]
    public void SetUp()
    {
        // Mock the ServiceAppointmentsController
        _mockServiceAppointmentsController = new Mock<ServiceAppointmentsController>(null);
    
        // Ensure the mock returns an Appointment JSON string when GetAppointmentByIdAsync is called
        _mockServiceAppointmentsController.Setup(service => service.GetAppointmentByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(@"
            {
                ""appointmentId"": ""f47ac10b-58cc-4372-a567-0e02b2c3d479"",
                ""title"": ""Haircut"",
                ""description"": ""Routine checkup"",
                ""appointmentDate"": ""2025-01-25T09:00:00"",
                ""customerName"": ""John Doe"",
                ""customerEmail"": ""john.doe@example.com"",
                ""status"": ""Upcoming"",
                ""createdAt"": ""2025-01-10T10:30:00""
            }");

        // ProxyAppointmentController depends on ServiceAppointmentsController
        _proxyAppointmentController = new ProxyAppointmentController(_mockServiceAppointmentsController.Object);
    }


    [Test]
    public async Task GetAllAppointments_ReturnsOkResult_WhenServiceReturnsData()
    {
        // Arrange: Prepare the mock response data
        var mockResult = @"
        [
            {
                ""id"": 1,
                ""appointment_id"": ""f47ac10b-58cc-4372-a567-0e02b2c3d479"",
                ""title"": ""Haircut1"",
                ""description"": ""Routine checkup"",
                ""appointment_date"": ""2025-01-25T09:00:00"",
                ""customer_name"": ""John Doe"",
                ""customer_email"": ""john.doe@example.com"",
                ""status"": ""Confirmed"",
                ""created_at"": ""2025-01-10T10:30:00""
            },
            {
                ""id"": 2,
                ""appointment_id"": ""a4f1f0c0-06f1-4b77-b5b5-9872b0d9f124"",
                ""title"": ""Haircut2"",
                ""description"": ""Comprehensive exam"",
                ""appointment_date"": ""2025-01-26T11:00:00"",
                ""customer_name"": ""Jane Smith"",
                ""customer_email"": ""jane.smith@example.com"",
                ""status"": ""Pending"",
                ""created_at"": ""2025-01-11T11:45:00""
            }
        ]";

        // Mock the ServiceAppointmentsController to return mock data
        _mockServiceAppointmentsController.Setup(service => service.GetAllAppointmentsAsync())
            .ReturnsAsync(mockResult);

        // Act: Call the method under test
        var result = await _proxyAppointmentController.GetAllAppointments();

        // Assert: Verify that the response is Ok with the expected data
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.That(okResult.Value, Is.EqualTo(mockResult));
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
    }
    
    [Test]
    public async Task GetAllAppointments_ReturnsBadRequest_WhenServiceReturnsErrorMessage()
    {
        // Arrange: Prepare the error message
        var errorMessage = "Error fetching appointments: Some error occurred";

        // Mock the ServiceAppointmentsController to return the error message
        _mockServiceAppointmentsController.Setup(service => service.GetAllAppointmentsAsync())
            .ReturnsAsync(errorMessage);

        // Act: Call the method under test
        var result = await _proxyAppointmentController.GetAllAppointments();

        // Assert: Verify that the response is BadRequest with the expected error message
        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));

        // Extract the message from the response for comparison
        var message = badRequestResult.Value.GetType().GetProperty("Message").GetValue(badRequestResult.Value, null);
        Assert.That(message, Is.EqualTo(errorMessage));
    }
    
    [Test]
    public async Task GetAllAppointments_ReturnsInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange: Mock an exception when calling GetAllAppointmentsAsync
        _mockServiceAppointmentsController.Setup(service => service.GetAllAppointmentsAsync())
            .ThrowsAsync(new HttpRequestException("Internal server error"));

        // Act: Call the method under test
        var result = await _proxyAppointmentController.GetAllAppointments();

        // Assert: Verify that the response is Internal Server Error (500)
        var internalServerErrorResult = result as ObjectResult;
        Assert.IsNotNull(internalServerErrorResult);
        Assert.That(internalServerErrorResult.StatusCode, Is.EqualTo(500));

        // Extract the message from the response for comparison
        var message = internalServerErrorResult.Value.GetType().GetProperty("Message").GetValue(internalServerErrorResult.Value, null);
        Assert.That(message, Is.EqualTo("Internal server error"));
    }
    
    [Test]
    public async Task CreateAppointment_ReturnsCreatedAtAction_WhenAppointmentIsValid()
    {
        // Arrange: Prepare a valid appointment object
        var appointment = new Appointment
        {
            AppointmentId = Guid.NewGuid(),
            Title = "Haircut",
            Description = "Routine checkup",
            AppointmentDate = DateTime.Now.AddHours(1),
            CustomerName = "John Doe",
            CustomerEmail = "john.doe@example.com",
            Status = "Upcoming", // Valid status
            CreatedAt = DateTime.UtcNow
        };

        var mockResult = "Appointment created successfully"; // Mocked response from service

        // Mock the ServiceAppointmentsController to return a success message
        _mockServiceAppointmentsController.Setup(service => service.CreateAppointmentAsync(appointment))
            .ReturnsAsync(mockResult);

        // Act: Call the CreateAppointment method
        var result = await _proxyAppointmentController.CreateAppointment(appointment);

        // Assert: Verify that the response is CreatedAtAction with the appropriate status code
        var createdResult = result as CreatedAtActionResult;
        Assert.IsNotNull(createdResult);
        Assert.That(createdResult.StatusCode, Is.EqualTo(201)); // Created status code
        Assert.That(createdResult.Value, Is.EqualTo(mockResult));
    }
    

    [Test]
    public async Task UpdateAppointment_ReturnsOk_WhenUpdateIsSuccessful()
    {
        // Arrange: Prepare an appointment object to update
        var appointment = new Appointment
        {
            AppointmentId = Guid.NewGuid(),
            Title = "Haircut",
            Description = "Checkup",
            AppointmentDate = DateTime.Now.AddHours(2),
            CustomerName = "John Doe",
            CustomerEmail = "john.doe@example.com",
            Status = "Finished",
            CreatedAt = DateTime.UtcNow
        };

        var mockResult = "Appointment updated successfully"; // Mocked response from service

        // Mock the ServiceAppointmentsController to return success message
        _mockServiceAppointmentsController.Setup(service => service.UpdateAppointmentAsync(appointment.AppointmentId, appointment))
            .ReturnsAsync(mockResult);

        // Act: Call the UpdateAppointment method
        var result = await _proxyAppointmentController.UpdateAppointment(appointment.AppointmentId, appointment);

        // Assert: Verify that the response is Ok
        var okResult = result as OkResult;
        Assert.IsNotNull(okResult);
        Assert.That(okResult.StatusCode, Is.EqualTo(200)); // OK status code
    }

    [Test]
    public async Task DeleteAppointment_ReturnsOk_WhenDeleteIsSuccessful()
    {
        // Arrange: Prepare the appointmentId to delete
        var appointmentId = Guid.NewGuid();

        var mockResult = "Appointment deleted successfully"; // Mocked response from service

        // Mock the ServiceAppointmentsController to return success message
        _mockServiceAppointmentsController.Setup(service => service.DeleteAppointmentAsync(appointmentId))
            .ReturnsAsync(mockResult);

        // Act: Call the DeleteAppointment method
        var result = await _proxyAppointmentController.DeleteAppointment(appointmentId);

        // Assert: Verify that the response is Ok
        var okResult = result as OkResult;
        Assert.IsNotNull(okResult);
        Assert.That(okResult.StatusCode, Is.EqualTo(200)); // OK status code
    }

    [Test]
    public async Task GetAppointmentById_ReturnsOk_WhenAppointmentExists()
    {
        // Arrange: Prepare the appointmentId to fetch
        var appointmentId = Guid.NewGuid();
        var mockResult = @"
    {
        ""appointmentId"": ""f47ac10b-58cc-4372-a567-0e02b2c3d479"",
        ""title"": ""Haircut"",
        ""description"": ""Routine checkup"",
        ""appointmentDate"": ""2025-01-25T09:00:00"",
        ""customerName"": ""John Doe"",
        ""customerEmail"": ""john.doe@example.com"",
        ""status"": ""Upcoming"",
        ""createdAt"": ""2025-01-10T10:30:00""
    }";

        // Mock the ServiceAppointmentsController to return the appointment
        _mockServiceAppointmentsController.Setup(service => service.GetAppointmentByIdAsync(appointmentId))
            .ReturnsAsync(mockResult);

        // Act: Call the GetAppointmentById method
        var result = await _proxyAppointmentController.GetAppointmentById(appointmentId);

        // Assert: Verify that the response is Ok with the correct appointment data
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.That(okResult.StatusCode, Is.EqualTo(200)); // OK status code
        Assert.That(okResult.Value, Is.EqualTo(mockResult)); // Verify the returned value (the JSON string)
    }
    
    [Test]
    public async Task GetAppointmentById_ReturnsBadRequest_WhenErrorOccurs()
    {
        // Arrange: Prepare the appointmentId to fetch
        var appointmentId = Guid.NewGuid();
        var errorMessage = "Error: Appointment not found";

        // Mock the ServiceAppointmentsController to return an error message
        _mockServiceAppointmentsController.Setup(service => service.GetAppointmentByIdAsync(appointmentId))
            .ReturnsAsync(errorMessage);

        // Act: Call the GetAppointmentById method
        var result = await _proxyAppointmentController.GetAppointmentById(appointmentId);

        // Assert: Verify that the response is BadRequest with the error message
        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.That(badRequestResult.StatusCode, Is.EqualTo(400)); // BadRequest status code

        // Verify the error message
        var message = badRequestResult.Value.GetType().GetProperty("Message").GetValue(badRequestResult.Value, null);
        Assert.That(message, Is.EqualTo(errorMessage));
    }
    
    
    [Test]
    public async Task GetAppointmentById_ReturnsInternalServerError_WhenExceptionOccurs()
    {
        // Arrange: Prepare the appointmentId to fetch
        var appointmentId = Guid.NewGuid();

        // Mock the ServiceAppointmentsController to throw an exception
        _mockServiceAppointmentsController.Setup(service => service.GetAppointmentByIdAsync(appointmentId))
            .ThrowsAsync(new Exception("Internal server error"));

        // Act: Call the GetAppointmentById method
        var result = await _proxyAppointmentController.GetAppointmentById(appointmentId);

        // Assert: Verify that the response is Internal Server Error (500)
        var internalServerErrorResult = result as ObjectResult;
        Assert.IsNotNull(internalServerErrorResult);
        Assert.That(internalServerErrorResult.StatusCode, Is.EqualTo(500)); // Internal Server Error
        var message = internalServerErrorResult.Value.GetType().GetProperty("Message").GetValue(internalServerErrorResult.Value, null);
        Assert.That(message, Is.EqualTo("Internal server error"));
    }


    [Test]
    public async Task UpdateAppointment_ReturnsInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange: Mock an exception when calling UpdateAppointmentAsync
        var appointment = new Appointment
        {
            AppointmentId = Guid.NewGuid(),
            Title = "Haircut",
            Description = "Routine checkup",
            AppointmentDate = DateTime.Now.AddHours(1),
            CustomerName = "John Doe",
            CustomerEmail = "john.doe@example.com",
            Status = "Upcoming",
            CreatedAt = DateTime.UtcNow
        };
        _mockServiceAppointmentsController.Setup(service => service.UpdateAppointmentAsync(It.IsAny<Guid>(), It.IsAny<Appointment>()))
            .ThrowsAsync(new Exception("Internal server error"));

        // Act: Call the UpdateAppointment method
        var result = await _proxyAppointmentController.UpdateAppointment(appointment.AppointmentId, appointment);

        // Assert: Verify that the response is Internal Server Error (500)
        var internalServerErrorResult = result as ObjectResult;
        Assert.IsNotNull(internalServerErrorResult);
        Assert.That(internalServerErrorResult.StatusCode, Is.EqualTo(500));
        var message = internalServerErrorResult.Value.GetType().GetProperty("Message").GetValue(internalServerErrorResult.Value, null);
        Assert.That(message, Is.EqualTo("Internal server error"));
    }

    // Negative Tests for Delete Appointment
    [Test]
    public async Task DeleteAppointment_ReturnsBadRequest_WhenErrorOccurs()
    {
        // Arrange: Prepare the appointmentId to delete
        var appointmentId = Guid.NewGuid();
        var errorMessage = "Error: Appointment could not be deleted";

        // Mock the ServiceAppointmentsController to return an error message
        _mockServiceAppointmentsController.Setup(service => service.DeleteAppointmentAsync(appointmentId))
            .ReturnsAsync(errorMessage);

        // Act: Call the DeleteAppointment method
        var result = await _proxyAppointmentController.DeleteAppointment(appointmentId);

        // Assert: Verify that the response is BadRequest with the error message
        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.That(badRequestResult.StatusCode, Is.EqualTo(400)); // BadRequest status code

        // Verify the error message
        var message = badRequestResult.Value.GetType().GetProperty("Message").GetValue(badRequestResult.Value, null);
        Assert.That(message, Is.EqualTo(errorMessage));
    }

    [Test]
    public async Task DeleteAppointment_ReturnsInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange: Mock an exception when calling DeleteAppointmentAsync
        var appointmentId = Guid.NewGuid();
        _mockServiceAppointmentsController.Setup(service => service.DeleteAppointmentAsync(It.IsAny<Guid>()))
            .ThrowsAsync(new Exception("Internal server error"));

        // Act: Call the DeleteAppointment method
        var result = await _proxyAppointmentController.DeleteAppointment(appointmentId);

        // Assert: Verify that the response is Internal Server Error (500)
        var internalServerErrorResult = result as ObjectResult;
        Assert.IsNotNull(internalServerErrorResult);
        Assert.That(internalServerErrorResult.StatusCode, Is.EqualTo(500));

        // Extract the message from the response for comparison
        var message = internalServerErrorResult.Value.GetType().GetProperty("Message").GetValue(internalServerErrorResult.Value, null);
        Assert.That(message, Is.EqualTo("Internal server error"));
    }
    
    [Test]
    public async Task CreateAppointment_ReturnsBadRequest_WhenStatusIsInvalid()
    {
        // Arrange: Prepare an appointment with an invalid status
        var appointment = new Appointment
        {
            AppointmentId = Guid.NewGuid(),
            Title = "Haircut",
            Description = "Routine checkup",
            AppointmentDate = DateTime.Now.AddHours(1),
            CustomerName = "John Doe",
            CustomerEmail = "john.doe@example.com",
            Status = "InvalidStatus", // Invalid status
            CreatedAt = DateTime.UtcNow
        };

        // Act: Call the CreateAppointment method
        var result = await _proxyAppointmentController.CreateAppointment(appointment);
        
        // Assert: Verify that the response is a BadRequest
        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.That(badRequestResult.StatusCode, Is.EqualTo(400)); // BadRequest status 
        var message = badRequestResult.Value.GetType().GetProperty("Message").GetValue(badRequestResult.Value, null);
        Assert.That(message, Is.EqualTo("Status must be 'Cancelled', 'Upcoming', or 'Finished'"));
    }

    [Test]
    public async Task CreateAppointment_ReturnsInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange: Prepare a valid appointment object
        var appointment = new Appointment
        {
            AppointmentId = Guid.NewGuid(),
            Title = "Haircut",
            Description = "Routine checkup",
            AppointmentDate = DateTime.Now.AddHours(1),
            CustomerName = "John Doe",
            CustomerEmail = "john.doe@example.com",
            Status = "Upcoming", // Valid status
            CreatedAt = DateTime.UtcNow
        };

        // Arrange: Mock the ServiceAppointmentsController to throw an exception
        _mockServiceAppointmentsController.Setup(service => service.CreateAppointmentAsync(appointment))
            .ThrowsAsync(new Exception("Internal server error"));

        // Act: Call the CreateAppointment method
        var result = await _proxyAppointmentController.CreateAppointment(appointment);

        // Assert: Verify that the response is Internal Server Error (500)
        var internalServerErrorResult = result as ObjectResult;
        Assert.IsNotNull(internalServerErrorResult);
        Assert.That(internalServerErrorResult.StatusCode, Is.EqualTo(500)); // Internal Server Error status code

        // Verify the exception message
        var message = internalServerErrorResult.Value.GetType().GetProperty("Message").GetValue(internalServerErrorResult.Value, null);
        Assert.That(message, Is.EqualTo("Internal server error"));
    }

    [Test]
    public async Task UpdateAppointment_ReturnsBadRequest_WhenAppointmentNotFound()
    {
        // Arrange: Prepare an appointment with a valid ID but simulate it not being found in the service
        var appointment = new Appointment
        {
            AppointmentId = Guid.NewGuid(),
            Title = "Haircut",
            Description = "Routine checkup",
            AppointmentDate = DateTime.Now.AddHours(1),
            CustomerName = "John Doe",
            CustomerEmail = "john.doe@example.com",
            Status = "Upcoming",
            CreatedAt = DateTime.UtcNow
        };

        _mockServiceAppointmentsController.Setup(service => service.UpdateAppointmentAsync(It.IsAny<Guid>(), It.IsAny<Appointment>()))
            .ReturnsAsync("Error 404: Appointment not found");

        // Act: Call the UpdateAppointment method
        var result = await _proxyAppointmentController.UpdateAppointment(appointment.AppointmentId, appointment);

        // Assert: Verify that the response is a BadRequest
        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.That(badRequestResult.StatusCode, Is.EqualTo(400)); // BadRequest status code
        var message = badRequestResult.Value.GetType().GetProperty("Message").GetValue(badRequestResult.Value, null);
        Assert.That(message, Is.EqualTo("Error 404: Appointment not found"));
    }

    [Test]
    public async Task UpdateAppointment_ReturnsBadRequest_WhenAppointmentIdIsInvalid()
    {
        // Arrange: Prepare an appointment with an invalid appointmentId
        var invalidAppointmentId = Guid.NewGuid(); // Assume this ID does not exist in the system
        var appointment = new Appointment
        {
            AppointmentId = invalidAppointmentId,
            Title = "Haircut",
            Description = "Routine checkup",
            AppointmentDate = DateTime.Now.AddHours(1),
            CustomerName = "John Doe",
            CustomerEmail = "john.doe@example.com",
            Status = "Upcoming",
            CreatedAt = DateTime.UtcNow
        };

        _mockServiceAppointmentsController.Setup(service => service.UpdateAppointmentAsync(It.IsAny<Guid>(), It.IsAny<Appointment>()))
            .ReturnsAsync("Error 404: Appointment not found");

        // Act: Call the UpdateAppointment method with an invalid ID
        var result = await _proxyAppointmentController.UpdateAppointment(invalidAppointmentId, appointment);

        // Assert: Verify that the response is a BadRequest
        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.That(badRequestResult.StatusCode, Is.EqualTo(400)); // BadRequest status code
        var message = badRequestResult.Value.GetType().GetProperty("Message").GetValue(badRequestResult.Value, null);
        Assert.That(message, Is.EqualTo("Error 404: Appointment not found"));
    }

}
