using HospitalApi.Models.Pharmacy;

namespace HospitalApi.DTOs.Pharmacy;

public interface IInventoryRepository
{
    // For now we'll leave this blank
    public Task<IReadOnlyList<InventoryItem>> GetAllAsync();
    public Task<InventoryItem?> GetInventoryItemByIdAsync(int id);
    public Task AddInventoryItemAsync(InventoryItem inventoryItem);
    public Task<bool> RemoveByIdAsync(int id);
    public Task<bool> ExistsAsync(int id);
    public Task SaveChangesAsync();
}