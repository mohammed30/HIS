using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace HIS.Accounting
{
    public class BankTransaction : FullAuditedAggregateRoot<Guid>
    {
        public DateTime Date { get; set; }
        public string ReferenceNumber { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; } // Positive for deposit, negative for withdrawal
        public BankTransactionType TransactionType { get; set; }
        public Guid BankAccountId { get; set; }
        public Guid OppositeAccountId { get; set; }
        public Guid? RelatedJournalEntryId { get; set; }
    }

}
