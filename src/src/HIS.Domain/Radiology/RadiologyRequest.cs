using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace HIS.Radiology;

public class RadiologyRequest : FullAuditedAggregateRoot<Guid>
{
    public Guid PatientId { get; private set; }
    public Guid? DoctorId { get; private set; }
    public bool IsExternalDoctor { get; private set; }
    public string? ExternalDoctorName { get; private set; }
    
    /// <summary>
    /// Service Item ID from RadiologyItems
    /// </summary>
    public Guid RadiologyItemId { get; private set; }
    
    public DateTime RequestDate { get; private set; }
    public RadiologyRequestStatus Status { get; set; }
    
    /// <summary>
    /// The final medical report typed by the radiologist
    /// </summary>
    public string? ReportBody { get; set; }
    
    /// <summary>
    /// Any notes entered by the technician during the exam
    /// </summary>
    public string? TechnicianNotes { get; set; }
    
    public DateTime? ReportDate { get; set; }
    public Guid? RadiologistId { get; set; }
    
    public string? RequestNumber { get; set; }

    protected RadiologyRequest() { }

    public RadiologyRequest(Guid id, Guid patientId, Guid? doctorId, Guid radiologyItemId, string? requestNumber = null, bool isExternalDoctor = false, string? externalDoctorName = null) : base(id)
    {
        PatientId = patientId;
        DoctorId = doctorId;
        RadiologyItemId = radiologyItemId;
        RequestNumber = requestNumber;
        IsExternalDoctor = isExternalDoctor;
        ExternalDoctorName = externalDoctorName;
        RequestDate = DateTime.Now;
        Status = RadiologyRequestStatus.Requested;
    }
}
