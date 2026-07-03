namespace Hospital.Data.Entities;

public class InventoryItem
{
    public int Id { get; set; }
    public int MedicationId { get; set; } //FK - 1-1 with product

    public string BatchNumber {get;set;} = default!;
    public int StockAmount { get; set; } // How many of this thing do we have

    public DateTime ExpirationDate {get;set;} = default!;

    public string Supplier {get;set;} = default!;
    public Medication Medication { get; set; } = default!;// We can have EF give a default value

    // Adding a RowVersion property - we will use this in OnModelCreation
    // We will use this to track concurrency

    public byte[] RowVersion {get;set;} = default!;
}