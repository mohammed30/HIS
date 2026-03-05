namespace HIS.HR.Enums;

public enum Gender
{
    Male = 1,
    Female = 2
}

public enum MaritalStatus
{
    Single = 1,
    Married = 2,
    Divorced = 3,
    Widowed = 4
}

public enum IdentityDocumentType
{
    NationalId = 1,
    Passport = 2,
    ResidencyPermit = 3,
    DrivingLicense = 4,
    NationalityCard = 5
}

public enum SalaryPaymentMethod
{
    Cash = 1,
    BankTransfer = 2,
    Check = 3
}

public enum ContractType
{
    Permanent = 1,
    Temporary = 2,
    PartTime = 3,
    Probation = 4
}

public enum CompensationNature
{
    /// <summary>
    /// بدل / استحقاق
    /// </summary>
    Allowance = 1,

    /// <summary>
    /// استقطاع / خصم
    /// </summary>
    Deduction = 2
}

public enum CompensationMethod
{
    /// <summary>
    /// دائن (Credit)
    /// </summary>
    Credit = 1,

    /// <summary>
    /// مدين (Debit)
    /// </summary>
    Debit = 2
}

public enum CompensationValueType
{
    Fixed = 1,
    Percentage = 2,
    Equation = 3
}

public enum PenaltyType
{
    Warning = 1,
    SalaryDeduction = 2,
    Suspension = 3,
    Termination = 4
}

public enum PayrollRunStatus
{
    Draft = 1,
    Processed = 2,
    Posted = 3
}

public enum LoanStatus
{
    Active = 1,
    PaidOff = 2,
    Cancelled = 3
}
