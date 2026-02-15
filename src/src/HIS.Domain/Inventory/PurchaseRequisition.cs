using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace HIS.Inventory;

public class PurchaseRequisition : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public string RequisitionNumber { get; set; }
    public Guid RequestorId { get; set; }
    public Guid DepartmentId { get; set; }
    public DateTime RequiredDate { get; set; }
    public PurchaseRequisitionStatus Status { get; set; }
    public string? Notes { get; set; }
    
    public virtual ICollection<PurchaseRequisitionLine> Lines { get; set; }

    protected PurchaseRequisition() 
    {
        Lines = new List<PurchaseRequisitionLine>();
    }

    public PurchaseRequisition(
        Guid id, 
        string requisitionNumber, 
        Guid requestorId, 
        Guid departmentId, 
        DateTime requiredDate) : base(id)
    {
        RequisitionNumber = requisitionNumber;
        RequestorId = requestorId;
        DepartmentId = departmentId;
        RequiredDate = requiredDate;
        Status = PurchaseRequisitionStatus.Draft;
        Lines = new List<PurchaseRequisitionLine>();
    }
}
