using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities.Auditing;

namespace HIS.Accounting
{
    public class ReceiptVoucher : FullAuditedAggregateRoot<Guid>
    {
        public string VoucherNumber { get; set; }
        public DateTime Date { get; set; }
        public Guid? PatientId { get; set; }
        public string PayerName { get; set; } // If not a registered patient
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public Guid? PaymentMethodId { get; set; }

        public long SerialNumber { get; set; }
        public bool IsCancelled { get; set; }
        public Guid? CancelledByUserId { get; set; }
        public string? CancelledByUserName { get; set; }
        public DateTime? CancellationTime { get; set; }
        public string? CancellationReason { get; set; }

        public ICollection<ReceiptVoucherLine> Lines { get; set; }

        public ReceiptVoucher()
        {
            Lines = new Collection<ReceiptVoucherLine>();
        }
    }
}
