using Hospital.Data;
using HospitalApi.DTOs;
using HospitalApi.DTOs.Pharmacy;
using Microsoft.AspNetCore.Mvc;
namespace HospitalApi.Models.Pharmacy.Services;


public interface IMedicationService
{
    public Task<IEnumerable<MedicationDto>> GetAllMedicationsAsync();

    public Task<MedicationDto> GetMedicationByIdAsync(int id);

    public Task<Medication> CreateMedicationAsync(CreateMedicationDto dto);

}