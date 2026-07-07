namespace HospitalApi.DTOs.Medical;

public record MedicalRecordDto(
    int RecordIDID,
    int PatientID,
    string PatientFullName,
    string? Diagnosis,
    string ClinicalNotes,
    DateTime VisitDate,
    bool IsEmergencyWalkIn
);
