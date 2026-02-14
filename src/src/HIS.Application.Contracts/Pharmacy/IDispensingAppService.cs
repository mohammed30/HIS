using System;
using System.Threading.Tasks;
using HIS.Pharmacy.Dtos;
using Volo.Abp.Application.Services;

namespace HIS.Pharmacy;

public interface IDispensingAppService : IApplicationService
{
    Task VerifyPrescriptionAsync(VerifyPrescriptionDto input);
    Task<DispensingVerificationDto> GetVerificationAsync(Guid medicalOrderId);
}
