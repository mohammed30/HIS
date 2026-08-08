using System;
using System.Linq;
using System.Threading.Tasks;
using HIS.HR;
using HIS.HR.Enums;
using Shouldly;
using Xunit;
using Volo.Abp.Domain.Repositories;
using HIS.Settings;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Modularity;

namespace HIS.HR.Tests;

public abstract class HRAppServiceTests<TStartupModule> : HRTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly HRAppService _hrAppService;
    private readonly IRepository<Employee, Guid> _employeeRepository;
    private readonly IRepository<CompensationItem, Guid> _compensationItemRepository;
    private readonly IRepository<SalarySetup, Guid> _salarySetupRepository;
    private readonly IRepository<LeaveType, Guid> _leaveTypeRepository;
    private readonly IRepository<EmployeeLeave, Guid> _employeeLeaveRepository;
    private readonly IRepository<PayrollRun, Guid> _payrollRunRepository;
    private readonly IRepository<Department, Guid> _departmentRepository;
    
    public HRAppServiceTests()
    {
        _hrAppService = GetRequiredService<HRAppService>();
        _employeeRepository = GetRequiredService<IRepository<Employee, Guid>>();
        _compensationItemRepository = GetRequiredService<IRepository<CompensationItem, Guid>>();
        _salarySetupRepository = GetRequiredService<IRepository<SalarySetup, Guid>>();
        _leaveTypeRepository = GetRequiredService<IRepository<LeaveType, Guid>>();
        _employeeLeaveRepository = GetRequiredService<IRepository<EmployeeLeave, Guid>>();
        _payrollRunRepository = GetRequiredService<IRepository<PayrollRun, Guid>>();
        _departmentRepository = GetRequiredService<IRepository<Department, Guid>>();
    }

    [Fact]
    public async Task CreateEmployeeAsync_Should_Create_Employee()
    {
        // Arrange
        Guid departmentId = Guid.NewGuid();
        await WithUnitOfWorkAsync(async () =>
        {
            await _departmentRepository.InsertAsync(new Department(departmentId, "IT", "Information Technology", null));
        });

        var input = new CreateUpdateEmployeeDto
        {
            EmployeeNumber = "EMP-001",
            NameAr = "عبدالله",
            NameEn = "Abdullah",
            Gender = Gender.Male,
            DepartmentId = departmentId,
            Phone = "0550000000",
            Email = "abd@test.com"
        };

        // Act
        var result = await _hrAppService.CreateEmployeeAsync(input);

        // Assert
        result.ShouldNotBeNull();
        result.EmployeeNumber.ShouldBe("EMP-001");
        result.NameAr.ShouldBe("عبدالله");

        var empInDb = await _employeeRepository.GetAsync(result.Id);
        empInDb.ShouldNotBeNull();
        empInDb.Gender.ShouldBe(Gender.Male);
    }

    [Fact]
    public async Task CreateSalarySetupAsync_Should_Setup_Employee_Salary()
    {
        // Arrange
        Guid employeeId = Guid.NewGuid();
        Guid compItemId = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            await _employeeRepository.InsertAsync(new Employee(employeeId, null, "EMP-002", "سالم"));
            await _compensationItemRepository.InsertAsync(new CompensationItem(compItemId, null, "Basic Salary", CompensationType.Earning, true));
        });

        var input = new CreateUpdateSalarySetupDto
        {
            EmployeeId = employeeId,
            CompensationItemId = compItemId,
            Amount = 5000m,
            IsRecurring = true,
            StartDate = DateTime.Now.AddMonths(-1),
            IsActive = true
        };

        // Act
        var result = await _hrAppService.CreateSalarySetupAsync(input);

        // Assert
        result.ShouldNotBeNull();
        result.EmployeeId.ShouldBe(employeeId);
        result.CompensationItemId.ShouldBe(compItemId);
        result.Amount.ShouldBe(5000m);

        var setups = await _salarySetupRepository.GetListAsync(s => s.EmployeeId == employeeId);
        setups.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task ProcessPayrollAsync_Should_Generate_PayrollRun()
    {
        // Arrange
        Guid departmentId = Guid.NewGuid();
        Guid employeeId = Guid.NewGuid();
        Guid compItemId = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            var dept = new Department(departmentId, null, "HR", "Human Resources");
            await _departmentRepository.InsertAsync(dept);

            var emp = new Employee(employeeId, null, "EMP-003", "محمد");
            emp.DepartmentId = departmentId;
            await _employeeRepository.InsertAsync(emp);

            await _compensationItemRepository.InsertAsync(new CompensationItem(compItemId, null, "Basic Salary", CompensationNature.Allowance));

            await _salarySetupRepository.InsertAsync(new SalarySetup(Guid.NewGuid(), null, employeeId, compItemId, 6000m));
        });

        var input = new ProcessPayrollDto
        {
            PeriodStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
            PeriodEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)),
            DepartmentId = departmentId
        };

        // Act
        var result = await _hrAppService.ProcessPayrollAsync(input);

        // Assert
        result.ShouldNotBeNull();
        result.TotalEarnings.ShouldBeGreaterThan(0);

        var runInDb = await _payrollRunRepository.GetAsync(result.Id);
        runInDb.ShouldNotBeNull();
    }

    [Fact]
    public async Task CreateEmployeeLeaveAsync_Should_Create_Leave()
    {
        // Arrange
        Guid employeeId = Guid.NewGuid();
        Guid leaveTypeId = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            await _employeeRepository.InsertAsync(new Employee(employeeId, null, "EMP-004", "عمر"));
            await _leaveTypeRepository.InsertAsync(new LeaveType(leaveTypeId, null, "Annual Leave"));
        });

        var input = new CreateUpdateEmployeeLeaveDto
        {
            EmployeeId = employeeId,
            LeaveTypeId = leaveTypeId,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(5),
            Duration = 5,
            Notes = "Vacation"
        };

        // Act
        var result = await _hrAppService.CreateEmployeeLeaveAsync(input);

        // Assert
        result.ShouldNotBeNull();
        result.EmployeeId.ShouldBe(employeeId);
        result.LeaveTypeId.ShouldBe(leaveTypeId);
        result.Duration.ShouldBe(5);

        var leaveInDb = await _employeeLeaveRepository.GetAsync(result.Id);
        leaveInDb.ShouldNotBeNull();
    }
}
