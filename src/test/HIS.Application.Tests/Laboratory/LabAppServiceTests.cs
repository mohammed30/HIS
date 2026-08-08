using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HIS.Laboratory.Dtos;
using HIS.Patients;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace HIS.Laboratory.Tests;

public abstract class LabAppServiceTests<TStartupModule> : LabTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly ILabAppService _labAppService;
    private readonly IRepository<Patient, Guid> _patientRepository;
    private readonly IRepository<LabTest, Guid> _labTestRepository;
    private readonly IRepository<LabRequest, Guid> _labRequestRepository;
    private readonly IRepository<LabTestCategory, Guid> _categoryRepository;

    protected LabAppServiceTests()
    {
        _labAppService = GetRequiredService<ILabAppService>();
        _patientRepository = GetRequiredService<IRepository<Patient, Guid>>();
        _labTestRepository = GetRequiredService<IRepository<LabTest, Guid>>();
        _labRequestRepository = GetRequiredService<IRepository<LabRequest, Guid>>();
        _categoryRepository = GetRequiredService<IRepository<LabTestCategory, Guid>>();
    }

    [Fact]
    public async Task CreateTestAsync_Should_Create_LabTest()
    {
        // Arrange
        Guid categoryId = Guid.NewGuid();
        await WithUnitOfWorkAsync(async () =>
        {
            await _categoryRepository.InsertAsync(new LabTestCategory(categoryId, "CAT-01", "Blood Tests", null));
        });

        var input = new CreateUpdateLabTestDto
        {
            Code = "TEST-01",
            Name = "Complete Blood Count",
            CategoryId = categoryId,
            Price = 150m,
            IsActive = true
        };

        // Act
        var result = await _labAppService.CreateTestAsync(input);

        // Assert
        result.ShouldNotBeNull();
        result.Code.ShouldBe("TEST-01");
        result.Name.ShouldBe("Complete Blood Count");

        var testInDb = await _labTestRepository.GetAsync(result.Id);
        testInDb.ShouldNotBeNull();
        testInDb.Price.ShouldBe(150m);
    }

    [Fact]
    public async Task CreateRequestAsync_Should_Create_LabRequest()
    {
        // Arrange
        Guid patientId = Guid.NewGuid();
        Guid testId = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            var patient = new Patient(patientId, null, "MRN_LAB_01", "علي", "سالم", new DateTime(1990, 1, 1), Gender.Male, IdentityType.NationalId, "1234567891", "0500000001");
            await _patientRepository.InsertAsync(patient);

            await _labTestRepository.InsertAsync(new LabTest(testId, "TEST-02", "HbA1c", 200m));
        });

        var input = new CreateLabRequestDto
        {
            PatientId = patientId,
            DoctorId = Guid.NewGuid(),
            ServiceItemId = testId,
            Notes = "Check diabetes"
        };

        // Act
        var result = await _labAppService.CreateRequestAsync(input);

        // Assert
        result.ShouldNotBeNull();
        result.PatientId.ShouldBe(patientId);
        result.ServiceItemId.ShouldBe(testId);
        result.Status.ShouldBe(LabRequestStatus.Requested);

        var requestInDb = await _labRequestRepository.GetAsync(result.Id);
        requestInDb.ShouldNotBeNull();
    }

    [Fact]
    public async Task CollectSampleAsync_Should_Update_Status()
    {
        // Arrange
        Guid requestId = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            var patient = new Patient(Guid.NewGuid(), null, "MRN_LAB_02", "سارة", "محمد", new DateTime(1992, 1, 1), Gender.Female, IdentityType.NationalId, "1234567892", "0500000002");
            await _patientRepository.InsertAsync(patient);

            var test = new LabTest(Guid.NewGuid(), "TEST-03", "Lipid Profile", 250m);
            await _labTestRepository.InsertAsync(test);

            var request = new LabRequest(requestId, patient.Id, Guid.NewGuid(), test.Id)
            {
                Status = LabRequestStatus.Requested
            };
            await _labRequestRepository.InsertAsync(request);
        });

        // Act
        var result = await _labAppService.CollectSampleAsync(requestId);

        // Assert
        result.ShouldNotBeNull();
        result.Status.ShouldBe(LabRequestStatus.SampleCollected);

        var requestInDb = await _labRequestRepository.GetAsync(requestId);
        requestInDb.Status.ShouldBe(LabRequestStatus.SampleCollected);
    }

    [Fact]
    public async Task CompleteRequestAsync_Should_Update_Result()
    {
        // Arrange
        Guid requestId = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            var patient = new Patient(Guid.NewGuid(), null, "MRN_LAB_03", "أحمد", "علي", new DateTime(1985, 1, 1), Gender.Male, IdentityType.NationalId, "1234567893", "0500000003");
            await _patientRepository.InsertAsync(patient);

            var test = new LabTest(Guid.NewGuid(), "TEST-04", "Vitamin D", 300m);
            await _labTestRepository.InsertAsync(test);

            var request = new LabRequest(requestId, patient.Id, Guid.NewGuid(), test.Id)
            {
                Status = LabRequestStatus.SampleCollected
            };
            await _labRequestRepository.InsertAsync(request);
        });

        var input = new UpdateLabResultDto
        {
            Result = "Normal level (30-100 ng/mL): 45 ng/mL",
            Notes = "OK"
        };

        // Act
        var result = await _labAppService.CompleteRequestAsync(requestId, input);

        // Assert
        result.ShouldNotBeNull();
        result.Status.ShouldBe(LabRequestStatus.Completed);
        result.Result.ShouldBe(input.Result);

        var requestInDb = await _labRequestRepository.GetAsync(requestId);
        requestInDb.Status.ShouldBe(LabRequestStatus.Completed);
    }
}
