using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using HIS.Inventory.Dtos;
using HIS.Settings;
using System.Linq;

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
    private readonly IRepository<Department, Guid> _departmentRepository;
    private readonly InventoryManager _inventoryManager;

    public InventoryAppService(
        IRepository<Warehouse, Guid> warehouseRepository,
        IRepository<InventoryItem, Guid> inventoryItemRepository,
        IRepository<InventoryTransaction, Guid> inventoryTransactionRepository,
        IRepository<Department, Guid> departmentRepository,
        InventoryManager inventoryManager)
    {
        _warehouseRepository = warehouseRepository;
        _inventoryItemRepository = inventoryItemRepository;
        _inventoryTransactionRepository = inventoryTransactionRepository;
        _departmentRepository = departmentRepository;
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
    [Authorize(HISPermissions.Inventory.ManageWarehouses)]
    public async Task<WarehouseDto> CreateWarehouseAsync(CreateUpdateWarehouseDto input)
    {
        var code = "WH-" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
        var warehouse = new Warehouse(GuidGenerator.Create(), input.Name, input.Location, code);
        await _warehouseRepository.InsertAsync(warehouse);
        return ObjectMapper.Map<Warehouse, WarehouseDto>(warehouse);
    }

    [HttpPut("warehouse/{id}")]
    [Authorize(HISPermissions.Inventory.ManageWarehouses)]
    public async Task<WarehouseDto> UpdateWarehouseAsync(Guid id, CreateUpdateWarehouseDto input)
    {
        var warehouse = await _warehouseRepository.GetAsync(id);
        warehouse.Name = input.Name;
        warehouse.Location = input.Location;
        await _warehouseRepository.UpdateAsync(warehouse);
        return ObjectMapper.Map<Warehouse, WarehouseDto>(warehouse);
    }

    [HttpDelete("warehouse/{id}")]
    [Authorize(HISPermissions.Inventory.ManageWarehouses)]
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
    [Authorize(HISPermissions.Inventory.StockOperations)]
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
    [Authorize(HISPermissions.Inventory.StockOperations)]
    public async Task IssueStockAsync(IssueStockDto input)
    {
        await _inventoryManager.IssueStockAsync(
            input.WarehouseId,
            input.ProductId,
            input.Quantity,
            input.ReferenceNumber,
            input.DepartmentId
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
    [HttpGet("consumption-report")]
    public async Task<List<DepartmentConsumptionReportDto>> GetConsumptionReportAsync(GetConsumptionReportInput input)
    {
        var query = await _inventoryTransactionRepository.GetQueryableAsync();
        var itemQuery = await _inventoryItemRepository.GetQueryableAsync();
        
        // Filter transactions
        var transactions = query.Where(x => 
            x.TransactionType == TransactionType.Issue &&
            x.TransactionDate >= input.StartDate &&
            x.TransactionDate <= input.EndDate &&
            (input.DepartmentId == null || x.DepartmentId == input.DepartmentId)
        );

        // Join with Items
        var joined = from t in transactions
                     join i in itemQuery on t.InventoryItemId equals i.Id
                     select new { t, i };

        var list = await AsyncExecuter.ToListAsync(joined);

        // Group and Project (Doing in memory for now due to complexity of aggregates with Department name lookup)
        // Optimization: Resolve department names separately
        
        var grouped = list
            .GroupBy(x => new { x.t.DepartmentId, x.i.ProductId, x.i.ProductName })
            .Select(g => new DepartmentConsumptionReportDto
            {
                DepartmentId = g.Key.DepartmentId ?? Guid.Empty,
                DepartmentName = "", // To be filled
                ProductId = g.Key.ProductId,
                ProductName = g.Key.ProductName,
                Quantity = g.Sum(x => x.t.Quantity),
                TotalCost = g.Sum(x => x.t.Quantity * x.t.UnitCost)
            })
            .Where(x => x.DepartmentId != Guid.Empty)
            .ToList();

        // Fill Department Names
        var departmentIds = grouped.Select(x => x.DepartmentId).Distinct().ToList();
        var departments = await _departmentRepository.GetListAsync(x => departmentIds.Contains(x.Id));
        var deptMap = departments.ToDictionary(x => x.Id, x => x.NameAr ?? x.NameEn);

        foreach (var item in grouped)
        {
             if (deptMap.TryGetValue(item.DepartmentId, out var name))
             {
                 item.DepartmentName = name;
             }
        }
        
        return grouped;
    }
}
