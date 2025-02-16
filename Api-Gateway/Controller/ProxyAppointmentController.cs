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
                // Validate the appointment ID
                if (appointmentId == Guid.Empty)
                {
                    return BadRequest(new
                    {
                        Message = "ProxyAppointmentController: Invalid appointment ID",
                        Details = "The provided appointment ID is empty or invalid."
                    });
                }

                // Call the service to get the appointment by ID
                var result = await _serviceAppointmentController.GetAppointmentByIdAsync(appointmentId);

                // Handle the result based on its type
                switch (result)
                {
                    case OkObjectResult okResult:
                        return Ok(okResult.Value);

                    case BadRequestObjectResult badRequest:
                        return BadRequest(new
                        {
                            Message = "ProxyAppointmentController: Invalid request parameters",
                            Details = badRequest.Value
                        });

                    case NotFoundResult _:
                        return NotFound(new
                        {
                            Message = "ProxyAppointmentController: Appointment not found",
                            Details = $"No appointment found with ID: {appointmentId}"
                        });

                    case ObjectResult objectResult when objectResult.StatusCode >= 400:
                        return StatusCode(objectResult.StatusCode ?? 500, new
                        {
                            Message = "ProxyAppointmentController: Request failed",
                            Details = objectResult.Value
                        });

                    case StatusCodeResult statusCodeResult when statusCodeResult.StatusCode >= 400:
                        return StatusCode(statusCodeResult.StatusCode, new
                        {
                            Message = "ProxyAppointmentController: Request failed",
                            StatusCode = statusCodeResult.StatusCode
                        });

                    default:
                        return StatusCode(500, new
                        {
                            Message = "ProxyAppointmentController: Unexpected response format from service"
                        });
                }
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(503, new
                {
                    Message = "ProxyAppointmentController: Service unavailable",
                    Details = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "ProxyAppointmentController: Internal server error",
                    Details = ex.Message
                });
            }
        }

        // POST: gateway/api/ProxyAppointment
        [HttpPost]
        [RequireAuth]
        public async Task<IActionResult> CreateAppointment([FromBody] Appointment appointment)
        {
            if (appointment == null)
            {
                return BadRequest(new { Message = "Appointment data is required" });
            }

            if (string.IsNullOrWhiteSpace(appointment.Title) || string.IsNullOrWhiteSpace(appointment.CustomerName) ||
                string.IsNullOrWhiteSpace(appointment.CustomerEmail) || appointment.AppointmentDate == default)
            {
                return BadRequest(new
                    { Message = "Title, Customer Name, Customer Email, and Appointment Date are required fields." });
            }

            if (string.IsNullOrWhiteSpace(appointment.Status) ||
                (appointment.Status != "Cancelled" && appointment.Status != "Upcoming" &&
                 appointment.Status != "Finished"))
            {
                return BadRequest(new { Message = "Status must be 'Cancelled', 'Upcoming', or 'Finished'" });
            }

            if (appointment.AppointmentDate < DateTime.UtcNow)
            {
                return BadRequest(new { Message = "Appointment date must be in the future." });
            }

            try
            {
                var result = await _serviceAppointmentController.CreateAppointmentAsync(appointment);

                if (result == null)
                {
                    return StatusCode(500, new { Message = "Unexpected null response from service" });
                }
                
                switch (result)
                {
                    case CreatedAtActionResult createdResult:
                        return CreatedAtAction(nameof(GetAppointmentById),
                            new { appointmentId = appointment.AppointmentId }, createdResult.Value);

                    case CreatedResult createdResult:
                        return Created(createdResult.Location, createdResult.Value);
                    
                    case BadRequestObjectResult badRequest:
                        return BadRequest(new
                        {
                            Message = "Invalid appointment data",
                            Details = badRequest.Value
                        });

                    case ConflictObjectResult conflict:
                        return Conflict(new
                        {
                            Message = "Appointment conflict detected",
                            Details = conflict.Value
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
                    Details = ex.Message
                });
            }
        }


        // PUT: gateway/api/ProxyAppointment/{appointmentId}
        [HttpPut("{appointmentId}")]
        [RequireAuth]
        public async Task<IActionResult> UpdateAppointment(Guid appointmentId, [FromBody] Appointment appointment)
        {
            if (appointment == null)
            {
                return BadRequest(new { Message = "Appointment data is required." });
            }

            if (appointmentId == Guid.Empty)
            {
                return BadRequest(new { Message = "Appointment ID is empty." });
            }

            if (string.IsNullOrWhiteSpace(appointment.Title) || string.IsNullOrWhiteSpace(appointment.CustomerName) ||
                string.IsNullOrWhiteSpace(appointment.CustomerEmail) || appointment.AppointmentDate == default)
            {
                return BadRequest(new
                    { Message = "Title, Customer Name, Customer Email, and Appointment Date are required fields." });
            }

            if (appointment.AppointmentDate < DateTime.UtcNow)
            {
                return BadRequest(new { Message = "Appointment date must be in the future." });
            }

            if (string.IsNullOrWhiteSpace(appointment.Status) ||
                (appointment.Status != "Cancelled" && appointment.Status != "Upcoming" &&
                 appointment.Status != "Finished"))
            {
                return BadRequest(new { Message = "Status must be 'Cancelled', 'Upcoming', or 'Finished'." });
            }

            try
            {
                var result = await _serviceAppointmentController.UpdateAppointmentAsync(appointmentId, appointment);

                switch (result)
                {
                    case OkResult _:
                        return Ok(new { Message = "Appointment updated successfully." });

                    case OkObjectResult okObjectResult:
                        return Ok(okObjectResult.Value);

                    case BadRequestObjectResult badRequest:
                        return BadRequest(new
                        {
                            Message = "Invalid request parameters.",
                            Details = badRequest.Value
                        });

                    case NotFoundResult _:
                        return NotFound(new { Message = "Appointment not found." });

                    case ConflictResult _:
                        return Conflict(new { Message = "Appointment update conflict detected." });

                    case ObjectResult objectResult when objectResult.StatusCode >= 400:
                        return StatusCode(objectResult.StatusCode ?? 500, new
                        {
                            Message = "Request failed.",
                            Details = objectResult.Value
                        });

                    case StatusCodeResult statusCodeResult when statusCodeResult.StatusCode >= 400:
                        return StatusCode(statusCodeResult.StatusCode, new
                        {
                            Message = "Request failed.",
                            StatusCode = statusCodeResult.StatusCode
                        });

                    default:
                        return StatusCode(500, new { Message = "Unexpected response format from service." });
                }
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(503, new
                {
                    Message = "Service unavailable.",
                    Details = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Internal server error.",
                    Details = ex.Message
                });
            }
        }


        // DELETE: gateway/api/ProxyAppointment/{appointmentId}
        [HttpDelete("{appointmentId}")]
        [RequireAuth]
        public async Task<IActionResult> DeleteAppointment(Guid appointmentId)
        {
            if (appointmentId == Guid.Empty)
            {
                return BadRequest(new { Message = "A valid appointment ID is required." });
            }

            try
            {
                var result = await _serviceAppointmentController.DeleteAppointmentAsync(appointmentId);

                switch (result)
                {
                    case OkResult _:
                        return Ok(new { Message = "Appointment deleted successfully." });

                    case NoContentResult _:
                        return NoContent();

                    case NotFoundResult _:
                        return NotFound(new { Message = "Appointment not found." });

                    case BadRequestObjectResult badRequest:
                        return BadRequest(new
                        {
                            Message = "Invalid request parameters.",
                            Details = badRequest.Value
                        });

                    case ObjectResult objectResult when objectResult.StatusCode >= 400:
                        return StatusCode(objectResult.StatusCode ?? 500, new
                        {
                            Message = "Request failed.",
                            Details = objectResult.Value
                        });

                    case StatusCodeResult statusCodeResult when statusCodeResult.StatusCode >= 400:
                        return StatusCode(statusCodeResult.StatusCode, new
                        {
                            Message = "Request failed.",
                            StatusCode = statusCodeResult.StatusCode
                        });

                    default:
                        return StatusCode(500, new { Message = "Unexpected response format from service." });
                }
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(503, new
                {
                    Message = "Service unavailable.",
                    Details = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Internal server error.",
                    Details = ex.Message
                });
            }
        }
    }

}
