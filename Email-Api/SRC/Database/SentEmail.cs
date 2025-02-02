using System.ComponentModel.DataAnnotations;

namespace Email_Api.Database;

public class SentEmail
{
    [Key]
    public int Id { get; set; }
    public string RecipientEmail { get; set; }
    public string EmailSubject { get; set; }
    public string EmailBody { get; set; }
}