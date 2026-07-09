namespace HospitalApi.DTOs.Pharmacy;

using HospitalApi.Data;
using HospitalApi.Models.Pharmacy;
using Microsoft.EntityFrameworkCore;


// This class will hold my DB access logic. ALL it is concerned wih is looking into the database
public class InventoryRepository : IInventoryRepository
{
    // Our repo class needs a db context - we can ask for a dbcontext from ASP.NET DI Container
    // Same pattern we've been using since day 1 of the minimal API

    private readonly IDbContextFactory<HospitalDbContext> _factory;

    // Still taking arguments in from ASP.NET during runtime.

    public InventoryRepository(IDbContextFactory<HospitalDbContext> factory)
    {
        _factory = factory;
    }

    // Let's make some CRUD
    // Actually pretty simple to do - because we don't have to concern ourselves with business logic checks etc.
    // All we write is DB access stuff

    // Let's write some Read methods
    // Get all inventoryItems

    public async Task<IReadOnlyList<InventoryItem>> GetAllAsync()
    {
        // Ask for db context
        await using var db = await _factory.CreateDbContextAsync();
        return await db.Inventory.Include(i=>i.Medication).ToListAsync();
    }

    // Get an item by its SKU
    public async Task<InventoryItem?> GetInventoryItemByIdAsync(int id)
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.Inventory.Include(i=>i.Medication).FirstOrDefaultAsync(i=>i.Medication.MedicationID == id);
    }

    // Let's do a simple add
    // 
    public async Task<InventoryItem> AddInventoryItemAsync(string name, string genericName, string brandName, string dosageForm, string strength, decimal unitPrice, int quantity)
    {
        await using var db = await _factory.CreateDbContextAsync();

        InventoryItem  newItem = new InventoryItem
        {
            Medication = new Medication {Name = name, GenericName = genericName, BrandName = brandName, DosageForm = dosageForm, Strength = strength, UnitPrice = unitPrice},
            StockQuantity = quantity
        };

        db.Inventory.Add(newItem);
        await db.SaveChangesAsync();
        return newItem; // because newItem is an object tracked by EF Core - EF will grab the PK for us
    }

    // Let's do a remove
    public async Task<bool> RemoveByIdAsync(int id)
    {
        await using var db = await _factory.CreateDbContextAsync();

        // First find the thing we want out of the database - grab it
        InventoryItem? itemToRemove = await db.Inventory.Include(i=>i.Medication).FirstOrDefaultAsync(i=>i.Medication.MedicationID == id);

        // Don't assume the search criteria produced a result - check for a null
        // If it's null we failed to remove it - because it didn't exist
        if (itemToRemove is null)
        {
            return false;
        }
        // Telling EF we want to remove this item from DB
        db.Medications.Remove(itemToRemove.Medication); // This should cascade based on how we setup our models

        await db.SaveChangesAsync();
        return true;

    }
}