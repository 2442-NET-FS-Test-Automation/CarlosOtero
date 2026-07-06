using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalApi.Models.Medical;

public class Appointment
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int PatientID { get; set; }

    [Required]
    public int DoctorID { get; set; } // Points to your inherited Staff system

    [Required]
    public DateTime AppointmentDate { get; set; }

    [Required]
    public TimeSpan AppointmentTime { get; set; }

    [StringLength(100)]
    public string? ReasonForVisit { get; set; }

    [Required, StringLength(20)]
    public string Status { get; set; } = "Scheduled";

    [ForeignKey(nameof(PatientID))]
    public Patient? Patient { get; set; }
}
