namespace Library.ControllersApi.Services;

using Library.ControllersApi.DTOs;
using Library.Data;
using Library.Data.Entities;
public class InventoryService : IInventoryService
{
    // Our InventoryService is what will call repo layer methods, so it
    // gets inventory

    private readonly IInventoryRepository _repo;

    public InventoryService(IInventoryRepository repo)
    {
        _repo = repo;
    }

    // When you first start writing your API, and you want to make sure DB access
    // is working, and get the skeleton/structure up - your methods will be very "Lean"
    // that's okay.

    public Task<IReadOnlyList<InventoryItem>> AllAsync()
    {
        // That's the method for now
        return _repo.GetAllAsync();
    }

    public Task<InventoryItem?> BySkuAsync(string sku)
    {
        return _repo.GetInventoryItemBySkuAsync(sku);
    }

     public Task<InventoryItem> AddAsync(InventoryCreateDto dto)
     {
         // This is going to need a DTO - we'll return to this
         return _repo.AddInventoryItemAsync(dto.Sku, dto.Name, dto.Price, dto.CurrentStock);
     }

    public Task<bool> RemoveAsync(string sku)
    {
        return _repo.RemoveBySkuAsync(sku);
    }
}