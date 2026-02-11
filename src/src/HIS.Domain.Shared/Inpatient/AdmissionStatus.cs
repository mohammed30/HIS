namespace HIS.Inpatient;

/// <summary>
/// حالة التنويم
/// </summary>
public enum AdmissionStatus
{
    /// <summary>منوم - نشط</summary>
    Active = 0,
    /// <summary>مخرج</summary>
    Discharged = 1,
    /// <summary>محول</summary>
    Transferred = 2,
    /// <summary>ملغي</summary>
    Cancelled = 3
}
