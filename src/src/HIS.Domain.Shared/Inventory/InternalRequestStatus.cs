namespace HIS.Inventory;

/// <summary>
/// حالة الطلب الداخلي
/// </summary>
public enum InternalRequestStatus
{
    /// <summary>مسودة / قيد التجهيز من قبل القسم الطالب</summary>
    Draft = 0,
    /// <summary>مرسل للإعتماد من قبل المستودع الرئيسي</summary>
    Submitted = 1,
    /// <summary>معتمد (تمت الموافقة وصرف الكمية من المستودع)</summary>
    Approved = 2,
    /// <summary>مستلم (تم التأكيد من قبل القسم الطالب)</summary>
    Received = 3,
    /// <summary>مرفوض</summary>
    Rejected = 4,
    /// <summary>ملغي</summary>
    Cancelled = 5
}
