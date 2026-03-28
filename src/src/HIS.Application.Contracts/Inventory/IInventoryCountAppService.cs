using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace HIS.Inventory;

public class InventoryCountDto : FullAuditedEntityDto<Guid>
{
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public DateTime CountDate { get; set; }
    public InventoryCountStatus Status { get; set; }
    public string? Notes { get; set; }
    public List<InventoryCountItemDto> Items { get; set; } = new();
}

public class InventoryCountItemDto : FullAuditedEntityDto<Guid>
{
    public Guid InventoryItemId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal SystemQuantity { get; set; }
    public decimal CountedQuantity { get; set; }
    public decimal Difference { get; set; }
    public string? Notes { get; set; }
}

public class CreateInventoryCountDto
{
    public Guid WarehouseId { get; set; }
    public DateTime CountDate { get; set; } = DateTime.Now;
    public string? Notes { get; set; }
}

public class UpdateInventoryCountItemDto
{
    public Guid Id { get; set; }
    public decimal CountedQuantity { get; set; }
    public string? Notes { get; set; }
}

public class GetInventoryCountsInput : PagedAndSortedResultRequestDto
{
    public Guid? WarehouseId { get; set; }
    public InventoryCountStatus? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public interface IInventoryCountAppService : IApplicationService
{
    Task<InventoryCountDto> CreateAsync(CreateInventoryCountDto input);
    Task<InventoryCountDto> GetAsync(Guid id);
    Task<PagedResultDto<InventoryCountDto>> GetListAsync(GetInventoryCountsInput input);
    Task UpdateItemAsync(Guid countId, UpdateInventoryCountItemDto input);
    Task FinalizeAsync(Guid id);
    Task CancelAsync(Guid id);
}
