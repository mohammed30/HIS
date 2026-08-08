using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HIS.Patients;
using HIS.Services;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace HIS.Radiology.Tests;

public abstract class RadiologyAppServiceTests<TStartupModule> : RadiologyTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IRadiologyAppService _radiologyAppService;
    private readonly IRepository<Patient, Guid> _patientRepository;
    private readonly IRepository<RadiologyItem, Guid> _radiologyItemRepository;
    private readonly IRepository<RadiologyRequest, Guid> _radiologyRequestRepository;

    protected RadiologyAppServiceTests()
    {
        _radiologyAppService = GetRequiredService<IRadiologyAppService>();
        _patientRepository = GetRequiredService<IRepository<Patient, Guid>>();
        _radiologyItemRepository = GetRequiredService<IRepository<RadiologyItem, Guid>>();
        _radiologyRequestRepository = GetRequiredService<IRepository<RadiologyRequest, Guid>>();
    }

    [Fact]
    public async Task CreateAsync_Should_Create_RadiologyRequest()
    {
        // Arrange
        Guid patientId = Guid.NewGuid();
        Guid itemId = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            var patient = new Patient(patientId, null, "MRN_RAD_01", "عمر", "فاروق", new DateTime(1985, 5, 5), Gender.Male, IdentityType.NationalId, "1234567895", "0500000005");
            await _patientRepository.InsertAsync(patient);

            var item = new RadiologyItem(itemId, "RAD-01", "X-Ray Chest", Guid.NewGuid(), "X-Ray", "Chest");
            await _radiologyItemRepository.InsertAsync(item);
        });

        var input = new CreateUpdateRadiologyRequestDto
        {
            PatientId = patientId,
            DoctorId = null,
            IsExternalDoctor = true,
            ExternalDoctorName = "د. صالح",
            RadiologyItemId = itemId,
            Status = RadiologyRequestStatus.Requested
        };

        // Act
        var result = await _radiologyAppService.CreateAsync(input);

        // Assert
        result.ShouldNotBeNull();
        result.PatientId.ShouldBe(patientId);
        result.RadiologyItemId.ShouldBe(itemId);
        result.Status.ShouldBe(RadiologyRequestStatus.Requested);

        var requestInDb = await _radiologyRequestRepository.GetAsync(result.Id);
        requestInDb.ShouldNotBeNull();
        requestInDb.IsExternalDoctor.ShouldBeTrue();
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_RadiologyRequest()
    {
        // Arrange
        Guid requestId = Guid.NewGuid();
        Guid patientId = Guid.NewGuid();
        Guid itemId = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            var patient = new Patient(patientId, null, "MRN_RAD_02", "نور", "علي", new DateTime(1993, 2, 2), Gender.Female, IdentityType.NationalId, "1234567896", "0500000006");
            await _patientRepository.InsertAsync(patient);

            var item = new RadiologyItem(itemId, "RAD-02", "MRI Brain", Guid.NewGuid(), "MRI", "Brain");
            await _radiologyItemRepository.InsertAsync(item);

            var request = new RadiologyRequest(requestId, patientId, Guid.NewGuid(), itemId, "REQ-002");
            request.Status = RadiologyRequestStatus.Requested;
            await _radiologyRequestRepository.InsertAsync(request);
        });

        var updateInput = new CreateUpdateRadiologyRequestDto
        {
            PatientId = patientId,
            RadiologyItemId = itemId,
            Status = RadiologyRequestStatus.Reported,
            ReportBody = "Normal MRI",
            TechnicianNotes = "Patient was cooperative"
        };

        // Act
        var result = await _radiologyAppService.UpdateAsync(requestId, updateInput);

        // Assert
        result.ShouldNotBeNull();
        result.Status.ShouldBe(RadiologyRequestStatus.Reported);
        result.ReportBody.ShouldBe("Normal MRI");

        var requestInDb = await _radiologyRequestRepository.GetAsync(requestId);
        requestInDb.Status.ShouldBe(RadiologyRequestStatus.Reported);
        requestInDb.ReportDate.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetPatientResultsAsync_Should_Return_Results()
    {
        // Arrange
        Guid patientId = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            var patient = new Patient(patientId, null, "MRN_RAD_03", "خالد", "عبدالله", new DateTime(1975, 4, 4), Gender.Male, IdentityType.NationalId, "1234567897", "0500000007");
            await _patientRepository.InsertAsync(patient);

            var item = new RadiologyItem(Guid.NewGuid(), "RAD-03", "CT Scan", Guid.NewGuid(), "CT", "Body");
            await _radiologyItemRepository.InsertAsync(item);

            var request = new RadiologyRequest(Guid.NewGuid(), patientId, Guid.NewGuid(), item.Id, "REQ-003");
            request.Status = RadiologyRequestStatus.Reported;
            await _radiologyRequestRepository.InsertAsync(request);
            
            var requestPending = new RadiologyRequest(Guid.NewGuid(), patientId, Guid.NewGuid(), item.Id, "REQ-004");
            requestPending.Status = RadiologyRequestStatus.Requested;
            await _radiologyRequestRepository.InsertAsync(requestPending);
        });

        // Act
        var results = await _radiologyAppService.GetPatientResultsAsync(patientId);

        // Assert
        results.ShouldNotBeNull();
        results.Count.ShouldBeGreaterThanOrEqualTo(1);
        results.ShouldContain(r => r.Status == RadiologyRequestStatus.Reported);
    }
}
