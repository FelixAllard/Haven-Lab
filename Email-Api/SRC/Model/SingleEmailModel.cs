namespace Email_Api.Model;

public class SingleEmailModel
{
    public string FromName { get; set; } 
    public string FromEmail { get; set; } 
    public string To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
}