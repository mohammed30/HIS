using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace HIS.Accounting;

/// <summary>
/// مركز التكلفة
/// يستخدم لتتبع الإيرادات والمصروفات على مستوى الأقسام أو المشاريع كبُعد تحليلي.
/// </summary>
public class CostCenter : FullAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// كود مركز التكلفة
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// الاسم بالعربية
    /// </summary>
    public string NameAr { get; set; }

    /// <summary>
    /// الاسم بالإنجليزية
    /// </summary>
    public string NameEn { get; set; }

    /// <summary>
    /// نشط أم لا
    /// </summary>
    public bool IsActive { get; set; }

    protected CostCenter() { }

    public CostCenter(Guid id, string code, string nameAr, string nameEn = null)
        : base(id)
    {
        Code = code;
        NameAr = nameAr;
        NameEn = nameEn;
        IsActive = true;
    }
}
