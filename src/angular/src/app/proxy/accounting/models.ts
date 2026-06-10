
export interface DashboardAccountBalanceDto {
  accountCode?: string;
  accountName?: string;
  balance?: number;
}

export interface DashboardBalanceSheetDto {
  asOfDate?: string;
  totalAssets?: number;
  totalLiabilities?: number;
  totalEquity?: number;
  assetAccounts?: DashboardAccountBalanceDto[];
  liabilityAccounts?: DashboardAccountBalanceDto[];
  equityAccounts?: DashboardAccountBalanceDto[];
}

export interface DashboardIncomeStatementDto {
  startDate?: string;
  endDate?: string;
  totalRevenue?: number;
  totalExpenses?: number;
  netIncome?: number;
  revenueAccounts?: DashboardAccountBalanceDto[];
  expenseAccounts?: DashboardAccountBalanceDto[];
}

export interface DepartmentProfitabilityDto {
  costCenterId?: string;
  costCenterName?: string;
  totalRevenue?: number;
  totalExpense?: number;
  profit?: number;
}

export interface FinancialDashboardSummaryDto {
  totalAssets?: number;
  totalLiabilities?: number;
  totalRevenue?: number;
  totalExpenses?: number;
  netIncome?: number;
  departmentProfitability?: DepartmentProfitabilityDto[];
}
