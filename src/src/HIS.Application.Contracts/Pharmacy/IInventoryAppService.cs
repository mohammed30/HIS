using System;
using System.Threading.Tasks;
using HIS.Pharmacy.Dtos;
using HIS.Inventory.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace HIS.Pharmacy;

public interface IInventoryAppService : IApplicationService
{
    Task<PagedResultDto<StockTransferDto>> GetTransfersAsync(PagedAndSortedResultRequestDto input);
    Task<StockTransferDto> CreateTransferAsync(CreateStockTransferDto input);
    Task ProcessTransferAsync(Guid id); // Approve/Complete
    Task<PagedResultDto<InventoryItemDto>> GetLowStockReportAsync(PagedAndSortedResultRequestDto input);
}
