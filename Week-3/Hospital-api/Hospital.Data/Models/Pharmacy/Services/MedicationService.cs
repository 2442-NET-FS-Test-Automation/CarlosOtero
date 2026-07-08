namespace HospitalApi.Models.Pharmacy.Services;

public interface MedicationService : IMedicationService
{
    static List<Medication> medications = new List<Medication>
    {
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
    };
}