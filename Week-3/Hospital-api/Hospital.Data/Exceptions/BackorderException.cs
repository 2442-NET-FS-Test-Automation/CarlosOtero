namespace HospitalApi.Exceptions;

public class BackorderException : Exception
{
    public int RecordId { get; }
    public int InventoryId { get; }
    public int RequestedQuantity { get; }

    public BackorderException(int recordId, int inventoryId, int requested)
        : base($"Fulfillment failed and marked as backorder for Record: {recordId}, Inventory: {inventoryId}, Requested: {requested}.")
    {
        RecordId = recordId;
        InventoryId = inventoryId;
        RequestedQuantity = requested;
    }
}