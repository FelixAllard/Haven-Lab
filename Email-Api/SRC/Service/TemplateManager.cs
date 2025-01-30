using System.Diagnostics;
using Email_Api.Exceptions;
using Email_Api.Model;

namespace Email_Api.Service;

public class TemplateManager : ITemplateManager
{
    Dictionary<string, EmailTemplate> fileContents = new();
    List<Template> templates = new();
    
    private readonly string _folderPath = "./Templates";
    
    
    
    

    public int CountTxtFiles()
    {
        return Directory.GetFiles(_folderPath, "*.html").Length;
    }

    public string ReadTxtFile(string fileName)
    {
        string filePath = Path.Combine(_folderPath, fileName);
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"The file '{fileName}' does not exist in the specified folder.");
        }
        return File.ReadAllText(filePath);
    }

    public EmailTemplate GetTemplate(string templateName)
    {
        if (CountTxtFiles() != fileContents.Count)
            ReadAllTxtFiles(); // Ensure fileContents is updated

        if (fileContents.TryGetValue(templateName, out EmailTemplate content))
            return content;

        throw new KeyNotFoundException($"Template '{templateName}' not found.");
    }

    public List<Template> GetTemplates()
    {
        if (CountTxtFiles() != fileContents.Count)
            ReadAllTxtFiles();
        return templates;
    }

    public Template PostTemplate(Template template)
    {
        if (CountTxtFiles() != fileContents.Count)
            ReadAllTxtFiles();
        // Ensure the directory exists
        if (!Directory.Exists(_folderPath))
            Directory.CreateDirectory(_folderPath);
        

        // Construct full file path using the provided name
        string filePath = Path.Combine(_folderPath, template.TemplateName + ".html");
        
        if(File.Exists(filePath))
            throw new TemplatesWithIdenticalNamesFound("A template already exists with the same name.", template.TemplateName);

        // Create and write to the file
        File.WriteAllText(filePath, template.EmailTemplate.HtmlFormat);
        ReadAllTxtFiles();
        return new Template()
        {
            TemplateName = template.TemplateName,
            EmailTemplate = GetTemplate(template.TemplateName)
        };
    }

    public Template PutTemplate(string templateName, Template template)
    {
        if (CountTxtFiles() != fileContents.Count)
            ReadAllTxtFiles();
        // Ensure the directory exists
        if (!Directory.Exists(_folderPath))
        {
            Directory.CreateDirectory(_folderPath);
        }
        string filePathOriginalFile = Path.Combine(_folderPath, templateName + ".html");
        if (!File.Exists(filePathOriginalFile))
            throw new KeyNotFoundException($"The file '{templateName}' does not exist in the specified folder.");
        File.Delete(filePathOriginalFile);
        
        // Construct full file path using the provided name
        string filePath = Path.Combine(_folderPath, template.TemplateName + ".html");

        // Create and write to the file
        File.WriteAllText(filePath, template.EmailTemplate.HtmlFormat);
        ReadAllTxtFiles();
        return new Template()
        {
            TemplateName = template.TemplateName,
            EmailTemplate = GetTemplate(template.TemplateName)
        };
    }

    public Template DeleteTemplate(string templateName)
    {
        if (CountTxtFiles() != fileContents.Count)
            ReadAllTxtFiles();
        // Ensure the directory exists
        if (!Directory.Exists(_folderPath))
        {
            Directory.CreateDirectory(_folderPath);
        }
        string filePathOriginalFile = Path.Combine(_folderPath, templateName + ".html");
        if (!File.Exists(filePathOriginalFile))
            throw new KeyNotFoundException($"The file '{templateName}' does not exist in the specified folder.");
        Template previousTemplate = new Template()
        {
            TemplateName = templateName,
            EmailTemplate = GetTemplate(templateName)
        };
        File.Delete(filePathOriginalFile);
        ReadAllTxtFiles();
        return previousTemplate;
    }

    public void ReadAllTxtFiles()
    {
        Debug.WriteLine($"Absolute Path: {Path.GetFullPath(_folderPath)}");
        foreach (string filePath in Directory.GetFiles(_folderPath, "*.html"))
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            fileContents[fileName] = new EmailTemplate(File.ReadAllText(filePath));
            Debug.WriteLine($"Added TEMPLATE : {fileName}");
        }

        templates = new List<Template>();
        foreach (var file in fileContents)
        {
            templates.Add(new Template()
                {
                    TemplateName = file.Key, 
                    EmailTemplate = file.Value
                }
            );
            
        }
    }
}