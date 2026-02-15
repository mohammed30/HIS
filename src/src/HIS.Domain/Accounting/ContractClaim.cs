using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace HIS.Accounting
{
    public class ContractClaim : FullAuditedAggregateRoot<Guid>
    {
        public string ClaimNumber { get; set; }
        public DateTime Date { get; set; }
        public Guid ContractId { get; set; } // Link to Contracts
        public decimal Amount { get; set; }
        public ClaimStatus Status { get; set; }
        public string Remarks { get; set; }
    }

}
