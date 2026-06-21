using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HIS.HR.Enums;
using HIS.Permissions;
using Microsoft.AspNetCore.Authorization;
using HIS.HR.Printing;
using Microsoft.AspNetCore.Hosting;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Content;
using MiniExcelLibs;
using System.IO;

namespace HIS.HR;

[Authorize(HISPermissions.HR.Default)]
public class HRAppService : ApplicationService
{
    private readonly IRepository<Employee, Guid> _employeeRepository;
    private readonly IRepository<JobGrade, Guid> _jobGradeRepository;
    private readonly IRepository<HIS.Settings.JobTitle, Guid> _jobTitleRepository;
    private readonly IRepository<CompensationItem, Guid> _compensationItemRepository;
    private readonly IRepository<LeaveType, Guid> _leaveTypeRepository;
    private readonly IRepository<EmployeeLeave, Guid> _employeeLeaveRepository;
    private readonly IRepository<EmployeeLoan, Guid> _employeeLoanRepository;
    private readonly IRepository<SalarySetup, Guid> _salarySetupRepository;
    private readonly IRepository<PayrollRun, Guid> _payrollRunRepository;
    private readonly IRepository<PayrollLine, Guid> _payrollLineRepository;
    private readonly IRepository<Penalty, Guid> _penaltyRepository;
    private readonly IRepository<AttendanceRecord, Guid> _attendanceRecordRepository;
    private readonly IRepository<DailyAttendance, Guid> _dailyAttendanceRepository;
    private readonly IRepository<HIS.Settings.Department, Guid> _departmentRepository;
    private readonly IRepository<HIS.Accounting.Account, Guid> _accountRepository;
    private readonly IRepository<HIS.Accounting.JournalEntry, Guid> _journalEntryRepository;
    private readonly IGuidGenerator _guidGenerator;
    private readonly IWebHostEnvironment _env;

    public HRAppService(
        IRepository<Employee, Guid> employeeRepository,
        IRepository<JobGrade, Guid> jobGradeRepository,
        IRepository<HIS.Settings.JobTitle, Guid> jobTitleRepository,
        IRepository<CompensationItem, Guid> compensationItemRepository,
        IRepository<LeaveType, Guid> leaveTypeRepository,
        IRepository<EmployeeLeave, Guid> employeeLeaveRepository,
        IRepository<EmployeeLoan, Guid> employeeLoanRepository,
        IRepository<SalarySetup, Guid> salarySetupRepository,
        IRepository<PayrollRun, Guid> payrollRunRepository,
        IRepository<PayrollLine, Guid> payrollLineRepository,
        IRepository<Penalty, Guid> penaltyRepository,
        IRepository<AttendanceRecord, Guid> attendanceRecordRepository,
        IRepository<DailyAttendance, Guid> dailyAttendanceRepository,
        IRepository<HIS.Settings.Department, Guid> departmentRepository,
        IRepository<HIS.Accounting.Account, Guid> accountRepository,
        IRepository<HIS.Accounting.JournalEntry, Guid> journalEntryRepository,
        IGuidGenerator guidGenerator,
        IWebHostEnvironment env)
    {
        _employeeRepository = employeeRepository;
        _jobGradeRepository = jobGradeRepository;
        _jobTitleRepository = jobTitleRepository;
        _compensationItemRepository = compensationItemRepository;
        _leaveTypeRepository = leaveTypeRepository;
        _employeeLeaveRepository = employeeLeaveRepository;
        _employeeLoanRepository = employeeLoanRepository;
        _salarySetupRepository = salarySetupRepository;
        _payrollRunRepository = payrollRunRepository;
        _payrollLineRepository = payrollLineRepository;
        _penaltyRepository = penaltyRepository;
        _attendanceRecordRepository = attendanceRecordRepository;
        _dailyAttendanceRepository = dailyAttendanceRepository;
        _departmentRepository = departmentRepository;
        _accountRepository = accountRepository;
        _journalEntryRepository = journalEntryRepository;
        _guidGenerator = guidGenerator;
        _env = env;
    }

    // ===== EMPLOYEES =====

    [Authorize(HISPermissions.HR.Employees)]
    public async Task<PagedResultDto<EmployeeDto>> GetEmployeesAsync(PagedAndSortedResultRequestDto input)
    {
        var query = await _employeeRepository.GetQueryableAsync();
        var totalCount = await AsyncExecuter.CountAsync(query);

        var employees = await AsyncExecuter.ToListAsync(query
            .OrderByDescending(x => x.CreationTime)
            .Skip(input.SkipCount).Take(input.MaxResultCount));

        // Optimized lookups for only the departments, grades, and titles in the current page
        var deptIds = employees.Where(e => e.DepartmentId.HasValue).Select(e => e.DepartmentId.Value).Distinct().ToList();
        var gradeIds = employees.Where(e => e.JobGradeId.HasValue).Select(e => e.JobGradeId.Value).Distinct().ToList();
        var titleIds = employees.Where(e => e.JobTitleId.HasValue).Select(e => e.JobTitleId.Value).Distinct().ToList();

        var deptLookup = (await _departmentRepository.GetListAsync(d => deptIds.Contains(d.Id))).ToDictionary(d => d.Id, d => d.NameAr);
        var gradeLookup = (await _jobGradeRepository.GetListAsync(g => gradeIds.Contains(g.Id))).ToDictionary(g => g.Id, g => g.NameAr);
        var titleLookup = (await _jobTitleRepository.GetListAsync(t => titleIds.Contains(t.Id))).ToDictionary(t => t.Id, t => t.NameAr);

        var items = employees
            .Select(e => {
                var dto = ObjectMapper.Map<Employee, EmployeeDto>(e);
                dto.DepartmentName = e.DepartmentId.HasValue && deptLookup.ContainsKey(e.DepartmentId.Value) ? deptLookup[e.DepartmentId.Value] : null;
                dto.JobGradeName = e.JobGradeId.HasValue && gradeLookup.ContainsKey(e.JobGradeId.Value) ? gradeLookup[e.JobGradeId.Value] : null;
                dto.JobTitleName = e.JobTitleId.HasValue && titleLookup.ContainsKey(e.JobTitleId.Value) ? titleLookup[e.JobTitleId.Value] : null;
                return dto;
            }).ToList();

        return new PagedResultDto<EmployeeDto>(totalCount, items);
    }

