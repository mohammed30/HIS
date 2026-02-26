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

    public MedicalOrderAppService(IRepository<MedicalOrder, Guid> repository, IRepository<ServiceItem, Guid> serviceItemRepository) 
        : base(repository)
    {
        _serviceItemRepository = serviceItemRepository;
    }

    public override async Task<MedicalOrderDto> CreateAsync(CreateUpdateMedicalOrderDto input)
    {
        var serviceItem = await _serviceItemRepository.GetAsync(input.ServiceItemId);

        var entity = new MedicalOrder(
            GuidGenerator.Create(),
            input.PatientId,
            input.Type,
            input.ServiceItemId,
            serviceItem.Name,
            serviceItem.Price
        );
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
                var mainWarehouse = await warehouseRepo.FirstOrDefaultAsync(w => w.Name.Contains("رئيسي") || w.Name.Contains("Main") || w.Name.Contains("مركزي"));
                if (mainWarehouse != null)
                {
                    await inventoryManager.DispenseStockAsync(
                        mainWarehouse.Id,
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
