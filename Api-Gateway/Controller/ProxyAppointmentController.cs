using Api_Gateway.Services;
using Api_Gateway.Models;
using Microsoft.AspNetCore.Mvc;
using Api_Gateway.Annotations;

namespace Api_Gateway.Controller
{

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
        [RequireAuth]
        public async Task<IActionResult> GetAllAppointments([FromQuery] AppointmentSearchArguments searchArguments)
        {
            try
            {
                var result = await _serviceAppointmentController.GetAllAppointmentsAsync(searchArguments);

                switch (result)
                {
                    case OkObjectResult okResult:
                        return Ok(okResult.Value);
            
                    case BadRequestObjectResult badRequest:
                        return BadRequest(new 
                        { 
                            Message = "Invalid request parameters",
                            Details = badRequest.Value 
                        });
            
                    case NotFoundResult _:
                        return NotFound(new 
                        { 
                            Message = "No appointments found matching the criteria" 
                        });
            
                    case ObjectResult objectResult when objectResult.StatusCode >= 400:
                        return StatusCode(objectResult.StatusCode ?? 500, new 
                        { 
                            Message = "Request failed",
                            Details = objectResult.Value 
                        });
            
                    case StatusCodeResult statusCodeResult when statusCodeResult.StatusCode >= 400:
                        return StatusCode(statusCodeResult.StatusCode, new 
                        { 
                            Message = "Request failed",
                            StatusCode = statusCodeResult.StatusCode 
                        });
            
                    default:
                        return StatusCode(500, new 
                        { 
                            Message = "Unexpected response format from service" 
                        });
                }
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(503, new 
                { 
                    Message = "Service unavailable",
                    Details = ex.Message 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new 
                { 
                    Message = "Internal server error",
                    Details = ex.Message,
                });
            }
        }


        // GET: gateway/api/ProxyAppointment/{appointmentId}
        [HttpGet("{appointmentId}")]
        [RequireAuth]
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
        [RequireAuth]
        public async Task<IActionResult> CreateAppointment([FromBody] Appointment appointment)
        {
            
            if (appointment.Status != "Cancelled" && appointment.Status != "Upcoming" && appointment.Status != "Finished")
            {
                return BadRequest(new { Message = "Status must be 'Cancelled', 'Upcoming', or 'Finished'" });
            }
            
            try
            {
                var result = await _serviceAppointmentController.CreateAppointmentAsync(appointment);

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
        [RequireAuth]
        public async Task<IActionResult> UpdateAppointment(Guid appointmentId, [FromBody] Appointment appointment)
        {
            
            try
            {
                var result = await _serviceAppointmentController.UpdateAppointmentAsync(appointmentId, appointment);

                if (result.StartsWith("Error"))
                {
                    return BadRequest(new { Message = result });
                }

                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500, new { Message = e.Message });
            }
        }

        // DELETE: gateway/api/ProxyAppointment/{appointmentId}
        [HttpDelete("{appointmentId}")]
        [RequireAuth]
        public async Task<IActionResult> DeleteAppointment(Guid appointmentId)
        {
            try
            {
                var result = await _serviceAppointmentController.DeleteAppointmentAsync(appointmentId);

                if (result.StartsWith("Error"))
                {
                    return BadRequest(new { Message = result });
                }

                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500, new { Message = e.Message });
            }
        }
    }
}
