using Volo.Abp.Application.Dtos;

namespace HIS.Pharmacy.Dtos;

public class GetDrugListDto : PagedAndSortedResultRequestDto
{
    public string? SearchText { get; set; }
}
