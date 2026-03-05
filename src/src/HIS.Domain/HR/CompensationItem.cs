using System;
using HIS.HR.Enums;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace HIS.HR;

/// <summary>
/// بند المرتب (بدل/استقطاع) - Compensation Item
/// </summary>
public class CompensationItem : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    /// <summary>
    /// اسم البند
    /// </summary>
    public string NameAr { get; set; } = string.Empty;

    /// <summary>
    /// اسم العرض
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// طبيعة البند (بدل / استقطاع)
    /// </summary>
    public CompensationNature Nature { get; set; }

    /// <summary>
    /// نوع البند (ثابت / نسبة / معادلة)
    /// </summary>
    public CompensationValueType ValueType { get; set; }

    /// <summary>
    /// طريقة الصرف (دائن / مدين)
    /// </summary>
    public CompensationMethod Method { get; set; }

    /// <summary>
    /// معادلة الحساب (إذا كان نوع البند معادلة)
    /// </summary>
    public string? FormulaExpression { get; set; }

    /// <summary>
    /// الحساب المالي المرتبط (دليل الحسابات)
    /// </summary>
    public Guid? AccountId { get; set; }

    /// <summary>
    /// نشط
    /// </summary>
    public bool IsActive { get; set; } = true;

    public int SortOrder { get; set; }

    protected CompensationItem() { }

    public CompensationItem(Guid id, Guid? tenantId, string nameAr, CompensationNature nature)
        : base(id)
    {
        TenantId = tenantId;
        NameAr = nameAr;
        Nature = nature;
    }
}