    [Authorize(HISPermissions.HR.Employees)]
    public async Task<EmployeeDto> GetEmployeeAsync(Guid id)
    {
        var entity = await _employeeRepository.GetAsync(id);
        var dto = ObjectMapper.Map<Employee, EmployeeDto>(entity);
        if (entity.DepartmentId.HasValue)
        {
            var dept = await _departmentRepository.FindAsync(entity.DepartmentId.Value);
            dto.DepartmentName = dept?.NameAr;
        }
        if (entity.JobGradeId.HasValue)
        {
            var grade = await _jobGradeRepository.FindAsync(entity.JobGradeId.Value);
            dto.JobGradeName = grade?.NameAr;
        }
        if (entity.JobTitleId.HasValue)
        {
            var title = await _jobTitleRepository.FindAsync(entity.JobTitleId.Value);
            dto.JobTitleName = title?.NameAr;
        }
        return dto;
    }

    [Authorize(HISPermissions.HR.EmployeesCreate)]
    public async Task<EmployeeDto> CreateEmployeeAsync(CreateUpdateEmployeeDto input)
    {
        // Auto-generate employee number
        var employeeNumber = await GenerateNextEmployeeNumberAsync();
        
        var entity = new Employee(_guidGenerator.Create(), CurrentTenant.Id, employeeNumber, input.NameAr);
        ObjectMapper.Map(input, entity);
        
        // Re-assign the generated number if it was overwritten by null/empty from input
        if (string.IsNullOrWhiteSpace(entity.EmployeeNumber))
        {
            entity.EmployeeNumber = employeeNumber;
        }
        
        await _employeeRepository.InsertAsync(entity);

        // Automatically create Salary Setup for Basic Salary if provided
        if (input.BasicSalary.HasValue && input.BasicSalary.Value > 0)
        {
            var basicSalaryItem = await _compensationItemRepository.FirstOrDefaultAsync(x => x.NameAr == "الراتب الأساسي");
            if (basicSalaryItem != null)
            {
                var salarySetup = new SalarySetup(_guidGenerator.Create(), CurrentTenant.Id, entity.Id, basicSalaryItem.Id, input.BasicSalary.Value);
                await _salarySetupRepository.InsertAsync(salarySetup);
            }
        }

        return ObjectMapper.Map<Employee, EmployeeDto>(entity);
    }

    [Authorize(HISPermissions.HR.EmployeesEdit)]
    public async Task<EmployeeDto> UpdateEmployeeAsync(Guid id, CreateUpdateEmployeeDto input)
    {
        var entity = await _employeeRepository.GetAsync(id);
        
        // If basic salary changed, update or create Salary Setup
        if (input.BasicSalary != entity.BasicSalary)
        {
            var basicSalaryItem = await _compensationItemRepository.FirstOrDefaultAsync(x => x.NameAr == "الراتب الأساسي");
            if (basicSalaryItem != null)
            {
                var setup = await _salarySetupRepository.FirstOrDefaultAsync(x => x.EmployeeId == id && x.CompensationItemId == basicSalaryItem.Id);
                if (setup != null)
                {
                    setup.Amount = input.BasicSalary ?? 0;
                    await _salarySetupRepository.UpdateAsync(setup);
                }
                else if (input.BasicSalary.HasValue)
                {
                    var salarySetup = new SalarySetup(_guidGenerator.Create(), CurrentTenant.Id, id, basicSalaryItem.Id, input.BasicSalary.Value);
                    await _salarySetupRepository.InsertAsync(salarySetup);
                }
            }
        }

        ObjectMapper.Map(input, entity);
        await _employeeRepository.UpdateAsync(entity);
        return ObjectMapper.Map<Employee, EmployeeDto>(entity);
    }

    private async Task<string> GenerateNextEmployeeNumberAsync()
    {
        var query = await _employeeRepository.GetQueryableAsync();
        var lastEmployee = await AsyncExecuter.FirstOrDefaultAsync(query
            .Where(x => x.EmployeeNumber.StartsWith("EMP"))
            .OrderByDescending(x => x.EmployeeNumber));

        if (lastEmployee == null)
        {
            return "EMP001";
        }

        var lastNumberStr = lastEmployee.EmployeeNumber.Replace("EMP-", "").Replace("EMP", "");
        if (int.TryParse(lastNumberStr, out int lastNumber))
        {
            return $"EMP{(lastNumber + 1):D3}";
        }

        return "EMP001";
    }

