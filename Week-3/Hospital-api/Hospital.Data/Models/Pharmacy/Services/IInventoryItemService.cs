using Hospital.Data;
using HospitalApi.DTOs;
using HospitalApi.DTOs.Pharmacy;
namespace HospitalApi.Models.Pharmacy;


public interface IInventoryItemService
{
    
    Task<IReadOnlyList<InventoryItemDto>> AllAsync();

    Task<InventoryItemDto?> ByIdAsync(int id);
    
    Task<InventoryItemDto> AddInventoryAsync(int medicationId, CreateInventoryItemDto dto);
    Task<bool> RemoveAsync(int id);

}