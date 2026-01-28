using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace HIS.MedicalRecords;

/// <summary>
/// التاريخ المرضي - Medical History Entity
/// </summary>
public class MedicalHistory : FullAuditedEntity<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    /// <summary>
    /// معرف المريض
    /// </summary>
    public Guid PatientId { get; set; }

    /// <summary>
    /// اسم الحالة المرضية بالعربية
    /// </summary>
    public string ConditionAr { get; set; } = string.Empty;

    /// <summary>
    /// اسم الحالة المرضية بالإنجليزية
    /// </summary>
    public string? ConditionEn { get; set; }

    /// <summary>
    /// رمز ICD-10
    /// </summary>
    public string? ICD10Code { get; set; }

    /// <summary>
    /// تاريخ التشخيص
    /// </summary>
    public DateTime? DiagnosedDate { get; set; }

    /// <summary>
    /// تاريخ الشفاء
    /// </summary>
    public DateTime? ResolvedDate { get; set; }

    /// <summary>
    /// هل هو مزمن؟
    /// </summary>
    public bool IsChronic { get; set; }

    /// <summary>
    /// ملاحظات
    /// </summary>
    public string? Notes { get; set; }

    protected MedicalHistory() { }

    public MedicalHistory(Guid id, Guid patientId, string conditionAr) : base(id)
    {
        PatientId = patientId;
        ConditionAr = conditionAr;
    }
}