    [Authorize(HISPermissions.HR.EmployeesDelete)]
    public async Task DeleteEmployeeAsync(Guid id)
    {
        await _employeeRepository.DeleteAsync(id);
    }

    public async Task<List<EmployeeLookupDto>> GetEmployeeLookupAsync()
    {
        var list = await _employeeRepository.GetListAsync();
        return list.Select(e => new EmployeeLookupDto { Id = e.Id, EmployeeNumber = e.EmployeeNumber, NameAr = e.NameAr }).ToList();
    }

    // ===== JOB GRADES =====

    public async Task<List<JobGradeDto>> GetJobGradesAsync()
    {
        var list = await _jobGradeRepository.GetListAsync();
        return ObjectMapper.Map<List<JobGrade>, List<JobGradeDto>>(list);
    }

    public async Task<JobGradeDto> CreateJobGradeAsync(CreateUpdateJobGradeDto input)
    {
        var entity = new JobGrade(_guidGenerator.Create(), CurrentTenant.Id, input.Code, input.NameAr);
        ObjectMapper.Map(input, entity);
        await _jobGradeRepository.InsertAsync(entity);
        return ObjectMapper.Map<JobGrade, JobGradeDto>(entity);
    }

    public async Task<JobGradeDto> UpdateJobGradeAsync(Guid id, CreateUpdateJobGradeDto input)
    {
        var entity = await _jobGradeRepository.GetAsync(id);
        ObjectMapper.Map(input, entity);
        await _jobGradeRepository.UpdateAsync(entity);
        return ObjectMapper.Map<JobGrade, JobGradeDto>(entity);
    }

    public async Task DeleteJobGradeAsync(Guid id) => await _jobGradeRepository.DeleteAsync(id);

    // ===== COMPENSATION ITEMS =====

    [Authorize(HISPermissions.HR.CompensationItems)]
    public async Task<List<CompensationItemDto>> GetCompensationItemsAsync()
    {
        var list = await _compensationItemRepository.GetListAsync();
        return ObjectMapper.Map<List<CompensationItem>, List<CompensationItemDto>>(list);
    }

    [Authorize(HISPermissions.HR.CompensationItems)]
    public async Task<CompensationItemDto> CreateCompensationItemAsync(CreateUpdateCompensationItemDto input)
    {
        var entity = new CompensationItem(_guidGenerator.Create(), CurrentTenant.Id, input.NameAr, input.Nature);
        ObjectMapper.Map(input, entity);
        await _compensationItemRepository.InsertAsync(entity);
        return ObjectMapper.Map<CompensationItem, CompensationItemDto>(entity);
    }

    [Authorize(HISPermissions.HR.CompensationItems)]
    public async Task<CompensationItemDto> UpdateCompensationItemAsync(Guid id, CreateUpdateCompensationItemDto input)
    {
        var entity = await _compensationItemRepository.GetAsync(id);
        ObjectMapper.Map(input, entity);
        await _compensationItemRepository.UpdateAsync(entity);
        return ObjectMapper.Map<CompensationItem, CompensationItemDto>(entity);
    }

    [Authorize(HISPermissions.HR.CompensationItems)]
    public async Task DeleteCompensationItemAsync(Guid id) => await _compensationItemRepository.DeleteAsync(id);

    // ===== LEAVE TYPES =====

    [Authorize(HISPermissions.HR.LeaveTypes)]
    public async Task<List<LeaveTypeDto>> GetLeaveTypesAsync()
    {
        var list = await _leaveTypeRepository.GetListAsync();
        return ObjectMapper.Map<List<LeaveType>, List<LeaveTypeDto>>(list);
    }

    [Authorize(HISPermissions.HR.LeaveTypes)]
    public async Task<LeaveTypeDto> CreateLeaveTypeAsync(CreateUpdateLeaveTypeDto input)
    {
        var entity = new LeaveType(_guidGenerator.Create(), CurrentTenant.Id, input.NameAr);
        ObjectMapper.Map(input, entity);
        await _leaveTypeRepository.InsertAsync(entity);
        return ObjectMapper.Map<LeaveType, LeaveTypeDto>(entity);
    }

    [Authorize(HISPermissions.HR.LeaveTypes)]
    public async Task<LeaveTypeDto> UpdateLeaveTypeAsync(Guid id, CreateUpdateLeaveTypeDto input)
    {
        var entity = await _leaveTypeRepository.GetAsync(id);
        ObjectMapper.Map(input, entity);
        await _leaveTypeRepository.UpdateAsync(entity);
        return ObjectMapper.Map<LeaveType, LeaveTypeDto>(entity);
    }

    [Authorize(HISPermissions.HR.LeaveTypes)]
    public async Task DeleteLeaveTypeAsync(Guid id) => await _leaveTypeRepository.DeleteAsync(id);

    // ===== EMPLOYEE LEAVES =====

    [Authorize(HISPermissions.HR.EmployeeLeaves)]
    public async Task<PagedResultDto<EmployeeLeaveDto>> GetEmployeeLeavesAsync(PagedAndSortedResultRequestDto input)
    {
        var leaves = await _employeeLeaveRepository.GetListAsync();
        var employees = await _employeeRepository.GetListAsync();
        var leaveTypes = await _leaveTypeRepository.GetListAsync();

        var empLookup = employees.ToDictionary(e => e.Id, e => e.NameAr);
        var ltLookup = leaveTypes.ToDictionary(lt => lt.Id, lt => lt.NameAr);

        var items = leaves
            .OrderByDescending(x => x.CreationTime)
            .Skip(input.SkipCount).Take(input.MaxResultCount)
            .Select(el =>
            {
                var dto = ObjectMapper.Map<EmployeeLeave, EmployeeLeaveDto>(el);
                dto.EmployeeName = empLookup.GetValueOrDefault(el.EmployeeId);
                dto.LeaveTypeName = ltLookup.GetValueOrDefault(el.LeaveTypeId);
                return dto;
            }).ToList();

        return new PagedResultDto<EmployeeLeaveDto>(leaves.Count, items);
    }

