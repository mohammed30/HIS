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
    private readonly IGuidGenerator _guidGenerator;
    private readonly ICurrentTenant _currentTenant;

    public HRDataSeedContributor(
        IRepository<Employee, Guid> employeeRepository,
        IRepository<Department, Guid> departmentRepository,
        IRepository<JobGrade, Guid> jobGradeRepository,
        IRepository<JobTitle, Guid> jobTitleRepository,
        IRepository<CompensationItem, Guid> compensationItemRepository,
        IGuidGenerator guidGenerator,
        ICurrentTenant currentTenant)
    {
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;
        _jobGradeRepository = jobGradeRepository;
        _jobTitleRepository = jobTitleRepository;
        _compensationItemRepository = compensationItemRepository;
        _guidGenerator = guidGenerator;
        _currentTenant = currentTenant;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        await SeedCompensationItemsAsync();
        await SeedJobGradesAsync();
        await SeedJobTitlesAsync();
        await SeedEmployeesAsync();
    }

    private async Task SeedCompensationItemsAsync()
    {
        // ... (unchanged)
        await CreateCompensationItemAsync("الراتب الأساسي", "Basic Salary", CompensationNature.Allowance, CompensationValueType.Fixed, CompensationMethod.Credit);
        await CreateCompensationItemAsync("بدل سكن", "Housing Allowance", CompensationNature.Allowance, CompensationValueType.Percentage, CompensationMethod.Credit);
        await CreateCompensationItemAsync("بدل نقل", "Transportation Allowance", CompensationNature.Allowance, CompensationValueType.Fixed, CompensationMethod.Credit);
        await CreateCompensationItemAsync("طبيب نبطشية", "On-Call Allowance", CompensationNature.Allowance, CompensationValueType.Fixed, CompensationMethod.Credit);
        await CreateCompensationItemAsync("تأمينات اجتماعية", "Social Insurance", CompensationNature.Deduction, CompensationValueType.Percentage, CompensationMethod.Debit);
        await CreateCompensationItemAsync("ضريبة الدخل", "Income Tax", CompensationNature.Deduction, CompensationValueType.Percentage, CompensationMethod.Debit);
    }

    private async Task SeedJobGradesAsync()
    {
        // ... (unchanged)
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
        // ... (unchanged)
}

    private async Task CreateJobGradeAsync(string code, string nameAr, string nameEn, decimal baseSalary)
    {
        // ... (unchanged)
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
}

