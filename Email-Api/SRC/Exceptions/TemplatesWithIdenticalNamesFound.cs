namespace Email_Api.Exceptions;

public class TemplatesWithIdenticalNamesFound : Exception
{
    public TemplatesWithIdenticalNamesFound(string message, string templateName) : base(message + templateName) { }
    
}