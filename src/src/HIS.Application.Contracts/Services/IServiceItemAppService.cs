using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace HIS.Services;

public interface IServiceItemAppService : IApplicationService
{
    Task<PagedResultDto<ServiceItemDto>> GetListAsync(PagedAndSortedResultRequestDto input);
    Task<ServiceItemDto> GetAsync(Guid id);
    Task<ServiceItemDto> CreateAsync(CreateUpdateServiceItemDto input);
    Task<ServiceItemDto> UpdateAsync(Guid id, CreateUpdateServiceItemDto input);
    Task DeleteAsync(Guid id);
    
    // Radiology Specifics
    Task<PagedResultDto<RadiologyItemDto>> GetRadiologyListAsync(PagedAndSortedResultRequestDto input);
    Task<RadiologyItemDto> CreateRadiologyAsync(CreateUpdateRadiologyItemDto input);
    Task<RadiologyItemDto> UpdateRadiologyAsync(Guid id, CreateUpdateRadiologyItemDto input);
}
