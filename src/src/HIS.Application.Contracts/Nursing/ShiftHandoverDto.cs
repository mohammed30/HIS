using System;
using Volo.Abp.Application.Dtos;

namespace HIS.Nursing;

public class ShiftHandoverDto : FullAuditedEntityDto<Guid>
{
    public ShiftType Shift { get; set; }
    public DateTime HandoverTime { get; set; }
    public string Notes { get; set; }
    public Guid OutgoingNurseId { get; set; }
    public Guid IncomingNurseId { get; set; }
}

public class CreateShiftHandoverDto
{
    public ShiftType Shift { get; set; }
    public string Notes { get; set; }
    // IncomingNurseId and OutgoingNurseId might be handled via current user or selection
    public Guid IncomingNurseId { get; set; }
}
