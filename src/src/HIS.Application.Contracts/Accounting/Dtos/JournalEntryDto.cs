using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace HIS.Accounting.Dtos;

public class JournalEntryDto : FullAuditedEntityDto<Guid>
{
    public DateTime Date { get; set; }
    public string ReferenceNumber { get; set; }
    public string Description { get; set; }
    public bool IsPosted { get; set; }
    public List<JournalEntryLineDto> Lines { get; set; }
}

public class JournalEntryLineDto : EntityDto<Guid>
{
    public Guid AccountId { get; set; }
    public string AccountName { get; set; }
    public string AccountNameAr { get; set; }
    public string AccountCode { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
}
