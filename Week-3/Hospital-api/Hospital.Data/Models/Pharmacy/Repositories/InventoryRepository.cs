using HospitalApi.Data;
using HospitalApi.DTOs.Pharmacy;
using HospitalApi.Models.Pharmacy;
using Microsoft.EntityFrameworkCore;

namespace HospitalApi.Repositories.Pharmacy;

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


    public async Task<IReadOnlyList<InventoryItem>> GetAllAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.Inventory
            .Include(i => i.Medication)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<InventoryItem?> GetInventoryItemByIdAsync(int id)
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.Inventory
            .Include(i => i.Medication)
            .FirstOrDefaultAsync(i => i.InventoryID == id);
    }

    // Let's do a simple add
    // 
    public async Task AddInventoryItemAsync(InventoryItem inventoryItem)
    {
        await using var db = await _factory.CreateDbContextAsync();

        await db.Inventory.AddAsync(inventoryItem);
        await db.SaveChangesAsync();
    }

    // Let's do a remove
    public async Task<bool> RemoveByIdAsync(int id)
    {
        await using var db = await _factory.CreateDbContextAsync();

        InventoryItem? itemToRemove = await db.Inventory
            .FirstOrDefaultAsync(i => i.InventoryID == id);

        if (itemToRemove is null)
        {
            return false;
        }

        db.Inventory.Remove(itemToRemove);
        await db.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.Inventory.AnyAsync(i => i.InventoryID == id);
    }

    public async Task SaveChangesAsync()
    {
        await Task.CompletedTask;
    }
}