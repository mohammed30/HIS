using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace HIS.MedicalRecords;

/// <summary>
/// ملاحظة طبية - Patient Note Entity
/// </summary>
public class PatientNote : FullAuditedEntity<Guid>, IMultiTenant
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
    /// نوع الملاحظة
    /// </summary>
    public NoteType NoteType { get; set; } = NoteType.Progress;

    /// <summary>
    /// العنوان
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// المحتوى
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// معرف الكاتب
    /// </summary>
    public Guid? CreatedById { get; set; }

    /// <summary>
    /// اسم الكاتب
    /// </summary>
    public string? CreatedByName { get; set; }

    /// <summary>
    /// هل الملاحظة خاصة؟
    /// </summary>
    public bool IsPrivate { get; set; }

    protected PatientNote() { }

    public PatientNote(Guid id, Guid patientId, string title, string content) : base(id)
    {
        PatientId = patientId;
        Title = title;
        Content = content;
    }
}
