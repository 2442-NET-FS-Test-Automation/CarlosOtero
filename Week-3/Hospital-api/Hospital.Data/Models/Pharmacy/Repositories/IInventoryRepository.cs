using HospitalApi.Models.Pharmacy;

namespace HospitalApi.DTOs.Pharmacy;

public interface IInventoryRepository
{
    // For now we'll leave this blank
    Task<IReadOnlyList<InventoryItem>> GetAllAsync();
    Task<InventoryItem?> GetInventoryItemByIdAsync(int id);
    Task<InventoryItem> AddInventoryItemAsync(string name, string genericName, string brandName, string dosageForm, string strength, decimal unitPrice, int quantity);
    Task<bool> RemoveByIdAsync(int id);
}