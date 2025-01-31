using System.Net;
using Api_Gateway.Models;
using Api_Gateway.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api_Gateway.Controller;

[Route("gateway/api/[controller]")]
[ApiController]
public class ProxyTemplateApiController : ControllerBase
{
    private readonly ServiceTemplateController _serviceTemplateController;

    // Constructor injects the ServicePromoController
    public ProxyTemplateApiController(ServiceTemplateController serviceTemplateController)
    {
        _serviceTemplateController = serviceTemplateController;
    }
    [HttpGet("names")]
    public async Task<IActionResult> GetAllTemplatesNames()
    {
        try
        {
            var result = await _serviceTemplateController.GetAllTemplateNames();

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
    [HttpGet]
    public async Task<IActionResult> GetAllTemplates()
    {
        try
        {
            var result = await _serviceTemplateController.GetAllTemplate();

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
    [HttpGet("{name}")]
    public async Task<IActionResult> GetTemplateByName([FromRoute]string name)
    {
        try
        {
            var result = await _serviceTemplateController.GetTemplateByName(name);

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
    [HttpPost]
    public async Task<IActionResult> PostTemplate([FromBody]Template template)
    {
        try
        {
            var response = await _serviceTemplateController.PostTemplate(template);

            if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                return StatusCode(503, new { message = "Service is currently unavailable, please try again later." });
            }

            var content = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, content);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred", details = ex.Message });
        }
        
    }
    [HttpPut("{name}")]
    public async Task<IActionResult> PutTemplate([FromRoute]string name, [FromBody]Template template)
    {
        try
        {
            var response = await _serviceTemplateController.PutTemplate(name,template);

            if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                return StatusCode(503, new { message = "Service is currently unavailable, please try again later." });
            }

            var content = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, content);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred", details = ex.Message });
        }
    }
    [HttpDelete("{name}")]

    public async Task<IActionResult> DeleteTemplate(string name)
    {
        try
        {
            var result = await _serviceTemplateController.DeleteTemplate(name);

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