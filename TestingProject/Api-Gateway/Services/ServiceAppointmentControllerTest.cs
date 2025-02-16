using Moq;
using System.Net;
using System.Text;
using Api_Gateway.Services;
using Api_Gateway.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
            Content = new StringContent(
                "[{\"AppointmentId\":\"a3a976b7-ec4b-4ae7-beb6-c92da3b0478c\", \"Title\":\"Test Appointment\", \"Description\":\"Test Description\", \"AppointmentDate\":\"2025-01-20T10:00:00\", \"CustomerName\":\"John Doe\", \"CustomerEmail\":\"john.doe@example.com\", \"Status\":\"Upcoming\", \"CreatedAt\":\"2025-01-19T12:00:00\"}]",
                Encoding.UTF8,
                "application/json"
            )
        };

        _mockHttpClientFactory
            .Setup(client => client.CreateClient(It.IsAny<string>()))
            .Returns(new HttpClient(new FakeHttpMessageHandler(appointmentsResponse)));

        // Act
        var result = await _controller.GetAllAppointmentsAsync();

        // Assert
        // Cast the result to OkObjectResult
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);

        // Extract the list of appointments
        var appointments = okResult.Value as List<Appointment>;
        Assert.IsNotNull(appointments);

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

        // Use reflection to access the "Message" property of the anonymous object
        var responseContent = objectResult.Value;
        Assert.IsNotNull(responseContent);

        var messageProperty = responseContent.GetType().GetProperty("Message");
        Assert.IsNotNull(messageProperty);

        var messageValue = messageProperty.GetValue(responseContent) as string;
        Assert.AreEqual("Client Error: Bad Request", messageValue);
    }
    
    [Test]
    public async Task GetAllAppointmentsAsync_InternalServerError_ReturnsStatusCode()
    {
        // Arrange
        var errorResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.InternalServerError,
            Content = new StringContent("Internal Server Error", Encoding.UTF8, "application/json")
        };

        _mockHttpClientFactory
            .Setup(client => client.CreateClient(It.IsAny<string>()))
            .Returns(new HttpClient(new FakeHttpMessageHandler(errorResponse)));

        // Act
        var result = await _controller.GetAllAppointmentsAsync();

        // Assert
        var statusCodeResult = result as StatusCodeResult;
        Assert.IsNotNull(statusCodeResult);
        Assert.AreEqual(500, statusCodeResult.StatusCode); // Ensure it's an Internal Server Error
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

        // Use reflection to access the properties of the anonymous object
        var responseContent = objectResult.Value;
        Assert.IsNotNull(responseContent);

        var messageProperty = responseContent.GetType().GetProperty("Message");
        var detailsProperty = responseContent.GetType().GetProperty("Details");

        Assert.IsNotNull(messageProperty);
        Assert.IsNotNull(detailsProperty);

        var messageValue = messageProperty.GetValue(responseContent) as string;
        var detailsValue = detailsProperty.GetValue(responseContent) as string;

        Assert.AreEqual("Internal Server Error", messageValue);
        Assert.AreEqual("Network error", detailsValue);
    }

    [Test]
    public async Task GetAppointmentByIdAsync_NetworkError_ReturnsServiceUnavailable()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
    
        // Simulating a network error by making the HTTP client throw an exception
        _mockHttpClientFactory
            .Setup(client => client.CreateClient(It.IsAny<string>()))
            .Throws(new HttpRequestException("Network error"));

        // Act: Call the method under test
        var actionResult = await _controller.GetAppointmentByIdAsync(appointmentId);

        // Assert: Ensure result is a StatusCodeResult (503 Service Unavailable)
        var statusCodeResult = actionResult as StatusCodeResult;
        Assert.IsNotNull(statusCodeResult, "Expected StatusCodeResult when a network error occurs.");
        Assert.AreEqual(503, statusCodeResult.StatusCode, "Expected status code 503 (Service Unavailable)");
    }

    [Test]
    public async Task GetAppointmentByIdAsync_InvalidAppointmentId_ReturnsBadRequest()
    {
        // Arrange
        var appointmentId = Guid.Empty; // Invalid appointment ID

        // Act
        var actionResult = await _controller.GetAppointmentByIdAsync(appointmentId);

        // Assert
        var badRequestResult = actionResult as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult, "Expected BadRequestObjectResult when an invalid appointment ID is provided.");
        Assert.AreEqual(400, badRequestResult.StatusCode, "Expected status code 400 (Bad Request)");

        // Validate the message and details returned in the response
        var response = badRequestResult.Value;
        Assert.IsNotNull(response, "Expected a response object in BadRequest result.");
        var messageProperty = response.GetType().GetProperty("Message")?.GetValue(response, null);
        Assert.AreEqual("ServiceAppointmentController: Invalid appointment ID", messageProperty);
    }
    
    [Test]
    public async Task GetAppointmentByIdAsync_AppointmentNotFound_ReturnsNotFound()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var notFoundResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NotFound,
            Content = new StringContent("Not Found")
        };

        // Mock the HTTP client factory to return the notFoundResponse
        _mockHttpClientFactory.Setup(client => client.CreateClient(It.IsAny<string>()))
            .Returns(new HttpClient(new FakeHttpMessageHandler(notFoundResponse)));

        // Act
        var actionResult = await _controller.GetAppointmentByIdAsync(appointmentId);

        // Assert
        Assert.IsNotNull(actionResult);

        // Check that the result is a NotFoundObjectResult
        if (actionResult is NotFoundObjectResult notFoundResult)
        {
            // Expected status code 404 for NotFoundObjectResult
            Assert.AreEqual(404, notFoundResult.StatusCode, "Expected status code 404 (Not Found)");

            // Extract the "Message" and "Details" properties
            var response = notFoundResult.Value;
            Assert.IsNotNull(response, "Expected a response object in NotFound result.");

            var messageProperty = response.GetType().GetProperty("Message")?.GetValue(response, null);
            var detailsProperty = response.GetType().GetProperty("Details")?.GetValue(response, null);

            // Check the values returned in the response
            Assert.AreEqual("ServiceAppointmentController: Appointment Not Found", messageProperty);
            Assert.AreEqual($"No appointment found with the ID: {appointmentId}", detailsProperty);
        }
        else
        {
            Assert.Fail("Expected NotFoundObjectResult but received a different result type.");
        }
    }


    [Test]
    public async Task GetAppointmentByIdAsync_ClientError_ReturnsBadRequest()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var clientErrorResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.BadRequest,
            ReasonPhrase = "Bad Request",
            Content = new StringContent("{ \"Message\": \"ServiceAppointmentController: Client Error\", \"Details\": \"Bad Request\" }")
        };

        _mockHttpClientFactory.Setup(client => client.CreateClient(It.IsAny<string>()))
            .Returns(new HttpClient(new FakeHttpMessageHandler(clientErrorResponse)));

        // Act
        var actionResult = await _controller.GetAppointmentByIdAsync(appointmentId);

        // Assert
        Assert.IsNotNull(actionResult);

        // Ensure we got a BadRequestObjectResult
        var badRequestResult = actionResult as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult, "Expected BadRequestObjectResult when an invalid status is provided.");
        Assert.AreEqual(400, badRequestResult.StatusCode);

        // Extract the response value from BadRequestObjectResult
        var response = badRequestResult.Value;
        Assert.IsNotNull(response, "Expected a response object in BadRequest result.");

        // Use reflection to access properties of the anonymous type
        var messageProperty = response.GetType().GetProperty("Message")?.GetValue(response, null);
        var detailsProperty = response.GetType().GetProperty("Details")?.GetValue(response, null);

        // Assert the values
        Assert.AreEqual("ServiceAppointmentController: Client Error", messageProperty);
        Assert.AreEqual("Bad Request", detailsProperty);
    }
    
    [Test]
    public async Task GetAppointmentByIdAsync_InternalServerError_Returns500()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();

        _mockHttpClientFactory.Setup(client => client.CreateClient(It.IsAny<string>()))
            .Throws(new Exception("Unexpected Error"));

        // Act
        var actionResult = await _controller.GetAppointmentByIdAsync(appointmentId);

        // Assert
        Assert.IsNotNull(actionResult);

        // Ensure we got an ObjectResult (which is expected for status code 500)
        var objectResult = actionResult as ObjectResult;
        Assert.IsNotNull(objectResult, "Expected ObjectResult when an exception is thrown.");
        Assert.AreEqual(500, objectResult.StatusCode, "Expected status code 500 (Internal Server Error)");

        // Extract the response value from ObjectResult
        var response = objectResult.Value;
        Assert.IsNotNull(response, "Expected a response object in ObjectResult.");

        // Use reflection to access properties of the anonymous type
        var messageProperty = response.GetType().GetProperty("Message")?.GetValue(response, null);
        var detailsProperty = response.GetType().GetProperty("Details")?.GetValue(response, null);

        // Assert the values
        Assert.AreEqual("Internal Server Error", messageProperty);
        Assert.AreEqual("Unexpected Error", detailsProperty);
    }

    
    [Test]
    public async Task GetAppointmentByIdAsync_Success_ReturnsOkWithAppointmentData()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var expectedAppointment = new Appointment
        {
            AppointmentId = appointmentId,
            AppointmentDate = DateTime.Now,
            Description = "Test Appointment"
        };

        var successResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonConvert.SerializeObject(expectedAppointment))
        };

        _mockHttpClientFactory.Setup(client => client.CreateClient(It.IsAny<string>()))
            .Returns(new HttpClient(new FakeHttpMessageHandler(successResponse)));

        // Act
        var actionResult = await _controller.GetAppointmentByIdAsync(appointmentId);

        // Assert
        Assert.IsNotNull(actionResult);

        if (actionResult is OkObjectResult okResult)
        {
            var appointment = okResult.Value as Appointment;
            Assert.IsNotNull(appointment);
            Assert.AreEqual(expectedAppointment.AppointmentId, appointment.AppointmentId);
            Assert.AreEqual(expectedAppointment.Description, appointment.Description);
        }
        else
        {
            Assert.Fail("Expected OkObjectResult but received a different result type.");
        }
    }
    
    [Test]
    public async Task GetAppointmentByIdAsync_UnhandledException_ReturnsInternalServerError()
    {
        // Arrange: Simulate an unhandled exception (e.g., null reference or any other error)
        var appointmentId = Guid.NewGuid(); // Create a random appointment ID

        // Mocking a generic exception throw
        _mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Throws(new Exception("Unhandled exception"));

        // Act: Call the method under test
        var result = await _controller.GetAppointmentByIdAsync(appointmentId);

        // Assert: Ensure result is Internal Server Error (500)
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult, "Expected ObjectResult for internal server error.");
        Assert.AreEqual(500, objectResult.StatusCode);

        // Extract the "Message" property from the response (anonymous object)
        var response = objectResult.Value;
        Assert.IsNotNull(response, "Expected a response object in Internal Server Error result.");

        // Use reflection to get the "Message" property value
        var messageProperty = response.GetType().GetProperty("Message")?.GetValue(response, null);
        Assert.IsNotNull(messageProperty, "Expected 'Message' property in response.");
        Assert.AreEqual("Internal Server Error", messageProperty);

        // Optionally, verify the "Details" property contains information about the exception
        var detailsProperty = response.GetType().GetProperty("Details")?.GetValue(response, null);
        Assert.IsNotNull(detailsProperty, "Expected 'Details' property in response.");
        StringAssert.Contains("Unhandled exception", detailsProperty.ToString());
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
    
    [Test]
    public async Task CreateAppointmentAsync_InvalidAppointment_ReturnsBadRequest()
    {
        // Arrange
        Appointment invalidAppointment = null;

        // Act
        var actionResult = await _controller.CreateAppointmentAsync(invalidAppointment);

        // Assert
        Assert.IsNotNull(actionResult);
    
        if (actionResult is BadRequestObjectResult badRequestResult)
        {
            Assert.AreEqual(400, badRequestResult.StatusCode, "Expected status code 400 (Bad Request)");

            var response = badRequestResult.Value;
            Assert.IsNotNull(response);
            Assert.AreEqual("ServiceAppointmentController: Invalid request", response.GetType().GetProperty("Message")?.GetValue(response, null));
            Assert.AreEqual("Appointment data is required.", response.GetType().GetProperty("Details")?.GetValue(response, null));
        }
        else
        {
            Assert.Fail("Expected BadRequestObjectResult but received a different result type.");
        }
    }

    [Test]
    public async Task CreateAppointmentAsync_InvalidStatus_ReturnsBadRequest()
    {
        // Arrange
        var invalidAppointment = new Appointment
        {
            AppointmentId = Guid.NewGuid(),
            Status = "InvalidStatus"
        };

        // Act
        var actionResult = await _controller.CreateAppointmentAsync(invalidAppointment);

        // Assert
        Assert.IsNotNull(actionResult);

        if (actionResult is BadRequestObjectResult badRequestResult)
        {
            Assert.AreEqual(400, badRequestResult.StatusCode, "Expected status code 400 (Bad Request)");

            var response = badRequestResult.Value;
            Assert.IsNotNull(response);
            Assert.AreEqual("ServiceAppointmentController: Invalid status", response.GetType().GetProperty("Message")?.GetValue(response, null));
            Assert.AreEqual("Status must be 'Cancelled', 'Upcoming', or 'Finished'.", response.GetType().GetProperty("Details")?.GetValue(response, null));
        }
        else
        {
            Assert.Fail("Expected BadRequestObjectResult but received a different result type.");
        }
    }

    [Test]
    public async Task CreateAppointmentAsync_SuccessfulCreation_ReturnsCreatedResult()
    {
        // Arrange
        var newAppointment = new Appointment
        {
            AppointmentId = Guid.NewGuid(),
            Status = "Upcoming"
        };

        var successResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.Created,
            Content = new StringContent(JsonConvert.SerializeObject(newAppointment))
        };

        _mockHttpClientFactory.Setup(client => client.CreateClient(It.IsAny<string>()))
            .Returns(new HttpClient(new FakeHttpMessageHandler(successResponse)));

        // Act
        var actionResult = await _controller.CreateAppointmentAsync(newAppointment);

        // Assert
        Assert.IsNotNull(actionResult);

        if (actionResult is CreatedResult createdResult)
        {
            Assert.AreEqual(201, createdResult.StatusCode, "Expected status code 201 (Created)");

            var createdAppointment = createdResult.Value as Appointment;
            Assert.IsNotNull(createdAppointment);
            Assert.AreEqual(newAppointment.AppointmentId, createdAppointment.AppointmentId);
        }
        else
        {
            Assert.Fail("Expected CreatedResult but received a different result type.");
        }
    }

    [Test]
    public async Task CreateAppointmentAsync_ClientError_ReturnsBadRequest()
    {
        // Arrange
        var newAppointment = new Appointment
        {
            AppointmentId = Guid.NewGuid(),
            Status = "Upcoming"
        };

        var clientErrorResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.BadRequest,
            ReasonPhrase = "Bad Request"
        };

        _mockHttpClientFactory.Setup(client => client.CreateClient(It.IsAny<string>()))
            .Returns(new HttpClient(new FakeHttpMessageHandler(clientErrorResponse)));

        // Act
        var actionResult = await _controller.CreateAppointmentAsync(newAppointment);

        // Assert
        Assert.IsNotNull(actionResult);

        if (actionResult is BadRequestObjectResult badRequestResult)
        {
            Assert.AreEqual(400, badRequestResult.StatusCode, "Expected status code 400 (Bad Request)");

            var response = badRequestResult.Value;
            Assert.IsNotNull(response);
            Assert.AreEqual("ServiceAppointmentController: Client Error", response.GetType().GetProperty("Message")?.GetValue(response, null));
            Assert.AreEqual("Bad Request", response.GetType().GetProperty("Details")?.GetValue(response, null));
        }
        else
        {
            Assert.Fail("Expected BadRequestObjectResult but received a different result type.");
        }
    }

    [Test]
    public async Task CreateAppointmentAsync_InternalServerError_Returns500()
    {
        // Arrange
        var newAppointment = new Appointment
        {
            AppointmentId = Guid.NewGuid(),
            Status = "Upcoming"
        };

        _mockHttpClientFactory.Setup(client => client.CreateClient(It.IsAny<string>()))
            .Throws(new Exception("Unexpected Error"));

        // Act
        var actionResult = await _controller.CreateAppointmentAsync(newAppointment);

        // Assert
        Assert.IsNotNull(actionResult);

        if (actionResult is ObjectResult objectResult)
        {
            Assert.AreEqual(500, objectResult.StatusCode, "Expected status code 500 (Internal Server Error)");

            var errorResponse = objectResult.Value as dynamic;
            Assert.IsNotNull(errorResponse);
            Assert.AreEqual("ServiceAppointmentController: Internal Server Error", errorResponse.GetType().GetProperty("Message")?.GetValue(errorResponse, null));
            Assert.AreEqual("Unexpected Error", errorResponse.GetType().GetProperty("Details")?.GetValue(errorResponse, null));
        }
        else
        {
            Assert.Fail("Expected ObjectResult (500) but received a different result type.");
        }
    }
    
    [Test]
    public async Task UpdateAppointmentAsync_InvalidAppointment_ReturnsBadRequest()
    {
        // Arrange
        Appointment invalidAppointment = null;
        var appointmentId = Guid.NewGuid();

        // Act
        var actionResult = await _controller.UpdateAppointmentAsync(appointmentId, invalidAppointment);

        // Assert
        Assert.IsNotNull(actionResult);

        if (actionResult is BadRequestObjectResult badRequestResult)
        {
            Assert.AreEqual(400, badRequestResult.StatusCode, "Expected status code 400 (Bad Request)");

            var response = badRequestResult.Value;
            Assert.IsNotNull(response);
            Assert.AreEqual("Invalid request", response.GetType().GetProperty("Message")?.GetValue(response, null));
            Assert.AreEqual("Appointment data is required.", response.GetType().GetProperty("Details")?.GetValue(response, null));
        }
        else
        {
            Assert.Fail("Expected BadRequestObjectResult but received a different result type.");
        }
    }

    [Test]
    public async Task UpdateAppointmentAsync_InvalidAppointmentId_ReturnsBadRequest()
    {
        // Arrange
        var validAppointment = new Appointment
        {
            AppointmentId = Guid.NewGuid(),
            Status = "Upcoming"
        };
        var invalidAppointmentId = Guid.Empty;

        // Act
        var actionResult = await _controller.UpdateAppointmentAsync(invalidAppointmentId, validAppointment);

        // Assert
        Assert.IsNotNull(actionResult);

        if (actionResult is BadRequestObjectResult badRequestResult)
        {
            Assert.AreEqual(400, badRequestResult.StatusCode, "Expected status code 400 (Bad Request)");

            var response = badRequestResult.Value;
            Assert.IsNotNull(response);
            Assert.AreEqual("Invalid appointment ID", response.GetType().GetProperty("Message")?.GetValue(response, null));
            Assert.AreEqual("The provided appointment ID is empty or invalid.", response.GetType().GetProperty("Details")?.GetValue(response, null));
        }
        else
        {
            Assert.Fail("Expected BadRequestObjectResult but received a different result type.");
        }
    }

    [Test]
    public async Task UpdateAppointmentAsync_InvalidStatus_ReturnsBadRequest()
    {
        // Arrange
        var invalidAppointment = new Appointment
        {
            AppointmentId = Guid.NewGuid(),
            Status = "InvalidStatus"
        };
        var appointmentId = Guid.NewGuid();

        // Act
        var actionResult = await _controller.UpdateAppointmentAsync(appointmentId, invalidAppointment);

        // Assert
        Assert.IsNotNull(actionResult);

        if (actionResult is BadRequestObjectResult badRequestResult)
        {
            Assert.AreEqual(400, badRequestResult.StatusCode, "Expected status code 400 (Bad Request)");

            var response = badRequestResult.Value;
            Assert.IsNotNull(response);
            Assert.AreEqual("Invalid status", response.GetType().GetProperty("Message")?.GetValue(response, null));
            Assert.AreEqual("Status must be 'Cancelled', 'Upcoming', or 'Finished'.", response.GetType().GetProperty("Details")?.GetValue(response, null));
        }
        else
        {
            Assert.Fail("Expected BadRequestObjectResult but received a different result type.");
        }
    }
    
    [Test]
    public async Task UpdateAppointmentAsync_SuccessfulUpdate_ReturnsOkResult()
    {
        // Arrange
        var updatedAppointment = new Appointment
        {
            AppointmentId = Guid.NewGuid(),
            Status = "Upcoming"
        };

        var successResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonConvert.SerializeObject(new 
            {
                Message = "Appointment updated successfully",
                Details = updatedAppointment
            }))
        };

        _mockHttpClientFactory.Setup(client => client.CreateClient(It.IsAny<string>()))
            .Returns(new HttpClient(new FakeHttpMessageHandler(successResponse)));

        // Act
        var actionResult = await _controller.UpdateAppointmentAsync(updatedAppointment.AppointmentId, updatedAppointment);

        // Assert
        Assert.IsNotNull(actionResult);

        if (actionResult is OkObjectResult okResult)
        {
            Assert.AreEqual(200, okResult.StatusCode, "Expected status code 200 (OK)");

            var resultData = okResult.Value as dynamic;
            Assert.IsNotNull(resultData);

            // Ensure correct structure and values
            Assert.AreEqual("Appointment updated successfully", resultData.GetType().GetProperty("Message")?.GetValue(resultData, null));
            Assert.AreEqual(updatedAppointment.AppointmentId, resultData.GetType().GetProperty("Details")?.GetValue(resultData, null));
        }
        else
        {
            Assert.Fail("Expected OkObjectResult but received a different result type.");
        }
    }
    
    [Test]
    public async Task UpdateAppointmentAsync_ClientError_ReturnsBadRequest()
    {
        // Arrange
        var updatedAppointment = new Appointment
        {
            AppointmentId = Guid.NewGuid(),
            Status = "Upcoming"
        };

        var clientErrorResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.BadRequest,
            ReasonPhrase = "Bad Request"
        };

        _mockHttpClientFactory.Setup(client => client.CreateClient(It.IsAny<string>()))
            .Returns(new HttpClient(new FakeHttpMessageHandler(clientErrorResponse)));

        // Act
        var actionResult = await _controller.UpdateAppointmentAsync(updatedAppointment.AppointmentId, updatedAppointment);

        // Assert
        Assert.IsNotNull(actionResult);

        if (actionResult is BadRequestObjectResult badRequestResult)
        {
            Assert.AreEqual(400, badRequestResult.StatusCode, "Expected status code 400 (Bad Request)");

            var response = badRequestResult.Value;
            Assert.IsNotNull(response);
            Assert.AreEqual("Client Error", response.GetType().GetProperty("Message")?.GetValue(response, null));
            Assert.AreEqual("Bad Request", response.GetType().GetProperty("Details")?.GetValue(response, null));
        }
        else
        {
            Assert.Fail("Expected BadRequestObjectResult but received a different result type.");
        }
    }

    [Test]
    public async Task UpdateAppointmentAsync_InternalServerError_Returns500()
    {
        // Arrange
        var updatedAppointment = new Appointment
        {
            AppointmentId = Guid.NewGuid(),
            Status = "Upcoming"
        };

        _mockHttpClientFactory.Setup(client => client.CreateClient(It.IsAny<string>()))
            .Throws(new Exception("Unexpected Error"));

        // Act
        var actionResult = await _controller.UpdateAppointmentAsync(updatedAppointment.AppointmentId, updatedAppointment);

        // Assert
        Assert.IsNotNull(actionResult);

        if (actionResult is ObjectResult objectResult)
        {
            Assert.AreEqual(500, objectResult.StatusCode, "Expected status code 500 (Internal Server Error)");

            var errorResponse = objectResult.Value as dynamic;
            Assert.IsNotNull(errorResponse);
            Assert.AreEqual("Internal Server Error", errorResponse.GetType().GetProperty("Message")?.GetValue(errorResponse, null));
            Assert.AreEqual("Unexpected Error", errorResponse.GetType().GetProperty("Details")?.GetValue(errorResponse, null));
        }
        else
        {
            Assert.Fail("Expected ObjectResult (500) but received a different result type.");
        }
    }
    
    [Test]
    public async Task DeleteAppointmentAsync_InvalidAppointmentId_ReturnsBadRequest()
    {
        // Arrange
        var invalidAppointmentId = Guid.Empty;

        // Act
        var actionResult = await _controller.DeleteAppointmentAsync(invalidAppointmentId);

        // Assert
        Assert.IsNotNull(actionResult);

        if (actionResult is BadRequestObjectResult badRequestResult)
        {
            Assert.AreEqual(400, badRequestResult.StatusCode, "Expected status code 400 (Bad Request)");

            var response = badRequestResult.Value;
            Assert.IsNotNull(response);
            Assert.AreEqual("Invalid appointment ID", response.GetType().GetProperty("Message")?.GetValue(response, null));
            Assert.AreEqual("The provided appointment ID is empty or invalid.", response.GetType().GetProperty("Details")?.GetValue(response, null));
        }
        else
        {
            Assert.Fail("Expected BadRequestObjectResult but received a different result type.");
        }
    }
    
    [Test]
    public async Task DeleteAppointmentAsync_ClientError_ReturnsBadRequest()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();

        var clientErrorResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.BadRequest,
            ReasonPhrase = "Bad Request"
        };

        _mockHttpClientFactory.Setup(client => client.CreateClient(It.IsAny<string>()))
            .Returns(new HttpClient(new FakeHttpMessageHandler(clientErrorResponse)));

        // Act
        var actionResult = await _controller.DeleteAppointmentAsync(appointmentId);

        // Assert
        Assert.IsNotNull(actionResult);

        if (actionResult is BadRequestObjectResult badRequestResult)
        {
            Assert.AreEqual(400, badRequestResult.StatusCode, "Expected status code 400 (Bad Request)");

            var response = badRequestResult.Value;
            Assert.IsNotNull(response);
            Assert.AreEqual("Client Error", response.GetType().GetProperty("Message")?.GetValue(response, null));
            Assert.AreEqual("Bad Request", response.GetType().GetProperty("Details")?.GetValue(response, null));
        }
        else
        {
            Assert.Fail("Expected BadRequestObjectResult but received a different result type.");
        }
    }

    [Test]
    public async Task DeleteAppointmentAsync_ServiceUnavailable_Returns503()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();

        _mockHttpClientFactory.Setup(client => client.CreateClient(It.IsAny<string>()))
            .Throws(new HttpRequestException("Service Unavailable"));

        // Act
        var actionResult = await _controller.DeleteAppointmentAsync(appointmentId);

        // Assert
        Assert.IsNotNull(actionResult);

        if (actionResult is StatusCodeResult statusCodeResult)
        {
            Assert.AreEqual(503, statusCodeResult.StatusCode, "Expected status code 503 (Service Unavailable)");
        }
        else
        {
            Assert.Fail("Expected StatusCodeResult (503) but received a different result type.");
        }
    }


} 

}
