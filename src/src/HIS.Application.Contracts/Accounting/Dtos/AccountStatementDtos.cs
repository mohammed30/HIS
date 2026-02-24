using System;
using System.Collections.Generic;

namespace HIS.Accounting.Dtos;

public class AccountStatementInputDto
{
    public Guid? AccountId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class AccountStatementDto
{
    public string AccountCode { get; set; }
    public string AccountName { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal TotalDebit { get; set; }
    public decimal TotalCredit { get; set; }
    public decimal ClosingBalance { get; set; }
    public List<AccountStatementLineDto> Lines { get; set; } = new();
}

public class AccountStatementLineDto
{
    public DateTime Date { get; set; }
    public string ReferenceNumber { get; set; }
    public string Description { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public decimal RunningBalance { get; set; }
}

public class AccountSummaryDto
{
    public Guid AccountId { get; set; }
    public string AccountCode { get; set; }
    public string AccountName { get; set; }
    public AccountType AccountType { get; set; }
    public bool IsParent { get; set; }
    public decimal TotalDebit { get; set; }
    public decimal TotalCredit { get; set; }
    public decimal Balance { get; set; }
    public List<AccountSummaryDto> Children { get; set; } = new();
}
