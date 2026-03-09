using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HIS.HR.Enums;
using HIS.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;

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
        IGuidGenerator guidGenerator)
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
    }

    // ===== EMPLOYEES =====

    [Authorize(HISPermissions.HR.Employees)]
    public async Task<PagedResultDto<EmployeeDto>> GetEmployeesAsync(PagedAndSortedResultRequestDto input)
    {
        var employees = await _employeeRepository.GetListAsync();
        var departments = await _departmentRepository.GetListAsync();
        var jobGrades = await _jobGradeRepository.GetListAsync();
        var jobTitles = await _jobTitleRepository.GetListAsync();

        var deptLookup = departments.ToDictionary(d => d.Id, d => d.NameAr);
        var gradeLookup = jobGrades.ToDictionary(g => g.Id, g => g.NameAr);
        var titleLookup = jobTitles.ToDictionary(t => t.Id, t => t.NameAr);

        var items = employees
            .OrderByDescending(x => x.CreationTime)
            .Skip(input.SkipCount).Take(input.MaxResultCount)
            .Select(e => {
                var dto = ObjectMapper.Map<Employee, EmployeeDto>(e);
                dto.DepartmentName = e.DepartmentId.HasValue && deptLookup.ContainsKey(e.DepartmentId.Value) ? deptLookup[e.DepartmentId.Value] : null;
                dto.JobGradeName = e.JobGradeId.HasValue && gradeLookup.ContainsKey(e.JobGradeId.Value) ? gradeLookup[e.JobGradeId.Value] : null;
                dto.JobTitleName = e.JobTitleId.HasValue && titleLookup.ContainsKey(e.JobTitleId.Value) ? titleLookup[e.JobTitleId.Value] : null;
                return dto;
            }).ToList();

        return new PagedResultDto<EmployeeDto>(employees.Count, items);
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
        var entity = new Employee(_guidGenerator.Create(), CurrentTenant.Id, input.EmployeeNumber, input.NameAr);
        ObjectMapper.Map(input, entity);
        await _employeeRepository.InsertAsync(entity);
        return ObjectMapper.Map<Employee, EmployeeDto>(entity);
    }

    [Authorize(HISPermissions.HR.EmployeesEdit)]
    public async Task<EmployeeDto> UpdateEmployeeAsync(Guid id, CreateUpdateEmployeeDto input)
    {
        var entity = await _employeeRepository.GetAsync(id);
        ObjectMapper.Map(input, entity);
        await _employeeRepository.UpdateAsync(entity);
        return ObjectMapper.Map<Employee, EmployeeDto>(entity);
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

        decimal totalEarnings = 0, totalDeductions = 0;

        foreach (var emp in empList)
        {
            var empSetups = salarySetups.Where(s => s.EmployeeId == emp.Id).ToList();
            foreach (var setup in empSetups)
            {
                var item = compensationItems.FirstOrDefault(ci => ci.Id == setup.CompensationItemId);
                if (item == null) continue;

                var line = new PayrollLine(
                    _guidGenerator.Create(), payrollRun.Id, emp.Id,
                    setup.CompensationItemId, setup.Amount, item.Nature);
                payrollRun.Lines.Add(line);

                if (item.Nature == CompensationNature.Allowance) totalEarnings += setup.Amount;
                else totalDeductions += setup.Amount;
            }
        }

        payrollRun.TotalEarnings = totalEarnings;
        payrollRun.TotalDeductions = totalDeductions;
        payrollRun.NetSalary = totalEarnings - totalDeductions;
        payrollRun.Status = PayrollRunStatus.Processed;

        // Create Journal Entry (Account 4100 Debit = Salary Expense, Account 2200 Credit = Employee Payables)
        var accounts = await _accountRepository.GetListAsync();
        var salaryExpenseAccount = accounts.FirstOrDefault(a => a.Code == "4100");
        var employeePayableAccount = accounts.FirstOrDefault(a => a.Code == "2200");

        if (salaryExpenseAccount != null && employeePayableAccount != null)
        {
            var je = new HIS.Accounting.JournalEntry(
                _guidGenerator.Create(),
                DateTime.Now,
                $"PAY-{input.PeriodStart:yyyyMMdd}",
                $"مرتبات الفترة من {input.PeriodStart:yyyy/MM/dd} إلى {input.PeriodEnd:yyyy/MM/dd}");
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
        var runs = await _payrollRunRepository.GetListAsync();
        var items = runs.OrderByDescending(x => x.CreationTime)
            .Skip(input.SkipCount).Take(input.MaxResultCount).ToList();
        return new PagedResultDto<PayrollRunDto>(runs.Count, ObjectMapper.Map<List<PayrollRun>, List<PayrollRunDto>>(items));
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

    // ===== PENALTIES =====

    [Authorize(HISPermissions.HR.Penalties)]
    public async Task<PagedResultDto<PenaltyDto>> GetPenaltiesAsync(PagedAndSortedResultRequestDto input)
    {
        var penalties = await _penaltyRepository.GetListAsync();
        var employees = await _employeeRepository.GetListAsync();
        var empLookup = employees.ToDictionary(e => e.Id, e => e.NameAr);

        var items = penalties
            .OrderByDescending(x => x.Date)
            .Skip(input.SkipCount).Take(input.MaxResultCount)
            .Select(p =>
            {
                var dto = ObjectMapper.Map<Penalty, PenaltyDto>(p);
                dto.EmployeeName = empLookup.GetValueOrDefault(p.EmployeeId);
                return dto;
            }).ToList();

        return new PagedResultDto<PenaltyDto>(penalties.Count, items);
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
        var records = await _attendanceRecordRepository.GetListAsync();
        var employees = await _employeeRepository.GetListAsync();
        var departments = await _departmentRepository.GetListAsync();

        var empLookup = employees.ToDictionary(e => e.Id, e => e.NameAr);
        var deptLookup = departments.ToDictionary(d => d.Id, d => d.NameAr);

        var items = records
            .OrderByDescending(x => x.Date)
            .Skip(input.SkipCount).Take(input.MaxResultCount)
            .Select(a =>
            {
                var dto = ObjectMapper.Map<AttendanceRecord, AttendanceRecordDto>(a);
                dto.EmployeeName = empLookup.GetValueOrDefault(a.EmployeeId);
                dto.DepartmentName = a.DepartmentId.HasValue && deptLookup.ContainsKey(a.DepartmentId.Value) ? deptLookup[a.DepartmentId.Value] : null;
                return dto;
            }).ToList();

        return new PagedResultDto<AttendanceRecordDto>(records.Count, items);
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
    public async Task DeleteDailyAttendanceAsync(Guid id) => await _dailyAttendanceRepository.DeleteAsync(id);
}
