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
    
    // Master Data IDs
    public Guid? NationalityId { get; set; }
    public string? NationalityName { get; set; }
    public Guid? ProfessionId { get; set; }
    public string? ProfessionName { get; set; }
    public Guid? ContractId { get; set; }
    public string? ContractName { get; set; }
    public Guid? PatientCategoryId { get; set; }
    public string? PatientCategoryName { get; set; }
    public Guid? ReferralSourceId { get; set; }
    public string? ReferralSourceName { get; set; }

    // Identity
    public IdentityType IdentityType { get; set; }
    public string IdentityNumber { get; set; } = string.Empty;
    public DateTime? IdentityExpiryDate { get; set; }
    public DateTime? IdentityIssueDate { get; set; }
    public string? IdentityIssuePlace { get; set; }

    // Passport
    public string? PassportNumber { get; set; }
    public DateTime? PassportIssueDate { get; set; }
    public string? PassportIssuePlace { get; set; }
    public DateTime? PassportExpiryDate { get; set; }

    // Visa
    public string? VisaNumber { get; set; }
    public DateTime? VisaIssueDate { get; set; }
    public string? VisaIssuePlace { get; set; }
    public DateTime? VisaExpiryDate { get; set; }

    // Contact
    public string MobileNumber { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }

    // Sponsor
    public string? SponsorName { get; set; }
    public string? SponsorId { get; set; }

    // Emergency
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactRelation { get; set; }
    public string? EmergencyContactPhone { get; set; }

    // Medical/Financial
    public string? CardNumber { get; set; }
    public string? TaxFile { get; set; }
    public string? BloodType { get; set; }
    public string? Allergies { get; set; }
    public string? Notes { get; set; }
    public string? PhotoUrl { get; set; }
    public bool IsSocialSecurity { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// DTO لإنشاء/تحديث المريض
/// </summary>
public class CreateUpdatePatientDto
{
    public string? FullNameAr { get; set; }
    public string? FullNameEn { get; set; }
    public string FirstNameAr { get; set; } = string.Empty;
    public string? MiddleNameAr { get; set; }
    public string LastNameAr { get; set; } = string.Empty;
    public string? FirstNameEn { get; set; }
    public string? MiddleNameEn { get; set; }
    public string? LastNameEn { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public MaritalStatus? MaritalStatus { get; set; }
    
    // Master Data
    public Guid? NationalityId { get; set; }
    public Guid? ProfessionId { get; set; }
    public Guid? ContractId { get; set; }
    public Guid? PatientCategoryId { get; set; }
    public Guid? ReferralSourceId { get; set; }

    // Identity
    public IdentityType IdentityType { get; set; }
    public string IdentityNumber { get; set; } = string.Empty;
    public DateTime? IdentityExpiryDate { get; set; }
    public DateTime? IdentityIssueDate { get; set; }
    public string? IdentityIssuePlace { get; set; }

    // Passport
    public string? PassportNumber { get; set; }
    public DateTime? PassportIssueDate { get; set; }
    public string? PassportIssuePlace { get; set; }
    public DateTime? PassportExpiryDate { get; set; }

    // Visa
    public string? VisaNumber { get; set; }
    public DateTime? VisaIssueDate { get; set; }
    public string? VisaIssuePlace { get; set; }
    public DateTime? VisaExpiryDate { get; set; }

    // Contact
    public string MobileNumber { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }

    // Sponsor
    public string? SponsorName { get; set; }
    public string? SponsorId { get; set; }

    // Emergency
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactRelation { get; set; }
    public string? EmergencyContactPhone { get; set; }

    // Additional
    public string? CardNumber { get; set; }
    public string? TaxFile { get; set; }
    public string? BloodType { get; set; }
    public string? Allergies { get; set; }
    public string? Notes { get; set; }
    public bool IsSocialSecurity { get; set; }
    public bool IsActive { get; set; } = true;
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
    public Guid? PatientCategoryId { get; set; }
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
