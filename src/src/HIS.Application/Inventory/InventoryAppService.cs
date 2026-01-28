using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using HIS.Inventory.Dtos;

using Microsoft.AspNetCore.Authorization;
using HIS.Permissions;

namespace HIS.Inventory;

[Authorize(HISPermissions.Inventory.Default)]
[Route("api/app/inventory")]
public class InventoryAppService : ApplicationService, IInventoryAppService
{
    private readonly IRepository<Warehouse, Guid> _warehouseRepository;
    private readonly IRepository<InventoryItem, Guid> _inventoryItemRepository;
    private readonly IRepository<InventoryTransaction, Guid> _inventoryTransactionRepository;
    private readonly InventoryManager _inventoryManager;

    public InventoryAppService(
        IRepository<Warehouse, Guid> warehouseRepository,
        IRepository<InventoryItem, Guid> inventoryItemRepository,
        IRepository<InventoryTransaction, Guid> inventoryTransactionRepository,
        InventoryManager inventoryManager)
    {
        _warehouseRepository = warehouseRepository;
        _inventoryItemRepository = inventoryItemRepository;
        _inventoryTransactionRepository = inventoryTransactionRepository;
        _inventoryManager = inventoryManager;
    }

    // Warehouse CRUD
    [HttpGet("warehouse")]
    public async Task<PagedResultDto<WarehouseDto>> GetWarehouseListAsync(PagedAndSortedResultRequestDto input)
    {
        var count = await _warehouseRepository.GetCountAsync();
        var list = await _warehouseRepository.GetPagedListAsync(input.SkipCount, input.MaxResultCount, input.Sorting ?? nameof(Warehouse.Name));

        return new PagedResultDto<WarehouseDto>(
            count,
            ObjectMapper.Map<List<Warehouse>, List<WarehouseDto>>(list)
        );
    }

    [HttpPost("warehouse")]
    public async Task<WarehouseDto> CreateWarehouseAsync(CreateUpdateWarehouseDto input)
    {
        var warehouse = new Warehouse(GuidGenerator.Create(), input.Name, input.Location);
        await _warehouseRepository.InsertAsync(warehouse);
        return ObjectMapper.Map<Warehouse, WarehouseDto>(warehouse);
    }

    [HttpPut("warehouse/{id}")]
    public async Task<WarehouseDto> UpdateWarehouseAsync(Guid id, CreateUpdateWarehouseDto input)
    {
        var warehouse = await _warehouseRepository.GetAsync(id);
        warehouse.Name = input.Name;
        warehouse.Location = input.Location;
        await _warehouseRepository.UpdateAsync(warehouse);
        return ObjectMapper.Map<Warehouse, WarehouseDto>(warehouse);
    }

    [HttpDelete("warehouse/{id}")]
    public async Task DeleteWarehouseAsync(Guid id)
    {
        await _warehouseRepository.DeleteAsync(id);
    }

    // Stock Operations
    [HttpGet("stock-levels")]
    public async Task<PagedResultDto<InventoryItemDto>> GetStockLevelsAsync(Guid warehouseId)
    {
        var query = await _inventoryItemRepository.GetQueryableAsync();
        var items = query.Where(x => x.WarehouseId == warehouseId).ToList();
        
        // Note: Ideally join with Product/ServiceItems to get names
        // For now returning basic DTOs
        
        return new PagedResultDto<InventoryItemDto>(
            items.Count,
            ObjectMapper.Map<List<InventoryItem>, List<InventoryItemDto>>(items)
        );
    }

    [HttpPost("receive-stock")]
    public async Task ReceiveStockAsync(ReceiveStockDto input)
    {
        await _inventoryManager.ReceiveStockAsync(
            input.WarehouseId,
            input.ProductId,
            input.ProductName,
            input.Type,
            input.Quantity,
            input.UnitCost,
            input.ReferenceNumber
        );
    }

    [HttpPost("issue-stock")]
    public async Task IssueStockAsync(IssueStockDto input)
    {
        // TODO: Pass DepartmentId to Manager if needed for Accounting
        await _inventoryManager.IssueStockAsync(
            input.WarehouseId,
            input.ProductId,
            input.Quantity,
            input.ReferenceNumber
        );
    }

    [HttpGet("item-transactions/{inventoryItemId}")]
    public async Task<List<InventoryTransactionDto>> GetItemTransactionsAsync(Guid inventoryItemId)
    {
        var transactions = await _inventoryTransactionRepository.GetListAsync(x => x.InventoryItemId == inventoryItemId);
        return ObjectMapper.Map<List<InventoryTransaction>, List<InventoryTransactionDto>>(transactions.OrderByDescending(x => x.TransactionDate).ToList());
    }

    [HttpGet("item/{id}")]
    public async Task<InventoryItemDto> GetItemAsync(Guid id)
    {
        var item = await _inventoryItemRepository.GetAsync(id);
        return ObjectMapper.Map<InventoryItem, InventoryItemDto>(item);
    }
}
