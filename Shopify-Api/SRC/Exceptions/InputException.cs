namespace Shopify_Api.Exceptions;

using System;

public class InputException : Exception
{
    public InputException() : base() { }

    public InputException(string message) : base(message) { }

    public InputException(string message, Exception innerException) : base(message, innerException) { }
}