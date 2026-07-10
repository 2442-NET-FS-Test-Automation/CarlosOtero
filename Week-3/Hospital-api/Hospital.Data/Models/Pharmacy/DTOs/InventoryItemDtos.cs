namespace HospitalApi.DTOs.Pharmacy;

public record InventoryItemDto(
    int Id,
    int MedicationId,
    string BatchNumber,
    int StockQuantity,
    DateTime ExpiryDate,
    string SupplierName
);

public record CreateInventoryItemDto(
    int StockQuantity,
    string BatchNumber,
    DateTime ExpiryDate,
    string SupplierName
);

public record SupplierReportDto(
    string SupplierName,
    int TotalBatchesTracked,
    int TotalStockAvailable
);
public record BurstRequestPayload(int AppointmentID, int InventoryID, int QuantityRequested);