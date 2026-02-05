using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace HIS.MedicalRecords;

public class MedicalRecordAppService : ApplicationService, IMedicalRecordAppService
{
    private readonly IRepository<MedicalHistory, Guid> _medicalHistoryRepository;
    private readonly IRepository<Diagnosis, Guid> _diagnosisRepository;
    private readonly IRepository<VitalSign, Guid> _vitalSignRepository;
    private readonly IRepository<Allergy, Guid> _allergyRepository;
    private readonly IRepository<PatientNote, Guid> _patientNoteRepository;
    private readonly IRepository<Patients.Patient, Guid> _patientRepository;

    public MedicalRecordAppService(
        IRepository<MedicalHistory, Guid> medicalHistoryRepository,
        IRepository<Diagnosis, Guid> diagnosisRepository,
        IRepository<VitalSign, Guid> vitalSignRepository,
        IRepository<Allergy, Guid> allergyRepository,
        IRepository<PatientNote, Guid> patientNoteRepository,
        IRepository<Patients.Patient, Guid> patientRepository)
    {
        _medicalHistoryRepository = medicalHistoryRepository;
        _diagnosisRepository = diagnosisRepository;
        _vitalSignRepository = vitalSignRepository;
        _allergyRepository = allergyRepository;
        _patientNoteRepository = patientNoteRepository;
        _patientRepository = patientRepository;
    }

    #region Patient Medical Summary

    public async Task<PatientMedicalSummaryDto> GetPatientMedicalSummaryAsync(Guid patientId)
    {
        var patient = await _patientRepository.GetAsync(patientId);
        
        var allergies = await _allergyRepository.GetListAsync(x => x.PatientId == patientId && x.Status == AllergyStatus.Active);
        var chronicConditions = await _medicalHistoryRepository.GetListAsync(x => x.PatientId == patientId && x.IsChronic);
        var activeDiagnoses = await _diagnosisRepository.CountAsync(x => x.PatientId == patientId && x.Status == DiagnosisStatus.Active);

        var allVitals = await _vitalSignRepository.GetListAsync(x => x.PatientId == patientId);
        var latestVitals = allVitals.OrderByDescending(x => x.RecordedAt).FirstOrDefault();

        return new PatientMedicalSummaryDto
        {
            PatientId = patientId,
            PatientName = patient.FullNameAr,
            BloodType = patient.BloodType,
            Age = patient.Age,
            Gender = patient.Gender,
            ActiveAllergiesCount = allergies.Count,
            ChronicConditionsCount = chronicConditions.Count,
            ActiveDiagnosesCount = (int)activeDiagnoses,
            LatestVitals = latestVitals != null ? ObjectMapper.Map<VitalSign, VitalSignDto>(latestVitals) : null,
            ActiveAllergies = ObjectMapper.Map<List<Allergy>, List<AllergyDto>>(allergies),
            ChronicConditions = ObjectMapper.Map<List<MedicalHistory>, List<MedicalHistoryDto>>(chronicConditions)
        };
    }

    #endregion

    #region Medical History CRUD

    public async Task<PagedResultDto<MedicalHistoryDto>> GetMedicalHistoryListAsync(Guid patientId, int skipCount = 0, int maxResultCount = 20)
    {
        var totalCount = await _medicalHistoryRepository.CountAsync(x => x.PatientId == patientId);
        var items = await _medicalHistoryRepository.GetListAsync(x => x.PatientId == patientId);
        var pagedItems = items.OrderByDescending(x => x.DiagnosedDate).Skip(skipCount).Take(maxResultCount).ToList();
        
        return new PagedResultDto<MedicalHistoryDto>(totalCount, ObjectMapper.Map<List<MedicalHistory>, List<MedicalHistoryDto>>(pagedItems));
    }

    public async Task<MedicalHistoryDto> CreateMedicalHistoryAsync(CreateUpdateMedicalHistoryDto input)
    {
        var entity = new MedicalHistory(GuidGenerator.Create(), input.PatientId, input.ConditionAr)
        {
            ConditionEn = input.ConditionEn,
            ICD10Code = input.ICD10Code,
            DiagnosedDate = input.DiagnosedDate,
            ResolvedDate = input.ResolvedDate,
            IsChronic = input.IsChronic,
            Notes = input.Notes
        };
        await _medicalHistoryRepository.InsertAsync(entity);
        return ObjectMapper.Map<MedicalHistory, MedicalHistoryDto>(entity);
    }

