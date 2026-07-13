using Library.ControllersApi.Services;

namespace Library.ControllersApi;

public class SupplierClient : ISupplierClient
{
    
    // This class will call outside API using HTTP Client
    private readonly HttpClient _http; // Comes from ASP.NET DI Container

    public SupplierClient(HttpClient http)
    {
        _http = http;
    }

    // Record to represent the response "shape" of that outside API
    private record SupplierProduct(int id, string Title, decimal Price);

    // This method sends a GET to a training API called dummyjson
    // GET https://dummyjson.com/products/{id} -> This is live

    public async Task<decimal?> GetListPriceAsync(string sku)
    {
        // Let's pretend we are grabbing the "Wholesale price" of our products from the supplier
        var digits = new string(sku.Where(char.IsDigit).ToArray()); // "BK-001" -> "001"

        // Check to make sure we don't have a null in digits
        if(!int.TryParse(digits, out var id)) return null; // If our string was empty, just return null

        // appending the rest of the URL to the base URL we set up with builder.Services
        var product = await _http.GetFromJsonAsync<SupplierProduct>($"products/{id}");

        return product?.Price;
    }
}