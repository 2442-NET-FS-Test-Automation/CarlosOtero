using Hospital.Data;
using HospitalApi.DTOs;
using HospitalApi.DTOs.Medical;
namespace HospitalApi.Models.Medical.Services;


public interface IPatientService
{
    Task<IEnumerable<PatientDto>> GetPatients();

    Task<PatientDto> CreatePatient(CreatePatientDto dto);

}