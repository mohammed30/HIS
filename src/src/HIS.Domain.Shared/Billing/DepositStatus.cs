namespace HIS.Billing;

public enum DepositStatus
{
    Active = 0,      // فعالة وقابلة للاستخدام
    Consumed = 1,    // تم خصمها بالكامل من الفاتورة النهائية
    Refunded = 2     // تم إرجاعها للمريض
}
