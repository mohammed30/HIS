using System;
using Volo.Abp.Application.Dtos;

namespace HIS.Clinical;

public class MedicalOrderDto : AuditedEntityDto<Guid>
{
    public Guid PatientId { get; set; }
    public Guid? DoctorId { get; set; }
    public OrderType Type { get; set; }
    public OrderStatus Status { get; set; }
    
    public Guid ServiceItemId { get; set; }
    public string ServiceName { get; set; }
    public decimal Price { get; set; }
    
    public string ClinicalNotes { get; set; }
    public string Details { get; set; }
}
