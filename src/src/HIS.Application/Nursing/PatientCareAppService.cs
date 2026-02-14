using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HIS.Patients;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace HIS.Nursing;

[Authorize]
public class PatientCareAppService : HISAppService, IPatientCareAppService
{
    private readonly IRepository<PatientRound, Guid> _patientRoundRepository;
    private readonly IRepository<PainAssessment, Guid> _painAssessmentRepository;
    private readonly IRepository<FallRiskAssessment, Guid> _fallRiskAssessmentRepository;
    private readonly IRepository<WoundCare, Guid> _woundCareRepository;
    private readonly IRepository<ShiftHandover, Guid> _shiftHandoverRepository;

    public PatientCareAppService(
        IRepository<PatientRound, Guid> patientRoundRepository,
        IRepository<PainAssessment, Guid> painAssessmentRepository,
        IRepository<FallRiskAssessment, Guid> fallRiskAssessmentRepository,
        IRepository<WoundCare, Guid> woundCareRepository,
        IRepository<ShiftHandover, Guid> shiftHandoverRepository)
    {
        _patientRoundRepository = patientRoundRepository;
        _painAssessmentRepository = painAssessmentRepository;
        _fallRiskAssessmentRepository = fallRiskAssessmentRepository;
        _woundCareRepository = woundCareRepository;
        _shiftHandoverRepository = shiftHandoverRepository;
    }

    // --- Patient Rounds ---
    public async Task<PagedResultDto<PatientRoundDto>> GetPatientRoundsAsync(Guid patientId)
    {
        var query = await _patientRoundRepository.GetQueryableAsync();
        var rounds = await AsyncExecuter.ToListAsync(
            query.Where(x => x.PatientId == patientId).OrderByDescending(x => x.CreationTime)
        );

        return new PagedResultDto<PatientRoundDto>(
            rounds.Count,
            ObjectMapper.Map<List<PatientRound>, List<PatientRoundDto>>(rounds)
        );
    }

    public async Task<PatientRoundDto> CreatePatientRoundAsync(CreatePatientRoundDto input)
    {
        var round = ObjectMapper.Map<CreatePatientRoundDto, PatientRound>(input);
        round.NurseId = CurrentUser.Id;
        await _patientRoundRepository.InsertAsync(round);
        return ObjectMapper.Map<PatientRound, PatientRoundDto>(round);
    }

    // --- Pain Assessment ---
    public async Task<PagedResultDto<PainAssessmentDto>> GetPainAssessmentsAsync(Guid patientId)
    {
        var query = await _painAssessmentRepository.GetQueryableAsync();
        var assessments = await AsyncExecuter.ToListAsync(
            query.Where(x => x.PatientId == patientId).OrderByDescending(x => x.AssessmentTime)
        );

        return new PagedResultDto<PainAssessmentDto>(
            assessments.Count,
            ObjectMapper.Map<List<PainAssessment>, List<PainAssessmentDto>>(assessments)
        );
    }

    public async Task<PainAssessmentDto> CreatePainAssessmentAsync(CreatePainAssessmentDto input)
    {
        var assessment = ObjectMapper.Map<CreatePainAssessmentDto, PainAssessment>(input);
        await _painAssessmentRepository.InsertAsync(assessment);
        return ObjectMapper.Map<PainAssessment, PainAssessmentDto>(assessment);
    }

    // --- Fall Risk Assessment ---
    public async Task<PagedResultDto<FallRiskAssessmentDto>> GetFallRiskAssessmentsAsync(Guid patientId)
    {
        var query = await _fallRiskAssessmentRepository.GetQueryableAsync();
        var assessments = await AsyncExecuter.ToListAsync(
            query.Where(x => x.PatientId == patientId).OrderByDescending(x => x.AssessmentTime)
        );

        return new PagedResultDto<FallRiskAssessmentDto>(
            assessments.Count,
            ObjectMapper.Map<List<FallRiskAssessment>, List<FallRiskAssessmentDto>>(assessments)
        );
    }

    public async Task<FallRiskAssessmentDto> CreateFallRiskAssessmentAsync(CreateFallRiskAssessmentDto input)
    {
        var assessment = ObjectMapper.Map<CreateFallRiskAssessmentDto, FallRiskAssessment>(input);
        
        // Simple logic calculate score or leave it to frontend/user?
        // Let's assume input has raw data and we calculate TotalScore & RiskLevel
        int score = 0;
        if (input.HistoryOfFalls) score += 25;
        if (input.SecondaryDiagnosis) score += 15;
        if (input.AmbulatoryAid) score += 15; // Simplified
        if (input.IVTherapy) score += 20;
        if (input.GaitProblem) score += 10; // Simplified
        if (input.MentalStatusIssue) score += 15;

        assessment.TotalScore = score;
        if (score < 25) assessment.RiskLevel = RiskLevel.Low;
        else if (score < 51) assessment.RiskLevel = RiskLevel.Medium;
        else assessment.RiskLevel = RiskLevel.High;

        await _fallRiskAssessmentRepository.InsertAsync(assessment);
        return ObjectMapper.Map<FallRiskAssessment, FallRiskAssessmentDto>(assessment);
    }

    // --- Wound Care ---
    public async Task<PagedResultDto<WoundCareDto>> GetWoundCaresAsync(Guid patientId)
    {
        var query = await _woundCareRepository.GetQueryableAsync();
        var cares = await AsyncExecuter.ToListAsync(
            query.Where(x => x.PatientId == patientId).OrderByDescending(x => x.AssessmentTime)
        );

        return new PagedResultDto<WoundCareDto>(
            cares.Count,
            ObjectMapper.Map<List<WoundCare>, List<WoundCareDto>>(cares)
        );
    }

    public async Task<WoundCareDto> CreateWoundCareAsync(CreateWoundCareDto input)
    {
        var care = ObjectMapper.Map<CreateWoundCareDto, WoundCare>(input);
        await _woundCareRepository.InsertAsync(care);
        return ObjectMapper.Map<WoundCare, WoundCareDto>(care);
    }

    // --- Shift Handover ---
    public async Task<PagedResultDto<ShiftHandoverDto>> GetShiftHandoversAsync(PagedResultRequestDto input)
    {
        // This usually should filter by something (e.g. date, or incoming nurse)
        // For now returning last 10
        var query = await _shiftHandoverRepository.GetQueryableAsync();
        var items = await AsyncExecuter.ToListAsync(
            query.OrderByDescending(x => x.HandoverTime).Take(input.MaxResultCount)
        );

        return new PagedResultDto<ShiftHandoverDto>(
            items.Count,
            ObjectMapper.Map<List<ShiftHandover>, List<ShiftHandoverDto>>(items)
        );
    }

    public async Task<ShiftHandoverDto> CreateShiftHandoverAsync(CreateShiftHandoverDto input)
    {
        var handover = ObjectMapper.Map<CreateShiftHandoverDto, ShiftHandover>(input);
        handover.OutgoingNurseId = CurrentUser.Id ?? Guid.Empty;
        handover.HandoverTime = DateTime.Now;
        await _shiftHandoverRepository.InsertAsync(handover);
        return ObjectMapper.Map<ShiftHandover, ShiftHandoverDto>(handover);
    }
}
