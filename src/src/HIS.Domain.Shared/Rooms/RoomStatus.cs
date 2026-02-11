namespace HIS.Rooms;

/// <summary>
/// حالة الغرفة
/// </summary>
public enum RoomStatus
{
    /// <summary>متاحة</summary>
    Available = 0,
    /// <summary>مشغولة</summary>
    Occupied = 1,
    /// <summary>محجوزة</summary>
    Reserved = 2,
    /// <summary>صيانة</summary>
    Maintenance = 3,
    /// <summary>معطلة</summary>
    OutOfService = 4
}
