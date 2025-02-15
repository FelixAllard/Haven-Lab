using Api_Gateway.Models;
using Api_Gateway.Services;
using Microsoft.AspNetCore.Mvc;
using Api_Gateway.Annotations;

namespace Api_Gateway.Controller;

[Route("gateway/api/[controller]")]
[ApiController]
public class ProxyEmailApiController : ControllerBase
{
    private readonly ServiceEmailApiController serviceEmailApiController;

    public ProxyEmailApiController(ServiceEmailApiController serviceEmailApiController)
    {
        this.serviceEmailApiController = serviceEmailApiController;
        
    }
    
    [HttpPost("sendwithformat")]
    [RequireAuth]
    public async Task<IActionResult> SendSingleEmailWithTemplate([FromBody]DirectEmailModel directEmailModel)
    {
        var response = serviceEmailApiController.PostEmail(directEmailModel);
        
        
        return StatusCode(response.Result.StatusCode, response.Result.Content);
        
    }
}