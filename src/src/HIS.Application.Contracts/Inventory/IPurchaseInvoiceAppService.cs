using System;
using System.Threading.Tasks;
using HIS.Inventory.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace HIS.Inventory;

public interface IPurchaseInvoiceAppService : ICrudAppService<
    PurchaseInvoiceDto, 
    Guid, 
    PagedAndSortedResultRequestDto, 
    CreateUpdatePurchaseInvoiceDto>
{
    Task PostInvoiceAsync(Guid id, Guid warehouseId);
}
