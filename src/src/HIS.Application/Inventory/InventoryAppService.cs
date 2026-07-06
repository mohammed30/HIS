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
using QuestPDF.Fluent;
using HIS.Inventory.Printing;
using HIS.Pharmacy;
using HIS.Services;

namespace HIS.Inventory;

[Authorize(HISPermissions.Inventory.Default)]
[Route("api/app/inventory")]
public class InventoryAppService : ApplicationService, IInventoryAppService
{
    private readonly IRepository<Warehouse, Guid> _warehouseRepository;
    private readonly IRepository<InventoryItem, Guid> _inventoryItemRepository;
    private readonly IRepository<InventoryTransaction, Guid> _inventoryTransactionRepository;
    private readonly IRepository<Department, Guid> _departmentRepository;
    private readonly IRepository<Drug, Guid> _drugRepository;
    private readonly IRepository<ServiceItem, Guid> _serviceItemRepository;
    private readonly InventoryManager _inventoryManager;

    public InventoryAppService(
        IRepository<Warehouse, Guid> warehouseRepository,
        IRepository<InventoryItem, Guid> inventoryItemRepository,
        IRepository<InventoryTransaction, Guid> inventoryTransactionRepository,
        IRepository<Department, Guid> departmentRepository,
        IRepository<Drug, Guid> drugRepository,
        IRepository<ServiceItem, Guid> serviceItemRepository,
        InventoryManager inventoryManager)
    {
        _warehouseRepository = warehouseRepository;
        _inventoryItemRepository = inventoryItemRepository;
        _inventoryTransactionRepository = inventoryTransactionRepository;
        _departmentRepository = departmentRepository;
        _drugRepository = drugRepository;
        _serviceItemRepository = serviceItemRepository;
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

    [HttpGet("warehouse-lookup")]
    public async Task<List<HIS.Appointments.Dtos.LookupDto<Guid>>> GetWarehouseLookupAsync()
    {
        var list = await _warehouseRepository.GetListAsync();
        return list.Select(x => new HIS.Appointments.Dtos.LookupDto<Guid> { Id = x.Id, Name = x.Name }).ToList();
    }

    [HttpPost("warehouse")]
    [Authorize(HISPermissions.Inventory.ManageWarehouses)]
    public async Task<WarehouseDto> CreateWarehouseAsync(CreateUpdateWarehouseDto input)
    {
        var code = "WH-" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
        var warehouse = new Warehouse(GuidGenerator.Create(), input.Name, input.Location, code);
        await _warehouseRepository.InsertAsync(warehouse);

        var accountRepository = LazyServiceProvider.LazyGetRequiredService<IRepository<HIS.Accounting.Account, Guid>>();
        var inventoryAccount = await accountRepository.FirstOrDefaultAsync(x => x.Code == "1130");
        if (inventoryAccount != null)
        {
            var whAccount = await accountRepository.FirstOrDefaultAsync(x => x.ParentId == inventoryAccount.Id && x.NameAr == input.Name);
            if (whAccount == null)
            {
                var codeSuffix = input.Name.Replace(" ", "");
                if (codeSuffix.Length > 3) codeSuffix = codeSuffix.Substring(0, 3);
                var newAccount = new HIS.Accounting.Account(
                    GuidGenerator.Create(), 
                    inventoryAccount.Code + "-" + codeSuffix, 
                    input.Name, 
                    input.Name, 
                    inventoryAccount.Type, 
                    inventoryAccount.Id
                );
                await accountRepository.InsertAsync(newAccount);
            }
        }

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
    public async Task<PagedResultDto<InventoryItemDto>> GetStockLevelsAsync(Guid warehouseId, string? filter = null, InventoryItemType? type = null)
    {
        var query = await _inventoryItemRepository.GetQueryableAsync();
        var q = query.Where(x => x.WarehouseId == warehouseId);

        if (type.HasValue)
        {
            q = q.Where(x => x.Type == type.Value);
        }

        var items = await AsyncExecuter.ToListAsync(q);

        var drugIds = items.Where(x => x.Type == InventoryItemType.Medication).Select(x => x.ProductId).Distinct().ToList();
        var serviceItemIds = items.Where(x => x.Type != InventoryItemType.Medication).Select(x => x.ProductId).Distinct().ToList();

        var drugs = drugIds.Any() ? await _drugRepository.GetListAsync(x => x.ServiceItemId.HasValue && drugIds.Contains(x.ServiceItemId.Value)) : new List<Drug>();
        var serviceItems = serviceItemIds.Any() ? await _serviceItemRepository.GetListAsync(x => serviceItemIds.Contains(x.Id)) : new List<ServiceItem>();

        var dtos = ObjectMapper.Map<List<InventoryItem>, List<InventoryItemDto>>(items);

        foreach (var dto in dtos)
        {
            if (dto.Type == InventoryItemType.Medication)
            {
                var drug = drugs.FirstOrDefault(d => d.ServiceItemId == dto.ProductId);
                dto.ProductCode = drug?.Barcode ?? $"MED-{dto.ProductId.ToString().Substring(0,4).ToUpper()}";
                dto.Barcode = dto.ProductCode;
            }
            else
            {
                var svc = serviceItems.FirstOrDefault(s => s.Id == dto.ProductId);
                dto.ProductCode = svc?.Code ?? $"ITM-{dto.ProductId.ToString().Substring(0,4).ToUpper()}";
                dto.Barcode = dto.ProductCode;
            }
        }

        if (!string.IsNullOrWhiteSpace(filter))
        {
            var lowerFilter = filter.ToLower();
            dtos = dtos.Where(x => 
                (x.ProductName != null && x.ProductName.ToLower().Contains(lowerFilter)) || 
                (x.ProductCode != null && x.ProductCode.ToLower().Contains(lowerFilter))
            ).ToList();
        }

        return new PagedResultDto<InventoryItemDto>(
            dtos.Count,
            dtos
        );
    }

    [HttpPut("stock-levels/{id}")]
    [Authorize(HISPermissions.Inventory.ManageWarehouses)] // Or specific permission
    public async Task UpdateStockLevelsAsync(Guid id, UpdateStockLevelsDto input)
    {
        var item = await _inventoryItemRepository.GetAsync(id);
        item.MinStockLevel = input.MinStockLevel;
        item.ReorderLevel = input.ReorderLevel;
        await _inventoryItemRepository.UpdateAsync(item);
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
    [HttpGet("reports/consumption")]
    public async Task<List<DepartmentConsumptionReportDto>> GetConsumptionReportAsync(GetConsumptionReportInput input)
    {
        var startDate = input.StartDate ?? DateTime.Now.AddMonths(-1);
        var endDate = input.EndDate ?? DateTime.Now;

        var query = await _inventoryTransactionRepository.GetQueryableAsync();
        var itemQuery = await _inventoryItemRepository.GetQueryableAsync();
        
        // Filter transactions
        var transactions = query.Where(x => 
            x.TransactionType == TransactionType.Issue &&
            x.TransactionDate >= startDate &&
            x.TransactionDate <= endDate &&
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

    [HttpGet("reports/consumption/pdf")]
    public async Task<byte[]> GetConsumptionReportPdfAsync(GetConsumptionReportInput input)
    {
        var data = await GetConsumptionReportAsync(input);
        var document = new ConsumptionReportDocument
        {
            Items = data,
            StartDate = input.StartDate ?? DateTime.Now.AddMonths(-1),
            EndDate = input.EndDate ?? DateTime.Now
        };
        return document.GeneratePdf();
    }

    [HttpGet("reports/low-stock")]
    public async Task<List<LowStockReportDto>> GetLowStockReportAsync(GetLowStockReportInput input)
    {
        var itemQuery = await _inventoryItemRepository.GetQueryableAsync();
        var warehouseQuery = await _warehouseRepository.GetQueryableAsync();

        var query = from item in itemQuery
                    join warehouse in warehouseQuery on item.WarehouseId equals warehouse.Id
                    where item.Quantity <= item.MinStockLevel 
                          && item.MinStockLevel > 0 // Only show items that actually have a min stock set
                          && (input.WarehouseId == null || item.WarehouseId == input.WarehouseId)
                    select new LowStockReportDto
                    {
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        WarehouseName = warehouse.Name,
                        CurrentQuantity = item.Quantity,
                        MinStockLevel = item.MinStockLevel
                    };

        var result = await AsyncExecuter.ToListAsync(query);
        return result.OrderByDescending(x => x.Deficit).ToList();
    }

    [HttpGet("reports/low-stock/pdf")]
    public async Task<byte[]> GetLowStockReportPdfAsync(GetLowStockReportInput input)
    {
        var data = await GetLowStockReportAsync(input);
        string warehouseName = "الكل";
        if (input.WarehouseId.HasValue)
        {
            var warehouse = await _warehouseRepository.FindAsync(input.WarehouseId.Value);
            warehouseName = warehouse?.Name ?? "غير معروف";
        }

        var document = new LowStockReportDocument
        {
            Items = data,
            WarehouseName = warehouseName
        };
        return document.GeneratePdf();
    }

    [HttpGet("reports/stagnant-stock")]
    public async Task<List<StagnantStockReportDto>> GetStagnantStockReportAsync(GetStagnantStockReportInput input)
    {
        var thresholdDate = DateTime.Now.AddDays(-input.ThresholdDays);
        
        var itemQuery = await _inventoryItemRepository.GetQueryableAsync();
        var warehouseQuery = await _warehouseRepository.GetQueryableAsync();
        var transactionQuery = await _inventoryTransactionRepository.GetQueryableAsync();

        // Items with positive quantity
        var availableItems = itemQuery.Where(x => x.Quantity > 0 && (input.WarehouseId == null || x.WarehouseId == input.WarehouseId));

        var list = new List<StagnantStockReportDto>();

        var items = await AsyncExecuter.ToListAsync(
            from item in availableItems
            join warehouse in warehouseQuery on item.WarehouseId equals warehouse.Id
            select new { item, warehouseName = warehouse.Name }
        );

        foreach (var i in items)
        {
            var lastTxDate = await AsyncExecuter.MaxAsync(
                transactionQuery.Where(tx => tx.InventoryItemId == i.item.Id && (tx.TransactionType == TransactionType.Dispensing || tx.TransactionType == TransactionType.Issue)),
                tx => (DateTime?)tx.TransactionDate
            );

            // If no outbound transactions ever, consider the creation date or beginning of time.
            // For simplicity, we could say if lastTxDate is null, it's stagnant since it was received.
            // Let's get the latest Receive date:
            if (lastTxDate == null) 
            {
               lastTxDate = await AsyncExecuter.MaxAsync(
                   transactionQuery.Where(tx => tx.InventoryItemId == i.item.Id && tx.TransactionType == TransactionType.Receipt),
                   tx => (DateTime?)tx.TransactionDate
               );
            }

            if (lastTxDate == null || lastTxDate < thresholdDate)
            {
                var days = lastTxDate.HasValue ? (int)(DateTime.Now - lastTxDate.Value).TotalDays : input.ThresholdDays; // Default to threshold if absolutely no transactions
                list.Add(new StagnantStockReportDto
                {
                    ProductId = i.item.ProductId,
                    ProductName = i.item.ProductName,
                    WarehouseName = i.warehouseName,
                    CurrentQuantity = i.item.Quantity,
                    LastTransactionDate = lastTxDate,
                    DaysStagnant = days
                });
            }
        }

        return list.OrderByDescending(x => x.DaysStagnant).ToList();
    }

    [HttpGet("reports/stagnant-stock/pdf")]
    public async Task<byte[]> GetStagnantStockReportPdfAsync(GetStagnantStockReportInput input)
    {
        var data = await GetStagnantStockReportAsync(input);
        string warehouseName = "الكل";
        if (input.WarehouseId.HasValue)
        {
            var warehouse = await _warehouseRepository.FindAsync(input.WarehouseId.Value);
            warehouseName = warehouse?.Name ?? "غير معروف";
        }

        var document = new StagnantStockReportDocument
        {
            Items = data,
            WarehouseName = warehouseName,
            ThresholdDays = input.ThresholdDays
        };
        return document.GeneratePdf();
    }
}