    [Authorize(HISPermissions.HR.EmployeeLeaves)]
    public async Task<EmployeeLeaveDto> CreateEmployeeLeaveAsync(CreateUpdateEmployeeLeaveDto input)
    {
        var entity = new EmployeeLeave(_guidGenerator.Create(), CurrentTenant.Id, input.EmployeeId, input.LeaveTypeId);
        ObjectMapper.Map(input, entity);

        var leaveType = await _leaveTypeRepository.GetAsync(input.LeaveTypeId);
        entity.Entitled = leaveType.Duration;
        var existingLeaves = await _employeeLeaveRepository.GetListAsync(x => x.EmployeeId == input.EmployeeId && x.LeaveTypeId == input.LeaveTypeId);
        entity.Used = existingLeaves.Sum(x => x.Duration) + input.Duration;
        entity.Balance = entity.Entitled - entity.Used;

        await _employeeLeaveRepository.InsertAsync(entity);
        return ObjectMapper.Map<EmployeeLeave, EmployeeLeaveDto>(entity);
    }

    [Authorize(HISPermissions.HR.EmployeeLeaves)]
    public async Task DeleteEmployeeLeaveAsync(Guid id) => await _employeeLeaveRepository.DeleteAsync(id);

    // ===== EMPLOYEE LOANS =====

    [Authorize(HISPermissions.HR.Loans)]
    public async Task<PagedResultDto<EmployeeLoanDto>> GetEmployeeLoansAsync(PagedAndSortedResultRequestDto input)
    {
        var loans = await _employeeLoanRepository.GetListAsync();
        var employees = await _employeeRepository.GetListAsync();
        var empLookup = employees.ToDictionary(e => e.Id, e => e.NameAr);

        var items = loans
            .OrderByDescending(x => x.CreationTime)
            .Skip(input.SkipCount).Take(input.MaxResultCount)
            .Select(l =>
            {
                var dto = ObjectMapper.Map<EmployeeLoan, EmployeeLoanDto>(l);
                dto.EmployeeName = empLookup.GetValueOrDefault(l.EmployeeId);
                return dto;
            }).ToList();

        return new PagedResultDto<EmployeeLoanDto>(loans.Count, items);
    }

    [Authorize(HISPermissions.HR.Loans)]
    public async Task<EmployeeLoanDto> CreateEmployeeLoanAsync(CreateUpdateEmployeeLoanDto input)
    {
        var entity = new EmployeeLoan(_guidGenerator.Create(), CurrentTenant.Id, input.EmployeeId, input.Amount);
        ObjectMapper.Map(input, entity);
        await _employeeLoanRepository.InsertAsync(entity);
        return ObjectMapper.Map<EmployeeLoan, EmployeeLoanDto>(entity);
    }

    [Authorize(HISPermissions.HR.Loans)]
    public async Task DeleteEmployeeLoanAsync(Guid id) => await _employeeLoanRepository.DeleteAsync(id);

    // ===== SALARY SETUP =====

    [Authorize(HISPermissions.HR.Payroll)]
    public async Task<PagedResultDto<SalarySetupDto>> GetSalarySetupsAsync(PagedAndSortedResultRequestDto input)
    {
        var setups = await _salarySetupRepository.GetListAsync();
        var employees = await _employeeRepository.GetListAsync();
        var compItems = await _compensationItemRepository.GetListAsync();

        var empLookup = employees.ToDictionary(e => e.Id, e => e.NameAr);
        var ciLookup = compItems.ToDictionary(ci => ci.Id, ci => ci.NameAr);

        var items = setups
            .OrderByDescending(x => x.CreationTime)
            .Skip(input.SkipCount).Take(input.MaxResultCount)
            .Select(s =>
            {
                var dto = ObjectMapper.Map<SalarySetup, SalarySetupDto>(s);
                dto.EmployeeName = empLookup.GetValueOrDefault(s.EmployeeId);
                dto.CompensationItemName = ciLookup.GetValueOrDefault(s.CompensationItemId);
                return dto;
            }).ToList();

        return new PagedResultDto<SalarySetupDto>(setups.Count, items);
    }

    [Authorize(HISPermissions.HR.Payroll)]
    public async Task<SalarySetupDto> CreateSalarySetupAsync(CreateUpdateSalarySetupDto input)
    {
        var entity = new SalarySetup(_guidGenerator.Create(), CurrentTenant.Id, input.EmployeeId, input.CompensationItemId, input.Amount);
        ObjectMapper.Map(input, entity);
        await _salarySetupRepository.InsertAsync(entity);
        return ObjectMapper.Map<SalarySetup, SalarySetupDto>(entity);
    }

