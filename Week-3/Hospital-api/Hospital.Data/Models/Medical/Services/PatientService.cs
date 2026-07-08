namespace HospitalApi.Models.Pharmacy.Services;

public interface PatientService : IPatientService
{
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

