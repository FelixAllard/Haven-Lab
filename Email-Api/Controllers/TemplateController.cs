using Email_Api.Model;
using Microsoft.AspNetCore.Mvc;

namespace Email_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TemplateController : ControllerBase
{
    [HttpGet("names")]
    public List<String> GetAllTemplatesNames()
    {
        throw new NotImplementedException();
    }
    [HttpGet]
    public List<Template> GetAllTemplates()
    {
        throw new NotImplementedException();
    }
    [HttpGet("{name}")]
    public Template GetTemplateByName([FromRoute]string name)
    {
        throw new NotImplementedException();
    }
    [HttpPost]
    public Template PostTemplate([FromBody]Template template)
    {
        throw new NotImplementedException();
    }
    [HttpPut("{name}")]
    public Template PutTemplate([FromRoute]string name, [FromBody]Template template)
    {
        throw new NotImplementedException();
    }
    [HttpDelete("{name}")]

    public Template DeleteTemplate(string name)
    {
        throw new NotImplementedException();
    }
}