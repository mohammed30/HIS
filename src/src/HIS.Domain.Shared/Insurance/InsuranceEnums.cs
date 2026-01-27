namespace HIS.Insurance;

/// <summary>
/// نوع خطة التأمين
/// </summary>
public enum InsurancePlanType
{
    /// <summary>فردي</summary>
    Individual = 0,
    /// <summary>عائلي</summary>
    Family = 1,
    /// <summary>شركات</summary>
    Corporate = 2,
    /// <summary>حكومي</summary>
    Government = 3
}

/// <summary>
/// حالة تأمين المريض
/// </summary>
public enum PatientInsuranceStatus
{
    /// <summary>نشط</summary>
    Active = 0,
    /// <summary>منتهي</summary>
    Expired = 1,
    /// <summary>ملغي</summary>
    Cancelled = 2,
    /// <summary>معلق</summary>
    Suspended = 3
}
