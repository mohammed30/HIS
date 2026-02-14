using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace HIS.Nursing;

public interface IPatientCareAppService : IApplicationService
{
    // Patient Rounds
    Task<PagedResultDto<PatientRoundDto>> GetPatientRoundsAsync(Guid patientId);
    Task<PatientRoundDto> CreatePatientRoundAsync(CreatePatientRoundDto input);
    
    // Pain Assessment
    Task<PagedResultDto<PainAssessmentDto>> GetPainAssessmentsAsync(Guid patientId);
    Task<PainAssessmentDto> CreatePainAssessmentAsync(CreatePainAssessmentDto input);
    
    // Fall Risk
    Task<PagedResultDto<FallRiskAssessmentDto>> GetFallRiskAssessmentsAsync(Guid patientId);
    Task<FallRiskAssessmentDto> CreateFallRiskAssessmentAsync(CreateFallRiskAssessmentDto input);
    
    // Wound Care
    Task<PagedResultDto<WoundCareDto>> GetWoundCaresAsync(Guid patientId);
    Task<WoundCareDto> CreateWoundCareAsync(CreateWoundCareDto input);
    
    // Shift Handover
    Task<PagedResultDto<ShiftHandoverDto>> GetShiftHandoversAsync(PagedResultRequestDto input);
    Task<ShiftHandoverDto> CreateShiftHandoverAsync(CreateShiftHandoverDto input);
}
