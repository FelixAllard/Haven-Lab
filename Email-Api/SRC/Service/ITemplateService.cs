using Email_Api.Exceptions;
using Email_Api.Model;

namespace Email_Api.Service;

public interface ITemplateService
{
    /// <summary>
    /// A function that gets the name of all the templates and return it as a List of strings
    /// </summary>
    /// <returns>List of the names</returns>
    public List<String> GetAllTemplatesNames();
    /// <summary>
    /// Gets and return all the template from the TemplateManager
    /// </summary>
    /// <returns>List of templates with different names</returns>
    public List<Template> GetAllTemplates();
    /// <summary>
    /// Will return a certain template based on the name
    /// </summary>
    /// <param name="name">The name of the template to search for</param>
    /// <returns>The template with the corresponding name</returns>
    public Template GetTemplateByName(string name);
    /// <summary>
    /// Creates a new template and the corresponding file. This will be saved on the machine running the program.
    /// </summary>
    /// <param name="template"> The Template to create</param>
    /// <returns>The template created</returns>
    /// <exception cref="TemplatesWithIdenticalNamesFound">A template with the same name already existed so we blocked it from being added</exception>
    public Template PostTemplate(Template template);
    /// <summary>
    /// Updates an already existing template with the use of it's name
    /// </summary>
    /// <param name="name">The name of the template to modify</param>
    /// <param name="template">The template to replace the current one with</param>
    /// <returns>The modified template</returns>
    /// <exception cref="UnauthorizedAccessException">We tried to modify the Default template</exception>
    /// <exception cref="KeyNotFoundException">We can't modify a template which does not exist</exception>
    public Template PutTemplate(string name, Template template);
    /// <summary>
    /// Delete a template using its name
    /// </summary>
    /// <param name="name">The name of the template to delete</param>
    /// <returns>The deleted template</returns>
    /// <exception cref="UnauthorizedAccessException">We tried to modify the Default template</exception>
    /// <exception cref="KeyNotFoundException">We can't modify a template which does not exist</exception>
    public Template DeleteTemplate(string name);
}