using HospitalApi.Models.Pharmacy;
using HospitalApi.Models.Medical;
using Microsoft.EntityFrameworkCore;
namespace HospitalApi.Data;

public class HospitalDbContext : DbContext
{
    public HospitalDbContext(DbContextOptions<HospitalDbContext> options) : base(options) { }

    public DbSet<Medication> Medications => Set<Medication>();
    public DbSet<InventoryItem> Inventory => Set<InventoryItem>();
    public DbSet<PrescriptionDetail> PrescriptionDetails => Set<PrescriptionDetail>();

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<MedicalRecord> MedicalRecords => Set<MedicalRecord>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Medication>()
            .HasOne(m => m.Inventory)
            .WithOne(i => i.Medication)
            .HasForeignKey<InventoryItem>(i => i.MedicationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Medication>()
            .HasMany(m => m.PrescriptionDetails)
            .WithOne(p => p.Medication)
            .HasForeignKey(p => p.MedicationId)
            .OnDelete(DeleteBehavior.Restrict); 

        modelBuilder.Entity<Patient>()
            .HasMany(p => p.Appointments)
            .WithOne(a => a.Patient)
            .HasForeignKey(a => a.PatientID)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Patient>()
            .HasMany(p => p.MedicalRecords)
            .WithOne(m => m.Patient)
            .HasForeignKey(m => m.PatientID)
            .OnDelete(DeleteBehavior.Restrict);
        
            modelBuilder.Entity<Medication>().HasData(
        new Medication 
        { 
            MedicationID = 1, 
            Name = "Metoprolol Succinate", 
            GenericName = "Metoprolol", 
            BrandName = "Toprol XL", 
            DosageForm = "Tablet", 
            Strength = "50mg", 
            UnitPrice = 0.75m 
        },
        new Medication 
        { 
            MedicationID = 2, 
            Name = "Amoxicillin Trihydrate", 
            GenericName = "Amoxicillin", 
            BrandName = "Amoxil", 
            DosageForm = "Capsule", 
            Strength = "500mg", 
            UnitPrice = 0.40m 
        },
        new Medication 
        { 
            MedicationID = 3, 
            Name = "Ibuprofen", 
            GenericName = "Ibuprofen", 
            BrandName = "Advil", 
            DosageForm = "Tablet", 
            Strength = "400mg", 
            UnitPrice = 0.15m 
        }
    );

    // Seed InventoryItems (Pharmacy Domain - 1:1 with Medication)
    modelBuilder.Entity<InventoryItem>().HasData(
        new InventoryItem 
        { 
            InventoryID = 1, 
            MedicationId = 1, 
            BatchNumber = "BATCH-M12", 
            StockQuantity = 500, 
            ExpiryDate = new DateTime(2028, 12, 1), 
            SupplierName = "PharmaCorp Inc" 
        },
        new InventoryItem 
        { 
            InventoryID = 2, 
            MedicationId = 2, 
            BatchNumber = "BATCH-A99", 
            StockQuantity = 1200, 
            ExpiryDate = new DateTime(2027, 6, 15), 
            SupplierName = "Global Meds Dist" 
        },
        new InventoryItem 
        { 
            InventoryID = 3, 
            MedicationId = 3, 
            BatchNumber = "BATCH-I44", 
            StockQuantity = 2000, 
            ExpiryDate = new DateTime(2029, 1, 20), 
            SupplierName = "PharmaCorp Inc" 
        }
    );

    // Seed Patients (Medical Domain)
    modelBuilder.Entity<Patient>().HasData(
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
    );

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
    );
    }

    
}
