using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HIS.Patients;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace HIS.Laboratory.Tests;

public abstract class LabReceptionAppServiceTests<TStartupModule> : LabTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly ILabReceptionAppService _labReceptionAppService;
    private readonly IRepository<Patient, Guid> _patientRepository;
    private readonly IRepository<LabTest, Guid> _labTestRepository;
    private readonly IRepository<LabRequest, Guid> _labRequestRepository;

    protected LabReceptionAppServiceTests()
    {
        _labReceptionAppService = GetRequiredService<ILabReceptionAppService>();
        _patientRepository = GetRequiredService<IRepository<Patient, Guid>>();
        _labTestRepository = GetRequiredService<IRepository<LabTest, Guid>>();
        _labRequestRepository = GetRequiredService<IRepository<LabRequest, Guid>>();
    }

    [Fact]
    public async Task CreateLabReceptionOrderAsync_Should_Create_Order_And_Requests()
    {
        // Arrange
        Guid patientId = Guid.NewGuid();
        Guid test1Id = Guid.NewGuid();
        Guid test2Id = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            var patient = new Patient(patientId, null, "MRN_LAB_REC_01", "محمود", "حسن", new DateTime(1988, 1, 1), Gender.Male, IdentityType.NationalId, "1234567894", "0500000004");
            await _patientRepository.InsertAsync(patient);

            await _labTestRepository.InsertAsync(new LabTest(test1Id, "TEST-10", "Fasting Blood Sugar", 100m));
            await _labTestRepository.InsertAsync(new LabTest(test2Id, "TEST-11", "Creatinine", 120m));
        });

        var input = new CreateLabReceptionOrderDto
        {
            PatientId = patientId,
            DoctorId = Guid.NewGuid(),
            TotalAmount = 220m,
            PaidAmount = 220m,
            TestIds = new List<Guid> { test1Id, test2Id }
        };

        // Act
        var result = await _labReceptionAppService.CreateLabReceptionOrderAsync(input);

        // Assert
        result.ShouldNotBe(Guid.Empty);

        var requests = await _labRequestRepository.GetListAsync(r => r.PatientId == patientId);
        requests.Count.ShouldBe(2);
        requests.ShouldContain(r => r.ServiceItemId == test1Id);
        requests.ShouldContain(r => r.ServiceItemId == test2Id);
        requests.ForEach(r => r.Status.ShouldBe(LabRequestStatus.Requested));
    }
}
