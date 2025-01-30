using Email_Api.Model;

namespace Email_Api.Service;

public interface ITemplateManager
{
    public int CountTxtFiles();
    
    public string ReadTxtFile(string fileName);
    
    /// <summary>
    /// Retrieves an email template by its name.
    /// </summary>
    /// <param name="templateName">The name of the template to retrieve.</param>
    /// <returns>The email template associated with the given name.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the template does not exist.</exception>
    public EmailTemplate GetTemplate(string templateName);

    /// <summary>
    /// Retrieves a list of all available email templates.
    /// </summary>
    /// <returns>A list of templates.</returns>
    public List<Template> GetTemplates();

    /// <summary>
    /// Creates a new email template and saves it to the file system.
    /// </summary>
    /// <param name="template">The template to be created.</param>
    /// <returns>The newly created template.</returns>
    public Template PostTemplate(Template template);

    /// <summary>
    /// Updates an existing email template by replacing its content.
    /// </summary>
    /// <param name="templateName">The name of the template to update.</param>
    /// <param name="template">The updated template data.</param>
    /// <returns>The updated template.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the template does not exist.</exception>
    public Template PutTemplate(string templateName, Template template);

    /// <summary>
    /// Deletes an email template from the file system.
    /// </summary>
    /// <param name="templateName">The name of the template to delete.</param>
    /// <returns>The deleted template before removal.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the template does not exist.</exception>
    public Template DeleteTemplate(string templateName);

}