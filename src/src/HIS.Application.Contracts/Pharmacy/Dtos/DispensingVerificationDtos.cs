using System;
using Volo.Abp.Application.Dtos;
using HIS.Pharmacy;

namespace HIS.Pharmacy.Dtos;

public class DispensingVerificationDto : FullAuditedEntityDto<Guid>
{
    public Guid MedicalOrderId { get; set; }
    public Guid? PharmacistId { get; set; }
    public string? PharmacistName { get; set; }
    public DateTime VerificationTime { get; set; }
    public bool IsApproved { get; set; }
    public string? SafetyCheckComments { get; set; }
    public VerificationStatus Status { get; set; }
}

public class VerifyPrescriptionDto
{
    public Guid MedicalOrderId { get; set; } // The Prescription ID
    public bool IsApproved { get; set; }
    public string? SafetyCheckComments { get; set; }
}
