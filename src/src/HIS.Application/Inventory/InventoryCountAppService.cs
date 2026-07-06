using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;

namespace HIS.Inventory;

public class InventoryCountAppService : ApplicationService, IInventoryCountAppService
{
    private readonly IRepository<InventoryCount, Guid> _countRepository;
    private readonly IRepository<InventoryCountItem, Guid> _countItemRepository;
    private readonly IRepository<InventoryItem, Guid> _inventoryItemRepository;
    private readonly IRepository<Warehouse, Guid> _warehouseRepository;
    private readonly InventoryManager _inventoryManager;

    public InventoryCountAppService(
        IRepository<InventoryCount, Guid> countRepository,
        IRepository<InventoryCountItem, Guid> countItemRepository,
        IRepository<InventoryItem, Guid> inventoryItemRepository,
        IRepository<Warehouse, Guid> warehouseRepository,
        InventoryManager inventoryManager)
    {
        _countRepository = countRepository;
        _countItemRepository = countItemRepository;
        _inventoryItemRepository = inventoryItemRepository;
        _warehouseRepository = warehouseRepository;
        _inventoryManager = inventoryManager;
    }

    public async Task<InventoryCountDto> CreateAsync(CreateInventoryCountDto input)
    {
        var warehouse = await _warehouseRepository.GetAsync(input.WarehouseId);
        var countId = GuidGenerator.Create();
        var inventoryCount = new InventoryCount(countId, input.WarehouseId, input.CountDate, CurrentTenant.Id)
        {
            Notes = input.Notes
        };

        // Snapshot all items in the warehouse
        var items = await _inventoryItemRepository.GetListAsync(x => x.WarehouseId == input.WarehouseId);
        foreach (var item in items)
        {
            inventoryCount.Items.Add(new InventoryCountItem(GuidGenerator.Create(), countId, item.Id, item.Quantity, CurrentTenant.Id));
        }

        await _countRepository.InsertAsync(inventoryCount);
        
        return await GetAsync(countId);
    }

    public async Task<InventoryCountDto> GetAsync(Guid id)
    {
        var queryable = await _countRepository.WithDetailsAsync(x => x.Items);
        var entity = await AsyncExecuter.FirstOrDefaultAsync(queryable.Where(x => x.Id == id));
        if (entity == null) throw new Volo.Abp.Domain.Entities.EntityNotFoundException(typeof(InventoryCount), id);

        var dto = ObjectMapper.Map<InventoryCount, InventoryCountDto>(entity);
        var warehouse = await _warehouseRepository.FindAsync(entity.WarehouseId);
        dto.WarehouseName = warehouse?.Name ?? "N/A";

        // Map item names
        foreach (var itemDto in dto.Items)
        {
            var invItem = await _inventoryItemRepository.FindAsync(itemDto.InventoryItemId);
            itemDto.ProductName = invItem?.ProductName ?? "Unknown";
        }

        return dto;
    }

    public async Task<PagedResultDto<InventoryCountDto>> GetListAsync(GetInventoryCountsInput input)
    {
        var queryable = await _countRepository.GetQueryableAsync();
        
        if (input.WarehouseId.HasValue)
            queryable = queryable.Where(x => x.WarehouseId == input.WarehouseId);
        
        if (input.Status.HasValue)
            queryable = queryable.Where(x => x.Status == input.Status);

        if (input.FromDate.HasValue)
            queryable = queryable.Where(x => x.CountDate >= input.FromDate.Value);

        if (input.ToDate.HasValue)
            queryable = queryable.Where(x => x.CountDate <= input.ToDate.Value);

        var totalCount = await AsyncExecuter.CountAsync(queryable);
        
        var items = await AsyncExecuter.ToListAsync(
            queryable.OrderByDescending(x => x.CountDate)
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
        );

        var dtos = ObjectMapper.Map<List<InventoryCount>, List<InventoryCountDto>>(items);
        
        if (dtos.Any())
        {
            var warehouseIds = dtos.Select(x => x.WarehouseId).Distinct().ToList();
            var warehouses = await _warehouseRepository.GetListAsync(x => warehouseIds.Contains(x.Id));
            var warehouseDict = warehouses.ToDictionary(x => x.Id, x => x.Name);
            
            foreach (var dto in dtos)
            {
                if (warehouseDict.TryGetValue(dto.WarehouseId, out var name))
                {
                    dto.WarehouseName = name;
                }
            }
        }

        return new PagedResultDto<InventoryCountDto>(totalCount, dtos);
    }

    public async Task UpdateItemAsync(Guid countId, UpdateInventoryCountItemDto input)
    {
        var count = await _countRepository.GetAsync(countId);
        if (count.Status != InventoryCountStatus.Draft)
            throw new Volo.Abp.BusinessException("Inventory:CannotUpdateCompletedCount");

        var item = await _countItemRepository.GetAsync(input.Id);
        item.CountedQuantity = input.CountedQuantity;
        item.Notes = input.Notes;
        
        await _countItemRepository.UpdateAsync(item);
    }

    public async Task FinalizeAsync(Guid id)
    {
        var queryable = await _countRepository.WithDetailsAsync(x => x.Items);
        var entity = await AsyncExecuter.FirstOrDefaultAsync(queryable.Where(x => x.Id == id));
        if (entity == null) throw new Volo.Abp.Domain.Entities.EntityNotFoundException(typeof(InventoryCount), id);

        if (entity.Status != InventoryCountStatus.Draft)
            throw new Volo.Abp.BusinessException("Inventory:CountAlreadyFinalized");

        foreach (var item in entity.Items)
        {
            if (item.Difference == 0) continue;

            var invItem = await _inventoryItemRepository.GetAsync(item.InventoryItemId);
            
            if (item.Difference > 0) // Surplus
            {
                await _inventoryManager.ReceiveStockAsync(
                    invItem.WarehouseId, 
                    invItem.ProductId, 
                    invItem.ProductName, 
                    invItem.Type, 
                    item.Difference, 
                    invItem.AverageCost, 
                    $"Inventory Count Ref: {entity.Id.ToString().Substring(0, 8)}"
                );
            }
            else // Deficit (Difference is negative)
            {
                await _inventoryManager.IssueStockAsync(
                    invItem.WarehouseId, 
                    invItem.ProductId, 
                    Math.Abs(item.Difference), 
                    $"Inventory Count Ref: {entity.Id.ToString().Substring(0, 8)}",
                    force: true
                );
            }
        }

        entity.Status = InventoryCountStatus.Completed;
        await _countRepository.UpdateAsync(entity);
    }

    public async Task CancelAsync(Guid id)
    {
        var entity = await _countRepository.GetAsync(id);
        if (entity.Status != InventoryCountStatus.Draft)
            throw new Volo.Abp.BusinessException("Inventory:CannotCancelCompletedCount");

        entity.Status = InventoryCountStatus.Canceled;
        await _countRepository.UpdateAsync(entity);
    }
}
