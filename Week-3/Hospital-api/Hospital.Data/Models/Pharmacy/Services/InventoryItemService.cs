using HospitalApi.DTOs.Pharmacy;
using HospitalApi.Models.Pharmacy;
using HospitalApi.Repositories.Pharmacy;

namespace HospitalApi.Services.Pharmacy;

public class InventoryItemService : IInventoryItemService
{
    private readonly IInventoryRepository _repo;

    public InventoryItemService(IInventoryRepository repo)
    {
        _repo = repo;
    }

    public async Task<IReadOnlyList<InventoryItemDto>> AllAsync()
    {
        var entities = await _repo.GetAllAsync();

        return entities.Select(i => new InventoryItemDto(
            i.InventoryID,
            i.MedicationID,
            i.BatchNumber,
            i.StockQuantity,
            i.ExpiryDate,
            i.SupplierName
        )).ToList();
    }

    public async Task<InventoryItemDto?> ByIdAsync(int id)
    {
        var i = await _repo.GetInventoryItemByIdAsync(id);
        if (i == null) return null;

        return new InventoryItemDto(
            i.InventoryID,
            i.MedicationID,
            i.BatchNumber,
            i.StockQuantity,
            i.ExpiryDate,
            i.SupplierName
        );
    }

    public async Task<InventoryItemDto> AddInventoryAsync(int medicationId, CreateInventoryItemDto dto)
    {
        // 1. Map your incoming request properties onto a fresh domain tracking entity card
        var newItem = new InventoryItem
        {
            MedicationID = medicationId,
            BatchNumber = dto.BatchNumber,
            StockQuantity = dto.StockQuantity,
            ExpiryDate = DateTime.UtcNow.AddYears(2), // Automatic default shelf life initialization
            SupplierName = dto.SupplierName
        };

        // 2. 🟢 FIXED: Call your factory repository exactly ONCE to handle the entire database insert sequence
        await _repo.AddInventoryItemAsync(newItem);

        // 3. 🟢 FIXED: Return the DTO instantly. DO NOT call any other saving method in this method scope!
        return new InventoryItemDto(
            newItem.InventoryID,
            newItem.MedicationID,
            newItem.BatchNumber,
            newItem.StockQuantity,
            newItem.ExpiryDate,
            newItem.SupplierName
        );
    }

    public async Task<bool> RemoveAsync(int id)
    {
        var itemExists = await _repo.ExistsAsync(id);
        if (!itemExists) return false;

        await _repo.RemoveByIdAsync(id);
        return true;
    }
}