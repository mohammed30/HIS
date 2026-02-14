using System;
using System.Threading.Tasks;
using HIS.Clinical;
using HIS.Pharmacy.Dtos;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace HIS.Pharmacy;

public class DispensingAppService : HISAppService, IDispensingAppService
{
    private readonly IRepository<DispensingVerification, Guid> _verificationRepository;
    private readonly IRepository<MedicalOrder, Guid> _medicalOrderRepository;
    private readonly ICurrentUser _currentUser;

    public DispensingAppService(
        IRepository<DispensingVerification, Guid> verificationRepository,
        IRepository<MedicalOrder, Guid> medicalOrderRepository,
        ICurrentUser currentUser)
    {
        _verificationRepository = verificationRepository;
        _medicalOrderRepository = medicalOrderRepository;
        _currentUser = currentUser;
    }

    public async Task VerifyPrescriptionAsync(VerifyPrescriptionDto input)
    {
        var order = await _medicalOrderRepository.GetAsync(input.MedicalOrderId);
        if (order == null) throw new UserFriendlyException("Prescription not found");

        var verification = new DispensingVerification(
            GuidGenerator.Create(),
            input.MedicalOrderId,
            _currentUser.Id.GetValueOrDefault(),
            input.IsApproved,
            input.SafetyCheckComments
        );

        await _verificationRepository.InsertAsync(verification);
        
        // Optionally update Order status if needed
    }

    public async Task<DispensingVerificationDto> GetVerificationAsync(Guid medicalOrderId)
    {
        var verification = await _verificationRepository.FirstOrDefaultAsync(x => x.MedicalOrderId == medicalOrderId);
        if (verification == null) return null;
        
        return ObjectMapper.Map<DispensingVerification, DispensingVerificationDto>(verification);
    }
}
