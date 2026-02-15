using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HIS.Rooms;

/// <summary>
/// الغرفة - Room Entity
/// </summary>
public class Room : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    /// <summary>
    /// رقم الغرفة
    /// </summary>
    public string RoomNumber { get; set; } = string.Empty;

    /// <summary>
    /// اسم الغرفة / الوصف
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// نوع الغرفة
    /// </summary>
    public RoomType Type { get; set; }

    /// <summary>
    /// عدد الأسرة
    /// </summary>
    public int BedCount { get; set; } = 1;

    /// <summary>
    /// الأسرة المتاحة حالياً
    /// </summary>
    public int AvailableBeds { get; set; } = 1;

    /// <summary>
    /// السعر اليومي
    /// </summary>
    public decimal DailyRate { get; set; }

    /// <summary>
    /// الطابق
    /// </summary>
    public string? Floor { get; set; }

    /// <summary>
    /// حالة الغرفة
    /// </summary>
    public RoomStatus Status { get; set; } = RoomStatus.Available;

    /// <summary>
    /// ملاحظات
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// وسائل الراحة (تكييف، ثلاجة، تلفاز، ...)
    /// </summary>
    public string? Amenities { get; set; }

    /// <summary>
    /// قائمة الأسرة
    /// </summary>
    public virtual ICollection<Bed> Beds { get; set; }

    protected Room() { }

    public Room(Guid id, Guid? tenantId, string roomNumber, RoomType type, decimal dailyRate, int bedCount = 1)
        : base(id)
    {
        TenantId = tenantId;
        RoomNumber = roomNumber;
        Type = type;
        DailyRate = dailyRate;
        BedCount = bedCount;
        AvailableBeds = bedCount;
        Beds = new Collection<Bed>();
    }
}
