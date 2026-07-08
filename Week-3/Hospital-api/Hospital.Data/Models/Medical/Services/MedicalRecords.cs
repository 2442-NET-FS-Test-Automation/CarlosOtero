namespace HospitalApi.Models.Pharmacy.Services;

public interface MedicalRecordsService : IMedicalRecordsService
{
        // Seed Appointments (Medical Domain - Links back to Patient IDs)
    // Note: DoctorID is mapped to mock values (1, 2) which would correspond to your Staff table.
    modelBuilder.Entity<Appointment>().HasData(
        new Appointment 
        { 
            AppointmentID = 1, 
            PatientID = 1, 
            DoctorID = 1, 
            AppointmentDate = new DateTime(2026, 7, 1), 
            AppointmentTime = new TimeSpan(9, 30, 0), 
            ReasonForVisit = "Chest pain follow-up", 
            Status = "Completed" 
        },
        new Appointment 
        { 
            AppointmentID = 2, 
            PatientID = 3, 
            DoctorID = 2, 
            AppointmentDate = new DateTime(2026, 7, 5), 
            AppointmentTime = new TimeSpan(14, 0, 0), 
            ReasonForVisit = "Routine pediatric checkup", 
            Status = "Scheduled" 
        }
    );

    // Seed MedicalRecords (Medical Domain)
    modelBuilder.Entity<MedicalRecord>().HasData(
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
}
}