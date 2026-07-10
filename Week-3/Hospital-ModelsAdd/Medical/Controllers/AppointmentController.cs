using HospitalApi.Data;
using HospitalApi.Models.Medical;
using HospitalApi.Models.Medical.Services;
using HospitalApi.Models.Pharmacy.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalApi.Controllers.Medical;

[ApiController]
[Route("api/medical/[controller]")] // Endpoint path: api/medical/appointments
[Produces("application/json")]
public class AppointmentsController : ControllerBase
{
    private readonly HospitalDbContext _context;
    private readonly IAppointmentService _service;

    public AppointmentsController(HospitalDbContext context, AppointmentService service)
    {
        _context = context;
        _service = service;
    }

    /// <summary>
    /// Retrieves all appointments scheduled for a specific date.
    /// </summary>
    [HttpGet("date/{date}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAppointmentsByDate(DateTime date)
    {
        var targetDate = date.Date;

        var appointments = await _context.Appointments
            .Include(a => a.Patient)
            .AsNoTracking()
            .Where(a => a.AppointmentDate == targetDate)
            .OrderBy(a => a.AppointmentTime)
            .Select(a => new {
                a.AppointmentID,
                PatientName = a.Patient != null ? $"{a.Patient.FirstName} {a.Patient.LastName}" : "Unknown",
                a.DoctorID,
                a.AppointmentTime,
                a.ReasonForVisit,
                a.Status
            })
            .ToListAsync();

        return Ok(appointments);
    }

    /// <summary>
    /// Books a brand new patient appointment slot after verifying data integrity.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BookAppointment(Appointment input)
    {
        // 1. Data Integrity Check: Ensure patient exists
        var patientExists = await _context.Patients.AnyAsync(p => p.PatientID == input.PatientID);
        if (!patientExists) return BadRequest("The requested Patient ID does not exist.");

        // 2. Schedule Check: Prevent double-booking the doctor at the exact same hour
        var isDoctorBusy = await _context.Appointments.AnyAsync(a => 
            a.DoctorID == input.DoctorID && 
            a.AppointmentDate == input.AppointmentDate && 
            a.AppointmentTime == input.AppointmentTime &&
            a.Status != "Cancelled");

        if (isDoctorBusy) return BadRequest("The requested doctor is already booked for this specific time slot.");

        input.Status = "Scheduled";
        _context.Appointments.Add(input);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAppointmentsByDate), new { date = input.AppointmentDate.ToString("yyyy-MM-dd") }, input);
    }

    /// <summary>
    /// Updates the status of an active appointment (e.g., Check-in, Cancel).
    /// </summary>
    [HttpPut("{id}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] string newStatus)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null) return NotFound($"Appointment ID {id} was not found.");

        appointment.Status = newStatus;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
