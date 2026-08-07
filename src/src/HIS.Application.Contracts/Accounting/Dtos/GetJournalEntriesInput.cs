using System;
using Volo.Abp.Application.Dtos;

namespace HIS.Accounting.Dtos;

public class GetJournalEntriesInput : PagedAndSortedResultRequestDto
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}
