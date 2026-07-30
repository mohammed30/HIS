using System;
using Volo.Abp.Application.Dtos;

namespace HIS.Reports
{
    public class GetUserFinancialTransactionsInput : PagedAndSortedResultRequestDto
    {
        public Guid? UserId { get; set; }
        public string? ModuleName { get; set; } // e.g. "Payment", "ReceiptVoucher", "PaymentVoucher"
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
