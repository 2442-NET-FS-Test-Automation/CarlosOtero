using Hospital.Data;
using HospitalApi.DTOs;
using HospitalApi.DTOs.Pharmacy;
namespace HospitalApi.Models.Pharmacy.Services;


public interface IMedicationService
{
    Task<IEnumerable<MedicationDto>> GetMedications();

    Task<MedicationDto> GetMedicationById(int id);

    Task<MedicationDto> CreateMedication(CreateMedicationDto dto);

    


}