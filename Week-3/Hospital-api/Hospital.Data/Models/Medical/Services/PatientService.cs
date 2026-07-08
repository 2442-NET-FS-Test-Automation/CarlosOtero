namespace HospitalApi.Models.Pharmacy.Services;

public interface PatientService : IPatientService
{

// Seed Patients (Medical Domain)
static List<Patient> patients = new List<Patient>
{
    new Patient
    {
        PatientID = 1,
        FirstName = "John",
        LastName = "Doe",
        DateOfBirth = new DateTime(1985, 5, 12),
        Insurance = "Blue Cross",
        Gender = "Male",
        BloodType = "A+",
        Allergies = "Peanuts"
    },
    new Patient
    {
        PatientID = 2,
        FirstName = "Mary",
        LastName = "Jane",
        DateOfBirth = new DateTime(1992, 10, 22),
        Insurance = "Cigna",
        Gender = "Female",
        BloodType = "O-",
        Allergies = "Penicillin"
    },
    new Patient
    {
        PatientID = 3,
        FirstName = "Timmy",
        LastName = "Turner",
        DateOfBirth = new DateTime(2018, 3, 15),
        Insurance = "Medicaid",
        Gender = "Male",
        BloodType = "B+",
        Allergies = null
    }
};
}

