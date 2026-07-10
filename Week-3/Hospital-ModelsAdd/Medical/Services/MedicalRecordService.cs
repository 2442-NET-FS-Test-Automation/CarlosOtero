using Hospital.Data;
using HospitalApi.DTOs;
using HospitalApi.DTOs.Medical;
using HospitalApi.Models.Medical;

namespace HospitalApi.Models.Pharmacy.Services;

public interface MedicalRecordsService : IMedicalRecordsService
{
    static List<MedicalRecord> medicalRecords = new List<MedicalRecord>
{
        new MedicalRecord
        {
            RecordID = 1,
            PatientID = 1,
            DoctorID = 1,
            AppointmentID = 1, // Tied to a scheduled appointment
            VisitDate = new DateTime(2026, 7, 1),
            Symptoms = "Mild chest tightness during exercise",
            Diagnosis = "Stable Angina",
            TreatmentPlan = "Prescribed Beta-Blockers. Low sodium diet.",
            ClinicalNotes = "Patient responding well to lifestyle adjustments."
        },
        new MedicalRecord
        {
            RecordID = 2,
            PatientID = 2,
            DoctorID = 3,
            AppointmentID = null, // NULL represents an emergency ER walk-in encounter
            VisitDate = new DateTime(2026, 7, 2),
            Symptoms = "Acute appendicitis symptoms, severe lower right quadrant pain",
            Diagnosis = "Appendicitis",
            TreatmentPlan = "Emergency Appendectomy scheduled immediately",
            ClinicalNotes = "ER walk-in. Patient transferred directly to surgery floor."
        }
};
}