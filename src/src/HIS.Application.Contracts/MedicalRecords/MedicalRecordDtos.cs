using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using HIS.Patients;

namespace HIS.MedicalRecords;

#region Medical History DTOs

public class MedicalHistoryDto : EntityDto<Guid>
{
    public Guid PatientId { get; set; }
    public string ConditionAr { get; set; } = string.Empty;
    public string? ConditionEn { get; set; }
    public string? ICD10Code { get; set; }
    public DateTime? DiagnosedDate { get; set; }
    public DateTime? ResolvedDate { get; set; }
    public bool IsChronic { get; set; }
    public string? Notes { get; set; }
}

public class CreateUpdateMedicalHistoryDto
{
    public Guid PatientId { get; set; }
    public string ConditionAr { get; set; } = string.Empty;
    public string? ConditionEn { get; set; }
    public string? ICD10Code { get; set; }
    public DateTime? DiagnosedDate { get; set; }
    public DateTime? ResolvedDate { get; set; }
    public bool IsChronic { get; set; }
    public string? Notes { get; set; }
}

#endregion

#region Diagnosis DTOs

public class DiagnosisDto : EntityDto<Guid>
{
    public Guid PatientId { get; set; }
    public Guid? VisitId { get; set; }
    public string? ICD10Code { get; set; }
    public string DiagnosisNameAr { get; set; } = string.Empty;
    public string? DiagnosisNameEn { get; set; }
    public DateTime DiagnosisDate { get; set; }
    public DiagnosisType Type { get; set; }
    public DiagnosisStatus Status { get; set; }
    public Guid? DiagnosedById { get; set; }
    public string? DiagnosedByName { get; set; }
    public string? Notes { get; set; }
}

public class CreateUpdateDiagnosisDto
{
    public Guid PatientId { get; set; }
    public Guid? VisitId { get; set; }
    public string? ICD10Code { get; set; }
    public string DiagnosisNameAr { get; set; } = string.Empty;
    public string? DiagnosisNameEn { get; set; }
    public DateTime DiagnosisDate { get; set; }
    public DiagnosisType Type { get; set; }
    public DiagnosisStatus Status { get; set; }
    public string? Notes { get; set; }
}

#endregion

#region Vital Sign DTOs

public class VitalSignDto : EntityDto<Guid>
{
    public Guid PatientId { get; set; }
    public Guid? VisitId { get; set; }
    public DateTime RecordedAt { get; set; }
    public decimal? Temperature { get; set; }
    public int? BloodPressureSystolic { get; set; }
    public int? BloodPressureDiastolic { get; set; }
    public int? HeartRate { get; set; }
    public int? RespiratoryRate { get; set; }
    public decimal? OxygenSaturation { get; set; }
    public decimal? Weight { get; set; }
    public decimal? Height { get; set; }
    public decimal? BMI { get; set; }
    public string? RecordedByName { get; set; }
    public string? Notes { get; set; }
}

public class CreateUpdateVitalSignDto
{
    public Guid PatientId { get; set; }
    public Guid? VisitId { get; set; }
    public DateTime RecordedAt { get; set; } = DateTime.Now;
    public decimal? Temperature { get; set; }
    public int? BloodPressureSystolic { get; set; }
    public int? BloodPressureDiastolic { get; set; }
    public int? HeartRate { get; set; }
    public int? RespiratoryRate { get; set; }
    public decimal? OxygenSaturation { get; set; }
    public decimal? Weight { get; set; }
    public decimal? Height { get; set; }
    public string? Notes { get; set; }
}

#endregion

#region Allergy DTOs

public class AllergyDto : EntityDto<Guid>
{
    public Guid PatientId { get; set; }
    public AllergenType AllergenType { get; set; }
    public string AllergenNameAr { get; set; } = string.Empty;
    public string? AllergenNameEn { get; set; }
    public string? Reaction { get; set; }
    public AllergySeverity Severity { get; set; }
    public DateTime? OnsetDate { get; set; }
    public AllergyStatus Status { get; set; }
    public string? Notes { get; set; }
}

public class CreateUpdateAllergyDto
{
    public Guid PatientId { get; set; }
    public AllergenType AllergenType { get; set; }
    public string AllergenNameAr { get; set; } = string.Empty;
    public string? AllergenNameEn { get; set; }
    public string? Reaction { get; set; }
    public AllergySeverity Severity { get; set; }
    public DateTime? OnsetDate { get; set; }
    public AllergyStatus Status { get; set; }
    public string? Notes { get; set; }
}

#endregion

#region Patient Note DTOs

public class PatientNoteDto : EntityDto<Guid>
{
    public Guid PatientId { get; set; }
    public Guid? VisitId { get; set; }
    public NoteType NoteType { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? CreatedByName { get; set; }
    public bool IsPrivate { get; set; }
    public DateTime CreationTime { get; set; }
}

public class CreateUpdatePatientNoteDto
{
    public Guid PatientId { get; set; }
    public Guid? VisitId { get; set; }
    public NoteType NoteType { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsPrivate { get; set; }
}

#endregion

#region Summary DTO

public class PatientMedicalSummaryDto
{
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string? BloodType { get; set; }
    public int Age { get; set; }
    public Gender Gender { get; set; }
    
    public int ActiveAllergiesCount { get; set; }
    public int ChronicConditionsCount { get; set; }
    public int ActiveDiagnosesCount { get; set; }
    
    public VitalSignDto? LatestVitals { get; set; }
    public List<AllergyDto> ActiveAllergies { get; set; } = new();
    public List<MedicalHistoryDto> ChronicConditions { get; set; } = new();
}

#endregion
