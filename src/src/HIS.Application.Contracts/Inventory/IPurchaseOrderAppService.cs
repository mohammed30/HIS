using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using HIS.Inventory.Dtos;

namespace HIS.Inventory;

public interface IPurchaseOrderAppService : ICrudAppService<PurchaseOrderDto, Guid, PagedAndSortedResultRequestDto, CreateUpdatePurchaseOrderDto>
{
    Task<PurchaseOrderDto> ConfirmOrderAsync(Guid id);
    Task<PurchaseOrderDto> CancelOrderAsync(Guid id);
}
