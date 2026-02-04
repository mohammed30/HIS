using System;
using System.Threading.Tasks;
using HIS.Emergency.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace HIS.Emergency;

public interface IEmergencyAppService : IApplicationService
{
    Task<PagedResultDto<EmergencyVisitDto>> GetActiveVisitsAsync(PagedAndSortedResultRequestDto input);
    Task<EmergencyVisitDto> RegisterAsync(CreateEmergencyVisitDto input);
    Task<EmergencyVisitDto> PerformTriageAsync(Guid id, TriageDto input);
    Task<EmergencyVisitDto> UpdateStatusAsync(Guid id, UpdateStatusDto input);
}
