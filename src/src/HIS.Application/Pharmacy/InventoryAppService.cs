using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HIS.Inventory;
using HIS.Inventory.Dtos;
using HIS.Pharmacy.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace HIS.Pharmacy;

public class InventoryAppService : HISAppService, IInventoryAppService
{
    private readonly IRepository<StockTransfer, Guid> _stockTransferRepository;
    private readonly IRepository<InventoryItem, Guid> _inventoryItemRepository;
    private readonly IRepository<Drug, Guid> _drugRepository;
    private readonly IRepository<Warehouse, Guid> _warehouseRepository;
    private readonly InventoryManager _inventoryManager;

    public InventoryAppService(
        IRepository<StockTransfer, Guid> stockTransferRepository,
        IRepository<InventoryItem, Guid> inventoryItemRepository,
        IRepository<Drug, Guid> drugRepository,
        IRepository<Warehouse, Guid> warehouseRepository,
        InventoryManager inventoryManager)
    {
        _stockTransferRepository = stockTransferRepository;
        _inventoryItemRepository = inventoryItemRepository;
        _drugRepository = drugRepository;
        _warehouseRepository = warehouseRepository;
        _inventoryManager = inventoryManager;
    }

    public async Task<PagedResultDto<StockTransferDto>> GetTransfersAsync(PagedAndSortedResultRequestDto input)
    {
        var count = await _stockTransferRepository.GetCountAsync();
        var list = await _stockTransferRepository.GetPagedListAsync(input.SkipCount, input.MaxResultCount, "CreationTime DESC");
        
        var dtos = ObjectMapper.Map<List<StockTransfer>, List<StockTransferDto>>(list);
        
        // Enrich DTOs with Warehouse Names (Optimization: Join or Batch Load)
        var warehouseIds = list.Select(x => x.FromWarehouseId).Concat(list.Select(x => x.ToWarehouseId)).Distinct().ToList();
        var warehouses = await _warehouseRepository.GetListAsync(x => warehouseIds.Contains(x.Id));
        
        foreach (var dto in dtos)
        {
            dto.FromWarehouseName = warehouses.FirstOrDefault(w => w.Id == dto.FromWarehouseId)?.Name;
            dto.ToWarehouseName = warehouses.FirstOrDefault(w => w.Id == dto.ToWarehouseId)?.Name;
        }

        return new PagedResultDto<StockTransferDto>(count, dtos);
    }

    public async Task<StockTransferDto> CreateTransferAsync(CreateStockTransferDto input)
    {
        var transfer = new StockTransfer(GuidGenerator.Create(), "TR-" + DateTime.Now.Ticks, input.FromWarehouseId, input.ToWarehouseId);
        transfer.Notes = input.Notes;
        
        foreach (var item in input.Items)
        {
            transfer.Items.Add(new StockTransferItem(GuidGenerator.Create(), transfer.Id, item.DrugId, item.Quantity, item.BatchNumber, item.ExpiryDate));
        }

        await _stockTransferRepository.InsertAsync(transfer);
        
        return ObjectMapper.Map<StockTransfer, StockTransferDto>(transfer);
    }

    public async Task ProcessTransferAsync(Guid id)
    {
        var transfer = await _stockTransferRepository.GetAsync(id);
        if (transfer.Status != TransferStatus.Draft) return;

        foreach (var item in transfer.Items)
        {
             var drug = await _drugRepository.GetAsync(item.DrugId);
             if (drug.ServiceItemId.HasValue)
             {
                 await _inventoryManager.TransferStockAsync(transfer.FromWarehouseId, transfer.ToWarehouseId, drug.ServiceItemId.Value, item.Quantity, transfer.TransferNumber);
             }
        }
        
        transfer.Status = TransferStatus.Received;
        transfer.TransferDate = DateTime.Now;
        await _stockTransferRepository.UpdateAsync(transfer);
    }

    public async Task<PagedResultDto<InventoryItemDto>> GetLowStockReportAsync(PagedAndSortedResultRequestDto input)
    {
        // Join InventoryItem with Drug to check levels
        // This requires complex query or fetching all.
        // For efficiency, we should query InventoryItems where Qty < Drug.MinLevel
        // But InventoryItem doesn't know about Drug.
        
        // Strategy: Get Drugs with MinLevel > 0
        var drugs = await _drugRepository.GetListAsync(x => x.MinimumStockLevel > 0);
        var serviceItemIds = drugs.Where(x => x.ServiceItemId.HasValue).Select(x => x.ServiceItemId.Value).ToList();
        
        // Get Inventory for these items        
        var mainWarehouse = await _warehouseRepository.FirstOrDefaultAsync(x => x.Name == "Main Warehouse" || x.Name == "المستودع الرئيسي");
        
        var pharmacy = await _warehouseRepository.FirstOrDefaultAsync(x => x.Name == "Pharmacy Warehouse" || x.Name == "مستودع الصيدلية");
        if (pharmacy == null) return new PagedResultDto<InventoryItemDto>();

        var inventoryItems = await _inventoryItemRepository.GetListAsync(x => x.WarehouseId == pharmacy.Id && serviceItemIds.Contains(x.ProductId));
        
        var lowStockItems = new List<InventoryItem>();
        
        foreach (var invItem in inventoryItems)
        {
            var drug = drugs.FirstOrDefault(d => d.ServiceItemId == invItem.ProductId);
            if (drug != null && invItem.Quantity < drug.MinimumStockLevel)
            {
                lowStockItems.Add(invItem);
            }
        }
        
        // Paging in memory for now (Optimization req for large data)
        var paged = lowStockItems.Skip(input.SkipCount).Take(input.MaxResultCount).ToList();
        
        return new PagedResultDto<InventoryItemDto>(lowStockItems.Count, ObjectMapper.Map<List<InventoryItem>, List<Inventory.Dtos.InventoryItemDto>>(paged));
    }
}
