using System;
using Volo.Abp.Domain.Entities.Auditing;
using HIS.Patients;

namespace HIS.Laboratory;

/// <summary>
/// يمثل العوامل الطبيعية (Reference Ranges) لتحليل معين
/// يمكن أن تختلف بناءً على الجنس والعمر
/// </summary>
public class LabTestNormalRange : FullAuditedEntity<Guid>
{
    public Guid LabTestId { get; set; }
    public virtual LabTest LabTest { get; set; }

    /// <summary>
    /// الجنس المستهدف (null يعني كلاهما)
    /// </summary>
    public Gender? TargetGender { get; set; }

    /// <summary>
    /// الحد الأدنى للعمر بالأيام (null يعني منذ الولادة)
    /// </summary>
    public int? MinAgeDays { get; set; }

    /// <summary>
    /// الحد الأقصى للعمر بالأيام (null يعني بلا حد)
    /// </summary>
    public int? MaxAgeDays { get; set; }

    /// <summary>
    /// نوع النتيجة (رقمي أو نصي)
    /// </summary>
    public LabResultType ResultType { get; set; }

    /// <summary>
    /// الحد الأدنى للقيمة (إذا كان رقمي)
    /// </summary>
    public decimal? MinValue { get; set; }

    /// <summary>
    /// الحد الأقصى للقيمة (إذا كان رقمي)
    /// </summary>
    public decimal? MaxValue { get; set; }

    /// <summary>
    /// القيمة النصية الطبيعية (إذا كان نصي)
    /// </summary>
    public string? NormalStringValue { get; set; }

    protected LabTestNormalRange() { }

    public LabTestNormalRange(Guid id, Guid labTestId, LabResultType resultType) : base(id)
    {
        LabTestId = labTestId;
        ResultType = resultType;
    }
}
