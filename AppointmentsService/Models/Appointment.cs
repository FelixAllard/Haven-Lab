using System.ComponentModel.DataAnnotations.Schema;

namespace AppointmentsService.Entity;

public class Appointment
{
    [Column("id")]
    public int Id { get; set; }

    [Column("appointment_id")]
    public Guid AppointmentId { get; set; }  // UUID for the new AppointmentId
    
    [Column("title")]
    public string Title { get; set; }
    
    [Column("description")]
    public string Description { get; set; }
    
    [Column("appointment_date")]
    public DateTime AppointmentDate { get; set; }
    
    [Column("customer_name")]
    public string CustomerName { get; set; }
    
    [Column("customer_email")]
    public string CustomerEmail { get; set; }
    
    [Column("status")]
    public string Status { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}
