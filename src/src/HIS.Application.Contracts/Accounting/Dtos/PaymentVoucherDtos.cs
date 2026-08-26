using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace HIS.Accounting.Dtos
{
    public class PaymentVoucherDto : AuditedEntityDto<Guid>
    {
        public string VoucherNumber { get; set; }
        public DateTime Date { get; set; }
        public Guid? SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string PayeeName { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public Guid? PaymentMethodId { get; set; }
        public string PaymentMethodName { get; set; }
        public long SerialNumber { get; set; }
        public bool IsCancelled { get; set; }
        public string? CancelledByUserName { get; set; }
        public DateTime? CancellationTime { get; set; }
        public string? CancellationReason { get; set; }
        public List<PaymentVoucherLineDto> Lines { get; set; }
    }

    public class PaymentVoucherLineDto : EntityDto<Guid>
    {
        public Guid AccountId { get; set; }
        public string AccountName { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }

    public class CreateUpdatePaymentVoucherDto
    {
        public DateTime Date { get; set; }
        public Guid? SupplierId { get; set; }
        public string PayeeName { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public Guid? PaymentMethodId { get; set; }
        public List<CreateUpdatePaymentVoucherLineDto> Lines { get; set; }
    }

    public class CreateUpdatePaymentVoucherLineDto
    {
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }
}
