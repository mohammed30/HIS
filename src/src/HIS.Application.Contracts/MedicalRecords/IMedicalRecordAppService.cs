using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace HIS.MedicalRecords;

public interface IMedicalRecordAppService : IApplicationService
{
    Task<PatientMedicalSummaryDto> GetPatientMedicalSummaryAsync(Guid patientId);
    
    // Medical History
    Task<PagedResultDto<MedicalHistoryDto>> GetMedicalHistoryListAsync(Guid patientId, int skipCount = 0, int maxResultCount = 20);
    Task<MedicalHistoryDto> CreateMedicalHistoryAsync(CreateUpdateMedicalHistoryDto input);
    Task<MedicalHistoryDto> UpdateMedicalHistoryAsync(Guid id, CreateUpdateMedicalHistoryDto input);
    Task DeleteMedicalHistoryAsync(Guid id);
    
    // Diagnosis
    Task<PagedResultDto<DiagnosisDto>> GetDiagnosisListAsync(Guid patientId, int skipCount = 0, int maxResultCount = 20);
    Task<DiagnosisDto> CreateDiagnosisAsync(CreateUpdateDiagnosisDto input);
    Task<DiagnosisDto> UpdateDiagnosisAsync(Guid id, CreateUpdateDiagnosisDto input);
    Task DeleteDiagnosisAsync(Guid id);
    
    // Vital Signs
    Task<PagedResultDto<VitalSignDto>> GetVitalSignListAsync(Guid patientId, int skipCount = 0, int maxResultCount = 20);
    Task<VitalSignDto?> GetLatestVitalsAsync(Guid patientId);
    Task<VitalSignDto> CreateVitalSignAsync(CreateUpdateVitalSignDto input);
    Task DeleteVitalSignAsync(Guid id);
    
    // Allergies
    Task<PagedResultDto<AllergyDto>> GetAllergyListAsync(Guid patientId, bool activeOnly = false, int skipCount = 0, int maxResultCount = 20);
    Task<AllergyDto> CreateAllergyAsync(CreateUpdateAllergyDto input);
    Task<AllergyDto> UpdateAllergyAsync(Guid id, CreateUpdateAllergyDto input);
    Task DeleteAllergyAsync(Guid id);
    
    // Patient Notes
    Task<PagedResultDto<PatientNoteDto>> GetPatientNoteListAsync(Guid patientId, int skipCount = 0, int maxResultCount = 20);
    Task<PatientNoteDto> CreatePatientNoteAsync(CreateUpdatePatientNoteDto input);
    Task<PatientNoteDto> UpdatePatientNoteAsync(Guid id, CreateUpdatePatientNoteDto input);
    Task DeletePatientNoteAsync(Guid id);
}
