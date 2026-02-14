using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace HIS.Nursing;

public class PatientRound : FullAuditedEntity<Guid>
{
    public Guid PatientId { get; set; }
    public string Note { get; set; }
    public Guid? NurseId { get; set; } // Ideally linked to IdentityUser

    public PatientRound() { }

    public PatientRound(Guid id, Guid patientId, string note) : base(id)
    {
        PatientId = patientId;
        Note = note;
    }
}
