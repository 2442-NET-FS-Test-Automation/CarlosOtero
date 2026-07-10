using HospitalApi.Data;
using HospitalApi.DTOs.Medical;
using HospitalApi.Models.Medical;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalApi.Controllers.Medical;

[ApiController]
[Route("api/medical/[controller]")] 
[Produces("application/json")]
public class PatientController : ControllerBase
{
    private readonly HospitalDbContext _context;

    public PatientController(HospitalDbContext context)
    {
        _context = context;
    }

    [HttpGet] // GET - Gets a list of all patients
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<PatientDto>))]
    public async Task<ActionResult<IEnumerable<PatientDto>>> GetPatients()
    {
        var patients = await _context.Patients
            .AsNoTracking()
            .Select(p => new PatientDto(
                p.PatientID,
                $"{p.FirstName} {p.LastName}",
                p.DateOfBirth,
                p.Gender,
                p.BloodType,
                p.Allergies,
                p.Appointments.Count
            ))
            .ToListAsync();

        return Ok(patients);
    }

    [HttpPost] // POST - Adds a new patient profile
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(PatientDto))]
    public async Task<ActionResult<PatientDto>> CreatePatient(CreatePatientDto dto)
    {
        var patient = new Patient
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender,
            BloodType = dto.BloodType,
            Insurance = dto.Insurance,
            Allergies = dto.Allergies
        };

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        var output = new PatientDto(
            patient.PatientID, $"{patient.FirstName} {patient.LastName}", 
            patient.DateOfBirth, patient.Gender, patient.BloodType, patient.Allergies, 0
        );

        return CreatedAtAction(nameof(GetPatients), new { id = patient.PatientID }, output);
    }
}
