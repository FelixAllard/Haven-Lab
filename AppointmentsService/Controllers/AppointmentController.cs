using AppointmentsService.Data;
using AppointmentsService.Entity;
namespace AppointmentsService.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly AppointmentDbContext _context;

    public AppointmentsController(AppointmentDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Appointment>>> GetAll()
    {
        var appointments = await _context.Appointments.ToListAsync();

        // If no appointments are found, still return a 200 OK response with an empty list
        return Ok(appointments);
    }
}
