using HospitalApi.Data;
using HospitalApi.DTOs.Pharmacy;
using HospitalApi.Models.Pharmacy.Services;
using HospitalApi.Services.Infrastructure;
using HospitalApi.Services.Pharmacy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HospitalApi.Controllers.Pharmacy;

[ApiController]
[Route("api/pharmacy/[controller]")] // Generates clean base endpoint path: api/pharmacy/medications
[Produces("application/json")]
public class MedicationsController : ControllerBase
{
    private readonly ISeederService _seederService;
    private readonly IMedicationService _service;

    public MedicationsController(IMedicationService service, ISeederService seederService)
    {
        _service = service;
        _seederService = seederService;
    }

    // GET: api/pharmacy/medications - GET ALL MEDICATIONS
    [HttpGet]
    public async Task<IActionResult> GetMedications()
    {
        var medications = await _service.GetAllMedicationsAsync();
        return Ok(medications);
    }

    [HttpPost("reset")]
    public async Task<IActionResult> ResetPharmacyDomain()
    {
        await _seederService.ResetDatabaseAsync();
        return Ok(new { message = "Catalog data reset." });
    }

    [HttpPost("add")]
    public async Task<IActionResult> CreateMedication([FromBody] CreateMedicationDto payload)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var createdEntity = await _service.CreateMedicationAsync(payload);

        // Map the created entity parameters back to a clean MedicationDto contract view response
        var responseDto = new MedicationDto(
            createdEntity.MedicationID,
            createdEntity.Name,
            createdEntity.GenericName,
            createdEntity.BrandName,
            createdEntity.DosageForm,
            createdEntity.Strength,
            createdEntity.UnitPrice,
            0,
            null
        );

        // Standard REST Pattern: Return a 201 Created status containing the resource location URL
        return CreatedAtAction(
            nameof(GetMedicationById),
            new { id = responseDto.MedicationID }, // Maps safely to your required DTO parameter spelling
            responseDto
        );
    }


    // GET: api/pharmacy/medications/5 - FIND BY ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetMedicationById(int id)
    {
        var medication = await _service.GetMedicationByIdAsync(id);

        if (medication == null) return NotFound($"Medication with ID {id} was not found.");
        return Ok(medication);
    }

}
