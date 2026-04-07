using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;

namespace HIS.Inventory;

/// <summary>
/// طلب صرف داخلي لربط الصيدليات أو التمريض بالمستودع الرئيسي
/// </summary>
public class InternalRequest : FullAuditedAggregateRoot<Guid>
{
    public string RequestNumber { get; set; }
    public Guid RequestingDepartmentId { get; set; } // e.g. Pharmacy Id, Nursing Ward Id
    public Guid FulfilledByWarehouseId { get; set; } // e.g. Main Store Id
    
    /// <summary>
    /// المريض المنوم (لخصم التكلفة على حسابه)
    /// </summary>
    public Guid? AdmissionId { get; set; }

    public DateTime RequestDate { get; set; }
    public InternalRequestStatus Status { get; set; }
    public string? Notes { get; set; }

    public virtual ICollection<InternalRequestLine> Lines { get; set; }

    protected InternalRequest() 
    {
        Lines = new List<InternalRequestLine>();
    }

    public InternalRequest(Guid id, string requestNumber, Guid requestingDepartmentId, Guid fulfilledByWarehouseId, DateTime requestDate) 
        : base(id)
    {
        RequestNumber = requestNumber;
        RequestingDepartmentId = requestingDepartmentId;
        FulfilledByWarehouseId = fulfilledByWarehouseId;
        RequestDate = requestDate;
        Status = InternalRequestStatus.Draft;
        Lines = new List<InternalRequestLine>();
    }
}

public class InternalRequestLine : FullAuditedEntity<Guid>
{
    public Guid InternalRequestId { get; set; }
    public Guid InventoryItemId { get; set; }
    public decimal RequestedQuantity { get; set; }
    public decimal ApprovedQuantity { get; set; }
    public string? Notes { get; set; }

    protected InternalRequestLine() { }

    public InternalRequestLine(Guid id, Guid internalRequestId, Guid inventoryItemId, decimal requestedQuantity)
        : base(id)
    {
        InternalRequestId = internalRequestId;
        InventoryItemId = inventoryItemId;
        RequestedQuantity = requestedQuantity;
        ApprovedQuantity = 0; // Filled by Store Manager during approval
    }
}
