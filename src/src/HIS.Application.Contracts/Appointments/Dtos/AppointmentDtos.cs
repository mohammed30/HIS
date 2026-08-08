using System;
using Volo.Abp.Application.Dtos;

namespace HIS.Appointments.Dtos;

public class AppointmentDto : AuditedEntityDto<Guid>
{
    public Guid PatientId { get; set; }
    public string PatientName { get; set; }
    public Guid DoctorId { get; set; }
    public string DoctorName { get; set; }
    public Guid ClinicId { get; set; }
    public string ClinicName { get; set; }
    
    public DateTime AppointmentDate { get; set; }
    public AppointmentStatus Status { get; set; }
    public AppointmentType Type { get; set; }
    public bool IsWalkIn { get; set; }
    public decimal ConsultationFee { get; set; }
    
    public string Notes { get; set; }
}

public class CreateAppointmentDto
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid ClinicId { get; set; }
    
    public DateTime AppointmentDate { get; set; }
    public AppointmentType Type { get; set; }
    public bool IsWalkIn { get; set; }
    public decimal ConsultationFee { get; set; }
    
    public string Notes { get; set; }
}

public class WaitingListDto : AuditedEntityDto<Guid>
{
    public Guid PatientId { get; set; }
    public string PatientName { get; set; }
    public Guid? DoctorId { get; set; }
    public string DoctorName { get; set; }
    public Guid DepartmentId { get; set; }
    public string DepartmentName { get; set; }
    
    public DateTime RequestDate { get; set; }
    public WaitingListPriority Priority { get; set; }
    public string Notes { get; set; }
    public bool IsResolved { get; set; }
}

public class CreateUpdateWaitingListDto
{
    public Guid PatientId { get; set; }
    public Guid? DoctorId { get; set; }
    public Guid DepartmentId { get; set; }
    
    public DateTime RequestDate { get; set; }
    public WaitingListPriority Priority { get; set; }
    public string Notes { get; set; }
    public bool IsResolved { get; set; }
}

public class BookClinicAppointmentDto
{
    public Guid PatientId { get; set; }
    public Guid ClinicId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid? ServiceItemId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public AppointmentType Type { get; set; } = AppointmentType.NewVisit;
    public bool IsWalkIn { get; set; } = true; // Default to true for lab reception as it's often more flexible
    
    public bool CreateInvoice { get; set; } = true;
    public string PaymentMethod { get; set; } = "Cash"; // Cash, Card, etc.
    public decimal? PaidAmount { get; set; }
    public decimal? Discount { get; set; }
    
    public decimal InsurancePercentage { get; set; }
    public Guid? PatientInsuranceId { get; set; }
}
