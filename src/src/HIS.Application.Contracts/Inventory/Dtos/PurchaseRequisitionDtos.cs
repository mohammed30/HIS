using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace HIS.Inventory.Dtos;

public class PurchaseRequisitionDto : FullAuditedEntityDto<Guid>
{
    public string RequisitionNumber { get; set; }
    public Guid RequestorId { get; set; }
    public string RequestorName { get; set; }
    public Guid DepartmentId { get; set; }
    public string DepartmentName { get; set; }
    public DateTime RequiredDate { get; set; }
    public PurchaseRequisitionStatus Status { get; set; }
    public string? Notes { get; set; }
    public List<PurchaseRequisitionLineDto> Lines { get; set; }
}

public class PurchaseRequisitionLineDto : EntityDto<Guid>
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal Quantity { get; set; }
    public string? Description { get; set; }
}

public class CreateUpdatePurchaseRequisitionDto
{
    public Guid DepartmentId { get; set; }
    public DateTime RequiredDate { get; set; }
    public string? Notes { get; set; }
    public List<CreateUpdatePurchaseRequisitionLineDto> Lines { get; set; }
}

public class CreateUpdatePurchaseRequisitionLineDto
{
    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }
    public string? Description { get; set; }
}

public class GetPurchaseRequisitionsInput : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    public PurchaseRequisitionStatus? Status { get; set; }
    public Guid? DepartmentId { get; set; }
}
