namespace Email_Api.Exceptions;

public class BadEmailModel : System.Exception
{
    public BadEmailModel() { }
    public BadEmailModel(string message) 
        : base("Could not build message because field " + message) { }

    public BadEmailModel(string message, string placeHolder) 
        : base("Could not build message because field " + message) { }

    public BadEmailModel(string message, System.Exception inner) 
        : base(message, inner) { }
    
}