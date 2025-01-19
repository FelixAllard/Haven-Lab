namespace Email_Api.Exceptions;

public class TriedToFindNonExistingTemplate : System.Exception
{
    public TriedToFindNonExistingTemplate() { }

    public TriedToFindNonExistingTemplate(string message) 
        : base(message) { }

    public TriedToFindNonExistingTemplate(string message, System.Exception inner) 
        : base(message, inner) { }
}