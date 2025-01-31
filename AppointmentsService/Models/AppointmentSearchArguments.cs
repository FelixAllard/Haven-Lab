namespace AppointmentsService.Models;

public class AppointmentSearchArguments
{
    public string Title { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; } = null;
    public DateTime? EndDate { get; set; } = null;

    public static string BuildQueryString(AppointmentSearchArguments searchArguments)
    {
        var queryParams = new List<string>();

        if (!string.IsNullOrWhiteSpace(searchArguments.Title))
        {
            queryParams.Add($"Title={Uri.EscapeDataString(searchArguments.Title)}");
        }
        if (!string.IsNullOrWhiteSpace(searchArguments.CustomerName))
        {
            queryParams.Add($"CustomerName={Uri.EscapeDataString(searchArguments.CustomerName)}");
        }
        if (!string.IsNullOrWhiteSpace(searchArguments.CustomerEmail))
        {
            queryParams.Add($"CustomerEmail={Uri.EscapeDataString(searchArguments.CustomerEmail)}");
        }
        if (!string.IsNullOrWhiteSpace(searchArguments.Status))
        {
            queryParams.Add($"Status={Uri.EscapeDataString(searchArguments.Status)}");
        }
        if (searchArguments.StartDate.HasValue)
        {
            queryParams.Add($"StartDate={searchArguments.StartDate.Value:yyyy-MM-ddTHH:mm:ss}");
        }
        if (searchArguments.EndDate.HasValue)
        {
            queryParams.Add($"EndDate={searchArguments.EndDate.Value:yyyy-MM-ddTHH:mm:ss}");
        }

        return queryParams.Any() ? $"?{string.Join("&", queryParams)}" : string.Empty;
    }
}