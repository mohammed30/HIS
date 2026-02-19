using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace HIS.Patients;

/// <summary>
/// كيان المريض - Patient Entity
/// </summary>
public class Patient : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    # region البيانات الشخصية - Personal Information

    /// <summary>
    /// رقم الملف الطبي - Medical Record Number
    /// </summary>
    public string MRN { get; set; } = string.Empty;

    /// <summary>
    /// الاسم الأول بالعربية
    /// </summary>
    public string FirstNameAr { get; set; } = string.Empty;

    /// <summary>
    /// اسم الأب بالعربية
    /// </summary>
    public string? MiddleNameAr { get; set; }

    /// <summary>
    /// اسم العائلة بالعربية
    /// </summary>
    public string LastNameAr { get; set; } = string.Empty;

    /// <summary>
    /// الاسم الأول بالإنجليزية
    /// </summary>
    public string? FirstNameEn { get; set; }

    /// <summary>
    /// اسم الأب بالإنجليزية
    /// </summary>
    public string? MiddleNameEn { get; set; }

    /// <summary>
    /// اسم العائلة بالإنجليزية
    /// </summary>
    public string? LastNameEn { get; set; }

    /// <summary>
    /// تاريخ الميلاد
    /// </summary>
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// الجنس
    /// </summary>
    public Gender Gender { get; set; }

    /// <summary>
    /// الحالة الاجتماعية
    /// </summary>
    public MaritalStatus? MaritalStatus { get; set; }

    /// <summary>
    /// معرف الجنسية
    /// </summary>
    public Guid? NationalityId { get; set; }

    /// <summary>
    /// معرف المهنة
    /// </summary>
    public Guid? ProfessionId { get; set; }

    #endregion

    #region بيانات الهوية - Identity Information

    /// <summary>
    /// نوع الهوية
    /// </summary>
    public IdentityType IdentityType { get; set; }

    /// <summary>
    /// رقم الهوية
    /// </summary>
    public string IdentityNumber { get; set; } = string.Empty;

    /// <summary>
    /// تاريخ انتهاء الهوية
    /// </summary>
    public DateTime? IdentityExpiryDate { get; set; }

    /// <summary>
    /// تاريخ إصدار الهوية
    /// </summary>
    public DateTime? IdentityIssueDate { get; set; }

    /// <summary>
    /// مكان إصدار الهوية
    /// </summary>
    public string? IdentityIssuePlace { get; set; }

    #endregion

    #region بيانات الجواز - Passport Information

    /// <summary>
    /// رقم الجواز
    /// </summary>
    public string? PassportNumber { get; set; }

    /// <summary>
    /// تاريخ إصدار الجواز
    /// </summary>
    public DateTime? PassportIssueDate { get; set; }

    /// <summary>
    /// مكان إصدار الجواز
    /// </summary>
    public string? PassportIssuePlace { get; set; }

    /// <summary>
    /// تاريخ انتهاء الجواز
    /// </summary>
    public DateTime? PassportExpiryDate { get; set; }

    #endregion

    #region بيانات التأشيرة - Visa Information

    /// <summary>
    /// رقم التأشيرة
    /// </summary>
    public string? VisaNumber { get; set; }

    /// <summary>
    /// تاريخ إصدار التأشيرة
    /// </summary>
    public DateTime? VisaIssueDate { get; set; }

    /// <summary>
    /// مكان إصدار التأشيرة
    /// </summary>
    public string? VisaIssuePlace { get; set; }

    /// <summary>
    /// تاريخ انتهاء التأشيرة
    /// </summary>
    public DateTime? VisaExpiryDate { get; set; }

    #endregion

    #region بيانات الاتصال - Contact Information

    /// <summary>
    /// رقم الجوال
    /// </summary>
    public string MobileNumber { get; set; } = string.Empty;

    /// <summary>
    /// رقم الهاتف
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// البريد الإلكتروني
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// العنوان
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// المدينة
    /// </summary>
    public string? City { get; set; }

    #endregion

    #region بيانات الكفيل / العائل - Sponsor / Guardian

    /// <summary>
    /// اسم العائل / الكفيل
    /// </summary>
    public string? SponsorName { get; set; }

    /// <summary>
    /// رقم الهوية للكفيل / العائل
    /// </summary>
    public string? SponsorId { get; set; }

    #endregion

    #region بيانات الطوارئ - Emergency Contact

    /// <summary>
    /// اسم جهة اتصال الطوارئ
    /// </summary>
    public string? EmergencyContactName { get; set; }

    /// <summary>
    /// علاقة جهة اتصال الطوارئ
    /// </summary>
    public string? EmergencyContactRelation { get; set; }

    /// <summary>
    /// رقم جهة اتصال الطوارئ
    /// </summary>
    public string? EmergencyContactPhone { get; set; }

    #endregion

    #region بيانات إضافية وتعاقدات - Additional & Contracts

    /// <summary>
    /// فئة المريض
    /// </summary>
    public Guid? PaymentMethodId { get; set; }

    /// <summary>
    /// التعاقد
    /// </summary>
    public Guid? ContractId { get; set; }

    /// <summary>
    /// الجهة المحولة
    /// </summary>
    public Guid? ReferralSourceId { get; set; }

    /// <summary>
    /// رقم بطاقة التأمين / العميل
    /// </summary>
    public string? CardNumber { get; set; }

    /// <summary>
    /// الملف الضريبي
    /// </summary>
    public string? TaxFile { get; set; }

    /// <summary>
    /// فصيلة الدم
    /// </summary>
    public string? BloodType { get; set; }

    /// <summary>
    /// الحساسية
    /// </summary>
    public string? Allergies { get; set; }

    /// <summary>
    /// هل لديه تأمين اجتماعي؟
    /// </summary>
    public bool IsSocialSecurity { get; set; }

    /// <summary>
    /// ملاحظات
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// صورة المريض
    /// </summary>
    public string? PhotoUrl { get; set; }

    /// <summary>
    /// نشط
    /// </summary>
    public bool IsActive { get; set; } = true;

    #endregion

    protected Patient()
    {
    }

    public Patient(
        Guid id,
        Guid? tenantId,
        string mrn,
        string firstNameAr,
        string lastNameAr,
        DateTime dateOfBirth,
        Gender gender,
        IdentityType identityType,
        string identityNumber,
        string mobileNumber)
        : base(id)
    {
        TenantId = tenantId;
        MRN = mrn;
        FirstNameAr = firstNameAr;
        LastNameAr = lastNameAr;
        DateOfBirth = dateOfBirth;
        Gender = gender;
        IdentityType = identityType;
        IdentityNumber = identityNumber;
        MobileNumber = mobileNumber;
    }

    /// <summary>
    /// الاسم الكامل بالعربية
    /// </summary>
    public string FullNameAr => $"{FirstNameAr} {MiddleNameAr} {LastNameAr}".Replace("  ", " ").Trim();

    /// <summary>
    /// الاسم الكامل بالإنجليزية
    /// </summary>
    public string? FullNameEn => string.IsNullOrEmpty(FirstNameEn) ? null : $"{FirstNameEn} {MiddleNameEn} {LastNameEn}".Replace("  ", " ").Trim();

    /// <summary>
    /// العمر بالسنوات
    /// </summary>
    public int Age => DateTime.Today.Year - DateOfBirth.Year - (DateTime.Today.DayOfYear < DateOfBirth.DayOfYear ? 1 : 0);
}
