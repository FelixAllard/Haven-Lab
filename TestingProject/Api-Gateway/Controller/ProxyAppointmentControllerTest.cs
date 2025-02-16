using Api_Gateway.Controller;
using Api_Gateway.Models;
using Api_Gateway.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;

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
    
        // Prepare the appointment object by deserializing the JSON string
        var appointment = JsonConvert.DeserializeObject<Appointment>(@"
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

        _mockServiceAppointmentsController.Setup(service => service.GetAppointmentByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new OkObjectResult(appointment));  // Return OkObjectResult with the deserialized appointment


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

        // Arrange: Deserialize the mock JSON into a List<Appointment>
        var mockAppointments = JsonConvert.DeserializeObject<List<Appointment>>(mockResult);

        _mockServiceAppointmentsController.Setup(service => service.GetAllAppointmentsAsync(null))
            .ReturnsAsync(new OkObjectResult(mockAppointments)); // Return a List<Appointment>

        // Act: Call the method under test
        var result = await _proxyAppointmentController.GetAllAppointments(null);

        // Assert: Ensure result is OkObjectResult and check response content
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));

        var returnedAppointments = okResult.Value as List<Appointment>;
        Assert.IsNotNull(returnedAppointments);
        Assert.That(returnedAppointments.Count, Is.EqualTo(2));
        Assert.That(returnedAppointments[0].Title, Is.EqualTo("Haircut1"));
        Assert.That(returnedAppointments[1].Title, Is.EqualTo("Haircut2"));
    }
    
    [Test]
    public async Task GetAllAppointments_ReturnsBadRequest_WhenServiceReturnsErrorMessage()
    {
        // Arrange: Prepare the error message
        var errorMessage = "Invalid request parameters";

        // Mock the ServiceAppointmentsController to return the error message
        _mockServiceAppointmentsController.Setup(service => service.GetAllAppointmentsAsync(null))
            .ReturnsAsync(new BadRequestObjectResult("Invalid request"));

        // Act: Call the method under test
        var result = await _proxyAppointmentController.GetAllAppointments(null);

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
        _mockServiceAppointmentsController.Setup(service => service.GetAllAppointmentsAsync(null))
            .ThrowsAsync(new HttpRequestException("Service unavailable"));

        // Act: Call the method under test
        var result = await _proxyAppointmentController.GetAllAppointments(null);

        // Assert: Verify that the response is Internal Server Error (500)
        var internalServerErrorResult = result as ObjectResult;
        Assert.IsNotNull(internalServerErrorResult);
        Assert.That(internalServerErrorResult.StatusCode, Is.EqualTo(503));

        // Extract the message from the response for comparison
        var message = internalServerErrorResult.Value.GetType().GetProperty("Message").GetValue(internalServerErrorResult.Value, null);
        Assert.That(message, Is.EqualTo("Service unavailable"));
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
            AppointmentDate = DateTime.UtcNow.AddHours(1), // Ensure future appointment date
            CustomerName = "John Doe",
            CustomerEmail = "john.doe@example.com",
            Status = "Upcoming", // Valid status
            CreatedAt = DateTime.UtcNow
        };

        // Expected result: CreatedAtActionResult
        var expectedResult = new CreatedAtActionResult(
            nameof(_proxyAppointmentController.GetAppointmentById), // Action name
            "ProxyAppointment", // Controller name
            new { appointmentId = appointment.AppointmentId }, // Route values
            appointment // Response content
        );

        // Mock the ServiceAppointmentsController to return a CreatedAtActionResult
        _mockServiceAppointmentsController
            .Setup(service => service.CreateAppointmentAsync(It.IsAny<Appointment>()))
            .ReturnsAsync(expectedResult);

        // Act: Call the CreateAppointment method
        var result = await _proxyAppointmentController.CreateAppointment(appointment);

        // Assert: Verify that the response is CreatedAtActionResult with the appropriate status code
        var createdResult = result as CreatedAtActionResult;
        Assert.IsNotNull(createdResult, "Expected CreatedAtActionResult, but got null.");
        Assert.That(createdResult.StatusCode, Is.EqualTo(201), "Expected HTTP 201 Created status.");
        Assert.That(createdResult.Value, Is.EqualTo(appointment), "Returned appointment data does not match.");
    }

    [Test]
