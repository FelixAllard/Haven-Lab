namespace Api_Gateway.Controller
{
    using Api_Gateway.Services;
    using Api_Gateway.Models;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Threading.Tasks;

    [Route("gateway/api/[controller]")]
    [ApiController]
    public class ProxyAppointmentController : ControllerBase
    {
        private readonly ServiceAppointmentsController _serviceAppointmentController;

        public ProxyAppointmentController(ServiceAppointmentsController serviceAppointmentsController)
        {
            _serviceAppointmentController = serviceAppointmentsController;
        }

        // GET: gateway/api/ProxyAppointment/all
        [HttpGet("all")]
        public async Task<IActionResult> GetAllAppointments()
        {
            try
            {
                var result = await _serviceAppointmentController.GetAllAppointmentsAsync();

                if (result.StartsWith("Error"))
                {
                    return BadRequest(new { Message = result });
                }

                return Ok(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500, new { Message = e.Message });
            }
        }

        // GET: gateway/api/ProxyAppointment/{appointmentId}
        [HttpGet("{appointmentId}")]
        public async Task<IActionResult> GetAppointmentById(Guid appointmentId)
        {
            try
            {
                var result = await _serviceAppointmentController.GetAppointmentByIdAsync(appointmentId);

                if (result.StartsWith("Error"))
                {
                    return BadRequest(new { Message = result });
                }

                return Ok(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500, new { Message = e.Message });
            }
        }

        // POST: gateway/api/ProxyAppointment
        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] Appointment appointment)
        {
            if (appointment.Status != "Cancelled" && appointment.Status != "Upcoming" && appointment.Status != "Finished")
            {
                return BadRequest("Status must be 'Cancelled', 'Upcoming', or 'Finished'");
            }
            
            try
            {
                var result = await _serviceAppointmentController.CreateAppointmentAsync(appointment);

                if (result.StartsWith("Error"))
                {
                    return BadRequest(new { Message = result });
                }

                return CreatedAtAction(nameof(GetAppointmentById), new { appointmentId = appointment.AppointmentId }, result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500, new { Message = e.Message });
            }
        }

        // PUT: gateway/api/ProxyAppointment/{appointmentId}
        [HttpPut("{appointmentId}")]
        public async Task<IActionResult> UpdateAppointment(Guid appointmentId, [FromBody] Appointment appointment)
        {
            
            if (appointment.Status != "Cancelled" && appointment.Status != "Upcoming" && appointment.Status != "Finished")
            {
                return BadRequest("Status must be 'Cancelled', 'Upcoming', or 'Finished'");
            }
            
            try
            {
                var result = await _serviceAppointmentController.UpdateAppointmentAsync(appointmentId, appointment);

                if (result.StartsWith("Error"))
                {
                    return BadRequest(new { Message = result });
                }

                return NoContent();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500, new { Message = e.Message });
            }
        }

        // DELETE: gateway/api/ProxyAppointment/{appointmentId}
        [HttpDelete("{appointmentId}")]
        public async Task<IActionResult> DeleteAppointment(Guid appointmentId)
        {
            try
            {
                var result = await _serviceAppointmentController.DeleteAppointmentAsync(appointmentId);

                if (result.StartsWith("Error"))
                {
                    return BadRequest(new { Message = result });
                }

                return NoContent();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500, new { Message = e.Message });
            }
        }
    }
}
