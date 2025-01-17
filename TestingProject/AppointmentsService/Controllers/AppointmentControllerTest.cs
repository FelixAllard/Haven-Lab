using AppointmentsService.Controllers;
using AppointmentsService.Data;
using AppointmentsService.Entity;
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
            var result = await _controller.GetAll();

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
            var result = await _controller.GetAll();

            // Assert: Ensure result is OkObjectResult with status code 200 and empty list
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult, "The result should not be null");
            Assert.AreEqual(200, okResult.StatusCode);

            // Assert that the returned appointments list is empty
            var returnedAppointments = okResult.Value as IEnumerable<Appointment>;
            Assert.IsNotNull(returnedAppointments, "Returned appointments should not be null");
            Assert.AreEqual(0, returnedAppointments.Count(), "The number of appointments returned should be 0 when none exist");
        }
        
    }
}
