using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace HIS.MedicalRecords;

/// <summary>
/// العلامات الحيوية - Vital Sign Entity
/// </summary>
public class VitalSign : FullAuditedEntity<Guid>, IMultiTenant
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
    /// وقت القياس
    /// </summary>
    public DateTime RecordedAt { get; set; }

    /// <summary>
    /// درجة الحرارة (سيليزيوس)
    /// </summary>
    public decimal? Temperature { get; set; }

    /// <summary>
    /// ضغط الدم الانقباضي
    /// </summary>
    public int? BloodPressureSystolic { get; set; }

    /// <summary>
    /// ضغط الدم الانبساطي
    /// </summary>
    public int? BloodPressureDiastolic { get; set; }

    /// <summary>
    /// معدل ضربات القلب (نبضة/دقيقة)
    /// </summary>
    public int? HeartRate { get; set; }

    /// <summary>
    /// معدل التنفس (نفس/دقيقة)
    /// </summary>
    public int? RespiratoryRate { get; set; }

    /// <summary>
    /// نسبة تشبع الأكسجين (%)
    /// </summary>
    public decimal? OxygenSaturation { get; set; }

    /// <summary>
    /// الوزن (كجم)
    /// </summary>
    public decimal? Weight { get; set; }

    /// <summary>
    /// الطول (سم)
    /// </summary>
    public decimal? Height { get; set; }

    /// <summary>
    /// مؤشر كتلة الجسم (محسوب)
    /// </summary>
    public decimal? BMI => (Weight.HasValue && Height.HasValue && Height > 0) 
        ? Math.Round(Weight.Value / ((Height.Value / 100) * (Height.Value / 100)), 1) 
        : null;

    /// <summary>
    /// معرف من سجل القياس
    /// </summary>
    public Guid? RecordedById { get; set; }

    /// <summary>
    /// اسم من سجل القياس
    /// </summary>
    public string? RecordedByName { get; set; }

    /// <summary>
    /// ملاحظات
    /// </summary>
    public string? Notes { get; set; }

    protected VitalSign() { }

    public VitalSign(Guid id, Guid patientId, DateTime recordedAt) : base(id)
    {
        PatientId = patientId;
        RecordedAt = recordedAt;
    }
}
