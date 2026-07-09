namespace HIS.Billing;

/// <summary>
/// حالة الفاتورة
/// </summary>
public enum InvoiceStatus
{
    /// <summary>مسودة</summary>
    Draft = 0,
    /// <summary>صادرة</summary>
    Issued = 1,
    /// <summary>مدفوعة جزئياً</summary>
    PartiallyPaid = 2,
    /// <summary>مدفوعة بالكامل</summary>
    Paid = 3,
    /// <summary>ملغية</summary>
    Cancelled = 4,
    /// <summary>مؤجلة</summary>
    Deferred = 5,
    /// <summary>مستردة</summary>
    Refunded = 6,
    /// <summary>في انتظار اعتماد المحاسب</summary>
    PendingApproval = 7,
    /// <summary>مرفوضة من المحاسب</summary>
    Rejected = 8,
    /// <summary>تم صرف الأصناف</summary>
    Dispensed = 9
}

/// <summary>
/// نوع الفاتورة
/// </summary>
public enum InvoiceType
{
    /// <summary>فاتورة بيع</summary>
    Sale = 0,
    /// <summary>فاتورة مرتجع</summary>
    Return = 1
}

/// <summary>
/// نوع الخدمة
/// </summary>
public enum ServiceType
{
    /// <summary>استشارة</summary>
    Consultation = 0,
    /// <summary>دواء</summary>
    Medication = 1,
    /// <summary>مختبر</summary>
    Laboratory = 2,
    /// <summary>أشعة</summary>
    Radiology = 3,
    /// <summary>إجراء</summary>
    Procedure = 4,
    /// <summary>تنويم</summary>
    Inpatient = 5,
    /// <summary>مستهلكات</summary>
    Consumables = 6,
    /// <summary>عملية جراحية</summary>
    Surgery = 7,
    /// <summary>جراحي</summary>
    Surgical = 8,
    /// <summary>أخرى</summary>
    Other = 99
}

/// <summary>
/// طريقة الدفع
/// </summary>
public enum PaymentMethod
{
    /// <summary>نقدي</summary>
    Cash = 0,
    /// <summary>بطاقة ائتمان</summary>
    CreditCard = 1,
    /// <summary>بطاقة مدى</summary>
    DebitCard = 2,
    /// <summary>تحويل بنكي</summary>
    BankTransfer = 3,
    /// <summary>شيك</summary>
    Check = 4,
    /// <summary>تأمين</summary>
    Insurance = 5,
    /// <summary>أخرى</summary>
    Other = 99
}

/// <summary>
/// حالة الدفع
/// </summary>
public enum PaymentStatus
{
    /// <summary>معلق</summary>
    Pending = 0,
    /// <summary>مكتمل</summary>
    Completed = 1,
    /// <summary>مرفوض</summary>
    Rejected = 2,
    /// <summary>مسترد</summary>
    Refunded = 3,
    /// <summary>ملغي</summary>
    Cancelled = 4
}

/// <summary>
/// حالة المؤجل
/// </summary>
public enum DeferredPaymentStatus
{
    /// <summary>نشط</summary>
    Active = 0,
    /// <summary>مسدد</summary>
    Settled = 1,
    /// <summary>متأخر</summary>
    Overdue = 2,
    /// <summary>معلق</summary>
    Suspended = 3,
    /// <summary>ملغي</summary>
    Cancelled = 4
}
