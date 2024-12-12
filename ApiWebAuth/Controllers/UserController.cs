using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiWebAuth.Controllers;

[Authorize(Roles = "User")]
[Microsoft.AspNetCore.Components.Route("api/User")]
[ApiController]
public class UserController : ControllerBase
{
    [HttpGet("/user")]
    public IActionResult Get()
    {
        return Ok("You Have Accessed the user Controller");
    }
    
}