public async Task GetAppointmentById_ReturnsOk_WhenAppointmentExists()
{
    // Arrange: Prepare the appointmentId to fetch
    var appointmentId = Guid.NewGuid();

    // Prepare a valid appointment object that matches the expected response
    var appointment = new Appointment
    {
        AppointmentId = Guid.NewGuid(),
        Title = "Haircut",
        Description = "Routine checkup",
        AppointmentDate = DateTime.Parse("2025-01-25T09:00:00"),
        CustomerName = "John Doe",
        CustomerEmail = "john.doe@example.com",
        Status = "Upcoming",
        CreatedAt = DateTime.Parse("2025-01-10T10:30:00")
    };

    // Mock the ServiceAppointmentsController to return an OkObjectResult with the appointment object
    _mockServiceAppointmentsController.Setup(service => service.GetAppointmentByIdAsync(appointmentId))
        .ReturnsAsync(new OkObjectResult(appointment));  // Return an OkObjectResult with the appointment

    // Act: Call the GetAppointmentById method
    var result = await _proxyAppointmentController.GetAppointmentById(appointmentId);

    // Assert: Verify that the response is Ok with the correct appointment data
    var okResult = result as OkObjectResult;
    Assert.IsNotNull(okResult);
    Assert.That(okResult.StatusCode, Is.EqualTo(200)); // OK status code
    
    // Assert that the returned object is of type Appointment and its properties match the mock appointment
    var returnedAppointment = okResult.Value as Appointment;
    Assert.IsNotNull(returnedAppointment);
    Assert.That(returnedAppointment.AppointmentId, Is.EqualTo(appointment.AppointmentId));
    Assert.That(returnedAppointment.Title, Is.EqualTo(appointment.Title));
    Assert.That(returnedAppointment.Description, Is.EqualTo(appointment.Description));
    Assert.That(returnedAppointment.AppointmentDate, Is.EqualTo(appointment.AppointmentDate));
    Assert.That(returnedAppointment.CustomerName, Is.EqualTo(appointment.CustomerName));
    Assert.That(returnedAppointment.CustomerEmail, Is.EqualTo(appointment.CustomerEmail));
    Assert.That(returnedAppointment.Status, Is.EqualTo(appointment.Status));
    Assert.That(returnedAppointment.CreatedAt, Is.EqualTo(appointment.CreatedAt));
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
        Assert.That(message, Is.EqualTo("ProxyAppointmentController: Internal server error"));
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
        Assert.That(message, Is.EqualTo("Internal server error."));
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
    public void BuildQueryString_ReturnsCorrectQueryString_WhenAllParamsAreProvided()
    {
        // Arrange
        var searchArguments = new AppointmentSearchArguments
        {
            Title = "Haircut",
            CustomerName = "John Doe",
            CustomerEmail = "john.doe@example.com",
            Status = "Confirmed",
            StartDate = new DateTime(2025, 1, 20, 10, 0, 0),
            EndDate = new DateTime(2025, 1, 21, 10, 0, 0)
        };

        // Act
        var queryString = AppointmentSearchArguments.BuildQueryString(searchArguments);

        // Assert
        Assert.IsNotNull(queryString);
        Assert.IsTrue(queryString.Contains("Title=Haircut"));
        Assert.IsTrue(queryString.Contains("CustomerName=John%20Doe"));
        Assert.IsTrue(queryString.Contains("CustomerEmail=john.doe%40example.com"));
        Assert.IsTrue(queryString.Contains("Status=Confirmed"));
        Assert.IsTrue(queryString.Contains("StartDate=2025-01-20T10:00:00"));
        Assert.IsTrue(queryString.Contains("EndDate=2025-01-21T10:00:00"));
    }
    
    [Test]
    public async Task GetAllAppointments_ServiceUnavailable_Returns503()
    {
        // Arrange
        _mockServiceAppointmentsController
            .Setup(service => service.GetAllAppointmentsAsync(It.IsAny<AppointmentSearchArguments>()))
            .ThrowsAsync(new HttpRequestException("Service unavailable"));

        // Act
        var result = await _proxyAppointmentController.GetAllAppointments(new AppointmentSearchArguments());

        // Assert
        var statusCodeResult = result as ObjectResult;
        Assert.NotNull(statusCodeResult);
        Assert.AreEqual(503, statusCodeResult.StatusCode);  // Check that the status code is 503
        Assert.AreEqual("Service unavailable", ((dynamic)statusCodeResult.Value).GetType().GetProperty("Message")?.GetValue((dynamic)statusCodeResult.Value, null));
    }

    [Test]
    public async Task GetAppointmentById_ServiceUnavailable_Returns503()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        _mockServiceAppointmentsController
            .Setup(service => service.GetAppointmentByIdAsync(It.IsAny<Guid>()))
            .ThrowsAsync(new HttpRequestException("Service unavailable"));

        // Act
        var result = await _proxyAppointmentController.GetAppointmentById(appointmentId);

        // Assert
        var statusCodeResult = result as ObjectResult;
        Assert.NotNull(statusCodeResult);
        Assert.AreEqual(503, statusCodeResult.StatusCode);  // Check that the status code is 503
        Assert.AreEqual("ProxyAppointmentController: Service unavailable", ((dynamic)statusCodeResult.Value).GetType().GetProperty("Message")?.GetValue((dynamic)statusCodeResult.Value, null));
    }

    [Test]
    public async Task CreateAppointment_ServiceUnavailable_Returns503()
    {
        // Arrange
        var appointment = new Appointment
        {
            AppointmentId = Guid.NewGuid(),
            Title = "Doctor Appointment",
            CustomerName = "Jane Doe",
            CustomerEmail = "jane.doe@example.com",
            AppointmentDate = DateTime.UtcNow.AddDays(1),
            Status = "Upcoming"
        };

        _mockServiceAppointmentsController
            .Setup(service => service.CreateAppointmentAsync(It.IsAny<Appointment>()))
            .ThrowsAsync(new HttpRequestException("Service unavailable"));

        // Act
        var result = await _proxyAppointmentController.CreateAppointment(appointment);

        // Assert
        var statusCodeResult = result as ObjectResult;
        Assert.NotNull(statusCodeResult);
        Assert.AreEqual(503, statusCodeResult.StatusCode);  // Check that the status code is 503
        Assert.AreEqual("Service unavailable", ((dynamic)statusCodeResult.Value).GetType().GetProperty("Message")?.GetValue((dynamic)statusCodeResult.Value, null));
    }

    [Test]
    public async Task UpdateAppointment_ServiceUnavailable_Returns503()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var appointment = new Appointment
        {
            AppointmentId = appointmentId,
            Title = "Updated Appointment",
            CustomerName = "John Doe",
            CustomerEmail = "john.doe@example.com",
            AppointmentDate = DateTime.UtcNow.AddDays(1),
            Status = "Upcoming"
        };

        _mockServiceAppointmentsController
            .Setup(service => service.UpdateAppointmentAsync(It.IsAny<Guid>(), It.IsAny<Appointment>()))
            .ThrowsAsync(new HttpRequestException("Service unavailable"));

        // Act
        var result = await _proxyAppointmentController.UpdateAppointment(appointmentId, appointment);

        // Assert
        var statusCodeResult = result as ObjectResult;
        Assert.NotNull(statusCodeResult);
        Assert.AreEqual(503, statusCodeResult.StatusCode);  // Check that the status code is 503
        Assert.AreEqual("Service unavailable.", ((dynamic)statusCodeResult.Value).GetType().GetProperty("Message")?.GetValue((dynamic)statusCodeResult.Value, null));
    }

    [Test]
    public async Task DeleteAppointment_ServiceUnavailable_Returns503()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        _mockServiceAppointmentsController
            .Setup(service => service.DeleteAppointmentAsync(It.IsAny<Guid>()))
            .ThrowsAsync(new HttpRequestException("Service unavailable"));

        // Act
        var result = await _proxyAppointmentController.DeleteAppointment(appointmentId);

        // Assert
        var statusCodeResult = result as ObjectResult;
        Assert.NotNull(statusCodeResult);
        Assert.AreEqual(503, statusCodeResult.StatusCode);  // Check that the status code is 503
        Assert.AreEqual("Service unavailable.", ((dynamic)statusCodeResult.Value).GetType().GetProperty("Message")?.GetValue((dynamic)statusCodeResult.Value, null));
    }
    
    [Test]
    public async Task CreateAppointment_MissingRequiredFields_Returns400BadRequest()
    {
        // Arrange
        var appointment = new Appointment
        {
            AppointmentId = Guid.NewGuid(),
            Title = "", // Missing Title
            CustomerName = "",
            CustomerEmail = "",
            AppointmentDate = default // Missing Date
        };

        // Act
        var result = await _proxyAppointmentController.CreateAppointment(appointment);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        Assert.NotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);
        Assert.IsTrue(badRequestResult.Value.ToString().Contains("required fields"));
    }

    [Test]
    public async Task CreateAppointment_PastAppointmentDate_Returns400BadRequest()
    {
        // Arrange
        var appointment = new Appointment
        {
            AppointmentId = Guid.NewGuid(),
            Title = "Past Meeting",
            CustomerName = "Bob",
            CustomerEmail = "bob@example.com",
            AppointmentDate = DateTime.UtcNow.AddDays(-1), // Past date
            Status = "Upcoming"
        };

        // Act
        var result = await _proxyAppointmentController.CreateAppointment(appointment);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        Assert.NotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);
        Assert.IsTrue(badRequestResult.Value.ToString().Contains("Appointment date must be in the future."));
    }

    [Test]
    public async Task CreateAppointment_ConflictDetected_Returns409Conflict()
    {
        // Arrange
        var appointment = new Appointment
        {
            AppointmentId = Guid.NewGuid(),
            Title = "Haircut",
            CustomerName = "John Doe",
            CustomerEmail = "john@example.com",
            AppointmentDate = DateTime.UtcNow.AddDays(3),
            Status = "Upcoming"
        };

        _mockServiceAppointmentsController
            .Setup(service => service.CreateAppointmentAsync(It.IsAny<Appointment>()))
            .ReturnsAsync(new ConflictObjectResult("Appointment conflict detected"));

        // Act
        var result = await _proxyAppointmentController.CreateAppointment(appointment);

        // Assert
        var conflictResult = result as ConflictObjectResult;
        Assert.NotNull(conflictResult);
        Assert.AreEqual(409, conflictResult.StatusCode);
        Assert.IsTrue(conflictResult.Value.ToString().Contains("Appointment conflict detected"));
    }
    
    [Test]
    public async Task CreateAppointment_InternalError_Returns500()
    {
        // Arrange
        var appointment = new Appointment
        {
            AppointmentId = Guid.NewGuid(),
            Title = "Medical Checkup",
            CustomerName = "Dr. Emily",
            CustomerEmail = "emily@example.com",
            AppointmentDate = DateTime.UtcNow.AddDays(5),
            Status = "Upcoming"
        };

        _mockServiceAppointmentsController
            .Setup(service => service.CreateAppointmentAsync(It.IsAny<Appointment>()))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await _proxyAppointmentController.CreateAppointment(appointment);

        // Assert
        var internalErrorResult = result as ObjectResult;
        Assert.NotNull(internalErrorResult);
        Assert.AreEqual(500, internalErrorResult.StatusCode);
        Assert.IsTrue(internalErrorResult.Value.ToString().Contains("Internal server error"));
    }

    [Test]
    public async Task UpdateAppointment_Success_ReturnsOk()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var appointment = new Appointment
        {
            AppointmentId = appointmentId,
            Title = "Dentist Visit",
            CustomerName = "Alice Johnson",
            CustomerEmail = "alice@example.com",
            AppointmentDate = DateTime.UtcNow.AddDays(3),
            Status = "Upcoming"
        };

        _mockServiceAppointmentsController
            .Setup(service => service.UpdateAppointmentAsync(appointmentId, It.IsAny<Appointment>()))
            .ReturnsAsync(new OkResult());

        // Act
        var result = await _proxyAppointmentController.UpdateAppointment(appointmentId, appointment);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.NotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.IsTrue(okResult.Value.ToString().Contains("Appointment updated successfully"));
    }
    
    [Test]
    public async Task UpdateAppointment_NullAppointment_ReturnsBadRequest()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();

        // Act
        var result = await _proxyAppointmentController.UpdateAppointment(appointmentId, null);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        Assert.NotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);
        Assert.IsTrue(badRequestResult.Value.ToString().Contains("Appointment data is required"));
    }
    
    [Test]
    public async Task UpdateAppointment_EmptyAppointmentId_ReturnsBadRequest()
    {
        // Arrange
        var appointment = new Appointment
        {
            Title = "Meeting",
            CustomerName = "Bob",
            CustomerEmail = "bob@example.com",
            AppointmentDate = DateTime.UtcNow.AddDays(1),
            Status = "Upcoming"
        };

        // Act
        var result = await _proxyAppointmentController.UpdateAppointment(Guid.Empty, appointment);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        Assert.NotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);
        Assert.IsTrue(badRequestResult.Value.ToString().Contains("Appointment ID is empty"));
    }
    
    [Test]
    public async Task UpdateAppointment_InvalidStatus_ReturnsBadRequest()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var appointment = new Appointment
        {
            AppointmentId = appointmentId,
            Title = "Consultation",
            CustomerName = "Jane Doe",
            CustomerEmail = "jane@example.com",
            AppointmentDate = DateTime.UtcNow.AddDays(2),
            Status = "InvalidStatus"
        };

        // Act
        var result = await _proxyAppointmentController.UpdateAppointment(appointmentId, appointment);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        Assert.NotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);
        Assert.IsTrue(badRequestResult.Value.ToString().Contains("Status must be 'Cancelled', 'Upcoming', or 'Finished'"));
    }

    [Test]
    public async Task UpdateAppointment_DateInPast_ReturnsBadRequest()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var appointment = new Appointment
        {
            AppointmentId = appointmentId,
            Title = "Past Meeting",
            CustomerName = "John Smith",
            CustomerEmail = "john@example.com",
            AppointmentDate = DateTime.UtcNow.AddDays(-1), // Past date
            Status = "Upcoming"
        };

        // Act
        var result = await _proxyAppointmentController.UpdateAppointment(appointmentId, appointment);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        Assert.NotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);
        Assert.IsTrue(badRequestResult.Value.ToString().Contains("Appointment date must be in the future"));
    }
    
    [Test]
    public async Task UpdateAppointment_NotFound_ReturnsNotFound()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var appointment = new Appointment
        {
            AppointmentId = appointmentId,
            Title = "Vision Check",
            CustomerName = "Eve",
            CustomerEmail = "eve@example.com",
            AppointmentDate = DateTime.UtcNow.AddDays(7),
            Status = "Upcoming"
        };

        _mockServiceAppointmentsController
            .Setup(service => service.UpdateAppointmentAsync(appointmentId, It.IsAny<Appointment>()))
            .ReturnsAsync(new NotFoundResult());

        // Act
        var result = await _proxyAppointmentController.UpdateAppointment(appointmentId, appointment);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        Assert.NotNull(notFoundResult);
        Assert.AreEqual(404, notFoundResult.StatusCode);
        Assert.IsTrue(notFoundResult.Value.ToString().Contains("Appointment not found"));
    }
    
    [Test]
    public async Task UpdateAppointment_ServiceError_Returns500()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var appointment = new Appointment
        {
            AppointmentId = appointmentId,
            Title = "Surgery Follow-up",
            CustomerName = "Dr. Martin",
            CustomerEmail = "martin@example.com",
            AppointmentDate = DateTime.UtcNow.AddDays(10),
            Status = "Upcoming"
        };

        _mockServiceAppointmentsController
            .Setup(service => service.UpdateAppointmentAsync(It.IsAny<Guid>(), It.IsAny<Appointment>()))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await _proxyAppointmentController.UpdateAppointment(appointmentId, appointment);

        // Assert
        var internalErrorResult = result as ObjectResult;
        Assert.NotNull(internalErrorResult);
        Assert.AreEqual(500, internalErrorResult.StatusCode);
        Assert.IsTrue(internalErrorResult.Value.ToString().Contains("Internal server error"));
    }
    
    [Test]
    public async Task DeleteAppointment_Success_ReturnsOk()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();

        _mockServiceAppointmentsController
            .Setup(service => service.DeleteAppointmentAsync(appointmentId))
            .ReturnsAsync(new OkResult());

        // Act
        var result = await _proxyAppointmentController.DeleteAppointment(appointmentId);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.NotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.IsTrue(okResult.Value.ToString().Contains("Appointment deleted successfully"));
    }
    
    [Test]
    public async Task DeleteAppointment_EmptyAppointmentId_ReturnsBadRequest()
    {
        // Arrange
        var emptyAppointmentId = Guid.Empty;

        // Act
        var result = await _proxyAppointmentController.DeleteAppointment(emptyAppointmentId);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        Assert.NotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);
        Assert.IsTrue(badRequestResult.Value.ToString().Contains("A valid appointment ID is required"));
    }
    
    [Test]
    public async Task DeleteAppointment_NotFound_ReturnsNotFound()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();

        _mockServiceAppointmentsController
            .Setup(service => service.DeleteAppointmentAsync(appointmentId))
            .ReturnsAsync(new NotFoundResult());

        // Act
        var result = await _proxyAppointmentController.DeleteAppointment(appointmentId);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        Assert.NotNull(notFoundResult);
        Assert.AreEqual(404, notFoundResult.StatusCode);
        Assert.IsTrue(notFoundResult.Value.ToString().Contains("Appointment not found"));
    }
    
    [Test]
    public async Task DeleteAppointment_NoContent_ReturnsNoContent()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();

        _mockServiceAppointmentsController
            .Setup(service => service.DeleteAppointmentAsync(appointmentId))
            .ReturnsAsync(new NoContentResult());

        // Act
        var result = await _proxyAppointmentController.DeleteAppointment(appointmentId);

        // Assert
        var noContentResult = result as NoContentResult;
        Assert.NotNull(noContentResult);
        Assert.AreEqual(204, noContentResult.StatusCode);
    }
    
    [Test]
    public async Task DeleteAppointment_BadRequestFromService_ReturnsBadRequestWithDetails()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var badRequestResult = new BadRequestObjectResult(new { Error = "Invalid appointment ID" });

        _mockServiceAppointmentsController
            .Setup(service => service.DeleteAppointmentAsync(appointmentId))
            .ReturnsAsync(badRequestResult);

        // Act
        var result = await _proxyAppointmentController.DeleteAppointment(appointmentId);

        // Assert
        var badRequestObjectResult = result as BadRequestObjectResult;
        Assert.NotNull(badRequestObjectResult);
        Assert.AreEqual(400, badRequestObjectResult.StatusCode);
        Assert.IsTrue(badRequestObjectResult.Value.ToString().Contains("Invalid request parameters"));
        Assert.IsTrue(badRequestObjectResult.Value.ToString().Contains("Invalid appointment ID"));
    }
    
    [Test]
    public async Task DeleteAppointment_ServiceReturns500_ReturnsInternalServerErrorWithDetails()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var internalServerError = new ObjectResult(new { Error = "Database failure" }) { StatusCode = 500 };

        _mockServiceAppointmentsController
            .Setup(service => service.DeleteAppointmentAsync(appointmentId))
            .ReturnsAsync(internalServerError);

        // Act
        var result = await _proxyAppointmentController.DeleteAppointment(appointmentId);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.NotNull(objectResult);
        Assert.AreEqual(500, objectResult.StatusCode);
        Assert.IsTrue(objectResult.Value.ToString().Contains("Request failed"));
        Assert.IsTrue(objectResult.Value.ToString().Contains("Database failure"));
    }
    
    [Test]
    public async Task GetAppointmentById_ReturnsBadRequest_WhenAppointmentIdIsInvalid()
    {
        // Arrange: Prepare the invalid appointmentId
        var appointmentId = Guid.Empty;
        var errorMessage = "ProxyAppointmentController: Invalid appointment ID";

        // Act: Call the GetAppointmentById method
        var result = await _proxyAppointmentController.GetAppointmentById(appointmentId);

        // Assert: Verify that the response is BadRequest with the expected error message
        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));

        // Extract the message from the response for comparison
        var message = badRequestResult.Value.GetType().GetProperty("Message").GetValue(badRequestResult.Value, null);
        Assert.That(message, Is.EqualTo(errorMessage));
    }

    [Test]
    public async Task GetAppointmentById_ReturnsNotFound_WhenAppointmentDoesNotExist()
    {
        // Arrange: Prepare the appointmentId to fetch
        var appointmentId = Guid.NewGuid();
        var errorMessage = "ProxyAppointmentController: Appointment not found";
        var errorDetails = $"No appointment found with ID: {appointmentId}";

        // Mock the ServiceAppointmentsController to return NotFoundResult
        _mockServiceAppointmentsController.Setup(service => service.GetAppointmentByIdAsync(appointmentId))
            .ReturnsAsync(new NotFoundResult());

        // Act: Call the GetAppointmentById method
        var result = await _proxyAppointmentController.GetAppointmentById(appointmentId);

        // Assert: Verify that the response is NotFound with the expected error message
        var notFoundResult = result as NotFoundObjectResult;
        Assert.IsNotNull(notFoundResult);
        Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));

        // Extract the message and details from the response for comparison
        var message = notFoundResult.Value.GetType().GetProperty("Message").GetValue(notFoundResult.Value, null);
        var details = notFoundResult.Value.GetType().GetProperty("Details").GetValue(notFoundResult.Value, null);
        Assert.That(message, Is.EqualTo(errorMessage));
        Assert.That(details, Is.EqualTo(errorDetails));
    }

    [Test]
    public async Task GetAppointmentById_ReturnsServiceUnavailable_WhenHttpRequestExceptionOccurs()
    {
        // Arrange: Prepare the appointmentId to fetch
        var appointmentId = Guid.NewGuid();
        var errorMessage = "ProxyAppointmentController: Service unavailable";
        var errorDetails = "Service unavailable";

        // Mock the ServiceAppointmentsController to throw an HttpRequestException
        _mockServiceAppointmentsController.Setup(service => service.GetAppointmentByIdAsync(appointmentId))
            .ThrowsAsync(new HttpRequestException("Service unavailable"));

        // Act: Call the GetAppointmentById method
        var result = await _proxyAppointmentController.GetAppointmentById(appointmentId);

        // Assert: Verify that the response is Service Unavailable with the expected error message
        var statusCodeResult = result as ObjectResult;
        Assert.IsNotNull(statusCodeResult);
        Assert.That(statusCodeResult.StatusCode, Is.EqualTo(503));

        // Extract the message and details from the response for comparison
        var message = statusCodeResult.Value.GetType().GetProperty("Message").GetValue(statusCodeResult.Value, null);
        var details = statusCodeResult.Value.GetType().GetProperty("Details").GetValue(statusCodeResult.Value, null);
        Assert.That(message, Is.EqualTo(errorMessage));
        Assert.That(details, Is.EqualTo(errorDetails));
    }

    [Test]
    public async Task GetAppointmentById_ReturnsInternalServerError_WhenUnexpectedErrorOccurs()
    {
        // Arrange: Prepare the appointmentId to fetch
        var appointmentId = Guid.NewGuid();
        var errorMessage = "ProxyAppointmentController: Internal server error";
        var errorDetails = "Unknown error";

        // Mock the ServiceAppointmentsController to throw an exception
        _mockServiceAppointmentsController.Setup(service => service.GetAppointmentByIdAsync(appointmentId))
            .ThrowsAsync(new Exception("Unknown error"));

        // Act: Call the GetAppointmentById method
        var result = await _proxyAppointmentController.GetAppointmentById(appointmentId);

        // Assert: Verify that the response is Internal Server Error with the expected error message
        var statusCodeResult = result as ObjectResult;
        Assert.IsNotNull(statusCodeResult);
        Assert.That(statusCodeResult.StatusCode, Is.EqualTo(500));

        // Extract the message and details from the response for comparison
        var message = statusCodeResult.Value.GetType().GetProperty("Message").GetValue(statusCodeResult.Value, null);
        var details = statusCodeResult.Value.GetType().GetProperty("Details").GetValue(statusCodeResult.Value, null);
        Assert.That(message, Is.EqualTo(errorMessage));
        Assert.That(details, Is.EqualTo(errorDetails));
    }

    [Test]
    public async Task GetAppointmentById_ReturnsBadRequest_WhenServiceReturnsBadRequest()
    {
        // Arrange: Prepare the appointmentId to fetch
        var appointmentId = Guid.NewGuid();
        var errorMessage = "ProxyAppointmentController: Invalid request parameters";
        var errorDetails = "Invalid data";

        // Mock the ServiceAppointmentsController to return BadRequestObjectResult
        _mockServiceAppointmentsController.Setup(service => service.GetAppointmentByIdAsync(appointmentId))
            .ReturnsAsync(new BadRequestObjectResult("Invalid data"));

        // Act: Call the GetAppointmentById method
        var result = await _proxyAppointmentController.GetAppointmentById(appointmentId);

        // Assert: Verify that the response is BadRequest with the expected error message
        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));

        // Extract the message and details from the response for comparison
        var message = badRequestResult.Value.GetType().GetProperty("Message").GetValue(badRequestResult.Value, null);
        var details = badRequestResult.Value.GetType().GetProperty("Details").GetValue(badRequestResult.Value, null);
        Assert.That(message, Is.EqualTo(errorMessage));
        Assert.That(details, Is.EqualTo(errorDetails));
    }

    [Test]
    public async Task GetAllAppointments_ReturnsNotFound_WhenNoAppointmentsFound()
    {
        // Arrange: Prepare the search arguments
        var searchArguments = new AppointmentSearchArguments { /* Add relevant properties */ };
        var errorMessage = "No appointments found matching the criteria";

        // Mock the ServiceAppointmentsController to return NotFoundResult
        _mockServiceAppointmentsController.Setup(service => service.GetAllAppointmentsAsync(searchArguments))
            .ReturnsAsync(new NotFoundResult());

        // Act: Call the GetAllAppointments method
        var result = await _proxyAppointmentController.GetAllAppointments(searchArguments);

        // Assert: Verify that the response is NotFound with the expected error message
        var notFoundResult = result as NotFoundObjectResult;
        Assert.IsNotNull(notFoundResult);
        Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));

        // Extract the message from the response
        var message = notFoundResult.Value.GetType().GetProperty("Message").GetValue(notFoundResult.Value, null);
        Assert.That(message, Is.EqualTo(errorMessage));
    }

    [Test]
    public async Task GetAllAppointments_ReturnsCorrectError_WhenServiceReturnsErrorStatusCode()
    {
        // Arrange: Prepare the search arguments
        var searchArguments = new AppointmentSearchArguments { /* Add relevant properties */ };
        var errorMessage = "Request failed";
        var errorDetails = "Some error details";
        var statusCode = 500;

        // Mock the ServiceAppointmentsController to return an error status code
        _mockServiceAppointmentsController.Setup(service => service.GetAllAppointmentsAsync(searchArguments))
            .ReturnsAsync(new ObjectResult(errorDetails) { StatusCode = statusCode });

        // Act: Call the GetAllAppointments method
        var result = await _proxyAppointmentController.GetAllAppointments(searchArguments);

        // Assert: Verify that the response has the correct status code and error message
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.That(objectResult.StatusCode, Is.EqualTo(statusCode));

        // Extract the message and details from the response
        var message = objectResult.Value.GetType().GetProperty("Message").GetValue(objectResult.Value, null);
        var details = objectResult.Value.GetType().GetProperty("Details").GetValue(objectResult.Value, null);
        Assert.That(message, Is.EqualTo(errorMessage));
        Assert.That(details, Is.EqualTo(errorDetails));
    }

}
