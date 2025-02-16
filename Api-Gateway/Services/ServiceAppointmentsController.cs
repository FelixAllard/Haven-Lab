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
        BASE_URL = Environment.GetEnvironmentVariable("BASE_URL_APPOINTMENT_API") ?? "http://localhost:5114";
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
    
    public virtual async Task<IActionResult> GetAppointmentByIdAsync(Guid appointmentId)
    {
        try
        {
            // Validate the appointment ID
            if (appointmentId == Guid.Empty)
            {
                return new BadRequestObjectResult(new 
                { 
                    Message = "ServiceAppointmentController: Invalid appointment ID",
                    Details = "The provided appointment ID is empty or invalid." 
                });
            }

            // Create the HTTP client and request URL
            var client = _httpClientFactory.CreateClient();
            var requestUrl = $"{BASE_URL}/api/Appointments/{appointmentId}";

            // Send the GET request
            var response = await client.GetAsync(requestUrl);

            // Handle the response
            if (response.IsSuccessStatusCode)
            {
                var appointment = await response.Content.ReadFromJsonAsync<Appointment>();
                return new OkObjectResult(appointment);
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)  // Handle 404 Not Found
            {
                return new NotFoundObjectResult(new
                {
                    Message = "ServiceAppointmentController: Appointment Not Found",
                    Details = $"No appointment found with the ID: {appointmentId}"
                });
            }
            else if ((int)response.StatusCode >= 400 && (int)response.StatusCode < 500)
            {
                return new BadRequestObjectResult(new 
                { 
                    Message = "ServiceAppointmentController: Client Error",
                    Details = response.ReasonPhrase 
                });
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
            return new ObjectResult(new 
            { 
                Message = "Internal Server Error", 
                Details = ex.Message 
            }) 
            { 
                StatusCode = 500 
            };
        }
    }
    
    public virtual async Task<IActionResult> CreateAppointmentAsync(Appointment appointment)
    {
        // Validate the appointment object
        if (appointment == null)
        {
            return new BadRequestObjectResult(new 
            { 
                Message = "ServiceAppointmentController: Invalid request",
                Details = "Appointment data is required." 
            });
        }

        // Validate the status
        if (appointment.Status != "Cancelled" && appointment.Status != "Upcoming" && appointment.Status != "Finished")
        {
            return new BadRequestObjectResult(new 
            { 
                Message = "ServiceAppointmentController: Invalid status",
                Details = "Status must be 'Cancelled', 'Upcoming', or 'Finished'." 
            });
        }

        try
        {
            // Create the HTTP client and request URL
            var client = _httpClientFactory.CreateClient();
            var requestUrl = $"{BASE_URL}/api/Appointments";

            // Serialize the appointment object to JSON
            var jsonContent = JsonConvert.SerializeObject(appointment);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Send the POST request
            var response = await client.PostAsync(requestUrl, content);

            // Handle the response
            if (response.IsSuccessStatusCode)
            {
                var createdAppointment = await response.Content.ReadFromJsonAsync<Appointment>();
                return new CreatedResult($"{BASE_URL}/api/Appointments/{createdAppointment.AppointmentId}", createdAppointment);
            }
            else if ((int)response.StatusCode >= 400 && (int)response.StatusCode < 500)
            {
                return new BadRequestObjectResult(new 
                { 
                    Message = "ServiceAppointmentController: Client Error",
                    Details = response.ReasonPhrase 
                });
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
            return new ObjectResult(new 
            { 
                Message = "ServiceAppointmentController: Internal Server Error", 
                Details = ex.Message 
            }) 
            { 
                StatusCode = 500 
            };
        }
    }
        
    public virtual async Task<IActionResult> UpdateAppointmentAsync(Guid appointmentId, Appointment appointment)
    {
        // Validate the appointment object
        if (appointment == null)
        {
            return new BadRequestObjectResult(new 
            { 
                Message = "Invalid request",
                Details = "Appointment data is required." 
            });
        }

        // Validate the appointment ID
        if (appointmentId == Guid.Empty)
        {
            return new BadRequestObjectResult(new 
            { 
                Message = "Invalid appointment ID",
                Details = "The provided appointment ID is empty or invalid." 
            });
        }

        // Validate the status
        if (appointment.Status != "Cancelled" && appointment.Status != "Upcoming" && appointment.Status != "Finished")
        {
            return new BadRequestObjectResult(new 
            { 
                Message = "Invalid status",
                Details = "Status must be 'Cancelled', 'Upcoming', or 'Finished'." 
            });
        }

        try
        {
            // Create the HTTP client and request URL
            var client = _httpClientFactory.CreateClient();
            var requestUrl = $"{BASE_URL}/api/Appointments/{appointmentId}";

            // Serialize the appointment object to JSON
            var jsonContent = JsonConvert.SerializeObject(appointment);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Send the PUT request
            var response = await client.PutAsync(requestUrl, content);

            // Handle the response
            if (response.IsSuccessStatusCode)
            {
                return new OkObjectResult(new 
                { 
                    Message = "Appointment updated successfully",
                    Details = appointment.AppointmentId 
                });
            }
            else if ((int)response.StatusCode >= 400 && (int)response.StatusCode < 500)
            {
                return new BadRequestObjectResult(new 
                { 
                    Message = "Client Error",
                    Details = response.ReasonPhrase 
                });
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
            return new ObjectResult(new 
            { 
                Message = "Internal Server Error", 
                Details = ex.Message 
            }) 
            { 
                StatusCode = 500 
            };
        }
    }

    public virtual async Task<IActionResult> DeleteAppointmentAsync(Guid appointmentId)
    {
        // Validate the appointment ID
        if (appointmentId == Guid.Empty)
        {
            return new BadRequestObjectResult(new 
            { 
                Message = "Invalid appointment ID",
                Details = "The provided appointment ID is empty or invalid." 
            });
        }

        try
        {
            // Create the HTTP client and request URL
            var client = _httpClientFactory.CreateClient();
            var requestUrl = $"{BASE_URL}/api/Appointments/{appointmentId}";

            // Send the DELETE request
            var response = await client.DeleteAsync(requestUrl);

            // Handle the response
            if (response.StatusCode == HttpStatusCode.NoContent) 
            {
                return new NoContentResult();
            }
            else if (response.IsSuccessStatusCode) 
            {
                return new OkObjectResult(new 
                { 
                    Message = "Appointment deleted successfully" 
                });
            }
            else if ((int)response.StatusCode >= 400 && (int)response.StatusCode < 500)
            {
                return new BadRequestObjectResult(new 
                { 
                    Message = "Client Error",
                    Details = response.ReasonPhrase 
                });
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
            return new ObjectResult(new 
            { 
                Message = "Internal Server Error", 
                Details = ex.Message 
            }) 
            { 
                StatusCode = 500 
            };
        }
    }


}
