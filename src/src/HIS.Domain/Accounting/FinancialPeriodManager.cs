using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.Domain.Repositories;

using Volo.Abp.Guids;

namespace HIS.Accounting;

public class FinancialPeriodManager : DomainService
{
    private readonly IRepository<FinancialPeriod, Guid> _financialPeriodRepository;
    private readonly IGuidGenerator _guidGenerator;

    public FinancialPeriodManager(
        IRepository<FinancialPeriod, Guid> financialPeriodRepository,
        IGuidGenerator guidGenerator)
    {
        _financialPeriodRepository = financialPeriodRepository;
        _guidGenerator = guidGenerator;
    }

    public async Task CheckIfPeriodIsClosedAsync(DateTime date)
    {
        var year = date.Year;
        var month = date.Month;

        var period = await _financialPeriodRepository.FirstOrDefaultAsync(p => p.Year == year && p.Month == month);

        if (period == null)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            period = new FinancialPeriod(_guidGenerator.Create(), year, month, startDate, endDate);
            await _financialPeriodRepository.InsertAsync(period);
        }
        else if (period.IsClosed)
        {
            throw new Volo.Abp.UserFriendlyException($"لا يمكن إجراء حركات مالية في فترة مغلقة ({period.Name}).");
        }
    }

    public async Task ClosePeriodAsync(int year, int month)
    {
        var period = await _financialPeriodRepository.FirstOrDefaultAsync(p => p.Year == year && p.Month == month);
        if (period == null)
        {
            throw new Volo.Abp.UserFriendlyException("الفترة المالية غير موجودة.");
        }

        period.IsClosed = true;
        await _financialPeriodRepository.UpdateAsync(period);
    }
    
    public async Task OpenPeriodAsync(int year, int month)
    {
        var period = await _financialPeriodRepository.FirstOrDefaultAsync(p => p.Year == year && p.Month == month);
        if (period == null)
        {
            throw new Volo.Abp.UserFriendlyException("الفترة المالية غير موجودة.");
        }

        period.IsClosed = false;
        await _financialPeriodRepository.UpdateAsync(period);
    }
}
