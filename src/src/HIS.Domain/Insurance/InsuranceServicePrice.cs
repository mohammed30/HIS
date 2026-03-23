using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using HIS.Services;

namespace HIS.Insurance;

/// <summary>
/// تسعير الخدمات خاص بشركة/خطة تأمين محددة
/// </summary>
public class InsuranceServicePrice : FullAuditedEntity<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    /// <summary>
    /// خطة التأمين
    /// </summary>
    public Guid InsurancePlanId { get; set; }

    /// <summary>
    /// الخدمة الطبية
    /// </summary>
    public Guid ServiceItemId { get; set; }

    /// <summary>
    /// السعر الخاص لهذه الخدمة في هذه الخطة
    /// </summary>
    public decimal CustomPrice { get; set; }

    /// <summary>
    /// ملاحظات
    /// </summary>
    public string? Notes { get; set; }

    // Navigation
    public virtual InsurancePlan? InsurancePlan { get; set; }
    public virtual ServiceItem? ServiceItem { get; set; }

    protected InsuranceServicePrice() { }

    public InsuranceServicePrice(Guid id, Guid? tenantId, Guid insurancePlanId, Guid serviceItemId, decimal customPrice)
        : base(id)
    {
        TenantId = tenantId;
        InsurancePlanId = insurancePlanId;
        ServiceItemId = serviceItemId;
        CustomPrice = customPrice;
    }
}
