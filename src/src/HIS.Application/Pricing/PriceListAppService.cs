using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace HIS.Pricing;

public class PriceListAppService : ApplicationService, IPriceListAppService
{
    private readonly IRepository<PriceList, Guid> _priceListRepository;
    private readonly IRepository<ServicePrice, Guid> _servicePriceRepository;
    private readonly IO<HIS.Services.ServiceItem, Guid> _serviceItemRepository; // For name lookup if needed, but we can do join query

    public PriceListAppService(
        IRepository<PriceList, Guid> priceListRepository,
        IRepository<ServicePrice, Guid> servicePriceRepository)
    {
        _priceListRepository = priceListRepository;
        _servicePriceRepository = servicePriceRepository;
    }

    public async Task<PagedResultDto<PriceListDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        var totalCount = await _priceListRepository.GetCountAsync();
        var items = await _priceListRepository.GetPagedListAsync(input.SkipCount, input.MaxResultCount, input.Sorting ?? nameof(PriceList.Name));

        return new PagedResultDto<PriceListDto>(
            totalCount,
            ObjectMapper.Map<List<PriceList>, List<PriceListDto>>(items)
        );
    }

    public async Task<PriceListDto> GetAsync(Guid id)
    {
        var item = await _priceListRepository.GetAsync(id);
        return ObjectMapper.Map<PriceList, PriceListDto>(item);
    }

    public async Task<PriceListDto> CreateAsync(CreateUpdatePriceListDto input)
    {
        var item = new PriceList(GuidGenerator.Create(), input.Name, input.IsDefault, input.EffectiveFrom, input.EffectiveTo);
        await _priceListRepository.InsertAsync(item);
        return ObjectMapper.Map<PriceList, PriceListDto>(item);
    }

    public async Task<PriceListDto> UpdateAsync(Guid id, CreateUpdatePriceListDto input)
    {
        var item = await _priceListRepository.GetAsync(id);
        item.Name = input.Name;
        item.IsDefault = input.IsDefault;
        item.EffectiveFrom = input.EffectiveFrom;
        item.EffectiveTo = input.EffectiveTo;
        
        await _priceListRepository.UpdateAsync(item);
        return ObjectMapper.Map<PriceList, PriceListDto>(item);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _priceListRepository.DeleteAsync(id);
    }

    // --- SERVICE PRICES ---

    public async Task<PagedResultDto<ServicePriceDto>> GetPricesAsync(Guid priceListId, PagedAndSortedResultRequestDto input)
    {
        var query = await _servicePriceRepository.GetQueryableAsync();
        query = query.Where(x => x.PriceListId == priceListId);

        var totalCount = await AsyncExecuter.CountAsync(query);
        
        // Paging
        query = query.PageBy(input);

        var items = await AsyncExecuter.ToListAsync(query);

        return new PagedResultDto<ServicePriceDto>(
            totalCount,
            ObjectMapper.Map<List<ServicePrice>, List<ServicePriceDto>>(items)
        );
    }

    public async Task<ServicePriceDto> SetPriceAsync(CreateUpdateServicePriceDto input)
    {
        // Check if price already exists for this service in this list
        var existing = await _servicePriceRepository.FirstOrDefaultAsync(x => x.PriceListId == input.PriceListId && x.ServiceItemId == input.ServiceItemId);

        if (existing != null)
        {
            existing.Amount = input.Amount;
            existing.CoPayAmount = input.CoPayAmount;
            await _servicePriceRepository.UpdateAsync(existing);
            return ObjectMapper.Map<ServicePrice, ServicePriceDto>(existing);
        }
        else
        {
            var newPrice = new ServicePrice(GuidGenerator.Create(), input.PriceListId, input.ServiceItemId, input.Amount, input.CoPayAmount);
            await _servicePriceRepository.InsertAsync(newPrice);
            return ObjectMapper.Map<ServicePrice, ServicePriceDto>(newPrice);
        }
    }
}
