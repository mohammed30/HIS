namespace HIS.Accounting
{
    public enum AccountMappingType
    {
        SalesRevenue,         // حساب إيرادات المبيعات
        CashAccount,          // حساب الخزينة الافتراضي
        VATOutput,            // ضريبة مخرجات
        VATInput,             // ضريبة مدخلات
        Inventory,            // المخزون
        COGS,                 // تكلفة المبيعات
        PatientsReceivable,   // ذمم العملاء / المرضى
        InsuranceReceivable,  // ذمم شركات التأمين
        InsuranceDiscounts,   // خصومات وفروقات التأمين
        InventoryAdjustment,  // تسوية عجز وزيادة المخزون
        AccruedInventory,     // البضاعة المستلمة غير المفوترة
        CardPaymentBank,      // حساب البنك لشبكة نقاط البيع
        PatientDeposits       // أمانات ودفعات مقدمة للمرضى
    }
}
