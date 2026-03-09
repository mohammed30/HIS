import type { AttendanceRecordDto, CompensationItemDto, CreateUpdateAttendanceRecordDto, CreateUpdateCompensationItemDto, CreateUpdateDailyAttendanceDto, CreateUpdateEmployeeDto, CreateUpdateEmployeeLeaveDto, CreateUpdateEmployeeLoanDto, CreateUpdateJobGradeDto, CreateUpdateLeaveTypeDto, CreateUpdatePenaltyDto, CreateUpdateSalarySetupDto, DailyAttendanceDto, EmployeeDto, EmployeeLeaveDto, EmployeeLoanDto, EmployeeLookupDto, JobGradeDto, LeaveTypeDto, PaySlipDto, PayrollRunDto, PenaltyDto, ProcessPayrollDto, SalarySetupDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class HRService {
  private restService = inject(RestService);
  apiName = 'Default';


  createAttendanceRecord = (input: CreateUpdateAttendanceRecordDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, AttendanceRecordDto>({
      method: 'POST',
      url: '/api/app/h-r/attendance-record',
      body: input,
    },
      { apiName: this.apiName, ...config });


  createCompensationItem = (input: CreateUpdateCompensationItemDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CompensationItemDto>({
      method: 'POST',
      url: '/api/app/h-r/compensation-item',
      body: input,
    },
      { apiName: this.apiName, ...config });


  createEmployee = (input: CreateUpdateEmployeeDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, EmployeeDto>({
      method: 'POST',
      url: '/api/app/h-r/employee',
      body: input,
    },
      { apiName: this.apiName, ...config });


  createEmployeeLeave = (input: CreateUpdateEmployeeLeaveDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, EmployeeLeaveDto>({
      method: 'POST',
      url: '/api/app/h-r/employee-leave',
      body: input,
    },
      { apiName: this.apiName, ...config });


  createEmployeeLoan = (input: CreateUpdateEmployeeLoanDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, EmployeeLoanDto>({
      method: 'POST',
      url: '/api/app/h-r/employee-loan',
      body: input,
    },
      { apiName: this.apiName, ...config });


  createJobGrade = (input: CreateUpdateJobGradeDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, JobGradeDto>({
      method: 'POST',
      url: '/api/app/h-r/job-grade',
      body: input,
    },
      { apiName: this.apiName, ...config });


  createLeaveType = (input: CreateUpdateLeaveTypeDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, LeaveTypeDto>({
      method: 'POST',
      url: '/api/app/h-r/leave-type',
      body: input,
    },
      { apiName: this.apiName, ...config });


  createPenalty = (input: CreateUpdatePenaltyDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PenaltyDto>({
      method: 'POST',
      url: '/api/app/h-r/penalty',
      body: input,
    },
      { apiName: this.apiName, ...config });


  createSalarySetup = (input: CreateUpdateSalarySetupDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, SalarySetupDto>({
      method: 'POST',
      url: '/api/app/h-r/salary-setup',
      body: input,
    },
      { apiName: this.apiName, ...config });


  deleteAttendanceRecord = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/h-r/${id}/attendance-record`,
    },
      { apiName: this.apiName, ...config });


  deleteCompensationItem = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/h-r/${id}/compensation-item`,
    },
      { apiName: this.apiName, ...config });


  deleteEmployee = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/h-r/${id}/employee`,
    },
      { apiName: this.apiName, ...config });


  deleteEmployeeLeave = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/h-r/${id}/employee-leave`,
    },
      { apiName: this.apiName, ...config });


  deleteEmployeeLoan = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/h-r/${id}/employee-loan`,
    },
      { apiName: this.apiName, ...config });


  deleteJobGrade = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/h-r/${id}/job-grade`,
    },
      { apiName: this.apiName, ...config });


  deleteLeaveType = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/h-r/${id}/leave-type`,
    },
      { apiName: this.apiName, ...config });


  deletePenalty = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/h-r/${id}/penalty`,
    },
      { apiName: this.apiName, ...config });


  deleteSalarySetup = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/h-r/${id}/salary-setup`,
    },
      { apiName: this.apiName, ...config });


  getAttendanceRecords = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<AttendanceRecordDto>>({
      method: 'GET',
      url: '/api/app/h-r/attendance-records',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
      { apiName: this.apiName, ...config });


  getCompensationItems = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, CompensationItemDto[]>({
      method: 'GET',
      url: '/api/app/h-r/compensation-items',
    },
      { apiName: this.apiName, ...config });


  getEmployee = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, EmployeeDto>({
      method: 'GET',
      url: `/api/app/h-r/${id}/employee`,
    },
      { apiName: this.apiName, ...config });


  getEmployeeLeaves = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<EmployeeLeaveDto>>({
      method: 'GET',
      url: '/api/app/h-r/employee-leaves',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
      { apiName: this.apiName, ...config });


  getEmployeeLoans = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<EmployeeLoanDto>>({
      method: 'GET',
      url: '/api/app/h-r/employee-loans',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
      { apiName: this.apiName, ...config });


  getEmployeeLookup = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, EmployeeLookupDto[]>({
      method: 'GET',
      url: '/api/app/h-r/employee-lookup',
    },
      { apiName: this.apiName, ...config });


  getEmployees = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<EmployeeDto>>({
      method: 'GET',
      url: '/api/app/h-r/employees',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
      { apiName: this.apiName, ...config });


  getJobGrades = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, JobGradeDto[]>({
      method: 'GET',
      url: '/api/app/h-r/job-grades',
    },
      { apiName: this.apiName, ...config });


  getLeaveTypes = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, LeaveTypeDto[]>({
      method: 'GET',
      url: '/api/app/h-r/leave-types',
    },
      { apiName: this.apiName, ...config });


  getPaySlip = (payrollRunId: string, employeeId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PaySlipDto>({
      method: 'GET',
      url: '/api/app/h-r/pay-slip',
      params: { payrollRunId, employeeId },
    },
      { apiName: this.apiName, ...config });


  getPayrollRuns = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<PayrollRunDto>>({
      method: 'GET',
      url: '/api/app/h-r/payroll-runs',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
      { apiName: this.apiName, ...config });


  getPenalties = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<PenaltyDto>>({
      method: 'GET',
      url: '/api/app/h-r/penalties',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
      { apiName: this.apiName, ...config });


  getSalarySetups = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<SalarySetupDto>>({
      method: 'GET',
      url: '/api/app/h-r/salary-setups',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
      { apiName: this.apiName, ...config });


  processPayroll = (input: ProcessPayrollDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PayrollRunDto>({
      method: 'POST',
      url: '/api/app/h-r/process-payroll',
      body: input,
    },
      { apiName: this.apiName, ...config });


  updateCompensationItem = (id: string, input: CreateUpdateCompensationItemDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CompensationItemDto>({
      method: 'PUT',
      url: `/api/app/h-r/${id}/compensation-item`,
      body: input,
    },
      { apiName: this.apiName, ...config });


  updateEmployee = (id: string, input: CreateUpdateEmployeeDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, EmployeeDto>({
      method: 'PUT',
      url: `/api/app/h-r/${id}/employee`,
      body: input,
    },
      { apiName: this.apiName, ...config });


  updateJobGrade = (id: string, input: CreateUpdateJobGradeDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, JobGradeDto>({
      method: 'PUT',
      url: `/api/app/h-r/${id}/job-grade`,
      body: input,
    },
      { apiName: this.apiName, ...config });


  updateLeaveType = (id: string, input: CreateUpdateLeaveTypeDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, LeaveTypeDto>({
      method: 'PUT',
      url: `/api/app/h-r/${id}/leave-type`,
      body: input,
    },
      { apiName: this.apiName, ...config });


  updateSalarySetup = (id: string, input: CreateUpdateSalarySetupDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, SalarySetupDto>({
      method: 'PUT',
      url: `/api/app/h-r/${id}/salary-setup`,
      body: input,
    },
      { apiName: this.apiName, ...config });

  // ===== Daily Attendance =====

  getDailyAttendance = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<DailyAttendanceDto>>({
      method: 'GET',
      url: '/api/app/h-r/daily-attendance',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
      { apiName: this.apiName, ...config });

  createDailyAttendance = (input: CreateUpdateDailyAttendanceDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DailyAttendanceDto>({
      method: 'POST',
      url: '/api/app/h-r/daily-attendance',
      body: input,
    },
      { apiName: this.apiName, ...config });

  updateDailyAttendance = (id: string, input: CreateUpdateDailyAttendanceDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DailyAttendanceDto>({
      method: 'PUT',
      url: `/api/app/h-r/${id}/daily-attendance`,
      body: input,
    },
      { apiName: this.apiName, ...config });

  deleteDailyAttendance = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/h-r/${id}/daily-attendance`,
    },
      { apiName: this.apiName, ...config });
}