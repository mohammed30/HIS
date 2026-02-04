namespace HIS.Laboratory;

public enum LabAppointmentStatus
{
    /// <summary>
    /// تم الحجز
    /// </summary>
    Scheduled = 0,
    
    /// <summary>
    /// تم التأكيد
    /// </summary>
    Confirmed = 1,
    
    /// <summary>
    /// وصول المريض
    /// </summary>
    CheckedIn = 2,
    
    /// <summary>
    /// جاري جمع العينة
    /// </summary>
    SampleCollecting = 3,
    
    /// <summary>
    /// مكتمل
    /// </summary>
    Completed = 4,
    
    /// <summary>
    /// ملغي
    /// </summary>
    Cancelled = 5
}
