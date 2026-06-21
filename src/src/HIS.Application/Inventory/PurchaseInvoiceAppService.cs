using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HIS.Inventory.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace HIS.Inventory;

public class PurchaseInvoiceAppService : CrudAppService<
    PurchaseInvoice, 
    PurchaseInvoiceDto, 
    Guid, 
    PagedAndSortedResultRequestDto, 
    CreateUpdatePurchaseInvoiceDto>,
    IPurchaseInvoiceAppService
{
    private readonly IRepository<PurchaseOrder, Guid> _purchaseOrderRepository;
    private readonly IRepository<Supplier, Guid> _supplierRepository;
    private readonly IRepository<HIS.Services.ServiceItem, Guid> _serviceItemRepository;
    private readonly IRepository<InventoryItem, Guid> _inventoryItemRepository;

    public PurchaseInvoiceAppService(
        IRepository<PurchaseInvoice, Guid> repository,
        IRepository<PurchaseOrder, Guid> purchaseOrderRepository,
        IRepository<Supplier, Guid> supplierRepository,
        IRepository<InventoryItem, Guid> inventoryItemRepository,
        IRepository<HIS.Services.ServiceItem, Guid> serviceItemRepository) 
        : base(repository)
    {
        _purchaseOrderRepository = purchaseOrderRepository;
        _supplierRepository = supplierRepository;
        _inventoryItemRepository = inventoryItemRepository;
        _serviceItemRepository = serviceItemRepository;
    }

    public override async Task<PurchaseInvoiceDto> GetAsync(Guid id)
    {
        var entity = await Repository.WithDetailsAsync(x => x.Lines);
        var invoice = await AsyncExecuter.FirstOrDefaultAsync(entity.Where(x => x.Id == id));
        
        if (invoice == null) throw new Volo.Abp.Domain.Entities.EntityNotFoundException(typeof(PurchaseInvoice), id);

        var dto = await MapToGetOutputDtoAsync(invoice);
        
        var supplier = await _supplierRepository.FindAsync(invoice.SupplierId);
        dto.SupplierName = supplier?.Name;

        if (invoice.PurchaseOrderId.HasValue)
        {
            var po = await _purchaseOrderRepository.FindAsync(invoice.PurchaseOrderId.Value);
            dto.PurchaseOrderNumber = po?.OrderNumber;
        }

        foreach (var line in dto.Lines)
        {
            var item = await _inventoryItemRepository.FindAsync(line.ProductId);
            line.ProductName = item?.ProductName;
        }

        return dto;
    }

    public async Task PostInvoiceAsync(Guid id, Guid warehouseId)
    {
        var entity = await Repository.WithDetailsAsync(x => x.Lines);
        var invoice = await AsyncExecuter.FirstOrDefaultAsync(entity.Where(x => x.Id == id));

        if (invoice == null) throw new Volo.Abp.Domain.Entities.EntityNotFoundException(typeof(PurchaseInvoice), id);
        if (invoice.Status != PurchaseInvoiceStatus.Draft)
        {
            throw new Volo.Abp.UserFriendlyException("Only draft invoices can be posted.");
        }

        var inventoryManager = LazyServiceProvider.LazyGetRequiredService<InventoryManager>();

        foreach (var line in invoice.Lines)
        {
            await inventoryManager.ReceiveStockAsync(
                warehouseId,
                line.ProductId,
                null, 
                InventoryItemType.Medication, // TODO: Logic to determine if it's consumable or medication
                line.Quantity,
                line.UnitCost,
                invoice.InvoiceNumber,
                line.BatchNumber,
                line.ExpiryDate,
                invoice.SupplierId
            );
        }

        invoice.Status = PurchaseInvoiceStatus.Posted;
        await Repository.UpdateAsync(invoice);

        // Update Sale Prices in ServiceItem
        var drugRepo = LazyServiceProvider.LazyGetRequiredService<IRepository<HIS.Pharmacy.Drug, Guid>>();
        foreach (var line in invoice.Lines)
        {
            if (line.SalePrice > 0)
            {
                var drug = await drugRepo.FirstOrDefaultAsync(x => x.Id == line.ProductId);
                if (drug != null && drug.ServiceItemId.HasValue)
                {
                    var serviceItem = await _serviceItemRepository.FirstOrDefaultAsync(x => x.Id == drug.ServiceItemId.Value);
                    if (serviceItem != null)
                    {
                        serviceItem.Price = line.SalePrice;
                        await _serviceItemRepository.UpdateAsync(serviceItem);
                    }
                }
            }
        }
    }
}
