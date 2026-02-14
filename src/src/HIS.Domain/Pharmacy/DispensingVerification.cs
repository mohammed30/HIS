using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace HIS.Pharmacy;

public class DispensingVerification : FullAuditedAggregateRoot<Guid>
{
    public Guid MedicalOrderId { get; set; } // Link to Prescription
    public Guid? PharmacistId { get; set; }
    public DateTime VerificationTime { get; set; }
    public bool IsApproved { get; set; }
    public string? SafetyCheckComments { get; set; } // Interaction overrides, dosage warnings
    public VerificationStatus Status { get; set; }

    protected DispensingVerification() { }

    public DispensingVerification(Guid id, Guid medicalOrderId, Guid pharmacistId, bool isApproved, string? comments)
        : base(id)
    {
        MedicalOrderId = medicalOrderId;
        PharmacistId = pharmacistId;
        VerificationTime = DateTime.UtcNow;
        IsApproved = isApproved;
        SafetyCheckComments = comments;
        Status = isApproved ? VerificationStatus.Verified : VerificationStatus.Rejected;
    }
}


