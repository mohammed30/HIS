using System;
using System.Threading.Tasks;
using HIS.Services;
using HIS.Inventory;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace HIS.Clinical;

public class MedicalOrderAppService : CrudAppService<MedicalOrder, MedicalOrderDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateMedicalOrderDto>, IMedicalOrderAppService
{
    private readonly IRepository<ServiceItem, Guid> _serviceItemRepository;
    private readonly IRepository<Insurance.InsuranceServicePrice, Guid> _insurancePriceRepository;
    private readonly IRepository<Inpatient.Admission, Guid> _admissionRepository;

    public MedicalOrderAppService(
        IRepository<MedicalOrder, Guid> repository, 
        IRepository<ServiceItem, Guid> serviceItemRepository,
        IRepository<Insurance.InsuranceServicePrice, Guid> insurancePriceRepository,
        IRepository<Inpatient.Admission, Guid> admissionRepository) 
        : base(repository)
    {
        _serviceItemRepository = serviceItemRepository;
        _insurancePriceRepository = insurancePriceRepository;
        _admissionRepository = admissionRepository;
    }

    public override async Task<MedicalOrderDto> CreateAsync(CreateUpdateMedicalOrderDto input)
    {
        var serviceItem = await _serviceItemRepository.GetAsync(input.ServiceItemId);
        decimal price = serviceItem.Price;

        // Check for custom insurance price if linked to an admission
        if (input.AdmissionId.HasValue)
        {
            var admission = await _admissionRepository.FindAsync(input.AdmissionId.Value);
            if (admission != null && admission.PatientInsuranceId.HasValue)
            {
                var patientInsuranceRepo = LazyServiceProvider.LazyGetRequiredService<IRepository<Insurance.PatientInsurance, Guid>>();
                var patientInsurance = await patientInsuranceRepo.FindAsync(admission.PatientInsuranceId.Value);
                if (patientInsurance != null)
                {
                    var customPrice = await _insurancePriceRepository.FirstOrDefaultAsync(x => 
                        x.InsurancePlanId == patientInsurance.InsurancePlanId && 
                        x.ServiceItemId == input.ServiceItemId);
                    
                    if (customPrice != null)
                    {
                        price = customPrice.CustomPrice;
                    }
                }
            }
        }

        var entity = new MedicalOrder(
            GuidGenerator.Create(),
            input.PatientId,
            input.Type,
            input.ServiceItemId,
            serviceItem.Name,
            price
        );
        entity.AdmissionId = input.AdmissionId;
        entity.ClinicalNotes = input.ClinicalNotes;
        entity.Details = input.Details;
        entity.Quantity = input.Quantity > 0 ? input.Quantity : 1;
        
        if (CurrentUser.Id.HasValue)
        {
            entity.DoctorId = CurrentUser.Id.Value;
        }

        await Repository.InsertAsync(entity);

        // For consumable orders, deduct stock from inventory
        if (input.Type == OrderType.Consumable)
        {
            try
            {
                var inventoryManager = LazyServiceProvider.LazyGetRequiredService<InventoryManager>();
                var warehouseRepo = LazyServiceProvider.LazyGetRequiredService<IRepository<Warehouse, Guid>>();
                var settingProvider = LazyServiceProvider.LazyGetRequiredService<Volo.Abp.Settings.ISettingProvider>();
                
                var mainWarehouseIdStr = await settingProvider.GetOrNullAsync("HIS.Inventory.MainWarehouseId");
                Guid? targetWarehouseId = null;
                
                if (!string.IsNullOrEmpty(mainWarehouseIdStr) && Guid.TryParse(mainWarehouseIdStr, out var mainId))
                {
                    targetWarehouseId = mainId;
                }
                else
                {
                    var fallback = await warehouseRepo.FirstOrDefaultAsync();
                    targetWarehouseId = fallback?.Id;
                }

                if (targetWarehouseId.HasValue)
                {
                    await inventoryManager.DispenseStockAsync(
                        targetWarehouseId.Value,
                        input.ServiceItemId,
                        entity.Quantity,
                        $"طلب مستهلكات - مريض"
                    );
                }
            }
            catch
            {
                // Inventory deduction is best-effort; order still proceeds
            }
        }

        return await MapToGetOutputDtoAsync(entity);
    }
}
