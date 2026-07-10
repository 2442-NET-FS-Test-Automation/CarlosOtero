using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalApi.Models.Medical;

public class MedicalRecord
{
    [Key]
    public int RecordID { get; set; }

    [Required]
    public int PatientID { get; set; }

    [Required]
    public int DoctorID { get; set; }

    // Nullable foreign key for emergency room walk-ins
    public int? AppointmentID { get; set; } 

    [Required]
    public DateTime VisitDate { get; set; } = DateTime.UtcNow;

    [StringLength(500)]
    public string? Symptoms { get; set; }

    [StringLength(500)]
    public string? Diagnosis { get; set; }

    [StringLength(500)]
    public string? TreatmentPlan { get; set; }

    [Required, StringLength(500)]
    public string ClinicalNotes { get; set; } = string.Empty;

    [ForeignKey(nameof(PatientID))]
    public Patient? Patient { get; set; }

    [ForeignKey(nameof(AppointmentID))]
    public Appointment? Appointment { get; set; }
}
