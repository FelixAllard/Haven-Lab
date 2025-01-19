namespace Email_Api.Model;

public class OperationResult
{
    public OperationResult(string result = null, string message = null, bool success = default)
    {
        Result = result;
        Message = message;
        Success = success;
    }

    public string Result { get; set; }
    public string Message { get; set; }
    public bool Success { get; set; }
}