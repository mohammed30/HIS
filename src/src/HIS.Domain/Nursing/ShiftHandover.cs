using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace HIS.Nursing;

public class ShiftHandover : FullAuditedEntity<Guid>
{
    public ShiftType Shift { get; set; }
    public DateTime HandoverTime { get; set; }
    public string Notes { get; set; }
    public Guid OutgoingNurseId { get; set; } // Current User
    public Guid IncomingNurseId { get; set; }

    public ShiftHandover() { }
}
