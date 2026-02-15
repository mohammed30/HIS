using System;
using System.Threading.Tasks;
using HIS.Clinical;
using HIS.Pharmacy.Dtos;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;
using HIS.Inventory;

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
    }

    public async Task DispenseAsync(CreateDispensingDto input)
    {
        var inventoryManager = LazyServiceProvider.LazyGetRequiredService<InventoryManager>();
        
        var dispensing = new Dispensing(GuidGenerator.Create(), input.MedicalOrderId, input.PatientId)
        {
            CounselingNotes = input.CounselingNotes
        };

        foreach (var item in input.Items)
        {
            // Note: In real app, we'd loop through batches here or let manager handle it.
            // For now, manager handles LIFO and returns details.
            var details = await inventoryManager.DispenseStockAsync(
                Guid.Empty, // Default Warehouse or from input
                item.InventoryItemId,
                item.Quantity,
                $"Dispensing:{input.MedicalOrderId}"
            );

            foreach (var detail in details)
            {
                dispensing.AddItem(item.InventoryItemId, detail.BatchId, detail.Quantity, detail.BatchNumber, detail.UnitCost);
            }
        }

        var dispensingRepository = LazyServiceProvider.LazyGetRequiredService<IRepository<Dispensing, Guid>>();
        await dispensingRepository.InsertAsync(dispensing);
    }

    public async Task<DispensingLabelDto> GetLabelAsync(Guid dispensingId)
    {
         var dispensingRepository = LazyServiceProvider.LazyGetRequiredService<IRepository<Dispensing, Guid>>();
         var patientRepository = LazyServiceProvider.LazyGetRequiredService<IRepository<Patients.Patient, Guid>>();
         
         var dispensing = await dispensingRepository.GetAsync(dispensingId);
         var patient = await patientRepository.GetAsync(dispensing.PatientId);

         return new DispensingLabelDto
         {
             PatientName = patient.FullNameAr,
             MRN = patient.MRN,
             DispensedDate = dispensing.CreationTime.ToShortDateString(),
             PharmacistName = _currentUser.UserName ?? "Pharmacist"
             // Drug name and instructions would come from MedicalOrder/DispensedItems
         };
    }

    public async Task<DispensingVerificationDto> GetVerificationAsync(Guid medicalOrderId)
    {
        var verification = await _verificationRepository.FirstOrDefaultAsync(x => x.MedicalOrderId == medicalOrderId);
        if (verification == null) return null;
        
        return ObjectMapper.Map<DispensingVerification, DispensingVerificationDto>(verification);
    }
}
