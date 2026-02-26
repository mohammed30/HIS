using System;
using Volo.Abp.Application.Dtos;

namespace HIS.Billing;

public class InpatientDepositDto : FullAuditedEntityDto<Guid>
{
    public Guid PatientId { get; set; }
    public string? PatientName { get; set; }
    public Guid AdmissionId { get; set; }
    public string ReceiptNumber { get; set; } = string.Empty;
    public DateTime DepositDate { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? ReferenceNumber { get; set; }
    public Guid? JournalEntryId { get; set; }
    public string? ReceivedBy { get; set; }
    public string? Notes { get; set; }
    public DepositStatus Status { get; set; }
}

public class CreateInpatientDepositDto
{
    public Guid PatientId { get; set; }
    public Guid AdmissionId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }
}

public class GetInpatientDepositsInput : PagedAndSortedResultRequestDto
{
    public Guid? PatientId { get; set; }
    public Guid? AdmissionId { get; set; }
    public DepositStatus? Status { get; set; }
}

public interface IInpatientDepositAppService
{
}
