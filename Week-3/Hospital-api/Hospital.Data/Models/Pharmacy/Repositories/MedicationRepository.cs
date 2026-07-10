using HospitalApi.Data;
using HospitalApi.DTOs.Pharmacy;
using HospitalApi.Models.Pharmacy;
using Microsoft.EntityFrameworkCore;

namespace HospitalApi.Repositories.Pharmacy;
// This class will hold my DB access logic. ALL it is concerned wih is looking into the database
public class MedicationRepository : IMedicationRepository
{
    // Our repo class needs a db context - we can ask for a dbcontext from ASP.NET DI Container
    // Same pattern we've been using since day 1 of the minimal API

    private readonly IDbContextFactory<HospitalDbContext> _factory;

    // Still taking arguments in from ASP.NET during runtime.

    public MedicationRepository(IDbContextFactory<HospitalDbContext> factory)
    {
        _factory = factory;
    }

    // Let's make some CRUD
    // Actually pretty simple to do - because we don't have to concern ourselves with business logic checks etc.
    // All we write is DB access stuff

    // Let's write some Read methods
    // Get all inventoryItems

    public async Task<IReadOnlyList<Medication>> GetAllAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();

        return await db.Medications
            .Include(m => m.Inventory)
            .ToListAsync();
    }

    // Get an item by its SKU
    public async Task<Medication?> GetByIdAsync(int id)
    {
        await using var db = await _factory.CreateDbContextAsync();

        return await db.Medications
            .Include(m => m.Inventory)
            .FirstOrDefaultAsync(m => m.MedicationID == id);
    }

    public async Task AddAsync(Medication medication) 
    {
        await using var db = await _factory.CreateDbContextAsync();

        await db.Medications.AddAsync(medication);
        await db.SaveChangesAsync(); 
    }

    public async Task UpdateAsync(Medication medication)
    {
        await using var db = await _factory.CreateDbContextAsync();
        db.Entry(medication).State = EntityState.Modified;
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Medication medication)
    {
        await using var db = await _factory.CreateDbContextAsync();
        db.Medications.Remove(medication);
        await db.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.Medications.AnyAsync(m => m.MedicationID == id);
    }

    public async Task SaveChangesAsync()
    {
        // Fully satisfied internally inside methods via factory instantiation lifetimes pattern loops
        await Task.CompletedTask;
    }

}