namespace Hospital.Data.Entities;
using Microsoft.EntityFrameworkCore;

// Product will act as a DB model or entity. This class will be a 1:1 representation
// of the table + rows in the database.
public class Medication
{
    // Do not forget getters and setters
    public int Id { get; set; } = default!;
    public string Name { get; set; } = default!;

    public string GenericName { get; set; } = default!;
    // Using a data annotation to enforce a constraint on my column
    // In this case, 10 total digits, 2 after the decimal place

    public string BrandName { get; set; } = default!;
    public string DosageForm { get; set; } = default!;
    public string Strength { get; set; } = default!;
    [Precision(10,2)]
    public decimal UnitPrice { get; set; } = default!;

    //Below is an example of using a collection to denote a relationship
    // A product has an inventory item, an inventory item is associated with one product
    //1 : 1 relationship for now
    public InventoryItem? Inventory { get; set; }

}