    [Authorize(HISPermissions.HR.Payroll)]
    public async Task<SalarySetupDto> UpdateSalarySetupAsync(Guid id, CreateUpdateSalarySetupDto input)
    {
        var entity = await _salarySetupRepository.GetAsync(id);
        ObjectMapper.Map(input, entity);
        await _salarySetupRepository.UpdateAsync(entity);
        return ObjectMapper.Map<SalarySetup, SalarySetupDto>(entity);
    }

    [Authorize(HISPermissions.HR.Payroll)]
    public async Task DeleteSalarySetupAsync(Guid id) => await _salarySetupRepository.DeleteAsync(id);

    // ===== PAYROLL PROCESSING =====

    [Authorize(HISPermissions.HR.PayrollProcess)]
    public async Task<PayrollRunDto> ProcessPayrollAsync(ProcessPayrollDto input)
    {
        var payrollRun = new PayrollRun(_guidGenerator.Create(), CurrentTenant.Id, input.PeriodStart, input.PeriodEnd);
        payrollRun.DepartmentId = input.DepartmentId;
        payrollRun.JobGradeId = input.JobGradeId;

        // Get active employees matching criteria
        var allEmployees = await _employeeRepository.GetListAsync(e => e.IsActive && !e.IsSuspended);
        var employees = allEmployees.AsEnumerable();
        if (input.DepartmentId.HasValue) employees = employees.Where(e => e.DepartmentId == input.DepartmentId);
        if (input.JobGradeId.HasValue) employees = employees.Where(e => e.JobGradeId == input.JobGradeId);
        var empList = employees.ToList();

        var salarySetups = (await _salarySetupRepository.GetListAsync(s => s.IsActive)).ToList();
        var compensationItems = await _compensationItemRepository.GetListAsync();
        var attendanceRecords = await _dailyAttendanceRepository.GetListAsync(a => a.Date >= input.PeriodStart && a.Date <= input.PeriodEnd);

        var overtimeItem = compensationItems.FirstOrDefault(ci => ci.NameAr == "بدل إضافي" || ci.NameAr == "Overtime Allowance");
        var absenceItem = compensationItems.FirstOrDefault(ci => ci.NameAr == "خصم غياب" || ci.NameAr == "Absence Deduction");

        decimal totalEarnings = 0, totalDeductions = 0;

        foreach (var emp in empList)
        {
            decimal empEarnings = 0, empDeductions = 0;

            // 1. Regular Salary Setups (Basic, Housing, etc.)
            var empSetups = salarySetups.Where(s => s.EmployeeId == emp.Id).ToList();
            foreach (var setup in empSetups)
            {
                var item = compensationItems.FirstOrDefault(ci => ci.Id == setup.CompensationItemId);
                if (item == null) continue;

                var line = new PayrollLine(_guidGenerator.Create(), payrollRun.Id, emp.Id, setup.CompensationItemId, setup.Amount, item.Nature);
                payrollRun.Lines.Add(line);

                if (item.Nature == CompensationNature.Allowance) empEarnings += setup.Amount;
                else empDeductions += setup.Amount;
            }

            // 2. Automated Overtime Calculation
            var empAttendance = attendanceRecords.Where(a => a.EmployeeId == emp.Id).ToList();
            var totalOvertimeHours = empAttendance.Sum(a => a.OvertimeHours);
            
            if (totalOvertimeHours > 0 && overtimeItem != null)
            {
                // Hourly rate = Basic / 240 (approx 30 days * 8 hours) * 1.5 multiplier
                decimal hourlyRate = (emp.BasicSalary ?? 0) / 240;
                decimal overtimeAmount = Math.Round(totalOvertimeHours * hourlyRate * 1.5m, 2);

                if (overtimeAmount > 0)
                {
                    var line = new PayrollLine(_guidGenerator.Create(), payrollRun.Id, emp.Id, overtimeItem.Id, overtimeAmount, CompensationNature.Allowance);
                    payrollRun.Lines.Add(line);
                    empEarnings += overtimeAmount;
                }
            }

            // 3. Automated Absence Calculation
            var absenceDays = empAttendance.Count(a => a.Status == AttendanceStatus.Absent);
            if (absenceDays > 0 && absenceItem != null)
            {
                // Daily rate = Basic / 30
                decimal dailyRate = (emp.BasicSalary ?? 0) / 30;
                decimal absenceAmount = Math.Round(absenceDays * dailyRate, 2);

                if (absenceAmount > 0)
                {
                    var line = new PayrollLine(_guidGenerator.Create(), payrollRun.Id, emp.Id, absenceItem.Id, absenceAmount, CompensationNature.Deduction);
                    payrollRun.Lines.Add(line);
                    empDeductions += absenceAmount;
                }
            }

            totalEarnings += empEarnings;
            totalDeductions += empDeductions;
        }

        payrollRun.TotalEarnings = totalEarnings;
        payrollRun.TotalDeductions = totalDeductions;
        payrollRun.NetSalary = totalEarnings - totalDeductions;
        payrollRun.Status = PayrollRunStatus.Processed;

        // Create Journal Entry (Account 5100 Debit = Salary Expense, Account 2200 Credit = Employee Payables)
        var salaryExpenseAccount = await _accountRepository.FirstOrDefaultAsync(a => a.Code == "5100");
        var employeePayableAccount = await _accountRepository.FirstOrDefaultAsync(a => a.Code == "2200");

        if (salaryExpenseAccount != null && employeePayableAccount != null)
        {
            var je = new HIS.Accounting.JournalEntry(
                _guidGenerator.Create(),
                DateTime.Now,
                $"PAY-{input.PeriodStart:yyyyMMdd}",
                $"مرتبات الفترة من {input.PeriodStart:yyyy/MM/dd} إلى {input.PeriodEnd:yyyy/MM/dd}");
            
            salaryExpenseAccount = await GetLeafAccountAsync(salaryExpenseAccount);
            employeePayableAccount = await GetLeafAccountAsync(employeePayableAccount);
            je.AddLine(_guidGenerator, salaryExpenseAccount.Id, totalEarnings, 0);
            je.AddLine(_guidGenerator, employeePayableAccount.Id, 0, payrollRun.NetSalary);
            je.IsPosted = true;
            await _journalEntryRepository.InsertAsync(je);
            payrollRun.JournalEntryId = je.Id;
        }

        await _payrollRunRepository.InsertAsync(payrollRun);
        return ObjectMapper.Map<PayrollRun, PayrollRunDto>(payrollRun);
    }

