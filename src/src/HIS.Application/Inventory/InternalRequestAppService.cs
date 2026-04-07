using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HIS.Inventory.Dtos;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp;

namespace HIS.Inventory;

[Authorize]
public class InternalRequestAppService : CrudAppService<InternalRequest, InternalRequestDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateInternalRequestDto>, IInternalRequestAppService
{
    private readonly InventoryManager _inventoryManager;
    private readonly IRepository<Warehouse, Guid> _warehouseRepository;

    public InternalRequestAppService(
        IRepository<InternalRequest, Guid> repository,
        IRepository<Warehouse, Guid> warehouseRepository,
        InventoryManager inventoryManager) 
        : base(repository)
    {
        _warehouseRepository = warehouseRepository;
        _inventoryManager = inventoryManager;
    }

    public override async Task<InternalRequestDto> CreateAsync(CreateUpdateInternalRequestDto input)
    {
        if (input.FulfilledByWarehouseId == Guid.Empty)
        {
            var pharmacyWarehouse = await _warehouseRepository.FirstOrDefaultAsync(x => x.Name == "Pharmacy Warehouse");
            if (pharmacyWarehouse != null)
            {
                input.FulfilledByWarehouseId = pharmacyWarehouse.Id;
            }
        }

        var requestNumber = $"REQ-{DateTime.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}";

        var entity = new InternalRequest(
            GuidGenerator.Create(), 
            requestNumber, 
            input.RequestingDepartmentId, 
            input.FulfilledByWarehouseId, 
            input.RequestDate)
        {
            AdmissionId = input.AdmissionId,
            Notes = input.Notes,
            Status = InternalRequestStatus.Draft
        };

        foreach (var line in input.Lines)
        {
            entity.Lines.Add(new InternalRequestLine(
                GuidGenerator.Create(),
                entity.Id,
                line.InventoryItemId,
                line.RequestedQuantity)
            {
                Notes = line.Notes
            });
        }

        await Repository.InsertAsync(entity);
        return await MapToGetOutputDtoAsync(entity);
    }

    public async Task<InternalRequestDto> SubmitRequestAsync(Guid id)
    {
        var entity = await Repository.GetAsync(id);
        
        if (entity.Status != InternalRequestStatus.Draft)
            throw new UserFriendlyException("Only draft requests can be submitted.");

        entity.Status = InternalRequestStatus.Submitted;
        await Repository.UpdateAsync(entity);

        return await MapToGetOutputDtoAsync(entity);
    }

    public async Task<InternalRequestDto> ApproveAndFulfillAsync(Guid id)
    {
        var query = await Repository.WithDetailsAsync(x => x.Lines);
        var entity = await AsyncExecuter.FirstOrDefaultAsync(query.Where(x => x.Id == id));

        if (entity == null) throw new Volo.Abp.Domain.Entities.EntityNotFoundException(typeof(InternalRequest), id);

        if (entity.Status != InternalRequestStatus.Submitted)
            throw new UserFriendlyException("Only submitted requests can be approved.");

        decimal totalChargeForPatient = 0m;
        var serviceItemRepo = LazyServiceProvider.LazyGetRequiredService<IRepository<HIS.Services.ServiceItem, Guid>>();
        var inventoryItemRepo = LazyServiceProvider.LazyGetRequiredService<IRepository<HIS.Inventory.InventoryItem, Guid>>();

        // For simplicity, we auto-approve the full requested quantity here.
        // In a real UI, the Store Manager would pass the exactly approved quantities.
        foreach (var line in entity.Lines)
        {
            line.ApprovedQuantity = line.RequestedQuantity; 

            // Issue stock out of the main store (FulfilledByWarehouseId)
            await _inventoryManager.IssueStockAsync(
                entity.FulfilledByWarehouseId,
                line.InventoryItemId,
                line.ApprovedQuantity,
                $"Approved Req: {entity.RequestNumber}",
                entity.RequestingDepartmentId // The department that takes the items
            );

            // Calculate selling price for patient if admission is linked
            if (entity.AdmissionId.HasValue)
            {
                var inventoryItem = await inventoryItemRepo.FindAsync(line.InventoryItemId);
                if (inventoryItem != null)
                {
                    var serviceItem = await serviceItemRepo.FindAsync(inventoryItem.ProductId);
                    if (serviceItem != null)
                    {
                        totalChargeForPatient += (serviceItem.Price * line.ApprovedQuantity);
                    }
                }
            }
        }

        if (entity.AdmissionId.HasValue && totalChargeForPatient > 0)
        {
            var admissionRepo = LazyServiceProvider.LazyGetRequiredService<IRepository<HIS.Inpatient.Admission, Guid>>();
            var admission = await admissionRepo.FindAsync(entity.AdmissionId.Value);
            if (admission != null)
            {
                admission.TotalAmount += totalChargeForPatient;
                await admissionRepo.UpdateAsync(admission);
            }
        }

        entity.Status = InternalRequestStatus.Approved;
        await Repository.UpdateAsync(entity);

        return await MapToGetOutputDtoAsync(entity);
    }

    public async Task<InternalRequestDto> ConfirmReceiptAsync(Guid id)
    {
        var query = await Repository.WithDetailsAsync(x => x.Lines);
        var entity = await AsyncExecuter.FirstOrDefaultAsync(query.Where(x => x.Id == id));

        if (entity == null) throw new Volo.Abp.Domain.Entities.EntityNotFoundException(typeof(InternalRequest), id);

        if (entity.Status != InternalRequestStatus.Approved)
            throw new UserFriendlyException("Only approved requests can be received by the department.");

        // Here we can either receive it into the generic department, or skip if the department doesn't track inventory strictly.
        // If the requesting department IS a pharmacy (Warehouse), we should run ReceiveStockAsync.
        // For now, logging it as "Received".
        
        entity.Status = InternalRequestStatus.Received;
        await Repository.UpdateAsync(entity);

        return await MapToGetOutputDtoAsync(entity);
    }
}
