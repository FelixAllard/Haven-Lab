using Microsoft.AspNetCore.Mvc;

namespace Api_Gateway.Services;

using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Api_Gateway.Models;

public class ServiceAppointmentsController
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string BASE_URL;

    public ServiceAppointmentsController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        BASE_URL = Environment.GetEnvironmentVariable("BASE_URL_APPOINTMENT_API") ?? Environment.GetEnvironmentVariable("ENV_BASE_URL_APPOINTMENT_API") ??"http://localhost:5114";
    }

    public virtual async Task<IActionResult> GetAllAppointmentsAsync(AppointmentSearchArguments searchArguments = null)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var queryString = searchArguments != null 
                ? AppointmentSearchArguments.BuildQueryString(searchArguments) 
                : string.Empty;
            var requestUrl = $"{BASE_URL}/api/Appointments{queryString}";

            var response = await client.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                var appointments = await response.Content.ReadFromJsonAsync<List<Appointment>>();
                return new OkObjectResult(appointments);
            }
            else if ((int)response.StatusCode >= 400 && (int)response.StatusCode < 500)
            {
                return new BadRequestObjectResult(new { Message = $"Client Error: {response.ReasonPhrase}" });
            }
            else
            {
                return new StatusCodeResult((int)response.StatusCode);
            }
        }
        catch (HttpRequestException ex)
        {
            return new StatusCodeResult(503);
        }
        catch (Exception ex)
        {
            return new ObjectResult(new { Message = "Internal Server Error", Details = ex.Message }) { StatusCode = 500 };
        }
    }
    
    public virtual async Task<string> GetAppointmentByIdAsync(Guid appointmentId)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var requestUrl = $"{BASE_URL}/api/Appointments/{appointmentId}";

            var response = await client.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                return $"Error fetching appointment: {response.ReasonPhrase}";
            }
        }
        catch (Exception ex)
        {
            return $"Error 500: Internal Server Error - {ex.Message}";
        }
    }
    
    public virtual async Task<string> CreateAppointmentAsync(Appointment appointment)
    {
        
        if (appointment.Status != "Cancelled" && appointment.Status != "Upcoming" && appointment.Status != "Finished")
        {
            return "Error 400: Status must be 'Cancelled', 'Upcoming', or 'Finished'";
        }
        
        try
        {
            var client = _httpClientFactory.CreateClient();
            var requestUrl = $"{BASE_URL}/api/Appointments";

            var jsonContent = JsonConvert.SerializeObject(appointment);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(requestUrl, content);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                return $"Error creating appointment: {response.ReasonPhrase}";
            }
        }
        catch (Exception ex)
        {
            return $"Error 500: Internal Server Error - {ex.Message}";
        }
    }
    
    public virtual async Task<string> UpdateAppointmentAsync(Guid appointmentId, Appointment appointment)
    {
        
        if (appointment.Status != "Cancelled" && appointment.Status != "Upcoming" && appointment.Status != "Finished")
        {
            return "Error 400: Status must be 'Cancelled', 'Upcoming', or 'Finished'";
        }
        
        try
        {
            var client = _httpClientFactory.CreateClient();
            var requestUrl = $"{BASE_URL}/api/Appointments/{appointmentId}";

            var jsonContent = JsonConvert.SerializeObject(appointment);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await client.PutAsync(requestUrl, content);

            if (response.IsSuccessStatusCode)
            {
                return "Appointment updated successfully";
            }
            else
            {
                return $"Error updating appointment: {response.ReasonPhrase}";
            }
        }
        catch (Exception ex)
        {
            return $"Error 500: Internal Server Error - {ex.Message}";
        }
    }

    public virtual async Task<string> DeleteAppointmentAsync(Guid appointmentId)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var requestUrl = $"{BASE_URL}/api/Appointments/{appointmentId}";

            var response = await client.DeleteAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                return "Appointment deleted successfully";
            }
            else
            {
                return $"Error deleting appointment: {response.ReasonPhrase}";
            }
        }
        catch (Exception ex)
        {
            return $"Error 500: Internal Server Error - {ex.Message}";
        }
    }


}
