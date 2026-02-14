using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;

namespace HIS.Inventory;

public class PurchaseOrder : FullAuditedAggregateRoot<Guid>
{
    public string OrderNumber { get; set; }
    public Guid SupplierId { get; set; }
    public virtual Supplier Supplier { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public PurchaseOrderStatus Status { get; set; }
    public string ReferenceNumber { get; set; } // e.g. Supplier's Quotation Ref
    public string Notes { get; set; }
    public decimal TotalAmount { get; set; }
    
    public ICollection<PurchaseOrderLine> PurchaseOrderLines { get; set; }

    protected PurchaseOrder() { }

    public PurchaseOrder(Guid id, string orderNumber, Guid supplierId, DateTime orderDate) : base(id)
    {
        OrderNumber = orderNumber;
        SupplierId = supplierId;
        OrderDate = orderDate;
        Status = PurchaseOrderStatus.Draft;
        PurchaseOrderLines = new List<PurchaseOrderLine>();
    }
}