    public async Task<MedicalHistoryDto> UpdateMedicalHistoryAsync(Guid id, CreateUpdateMedicalHistoryDto input)
    {
        var entity = await _medicalHistoryRepository.GetAsync(id);
        entity.ConditionAr = input.ConditionAr;
        entity.ConditionEn = input.ConditionEn;
        entity.ICD10Code = input.ICD10Code;
        entity.DiagnosedDate = input.DiagnosedDate;
        entity.ResolvedDate = input.ResolvedDate;
        entity.IsChronic = input.IsChronic;
        entity.Notes = input.Notes;
        await _medicalHistoryRepository.UpdateAsync(entity);
        return ObjectMapper.Map<MedicalHistory, MedicalHistoryDto>(entity);
    }

    public async Task DeleteMedicalHistoryAsync(Guid id) => await _medicalHistoryRepository.DeleteAsync(id);

    #endregion

    #region Diagnosis CRUD

    public async Task<PagedResultDto<DiagnosisDto>> GetDiagnosisListAsync(Guid patientId, int skipCount = 0, int maxResultCount = 20)
    {
        var totalCount = await _diagnosisRepository.CountAsync(x => x.PatientId == patientId);
        var items = await _diagnosisRepository.GetListAsync(x => x.PatientId == patientId);
        var pagedItems = items.OrderByDescending(x => x.DiagnosisDate).Skip(skipCount).Take(maxResultCount).ToList();
        
        return new PagedResultDto<DiagnosisDto>(totalCount, ObjectMapper.Map<List<Diagnosis>, List<DiagnosisDto>>(pagedItems));
    }

    public async Task<DiagnosisDto> CreateDiagnosisAsync(CreateUpdateDiagnosisDto input)
    {
        var entity = new Diagnosis(GuidGenerator.Create(), input.PatientId, input.DiagnosisNameAr, input.DiagnosisDate)
        {
            VisitId = input.VisitId,
            ICD10Code = input.ICD10Code,
            DiagnosisNameEn = input.DiagnosisNameEn,
            Type = input.Type,
            Status = input.Status,
            DiagnosedById = CurrentUser.Id,
            DiagnosedByName = CurrentUser.Name,
            Notes = input.Notes
        };
        await _diagnosisRepository.InsertAsync(entity);
        return ObjectMapper.Map<Diagnosis, DiagnosisDto>(entity);
    }

    public async Task<DiagnosisDto> UpdateDiagnosisAsync(Guid id, CreateUpdateDiagnosisDto input)
    {
        var entity = await _diagnosisRepository.GetAsync(id);
        entity.ICD10Code = input.ICD10Code;
        entity.DiagnosisNameAr = input.DiagnosisNameAr;
        entity.DiagnosisNameEn = input.DiagnosisNameEn;
        entity.DiagnosisDate = input.DiagnosisDate;
        entity.Type = input.Type;
        entity.Status = input.Status;
        entity.Notes = input.Notes;
        await _diagnosisRepository.UpdateAsync(entity);
        return ObjectMapper.Map<Diagnosis, DiagnosisDto>(entity);
    }

    public async Task DeleteDiagnosisAsync(Guid id) => await _diagnosisRepository.DeleteAsync(id);

    #endregion

    #region Vital Signs CRUD

    public async Task<PagedResultDto<VitalSignDto>> GetVitalSignListAsync(Guid patientId, int skipCount = 0, int maxResultCount = 20)
    {
        var totalCount = await _vitalSignRepository.CountAsync(x => x.PatientId == patientId);
        var items = await _vitalSignRepository.GetListAsync(x => x.PatientId == patientId);
        var pagedItems = items.OrderByDescending(x => x.RecordedAt).Skip(skipCount).Take(maxResultCount).ToList();
        
        return new PagedResultDto<VitalSignDto>(totalCount, ObjectMapper.Map<List<VitalSign>, List<VitalSignDto>>(pagedItems));
    }

    public async Task<VitalSignDto?> GetLatestVitalsAsync(Guid patientId)
    {
        var items = await _vitalSignRepository.GetListAsync(x => x.PatientId == patientId);
        var latest = items.OrderByDescending(x => x.RecordedAt).FirstOrDefault();
        return latest != null ? ObjectMapper.Map<VitalSign, VitalSignDto>(latest) : null;
    }

    public async Task<VitalSignDto> CreateVitalSignAsync(CreateUpdateVitalSignDto input)
    {
        var entity = new VitalSign(GuidGenerator.Create(), input.PatientId, input.RecordedAt)
        {
            VisitId = input.VisitId,
            Temperature = input.Temperature,
            BloodPressureSystolic = input.BloodPressureSystolic,
            BloodPressureDiastolic = input.BloodPressureDiastolic,
            HeartRate = input.HeartRate,
            RespiratoryRate = input.RespiratoryRate,
            OxygenSaturation = input.OxygenSaturation,
            Weight = input.Weight,
            Height = input.Height,
            RecordedById = CurrentUser.Id,
            RecordedByName = CurrentUser.Name,
            Notes = input.Notes
        };
        await _vitalSignRepository.InsertAsync(entity);
        return ObjectMapper.Map<VitalSign, VitalSignDto>(entity);
    }

