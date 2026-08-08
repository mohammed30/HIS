using System;
using System.Threading.Tasks;
using HIS.Insurance;
using Shouldly;
using Xunit;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using HIS.Patients;
using HIS.Settings;
using HIS.Services;

namespace HIS.Insurance.Tests;

public abstract class InsuranceAppServiceTests<TStartupModule> : HISTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IInsuranceCompanyAppService _insuranceCompanyAppService;
    private readonly IInsurancePlanAppService _insurancePlanAppService;
    private readonly IPatientInsuranceAppService _patientInsuranceAppService;
    private readonly IInsuranceServicePriceAppService _insuranceServicePriceAppService;

    private readonly IRepository<InsuranceCompany, Guid> _insuranceCompanyRepository;
    private readonly IRepository<InsurancePlan, Guid> _insurancePlanRepository;
    private readonly IRepository<Patient, Guid> _patientRepository;
    private readonly IRepository<ServiceItem, Guid> _serviceItemRepository;

    protected InsuranceAppServiceTests()
    {
        _insuranceCompanyAppService = GetRequiredService<IInsuranceCompanyAppService>();
        _insurancePlanAppService = GetRequiredService<IInsurancePlanAppService>();
        _patientInsuranceAppService = GetRequiredService<IPatientInsuranceAppService>();
        _insuranceServicePriceAppService = GetRequiredService<IInsuranceServicePriceAppService>();

        _insuranceCompanyRepository = GetRequiredService<IRepository<InsuranceCompany, Guid>>();
        _insurancePlanRepository = GetRequiredService<IRepository<InsurancePlan, Guid>>();
        _patientRepository = GetRequiredService<IRepository<Patient, Guid>>();
        _serviceItemRepository = GetRequiredService<IRepository<ServiceItem, Guid>>();
    }

    [Fact]
    public async Task CreateInsuranceCompanyAsync_Should_Create_Company()
    {
        // Arrange
        var input = new CreateUpdateInsuranceCompanyDto
        {
            Code = "INS-001",
            NameAr = "شركة التأمين الوطنية",
            NameEn = "National Insurance Co.",
            IsActive = true
        };

        // Act
        var result = await _insuranceCompanyAppService.CreateAsync(input);

        // Assert
        result.ShouldNotBeNull();
        result.Code.ShouldBe("INS-001");

        var companyInDb = await _insuranceCompanyRepository.GetAsync(result.Id);
        companyInDb.ShouldNotBeNull();
        companyInDb.NameAr.ShouldBe("شركة التأمين الوطنية");
    }

    [Fact]
    public async Task CreateInsurancePlanAsync_Should_Create_Plan_Under_Company()
    {
        // Arrange
        Guid companyId = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            await _insuranceCompanyRepository.InsertAsync(new InsuranceCompany(companyId, null, "INS-002", "التأمين التعاوني"));
        });

        var input = new CreateUpdateInsurancePlanDto
        {
            InsuranceCompanyId = companyId,
            Code = "PLAN-A",
            NameAr = "خطة الفئة أ",
            PatientDeductiblePercentage = 10, // 10%
            IsActive = true
        };

        // Act
        var result = await _insurancePlanAppService.CreateAsync(input);

        // Assert
        result.ShouldNotBeNull();
        result.InsuranceCompanyId.ShouldBe(companyId);
        result.PatientDeductiblePercentage.ShouldBe(10);

        var planInDb = await _insurancePlanRepository.GetAsync(result.Id);
        planInDb.ShouldNotBeNull();
    }

    [Fact]
    public async Task CreatePatientInsuranceAsync_Should_Link_Patient_With_Insurance()
    {
        // Arrange
        Guid companyId = Guid.NewGuid();
        Guid planId = Guid.NewGuid();
        Guid patientId = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            await _insuranceCompanyRepository.InsertAsync(new InsuranceCompany(companyId, null, "INS-003", "تأمين 3"));
            await _insurancePlanRepository.InsertAsync(new InsurancePlan(planId, null, companyId, "P-3", "خطة 3"));
            
            var patient = new Patient(patientId, null, "PAT-001", "أحمد علي", "Ahmed Ali", new DateTime(1990, 1, 1), HIS.Patients.Enums.Gender.Male, HIS.Patients.Enums.IdentityType.NationalId, "1000000000", "0500000000");
            await _patientRepository.InsertAsync(patient);
        });

        var input = new CreateUpdatePatientInsuranceDto
        {
            PatientId = patientId,
            InsurancePlanId = planId,
            PolicyNumber = "POL-12345",
            StartDate = DateTime.Now.AddDays(-10),
            EndDate = DateTime.Now.AddYears(1)
        };

        // Act
        var result = await _patientInsuranceAppService.CreateAsync(input);

        // Assert
        result.ShouldNotBeNull();
        result.PatientId.ShouldBe(patientId);
        result.InsurancePlanId.ShouldBe(planId);
        result.PolicyNumber.ShouldBe("POL-12345");
    }

    [Fact]
    public async Task CreateInsuranceServicePriceAsync_Should_Set_Custom_Price()
    {
        // Arrange
        Guid companyId = Guid.NewGuid();
        Guid planId = Guid.NewGuid();
        Guid serviceItemId = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            await _insuranceCompanyRepository.InsertAsync(new InsuranceCompany(companyId, null, "INS-004", "تأمين 4"));
            await _insurancePlanRepository.InsertAsync(new InsurancePlan(planId, null, companyId, "P-4", "خطة 4"));
            
            var service = new ServiceItem(serviceItemId, "SRV-001", "كشف طبيب عام", ServiceCategory.Consultation, null);
            await _serviceItemRepository.InsertAsync(service);
        });

        var input = new CreateUpdateInsuranceServicePriceDto
        {
            InsurancePlanId = planId,
            ServiceItemId = serviceItemId,
            CustomPrice = 150m,
            Notes = "Custom price for plan 4"
        };

        // Act
        var result = await _insuranceServicePriceAppService.CreateAsync(input);

        // Assert
        result.ShouldNotBeNull();
        result.CustomPrice.ShouldBe(150m);
    }
}
