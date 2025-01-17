namespace Api_Gateway.Services;

using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class ServiceAppointmentsController
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string BASE_URL;

    public ServiceAppointmentsController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        BASE_URL = Environment.GetEnvironmentVariable("BASE_URL_APPOINTMENT_API") ?? "http://localhost:5114";
    }

    public async Task<string> GetAllAppointmentsAsync()
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var requestUrl = $"{BASE_URL}/api/Appointments";

            var response = await client.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                return $"Error fetching appointments: {response.ReasonPhrase}";
            }
        }
        catch (Exception ex)
        {
            return $"Error 500: Internal Server Error - {ex.Message}";
        }
    }
}
