namespace HospitalApi.DTOs.Medical;

public record AppointmentDtos(
    int AppointmentID,
    string? PatientFullName,
    int DoctorID,
    DateTime AppointmentTime,
    string ReasonForVisit,
    bool IsEmergencyWalkIn
);
