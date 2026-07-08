using HospitalApi.Data;
using HospitalApi.DTOs.Medical;
using HospitalApi.Models.Medical;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalApi.Controllers.Medical;

[ApiController]
[Route("api/medical/[controller]")] // Generates endpoint path: api/medical/medicalrecords
[Produces("application/json")]
public class MedicalRecordsController : ControllerBase
{
    private readonly HospitalDbContext _context;

    public MedicalRecordsController(HospitalDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves a list of all clinical medical records mapped directly to custom DTO definitions.
    /// </summary>
    /// <response code="200">Returns the complete list of patient clinical charts.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<MedicalRecordDto>))]
    public async Task<ActionResult<IEnumerable<MedicalRecordDto>>> GetMedicalRecords()
    {
        var records = await _context.MedicalRecords
            .Include(r => r.Patient)
            .AsNoTracking() // EF Core 10 Optimization: Disables object graph caching for speed
            .Select(r => new MedicalRecordDto(
                r.RecordID,
                r.PatientID,
                r.Patient != null ? $"{r.Patient.FirstName} {r.Patient.LastName}" : "Unknown Patient",
                r.Diagnosis,
                r.ClinicalNotes,
                r.VisitDate,
                r.AppointmentID == null // True if it is an emergency walk-in, False if scheduled
            ))
            .ToListAsync();

        return Ok(records);
    }

    /// <summary>
    /// Retrieves a specific patient's medical timeline by their unique Patient ID.
    /// </summary>
    /// <param name="patientId">The unique structural identifier of the target patient.</param>
    /// <response code="200">Returns the specific chart logs associated with the patient.</response>
    /// <response code="404">Returned if no matching patient logs exist in the system database context.</response>
    [HttpGet("patient/{patientId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<MedicalRecordDto>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<MedicalRecordDto>>> GetRecordsByPatientId(int patientId)
    {
        // First check if the patient exists to provide a clean API response
        var patientExists = await _context.Patients.AnyAsync(p => p.PatientID == patientId);
        if (!patientExists) return NotFound($"Patient with ID {patientId} does not exist.");

        var records = await _context.MedicalRecords
            .Include(r => r.Patient)
            .AsNoTracking()
            .Where(r => r.PatientID == patientId)
            .OrderByDescending(r => r.VisitDate) // Displays the newest clinical notes first
            .Select(r => new MedicalRecordDto(
                r.RecordID,
                r.PatientID,
                r.Patient != null ? $"{r.Patient.FirstName} {r.Patient.LastName}" : "Unknown Patient",
                r.Diagnosis,
                r.ClinicalNotes,
                r.VisitDate,
                r.AppointmentID == null
            ))
            .ToListAsync();

        return Ok(records);
    }

    /// <summary>
    /// Creates and logs a fresh clinical medical record.
    /// </summary>
    /// <remarks>
    /// If logging a walk-in or emergency treatment, leave the AppointmentID parameter null.
    /// </remarks>
    /// <response code="201">Returns the newly generated clinical chart metadata profile.</response>
    /// <response code="400">Returned if foreign key parent constraints fail verification.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(MedicalRecordDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MedicalRecordDto>> CreateMedicalRecord(MedicalRecord recordInput)
    {
        // Data Integrity Step: Confirm target patient exists before creating an orphaned entry
        var targetPatient = await _context.Patients.FindAsync(recordInput.PatientID);
        if (targetPatient == null) return BadRequest($"Foreign key violation: Patient ID {recordInput.PatientID} does not exist.");

        // Data Integrity Step: Confirm target appointment exists if one was explicitly passed
        if (recordInput.AppointmentID.HasValue)
        {
            var appointmentExists = await _context.Appointments.AnyAsync(a => a.AppointmentID == recordInput.AppointmentID.Value);
            if (!appointmentExists) return BadRequest($"Foreign key violation: Appointment ID {recordInput.AppointmentID.Value} does not exist.");
        }

        // Force Utc time tracking for seamless scaling across cloud server clusters
        recordInput.VisitDate = DateTime.UtcNow;

        _context.MedicalRecords.Add(recordInput);
        await _context.SaveChangesAsync();

        var outputDto = new MedicalRecordDto(
            recordInput.RecordID,
            recordInput.PatientID,
            $"{targetPatient.FirstName} {targetPatient.LastName}",
            recordInput.Diagnosis,
            recordInput.ClinicalNotes,
            recordInput.VisitDate,
            recordInput.AppointmentID == null
        );

        // Uses a safe reference to your index get endpoint method
        return CreatedAtAction(nameof(GetMedicalRecords), new { id = recordInput.RecordID }, outputDto);
    }
}
