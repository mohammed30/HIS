using System;
using Volo.Abp.Application.Dtos;

namespace HIS.Accounting.Dtos
{
    public class BankTransactionDto : AuditedEntityDto<Guid>
    {
        public DateTime Date { get; set; }
        public string ReferenceNumber { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public BankTransactionType TransactionType { get; set; }
        public Guid? BankAccountId { get; set; }
        public string BankAccountName { get; set; }
        public Guid? OppositeAccountId { get; set; }
        public string OppositeAccountName { get; set; }
        public Guid? RelatedJournalEntryId { get; set; }
    }

    public class CreateUpdateBankTransactionDto
    {
        public DateTime Date { get; set; }
        public string ReferenceNumber { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public BankTransactionType TransactionType { get; set; }
        public Guid BankAccountId { get; set; }
        public Guid OppositeAccountId { get; set; }
    }
}
