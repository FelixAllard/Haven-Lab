namespace Api_Gateway.Models;

public class OrderSearchArgument
{
    public string CustomerName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? DateBefore { get; set; } = null;
    public DateTime? DateAfter { get; set; } = null;
    
    public static string BuildQueryString(OrderSearchArgument searchArguments)
    {
        var queryParams = new List<string>();

        if (!string.IsNullOrWhiteSpace(searchArguments.CustomerName))
        {
            queryParams.Add($"CustomerName={Uri.EscapeDataString(searchArguments.CustomerName)}");
        }

        if (!string.IsNullOrWhiteSpace(searchArguments.Status))
        {
            queryParams.Add($"Status={searchArguments.Status}");
        }

        if (searchArguments.DateBefore.HasValue)
        {
            queryParams.Add($"DateBefore={searchArguments.DateBefore.Value:yyyy-MM-ddTHH:mm:ss}");
        }
        if (searchArguments.DateAfter.HasValue)
        {
            queryParams.Add($"DateAfter={searchArguments.DateAfter.Value:yyyy-MM-ddTHH:mm:ss}");
        }

        return queryParams.Any() ? $"?{string.Join("&", queryParams)}" : string.Empty;
    }
}