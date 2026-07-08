namespace Library.ControllersApi.Services;

using Library.ControllersApi.DTOs;
using Library.Data.Entities;

public interface IInventoryService
{
    public Task<IReadOnlyList<InventoryItem>> AllAsync();

    public Task<InventoryItem?> BySkuAsync(string sku);

    public Task<InventoryItem> AddAsync(InventoryCreateDto dto);

    public Task<bool> RemoveAsync(string sku);
}