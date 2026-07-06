namespace HospitalApi.DTOs.Pharmacy;

public record MedicationDto(
    int Id,
    string Name,
    string GenericName,
    string BrandName,
    string DosageForm,
    string Strength,
    decimal UnitPrice,
    int? AvailableStock,
    DateTime? NextExpirationDate
);

public record CreateMedicationDto(
    string Name,
    string GenericName,
    string BrandName,
    string DosageForm,
    string Strength,
    decimal UnitPrice
);
