using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace HIS.MedicalRecords;

/// <summary>
/// الحساسية - Allergy Entity
/// </summary>
public class Allergy : FullAuditedEntity<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    /// <summary>
    /// معرف المريض
    /// </summary>
    public Guid PatientId { get; set; }

    /// <summary>
    /// نوع المادة المسببة للحساسية
    /// </summary>
    public AllergenType AllergenType { get; set; }

    /// <summary>
    /// اسم المادة المسببة للحساسية بالعربية
    /// </summary>
    public string AllergenNameAr { get; set; } = string.Empty;

    /// <summary>
    /// اسم المادة المسببة للحساسية بالإنجليزية
    /// </summary>
    public string? AllergenNameEn { get; set; }

    /// <summary>
    /// رد الفعل التحسسي
    /// </summary>
    public string? Reaction { get; set; }

    /// <summary>
    /// شدة الحساسية
    /// </summary>
    public AllergySeverity Severity { get; set; } = AllergySeverity.Moderate;

    /// <summary>
    /// تاريخ بدء الحساسية
    /// </summary>
    public DateTime? OnsetDate { get; set; }

    /// <summary>
    /// حالة الحساسية
    /// </summary>
    public AllergyStatus Status { get; set; } = AllergyStatus.Active;

    /// <summary>
    /// ملاحظات
    /// </summary>
    public string? Notes { get; set; }

    protected Allergy() { }

    public Allergy(Guid id, Guid patientId, AllergenType allergenType, string allergenNameAr) : base(id)
    {
        PatientId = patientId;
        AllergenType = allergenType;
        AllergenNameAr = allergenNameAr;
    }
}
