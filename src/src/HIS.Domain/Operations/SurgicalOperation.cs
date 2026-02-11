using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace HIS.Operations;

/// <summary>
/// العملية الجراحية - Surgical Operation Entity
/// </summary>
public class SurgicalOperation : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    /// <summary>
    /// المريض
    /// </summary>
    public Guid PatientId { get; set; }

    /// <summary>
    /// الطبيب
    /// </summary>
    public Guid? DoctorId { get; set; }

    /// <summary>
    /// نوع العملية (من ServiceItem أو نص حر)
    /// </summary>
    public Guid? OperationTypeId { get; set; }

    /// <summary>
    /// اسم العملية
    /// </summary>
    public string OperationName { get; set; } = string.Empty;

    /// <summary>
    /// تاريخ ووقت العملية
    /// </summary>
    public DateTime OperationDate { get; set; }

    /// <summary>
    /// تفصيل العملية
    /// </summary>
    public string? Details { get; set; }

    /// <summary>
    /// مبلغ العملية الإجمالي
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// تحمل الشركة (التأمين)
    /// </summary>
    public decimal CompanyShare { get; set; }

    /// <summary>
    /// تحمل المريض
    /// </summary>
    public decimal PatientShare { get; set; }

    /// <summary>
    /// إجمالي التأمين
    /// </summary>
    public decimal InsuranceTotal { get; set; }

    /// <summary>
    /// حالة العملية
    /// </summary>
    public OperationStatus Status { get; set; } = OperationStatus.Scheduled;

    /// <summary>
    /// الفاتورة المرتبطة
    /// </summary>
    public Guid? InvoiceId { get; set; }

    /// <summary>
    /// التنويم المرتبط (إن وجد)
    /// </summary>
    public Guid? AdmissionId { get; set; }

    /// <summary>
    /// ملاحظات
    /// </summary>
    public string? Notes { get; set; }

    protected SurgicalOperation() { }

    public SurgicalOperation(Guid id, Guid? tenantId, Guid patientId, string operationName, DateTime operationDate)
        : base(id)
    {
        TenantId = tenantId;
        PatientId = patientId;
        OperationName = operationName;
        OperationDate = operationDate;
        Status = OperationStatus.Scheduled;
    }
}
