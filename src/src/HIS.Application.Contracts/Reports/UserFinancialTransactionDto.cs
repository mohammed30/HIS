using System;
using Volo.Abp.Application.Dtos;

namespace HIS.Reports
{
    public class UserFinancialTransactionDto
    {
        public Guid TransactionId { get; set; }
        public Guid? UserId { get; set; }
        public string? UserName { get; set; }
        public string? ModuleName { get; set; }
        public string? TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? Description { get; set; }
        public string? ReferenceNumber { get; set; }
    }
}