    [Authorize(HISPermissions.HR.Payroll)]
    public async Task<PagedResultDto<PayrollRunDto>> GetPayrollRunsAsync(PagedAndSortedResultRequestDto input)
    {
        var query = await _payrollRunRepository.GetQueryableAsync();
        var totalCount = await AsyncExecuter.CountAsync(query);

        var runs = await AsyncExecuter.ToListAsync(query
            .OrderByDescending(x => x.CreationTime)
            .Skip(input.SkipCount).Take(input.MaxResultCount));

        var items = ObjectMapper.Map<List<PayrollRun>, List<PayrollRunDto>>(runs);
        return new PagedResultDto<PayrollRunDto>(totalCount, items);
    }

    [Authorize(HISPermissions.HR.Payroll)]
    public async Task<List<EmployeeLookupDto>> GetPayrollRunEmployeesAsync(Guid payrollRunId)
    {
        var lines = await _payrollLineRepository.GetListAsync(l => l.PayrollRunId == payrollRunId);
        var employeeIds = lines.Select(l => l.EmployeeId).Distinct().ToList();
        var employees = await _employeeRepository.GetListAsync(e => employeeIds.Contains(e.Id));
        return employees.Select(e => new EmployeeLookupDto { Id = e.Id, EmployeeNumber = e.EmployeeNumber, NameAr = e.NameAr }).ToList();
    }

    // ===== PAY SLIP =====

    [Authorize(HISPermissions.HR.PaySlip)]
    public async Task<PaySlipDto> GetPaySlipAsync(Guid payrollRunId, Guid employeeId)
    {
        var payrollRun = await _payrollRunRepository.GetAsync(payrollRunId);
        var employee = await _employeeRepository.GetAsync(employeeId);
        var allLines = await _payrollLineRepository.GetListAsync(l => l.PayrollRunId == payrollRunId);
        var lines = allLines.Where(l => l.EmployeeId == employeeId).ToList();
        var compensationItems = await _compensationItemRepository.GetListAsync();

        var dept = employee.DepartmentId.HasValue
            ? await _departmentRepository.FindAsync(employee.DepartmentId.Value) : null;

        var paySlip = new PaySlipDto
        {
            EmployeeId = employee.Id,
            EmployeeName = employee.NameAr,
            EmployeeNumber = employee.EmployeeNumber,
            DepartmentName = dept?.NameAr,
            JobTitle = employee.JobTitle,
            PeriodStart = payrollRun.PeriodStart,
            PeriodEnd = payrollRun.PeriodEnd,
        };

        foreach (var line in lines)
        {
            var item = compensationItems.FirstOrDefault(ci => ci.Id == line.CompensationItemId);
            var slipLine = new PaySlipLineDto
            {
                ItemName = item?.NameAr ?? "بند غير محدد",
                Amount = line.Amount
            };

            if (line.Nature == CompensationNature.Allowance) paySlip.Earnings.Add(slipLine);
            else paySlip.Deductions.Add(slipLine);
        }

        paySlip.TotalEarnings = paySlip.Earnings.Sum(e => e.Amount);
        paySlip.TotalDeductions = paySlip.Deductions.Sum(d => d.Amount);
        paySlip.NetSalary = paySlip.TotalEarnings - paySlip.TotalDeductions;

        return paySlip;
    }

    [Authorize(HISPermissions.HR.PaySlip)]
    [Microsoft.AspNetCore.Mvc.HttpGet]
    [Microsoft.AspNetCore.Mvc.Route("api/app/h-r/pay-slip-pdf/{payrollRunId}/{employeeId}")]
    public async Task<Volo.Abp.Content.IRemoteStreamContent> GetPaySlipPdfAsync(Guid payrollRunId, Guid employeeId)
    {
        var paySlip = await GetPaySlipAsync(payrollRunId, employeeId);
        
        byte[] logoBytes = null;
        var logoPath = System.IO.Path.Combine(_env.WebRootPath ?? "", "images", "logo", "Dark.png");
        
        if (!System.IO.File.Exists(logoPath))
        {
            var devPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot", "images", "logo", "Dark.png");
            if (System.IO.File.Exists(devPath)) logoPath = devPath;
        }

        if (System.IO.File.Exists(logoPath)) logoBytes = await System.IO.File.ReadAllBytesAsync(logoPath);

        var document = new PaySlipDocument
        {
            Data = paySlip,
            LogoBytes = logoBytes
        };

        var pdfBytes = QuestPDF.Fluent.GenerateExtensions.GeneratePdf(document);
        var stream = new System.IO.MemoryStream(pdfBytes);
        var fileName = $"قسيمة_راتب_{paySlip.EmployeeNumber}_{DateTime.Now:yyyyMMdd}.pdf";
        return new Volo.Abp.Content.RemoteStreamContent(stream, fileName, "application/pdf");
    }

