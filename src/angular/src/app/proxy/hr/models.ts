import type { AuditedEntityDto, EntityDto } from '@abp/ng.core';
import type { CompensationNature } from './enums/compensation-nature.enum';
import type { CompensationValueType } from './enums/compensation-value-type.enum';
import type { CompensationMethod } from './enums/compensation-method.enum';
import type { AttendanceStatus } from './enums/attendance-status.enum';
import type { Gender } from './enums/gender.enum';
import type { MaritalStatus } from './enums/marital-status.enum';
import type { IdentityDocumentType } from './enums/identity-document-type.enum';
import type { SalaryPaymentMethod } from './enums/salary-payment-method.enum';
import type { ContractType } from './enums/contract-type.enum';
import type { PenaltyType } from './enums/penalty-type.enum';
import type { LoanStatus } from './enums/loan-status.enum';
import type { PayrollRunStatus } from './enums/payroll-run-status.enum';

export interface AttendanceRecordDto extends AuditedEntityDto<string> {
  employeeId?: string;
  employeeName?: string | null;
  departmentId?: string | null;
  departmentName?: string | null;
  permitType?: string | null;
  date?: string;
  hours?: number | null;
  minutes?: number | null;
  reason?: string | null;
  notes?: string | null;
}

export interface CompensationItemDto extends AuditedEntityDto<string> {
  nameAr?: string;
  displayName?: string | null;
  nature?: CompensationNature;
  valueType?: CompensationValueType;
  method?: CompensationMethod;
  formulaExpression?: string | null;
  accountId?: string | null;
  accountName?: string | null;
  isActive?: boolean;
}

export interface CreateUpdateAttendanceRecordDto {
  employeeId?: string;
  departmentId?: string | null;
  permitType?: string | null;
  date?: string;
  hours?: number | null;
  minutes?: number | null;
  reason?: string | null;
  notes?: string | null;
}

export interface CreateUpdateCompensationItemDto {
  nameAr?: string;
  displayName?: string | null;
  nature?: CompensationNature;
  valueType?: CompensationValueType;
  method?: CompensationMethod;
  formulaExpression?: string | null;
  accountId?: string | null;
  isActive?: boolean;
}

export interface CreateUpdateDailyAttendanceDto {
  employeeId?: string;
  date?: string;
  checkInTime?: string | null;
  checkOutTime?: string | null;
  status?: AttendanceStatus;
  overtimeHours?: number | null;
  notes?: string | null;
}

export interface CreateUpdateEmployeeDto {
  employeeNumber?: string | null;
  nameAr?: string;
  nameEn?: string | null;
  gender?: Gender;
  birthDate?: string | null;
  maritalStatus?: MaritalStatus | null;
  address?: string | null;
  phone?: string | null;
  email?: string | null;
  qualification?: string | null;
  identityType?: IdentityDocumentType | null;
  identityNumber?: string | null;
  insuranceNumber?: string | null;
  bankName?: string | null;
  bankAccountNumber?: string | null;
  iban?: string | null;
  departmentId?: string | null;
  sectionName?: string | null;
  jobGradeId?: string | null;
  jobTitleId?: string | null;
  jobTitle?: string | null;
  employmentClassification?: string | null;
  salaryPaymentMethod?: SalaryPaymentMethod | null;
  contractType?: ContractType | null;
  hireDate?: string | null;
  terminationDate?: string | null;
  reminderEnabled?: boolean;
  isSuspended?: boolean;
  isActive?: boolean;
  basicSalary?: number | null;
}

export interface CreateUpdateEmployeeLeaveDto {
  employeeId?: string;
  departmentId?: string | null;
  leaveTypeId?: string;
  startDate?: string;
  endDate?: string;
  duration?: number;
  notes?: string | null;
}

export interface CreateUpdateEmployeeLoanDto {
  employeeId?: string;
  departmentId?: string | null;
  compensationItemId?: string | null;
  amount?: number;
  installments?: number;
  startDate?: string;
  notes?: string | null;
}

export interface CreateUpdateJobGradeDto {
  code?: string;
  nameAr?: string;
  nameEn?: string | null;
  baseSalary?: number;
  isActive?: boolean;
}

export interface CreateUpdateLeaveTypeDto {
  nameAr?: string;
  duration?: number;
  employeeClass?: string | null;
  affectsSalary?: boolean;
  isBalance?: boolean;
  isPublicHoliday?: boolean;
  startDate?: string | null;
  endDate?: string | null;
  isActive?: boolean;
}

export interface CreateUpdatePenaltyDto {
  employeeId?: string;
  penaltyType?: PenaltyType;
  description?: string | null;
  amount?: number | null;
  suspensionDays?: number | null;
  date?: string;
  notes?: string | null;
}

