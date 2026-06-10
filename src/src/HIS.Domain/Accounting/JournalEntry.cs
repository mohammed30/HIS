using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Guids;

namespace HIS.Accounting;

public class JournalEntry : FullAuditedAggregateRoot<Guid>
{
    public DateTime Date { get; set; }
    public string ReferenceNumber { get; set; }
    public string Description { get; set; }
    public bool IsPosted { get; set; }
    
    public ICollection<JournalEntryLine> Lines { get; set; }

    protected JournalEntry() { }

    public JournalEntry(Guid id, DateTime date, string referenceNumber, string description)
        : base(id)
    {
        Date = date;
        ReferenceNumber = referenceNumber;
        Description = description;
        Lines = new List<JournalEntryLine>();
    }

    public void AddLine(IGuidGenerator guidGenerator, Guid accountId, decimal debit, decimal credit, Guid? costCenterId = null)
    {
        Lines.Add(new JournalEntryLine(guidGenerator.Create(), Id, accountId, debit, credit, costCenterId));
    }
}
