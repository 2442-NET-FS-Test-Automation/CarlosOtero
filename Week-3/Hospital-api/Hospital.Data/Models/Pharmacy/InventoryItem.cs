using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalApi.Models.Pharmacy;

public class InventoryItem
{
    [Key]
    public int InventoryID { get; set; }

    public int MedicationID { get; set; }
    public Medication? Medication { get; set; }
    
    [Required, StringLength(50)]
    public string BatchNumber { get; set; } = string.Empty;

    public int StockQuantity { get; set; }
    
    public DateTime ExpiryDate { get; set; }

    [Required, StringLength(100)]
    public string SupplierName { get; set; } = string.Empty;

    public byte[] RowVersion { get; set; } = default!;
}