    public async Task DeleteVitalSignAsync(Guid id) => await _vitalSignRepository.DeleteAsync(id);

    #endregion

    #region Allergy CRUD

    public async Task<PagedResultDto<AllergyDto>> GetAllergyListAsync(Guid patientId, bool activeOnly = false, int skipCount = 0, int maxResultCount = 20)
    {
        var totalCount = activeOnly 
            ? await _allergyRepository.CountAsync(x => x.PatientId == patientId && x.Status == AllergyStatus.Active)
            : await _allergyRepository.CountAsync(x => x.PatientId == patientId);
            
        var items = activeOnly 
            ? await _allergyRepository.GetListAsync(x => x.PatientId == patientId && x.Status == AllergyStatus.Active)
            : await _allergyRepository.GetListAsync(x => x.PatientId == patientId);
            
        var pagedItems = items.OrderBy(x => x.AllergenNameAr).Skip(skipCount).Take(maxResultCount).ToList();
        
        return new PagedResultDto<AllergyDto>(totalCount, ObjectMapper.Map<List<Allergy>, List<AllergyDto>>(pagedItems));
    }

    public async Task<AllergyDto> CreateAllergyAsync(CreateUpdateAllergyDto input)
    {
        var entity = new Allergy(GuidGenerator.Create(), input.PatientId, input.AllergenType, input.AllergenNameAr)
        {
            AllergenNameEn = input.AllergenNameEn,
            Reaction = input.Reaction,
            Severity = input.Severity,
            OnsetDate = input.OnsetDate,
            Status = input.Status,
            Notes = input.Notes
        };
        await _allergyRepository.InsertAsync(entity);
        return ObjectMapper.Map<Allergy, AllergyDto>(entity);
    }

    public async Task<AllergyDto> UpdateAllergyAsync(Guid id, CreateUpdateAllergyDto input)
    {
        var entity = await _allergyRepository.GetAsync(id);
        entity.AllergenType = input.AllergenType;
        entity.AllergenNameAr = input.AllergenNameAr;
        entity.AllergenNameEn = input.AllergenNameEn;
        entity.Reaction = input.Reaction;
        entity.Severity = input.Severity;
        entity.OnsetDate = input.OnsetDate;
        entity.Status = input.Status;
        entity.Notes = input.Notes;
        await _allergyRepository.UpdateAsync(entity);
        return ObjectMapper.Map<Allergy, AllergyDto>(entity);
    }

    public async Task DeleteAllergyAsync(Guid id) => await _allergyRepository.DeleteAsync(id);

    #endregion

    #region Patient Notes CRUD

    public async Task<PagedResultDto<PatientNoteDto>> GetPatientNoteListAsync(Guid patientId, int skipCount = 0, int maxResultCount = 20)
    {
        var totalCount = await _patientNoteRepository.CountAsync(x => x.PatientId == patientId && !x.IsPrivate);
        var items = await _patientNoteRepository.GetListAsync(x => x.PatientId == patientId && !x.IsPrivate);
        var pagedItems = items.OrderByDescending(x => x.CreationTime).Skip(skipCount).Take(maxResultCount).ToList();
        
        return new PagedResultDto<PatientNoteDto>(totalCount, ObjectMapper.Map<List<PatientNote>, List<PatientNoteDto>>(pagedItems));
    }

    public async Task<PatientNoteDto> CreatePatientNoteAsync(CreateUpdatePatientNoteDto input)
    {
        var entity = new PatientNote(GuidGenerator.Create(), input.PatientId, input.Title, input.Content)
        {
            VisitId = input.VisitId,
            NoteType = input.NoteType,
            IsPrivate = input.IsPrivate,
            CreatedById = CurrentUser.Id,
            CreatedByName = CurrentUser.Name
        };
        await _patientNoteRepository.InsertAsync(entity);
        return ObjectMapper.Map<PatientNote, PatientNoteDto>(entity);
    }

    public async Task<PatientNoteDto> UpdatePatientNoteAsync(Guid id, CreateUpdatePatientNoteDto input)
    {
        var entity = await _patientNoteRepository.GetAsync(id);
        entity.Title = input.Title;
        entity.Content = input.Content;
        entity.NoteType = input.NoteType;
        entity.IsPrivate = input.IsPrivate;
        await _patientNoteRepository.UpdateAsync(entity);
        return ObjectMapper.Map<PatientNote, PatientNoteDto>(entity);
    }

    public async Task DeletePatientNoteAsync(Guid id) => await _patientNoteRepository.DeleteAsync(id);

    #endregion
}
