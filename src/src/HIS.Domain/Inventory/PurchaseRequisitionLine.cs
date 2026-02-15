using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace HIS.Inventory;

public class PurchaseRequisitionLine : FullAuditedEntity<Guid>
{
    public Guid PurchaseRequisitionId { get; set; }
    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }
    public string? Description { get; set; }

    protected PurchaseRequisitionLine() { }

    public PurchaseRequisitionLine(Guid id, Guid purchaseRequisitionId, Guid productId, decimal quantity) : base(id)
    {
        PurchaseRequisitionId = purchaseRequisitionId;
        ProductId = productId;
        Quantity = quantity;
    }
}
