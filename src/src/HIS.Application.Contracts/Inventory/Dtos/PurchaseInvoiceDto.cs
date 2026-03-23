using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace HIS.Inventory.Dtos;

public class PurchaseInvoiceDto : FullAuditedEntityDto<Guid>
{
    public string InvoiceNumber { get; set; }
    public Guid SupplierId { get; set; }
    public string SupplierName { get; set; }
    public Guid? PurchaseOrderId { get; set; }
    public string PurchaseOrderNumber { get; set; }
    public DateTime InvoiceDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal NetAmount { get; set; }
    public PurchaseInvoiceStatus Status { get; set; }
    public string Notes { get; set; }
    public List<PurchaseInvoiceLineDto> Lines { get; set; }
}

public class PurchaseInvoiceLineDto : EntityDto<Guid>
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalLineAmount { get; set; }
    public string BatchNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
}

public class CreateUpdatePurchaseInvoiceDto
{
    public string InvoiceNumber { get; set; }
    public Guid SupplierId { get; set; }
    public Guid? PurchaseOrderId { get; set; }
    public DateTime InvoiceDate { get; set; }
    public string Notes { get; set; }
    public List<CreateUpdatePurchaseInvoiceLineDto> Lines { get; set; } = new();
}

public class CreateUpdatePurchaseInvoiceLineDto
{
    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal Discount { get; set; }
    public string BatchNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
}
