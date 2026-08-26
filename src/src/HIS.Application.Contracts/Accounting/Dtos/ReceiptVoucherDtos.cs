using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace HIS.Accounting.Dtos
{
    public class ReceiptVoucherDto : AuditedEntityDto<Guid>
    {
        public string VoucherNumber { get; set; }
        public DateTime Date { get; set; }
        public Guid? PatientId { get; set; }
        public string PatientName { get; set; }
        public string PayerName { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public Guid? PaymentMethodId { get; set; }
        public string PaymentMethodName { get; set; }
        public long SerialNumber { get; set; }
        public bool IsCancelled { get; set; }
        public string? CancelledByUserName { get; set; }
        public DateTime? CancellationTime { get; set; }
        public string? CancellationReason { get; set; }
        public List<ReceiptVoucherLineDto> Lines { get; set; }
    }

    public class ReceiptVoucherLineDto : EntityDto<Guid>
    {
        public Guid AccountId { get; set; }
        public string AccountName { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }

    public class CreateUpdateReceiptVoucherDto
    {
        public DateTime Date { get; set; }
        public Guid? PatientId { get; set; }
        public string PayerName { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public Guid? PaymentMethodId { get; set; }
        public List<CreateUpdateReceiptVoucherLineDto> Lines { get; set; }
    }

    public class CreateUpdateReceiptVoucherLineDto
    {
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }
}
