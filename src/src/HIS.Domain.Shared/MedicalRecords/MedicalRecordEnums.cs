namespace HIS.MedicalRecords;

/// <summary>
/// نوع التشخيص
/// </summary>
public enum DiagnosisType
{
    /// <summary>رئيسي</summary>
    Primary = 0,
    /// <summary>ثانوي</summary>
    Secondary = 1,
    /// <summary>تفريقي</summary>
    Differential = 2
}

/// <summary>
/// حالة التشخيص
/// </summary>
public enum DiagnosisStatus
{
    /// <summary>نشط</summary>
    Active = 0,
    /// <summary>تم الشفاء</summary>
    Resolved = 1,
    /// <summary>مزمن</summary>
    Chronic = 2
}

/// <summary>
/// نوع المادة المسببة للحساسية
/// </summary>
public enum AllergenType
{
    /// <summary>دواء</summary>
    Drug = 0,
    /// <summary>طعام</summary>
    Food = 1,
    /// <summary>بيئي</summary>
    Environmental = 2,
    /// <summary>أخرى</summary>
    Other = 99
}

/// <summary>
/// شدة الحساسية
/// </summary>
public enum AllergySeverity
{
    /// <summary>خفيفة</summary>
    Mild = 0,
    /// <summary>متوسطة</summary>
    Moderate = 1,
    /// <summary>شديدة</summary>
    Severe = 2,
    /// <summary>مهددة للحياة</summary>
    LifeThreatening = 3
}

/// <summary>
/// حالة الحساسية
/// </summary>
public enum AllergyStatus
{
    /// <summary>نشطة</summary>
    Active = 0,
    /// <summary>تم الشفاء</summary>
    Resolved = 1
}

/// <summary>
/// نوع الملاحظة الطبية
/// </summary>
public enum NoteType
{
    /// <summary>متابعة</summary>
    Progress = 0,
    /// <summary>استشارة</summary>
    Consultation = 1,
    /// <summary>خروج</summary>
    Discharge = 2,
    /// <summary>تحويل</summary>
    Referral = 3,
    /// <summary>أخرى</summary>
    Other = 99
}
