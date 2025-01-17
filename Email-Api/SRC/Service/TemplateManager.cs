using System.Diagnostics;
using Email_Api.Model;

namespace Email_Api.Service;

public class TemplateManager : ITemplateManager
{
    Dictionary<string, EmailTemplate> fileContents = new();
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

    public void ReadAllTxtFiles()
    {
        Debug.WriteLine($"Absolute Path: {Path.GetFullPath(_folderPath)}");
        foreach (string filePath in Directory.GetFiles(_folderPath, "*.html"))
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            fileContents[fileName] = new EmailTemplate(File.ReadAllText(filePath));
            Debug.WriteLine($"Added TEMPLATE : {fileName}");
        }
    }
}