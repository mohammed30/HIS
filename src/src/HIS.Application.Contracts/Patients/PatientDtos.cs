using System;
using Volo.Abp.Application.Dtos;

namespace HIS.Patients;

/// <summary>
/// DTO لعرض بيانات المريض
/// </summary>
public class PatientDto : FullAuditedEntityDto<Guid>
{
    public string MRN { get; set; } = string.Empty;
    public string FirstNameAr { get; set; } = string.Empty;
    public string? MiddleNameAr { get; set; }
    public string LastNameAr { get; set; } = string.Empty;
    public string? FirstNameEn { get; set; }
    public string? MiddleNameEn { get; set; }
    public string? LastNameEn { get; set; }
    public string FullNameAr { get; set; } = string.Empty;
    public string? FullNameEn { get; set; }
    public DateTime DateOfBirth { get; set; }
    public int Age { get; set; }
    public Gender Gender { get; set; }
    public MaritalStatus? MaritalStatus { get; set; }
    public string? Nationality { get; set; }
    public IdentityType IdentityType { get; set; }
    public string IdentityNumber { get; set; } = string.Empty;
    public DateTime? IdentityExpiryDate { get; set; }
    public string MobileNumber { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactRelation { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public PatientCategory Category { get; set; }
    public string? BloodType { get; set; }
    public string? Allergies { get; set; }
    public string? Notes { get; set; }
    public string? PhotoUrl { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// DTO لإنشاء/تحديث المريض
/// </summary>
public class CreateUpdatePatientDto
{
    public string FirstNameAr { get; set; } = string.Empty;
    public string? MiddleNameAr { get; set; }
    public string LastNameAr { get; set; } = string.Empty;
    public string? FirstNameEn { get; set; }
    public string? MiddleNameEn { get; set; }
    public string? LastNameEn { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public MaritalStatus? MaritalStatus { get; set; }
    public string? Nationality { get; set; }
    public IdentityType IdentityType { get; set; }
    public string IdentityNumber { get; set; } = string.Empty;
    public DateTime? IdentityExpiryDate { get; set; }
    public string MobileNumber { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactRelation { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public PatientCategory Category { get; set; } = PatientCategory.Regular;
    public string? BloodType { get; set; }
    public string? Allergies { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// DTO للبحث عن المرضى
/// </summary>
public class GetPatientsInput : PagedAndSortedResultRequestDto
{
    public string? SearchText { get; set; }
    public string? MRN { get; set; }
    public string? IdentityNumber { get; set; }
    public string? MobileNumber { get; set; }
    public Gender? Gender { get; set; }
    public PatientCategory? Category { get; set; }
    public bool? IsActive { get; set; }
}

/// <summary>
/// DTO مختصر للقوائم المنسدلة
/// </summary>
public class PatientLookupDto
{
    public Guid Id { get; set; }
    public string MRN { get; set; } = string.Empty;
    public string FullNameAr { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;
}
