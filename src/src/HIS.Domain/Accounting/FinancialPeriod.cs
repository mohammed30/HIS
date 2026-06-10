using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace HIS.Accounting;

/// <summary>
/// الفترة المالية (شهر / سنة)
/// </summary>
public class FinancialPeriod : FullAuditedAggregateRoot<Guid>
{
    public int Year { get; set; }
    public int Month { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsClosed { get; set; }
    
    /// <summary>
    /// اسم الفترة (مثال: يناير 2026)
    /// </summary>
    public string Name { get; set; }

    protected FinancialPeriod() { }

    public FinancialPeriod(Guid id, int year, int month, DateTime startDate, DateTime endDate)
        : base(id)
    {
        Year = year;
        Month = month;
        StartDate = startDate;
        EndDate = endDate;
        IsClosed = false;
        Name = $"{System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month)} {year}";
    }
}
