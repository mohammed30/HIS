using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace HIS.Laboratory;

public class LabRequest : FullAuditedAggregateRoot<Guid>
{
    public Guid PatientId { get; private set; }
    public Guid DoctorId { get; private set; }
    
    /// <summary>
    /// معرف الخدمة (التحليل) من جدول الخدمات الموحد
    /// </summary>
    public Guid ServiceItemId { get; private set; }
    
    public DateTime RequestDate { get; private set; }
    public LabRequestStatus Status { get; set; }
    
    public string? Result { get; set; }
    public string? SampleNumber { get; set; }
    public string? Notes { get; set; }

    protected LabRequest() { }

    public LabRequest(Guid id, Guid patientId, Guid doctorId, Guid serviceItemId) : base(id)
    {
        PatientId = patientId;
        DoctorId = doctorId;
        ServiceItemId = serviceItemId;
        RequestDate = DateTime.Now;
        Status = LabRequestStatus.Requested;
    }
}
