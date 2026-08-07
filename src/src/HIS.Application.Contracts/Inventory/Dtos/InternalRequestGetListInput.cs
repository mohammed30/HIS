using System;
using Volo.Abp.Application.Dtos;

namespace HIS.Inventory.Dtos;

public class InternalRequestGetListInput : PagedAndSortedResultRequestDto
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? FilterText { get; set; }
}
