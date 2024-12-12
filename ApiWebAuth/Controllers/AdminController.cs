using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiWebAuth.Controllers;

[Authorize (Roles = "Admin")]
[Microsoft.AspNetCore.Components.Route("api/Admin")]
[ApiController]
public class AdminController : ControllerBase
{
    [HttpGet("/Admin")]
    public IActionResult Get()
    {
        return Ok("You Have Accessed the admin Controller");
    }
    
}