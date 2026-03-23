using System;
using System.Collections.Generic;
using HIS.HR.Enums;
using Volo.Abp.Application.Dtos;

namespace HIS.HR;

// ===== Employee =====
public class EmployeeDto : AuditedEntityDto<Guid>
{
    public string EmployeeNumber { get; set; }
    public string NameAr { get; set; }
    public string? NameEn { get; set; }
    public Gender Gender { get; set; }
    public DateTime? BirthDate { get; set; }
    public MaritalStatus? MaritalStatus { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Qualification { get; set; }
    public IdentityDocumentType? IdentityType { get; set; }
    public string? IdentityNumber { get; set; }
    public string? InsuranceNumber { get; set; }
    public string? BankName { get; set; }
    public string? BankAccountNumber { get; set; }
    public string? IBAN { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public string? SectionName { get; set; }
    public Guid? JobGradeId { get; set; }
    public string? JobGradeName { get; set; }
    public Guid? JobTitleId { get; set; }
    public string? JobTitleName { get; set; }
    public string? JobTitle { get; set; }
    public string? EmploymentClassification { get; set; }
    public SalaryPaymentMethod? SalaryPaymentMethod { get; set; }
    public ContractType? ContractType { get; set; }
    public DateTime? HireDate { get; set; }
    public DateTime? TerminationDate { get; set; }
    public bool IsSuspended { get; set; }
    public bool IsActive { get; set; }
    public decimal? BasicSalary { get; set; }
}

public class CreateUpdateEmployeeDto
{
    public string? EmployeeNumber { get; set; }
    public string NameAr { get; set; }
    public string? NameEn { get; set; }
    public Gender Gender { get; set; }
    public DateTime? BirthDate { get; set; }
    public MaritalStatus? MaritalStatus { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Qualification { get; set; }
    public IdentityDocumentType? IdentityType { get; set; }
    public string? IdentityNumber { get; set; }
    public string? InsuranceNumber { get; set; }
    public string? BankName { get; set; }
    public string? BankAccountNumber { get; set; }
    public string? IBAN { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? SectionName { get; set; }
    public Guid? JobGradeId { get; set; }
    public Guid? JobTitleId { get; set; }
    public string? JobTitle { get; set; }
    public string? EmploymentClassification { get; set; }
    public SalaryPaymentMethod? SalaryPaymentMethod { get; set; }
    public ContractType? ContractType { get; set; }
    public DateTime? HireDate { get; set; }
    public DateTime? TerminationDate { get; set; }
    public bool ReminderEnabled { get; set; }
    public bool IsSuspended { get; set; }
    public bool IsActive { get; set; } = true;
    public decimal? BasicSalary { get; set; }
}

// ===== JobGrade =====
public class JobGradeDto : AuditedEntityDto<Guid>
{
    public string Code { get; set; }
    public string NameAr { get; set; }
    public string? NameEn { get; set; }
    public decimal BaseSalary { get; set; }
    public bool IsActive { get; set; }
}

public class CreateUpdateJobGradeDto
{
    public string Code { get; set; }
    public string NameAr { get; set; }
    public string? NameEn { get; set; }
    public decimal BaseSalary { get; set; }
    public bool IsActive { get; set; } = true;
}

// ===== CompensationItem =====
public class CompensationItemDto : AuditedEntityDto<Guid>
{
    public string NameAr { get; set; }
    public string? DisplayName { get; set; }
    public CompensationNature Nature { get; set; }
    public CompensationValueType ValueType { get; set; }
    public CompensationMethod Method { get; set; }
    public string? FormulaExpression { get; set; }
    public Guid? AccountId { get; set; }
    public string? AccountName { get; set; }
    public bool IsActive { get; set; }
}

public class CreateUpdateCompensationItemDto
{
    public string NameAr { get; set; }
    public string? DisplayName { get; set; }
    public CompensationNature Nature { get; set; }
    public CompensationValueType ValueType { get; set; }
    public CompensationMethod Method { get; set; }
    public string? FormulaExpression { get; set; }
    public Guid? AccountId { get; set; }
    public bool IsActive { get; set; } = true;
}

// ===== LeaveType =====
public class LeaveTypeDto : AuditedEntityDto<Guid>
{
    public string NameAr { get; set; }
    public int Duration { get; set; }
    public string? EmployeeClass { get; set; }
    public bool AffectsSalary { get; set; }
    public bool IsBalance { get; set; }
    public bool IsPublicHoliday { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
}

public class CreateUpdateLeaveTypeDto
{
    public string NameAr { get; set; }
    public int Duration { get; set; }
    public string? EmployeeClass { get; set; }
    public bool AffectsSalary { get; set; }
    public bool IsBalance { get; set; }
    public bool IsPublicHoliday { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; } = true;
}

// ===== EmployeeLeave =====
public class EmployeeLeaveDto : AuditedEntityDto<Guid>
{
    public Guid EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public Guid LeaveTypeId { get; set; }
    public string? LeaveTypeName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Duration { get; set; }
    public int Entitled { get; set; }
    public int Used { get; set; }
    public int Balance { get; set; }
    public string? Notes { get; set; }
}

public class CreateUpdateEmployeeLeaveDto
{
    public Guid EmployeeId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid LeaveTypeId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Duration { get; set; }
    public string? Notes { get; set; }
}

// ===== EmployeeLoan =====
public class EmployeeLoanDto : AuditedEntityDto<Guid>
{
    public Guid EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? CompensationItemId { get; set; }
    public string? CompensationItemName { get; set; }
    public decimal Amount { get; set; }
    public int Installments { get; set; }
    public DateTime StartDate { get; set; }
    public LoanStatus Status { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RemainingAmount => Amount - PaidAmount;
    public string? Notes { get; set; }
}

public class CreateUpdateEmployeeLoanDto
{
    public Guid EmployeeId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? CompensationItemId { get; set; }
    public decimal Amount { get; set; }
    public int Installments { get; set; } = 1;
    public DateTime StartDate { get; set; }
    public string? Notes { get; set; }
}

// ===== SalarySetup =====
public class SalarySetupDto : AuditedEntityDto<Guid>
{
    public Guid EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public Guid CompensationItemId { get; set; }
    public string? CompensationItemName { get; set; }
    public decimal Amount { get; set; }
    public bool IsRecurring { get; set; }
    public DateTime? StartDate { get; set; }
    public bool IsActive { get; set; }
}

public class CreateUpdateSalarySetupDto
{
    public Guid EmployeeId { get; set; }
    public Guid CompensationItemId { get; set; }
    public decimal Amount { get; set; }
    public bool IsRecurring { get; set; } = true;
    public DateTime? StartDate { get; set; }
    public bool IsActive { get; set; } = true;
}

// ===== PayrollRun =====
public class PayrollRunDto : AuditedEntityDto<Guid>
{
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public Guid? JobGradeId { get; set; }
    public PayrollRunStatus Status { get; set; }
    public Guid? JournalEntryId { get; set; }
    public decimal TotalEarnings { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal NetSalary { get; set; }
}

public class ProcessPayrollDto
{
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? JobGradeId { get; set; }
}

// ===== Penalty =====
public class PenaltyDto : AuditedEntityDto<Guid>
{
    public Guid EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public PenaltyType PenaltyType { get; set; }
    public string? Description { get; set; }
    public decimal? Amount { get; set; }
    public int? SuspensionDays { get; set; }
    public DateTime Date { get; set; }
    public string? Notes { get; set; }
}

public class CreateUpdatePenaltyDto
{
    public Guid EmployeeId { get; set; }
    public PenaltyType PenaltyType { get; set; }
    public string? Description { get; set; }
    public decimal? Amount { get; set; }
    public int? SuspensionDays { get; set; }
    public DateTime Date { get; set; }
    public string? Notes { get; set; }
}

// ===== AttendanceRecord =====
public class AttendanceRecordDto : AuditedEntityDto<Guid>
{
    public Guid EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public string? PermitType { get; set; }
    public DateTime Date { get; set; }
    public int? Hours { get; set; }
    public int? Minutes { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
}

public class CreateUpdateAttendanceRecordDto
{
    public Guid EmployeeId { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? PermitType { get; set; }
    public DateTime Date { get; set; }
    public int? Hours { get; set; }
    public int? Minutes { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
}

// ===== DailyAttendance (حضور وانصراف) =====
public class DailyAttendanceDto : AuditedEntityDto<Guid>
{
    public Guid EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public string? EmployeeNumber { get; set; }
    public string? DepartmentName { get; set; }
    public DateTime Date { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public Enums.AttendanceStatus Status { get; set; }
    public decimal WorkedHours { get; set; }
    public decimal OvertimeHours { get; set; }
    public string? Notes { get; set; }
}

public class CreateUpdateDailyAttendanceDto
{
    public Guid EmployeeId { get; set; }
    public DateTime Date { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public Enums.AttendanceStatus Status { get; set; }
    public decimal? OvertimeHours { get; set; }
    public string? Notes { get; set; }
}

// ===== PaySlip =====
public class PaySlipDto
{
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; }
    public string EmployeeNumber { get; set; }
    public string? DepartmentName { get; set; }
    public string? JobTitle { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public List<PaySlipLineDto> Earnings { get; set; } = new();
    public List<PaySlipLineDto> Deductions { get; set; } = new();
    public decimal TotalEarnings { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal NetSalary { get; set; }
}

public class PaySlipLineDto
{
    public string ItemName { get; set; }
    public decimal Amount { get; set; }
}

// ===== Employee Lookup =====
public class EmployeeLookupDto : EntityDto<Guid>
{
    public string EmployeeNumber { get; set; }
    public string NameAr { get; set; }
}
