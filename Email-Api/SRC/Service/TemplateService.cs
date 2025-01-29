using Email_Api.Exceptions;
using Email_Api.Model;

namespace Email_Api.Service;

public class TemplateService : ITemplateService
{

    private readonly ITemplateManager _templateManager;
    
    
    
    public TemplateService(ITemplateManager templateManager)
    {
        _templateManager = templateManager;
    }
    
    public List<string> GetAllTemplatesNames()
    {
        List<Template> templates = _templateManager.GetTemplates();
        List<string> verifiedNames = new List<string>();

        foreach (var template in templates)
        {
            verifiedNames.Add(template.TemplateName);
        }
        return verifiedNames;
    }
    
    public List<Template> GetAllTemplates()
    {
        return _templateManager.GetTemplates();
    }

    public Template GetTemplateByName(string name)
    {
        Template template = new Template()
        {
            TemplateName = name,
            EmailTemplate = _templateManager.GetTemplate(name)
        };
        return template;
    }

    public Template PostTemplate(Template template)
    {
        foreach (var existingTemplate in _templateManager.GetTemplates())
        {
            if(existingTemplate.TemplateName == template.TemplateName)
                throw new TemplatesWithIdenticalNamesFound("Can't create a template with the same name, as an already existing template : ", template.TemplateName);
        }
        
        return _templateManager.PostTemplate(template);
    }

    public Template PutTemplate(string name, Template template)
    {
        if (name == "Default")
            throw new UnauthorizedAccessException("Default template can't be modified from the website");
        foreach (var existingTemplate in _templateManager.GetTemplates())
        {
            if(existingTemplate.TemplateName == template.TemplateName)
                return _templateManager.PutTemplate(name, template);
        }
        throw new KeyNotFoundException("Template does not exist");
    }

    public Template DeleteTemplate(string name)
    {
        if (name == "Default")
            throw new UnauthorizedAccessException("Default template can't be modified from the website");
        foreach (var existingTemplate in _templateManager.GetTemplates())
        {
            if(existingTemplate.TemplateName == name)
                return _templateManager.DeleteTemplate(name);
        }
        throw new KeyNotFoundException("Template does not exist");
    }
}