namespace HIS.Inventory;

/// <summary>
/// نوع الطلب للمريض المنوم
/// </summary>
public enum InternalRequestType
{
    /// <summary>أدوية</summary>
    Medication = 0,
    /// <summary>مستلزمات طبية</summary>
    Consumable = 1,
    /// <summary>تحاليل مخبرية</summary>
    Laboratory = 2,
    /// <summary>أشعة</summary>
    Radiology = 3,
    /// <summary>أخرى</summary>
    Other = 4
}
