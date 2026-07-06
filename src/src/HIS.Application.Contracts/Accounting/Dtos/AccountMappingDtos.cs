using System;

namespace HIS.Accounting.Dtos
{
    public class AccountMappingDto
    {
        public Guid Id { get; set; }
        public AccountMappingType MappingType { get; set; }
        public string MappingTypeName => MappingType.ToString();
        public Guid? AccountId { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public string AccountNameAr { get; set; }
        public bool IsMandatory { get; set; }
        public string Description { get; set; }
        public string DescriptionAr { get; set; }
    }

    public class UpdateAccountMappingDto
    {
        public Guid? AccountId { get; set; }
    }
}
