using System.Net;
using Email_Api.Exceptions;
using Email_Api.Model;
using Email_Api.Service;
using Microsoft.AspNetCore.Mvc;

namespace Email_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmailController : ControllerBase
{
    private readonly ISmtpConnection smtpConnection;
    private readonly IEmailService emailService;

    public EmailController(ISmtpConnection smtpConnection, IEmailService emailService)
    {
        this.smtpConnection = smtpConnection;
        this.emailService = emailService;
    }
    
    [HttpPost("send")]

    public async Task<IActionResult> SendSingleEmail([FromBody]SingleEmailModel singleEmailModel)
    {
        smtpConnection.SendEmailAsync(singleEmailModel);
        return Ok("");
    }
    [HttpPost("sendwithformat")]

    public async Task<IActionResult> SendSingleEmailWithTemplate([FromBody]DirectEmailModel directEmailModel)
    {
        if (directEmailModel == null || directEmailModel.IsEmpty())
        {
            return NoContent();
        }

        try
        {
            return Ok(emailService.SendEmail(directEmailModel));
        }
        catch (BadEmailModel e)
        {
            Console.WriteLine(e);
            return BadRequest(new { message = e.Message });
        }
        catch (TriedToFindNonExistingTemplate e)
        {
            Console.WriteLine(e);
            return NotFound(new { message = e.Message });
        }
        /*catch (NullReferenceException e)
        {
            Console.WriteLine(e);
            return NotFound(new { message = e.Message });
        }*/
        catch (EmailStringContainsPlaceholder e)
        {
            Console.WriteLine(e);
            return BadRequest(new { message = e.Message });
        }
        catch (TemplateRequiredFieldNotSet e)
        {
            Console.WriteLine(e);
            return BadRequest(new { message = e.Message });
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(new { message = e.Message });
        }
    }
    
    
}