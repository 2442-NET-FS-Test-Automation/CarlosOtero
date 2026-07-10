using HospitalApi.DTOs.Pharmacy;
using HospitalApi.Models.Pharmacy;
using HospitalApi.Models.Pharmacy.Services;

namespace HospitalApi.Services.Pharmacy;

public class MedicationService : IMedicationService
{
    private readonly IMedicationRepository _repo;

    // Correctly structured constructor matching class initialization boundaries

    public MedicationService(IMedicationRepository repo)
    {
        _repo = repo;
    }
    private static readonly List<Medication> _medications = new()
    {
        new Medication { MedicationID = 1, Name = "Metoprolol Succinate", GenericName = "Metoprolol", BrandName = "Toprol XL", DosageForm = "Tablet", Strength = "50mg", UnitPrice = 0.75m },
        new Medication { MedicationID = 2, Name = "Amoxicillin Trihydrate", GenericName = "Amoxicillin", BrandName = "Amoxil", DosageForm = "Capsule", Strength = "500mg", UnitPrice = 0.40m },
        new Medication { MedicationID = 3, Name = "Ibuprofen", GenericName = "Ibuprofen", BrandName = "Advil", DosageForm = "Tablet", Strength = "400mg", UnitPrice = 0.15m }
    };
    // To verify your skeleton setup works before hooking up Entity Framework databases:
   public async Task<IEnumerable<MedicationDto>> GetAllMedicationsAsync()
    {
        // 1. Fetch live records from SQL Server (Includes the 1:1 Inventory table link natively)
        var dbEntities = await _repo.GetAllAsync();
        
        // 2. Project those live records directly into your clean transport DTOs
        var output = dbEntities.Select(m => new MedicationDto(
              m.MedicationID, 
              m.Name,
              m.GenericName,
              m.BrandName,
              m.DosageForm,
              m.Strength,
              m.UnitPrice,
              m.Inventory != null ? m.Inventory.StockQuantity : 0, // Calculates live stock count over the database wire
              m.Inventory != null ? m.Inventory.ExpiryDate : null
          ));

        return output;
    }

     public async Task<MedicationDto?> GetMedicationByIdAsync(int id)
    {
        // 1. Look up data against the repository
        var m = await _repo.GetByIdAsync(id);
        if (m == null) return null;

        // 2. Map and return the safe public representation contract model
        return new MedicationDto(
            m.MedicationID,
            m.Name,
            m.GenericName,
            m.BrandName,
            m.DosageForm,
            m.Strength,
            m.UnitPrice,
            m.Inventory != null ? m.Inventory.StockQuantity : 0,
            m.Inventory != null ? m.Inventory.ExpiryDate : null
        );
    }


   public async Task<Medication> CreateMedicationAsync(CreateMedicationDto dto)
    {
        var newMedication = new Medication
        {
            Name = dto.Name,
            GenericName = dto.GenericName,
            BrandName = dto.BrandName,
            DosageForm = dto.DosageForm,
            Strength = dto.Strength,
            UnitPrice = dto.UnitPrice
        };

        // 2. 🟢 FIXED: Pass the fully initialized object to the repository contract signature
        await _repo.AddAsync(newMedication);
        
        // 3. 🟢 FIXED: Commit and persist transaction variables explicitly down to SQL Server disk
        await _repo.SaveChangesAsync();

        return newMedication;
    }
}