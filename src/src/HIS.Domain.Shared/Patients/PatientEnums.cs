namespace HIS.Patients;

/// <summary>
/// جنس المريض
/// </summary>
public enum Gender
{
    /// <summary>
    /// ذكر
    /// </summary>
    Male = 0,

    /// <summary>
    /// أنثى
    /// </summary>
    Female = 1
}

/// <summary>
/// الحالة الاجتماعية
/// </summary>
public enum MaritalStatus
{
    /// <summary>
    /// أعزب
    /// </summary>
    Single = 0,

    /// <summary>
    /// متزوج
    /// </summary>
    Married = 1,

    /// <summary>
    /// مطلق
    /// </summary>
    Divorced = 2,

    /// <summary>
    /// أرمل
    /// </summary>
    Widowed = 3
}

/// <summary>
/// نوع وثيقة الهوية
/// </summary>
public enum IdentityType
{
    /// <summary>
    /// هوية وطنية
    /// </summary>
    NationalId = 0,

    /// <summary>
    /// جواز سفر
    /// </summary>
    Passport = 1,

    /// <summary>
    /// إقامة
    /// </summary>
    ResidencePermit = 2,

    /// <summary>
    /// رخصة قيادة
    /// </summary>
    DrivingLicense = 3
}

/// <summary>
/// فئة المريض
/// </summary>
public enum PatientCategory
{
    /// <summary>
    /// عادي
    /// </summary>
    Regular = 0,

    /// <summary>
    /// VIP
    /// </summary>
    VIP = 1,

    /// <summary>
    /// موظف
    /// </summary>
    Employee = 2,

    /// <summary>
    /// متقاعد
    /// </summary>
    Retired = 3
}
