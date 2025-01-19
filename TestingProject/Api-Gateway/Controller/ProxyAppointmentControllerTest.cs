using Api_Gateway.Controller;
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
}
