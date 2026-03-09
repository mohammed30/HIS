using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HIS.HR.Enums;
using HIS.Settings;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;

namespace HIS.HR;

public class HRDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<Employee, Guid> _employeeRepository;
    private readonly IRepository<Department, Guid> _departmentRepository;
    private readonly IRepository<JobGrade, Guid> _jobGradeRepository;
    private readonly IRepository<JobTitle, Guid> _jobTitleRepository;
    private readonly IRepository<CompensationItem, Guid> _compensationItemRepository;
    private readonly IRepository<LeaveType, Guid> _leaveTypeRepository;
    private readonly IRepository<EmployeeLeave, Guid> _employeeLeaveRepository;
    private readonly IRepository<EmployeeLoan, Guid> _employeeLoanRepository;
    private readonly IRepository<Penalty, Guid> _penaltyRepository;
    private readonly IRepository<AttendanceRecord, Guid> _attendanceRecordRepository;
    private readonly IRepository<DailyAttendance, Guid> _dailyAttendanceRepository;
    private readonly IRepository<SalarySetup, Guid> _salarySetupRepository;
    private readonly IRepository<PayrollRun, Guid> _payrollRunRepository;
    private readonly IRepository<PayrollLine, Guid> _payrollLineRepository;
    private readonly IGuidGenerator _guidGenerator;
    private readonly ICurrentTenant _currentTenant;

    public HRDataSeedContributor(
        IRepository<Employee, Guid> employeeRepository,
        IRepository<Department, Guid> departmentRepository,
        IRepository<JobGrade, Guid> jobGradeRepository,
        IRepository<JobTitle, Guid> jobTitleRepository,
        IRepository<CompensationItem, Guid> compensationItemRepository,
        IRepository<LeaveType, Guid> leaveTypeRepository,
        IRepository<EmployeeLeave, Guid> employeeLeaveRepository,
        IRepository<EmployeeLoan, Guid> employeeLoanRepository,
        IRepository<Penalty, Guid> penaltyRepository,
        IRepository<AttendanceRecord, Guid> attendanceRecordRepository,
        IRepository<DailyAttendance, Guid> dailyAttendanceRepository,
        IRepository<SalarySetup, Guid> salarySetupRepository,
        IRepository<PayrollRun, Guid> payrollRunRepository,
        IRepository<PayrollLine, Guid> payrollLineRepository,
        IGuidGenerator guidGenerator,
        ICurrentTenant currentTenant)
    {
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;
        _jobGradeRepository = jobGradeRepository;
        _jobTitleRepository = jobTitleRepository;
        _compensationItemRepository = compensationItemRepository;
        _leaveTypeRepository = leaveTypeRepository;
        _employeeLeaveRepository = employeeLeaveRepository;
        _employeeLoanRepository = employeeLoanRepository;
        _penaltyRepository = penaltyRepository;
        _attendanceRecordRepository = attendanceRecordRepository;
        _dailyAttendanceRepository = dailyAttendanceRepository;
        _salarySetupRepository = salarySetupRepository;
        _payrollRunRepository = payrollRunRepository;
        _payrollLineRepository = payrollLineRepository;
        _guidGenerator = guidGenerator;
        _currentTenant = currentTenant;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        await SeedCompensationItemsAsync();
        await SeedJobGradesAsync();
        await SeedJobTitlesAsync();
        await SeedEmployeesAsync();
        await SeedLeaveTypesAsync();
        await SeedAdditionalTestDataAsync();
    }

    private async Task SeedLeaveTypesAsync()
    {
        await CreateLeaveTypeAsync("إجازة سنوية", 30, true);
        await CreateLeaveTypeAsync("إجازة مرضية", 15, false);
        await CreateLeaveTypeAsync("إجازة اضطرارية", 5, true);
        await CreateLeaveTypeAsync("إجازة بدون راتب", 90, false, true);
    }

    private async Task SeedAdditionalTestDataAsync()
    {
        var emp1 = await _employeeRepository.FirstOrDefaultAsync(x => x.EmployeeNumber == "EMP001");
        var emp2 = await _employeeRepository.FirstOrDefaultAsync(x => x.EmployeeNumber == "EMP002");
        var annualLeave = await _leaveTypeRepository.FirstOrDefaultAsync(x => x.NameAr == "إجازة سنوية");
        var basicSalary = await _compensationItemRepository.FirstOrDefaultAsync(x => x.NameAr == "الراتب الأساسي");
        var housing = await _compensationItemRepository.FirstOrDefaultAsync(x => x.NameAr == "بدل سكن");
        var socialInsurance = await _compensationItemRepository.FirstOrDefaultAsync(x => x.NameAr == "تأمينات اجتماعية");

        if (emp1 != null)
        {
            // Salary Setup
            if (basicSalary != null) await CreateSalarySetupAsync(emp1.Id, basicSalary.Id, 15000);
            if (housing != null) await CreateSalarySetupAsync(emp1.Id, housing.Id, 3000);
            if (socialInsurance != null) await CreateSalarySetupAsync(emp1.Id, socialInsurance.Id, 1500);

            // Attendance
            await CreateDailyAttendanceAsync(emp1.Id, DateTime.Now.AddDays(-1), 8, 0);
            await CreateDailyAttendanceAsync(emp1.Id, DateTime.Now.AddDays(-2), 8, 30);
            
            // Permits
            await CreateAttendanceRecordAsync(emp1.Id, DateTime.Now.AddDays(-3), "إذن شخصي", 2, 0);
            
            // Loans
            await CreateEmployeeLoanAsync(emp1.Id, 5000, 5);
        }

        if (emp2 != null)
        {
            // Leaves
            if (annualLeave != null) await CreateEmployeeLeaveAsync(emp2.Id, annualLeave.Id, DateTime.Now.AddDays(5), 10);

            // Penalties
            await CreatePenaltyAsync(emp2.Id, PenaltyType.Warning, DateTime.Now.AddDays(-10), "تأخير متكرر");
            await CreatePenaltyAsync(emp2.Id, PenaltyType.SalaryDeduction, DateTime.Now.AddDays(-5), "عدم الالتزام بالزي الرسمي", 200);
        }

        var emp3 = await _employeeRepository.FirstOrDefaultAsync(x => x.EmployeeNumber == "EMP003");
        if (emp3 != null)
        {
            // Loan for EMP003
            await CreateEmployeeLoanAsync(emp3.Id, 10000, 10); // 10,000 Loan, 10 installments

            // Attendance for EMP003
            await CreateDailyAttendanceAsync(emp3.Id, DateTime.Now.AddDays(-1), 8, 0);
            await CreateDailyAttendanceAsync(emp3.Id, DateTime.Now.AddDays(-2), 8, 0);
        }

        await SeedFullMonthAttendanceAsync();
        await SeedFebruaryPayrollAsync();
    }

    private async Task SeedFullMonthAttendanceAsync()
    {
        var emp1 = await _employeeRepository.FirstOrDefaultAsync(x => x.EmployeeNumber == "EMP001");
        if (emp1 == null) return;

        // Seed February 2026
        var startDate = new DateTime(2026, 2, 1);
        var endDate = new DateTime(2026, 2, 28);

        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            // Skip Fridays (Weekend)
            if (date.DayOfWeek == DayOfWeek.Friday) continue;

            await CreateDailyAttendanceAsync(emp1.Id, date, 8, 0);
        }
    }

    private async Task SeedFebruaryPayrollAsync()
    {
        var startDate = new DateTime(2026, 2, 1);
        var endDate = new DateTime(2026, 2, 28);

        if (await _payrollRunRepository.FirstOrDefaultAsync(x => x.PeriodStart == startDate) != null) return;

        var runId = _guidGenerator.Create();
        var run = new PayrollRun(runId, _currentTenant.Id, startDate, endDate)
        {
            Status = PayrollRunStatus.Draft,
            TotalEarnings = 43000, // Combined for testing
            TotalDeductions = 3500,
            NetSalary = 39500
        };

        var emp1 = await _employeeRepository.FirstOrDefaultAsync(x => x.EmployeeNumber == "EMP001");
        var emp2 = await _employeeRepository.FirstOrDefaultAsync(x => x.EmployeeNumber == "EMP002");
        var basicItem = await _compensationItemRepository.FirstOrDefaultAsync(x => x.NameAr == "الراتب الأساسي");
        var housingItem = await _compensationItemRepository.FirstOrDefaultAsync(x => x.NameAr == "بدل سكن");
        var insuranceItem = await _compensationItemRepository.FirstOrDefaultAsync(x => x.NameAr == "تأمينات اجتماعية");

        if (emp1 != null && basicItem != null && housingItem != null && insuranceItem != null)
        {
            run.Lines.Add(new PayrollLine(_guidGenerator.Create(), runId, emp1.Id, basicItem.Id, 25000, CompensationNature.Allowance));
            run.Lines.Add(new PayrollLine(_guidGenerator.Create(), runId, emp1.Id, housingItem.Id, 5000, CompensationNature.Allowance));
            run.Lines.Add(new PayrollLine(_guidGenerator.Create(), runId, emp1.Id, insuranceItem.Id, 2000, CompensationNature.Deduction));
        }

        if (emp2 != null && basicItem != null && housingItem != null && insuranceItem != null)
        {
            run.Lines.Add(new PayrollLine(_guidGenerator.Create(), runId, emp2.Id, basicItem.Id, 18000, CompensationNature.Allowance));
            run.Lines.Add(new PayrollLine(_guidGenerator.Create(), runId, emp2.Id, housingItem.Id, 3000, CompensationNature.Allowance));
            run.Lines.Add(new PayrollLine(_guidGenerator.Create(), runId, emp2.Id, insuranceItem.Id, 1500, CompensationNature.Deduction));
        }

        await _payrollRunRepository.InsertAsync(run);
    }

    private async Task SeedCompensationItemsAsync()
    {
        await CreateCompensationItemAsync("الراتب الأساسي", "Basic Salary", CompensationNature.Allowance, CompensationValueType.Fixed, CompensationMethod.Credit);
        await CreateCompensationItemAsync("بدل سكن", "Housing Allowance", CompensationNature.Allowance, CompensationValueType.Percentage, CompensationMethod.Credit);
        await CreateCompensationItemAsync("بدل نقل", "Transportation Allowance", CompensationNature.Allowance, CompensationValueType.Fixed, CompensationMethod.Credit);
        await CreateCompensationItemAsync("طبيب نبطشية", "On-Call Allowance", CompensationNature.Allowance, CompensationValueType.Fixed, CompensationMethod.Credit);
        await CreateCompensationItemAsync("تأمينات اجتماعية", "Social Insurance", CompensationNature.Deduction, CompensationValueType.Percentage, CompensationMethod.Debit);
        await CreateCompensationItemAsync("ضريبة الدخل", "Income Tax", CompensationNature.Deduction, CompensationValueType.Percentage, CompensationMethod.Debit);
        await CreateCompensationItemAsync("بدل ساعات إضافية", "Overtime Allowance", CompensationNature.Allowance, CompensationValueType.Fixed, CompensationMethod.Credit);
        await CreateCompensationItemAsync("تأمين طبي", "Medical Insurance", CompensationNature.Deduction, CompensationValueType.Fixed, CompensationMethod.Debit);
        await CreateCompensationItemAsync("مكافأة أداء", "Performance Bonus", CompensationNature.Allowance, CompensationValueType.Fixed, CompensationMethod.Credit);
        await CreateCompensationItemAsync("بدل تمريض", "Nursing Allowance", CompensationNature.Allowance, CompensationValueType.Fixed, CompensationMethod.Credit);
        await CreateCompensationItemAsync("خصم غياب", "Absence Deduction", CompensationNature.Deduction, CompensationValueType.Fixed, CompensationMethod.Debit);
        await CreateCompensationItemAsync("بدل تخصص نادر", "Rare Specialty Allowance", CompensationNature.Allowance, CompensationValueType.Percentage, CompensationMethod.Credit);
    }

    private async Task SeedJobGradesAsync()
    {
        await CreateJobGradeAsync("G1", "درجة القيادة العليا", "Executive Grade", 15000);
        await CreateJobGradeAsync("G2", "درجة استشاري / مدير قسم", "Consultant / Manager Grade", 12000);
        await CreateJobGradeAsync("G3", "درجة أخصائي أول", "Senior Specialist Grade", 8000);
        await CreateJobGradeAsync("G4", "درجة فني / ممرض", "Technician / Nurse Grade", 5000);
        await CreateJobGradeAsync("G5", "درجة إدارية", "Administrative Grade", 4000);
    }

    private async Task SeedJobTitlesAsync()
    {
        await CreateJobTitleAsync("مدير المستشفى", "Hospital Manager");
        await CreateJobTitleAsync("مديرة الموارد البشرية", "HR Manager");
        await CreateJobTitleAsync("المدير المالي", "Finance Manager");
        await CreateJobTitleAsync("مدير تقنية المعلومات", "IT Manager");
        await CreateJobTitleAsync("استشاري أمراض باطنة", "Internal Medicine Consultant");
        await CreateJobTitleAsync("أخصائية جراحة عامة", "General Surgery Specialist");
        await CreateJobTitleAsync("رئيسة تمريض", "Head Nurse");
        await CreateJobTitleAsync("موظف استقبال", "Receptionist");
    }

    private async Task SeedEmployeesAsync()
    {
        // Administrtive / Management
        await CreateEmployeeAsync("EMP001", "أحمد الفلان", "Ahmed Al-Fulan", "ADM-HOSP", "مدير المستشفى", "G1");
        await CreateEmployeeAsync("EMP002", "سارة العلي", "Sarah Al-Ali", "ADM-HR", "مديرة الموارد البشرية", "G2");
        await CreateEmployeeAsync("EMP003", "محمد السيد", "Mohamed Al-Said", "ADM-FIN", "المدير المالي", "G2");
        await CreateEmployeeAsync("EMP004", "علي الحربي", "Ali Al-Harbi", "ADM-IT", "مدير تقنية المعلومات", "G3");

        // Medical
        await CreateEmployeeAsync("EMP005", "خالد العمري", "Khalid Al-Amri", "SPEC-IM", "استشاري أمراض باطنة", "G2");
        await CreateEmployeeAsync("EMP006", "فاطمة الزهراني", "Fatima Al-Zahrani", "SPEC-GS", "أخصائية جراحة عامة", "G3");
        
        // Nursing / Support
        await CreateEmployeeAsync("EMP007", "ليلى العتيبي", "Layla Al-Otaibi", "DEP-INP", "رئيسة تمريض", "G3");
        await CreateEmployeeAsync("EMP008", "عمر الغامدي", "Omar Al-Ghamdi", "ADM-RECB", "موظف استقبال", "G5");
    }

    private async Task CreateCompensationItemAsync(string nameAr, string nameEn, CompensationNature nature, CompensationValueType valueType, CompensationMethod method)
    {
        if (await _compensationItemRepository.FirstOrDefaultAsync(x => x.NameAr == nameAr) == null)
        {
            await _compensationItemRepository.InsertAsync(new CompensationItem(_guidGenerator.Create(), _currentTenant.Id, nameAr, nature)
            {
                DisplayName = nameEn,
                ValueType = valueType,
                Method = method,
                IsActive = true
            });
        }
    }

    private async Task CreateJobGradeAsync(string code, string nameAr, string nameEn, decimal baseSalary)
    {
        if (await _jobGradeRepository.FirstOrDefaultAsync(x => x.Code == code) == null)
        {
            await _jobGradeRepository.InsertAsync(new JobGrade(_guidGenerator.Create(), _currentTenant.Id, code, nameAr)
            {
                NameEn = nameEn,
                BaseSalary = baseSalary,
                IsActive = true
            });
        }
    }

    private async Task CreateJobTitleAsync(string nameAr, string nameEn)
    {
        if (await _jobTitleRepository.FirstOrDefaultAsync(x => x.NameAr == nameAr) == null)
        {
            await _jobTitleRepository.InsertAsync(new JobTitle(_guidGenerator.Create(), nameAr, nameEn));
        }
    }

    private async Task CreateEmployeeAsync(string employeeNumber, string nameAr, string nameEn, string departmentCode, string jobTitleAr, string jobGradeCode)
    {
        if (await _employeeRepository.FirstOrDefaultAsync(x => x.EmployeeNumber == employeeNumber) == null)
        {
            var dept = await _departmentRepository.FirstOrDefaultAsync(d => d.Code == departmentCode);
            var grade = await _jobGradeRepository.FirstOrDefaultAsync(g => g.Code == jobGradeCode);
            var title = await _jobTitleRepository.FirstOrDefaultAsync(t => t.NameAr == jobTitleAr);

            var employee = new Employee(_guidGenerator.Create(), _currentTenant.Id, employeeNumber, nameAr)
            {
                NameEn = nameEn,
                DepartmentId = dept?.Id,
                JobTitleId = title?.Id,
                JobTitle = jobTitleAr,
                JobGradeId = grade?.Id,
                Gender = nameAr.EndsWith("ة") || nameAr.Contains("فاطمة") || nameAr.Contains("سارة") || nameAr.Contains("ليلى") ? Gender.Female : Gender.Male,
                HireDate = DateTime.Now.AddYears(-2),
                IsActive = true,
                SalaryPaymentMethod = SalaryPaymentMethod.BankTransfer,
                ContractType = ContractType.Permanent
            };

            await _employeeRepository.InsertAsync(employee);
        }
    }

    private async Task CreateLeaveTypeAsync(string nameAr, int duration, bool isBalance, bool affectsSalary = false)
    {
        if (await _leaveTypeRepository.FirstOrDefaultAsync(x => x.NameAr == nameAr) == null)
        {
            await _leaveTypeRepository.InsertAsync(new LeaveType(_guidGenerator.Create(), _currentTenant.Id, nameAr)
            {
                Duration = duration,
                IsBalance = isBalance,
                AffectsSalary = affectsSalary,
                IsActive = true
            });
        }
    }

    private async Task CreateSalarySetupAsync(Guid empId, Guid itemId, decimal amount)
    {
        if (await _salarySetupRepository.FirstOrDefaultAsync(x => x.EmployeeId == empId && x.CompensationItemId == itemId) == null)
        {
            await _salarySetupRepository.InsertAsync(new SalarySetup(_guidGenerator.Create(), _currentTenant.Id, empId, itemId, amount));
        }
    }

    private async Task CreateDailyAttendanceAsync(Guid empId, DateTime date, int hours, int minutes)
    {
        if (await _dailyAttendanceRepository.FirstOrDefaultAsync(x => x.EmployeeId == empId && x.Date.Date == date.Date) == null)
        {
            var att = new DailyAttendance(_guidGenerator.Create(), _currentTenant.Id, empId, date.Date)
            {
                CheckInTime = date.Date.AddHours(8),
                CheckOutTime = date.Date.AddHours(8 + hours).AddMinutes(minutes),
                Status = AttendanceStatus.Present
            };
            att.CalculateWorkedHours();
            await _dailyAttendanceRepository.InsertAsync(att);
        }
    }

    private async Task CreateAttendanceRecordAsync(Guid empId, DateTime date, string type, int hours, int minutes)
    {
        if (await _attendanceRecordRepository.FirstOrDefaultAsync(x => x.EmployeeId == empId && x.Date.Date == date.Date && x.PermitType == type) == null)
        {
            await _attendanceRecordRepository.InsertAsync(new AttendanceRecord(_guidGenerator.Create(), _currentTenant.Id, empId, date)
            {
                PermitType = type,
                Hours = hours,
                Minutes = minutes,
                Reason = "ظرف خاص"
            });
        }
    }

    private async Task CreateEmployeeLoanAsync(Guid empId, decimal amount, int installments)
    {
        if (await _employeeLoanRepository.FirstOrDefaultAsync(x => x.EmployeeId == empId && x.Status == LoanStatus.Active) == null)
        {
            await _employeeLoanRepository.InsertAsync(new EmployeeLoan(_guidGenerator.Create(), _currentTenant.Id, empId, amount)
            {
                Installments = installments,
                StartDate = DateTime.Now.Date,
                Status = LoanStatus.Active,
                PaidAmount = 0
            });
        }
    }

    private async Task CreateEmployeeLeaveAsync(Guid empId, Guid leaveTypeId, DateTime start, int duration)
    {
        if (await _employeeLeaveRepository.FirstOrDefaultAsync(x => x.EmployeeId == empId && x.LeaveTypeId == leaveTypeId && x.StartDate.Date == start.Date) == null)
        {
            await _employeeLeaveRepository.InsertAsync(new EmployeeLeave(_guidGenerator.Create(), _currentTenant.Id, empId, leaveTypeId)
            {
                StartDate = start.Date,
                EndDate = start.Date.AddDays(duration - 1),
                Duration = duration,
                Entitled = 30,
                Used = 0,
                Balance = 30
            });
        }
    }

    private async Task CreatePenaltyAsync(Guid empId, PenaltyType type, DateTime date, string reason, decimal? amount = null)
    {
        if (await _penaltyRepository.FirstOrDefaultAsync(x => x.EmployeeId == empId && x.Date.Date == date.Date && x.PenaltyType == type) == null)
        {
            await _penaltyRepository.InsertAsync(new Penalty(_guidGenerator.Create(), _currentTenant.Id, empId, type, date)
            {
                Description = reason,
                Amount = amount,
                Date = date
            });
        }
    }
}

