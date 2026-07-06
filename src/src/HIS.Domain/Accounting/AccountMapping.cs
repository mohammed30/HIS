using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace HIS.Accounting
{
    public class AccountMapping : FullAuditedAggregateRoot<Guid>
    {
        public AccountMappingType MappingType { get; set; }
        public Guid? AccountId { get; set; }
        public bool IsMandatory { get; set; }

        // Navigation property if needed, but simple Guid mapping is enough. We can add Account navigation property or just keep AccountId.
        // Let's keep it simple with AccountId. We can also add navigation property if required by EF Core.
        public Account Account { get; set; }

        protected AccountMapping() { }

        public AccountMapping(Guid id, AccountMappingType mappingType, Guid? accountId = null, bool isMandatory = true)
            : base(id)
        {
            MappingType = mappingType;
            AccountId = accountId;
            IsMandatory = isMandatory;
        }
    }
}
