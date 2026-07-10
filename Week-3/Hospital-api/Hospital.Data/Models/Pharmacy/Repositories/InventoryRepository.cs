using HospitalApi.Data;
using HospitalApi.DTOs.Pharmacy;
using HospitalApi.Models.Pharmacy;
using Microsoft.EntityFrameworkCore;

namespace HospitalApi.Repositories.Pharmacy;

public class InventoryRepository : IInventoryRepository
{


    private readonly IDbContextFactory<HospitalDbContext> _factory;


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


    public async Task AddInventoryItemAsync(InventoryItem inventoryItem)
    {
        await using var db = await _factory.CreateDbContextAsync();

        await db.Inventory.AddAsync(inventoryItem);
        await db.SaveChangesAsync();
    }


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