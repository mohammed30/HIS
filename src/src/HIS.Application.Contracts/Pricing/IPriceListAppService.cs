using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace HIS.Pricing;

public interface IPriceListAppService : IApplicationService
{
    Task<PagedResultDto<PriceListDto>> GetListAsync(PagedAndSortedResultRequestDto input);
    Task<PriceListDto> GetAsync(Guid id);
    Task<PriceListDto> CreateAsync(CreateUpdatePriceListDto input);
    Task<PriceListDto> UpdateAsync(Guid id, CreateUpdatePriceListDto input);
    Task DeleteAsync(Guid id);

    // Pricing Items
    Task<PagedResultDto<ServicePriceDto>> GetPricesAsync(Guid priceListId, PagedAndSortedResultRequestDto input);
    Task<ServicePriceDto> SetPriceAsync(CreateUpdateServicePriceDto input);
}
