using System;
using Volo.Abp.Domain.Entities;

namespace HIS.Accounting;

public class JournalEntryLine : Entity<Guid>
{
    public Guid JournalEntryId { get; set; }
    public Guid AccountId { get; set; }
    public Guid? CostCenterId { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }

    protected JournalEntryLine() { }

    internal JournalEntryLine(Guid id, Guid journalEntryId, Guid accountId, decimal debit, decimal credit, Guid? costCenterId = null)
        : base(id)
    {
        JournalEntryId = journalEntryId;
        AccountId = accountId;
        Debit = debit;
        Credit = credit;
        CostCenterId = costCenterId;
    }
}
// Correcting: Entity<Guid> usually implies a Guid Id.
// Let's redefine slightly to be standard Entity<Guid>.
