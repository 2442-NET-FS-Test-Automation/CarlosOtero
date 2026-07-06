using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
namespace HospitalApi.Models.Pharmacy;

public class Medication
{
    [Key]
    public int MedicationID { get; set; }
    
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string GenericName { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string BrandName { get; set; } = string.Empty;

    [Required, StringLength(30)]
    public string DosageForm { get; set; } = string.Empty;

    [Required, StringLength(20)]
    public string Strength { get; set; } = string.Empty;

    [Precision(10, 2)]
    public decimal UnitPrice { get; set; }

    // Scalable Navigation Property: 1-to-1 connection to physical tracking stock
    public InventoryItem? Inventory { get; set; }

    // Scalable Navigation Property: 1-to-Many connection to prescription logs
    public ICollection<PrescriptionDetail> PrescriptionDetails { get; set; } = new List<PrescriptionDetail>();
}
