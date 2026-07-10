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
        var newItem = new InventoryItem
        {
            MedicationID = medicationId,
            BatchNumber = dto.BatchNumber,
            StockQuantity = dto.StockQuantity,
            ExpiryDate = DateTime.UtcNow.AddYears(2), 
            SupplierName = dto.SupplierName
        };

        await _repo.AddInventoryItemAsync(newItem);

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