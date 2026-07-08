using Hospital.Data;
using HospitalApi.DTOs;
using HospitalApi.DTOs.Medical;
namespace HospitalApi.Models.Pharmacy.Services;


public interface IPatientService
{
    Task<IEnumerable<PatientDto>> GetPatients();

    Task<PatientDto> CreatePatient(CreatePatientDto dto);

}