namespace HIS.Rooms;

/// <summary>
/// حالة السرير
/// </summary>
public enum BedStatus
{
    /// <summary>متاح</summary>
    Available = 0,
    /// <summary>مشغول</summary>
    Occupied = 1,
    /// <summary>محجوز</summary>
    Reserved = 2,
    /// <summary>صيانة</summary>
    Maintenance = 3,
    /// <summary>تنظيف</summary>
    Cleaning = 4,
    /// <summary>معطل</summary>
    OutOfService = 5
}
