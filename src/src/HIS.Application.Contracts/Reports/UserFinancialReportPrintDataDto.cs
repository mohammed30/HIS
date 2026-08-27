using System;
using System.Collections.Generic;

namespace HIS.Reports
{
    public class UserFinancialReportPrintDataDto
    {
        public string? UserName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime PrintDate { get; set; }
        public List<UserFinancialTransactionDto> Transactions { get; set; } = new();
    }
}
