using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

// Assuming these namespaces based on typical ABP structure for HIS
using HIS.Patients;
using HIS.Laboratory.Dtos;
using System.Collections.Generic;

namespace HIS.Laboratory;

// Example DTO for the transaction
public class CreateLabReceptionOrderDto
{
    public Guid PatientId { get; set; }
    public Guid? DoctorId { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public List<Guid> TestIds { get; set; } = new List<Guid>();
}

public interface ILabReceptionAppService : IApplicationService
{
    Task<Guid> CreateLabReceptionOrderAsync(CreateLabReceptionOrderDto input);
}

public class LabReceptionAppService : ApplicationService, ILabReceptionAppService
{
    private readonly IRepository<Patient, Guid> _patientRepository;
    private readonly IRepository<LabRequest, Guid> _labRequestRepository;

    public LabReceptionAppService(
        IRepository<Patient, Guid> patientRepository,
        IRepository<LabRequest, Guid> labRequestRepository)
    {
        _patientRepository = patientRepository;
        _labRequestRepository = labRequestRepository;
    }

    [HttpPost]
    [Route("api/app/lab-reception/create-order")]
    [UnitOfWork] // Ensures all operations succeed or fail together
    public async Task<Guid> CreateLabReceptionOrderAsync(CreateLabReceptionOrderDto input)
    {
        // 1. Ensure Patient exists
        var patient = await _patientRepository.GetAsync(input.PatientId);

        // TODO: Create Invoice using Invoice Domain Service
        // TODO: Create Payment using Payment Domain Service

        // 2. Create Lab Requests
        foreach (var testId in input.TestIds)
        {
            var request = new LabRequest(GuidGenerator.Create(), patient.Id, input.DoctorId.GetValueOrDefault(), testId);
            await _labRequestRepository.InsertAsync(request);
        }

        return Guid.NewGuid(); // Should return the invoice ID
    }
}
