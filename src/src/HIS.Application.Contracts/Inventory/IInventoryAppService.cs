using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using HIS.Inventory.Dtos;

namespace HIS.Inventory;

public interface IInventoryAppService : IApplicationService
{
    // Warehouse Management
    Task<PagedResultDto<WarehouseDto>> GetWarehouseListAsync(PagedAndSortedResultRequestDto input);
    Task<WarehouseDto> CreateWarehouseAsync(CreateUpdateWarehouseDto input);
    Task<WarehouseDto> UpdateWarehouseAsync(Guid id, CreateUpdateWarehouseDto input);
    Task DeleteWarehouseAsync(Guid id);

    // Stock Management
    Task<PagedResultDto<InventoryItemDto>> GetStockLevelsAsync(Guid warehouseId);
    Task ReceiveStockAsync(ReceiveStockDto input);
    Task IssueStockAsync(IssueStockDto input);
    Task<List<DepartmentConsumptionReportDto>> GetConsumptionReportAsync(GetConsumptionReportInput input);
}
