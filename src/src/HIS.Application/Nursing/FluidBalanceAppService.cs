using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace HIS.Nursing;

[Authorize]
public class FluidBalanceAppService : HISAppService, IFluidBalanceAppService
{
    private readonly IRepository<FluidBalance, Guid> _fluidBalanceRepository;

    public FluidBalanceAppService(IRepository<FluidBalance, Guid> fluidBalanceRepository)
    {
        _fluidBalanceRepository = fluidBalanceRepository;
    }

    public async Task<PagedResultDto<FluidBalanceDto>> GetListAsync(Guid patientId)
    {
        var query = await _fluidBalanceRepository.GetQueryableAsync();
        var items = await AsyncExecuter.ToListAsync(
            query.Where(x => x.PatientId == patientId).OrderByDescending(x => x.EntryTime)
        );

        return new PagedResultDto<FluidBalanceDto>(
            items.Count,
            ObjectMapper.Map<List<FluidBalance>, List<FluidBalanceDto>>(items)
        );
    }

    public async Task<FluidBalanceDto> CreateAsync(CreateFluidBalanceDto input)
    {
        var entity = ObjectMapper.Map<CreateFluidBalanceDto, FluidBalance>(input);
        await _fluidBalanceRepository.InsertAsync(entity);
        return ObjectMapper.Map<FluidBalance, FluidBalanceDto>(entity);
    }

    public async Task<FluidBalanceSummaryDto> GetSummaryAsync(Guid patientId, DateTime date)
    {
        var query = await _fluidBalanceRepository.GetQueryableAsync();
        
        // Filter by date (ignoring time)
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);

        var records = await AsyncExecuter.ToListAsync(
            query.Where(x => x.PatientId == patientId && x.EntryTime >= startOfDay && x.EntryTime < endOfDay)
        );

        var totalInput = records.Where(x => x.Type == FluidType.Input).Sum(x => x.Amount);
        var totalOutput = records.Where(x => x.Type == FluidType.Output).Sum(x => x.Amount);

        return new FluidBalanceSummaryDto
        {
            TotalInput = totalInput,
            TotalOutput = totalOutput,
            Balance = totalInput - totalOutput
        };
    }
}
