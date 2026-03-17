import type { AuditedEntityDto, EntityDto } from '@abp/ng.core';
import type { CompensationNature } from './enums/compensation-nature.enum';
import type { CompensationValueType } from './enums/compensation-value-type.enum';
import type { CompensationMethod } from './enums/compensation-method.enum';
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
  employeeName?: string;
  departmentId?: string;
  departmentName?: string;
  permitType?: string;
  date?: string;
  hours?: number;
  minutes?: number;
  reason?: string;
  notes?: string;
}

export interface CompensationItemDto extends AuditedEntityDto<string> {
  nameAr?: string;
  displayName?: string;
  nature?: CompensationNature;
  valueType?: CompensationValueType;
  method?: CompensationMethod;
  formulaExpression?: string;
  accountId?: string;
  accountName?: string;
  isActive?: boolean;
}

export interface CreateUpdateAttendanceRecordDto {
  employeeId?: string;
  departmentId?: string;
  permitType?: string;
  date?: string;
  hours?: number;
  minutes?: number;
  reason?: string;
  notes?: string;
}

export interface CreateUpdateCompensationItemDto {
  nameAr?: string;
  displayName?: string;
  nature?: CompensationNature;
  valueType?: CompensationValueType;
  method?: CompensationMethod;
  formulaExpression?: string;
  accountId?: string;
  isActive?: boolean;
}

export interface CreateUpdateEmployeeDto {
  employeeNumber?: string;
  nameAr?: string;
  nameEn?: string;
  gender?: Gender;
  birthDate?: string;
  maritalStatus?: MaritalStatus;
  address?: string;
  phone?: string;
  qualification?: string;
  identityType?: IdentityDocumentType;
  identityNumber?: string;
  insuranceNumber?: string;
  bankName?: string;
  bankAccountNumber?: string;
  departmentId?: string;
  sectionName?: string;
  jobGradeId?: string;
  jobTitleId?: string;
  jobTitle?: string;
  employmentClassification?: string;
  salaryPaymentMethod?: SalaryPaymentMethod;
  contractType?: ContractType;
  hireDate?: string;
  terminationDate?: string;
  reminderEnabled?: boolean;
  isSuspended?: boolean;
  isActive?: boolean;
  basicSalary?: number;
}

export interface CreateUpdateEmployeeLeaveDto {
  employeeId?: string;
  departmentId?: string;
  leaveTypeId?: string;
  startDate?: string;
  endDate?: string;
  duration?: number;
  notes?: string;
}

export interface CreateUpdateEmployeeLoanDto {
  employeeId?: string;
  departmentId?: string;
  compensationItemId?: string;
  amount?: number;
  installments?: number;
  startDate?: string;
  notes?: string;
}

export interface CreateUpdateJobGradeDto {
  code?: string;
  nameAr?: string;
  nameEn?: string;
  baseSalary?: number;
  isActive?: boolean;
}

export interface CreateUpdateLeaveTypeDto {
  nameAr?: string;
  duration?: number;
  employeeClass?: string;
  affectsSalary?: boolean;
  isBalance?: boolean;
  isPublicHoliday?: boolean;
  startDate?: string;
  endDate?: string;
  isActive?: boolean;
}

export interface CreateUpdatePenaltyDto {
  employeeId?: string;
  penaltyType?: PenaltyType;
  description?: string;
  amount?: number;
  suspensionDays?: number;
  date?: string;
  notes?: string;
}

export interface CreateUpdateSalarySetupDto {
  employeeId?: string;
  compensationItemId?: string;
  amount?: number;
  isRecurring?: boolean;
  startDate?: string;
  isActive?: boolean;
}

export interface EmployeeDto extends AuditedEntityDto<string> {
  employeeNumber?: string;
  nameAr?: string;
  nameEn?: string;
  gender?: Gender;
  birthDate?: string;
  maritalStatus?: MaritalStatus;
  address?: string;
  phone?: string;
  qualification?: string;
  identityType?: IdentityDocumentType;
  identityNumber?: string;
  insuranceNumber?: string;
  bankName?: string;
  bankAccountNumber?: string;
  departmentId?: string;
  departmentName?: string;
  sectionName?: string;
  jobGradeId?: string;
  jobGradeName?: string;
  jobTitleId?: string;
  jobTitleName?: string;
  jobTitle?: string;
  employmentClassification?: string;
  salaryPaymentMethod?: SalaryPaymentMethod;
  contractType?: ContractType;
  hireDate?: string;
  terminationDate?: string;
  isSuspended?: boolean;
  isActive?: boolean;
  basicSalary?: number;
}

export interface EmployeeLeaveDto extends AuditedEntityDto<string> {
  employeeId?: string;
  employeeName?: string;
  departmentId?: string;
  departmentName?: string;
  leaveTypeId?: string;
  leaveTypeName?: string;
  startDate?: string;
  endDate?: string;
  duration?: number;
  entitled?: number;
  used?: number;
  balance?: number;
  notes?: string;
}

export interface EmployeeLoanDto extends AuditedEntityDto<string> {
  employeeId?: string;
  employeeName?: string;
  departmentId?: string;
  compensationItemId?: string;
  compensationItemName?: string;
  amount?: number;
  installments?: number;
  startDate?: string;
  status?: LoanStatus;
  paidAmount?: number;
  remainingAmount?: number;
  notes?: string;
}

export interface EmployeeLookupDto extends EntityDto<string> {
  employeeNumber?: string;
  nameAr?: string;
}

export interface JobGradeDto extends AuditedEntityDto<string> {
  code?: string;
  nameAr?: string;
  nameEn?: string;
  baseSalary?: number;
  isActive?: boolean;
}

export interface LeaveTypeDto extends AuditedEntityDto<string> {
  nameAr?: string;
  duration?: number;
  employeeClass?: string;
  affectsSalary?: boolean;
  isBalance?: boolean;
  isPublicHoliday?: boolean;
  startDate?: string;
  endDate?: string;
  isActive?: boolean;
}

export interface PaySlipDto {
  employeeId?: string;
  employeeName?: string;
  employeeNumber?: string;
  departmentName?: string;
  jobTitle?: string;
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
  departmentId?: string;
  departmentName?: string;
  jobGradeId?: string;
  status?: PayrollRunStatus;
  journalEntryId?: string;
  totalEarnings?: number;
  totalDeductions?: number;
  netSalary?: number;
}

export interface PenaltyDto extends AuditedEntityDto<string> {
  employeeId?: string;
  employeeName?: string;
  penaltyType?: PenaltyType;
  description?: string;
  amount?: number;
  suspensionDays?: number;
  date?: string;
  notes?: string;
}

export interface ProcessPayrollDto {
  periodStart?: string;
  periodEnd?: string;
  departmentId?: string;
  jobGradeId?: string;
}

export interface SalarySetupDto extends AuditedEntityDto<string> {
  employeeId?: string;
  employeeName?: string;
  compensationItemId?: string;
  compensationItemName?: string;
  amount?: number;
  isRecurring?: boolean;
  startDate?: string;
  isActive?: boolean;
}

export interface DailyAttendanceDto extends AuditedEntityDto<string> {
  employeeId?: string;
  employeeName?: string;
  employeeNumber?: string;
  departmentName?: string;
  date?: string;
  checkInTime?: string;
  checkOutTime?: string;
  status?: number;
  workedHours?: number;
  notes?: string;
}

export interface CreateUpdateDailyAttendanceDto {
  employeeId?: string;
  date?: string;
  checkInTime?: string;
  checkOutTime?: string;
  status?: number;
  notes?: string;
}
