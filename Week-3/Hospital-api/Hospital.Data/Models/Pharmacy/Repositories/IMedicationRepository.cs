using HospitalApi.Models.Pharmacy;

namespace HospitalApi.DTOs.Pharmacy;

public interface IMedicationRepository
{
    public Task<IReadOnlyList<Medication>> GetAllAsync();
    public Task<Medication?> GetByIdAsync(int id);
    public  Task AddAsync(Medication medication);

    public Task UpdateAsync(Medication medication);
    public  Task DeleteAsync(Medication medication);

    public Task<bool> ExistsAsync(int id);
    public Task SaveChangesAsync();
}