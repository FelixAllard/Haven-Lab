using AppointmentsService.Controllers;
using AppointmentsService.Data;
using AppointmentsService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace TestingProject.AppointmentsService.Controllers
{
    [TestFixture]
    public class AppointmentControllerTest
    {
        private AppointmentDbContext _context;
        private AppointmentsController _controller;

        [SetUp]
        public void Setup()
        {
            // Set up a unique in-memory database for each test method
            var options = new DbContextOptionsBuilder<AppointmentDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())  // Unique database name
                .Options;

            // Create the context using the in-memory database
            _context = new AppointmentDbContext(options);

            // Add mock data to the in-memory database
            var mockAppointments = new List<Appointment>
            {
                new Appointment
                {
                    Id = 1,
                    AppointmentId = Guid.NewGuid(),
                    Title = "Haircut",
                    Description = "Routine checkup",
                    AppointmentDate = DateTime.Now.AddDays(1),
                    CustomerName = "John Doe",
                    CustomerEmail = "john.doe@example.com",
                    Status = "Confirmed",
                    CreatedAt = DateTime.Now
                },
                new Appointment
                {
                    Id = 2,
                    AppointmentId = Guid.NewGuid(),
                    Title = "Dental Checkup",
                    Description = "Routine dental exam",
                    AppointmentDate = DateTime.Now.AddDays(2),
                    CustomerName = "Jane Smith",
                    CustomerEmail = "jane.smith@example.com",
                    Status = "Pending",
                    CreatedAt = DateTime.Now
                }
            };

            _context.Appointments.AddRange(mockAppointments);
            _context.SaveChanges();

            // Create the controller with the in-memory context
            _controller = new AppointmentsController(_context);
        }


        [TearDown]
        public void TearDown()
        {
            // Remove all entries from the database to reset state between tests
            _context.Appointments.RemoveRange(_context.Appointments);
            _context.SaveChanges();

            _context.Dispose(); // Dispose the context after each test
            _controller = null;
        }

        [Test]
        public async Task GetAll_ReturnsOkResult_WhenAppointmentsExist()
        {
            // Arrange: Create a default search argument instead of passing null
            var searchArguments = new AppointmentSearchArguments(); // 🔹 Avoids NullReferenceException

            // Act: Call the method under test
            var result = await _controller.GetAll(searchArguments);

            // Assert: Ensure result is of type OkObjectResult
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult, "The result should be of type OkObjectResult");

            // Assert status code
            Assert.AreEqual(200, okResult.StatusCode);

            // Assert the result contains the expected number of appointments
            var returnedAppointments = okResult.Value as IEnumerable<Appointment>;
            Assert.IsNotNull(returnedAppointments, "Returned appointments should not be null");
            Assert.AreEqual(2, returnedAppointments.Count(), "The number of appointments returned should be 2");
        }

        [Test]
        public async Task GetAll_ReturnsOkResult_WithMatchingTitle()
        {
            // Arrange: Add test data
            var appointment = new Appointment
            {
                Title = "Test Title",
                CustomerEmail = "john.doe@example.com",
                CustomerName = "John Doe",
                Description = "Routine checkup",
                Status = "Confirmed",
                AppointmentDate = DateTime.UtcNow
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            var searchArguments = new AppointmentSearchArguments { Title = "Test Title" };

            // Act: Call the method under test
            var result = await _controller.GetAll(searchArguments);

            // Assert: Ensure result is OkObjectResult
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult, "Expected an OkObjectResult.");
            Assert.AreEqual(200, okResult.StatusCode);

            var returnedAppointments = okResult.Value as IEnumerable<Appointment>;
            Assert.IsNotNull(returnedAppointments, "Returned appointments should not be null.");
            Assert.AreEqual(1, returnedAppointments.Count(), "The number of appointments returned should be 1.");
        }
        
        [Test]
        public async Task GetAll_ReturnsOkResult_WithMatchingCustomerNameCustomerEmailAndStatus()
        {
            // Arrange: Add test data
            var appointment1 = new Appointment
            {
                Title = "Test Title",
                CustomerName = "John Doe",
                CustomerEmail = "john.doe@example.com",
                Status = "Confirmed",
                Description = "Routine dental exam",
                AppointmentDate = DateTime.UtcNow
            };

            var appointment2 = new Appointment
            {
                Title = "Test Title",
                CustomerName = "Jane Doe",
                CustomerEmail = "jane.doe@example.com",
                Status = "Pending",
                Description = "Routine dental exam",
                AppointmentDate = DateTime.UtcNow
            };

            var appointment3 = new Appointment
            {
                Title = "Test Title",
                CustomerName = "John Smith",
                CustomerEmail = "john.smith@example.com",
                Status = "Confirmed",
                Description = "Routine dental exam",
                AppointmentDate = DateTime.UtcNow
            };

            _context.Appointments.AddRange(appointment1, appointment2, appointment3);
            await _context.SaveChangesAsync();

            var searchArguments = new AppointmentSearchArguments
            {
                CustomerName = "John",
                CustomerEmail = "john.doe@example.com",
                Status = "Confirmed"
            };

            // Act: Call the method under test
            var result = await _controller.GetAll(searchArguments);

            // Assert: Ensure result is OkObjectResult
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult, "Expected an OkObjectResult.");
            Assert.AreEqual(200, okResult.StatusCode);

            var returnedAppointments = okResult.Value as IEnumerable<Appointment>;
            Assert.IsNotNull(returnedAppointments, "Returned appointments should not be null.");
            Assert.AreEqual(2, returnedAppointments.Count(), "The number of appointments returned should be 2.");

            // Assert that the returned appointment matches the search criteria
            var returnedAppointment = returnedAppointments.First();
            Assert.AreEqual("John Doe", returnedAppointment.CustomerName);
            Assert.AreEqual("john.doe@example.com", returnedAppointment.CustomerEmail);
            Assert.AreEqual("Confirmed", returnedAppointment.Status);
        }

        
        [Test]
        public async Task GetAll_ReturnsBadRequest_WhenStartDateIsGreaterThanEndDate()
        {
            // Arrange: Invalid date range
            var searchArguments = new AppointmentSearchArguments
            {
                StartDate = DateTime.UtcNow.AddDays(5),
                EndDate = DateTime.UtcNow
            };

            // Act: Call the method under test
            var result = await _controller.GetAll(searchArguments);

            // Assert: Ensure result is BadRequestObjectResult
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult, "Expected a BadRequestObjectResult when StartDate is greater than EndDate.");
            Assert.AreEqual(400, badRequestResult.StatusCode);

            // Extract and verify the response message
            var response = badRequestResult.Value;
            Assert.IsNotNull(response, "Expected a response object in BadRequest result.");
    
            var messageProperty = response.GetType().GetProperty("Message")?.GetValue(response, null);
            Assert.AreEqual("StartDate cannot be later than EndDate.", messageProperty);
        }

        [Test]
        public async Task GetByAppointmentId_ReturnsOkResult_WhenAppointmentExists()
        {
            // Arrange: Use the existing appointment's ID
            var appointmentId = _context.Appointments.First().AppointmentId;

            // Act: Call the method under test
            var result = await _controller.GetByAppointmentId(appointmentId);

            // Assert: Ensure result is OkObjectResult and contains the expected appointment data
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var returnedAppointment = okResult.Value as Appointment;
            Assert.IsNotNull(returnedAppointment);
            Assert.AreEqual(appointmentId, returnedAppointment.AppointmentId);
        }

        [Test]
        public async Task GetByAppointmentId_ReturnsNotFound_WhenAppointmentDoesNotExist()
        {
            // Arrange: Use a non-existing appointmentId
            var appointmentId = Guid.NewGuid();

            // Act: Call the method under test
            var result = await _controller.GetByAppointmentId(appointmentId);

            // Assert: Ensure result is NotFoundResult
            var notFoundResult = result.Result as NotFoundResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [Test]
        public async Task Post_ReturnsCreatedAtAction_WhenAppointmentIsValid()
        {
            // Arrange: Create a valid appointment
            var newAppointment = new Appointment
            {
                Title = "Doctor's Appointment",
                Description = "General checkup",
                AppointmentDate = DateTime.Now.AddDays(3),
                CustomerName = "Michael Scott",
                CustomerEmail = "michael.scott@example.com",
                Status = "Upcoming",
                CreatedAt = DateTime.Now
            };

            // Act: Call the method under test
            var result = await _controller.Post(newAppointment);

            // Assert: Ensure result is CreatedAtActionResult with status 201
            var createdResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(201, createdResult.StatusCode);

            // Assert that the returned appointment matches the one created
            var returnedAppointment = createdResult.Value as Appointment;
            Assert.IsNotNull(returnedAppointment);
            Assert.AreEqual(newAppointment.Title, returnedAppointment.Title);
        }

        [Test]
        public async Task Post_ReturnsBadRequest_WhenStatusIsInvalid()
        {
            // Arrange: Create an appointment with an invalid status
            var newAppointment = new Appointment
            {
                Title = "Invalid Status Appointment",
                Description = "This status is invalid",
                AppointmentDate = DateTime.Now.AddDays(5),
                CustomerName = "Stanley Hudson",
                CustomerEmail = "stanley.hudson@example.com",
                Status = "InvalidStatus",  // Invalid status
                CreatedAt = DateTime.Now
            };

            // Act: Call the method under test
            var result = await _controller.Post(newAppointment);

            // Assert: Ensure result is BadRequestObjectResult
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult, "Expected a BadRequestObjectResult.");
            Assert.AreEqual(400, badRequestResult.StatusCode, "Expected status code 400.");

            // Extract the "Message" property from the response object
            var responseObject = badRequestResult.Value;
            Assert.IsNotNull(responseObject, "Expected a response object.");

            // Use reflection to access the "Message" property
            var messageProperty = responseObject.GetType().GetProperty("Message")?.GetValue(responseObject, null);
            Assert.AreEqual("Status must be 'Cancelled', 'Upcoming', or 'Finished'", messageProperty);
        }

        [Test]
        public async Task Post_ReturnsBadRequest_WhenAppointmentDataIsNull()
        {
            // Arrange: No appointment data
            Appointment appointment = null;

            // Act: Call the method under test
            var result = await _controller.Post(appointment);

            // Assert: Ensure result is BadRequestObjectResult
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult, "Expected a BadRequestObjectResult when appointment data is null.");
            Assert.AreEqual(400, badRequestResult.StatusCode);

            // Extract and verify the response message
            var response = badRequestResult.Value;
            Assert.IsNotNull(response, "Expected a response object in BadRequest result.");
    
            var messageProperty = response.GetType().GetProperty("Message")?.GetValue(response, null);
            Assert.AreEqual("Appointment data is required", messageProperty);
        }
        
        [Test]
        public async Task Post_ReturnsBadRequest_WhenRequiredFieldsAreMissing()
        {
            // Arrange: Missing required fields (Title, CustomerName, CustomerEmail)
            var appointment = new Appointment
            {
                Title = "Test Appointment",
                CustomerName = "",  // Missing CustomerName
                CustomerEmail = "john.doe@example.com",
                Status = "Upcoming",
                AppointmentDate = DateTime.UtcNow
            };

            // Act: Call the method under test
            var result = await _controller.Post(appointment);

            // Assert: Ensure result is BadRequestObjectResult
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult, "Expected a BadRequestObjectResult when required fields are missing.");
            Assert.AreEqual(400, badRequestResult.StatusCode);

            // Extract and verify the response message
            var response = badRequestResult.Value;
            Assert.IsNotNull(response, "Expected a response object in BadRequest result.");
    
            var messageProperty = response.GetType().GetProperty("Message")?.GetValue(response, null);
            Assert.AreEqual("Title, Customer Name, and Customer Email are required fields", messageProperty);
        }

        [Test]
        public async Task Put_ReturnsNoContent_WhenAppointmentIsUpdated()
        {
            // Arrange: Use an existing appointment
            var appointmentId = _context.Appointments.First().AppointmentId;
            var updatedAppointment = new Appointment
            {
                AppointmentId = appointmentId,
                Title = "Updated Appointment",
                Description = "Updated Description",
                AppointmentDate = DateTime.Now.AddDays(4),
                CustomerName = "Jim Halpert",
                CustomerEmail = "jim.halpert@example.com",
                Status = "Finished",
                CreatedAt = DateTime.Now
            };

            // Act: Call the method under test
            var result = await _controller.Put(appointmentId, updatedAppointment);

            // Assert: Ensure result is NoContentResult
            var noContentResult = result as NoContentResult;
            Assert.IsNotNull(noContentResult);
            Assert.AreEqual(204, noContentResult.StatusCode);
        }

        [Test]
        public async Task Put_ReturnsNotFound_WhenAppointmentDoesNotExist()
        {
            // Arrange: Use a non-existing appointmentId
            var appointmentId = Guid.NewGuid();
            var updatedAppointment = new Appointment
            {
                AppointmentId = appointmentId,
                Title = "Updated Appointment",
                Description = "Updated Description",
                AppointmentDate = DateTime.Now.AddDays(4),
                CustomerName = "Jim Halpert",
                CustomerEmail = "jim.halpert@example.com",
                Status = "Finished",
                CreatedAt = DateTime.Now
            };

            // Act: Call the method under test
            var result = await _controller.Put(appointmentId, updatedAppointment);

            // Assert: Ensure result is NotFoundObjectResult
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);

            // Extract the message from the anonymous object
            var response = notFoundResult.Value;
            Assert.IsNotNull(response);

            // Use reflection to get the "Message" property value
            var messageProperty = response.GetType().GetProperty("Message")?.GetValue(response, null);
            Assert.AreEqual("Appointment not found", messageProperty);
        }


        [Test]
        public async Task Put_ReturnsBadRequest_WhenStatusIsInvalid()
        {
            // Arrange: Use an existing appointment and invalid status
            var appointmentId = _context.Appointments.First().AppointmentId;
            var updatedAppointment = new Appointment
            {
                AppointmentId = appointmentId,
                Title = "Updated Appointment",
                Description = "Updated Description",
                AppointmentDate = DateTime.Now.AddDays(4),
                CustomerName = "Jim Halpert",
                CustomerEmail = "jim.halpert@example.com",
                Status = "InvalidStatus", // Invalid status
                CreatedAt = DateTime.Now
            };

            // Act: Call the method under test
            var result = await _controller.Put(appointmentId, updatedAppointment);

            // Assert: Ensure result is BadRequestObjectResult
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult, "Expected BadRequestObjectResult when an invalid status is provided.");
            Assert.AreEqual(400, badRequestResult.StatusCode);

            // Extract the "Message" property from the response (anonymous object)
            var response = badRequestResult.Value;
            Assert.IsNotNull(response, "Expected a response object in BadRequest result.");

            // Use reflection to get the "Message" property value
            var messageProperty = response.GetType().GetProperty("Message")?.GetValue(response, null);
            Assert.AreEqual("Status must be 'Cancelled', 'Upcoming', or 'Finished'", messageProperty);
        }

        [Test]
        public async Task Put_ReturnsBadRequest_WhenAppointmentDataIsNull()
        {
            // Arrange: No appointment data
            Appointment appointment = null;

            // Act: Call the method under test
            var result = await _controller.Put(Guid.NewGuid(), appointment);

            // Assert: Ensure result is BadRequestObjectResult
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult, "Expected a BadRequestObjectResult when appointment data is null.");
            Assert.AreEqual(400, badRequestResult.StatusCode);

            // Extract and verify the response message
            var response = badRequestResult.Value;
            Assert.IsNotNull(response, "Expected a response object in BadRequest result.");
    
            var messageProperty = response.GetType().GetProperty("Message")?.GetValue(response, null);
            Assert.AreEqual("Appointment data is required", messageProperty);
        }
    
        [Test]
        public async Task Delete_ReturnsNoContent_WhenAppointmentExists()
        {
            // Arrange: Use an existing appointment's ID
            var appointmentId = _context.Appointments.First().AppointmentId;

            // Act: Call the method under test
            var result = await _controller.Delete(appointmentId);

            // Assert: Ensure result is NoContentResult
            var noContentResult = result as NoContentResult;
            Assert.IsNotNull(noContentResult);
            Assert.AreEqual(204, noContentResult.StatusCode);

            // Ensure that the appointment is actually removed from the context
            var deletedAppointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);
            Assert.IsNull(deletedAppointment, "The appointment should be deleted from the database.");
        }

        [Test]
        public async Task Delete_ReturnsNotFound_WhenAppointmentDoesNotExist()
        {
            // Arrange: Use a non-existing appointmentId
            var appointmentId = Guid.NewGuid();

            // Act: Call the method under test
            var result = await _controller.Delete(appointmentId);

            // Assert: Ensure result is NotFoundResult
            var notFoundResult = result as NotFoundResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
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
        public async Task GetAll_FiltersByTitle_ReturnsFilteredAppointments()
        {
            // Arrange
            var searchArguments = new AppointmentSearchArguments { Title = "Haircut" };

            // Act
            var result = await _controller.GetAll(searchArguments);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var appointments = okResult.Value as List<Appointment>;
            Assert.IsNotNull(appointments);
            Assert.IsTrue(appointments.All(a => a.Title.Contains("Haircut")));
        }

        [Test]
        public async Task GetAll_FiltersByCustomerName_ReturnsFilteredAppointments()
        {
            // Arrange
            var searchArguments = new AppointmentSearchArguments { CustomerName = "John Doe" };

            // Act
            var result = await _controller.GetAll(searchArguments);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var appointments = okResult.Value as List<Appointment>;
            Assert.IsNotNull(appointments);
            Assert.IsTrue(appointments.All(a => a.CustomerName.Contains("John Doe")));
        }

        [Test]
        public async Task GetAll_FiltersByCustomerEmail_ReturnsFilteredAppointments()
        {
            // Arrange
            var searchArguments = new AppointmentSearchArguments { CustomerEmail = "john.doe@example.com" };

            // Act
            var result = await _controller.GetAll(searchArguments);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var appointments = okResult.Value as List<Appointment>;
            Assert.IsNotNull(appointments);
            Assert.IsTrue(appointments.All(a => a.CustomerEmail.Contains("john.doe@example.com")));
        }

        [Test]
        public async Task GetAll_FiltersByStatus_ReturnsFilteredAppointments()
        {
            // Arrange
            var searchArguments = new AppointmentSearchArguments { Status = "Confirmed" };

            // Act
            var result = await _controller.GetAll(searchArguments);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var appointments = okResult.Value as List<Appointment>;
            Assert.IsNotNull(appointments);
            Assert.IsTrue(appointments.All(a => a.Status == "Confirmed"));
        }

        [Test]
        public async Task GetAll_FiltersByStartDate_ReturnsFilteredAppointments()
        {
            // Arrange
            var searchArguments = new AppointmentSearchArguments { StartDate = DateTime.Now };

            // Act
            var result = await _controller.GetAll(searchArguments);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var appointments = okResult.Value as List<Appointment>;
            Assert.IsNotNull(appointments);
            Assert.IsTrue(appointments.All(a => a.AppointmentDate >= DateTime.Now));
        }

        [Test]
        public async Task GetAll_FiltersByEndDate_ReturnsFilteredAppointments()
        {
            // Arrange
            var searchArguments = new AppointmentSearchArguments { EndDate = DateTime.Now.AddDays(1) };

            // Act
            var result = await _controller.GetAll(searchArguments);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var appointments = okResult.Value as List<Appointment>;
            Assert.IsNotNull(appointments);
            Assert.IsTrue(appointments.All(a => a.AppointmentDate <= DateTime.Now.AddDays(1)));
        }
        
        [Test]
        public void Appointment_Id_CanBeSetAndRetrieved()
        {
            // Arrange
            var appointment = new Appointment
            {
                Id = 1, // Set the Id
                AppointmentId = Guid.NewGuid(),
                Title = "Test Appointment",
                Description = "Test Description",
                AppointmentDate = DateTime.Now,
                CustomerName = "John Doe",
                CustomerEmail = "john.doe@example.com",
                Status = "Upcoming",
                CreatedAt = DateTime.Now
            };

            // Act
            var retrievedId = appointment.Id;

            // Assert
            Assert.AreEqual(1, retrievedId);
        }
    }
}
