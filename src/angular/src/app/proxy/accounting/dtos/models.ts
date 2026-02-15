import type { AuditedEntityDto, EntityDto, FullAuditedEntityDto } from '@abp/ng.core';
import type { AccountType } from '../account-type.enum';

export interface AccountDto extends AuditedEntityDto<string> {
  code?: string;
  name?: string;
  nameAr?: string;
  type?: AccountType;
  parentId?: string;
  parentName?: string;
  isActive?: boolean;
}

export interface CreateUpdateAccountDto {
  code: string;
  name: string;
  nameAr?: string;
  type?: AccountType;
  parentId?: string;
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

export interface CreateUpdateJournalEntryDto {
  date: string;
  referenceNumber?: string;
  description: string;
  lines: CreateUpdateJournalEntryLineDto[];
}

export interface CreateUpdateJournalEntryLineDto {
  accountId: string;
  debit: number;
  credit: number;
}

export interface AccountLookupDto extends EntityDto<string> {
  code?: string;
  name?: string;
  nameAr?: string;
  type?: AccountType;
  parentId?: string;
  hasChildren?: boolean;
}

import type { ClaimStatus } from '../claim-status.enum';
import type { BankTransactionType } from '../bank-transaction-type.enum';

export interface PaymentVoucherDto extends AuditedEntityDto<string> {
  voucherNumber?: string;
  date?: string;
  supplierId?: string;
  supplierName?: string;
  payeeName?: string;
  amount?: number;
  description?: string;
  paymentMethodId?: string;
  paymentMethodName?: string;
  lines?: PaymentVoucherLineDto[];
}

export interface PaymentVoucherLineDto extends EntityDto<string> {
  accountId?: string;
  accountName?: string;
  amount?: number;
  description?: string;
}

export interface CreateUpdatePaymentVoucherDto {
  date: string;
  supplierId?: string;
  payeeName?: string;
  amount: number;
  description?: string;
  paymentMethodId?: string;
  lines: CreateUpdatePaymentVoucherLineDto[];
}

export interface CreateUpdatePaymentVoucherLineDto {
  accountId: string;
  amount: number;
  description?: string;
}

export interface ReceiptVoucherDto extends AuditedEntityDto<string> {
  voucherNumber?: string;
  date?: string;
  patientId?: string;
  patientName?: string;
  payerName?: string;
  amount?: number;
  description?: string;
  paymentMethodId?: string;
  paymentMethodName?: string;
  lines?: ReceiptVoucherLineDto[];
}

export interface ReceiptVoucherLineDto extends EntityDto<string> {
  accountId?: string;
  accountName?: string;
  amount?: number;
  description?: string;
}

export interface CreateUpdateReceiptVoucherDto {
  date: string;
  patientId?: string;
  payerName?: string;
  amount: number;
  description?: string;
  paymentMethodId?: string;
  lines: CreateUpdateReceiptVoucherLineDto[];
}

export interface CreateUpdateReceiptVoucherLineDto {
  accountId: string;
  amount: number;
  description?: string;
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

export interface CreateUpdateContractClaimDto {
  date: string;
  contractId: string;
  amount: number;
  status: ClaimStatus;
  remarks?: string;
}

export interface BankTransactionDto extends AuditedEntityDto<string> {
  date?: string;
  referenceNumber?: string;
  description?: string;
  amount?: number;
  transactionType?: BankTransactionType;
  relatedJournalEntryId?: string;
}

export interface CreateUpdateBankTransactionDto {
  date: string;
  referenceNumber?: string;
  description?: string;
  amount: number;
  transactionType: BankTransactionType;
}

export interface DateRangeDto {
  startDate: string;
  endDate: string;
}

export interface ReportTransactionDto {
  date: string;
  referenceNumber: string;
  description: string;
  amount: number;
  type: string;
  accountName: string;
}

export interface DailyAccountsReportDto {
  transactions: ReportTransactionDto[];
  totalReceipts: number;
  totalPayments: number;
}

export interface CustomerDebtDto {
  patientId: string;
  patientName: string;
  mrn: string;
  totalInvoiced: number;
  totalPaid: number;
  dueAmount: number;
}

export interface CustomerDebtsReportDto {
  debts: CustomerDebtDto[];
  totalOverallDebt: number;
}

export interface DiscountReportLineDto {
  date: string;
  invoiceNumber: string;
  patientName: string;
  totalAmount: number;
  discountAmount: number;
}

export interface DiscountsReportDto {
  lines: DiscountReportLineDto[];
  totalDiscounts: number;
}
