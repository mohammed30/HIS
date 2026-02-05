using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace HIS.Clinical;

public class MedicalOrder : FullAuditedEntity<Guid>
{
    public Guid PatientId { get; set; }
    public Guid? DoctorId { get; set; }
    public OrderType Type { get; set; }
    public OrderStatus Status { get; set; }
    
    public Guid ServiceItemId { get; set; }
    public string ServiceName { get; set; } // Snapshot in case item name changes
    public decimal Price { get; set; }      // Snapshot price
    
    public string ClinicalNotes { get; set; }
    
    // For Radiology: Modality, BodyPart (can be copied from ServiceItem or specific to order)
    public string Details { get; set; } 

    public MedicalOrder()
    {
    }

    public MedicalOrder(Guid id, Guid patientId, OrderType type, Guid serviceItemId, string serviceName, decimal price) 
        : base(id)
    {
        PatientId = patientId;
        Type = type;
        ServiceItemId = serviceItemId;
        ServiceName = serviceName;
        Price = price;
        Status = OrderStatus.Pending;
    }
}
