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
        public async Task<IActionResult> GetAll([FromQuery] AppointmentSearchArguments searchArguments)
        {
            try
            {
                IQueryable<Appointment> query = _context.Appointments.AsQueryable();

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
                if (searchArguments.StartDate.HasValue && searchArguments.EndDate.HasValue)
                {
                    if (searchArguments.StartDate > searchArguments.EndDate)
                    {
                        return BadRequest(new { Message = "StartDate cannot be later than EndDate." });
                    }

                    query = query.Where(a => a.AppointmentDate >= searchArguments.StartDate.Value &&
                                             a.AppointmentDate <= searchArguments.EndDate.Value);
                }
                else if (searchArguments.StartDate.HasValue)
                {
                    query = query.Where(a => a.AppointmentDate >= searchArguments.StartDate.Value);
                }
                else if (searchArguments.EndDate.HasValue)
                {
                    query = query.Where(a => a.AppointmentDate <= searchArguments.EndDate.Value);
                }

                var appointments = await query.ToListAsync();

                if (appointments == null || !appointments.Any())
                {
                    return NotFound(new { Message = "No appointments found matching the criteria." });
                }

                return Ok(appointments);
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, new 
                { 
                    Message = "Database error occurred while fetching the appointment", 
                    Details = dbEx.Message 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An unexpected error occurred.",
                    Details = ex.Message
                });
            }
        }

        // GET: api/Appointments/{appointmentId}
        [HttpGet("{appointmentId}")]
        public async Task<ActionResult<Appointment>> GetByAppointmentId(Guid appointmentId)
        {
            try
            {
                var appointment = await _context.Appointments
                    .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);

                if (!AppointmentExists(appointmentId))
                {
                    return NotFound();
                }

                return Ok(appointment);
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, new 
                { 
                    Message = "Database error occurred while fetching the appointment", 
                    Details = dbEx.Message 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new 
                { 
                    Message = "An unexpected error occurred", 
                    Details = ex.Message 
                });
            }
        }
        
        // POST: api/Appointments
        [HttpPost]
        public async Task<ActionResult<Appointment>> Post([FromBody] Appointment appointment)
        {
            try
            {
                if (appointment == null)
                {
                    return BadRequest(new { Message = "Appointment data is required" });
                }

                if (string.IsNullOrWhiteSpace(appointment.Title) || string.IsNullOrWhiteSpace(appointment.CustomerName) || string.IsNullOrWhiteSpace(appointment.CustomerEmail))
                {
                    return BadRequest(new { Message = "Title, Customer Name, and Customer Email are required fields" });
                }

                if (appointment.Status != "Cancelled" && appointment.Status != "Upcoming" && appointment.Status != "Finished")
                {
                    return BadRequest(new { Message = "Status must be 'Cancelled', 'Upcoming', or 'Finished'" });
                }

                // Generate a new UUID for the appointment
                appointment.AppointmentId = Guid.NewGuid();

                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetByAppointmentId), new { appointmentId = appointment.AppointmentId }, appointment);
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, new
                {
                    Message = "Database error occurred while saving the appointment",
                    Details = dbEx.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An unexpected error occurred",
                    Details = ex.Message
                });
            }
        }

        // PUT: api/Appointments/{appointmentId}
        [HttpPut("{appointmentId}")]
        public async Task<IActionResult> Put(Guid appointmentId, [FromBody] Appointment appointment)
        {
            try
            {
                if (appointment == null)
                {
                    return BadRequest(new { Message = "Appointment data is required" });
                }
                
                if (!AppointmentExists(appointmentId))
                {
                    return NotFound(new { Message = "Appointment not found" });
                }

                if (appointment.Status != "Cancelled" && appointment.Status != "Upcoming" && appointment.Status != "Finished")
                {
                    return BadRequest(new { Message = "Status must be 'Cancelled', 'Upcoming', or 'Finished'" });
                }

                var existingAppointment = await _context.Appointments
                    .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);

                if (existingAppointment == null)
                {
                    return NotFound(new { Message = $"Appointment with ID {appointmentId} not found" });
                }

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
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, new
                {
                    Message = "Database error occurred while updating the appointment",
                    Details = dbEx.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An unexpected error occurred",
                    Details = ex.Message
                });
            }
        }

        private bool AppointmentExists(Guid appointmentId)
        {
            return _context.Appointments.Any(e => e.AppointmentId == appointmentId);
        }


        // DELETE: api/Appointments/{appointmentId}
        [HttpDelete("{appointmentId}")]
        public async Task<IActionResult> Delete(Guid appointmentId)
        {
            try
            {
                if (!AppointmentExists(appointmentId))
                {
                    return NotFound(new { Message = "Appointment not found" });
                }
                
                var appointment = await _context.Appointments
                    .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);

                if (appointment == null)
                {
                    return NotFound(new { Message = $"Appointment with ID {appointmentId} not found" });
                }
                
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();

                return Ok(new { Message = $"Appointment with ID {appointmentId} successfully deleted" });
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, new
                {
                    Message = "Database error occurred while deleting the appointment",
                    Details = dbEx.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An unexpected error occurred",
                    Details = ex.Message
                });
            }
        }
    }
}
