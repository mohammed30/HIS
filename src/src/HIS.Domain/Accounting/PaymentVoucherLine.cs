using System;
using Volo.Abp.Domain.Entities;

namespace HIS.Accounting
{
    public class PaymentVoucherLine : Entity<Guid>
    {
        public Guid PaymentVoucherId { get; set; }
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }

        public PaymentVoucherLine()
        {
            
        }
    }
}
