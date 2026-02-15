using System;
using Volo.Abp.Application.Dtos;

namespace HIS.Accounting.Dtos
{
    public class ContractClaimDto : AuditedEntityDto<Guid>
    {
        public string ClaimNumber { get; set; }
        public DateTime Date { get; set; }
        public Guid ContractId { get; set; }
        public string ContractName { get; set; }
        public decimal Amount { get; set; }
        public ClaimStatus Status { get; set; }
        public string Remarks { get; set; }
    }

    public class CreateUpdateContractClaimDto
    {
        public DateTime Date { get; set; }
        public Guid ContractId { get; set; }
        public decimal Amount { get; set; }
        public ClaimStatus Status { get; set; }
        public string Remarks { get; set; }
    }
}
