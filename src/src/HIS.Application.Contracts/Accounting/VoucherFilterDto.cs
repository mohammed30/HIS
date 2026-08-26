using Volo.Abp.Application.Dtos;

namespace HIS.Accounting
{
    public class VoucherFilterDto : PagedAndSortedResultRequestDto
    {
        public string? Filter { get; set; }
    }
}