namespace Api_Gateway.Models;

public class TranslationRequest
{
    public string Locale { get; set; } 
    public string Title { get; set; }
    public string Description { get; set; }
}