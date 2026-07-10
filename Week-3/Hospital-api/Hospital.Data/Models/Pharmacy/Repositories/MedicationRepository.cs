using HospitalApi.Data;
using HospitalApi.DTOs.Pharmacy;
using HospitalApi.Models.Pharmacy;
using Microsoft.EntityFrameworkCore;

namespace HospitalApi.Repositories.Pharmacy;

public class MedicationRepository : IMedicationRepository
{


    private readonly IDbContextFactory<HospitalDbContext> _factory;


    public MedicationRepository(IDbContextFactory<HospitalDbContext> factory)
    {
        _factory = factory;
    }


    public async Task<IReadOnlyList<Medication>> GetAllAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();

        return await db.Medications
            .Include(m => m.Inventory)
            .ToListAsync();
    }

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

        await Task.CompletedTask;
    }

}