namespace Email_Api.Exceptions;

public class TriedToFillEmailFieldWithEmptyWhiteSpace : Exception
{

    public TriedToFillEmailFieldWithEmptyWhiteSpace() { }

    public TriedToFillEmailFieldWithEmptyWhiteSpace(string message) 
        : base("Tried to fill email field with an empty whitespace : " + message ) { }

    public TriedToFillEmailFieldWithEmptyWhiteSpace(string message, System.Exception inner) 
        : base(message, inner) { }

}