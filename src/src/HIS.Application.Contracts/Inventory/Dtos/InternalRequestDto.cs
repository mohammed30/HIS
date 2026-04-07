using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace HIS.Inventory.Dtos;

public class InternalRequestDto : FullAuditedEntityDto<Guid>
{
    public string RequestNumber { get; set; }
    public Guid RequestingDepartmentId { get; set; }
    public string RequestingDepartmentName { get; set; }
    public Guid FulfilledByWarehouseId { get; set; }
    public string FulfilledByWarehouseName { get; set; }
    
    public Guid? AdmissionId { get; set; }
    public string PatientName { get; set; }
    
    public DateTime RequestDate { get; set; }
    public InternalRequestStatus Status { get; set; }
    public string Notes { get; set; }

    public List<InternalRequestLineDto> Lines { get; set; }
}

public class InternalRequestLineDto : FullAuditedEntityDto<Guid>
{
    public Guid InternalRequestId { get; set; }
    public Guid InventoryItemId { get; set; }
    public string InventoryItemName { get; set; }
    public decimal RequestedQuantity { get; set; }
    public decimal ApprovedQuantity { get; set; }
    public string Notes { get; set; }
}

public class CreateUpdateInternalRequestDto
{
    public Guid RequestingDepartmentId { get; set; }
    public Guid FulfilledByWarehouseId { get; set; }
    public Guid? AdmissionId { get; set; }
    public DateTime RequestDate { get; set; }
    public string Notes { get; set; }

    public List<CreateUpdateInternalRequestLineDto> Lines { get; set; }
}

public class CreateUpdateInternalRequestLineDto
{
    public Guid InventoryItemId { get; set; }
    public decimal RequestedQuantity { get; set; }
    public string Notes { get; set; }
}
