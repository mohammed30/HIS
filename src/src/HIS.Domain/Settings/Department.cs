using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace HIS.Settings;

/// <summary>
/// القسم الطبي - Department Entity
/// </summary>
public class Department : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    /// <summary>
    /// كود القسم
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// اسم القسم بالعربية
    /// </summary>
    public string NameAr { get; set; } = string.Empty;

    /// <summary>
    /// اسم القسم بالإنجليزية
    /// </summary>
    public string? NameEn { get; set; }

    /// <summary>
    /// الوصف
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// الموقع
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// رقم الهاتف الداخلي
    /// </summary>
    public string? ExtensionNumber { get; set; }

    /// <summary>
    /// مدير القسم
    /// </summary>
    public Guid? ManagerId { get; set; }

    /// <summary>
    /// نشط
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// الترتيب
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// هل هو قسم طبي؟ إذا كان true يظهر في قائمة تعريف الأطباء
    /// </summary>
    /// <summary>
    /// مركز التكلفة (Cost Center) - تم ربطه الآن بكيان CostCenter المستقل
    /// </summary>
    public Guid? CostCenterId { get; set; }
    
    public virtual HIS.Accounting.CostCenter CostCenter { get; set; }

    public bool IsMedical { get; set; } = false;

    protected Department()
    {
    }

    public Department(Guid id, Guid? tenantId, string code, string nameAr)
        : base(id)
    {
        TenantId = tenantId;
        Code = code;
        NameAr = nameAr;
    }
}
