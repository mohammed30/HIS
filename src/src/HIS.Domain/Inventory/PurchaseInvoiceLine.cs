using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace HIS.Inventory;

public class PurchaseInvoiceLine : FullAuditedEntity<Guid>
{
    public Guid PurchaseInvoiceId { get; set; }
    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalLineAmount { get; set; }
    
    public string BatchNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }

    protected PurchaseInvoiceLine() { }

    public PurchaseInvoiceLine(
        Guid id, 
        Guid purchaseInvoiceId, 
        Guid productId, 
        decimal quantity, 
        decimal unitCost) : base(id)
    {
        PurchaseInvoiceId = purchaseInvoiceId;
        ProductId = productId;
        Quantity = quantity;
        UnitCost = unitCost;
        TotalLineAmount = quantity * unitCost;
    }
}
