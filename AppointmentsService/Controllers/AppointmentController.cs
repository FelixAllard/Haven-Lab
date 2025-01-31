using AppointmentsService.Data;
using AppointmentsService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppointmentsService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly AppointmentDbContext _context;

        public AppointmentsController(AppointmentDbContext context)
        {
            _context = context;
        }

        // GET: api/Appointments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAll([FromQuery] AppointmentSearchArguments searchArguments)
        {
            var query = _context.Appointments.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchArguments.Title))
            {
                query = query.Where(a => a.Title.Contains(searchArguments.Title));
            }
            if (!string.IsNullOrWhiteSpace(searchArguments.CustomerName))
            {
                query = query.Where(a => a.CustomerName.Contains(searchArguments.CustomerName));
            }
            if (!string.IsNullOrWhiteSpace(searchArguments.CustomerEmail))
            {
                query = query.Where(a => a.CustomerEmail.Contains(searchArguments.CustomerEmail));
            }
            if (!string.IsNullOrWhiteSpace(searchArguments.Status))
            {
                query = query.Where(a => a.Status == searchArguments.Status);
            }
            if (searchArguments.StartDate.HasValue)
            {
                query = query.Where(a => a.AppointmentDate >= searchArguments.StartDate.Value);
            }
            if (searchArguments.EndDate.HasValue)
            {
                query = query.Where(a => a.AppointmentDate <= searchArguments.EndDate.Value);
            }

            var appointments = await query.ToListAsync();
            return Ok(appointments);
        }


        // GET: api/Appointments/{appointmentId}
        [HttpGet("{appointmentId}")]
        public async Task<ActionResult<Appointment>> GetByAppointmentId(Guid appointmentId)
        {
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);

            if (appointment == null)
            {
                return NotFound();
            }

            return Ok(appointment);
        }

        // POST: api/Appointments
        [HttpPost]
        public async Task<ActionResult<Appointment>> Post(Appointment appointment)
        {
            if (appointment.Status != "Cancelled" && appointment.Status != "Upcoming" && appointment.Status != "Finished")
            {
                return BadRequest("Status must be 'Cancelled', 'Upcoming', or 'Finished'");
            }
            
            // Generate a new UUID for the appointment
            appointment.AppointmentId = Guid.NewGuid();

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetByAppointmentId), new { appointmentId = appointment.AppointmentId }, appointment);
        }


        // PUT: api/Appointments/{appointmentId}
        [HttpPut("{appointmentId}")]
        public async Task<IActionResult> Put(Guid appointmentId, Appointment appointment)
        {
            
            if (!AppointmentExists(appointmentId))
            {
                return NotFound();
            }
            
            if (appointment.Status != "Cancelled" && appointment.Status != "Upcoming" && appointment.Status != "Finished")
            {
                return BadRequest("Status must be 'Cancelled', 'Upcoming', or 'Finished'");
            }

            var existingAppointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);

            // Update the fields of the existing appointment with the new values
            existingAppointment.Title = appointment.Title;
            existingAppointment.Description = appointment.Description;
            existingAppointment.AppointmentDate = appointment.AppointmentDate;
            existingAppointment.CustomerName = appointment.CustomerName;
            existingAppointment.CustomerEmail = appointment.CustomerEmail;
            existingAppointment.Status = appointment.Status;
            existingAppointment.CreatedAt = appointment.CreatedAt;
            
            await _context.SaveChangesAsync();
            
            return NoContent();
        }


        private bool AppointmentExists(Guid appointmentId)
        {
            return _context.Appointments.Any(e => e.AppointmentId == appointmentId);
        }


        // DELETE: api/Appointments/{appointmentId}
        [HttpDelete("{appointmentId}")]
        public async Task<IActionResult> Delete(Guid appointmentId)
        {
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);
    
            if (appointment == null)
            {
                return NotFound();
            }

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
