using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace HIS.Inventory.Dtos;

public class PurchaseOrderDto : AuditedEntityDto<Guid>
{
    public string OrderNumber { get; set; }
    public Guid SupplierId { get; set; }
    public string SupplierName { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public PurchaseOrderStatus Status { get; set; }
    public string ReferenceNumber { get; set; }
    public string Notes { get; set; }
    public decimal TotalAmount { get; set; }
    public List<PurchaseOrderLineDto> PurchaseOrderLines { get; set; }
}

public class PurchaseOrderLineDto : EntityDto<Guid>
{
    public Guid PurchaseOrderId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalAmount { get; set; }
    public string Description { get; set; }
}

public class CreateUpdatePurchaseOrderDto
{
    public Guid SupplierId { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public string ReferenceNumber { get; set; }
    public string Notes { get; set; }
    
    public List<CreateUpdatePurchaseOrderLineDto> PurchaseOrderLines { get; set; }
}

public class CreateUpdatePurchaseOrderLineDto
{
    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public string Description { get; set; }
}
