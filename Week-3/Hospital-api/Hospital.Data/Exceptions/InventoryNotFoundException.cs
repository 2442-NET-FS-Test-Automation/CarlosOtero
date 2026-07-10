namespace HospitalApi.Exceptions;

public class InventoryNotFoundException : Exception
{
    public int InventoryID { get; }

    public InventoryNotFoundException(int inventoryID)
        : base($"Target inventory identifier tracking token {inventoryID} could not be resolved inside the active database context.")
    {
        InventoryID = inventoryID;
    }
}