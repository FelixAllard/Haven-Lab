using Email_Api.Model;

namespace Email_Api.Service;

public interface ITemplateManager
{
    public int CountTxtFiles();
    
    public string ReadTxtFile(string fileName);
    public EmailTemplate GetTemplate(string templateName);

}