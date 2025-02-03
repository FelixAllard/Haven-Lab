using Api_Gateway.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api_Gateway.Controller;
[ApiController]
[Route("gateway/api/[controller]")]
public class ProxyEmailLogController : ControllerBase
{
    private readonly ServiceEmailLogController _serviceEmailLog;
    

    // Constructor injects the ServicePromoController
    public ProxyEmailLogController(ServiceEmailLogController _serviceEmailLog)
    {
        this._serviceEmailLog = _serviceEmailLog;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllTemplatesNames()
    {
        try
        {
            var result = await _serviceEmailLog.GetAllEmailLogs();
            if (result.Contains("404 Not Found"))
            {
                return NotFound(new { message = result });
            }
            else if (result.StartsWith("401 Unauthorized"))
            {
                return Unauthorized(new { message = result });
            }

            if (result.StartsWith("Error"))
            {
                return StatusCode(500, new { Message = result });
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