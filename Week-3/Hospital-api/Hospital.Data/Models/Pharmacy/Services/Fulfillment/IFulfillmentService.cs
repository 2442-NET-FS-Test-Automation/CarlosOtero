using HospitalApi.DTOs.Pharmacy; 

namespace HospitalApi.Services.Pharmacy;

public enum FulfillmentResult { Fulfilled, Backordered }

public record BurstResult(int Fulfilled, int Backordered);

public interface IFulfillmentService
{
    Task<FulfillmentResult> FulfillOneAsync(int recordId, int inventoryID, int quantity, CancellationToken ct);
    
    Task<BurstResult> FulfillBurstAsync(IEnumerable<BurstRequestPayload> orders, CancellationToken ct); 
}