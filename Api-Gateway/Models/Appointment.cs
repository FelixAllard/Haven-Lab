using System.ComponentModel.DataAnnotations.Schema;

namespace Api_Gateway.Models;

public class Appointment
{
    public Guid AppointmentId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string CustomerName { get; set; }
    public string CustomerEmail { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
