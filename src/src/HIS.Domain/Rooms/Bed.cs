using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace HIS.Rooms;

/// <summary>
/// السرير - Bed Entity
/// </summary>
public class Bed : FullAuditedEntity<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    public Guid RoomId { get; set; }

    /// <summary>
    /// رقم السرير
    /// </summary>
    public string BedNumber { get; set; } = string.Empty;

    /// <summary>
    /// حالة السرير
    /// </summary>
    public BedStatus Status { get; set; } = BedStatus.Available;

    /// <summary>
    /// نوع السرير
    /// </summary>
    /// <remarks>
    /// يمكن أن يختلف عن نوع الغرفة (مثلاً سرير عناية في غرفة عادية مؤقتاً)
    /// </remarks>
    public BedType Type { get; set; }

    protected Bed() { }

    public Bed(Guid id, Guid? tenantId, Guid roomId, string bedNumber, BedType type, BedStatus status = BedStatus.Available)
        : base(id)
    {
        TenantId = tenantId;
        RoomId = roomId;
        BedNumber = bedNumber;
        Type = type;
        Status = status;
    }
}
