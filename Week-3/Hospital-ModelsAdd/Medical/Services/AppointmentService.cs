using Hospital.Data;
using HospitalApi.DTOs;
using HospitalApi.DTOs.Medical;
using Microsoft.AspNetCore.Mvc;
using HospitalApi.Models.Medical;
using HospitalApi.Models.Medical.Services;

namespace HospitalApi.Models.Pharmacy.Services;

public interface AppointmentService : IAppointmentService
{
    static List<Appointment> inventoryItems = new List<Appointment>
    {
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
    };


}