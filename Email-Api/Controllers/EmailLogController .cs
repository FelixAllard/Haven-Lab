using Email_Api.Database;
using Email_Api.Service;
using Microsoft.AspNetCore.Mvc;

namespace Email_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmailLogController : ControllerBase
{
    private readonly IEmailLogService emailLogService;

    public EmailLogController(IEmailLogService emailLogService)
    {
        this.emailLogService = emailLogService;
    }
    [HttpGet]
    public async Task<IActionResult> GetEmailLogs()
    {
        try
        {
            return Ok(await emailLogService.GetSentEmailsAsync());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
}