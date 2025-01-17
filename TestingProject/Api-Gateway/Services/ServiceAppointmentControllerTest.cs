using System;
using System.Linq;
using System.Threading.Tasks;
using AppointmentsService.Data;
using AppointmentsService.Models;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace TestingProject.AppointmentsService.Tests
{
    [TestFixture]
    public class ServiceAppointmentControllerTest
    {
        private AppointmentDbContext _dbContext;
        private DbContextOptions<AppointmentDbContext> _dbOptions;

        [SetUp]
        public void SetUp()
        {
            // Setup in-memory database options
            _dbOptions = new DbContextOptionsBuilder<AppointmentDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            // Initialize DbContext with in-memory database
            _dbContext = new AppointmentDbContext(_dbOptions);
        }

        [TearDown]
        public void TearDown()
        {
            // Dispose the DbContext after each test
            _dbContext.Dispose();
        }
        
    [Test]
    public async Task GetAllAppointments_Success_ReturnsAllAppointments()
    {
        // Arrange
        var appointment1 = new Appointment
        {
            Id = 1,
            AppointmentId = Guid.NewGuid(),
            Title = "Test Appointment",
            Description = "Test Description",
            AppointmentDate = DateTime.UtcNow,
            CustomerName = "John Doe",
            CustomerEmail = "john.doe@example.com",
            Status = "Scheduled",
            CreatedAt = DateTime.UtcNow
        };
        var appointment2 = new Appointment
        {
            Id = 2,
            AppointmentId = Guid.NewGuid(),
            Title = "Test Appointment2",
            Description = "Test Description2",
            AppointmentDate = DateTime.UtcNow.AddDays(1),
            CustomerName = "John Doe2",
            CustomerEmail = "john.doe2@example.com",
            Status = "Scheduled2",
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.Appointments.AddRangeAsync(appointment1, appointment2);
        await _dbContext.SaveChangesAsync();

        // Act
        var allAppointments = await _dbContext.Appointments.ToListAsync();

        // Assert
        Assert.AreEqual(2, allAppointments.Count);

        var fetchedAppointment1 = allAppointments.FirstOrDefault(a => a.Id == 1);
        var fetchedAppointment2 = allAppointments.FirstOrDefault(a => a.Id == 2);

        Assert.NotNull(fetchedAppointment1);
        Assert.NotNull(fetchedAppointment2);

        // Assert each field for appointment1
        Assert.AreEqual(appointment1.Id, fetchedAppointment1.Id);
        Assert.AreEqual(appointment1.AppointmentId, fetchedAppointment1.AppointmentId);
        Assert.AreEqual(appointment1.Title, fetchedAppointment1.Title);
        Assert.AreEqual(appointment1.Description, fetchedAppointment1.Description);
        Assert.AreEqual(appointment1.AppointmentDate, fetchedAppointment1.AppointmentDate);
        Assert.AreEqual(appointment1.CustomerName, fetchedAppointment1.CustomerName);
        Assert.AreEqual(appointment1.CustomerEmail, fetchedAppointment1.CustomerEmail);
        Assert.AreEqual(appointment1.Status, fetchedAppointment1.Status);
        Assert.AreEqual(appointment1.CreatedAt, fetchedAppointment1.CreatedAt);

        // Assert each field for appointment2
        Assert.AreEqual(appointment2.Id, fetchedAppointment2.Id);
        Assert.AreEqual(appointment2.AppointmentId, fetchedAppointment2.AppointmentId);
        Assert.AreEqual(appointment2.Title, fetchedAppointment2.Title);
        Assert.AreEqual(appointment2.Description, fetchedAppointment2.Description);
        Assert.AreEqual(appointment2.AppointmentDate, fetchedAppointment2.AppointmentDate);
        Assert.AreEqual(appointment2.CustomerName, fetchedAppointment2.CustomerName);
        Assert.AreEqual(appointment2.CustomerEmail, fetchedAppointment2.CustomerEmail);
        Assert.AreEqual(appointment2.Status, fetchedAppointment2.Status);
        Assert.AreEqual(appointment2.CreatedAt, fetchedAppointment2.CreatedAt);
    }
    }
}
