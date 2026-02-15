using System;
using Volo.Abp.Domain.Entities;

namespace HIS.Accounting
{
    public class ReceiptVoucherLine : Entity<Guid>
    {
        public Guid ReceiptVoucherId { get; set; }
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }

        public ReceiptVoucherLine()
        {
            
        }
    }
}
