using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;

namespace HIS.Inventory;

public class PurchaseInvoice : FullAuditedAggregateRoot<Guid>
{
    public string InvoiceNumber { get; set; }
    public Guid SupplierId { get; set; }
    public virtual Supplier Supplier { get; set; }
    public Guid? PurchaseOrderId { get; set; }
    public DateTime InvoiceDate { get; set; }
    
    public decimal TotalAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal NetAmount { get; set; }
    
    public PurchaseInvoiceStatus Status { get; set; }
    public string Notes { get; set; }

    public ICollection<PurchaseInvoiceLine> Lines { get; set; }

    protected PurchaseInvoice() { }

    public PurchaseInvoice(
        Guid id, 
        string invoiceNumber, 
        Guid supplierId, 
        DateTime invoiceDate, 
        Guid? purchaseOrderId = null) : base(id)
    {
        InvoiceNumber = invoiceNumber;
        SupplierId = supplierId;
        InvoiceDate = invoiceDate;
        PurchaseOrderId = purchaseOrderId;
        Status = PurchaseInvoiceStatus.Draft;
        Lines = new List<PurchaseInvoiceLine>();
    }
}
