using System;
using Volo.Abp.Application.Dtos;

namespace HIS.Reports
{
    public class GetUserActivityFrequencyInput : PagedAndSortedResultRequestDto
    {
        public Guid? UserId { get; set; }
        public string? Module { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
