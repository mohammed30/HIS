using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using HIS.Inventory.Dtos;

namespace HIS.Inventory;

[Authorize]
public class PurchaseOrderAppService : CrudAppService<PurchaseOrder, PurchaseOrderDto, Guid, PagedAndSortedResultRequestDto, CreateUpdatePurchaseOrderDto>, IPurchaseOrderAppService
{
    private readonly IRepository<Supplier, Guid> _supplierRepository;
    private readonly IRepository<InventoryItem, Guid> _inventoryItemRepository;

    public PurchaseOrderAppService(
        IRepository<PurchaseOrder, Guid> repository,
        IRepository<Supplier, Guid> supplierRepository,
        IRepository<InventoryItem, Guid> inventoryItemRepository) 
        : base(repository)
    {
        _supplierRepository = supplierRepository;
        _inventoryItemRepository = inventoryItemRepository;
    }


    public override async Task<PurchaseOrderDto> CreateAsync(CreateUpdatePurchaseOrderDto input)
    {
        var orderNumber = $"PO-{DateTime.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}";
        
        var entity = new PurchaseOrder(GuidGenerator.Create(), orderNumber, input.SupplierId, input.OrderDate)
        {
            ExpectedDeliveryDate = input.ExpectedDeliveryDate,
            ReferenceNumber = input.ReferenceNumber,
            Notes = input.Notes,
            Status = PurchaseOrderStatus.Draft
        };

        decimal totalAmount = 0;
        foreach (var lineDto in input.PurchaseOrderLines)
        {
            var lineTotal = (lineDto.Quantity * lineDto.UnitPrice) - lineDto.Discount;
            totalAmount += lineTotal;
            
            entity.PurchaseOrderLines.Add(new PurchaseOrderLine(
                GuidGenerator.Create(),
                entity.Id,
                lineDto.ProductId,
                lineDto.Quantity,
                lineDto.UnitPrice
            )
            {
                Discount = lineDto.Discount,
                TotalAmount = lineTotal,
                Description = lineDto.Description
            });
        }
        
        entity.TotalAmount = totalAmount;

        await Repository.InsertAsync(entity);
        
        return await MapToGetOutputDtoAsync(entity);
    }

    public override async Task<PurchaseOrderDto> UpdateAsync(Guid id, CreateUpdatePurchaseOrderDto input)
    {
        var entity = await Repository.GetAsync(id);
        
        if (entity.Status != PurchaseOrderStatus.Draft)
        {
            throw new Volo.Abp.UserFriendlyException("Cannot update a non-draft purchase order.");
        }

        entity.SupplierId = input.SupplierId;
        entity.OrderDate = input.OrderDate;
        entity.ExpectedDeliveryDate = input.ExpectedDeliveryDate;
        entity.ReferenceNumber = input.ReferenceNumber;
        entity.Notes = input.Notes;
        
        // Naive update: Clear lines and re-add (for simplicity in this iteration)
        // In production, we should reconcile lines.
        entity.PurchaseOrderLines.Clear();
        
        decimal totalAmount = 0;
        foreach (var lineDto in input.PurchaseOrderLines)
        {
            var lineTotal = (lineDto.Quantity * lineDto.UnitPrice) - lineDto.Discount;
            totalAmount += lineTotal;

            entity.PurchaseOrderLines.Add(new PurchaseOrderLine(
                GuidGenerator.Create(),
                entity.Id,
                lineDto.ProductId,
                lineDto.Quantity,
                lineDto.UnitPrice
            )
            {
                Discount = lineDto.Discount,
                TotalAmount = lineTotal,
                Description = lineDto.Description
            });
        }
        entity.TotalAmount = totalAmount;

        await Repository.UpdateAsync(entity);
        
        return await MapToGetOutputDtoAsync(entity);
    }

    public override async Task<PurchaseOrderDto> GetAsync(Guid id)
    {
        var query = await Repository.WithDetailsAsync(x => x.PurchaseOrderLines);
        var entity = await AsyncExecuter.FirstOrDefaultAsync(query.Where(x => x.Id == id));
        
        if (entity == null) throw new Volo.Abp.Domain.Entities.EntityNotFoundException(typeof(PurchaseOrder), id);

        return await MapToGetOutputDtoAsync(entity);
    }

    public async Task<PurchaseOrderDto> ConfirmOrderAsync(Guid id)
    {
        var entity = await Repository.GetAsync(id);
        if (entity.Status != PurchaseOrderStatus.Draft)
        {
            throw new Volo.Abp.UserFriendlyException("Only draft orders can be confirmed.");
        }
        
        entity.Status = PurchaseOrderStatus.Confirmed;
        await Repository.UpdateAsync(entity);
        
        return await MapToGetOutputDtoAsync(entity);
    }

    public async Task<PurchaseOrderDto> CancelOrderAsync(Guid id)
    {
        var entity = await Repository.GetAsync(id);
        if (entity.Status == PurchaseOrderStatus.Received)
        {
             throw new Volo.Abp.UserFriendlyException("Cannot cancel a received order.");
        }
        
        entity.Status = PurchaseOrderStatus.Cancelled;
        await Repository.UpdateAsync(entity);
        
        return await MapToGetOutputDtoAsync(entity);
    }
}
