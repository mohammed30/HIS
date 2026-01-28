using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace HIS.MedicalRecords;

/// <summary>
/// التشخيص - Diagnosis Entity
/// </summary>
public class Diagnosis : FullAuditedEntity<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    /// <summary>
    /// معرف المريض
    /// </summary>
    public Guid PatientId { get; set; }

    /// <summary>
    /// معرف الزيارة (اختياري)
    /// </summary>
    public Guid? VisitId { get; set; }

    /// <summary>
    /// رمز ICD-10
    /// </summary>
    public string? ICD10Code { get; set; }

    /// <summary>
    /// اسم التشخيص بالعربية
    /// </summary>
    public string DiagnosisNameAr { get; set; } = string.Empty;

    /// <summary>
    /// اسم التشخيص بالإنجليزية
    /// </summary>
    public string? DiagnosisNameEn { get; set; }

    /// <summary>
    /// تاريخ التشخيص
    /// </summary>
    public DateTime DiagnosisDate { get; set; }

    /// <summary>
    /// نوع التشخيص (رئيسي/ثانوي/تفريقي)
    /// </summary>
    public DiagnosisType Type { get; set; } = DiagnosisType.Primary;

    /// <summary>
    /// حالة التشخيص
    /// </summary>
    public DiagnosisStatus Status { get; set; } = DiagnosisStatus.Active;

    /// <summary>
    /// معرف الطبيب المشخص
    /// </summary>
    public Guid? DiagnosedById { get; set; }

    /// <summary>
    /// اسم الطبيب المشخص
    /// </summary>
    public string? DiagnosedByName { get; set; }

    /// <summary>
    /// ملاحظات
    /// </summary>
    public string? Notes { get; set; }

    protected Diagnosis() { }

    public Diagnosis(Guid id, Guid patientId, string diagnosisNameAr, DateTime diagnosisDate) : base(id)
    {
        PatientId = patientId;
        DiagnosisNameAr = diagnosisNameAr;
        DiagnosisDate = diagnosisDate;
    }
}
