using System;
using Volo.Abp.Application.Dtos;

namespace HIS.Accounting
{
    public class VoucherFilterDto : PagedAndSortedResultRequestDto
    {
        public string? Filter { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}