    // ===== PENALTIES =====

    [Authorize(HISPermissions.HR.Penalties)]
    public async Task<PagedResultDto<PenaltyDto>> GetPenaltiesAsync(PagedAndSortedResultRequestDto input)
    {
        var query = await _penaltyRepository.GetQueryableAsync();
        var totalCount = await AsyncExecuter.CountAsync(query);

        var penalties = await AsyncExecuter.ToListAsync(query
            .OrderByDescending(x => x.Date)
            .Skip(input.SkipCount).Take(input.MaxResultCount));

        var employeeIds = penalties.Select(x => x.EmployeeId).Distinct().ToList();
        var employees = await _employeeRepository.GetListAsync(x => employeeIds.Contains(x.Id));
        var empLookup = employees.ToDictionary(e => e.Id, e => e.NameAr);

        var items = penalties
            .Select(p =>
            {
                var dto = ObjectMapper.Map<Penalty, PenaltyDto>(p);
                dto.EmployeeName = empLookup.GetValueOrDefault(p.EmployeeId);
                return dto;
            }).ToList();

        return new PagedResultDto<PenaltyDto>(totalCount, items);
    }

    [Authorize(HISPermissions.HR.Penalties)]
    public async Task<PenaltyDto> CreatePenaltyAsync(CreateUpdatePenaltyDto input)
    {
        var entity = new Penalty(_guidGenerator.Create(), CurrentTenant.Id, input.EmployeeId, input.PenaltyType, input.Date);
        ObjectMapper.Map(input, entity);
        await _penaltyRepository.InsertAsync(entity);
        return ObjectMapper.Map<Penalty, PenaltyDto>(entity);
    }

    [Authorize(HISPermissions.HR.Penalties)]
    public async Task DeletePenaltyAsync(Guid id) => await _penaltyRepository.DeleteAsync(id);

    // ===== ATTENDANCE =====

    [Authorize(HISPermissions.HR.Attendance)]
    public async Task<PagedResultDto<AttendanceRecordDto>> GetAttendanceRecordsAsync(PagedAndSortedResultRequestDto input)
    {
        var query = await _attendanceRecordRepository.GetQueryableAsync();
        var totalCount = await AsyncExecuter.CountAsync(query);

        var records = await AsyncExecuter.ToListAsync(query
            .OrderByDescending(x => x.Date)
            .Skip(input.SkipCount).Take(input.MaxResultCount));

        var employeeIds = records.Select(x => x.EmployeeId).Distinct().ToList();
        var employees = await _employeeRepository.GetListAsync(x => employeeIds.Contains(x.Id));
        var empLookup = employees.ToDictionary(e => e.Id, e => e.NameAr);

        var departmentIds = records.Where(x => x.DepartmentId.HasValue).Select(x => x.DepartmentId.Value).Distinct().ToList();
        var departments = await _departmentRepository.GetListAsync(x => departmentIds.Contains(x.Id));
        var deptLookup = departments.ToDictionary(d => d.Id, d => d.NameAr);

        var items = records
            .Select(a =>
            {
                var dto = ObjectMapper.Map<AttendanceRecord, AttendanceRecordDto>(a);
                dto.EmployeeName = empLookup.GetValueOrDefault(a.EmployeeId);
                dto.DepartmentName = a.DepartmentId.HasValue && deptLookup.ContainsKey(a.DepartmentId.Value) ? deptLookup[a.DepartmentId.Value] : null;
                return dto;
            }).ToList();

        return new PagedResultDto<AttendanceRecordDto>(totalCount, items);
    }

    [Authorize(HISPermissions.HR.Attendance)]
    public async Task<AttendanceRecordDto> CreateAttendanceRecordAsync(CreateUpdateAttendanceRecordDto input)
    {
        var entity = new AttendanceRecord(_guidGenerator.Create(), CurrentTenant.Id, input.EmployeeId, input.Date);
        ObjectMapper.Map(input, entity);
        await _attendanceRecordRepository.InsertAsync(entity);
        return ObjectMapper.Map<AttendanceRecord, AttendanceRecordDto>(entity);
    }

    [Authorize(HISPermissions.HR.Attendance)]
    public async Task DeleteAttendanceRecordAsync(Guid id) => await _attendanceRecordRepository.DeleteAsync(id);

    // ===== DAILY ATTENDANCE (حضور وانصراف) =====

    [Authorize(HISPermissions.HR.Attendance)]
    public async Task<PagedResultDto<DailyAttendanceDto>> GetDailyAttendanceAsync(PagedAndSortedResultRequestDto input)
    {
        var records = await _dailyAttendanceRepository.GetListAsync();
        var employees = await _employeeRepository.GetListAsync();
        var departments = await _departmentRepository.GetListAsync();

        var empLookup = employees.ToDictionary(e => e.Id);
        var deptLookup = departments.ToDictionary(d => d.Id, d => d.NameAr);

        var items = records
            .OrderByDescending(x => x.Date).ThenByDescending(x => x.CheckInTime)
            .Skip(input.SkipCount).Take(input.MaxResultCount)
            .Select(a =>
            {
                var dto = ObjectMapper.Map<DailyAttendance, DailyAttendanceDto>(a);
                if (empLookup.TryGetValue(a.EmployeeId, out var emp))
                {
                    dto.EmployeeName = emp.NameAr;
                    dto.EmployeeNumber = emp.EmployeeNumber;
                    dto.DepartmentName = emp.DepartmentId.HasValue && deptLookup.ContainsKey(emp.DepartmentId.Value)
                        ? deptLookup[emp.DepartmentId.Value] : null;
                }
                return dto;
            }).ToList();

        return new PagedResultDto<DailyAttendanceDto>(records.Count, items);
    }

