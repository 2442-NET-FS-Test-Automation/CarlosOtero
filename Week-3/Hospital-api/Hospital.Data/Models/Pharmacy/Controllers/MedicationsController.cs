using HospitalApi.DTOs.Pharmacy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HospitalApi.Models.Pharmacy;
using HospitalApi.Data;

namespace HospitalApi.Controllers.Pharmacy;

[ApiController]
[Route("api/pharmacy/[controller]")] // Generates endpoint path: api/pharmacy/medications
public class MedicationsController : ControllerBase
{
    private readonly HospitalDbContext _context;

    public MedicationsController(HospitalDbContext context)
    {
        _context = context;
    }

    // GET: api/pharmacy/medications - GET ALL MEDICATIONS
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MedicationDto>>> GetMedications()
    {
        var medications = await _context.Medications
            .Include(m => m.Inventory)
            .AsNoTracking()
            .Select(m => new MedicationDto(
                m.MedicationID,
                m.Name,
                m.GenericName,
                m.BrandName,
                m.DosageForm,
                m.Strength,
                m.UnitPrice,
                m.Inventory != null ? m.Inventory.StockQuantity : 0,
                m.Inventory != null ? m.Inventory.ExpiryDate : null
            ))
            .ToListAsync();

        return Ok(medications);
    }

    // GET: api/pharmacy/medications/5 - FIND BY ID
    [HttpGet("{id}")]
    public async Task<ActionResult<MedicationDto>> GetMedication(int id)
    {
        var m = await _context.Medications
            .Include(m => m.Inventory)
            .AsNoTracking()
            .Where(m => m.MedicationID == id)
            .Select(m => new MedicationDto(
                m.MedicationID,
                m.Name,
                m.GenericName,
                m.BrandName,
                m.DosageForm,
                m.Strength,
                m.UnitPrice,
                m.Inventory != null ? m.Inventory.StockQuantity : 0,
                m.Inventory != null ? m.Inventory.ExpiryDate : null
            ))
            .FirstOrDefaultAsync();

        if (m == null) return NotFound($"Medication with ID {id} was not found.");
        return Ok(m);
    }

    // POST: api/pharmacy/medications
    [HttpPost]
    public async Task<ActionResult<MedicationDto>> CreateMedication(CreateMedicationDto dto)
    {
        var medication = new Medication
        {
            Name = dto.Name,
            GenericName = dto.GenericName,
            BrandName = dto.BrandName,
            DosageForm = dto.DosageForm,
            Strength = dto.Strength,
            UnitPrice = dto.UnitPrice
        };

        _context.Medications.Add(medication);
        await _context.SaveChangesAsync();

        var responseDto = new MedicationDto(
            medication.MedicationID, medication.Name, medication.GenericName, medication.BrandName,
            medication.DosageForm, medication.Strength, medication.UnitPrice, 0, null
        );

        return CreatedAtAction(nameof(GetMedication), new { id = medication.MedicationID }, responseDto);
    }
}
