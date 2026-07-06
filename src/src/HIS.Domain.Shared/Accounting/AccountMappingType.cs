namespace HIS.Accounting
{
    public enum AccountMappingType
    {
        SalesRevenue,  // حساب إيرادات المبيعات
        CashAccount,   // حساب الخزينة الافتراضي
        VATOutput,     // ضريبة مخرجات
        VATInput,      // ضريبة مدخلات
        Inventory,     // المخزون
        COGS           // تكلفة المبيعات
    }
}