    [Authorize(HISPermissions.HR.Attendance)]
    public async Task<DailyAttendanceDto> CreateDailyAttendanceAsync(CreateUpdateDailyAttendanceDto input)
    {
        var entity = new DailyAttendance(_guidGenerator.Create(), CurrentTenant.Id, input.EmployeeId, input.Date);
        entity.CheckInTime = input.CheckInTime;
        entity.CheckOutTime = input.CheckOutTime;
        entity.Status = input.Status;
        entity.Notes = input.Notes;
        entity.CalculateWorkedHours();
        await _dailyAttendanceRepository.InsertAsync(entity);
        return ObjectMapper.Map<DailyAttendance, DailyAttendanceDto>(entity);
    }

    [Authorize(HISPermissions.HR.Attendance)]
    public async Task<DailyAttendanceDto> UpdateDailyAttendanceAsync(Guid id, CreateUpdateDailyAttendanceDto input)
    {
        var entity = await _dailyAttendanceRepository.GetAsync(id);
        entity.CheckInTime = input.CheckInTime;
        entity.CheckOutTime = input.CheckOutTime;
        entity.Status = input.Status;
        entity.Notes = input.Notes;
        entity.CalculateWorkedHours();
        await _dailyAttendanceRepository.UpdateAsync(entity);
        return ObjectMapper.Map<DailyAttendance, DailyAttendanceDto>(entity);
    }

    [Authorize(HISPermissions.HR.Attendance)]
    public async Task ImportAttendanceAsync(IRemoteStreamContent file)
    {
        using (var stream = file.GetStream())
        {
            var rows = stream.Query().ToList();
            var employees = await _employeeRepository.GetListAsync();
            var empDict = employees.ToDictionary(e => e.EmployeeNumber, e => e.Id);

            foreach (var row in rows)
            {
                // Expected columns: EmployeeNumber, Date, CheckInTime, CheckOutTime
                IDictionary<string, object> rowDict = row;
                string empNum = rowDict.ContainsKey("EmployeeNumber") ? rowDict["EmployeeNumber"]?.ToString() : null;
                if (string.IsNullOrEmpty(empNum) || !empDict.ContainsKey(empNum)) continue;

                if (!DateTime.TryParse(rowDict.ContainsKey("Date") ? rowDict["Date"]?.ToString() : null, out DateTime date)) continue;

                var attendance = await _dailyAttendanceRepository.FirstOrDefaultAsync(x => x.EmployeeId == empDict[empNum] && x.Date.Date == date.Date);
                
                if (attendance == null)
                {
                    attendance = new DailyAttendance(_guidGenerator.Create(), CurrentTenant.Id, empDict[empNum], date.Date);
                    await _dailyAttendanceRepository.InsertAsync(attendance);
                }

                if (DateTime.TryParse(rowDict.ContainsKey("CheckInTime") ? rowDict["CheckInTime"]?.ToString() : null, out DateTime checkIn))
                {
                    attendance.CheckInTime = new DateTime(date.Year, date.Month, date.Day, checkIn.Hour, checkIn.Minute, 0);
                }

                if (DateTime.TryParse(rowDict.ContainsKey("CheckOutTime") ? rowDict["CheckOutTime"]?.ToString() : null, out DateTime checkOut))
                {
                    attendance.CheckOutTime = new DateTime(date.Year, date.Month, date.Day, checkOut.Hour, checkOut.Minute, 0);
                }

                attendance.Status = AttendanceStatus.Present;
                attendance.CalculateWorkedHours();
            }
        }
    }

    [Authorize(HISPermissions.HR.Attendance)]
    public async Task DeleteDailyAttendanceAsync(Guid id) => await _dailyAttendanceRepository.DeleteAsync(id);

    private async Task<HIS.Accounting.Account> GetLeafAccountAsync(HIS.Accounting.Account account)
    {
        if (account == null) return null;

<<<<<<< HEAD
        var hasChildren = await _accountRepository.AnyAsync(x => x.ParentId == account.Id);
=======
        var hasChildren = await _accountRepository.AnyAsync(x => x.ParentId == account.Id && x.IsActive);
>>>>>>> e13898f2b35a3a8419d073a4378cb81bc374fc49
        if (!hasChildren)
        {
            return account;
        }

<<<<<<< HEAD
        var children = await _accountRepository.GetListAsync(x => x.ParentId == account.Id);
=======
        var children = await _accountRepository.GetListAsync(x => x.ParentId == account.Id && x.IsActive);
>>>>>>> e13898f2b35a3a8419d073a4378cb81bc374fc49
        if (!children.Any())
        {
            return account;
        }

        foreach (var child in children.OrderBy(x => x.Code))
        {
            var leaf = await GetLeafAccountAsync(child);
            if (leaf != null)
            {
                return leaf;
            }
        }

        return account;
    }
}
