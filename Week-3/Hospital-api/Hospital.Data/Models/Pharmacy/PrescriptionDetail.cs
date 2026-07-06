using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalApi.Models.Pharmacy;

public class PrescriptionDetail
{
    [Key]
    public int Id { get; set; }

    // Scalable Foreign Key referencing external Medical Records domain
    [Required]
    public int RecordId { get; set; } 

    [Required]
    public int MedicationId { get; set; }

    [Required, StringLength(300)]
    public string DosageInstructions { get; set; } = string.Empty;

    public int DurationDays { get; set; }

    public int QuantityDispensed { get; set; }

    [ForeignKey(nameof(MedicationId))]
    public Medication? Medication { get; set; }
}
