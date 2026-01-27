using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace HIS.Insurance;

/// <summary>
/// تأمين المريض - Patient Insurance Entity
/// </summary>
public class PatientInsurance : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    /// <summary>
    /// المريض
    /// </summary>
    public Guid PatientId { get; set; }

    /// <summary>
    /// خطة التأمين
    /// </summary>
    public Guid InsurancePlanId { get; set; }

    /// <summary>
    /// رقم البوليصة/العضوية
    /// </summary>
    public string PolicyNumber { get; set; } = string.Empty;

    /// <summary>
    /// رقم البطاقة
    /// </summary>
    public string? CardNumber { get; set; }

    /// <summary>
    /// تاريخ البداية
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// تاريخ الانتهاء
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// تأمين أساسي؟
    /// </summary>
    public bool IsPrimary { get; set; } = true;

    /// <summary>
    /// الحالة
    /// </summary>
    public PatientInsuranceStatus Status { get; set; } = PatientInsuranceStatus.Active;

    /// <summary>
    /// اسم المؤمن عليه (إذا كان مختلف)
    /// </summary>
    public string? SubscriberName { get; set; }

    /// <summary>
    /// العلاقة بالمؤمن عليه
    /// </summary>
    public string? RelationToSubscriber { get; set; }

    /// <summary>
    /// جهة العمل
    /// </summary>
    public string? EmployerName { get; set; }

    /// <summary>
    /// ملاحظات
    /// </summary>
    public string? Notes { get; set; }

    // Navigation
    public virtual InsurancePlan? InsurancePlan { get; set; }

    protected PatientInsurance() { }

    public PatientInsurance(Guid id, Guid? tenantId, Guid patientId, Guid insurancePlanId, string policyNumber)
        : base(id)
    {
        TenantId = tenantId;
        PatientId = patientId;
        InsurancePlanId = insurancePlanId;
        PolicyNumber = policyNumber;
    }
}
