namespace Api_Gateway.Controller;

using Api_Gateway.Services;
using Microsoft.AspNetCore.Mvc;
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
}