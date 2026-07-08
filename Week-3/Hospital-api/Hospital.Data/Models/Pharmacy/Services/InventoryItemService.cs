namespace HospitalApi.Models.Pharmacy.Services;

public interface InventoryItemService : IInventoryItemService
{
    static List<InventoryItem> inventoryItems = new List<InventoryItem>
    {
        new InventoryItem
        {
        InventoryID = 1, 
            MedicationId = 1, 
            BatchNumber = "BATCH-M12", 
            StockQuantity = 500, 
            ExpiryDate = new DateTime(2028, 12, 1), 
            SupplierName = "PharmaCorp Inc" 
        },
        new InventoryItem
        {
    InventoryID = 2, 
            MedicationId = 2, 
            BatchNumber = "BATCH-A99", 
            StockQuantity = 1200, 
            ExpiryDate = new DateTime(2027, 6, 15), 
            SupplierName = "Global Meds Dist" 
        },
        new InventoryItem
        {
            InventoryID = 3,
            MedicationId = 3,
            BatchNumber = "BATCH-I44",
            StockQuantity = 2000,
            ExpiryDate = new DateTime(2029, 1, 20),
            SupplierName = "PharmaCorp Inc"
        }
    };
}