namespace HospitalApi.Exceptions;

public class BackorderException : Exception
{
    public int RecordId { get; }
    public int InventoryID { get; }
    public int RequestedQuantity { get; }

    public BackorderException(int recordId, int inventoryID, int requested)
        : base($"Fulfillment failed and marked as backorder for Record: {recordId}, Inventory: {inventoryID}, Requested: {requested}.")
    {
        RecordId = recordId;
        InventoryID = inventoryID;
        RequestedQuantity = requested;
    }
}