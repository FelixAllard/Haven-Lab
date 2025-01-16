namespace AppointmentsService.Entity;

public class Appointment
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string CustomerName { get; set; }  // New field
    public string CustomerEmail { get; set; } // New field
    public string Status { get; set; } // New field (should be one of the statuses: "Upcoming", "Cancelled", "Passed")
    public DateTime CreatedAt { get; set; }
}
