using System.ComponentModel.DataAnnotations;
using HospitalApi.Models.Pharmacy; // Access to prescription links if needed later

namespace HospitalApi.Models.Medical;

public class Patient
{
    [Key]
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    public DateTime DateOfBirth { get; set; }

    [Required, StringLength(10)]
    public string Gender { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Insurance { get; set; }

    [Required, StringLength(5)]
    public string BloodType { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Allergies { get; set; }

    // Navigation properties for cascading scale
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
}
