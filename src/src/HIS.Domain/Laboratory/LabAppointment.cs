using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace HIS.Laboratory;

/// <summary>
/// موعد المعمل - Lab Appointment
/// </summary>
public class LabAppointment : FullAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// معرف المريض
    /// </summary>
    public Guid PatientId { get; private set; }
    
    /// <summary>
    /// معرف الخدمة/التحليل (اختياري - يمكن حجز موعد عام)
    /// </summary>
    public Guid? ServiceItemId { get; set; }
    
    /// <summary>
    /// تاريخ الموعد
    /// </summary>
    public DateTime AppointmentDate { get; set; }
    
    /// <summary>
    /// الوقت المفضل
    /// </summary>
    public TimeSpan? PreferredTime { get; set; }
    
    /// <summary>
    /// حالة الموعد
    /// </summary>
    public LabAppointmentStatus Status { get; set; }
    
    /// <summary>
    /// ملاحظات
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// تعليمات التحضير (منسوخة من ServiceItem)
    /// </summary>
    public string? PreparationInstructions { get; set; }
    
    /// <summary>
    /// هل يحتاج صيام؟
    /// </summary>
    public bool IsFasting { get; set; }

    protected LabAppointment() { }

    public LabAppointment(
        Guid id, 
        Guid patientId, 
        DateTime appointmentDate,
        Guid? serviceItemId = null) : base(id)
    {
        PatientId = patientId;
        AppointmentDate = appointmentDate;
        ServiceItemId = serviceItemId;
        Status = LabAppointmentStatus.Scheduled;
    }

    public void Confirm()
    {
        Status = LabAppointmentStatus.Confirmed;
    }

    public void CheckIn()
    {
        Status = LabAppointmentStatus.CheckedIn;
    }

    public void StartSampleCollection()
    {
        Status = LabAppointmentStatus.SampleCollecting;
    }

    public void Complete()
    {
        Status = LabAppointmentStatus.Completed;
    }

    public void Cancel()
    {
        Status = LabAppointmentStatus.Cancelled;
    }
}
