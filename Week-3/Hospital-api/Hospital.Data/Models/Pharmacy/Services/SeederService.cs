using HospitalApi.Data;
using HospitalApi.Models.Pharmacy;
using Microsoft.EntityFrameworkCore;

namespace HospitalApi.Services.Infrastructure;

public class SeederService : ISeederService
{
    private readonly IDbContextFactory<HospitalDbContext> _factory;

    public SeederService(IDbContextFactory<HospitalDbContext> factory)
    {
        _factory = factory;
    }

    public async Task ResetDatabaseAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();

        await db.Database.ExecuteSqlRawAsync("TRUNCATE TABLE [Inventory]");
        await db.Database.ExecuteSqlRawAsync("TRUNCATE TABLE [PrescriptionDetails]");
        await db.Database.ExecuteSqlRawAsync("DELETE FROM [Medications]");
        await db.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Medications', RESEED, 0)");

        var defaultMedications = new List<Medication>
        {
            new Medication { MedicationID = 1, Name = "Metoprolol Succinate", GenericName = "Metoprolol", BrandName = "Toprol XL", DosageForm = "Tablet", Strength = "50mg", UnitPrice = 0.75m },
            new Medication { MedicationID = 2, Name = "Amoxicillin Trihydrate", GenericName = "Amoxicillin", BrandName = "Amoxil", DosageForm = "Capsule", Strength = "500mg", UnitPrice = 0.40m },
            new Medication { MedicationID = 3, Name = "Ibuprofen", GenericName = "Ibuprofen", BrandName = "Advil", DosageForm = "Tablet", Strength = "400mg", UnitPrice = 0.15m }
        };

        await db.Database.OpenConnectionAsync();
        using var transaction = await db.Database.BeginTransactionAsync();
        try
        {
            await db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [Medications] ON");

            await db.Medications.AddRangeAsync(defaultMedications);
            await db.SaveChangesAsync();

            await db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [Medications] OFF");


            var defaultInventory = new List<InventoryItem>
            {
                new InventoryItem { MedicationID = 1, BatchNumber = "BATCH-M12", StockQuantity = 500, ExpiryDate = new DateTime(2028, 12, 1), SupplierName = "PharmaCorp Inc" },
                new InventoryItem { MedicationID = 2, BatchNumber = "BATCH-A99", StockQuantity = 1200, ExpiryDate = new DateTime(2027, 6, 15), SupplierName = "Global Meds Dist" },
                new InventoryItem { MedicationID = 3, BatchNumber = "BATCH-I44", StockQuantity = 2000, ExpiryDate = new DateTime(2029, 1, 20), SupplierName = "PharmaCorp Inc" }
            };

            await db.Inventory.AddRangeAsync(defaultInventory);
            await db.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
        finally
        {
            await db.Database.CloseConnectionAsync();
        }
    }
}