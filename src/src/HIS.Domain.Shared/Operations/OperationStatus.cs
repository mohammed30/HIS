namespace HIS.Operations;

/// <summary>
/// حالة العملية
/// </summary>
public enum OperationStatus
{
    /// <summary>مجدولة</summary>
    Scheduled = 0,
    /// <summary>قيد التنفيذ</summary>
    InProgress = 1,
    /// <summary>مكتملة</summary>
    Completed = 2,
    /// <summary>ملغية</summary>
    Cancelled = 3
}
