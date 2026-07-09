using Hospital.Data;
using HospitalApi.DTOs;
using Microsoft.AspNetCore.Mvc;
namespace HospitalApi.Models.Medical.Services;


public interface IAppointmentService
{
    Task<IActionResult> UpdateStatus(int id, [FromBody] string newStatus);
    Task<IActionResult> BookAppointment(Appointment input);
    Task<IActionResult> GetAppointmentsByDate(DateTime date);

}