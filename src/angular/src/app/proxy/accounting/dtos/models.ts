import type { AuditedEntityDto, EntityDto, FullAuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';
import type { AccountType } from '../account-type.enum';
import type { AccountMappingType } from '../account-mapping-type.enum';
import type { BankTransactionType } from '../bank-transaction-type.enum';
import type { ClaimStatus } from '../claim-status.enum';

export interface AccountDto extends AuditedEntityDto<string> {
  code?: string;
  name?: string;
  nameAr?: string;
  type?: AccountType;
  parentId?: string | null;
  parentName?: string;
  isActive?: boolean;
}

export interface AccountLookupDto extends EntityDto<string> {
  code?: string;
  name?: string;
  nameAr?: string;
  type?: AccountType;
  parentId?: string | null;
  hasChildren?: boolean;
}

export interface AccountMappingDto {
  id?: string;
  mappingType?: AccountMappingType;
  mappingTypeName?: string;
  accountId?: string | null;
  accountCode?: string;
  accountName?: string;
  accountNameAr?: string;
  isMandatory?: boolean;
  description?: string;
  descriptionAr?: string;
}

export interface AccountStatementDto {
  accountCode?: string;
  accountName?: string;
  openingBalance?: number;
  totalDebit?: number;
  totalCredit?: number;
  closingBalance?: number;
  lines?: AccountStatementLineDto[];
}

export interface AccountStatementInputDto {
  accountId?: string | null;
  startDate?: string;
  endDate?: string;
}

export interface AccountStatementLineDto {
  date?: string;
  referenceNumber?: string;
  description?: string;
  debit?: number;
  credit?: number;
  runningBalance?: number;
}

export interface AccountSummaryDto {
  accountId?: string;
  accountCode?: string;
  accountName?: string;
  accountType?: AccountType;
  isParent?: boolean;
  totalDebit?: number;
  totalCredit?: number;
  balance?: number;
  children?: AccountSummaryDto[];
}

export interface BalanceSheetDto {
  assetLines?: FinancialReportLineDto[];
  liabilityLines?: FinancialReportLineDto[];
  equityLines?: FinancialReportLineDto[];
  totalAssets?: number;
  totalLiabilities?: number;
  totalEquity?: number;
  previousYearEquity?: number;
  totalPreviousAssets?: number;
  totalPreviousLiabilities?: number;
  totalPreviousEquity?: number;
}

export interface BankTransactionDto extends AuditedEntityDto<string> {
  date?: string;
  referenceNumber?: string;
  description?: string;
  amount?: number;
  transactionType?: BankTransactionType;
  bankAccountId?: string | null;
  bankAccountName?: string;
  bankAccountNameAr?: string;
  oppositeAccountId?: string | null;
  oppositeAccountName?: string;
  oppositeAccountNameAr?: string;
  relatedJournalEntryId?: string | null;
}

export interface CashFlowStatementDto {
  operatingActivities?: FinancialReportLineDto[];
  investingActivities?: FinancialReportLineDto[];
  financingActivities?: FinancialReportLineDto[];
  totalOperating?: number;
  totalInvesting?: number;
  totalFinancing?: number;
  netCashFlow?: number;
  cashAtBeginning?: number;
  cashAtEnd?: number;
}

export interface ChangesInEquityDto {
  capital?: EquityItemDto;
  retainedEarnings?: EquityItemDto;
  netIncome?: EquityItemDto;
  dividends?: EquityItemDto;
  totalPreviousYear?: number;
  totalChange?: number;
  totalCurrentYear?: number;
}

export interface ContractClaimDto extends AuditedEntityDto<string> {
  claimNumber?: string;
  date?: string;
  contractId?: string;
  contractName?: string;
  amount?: number;
  status?: ClaimStatus;
  remarks?: string;
}

export interface CreateUpdateAccountDto {
  code?: string | null;
  name: string;
  nameAr?: string;
  type?: AccountType;
  parentId?: string | null;
}

export interface CreateUpdateBankTransactionDto {
  date?: string;
  referenceNumber?: string;
  description?: string;
  amount?: number;
  transactionType?: BankTransactionType;
  bankAccountId?: string;
  oppositeAccountId?: string;
}

export interface CreateUpdateContractClaimDto {
  date?: string;
  contractId?: string;
  amount?: number;
  status?: ClaimStatus;
  remarks?: string;
}

export interface CreateUpdateJournalEntryDto {
  date: string;
  referenceNumber?: string;
  description: string;
  lines: CreateUpdateJournalEntryLineDto[];
}

export interface CreateUpdateJournalEntryLineDto {
  accountId: string;
  debit?: number;
  credit?: number;
}

export interface CreateUpdatePaymentVoucherDto {
  date?: string;
  supplierId?: string | null;
  payeeName?: string;
  amount?: number;
  description?: string;
  paymentMethodId?: string | null;
  lines?: CreateUpdatePaymentVoucherLineDto[];
}

export interface CreateUpdatePaymentVoucherLineDto {
  accountId?: string;
  amount?: number;
  description?: string;
}

export interface CreateUpdateReceiptVoucherDto {
  date?: string;
  patientId?: string | null;
  payerName?: string;
  amount?: number;
  description?: string;
  paymentMethodId?: string | null;
  lines?: CreateUpdateReceiptVoucherLineDto[];
}

export interface CreateUpdateReceiptVoucherLineDto {
  accountId?: string;
  amount?: number;
  description?: string;
}

export interface CustomerDebtDto {
  patientId?: string;
  patientName?: string;
  mrn?: string;
  totalInvoiced?: number;
  totalPaid?: number;
  dueAmount?: number;
}

export interface CustomerDebtsReportDto {
  debts?: CustomerDebtDto[];
  totalOverallDebt?: number;
}

export interface DailyAccountsReportDto {
  transactions?: ReportTransactionDto[];
  totalReceipts?: number;
  totalPayments?: number;
}

export interface DateRangeDto {
  startDate?: string;
  endDate?: string;
}

export interface DiscountReportLineDto {
  date?: string;
  invoiceNumber?: string;
  patientName?: string;
  totalAmount?: number;
  discountAmount?: number;
}

export interface DiscountsReportDto {
  lines?: DiscountReportLineDto[];
  totalDiscounts?: number;
}

export interface EquityItemDto {
  name?: string;
  previousYear?: number;
  change?: number;
  currentYear?: number;
}

export interface FinancialReportLineDto {
  accountCode?: string;
  accountName?: string;
  amount?: number;
  previousAmount?: number;
}

export interface GetJournalEntriesInput extends PagedAndSortedResultRequestDto {
  dateFrom?: string | null;
  dateTo?: string | null;
}

export interface IncomeStatementDto {
  revenueLines?: FinancialReportLineDto[];
  costOfSalesLines?: FinancialReportLineDto[];
  totalRevenue?: number;
  totalCostOfSales?: number;
  grossProfit?: number;
  generalAndAdminExpenseLines?: FinancialReportLineDto[];
  totalGeneralAndAdminExpenses?: number;
  operatingProfit?: number;
  otherRevenueLines?: FinancialReportLineDto[];
  totalOtherRevenues?: number;
  otherExpenseLines?: FinancialReportLineDto[];
  totalOtherExpenses?: number;
  profitBeforeTax?: number;
  netIncome?: number;
}

export interface JournalEntryDto extends FullAuditedEntityDto<string> {
  date?: string;
  referenceNumber?: string;
  description?: string;
  isPosted?: boolean;
  lines?: JournalEntryLineDto[];
}

export interface JournalEntryLineDto extends EntityDto<string> {
  accountId?: string;
  accountName?: string;
  accountNameAr?: string;
  accountCode?: string;
  debit?: number;
  credit?: number;
}

export interface PaymentVoucherDto extends AuditedEntityDto<string> {
  voucherNumber?: string;
  date?: string;
  supplierId?: string | null;
  supplierName?: string;
  payeeName?: string;
  amount?: number;
  description?: string;
  paymentMethodId?: string | null;
  paymentMethodName?: string;
  lines?: PaymentVoucherLineDto[];
}

export interface PaymentVoucherLineDto extends EntityDto<string> {
  accountId?: string;
  accountName?: string;
  amount?: number;
  description?: string;
}

export interface ReceiptVoucherDto extends AuditedEntityDto<string> {
  voucherNumber?: string;
  date?: string;
  patientId?: string | null;
  patientName?: string;
  payerName?: string;
  amount?: number;
  description?: string;
  paymentMethodId?: string | null;
  paymentMethodName?: string;
  lines?: ReceiptVoucherLineDto[];
}

export interface ReceiptVoucherLineDto extends EntityDto<string> {
  accountId?: string;
  accountName?: string;
  amount?: number;
  description?: string;
}

export interface ReportTransactionDto {
  date?: string;
  referenceNumber?: string;
  description?: string;
  amount?: number;
  type?: string;
  accountName?: string;
}

export interface UpdateAccountMappingDto {
  accountId?: string | null;
}
