using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalApi.Models.Pharmacy;

public class InventoryItem
{
    [Key]
    public int InventoryID { get; set; }

    [Required, StringLength(50)]
    public string BatchNumber { get; set; } = string.Empty;

    public int StockQuantity { get; set; }
    
    public DateTime ExpiryDate { get; set; }

    [Required, StringLength(100)]
    public string SupplierName { get; set; } = string.Empty;

    // Foreign Key backing field 
    public int MedicationId { get; set; }
    
    [ForeignKey(nameof(MedicationId))]
    public Medication? Medication { get; set; }
}
