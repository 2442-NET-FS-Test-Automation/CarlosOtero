namespace HospitalApi.DTOs.Medical;

public record PatientDto(
    int Id,
    string FullName,
    DateTime DateOfBirth,
    string Gender,
    string BloodType,
    string? Allergies,
    int TotalAppointmentsLogged
);

public record CreatePatientDto(
    string FirstName,
    string LastName,
    DateTime DateOfBirth,
    string Gender,
    string BloodType,
    string? Insurance,
    string? Allergies
);
