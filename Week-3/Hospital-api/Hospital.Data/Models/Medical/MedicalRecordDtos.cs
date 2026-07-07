namespace HospitalApi.DTOs.Medical;

public record MedicalRecordDto(
    int Id,
    int PatientId,
    string PatientFullName,
    string? Diagnosis,
    string ClinicalNotes,
    DateTime VisitDate,
    bool IsEmergencyWalkIn
);