export interface CreateUpdateSalarySetupDto {
  employeeId?: string;
  compensationItemId?: string;
  amount?: number;
  isRecurring?: boolean;
  startDate?: string | null;
  isActive?: boolean;
}

export interface DailyAttendanceDto extends AuditedEntityDto<string> {
  employeeId?: string;
  employeeName?: string | null;
  employeeNumber?: string | null;
  departmentName?: string | null;
  date?: string;
  checkInTime?: string | null;
  checkOutTime?: string | null;
  status?: AttendanceStatus;
  workedHours?: number;
  overtimeHours?: number;
  notes?: string | null;
}

export interface EmployeeDto extends AuditedEntityDto<string> {
  employeeNumber?: string;
  nameAr?: string;
  nameEn?: string | null;
  gender?: Gender;
  birthDate?: string | null;
  maritalStatus?: MaritalStatus | null;
  address?: string | null;
  phone?: string | null;
  email?: string | null;
  qualification?: string | null;
  identityType?: IdentityDocumentType | null;
  identityNumber?: string | null;
  insuranceNumber?: string | null;
  bankName?: string | null;
  bankAccountNumber?: string | null;
  iban?: string | null;
  departmentId?: string | null;
  departmentName?: string | null;
  sectionName?: string | null;
  jobGradeId?: string | null;
  jobGradeName?: string | null;
  jobTitleId?: string | null;
  jobTitleName?: string | null;
  jobTitle?: string | null;
  employmentClassification?: string | null;
  salaryPaymentMethod?: SalaryPaymentMethod | null;
  contractType?: ContractType | null;
  hireDate?: string | null;
  terminationDate?: string | null;
  isSuspended?: boolean;
  isActive?: boolean;
  basicSalary?: number | null;
}

export interface EmployeeLeaveDto extends AuditedEntityDto<string> {
  employeeId?: string;
  employeeName?: string | null;
  departmentId?: string | null;
  departmentName?: string | null;
  leaveTypeId?: string;
  leaveTypeName?: string | null;
  startDate?: string;
  endDate?: string;
  duration?: number;
  entitled?: number;
  used?: number;
  balance?: number;
  notes?: string | null;
}

export interface EmployeeLoanDto extends AuditedEntityDto<string> {
  employeeId?: string;
  employeeName?: string | null;
  departmentId?: string | null;
  compensationItemId?: string | null;
  compensationItemName?: string | null;
  amount?: number;
  installments?: number;
  startDate?: string;
  status?: LoanStatus;
  paidAmount?: number;
  remainingAmount?: number;
  notes?: string | null;
}

export interface EmployeeLookupDto extends EntityDto<string> {
  employeeNumber?: string;
  nameAr?: string;
}

export interface JobGradeDto extends AuditedEntityDto<string> {
  code?: string;
  nameAr?: string;
  nameEn?: string | null;
  baseSalary?: number;
  isActive?: boolean;
}

export interface LeaveTypeDto extends AuditedEntityDto<string> {
  nameAr?: string;
  duration?: number;
  employeeClass?: string | null;
  affectsSalary?: boolean;
  isBalance?: boolean;
  isPublicHoliday?: boolean;
  startDate?: string | null;
  endDate?: string | null;
  isActive?: boolean;
}

export interface PaySlipDto {
  employeeId?: string;
  employeeName?: string;
  employeeNumber?: string;
  departmentName?: string | null;
  jobTitle?: string | null;
  periodStart?: string;
  periodEnd?: string;
  earnings?: PaySlipLineDto[];
  deductions?: PaySlipLineDto[];
  totalEarnings?: number;
  totalDeductions?: number;
  netSalary?: number;
}

export interface PaySlipLineDto {
  itemName?: string;
  amount?: number;
}

export interface PayrollRunDto extends AuditedEntityDto<string> {
  periodStart?: string;
  periodEnd?: string;
  departmentId?: string | null;
  departmentName?: string | null;
  jobGradeId?: string | null;
  status?: PayrollRunStatus;
  journalEntryId?: string | null;
  totalEarnings?: number;
  totalDeductions?: number;
  netSalary?: number;
}

export interface PenaltyDto extends AuditedEntityDto<string> {
  employeeId?: string;
  employeeName?: string | null;
  penaltyType?: PenaltyType;
  description?: string | null;
  amount?: number | null;
  suspensionDays?: number | null;
  date?: string;
  notes?: string | null;
}

export interface ProcessPayrollDto {
  periodStart?: string;
  periodEnd?: string;
  departmentId?: string | null;
  jobGradeId?: string | null;
}

export interface SalarySetupDto extends AuditedEntityDto<string> {
  employeeId?: string;
  employeeName?: string | null;
  compensationItemId?: string;
  compensationItemName?: string | null;
  amount?: number;
  isRecurring?: boolean;
  startDate?: string | null;
  isActive?: boolean;
}
