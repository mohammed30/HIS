using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace HIS.Insurance;

/// <summary>
/// خطة/وثيقة التأمين - Insurance Plan Entity
/// </summary>
public class InsurancePlan : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    /// <summary>
    /// شركة التأمين
    /// </summary>
    public Guid InsuranceCompanyId { get; set; }

    /// <summary>
    /// كود الخطة
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// اسم الخطة بالعربية
    /// </summary>
    public string NameAr { get; set; } = string.Empty;

    /// <summary>
    /// اسم الخطة بالإنجليزية
    /// </summary>
    public string? NameEn { get; set; }

    /// <summary>
    /// نوع الخطة
    /// </summary>
    public InsurancePlanType PlanType { get; set; } = InsurancePlanType.Individual;

    /// <summary>
    /// فئة الخطة (A, B, C)
    /// </summary>
    public InsurancePlanClass PlanClass { get; set; } = InsurancePlanClass.ClassB;

    /// <summary>
    /// نسبة التغطية (%)
    /// </summary>
    public decimal CoveragePercentage { get; set; } = 80;

    /// <summary>
    /// الحد الأقصى للتغطية
    /// </summary>
    public decimal? MaxCoverageAmount { get; set; }

    /// <summary>
    /// نسبة المشاركة للمريض (Co-payment %)
    /// </summary>
    public decimal CoPaymentPercentage { get; set; } = 20;

    /// <summary>
    /// قيمة الخصم الثابت (Deductible)
    /// </summary>
    public decimal DeductibleAmount { get; set; } = 0;

    /// <summary>
    /// يشمل الأدوية
    /// </summary>
    public bool IncludesMedications { get; set; } = true;

    /// <summary>
    /// يشمل المختبر
    /// </summary>
    public bool IncludesLab { get; set; } = true;

    /// <summary>
    /// يشمل الأشعة
    /// </summary>
    public bool IncludesRadiology { get; set; } = true;

    /// <summary>
    /// يشمل التنويم
    /// </summary>
    public bool IncludesInpatient { get; set; } = false;

    /// <summary>
    /// ملاحظات
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// نشط
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// الترتيب
    /// </summary>
    public int SortOrder { get; set; }

    // Navigation
    public virtual InsuranceCompany? InsuranceCompany { get; set; }

    protected InsurancePlan() { }

    public InsurancePlan(Guid id, Guid? tenantId, Guid insuranceCompanyId, string code, string nameAr)
        : base(id)
    {
        TenantId = tenantId;
        InsuranceCompanyId = insuranceCompanyId;
        Code = code;
        NameAr = nameAr;
    }
}
