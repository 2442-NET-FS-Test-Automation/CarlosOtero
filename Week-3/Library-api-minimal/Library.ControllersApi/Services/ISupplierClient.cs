namespace Library.ControllersApi.Services;

public interface ISupplierClient
{
    Task<decimal?> GetListPriceAsync(string sku);
}