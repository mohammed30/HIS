namespace HIS.Inpatient;

/// <summary>
/// حالة الحجز
/// </summary>
public enum ReservationStatus
{
    /// <summary>قيد الانتظار</summary>
    Pending = 0,
    /// <summary>مؤكد</summary>
    Confirmed = 1,
    /// <summary>ملغى</summary>
    Cancelled = 2,
    /// <summary>مكتمل - تم التنويم</summary>
    Completed = 3,
    /// <summary>فائت الموعد</summary>
    NoShow = 4
}
