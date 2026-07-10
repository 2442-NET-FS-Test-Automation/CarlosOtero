namespace HospitalApi.Exceptions;

public class InventoryNotFoundException : Exception
{
    public int InventoryId { get; }

    public InventoryNotFoundException(int inventoryId)
        : base($"Target inventory identifier tracking token {inventoryId} could not be resolved inside the active database context.")
    {
        InventoryId = inventoryId;
    }
}