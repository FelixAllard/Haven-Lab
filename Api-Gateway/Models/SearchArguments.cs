namespace Api_Gateway.Models;

public class SearchArguments
{
    public string Name { get; set; } = string.Empty;
    public decimal MinimumPrice { get; set; } = decimal.MinValue;
    public decimal MaximumPrice { get; set; } = decimal.MaxValue;
    public bool Available { get; set; } = false;
    
    public static string BuildQueryString(SearchArguments searchArguments)
    {
        var queryParams = new List<string>();

        if (!string.IsNullOrWhiteSpace(searchArguments.Name))
        {
            queryParams.Add($"Name={Uri.EscapeDataString(searchArguments.Name)}");
        }

        if (searchArguments.MinimumPrice > 0)
        {
            queryParams.Add($"MinimumPrice={searchArguments.MinimumPrice}");
        }

        if (searchArguments.MaximumPrice > 0)
        {
            queryParams.Add($"MaximumPrice={searchArguments.MaximumPrice}");
        }

        if (searchArguments.Available)
        {
            queryParams.Add("Available=true");
        }

        return queryParams.Any() ? $"?{string.Join("&", queryParams)}" : string.Empty;
    }
}

