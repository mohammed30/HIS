using System;
using HIS.HR.Enums;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace HIS.HR;

/// <summary>
/// الموظف - Employee Entity
/// </summary>
public class Employee : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    /// <summary>
    /// رقم الموظف
    /// </summary>
    public string EmployeeNumber { get; set; } = string.Empty;

    /// <summary>
    /// الاسم بالعربية
    /// </summary>
    public string NameAr { get; set; } = string.Empty;

    /// <summary>
    /// الاسم بالإنجليزية
    /// </summary>
    public string? NameEn { get; set; }

    /// <summary>
    /// النوع (ذكر/أنثى)
    /// </summary>
    public Gender Gender { get; set; }

    /// <summary>
    /// تاريخ الميلاد
    /// </summary>
    public DateTime? BirthDate { get; set; }

    /// <summary>
    /// الحالة الاجتماعية
    /// </summary>
    public MaritalStatus? MaritalStatus { get; set; }

    /// <summary>
    /// العنوان
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// رقم الهاتف
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// البريد الإلكتروني
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// المؤهل العلمي
    /// </summary>
    public string? Qualification { get; set; }

    /// <summary>
    /// نوع إثبات الشخصية
    /// </summary>
    public IdentityDocumentType? IdentityType { get; set; }

    /// <summary>
    /// رقم إثبات الشخصية
    /// </summary>
    public string? IdentityNumber { get; set; }

    /// <summary>
    /// رقم التأمين الصحي
    /// </summary>
    public string? InsuranceNumber { get; set; }

    /// <summary>
    /// البنك
    /// </summary>
    public string? BankName { get; set; }

    /// <summary>
    /// رقم الحساب البنكي
    /// </summary>
    public string? BankAccountNumber { get; set; }

    /// <summary>
    /// رقم الآيبان
    /// </summary>
    public string? IBAN { get; set; }

    // ======== القسم والتخصص ========

    /// <summary>
    /// الإدارة (مرتبط بـ Department الحالي)
    /// </summary>
    public Guid? DepartmentId { get; set; }

    /// <summary>
    /// القسم (اسم نصي)
    /// </summary>
    public string? SectionName { get; set; }

    /// <summary>
    /// الدرجة الوظيفية
    /// </summary>
    public Guid? JobGradeId { get; set; }

    /// <summary>
    /// المسمى الوظيفي (ID)
    /// </summary>
    public Guid? JobTitleId { get; set; }

    /// <summary>
    /// المسمى الوظيفي (اسم نصي للتوافق أو مسمى إضافي)
    /// </summary>
    public string? JobTitle { get; set; }

    /// <summary>
    /// التصنيف الوظيفي
    /// </summary>
    public string? EmploymentClassification { get; set; }

    /// <summary>
    /// طريقة صرف المرتب
    /// </summary>
    public SalaryPaymentMethod? SalaryPaymentMethod { get; set; }

    /// <summary>
    /// نوع التعاقد
    /// </summary>
    public ContractType? ContractType { get; set; }

    /// <summary>
    /// تاريخ التعيين
    /// </summary>
    public DateTime? HireDate { get; set; }

    /// <summary>
    /// تاريخ إيقاف مؤقت / إنهاء الخدمة
    /// </summary>
    public DateTime? TerminationDate { get; set; }

    /// <summary>
    /// تذكير
    /// </summary>
    public bool ReminderEnabled { get; set; }

    /// <summary>
    /// صورة الموظف
    /// </summary>
    public string? PhotoUrl { get; set; }

    /// <summary>
    /// إيقاف الموظف
    /// </summary>
    public bool IsSuspended { get; set; }

    /// <summary>
    /// نشط
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// الراتب الأساسي
    /// </summary>
    public decimal? BasicSalary { get; set; }

    public int SortOrder { get; set; }

    protected Employee() { }

    public Employee(Guid id, Guid? tenantId, string employeeNumber, string nameAr)
        : base(id)
    {
        TenantId = tenantId;
        EmployeeNumber = employeeNumber;
        NameAr = nameAr;
    }
}
