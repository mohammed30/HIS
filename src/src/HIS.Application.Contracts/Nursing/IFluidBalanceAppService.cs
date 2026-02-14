using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace HIS.Nursing;

public interface IFluidBalanceAppService : IApplicationService
{
    Task<PagedResultDto<FluidBalanceDto>> GetListAsync(Guid patientId);
    Task<FluidBalanceDto> CreateAsync(CreateFluidBalanceDto input);
    
    // Calculation
    Task<FluidBalanceSummaryDto> GetSummaryAsync(Guid patientId, DateTime date);
}

public class FluidBalanceSummaryDto
{
    public double TotalInput { get; set; }
    public double TotalOutput { get; set; }
    public double Balance { get; set; }
}
