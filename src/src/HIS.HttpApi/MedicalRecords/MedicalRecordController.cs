using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace HIS.MedicalRecords;

[RemoteService(Name = "MedicalRecord")]
[Area("app")]
[Route("api/app/medical-records")]
public class MedicalRecordController : AbpControllerBase
{
    private readonly IMedicalRecordAppService _medicalRecordAppService;

    public MedicalRecordController(IMedicalRecordAppService medicalRecordAppService)
    {
        _medicalRecordAppService = medicalRecordAppService;
    }

    [HttpGet("summary/{patientId}")]
    public Task<PatientMedicalSummaryDto> GetSummaryAsync(Guid patientId)
        => _medicalRecordAppService.GetPatientMedicalSummaryAsync(patientId);

    // Vital Signs
    [HttpGet("vital-signs/{patientId}")]
    public Task<PagedResultDto<VitalSignDto>> GetVitalSignsAsync(Guid patientId, int skipCount = 0, int maxResultCount = 20)
        => _medicalRecordAppService.GetVitalSignListAsync(patientId, skipCount, maxResultCount);

    [HttpGet("vital-signs/{patientId}/latest")]
    public Task<VitalSignDto?> GetLatestVitalsAsync(Guid patientId)
        => _medicalRecordAppService.GetLatestVitalsAsync(patientId);

    [HttpPost("vital-signs")]
    public Task<VitalSignDto> CreateVitalSignAsync([FromBody] CreateUpdateVitalSignDto input)
        => _medicalRecordAppService.CreateVitalSignAsync(input);

    [HttpDelete("vital-signs/{id}")]
    public Task DeleteVitalSignAsync(Guid id)
        => _medicalRecordAppService.DeleteVitalSignAsync(id);

    // Diagnoses
    [HttpGet("diagnoses/{patientId}")]
    public Task<PagedResultDto<DiagnosisDto>> GetDiagnosesAsync(Guid patientId, int skipCount = 0, int maxResultCount = 20)
        => _medicalRecordAppService.GetDiagnosisListAsync(patientId, skipCount, maxResultCount);

    [HttpPost("diagnoses")]
    public Task<DiagnosisDto> CreateDiagnosisAsync([FromBody] CreateUpdateDiagnosisDto input)
        => _medicalRecordAppService.CreateDiagnosisAsync(input);

    [HttpPut("diagnoses/{id}")]
    public Task<DiagnosisDto> UpdateDiagnosisAsync(Guid id, [FromBody] CreateUpdateDiagnosisDto input)
        => _medicalRecordAppService.UpdateDiagnosisAsync(id, input);

    [HttpDelete("diagnoses/{id}")]
    public Task DeleteDiagnosisAsync(Guid id)
        => _medicalRecordAppService.DeleteDiagnosisAsync(id);

    // Allergies
    [HttpGet("allergies/{patientId}")]
    public Task<PagedResultDto<AllergyDto>> GetAllergiesAsync(Guid patientId, bool activeOnly = false, int skipCount = 0, int maxResultCount = 20)
        => _medicalRecordAppService.GetAllergyListAsync(patientId, activeOnly, skipCount, maxResultCount);

    [HttpPost("allergies")]
    public Task<AllergyDto> CreateAllergyAsync([FromBody] CreateUpdateAllergyDto input)
        => _medicalRecordAppService.CreateAllergyAsync(input);

    [HttpPut("allergies/{id}")]
    public Task<AllergyDto> UpdateAllergyAsync(Guid id, [FromBody] CreateUpdateAllergyDto input)
        => _medicalRecordAppService.UpdateAllergyAsync(id, input);

    [HttpDelete("allergies/{id}")]
    public Task DeleteAllergyAsync(Guid id)
        => _medicalRecordAppService.DeleteAllergyAsync(id);

    // Medical History
    [HttpGet("history/{patientId}")]
    public Task<PagedResultDto<MedicalHistoryDto>> GetMedicalHistoryAsync(Guid patientId, int skipCount = 0, int maxResultCount = 20)
        => _medicalRecordAppService.GetMedicalHistoryListAsync(patientId, skipCount, maxResultCount);

    [HttpPost("history")]
    public Task<MedicalHistoryDto> CreateMedicalHistoryAsync([FromBody] CreateUpdateMedicalHistoryDto input)
        => _medicalRecordAppService.CreateMedicalHistoryAsync(input);

    [HttpPut("history/{id}")]
    public Task<MedicalHistoryDto> UpdateMedicalHistoryAsync(Guid id, [FromBody] CreateUpdateMedicalHistoryDto input)
        => _medicalRecordAppService.UpdateMedicalHistoryAsync(id, input);

    [HttpDelete("history/{id}")]
    public Task DeleteMedicalHistoryAsync(Guid id)
        => _medicalRecordAppService.DeleteMedicalHistoryAsync(id);

    // Patient Notes
    [HttpGet("notes/{patientId}")]
    public Task<PagedResultDto<PatientNoteDto>> GetNotesAsync(Guid patientId, int skipCount = 0, int maxResultCount = 20)
        => _medicalRecordAppService.GetPatientNoteListAsync(patientId, skipCount, maxResultCount);

    [HttpPost("notes")]
    public Task<PatientNoteDto> CreateNoteAsync([FromBody] CreateUpdatePatientNoteDto input)
        => _medicalRecordAppService.CreatePatientNoteAsync(input);

    [HttpPut("notes/{id}")]
    public Task<PatientNoteDto> UpdateNoteAsync(Guid id, [FromBody] CreateUpdatePatientNoteDto input)
        => _medicalRecordAppService.UpdatePatientNoteAsync(id, input);

    [HttpDelete("notes/{id}")]
    public Task DeleteNoteAsync(Guid id)
        => _medicalRecordAppService.DeletePatientNoteAsync(id);
}
