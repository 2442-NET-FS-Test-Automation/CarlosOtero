using Hospital.Data;
using HospitalApi.DTOs;
using HospitalApi.DTOs.Medical;
using HospitalApi.Models.Medical;
namespace HospitalApi.Models.Pharmacy.Services;


public interface IMedicalRecordsService
{
    Task<IEnumerable<MedicalRecordDto>> GetMedicalRecords();

    Task<IEnumerable<MedicalRecordDto>> GetRecordsByPatientId(int patientId);

    Task<MedicalRecordDto> CreateMedicalRecord(MedicalRecord recordInput);
}