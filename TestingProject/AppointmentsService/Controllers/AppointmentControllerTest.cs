using AppointmentsService.Controllers;
using AppointmentsService.Data;
using AppointmentsService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            // Act: Call the method under test
            var result = await _controller.GetAll(null);

            // Assert: Ensure result is of type ActionResult
            Assert.IsInstanceOf<ActionResult<IEnumerable<Appointment>>>(result);

            // Check if the result is OkObjectResult
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult, "The result should not be null"); // Check that it's not null

            // Assert status code
            Assert.AreEqual(200, okResult.StatusCode);

            // Assert the result contains the expected number of appointments
            var returnedAppointments = okResult.Value as IEnumerable<Appointment>;
            Assert.IsNotNull(returnedAppointments, "Returned appointments should not be null");
            Assert.AreEqual(2, returnedAppointments.Count(), "The number of appointments returned should be 2");
        }
        
        [Test]
        public async Task GetAll_ReturnsOkResult_WhenNoAppointmentsExist()
        {
            // Arrange: Clear the appointments in the context (simulating an empty database)
            _context.Appointments.RemoveRange(_context.Appointments);
            await _context.SaveChangesAsync();

            // Act: Call the method under test
            var result = await _controller.GetAll(null);

            // Assert: Ensure result is OkObjectResult with status code 200 and empty list
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult, "The result should not be null");
            Assert.AreEqual(200, okResult.StatusCode);

            // Assert that the returned appointments list is empty
            var returnedAppointments = okResult.Value as IEnumerable<Appointment>;
            Assert.IsNotNull(returnedAppointments, "Returned appointments should not be null");
            Assert.AreEqual(0, returnedAppointments.Count(), "The number of appointments returned should be 0 when none exist");
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

            // Assert: Ensure result is BadRequest
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual("Status must be 'Cancelled', 'Upcoming', or 'Finished'", badRequestResult.Value);
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

            // Assert: Ensure result is NotFoundResult
            var notFoundResult = result as NotFoundResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
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

            // Assert: Ensure result is BadRequest
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual("Status must be 'Cancelled', 'Upcoming', or 'Finished'", badRequestResult.Value);
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
    }
}
