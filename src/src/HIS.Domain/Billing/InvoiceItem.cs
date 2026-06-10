using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace HIS.Billing;

/// <summary>
/// بند الفاتورة - Invoice Item Entity
/// </summary>
public class InvoiceItem : FullAuditedEntity<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    /// <summary>
    /// الفاتورة
    /// </summary>
    public Guid InvoiceId { get; set; }

    /// <summary>
    /// القسم المانح للخدمة (لتوجيه الإيراد لمركز التكلفة الصحيح)
    /// </summary>
    public Guid? DepartmentId { get; set; }

    /// <summary>
    /// نوع الخدمة
    /// </summary>
    public ServiceType ServiceType { get; set; }

    /// <summary>
    /// كود الخدمة
    /// </summary>
    public string? ServiceCode { get; set; }

    /// <summary>
    /// وصف الخدمة
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// الكمية
    /// </summary>
    public decimal Quantity { get; set; } = 1;

    /// <summary>
    /// سعر الوحدة
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// نسبة الخصم
    /// </summary>
    public decimal DiscountPercentage { get; set; }

    /// <summary>
    /// مبلغ الخصم
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// الإجمالي
    /// </summary>
    public decimal TotalPrice => (Quantity * UnitPrice) - DiscountAmount;

    /// <summary>
    /// مغطى بالتأمين؟
    /// </summary>
    public bool IsCoveredByInsurance { get; set; } = true;

    /// <summary>
    /// ملاحظات
    /// </summary>
    public string? Notes { get; set; }

    // Navigation
    public virtual Invoice? Invoice { get; set; }

    protected InvoiceItem() { }

    public InvoiceItem(Guid id, Guid? tenantId, Guid invoiceId, string description, decimal unitPrice)
    {
        Id = id;
        TenantId = tenantId;
        InvoiceId = invoiceId;
        Description = description;
        UnitPrice = unitPrice;
    }
}
