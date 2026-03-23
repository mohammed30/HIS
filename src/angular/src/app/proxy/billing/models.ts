import type { PaymentMethod } from './payment-method.enum';
import type { ServiceType } from './service-type.enum';
import type { FullAuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';
import type { DeferredPaymentStatus } from './deferred-payment-status.enum';
import type { DepositStatus } from './deposit-status.enum';
import type { InvoiceStatus } from './invoice-status.enum';
import type { PaymentStatus } from './payment-status.enum';

export interface CreateDeferredPaymentDto {
  patientId?: string;
  invoiceId?: string;
  totalAmount?: number;
  dueDate?: string;
  numberOfInstallments?: number;
  reason?: string;
  contactPhone?: string;
  notes?: string;
}

export interface CreateInpatientDepositDto {
  patientId?: string;
  admissionId?: string;
  amount?: number;
  paymentMethod?: PaymentMethod;
  referenceNumber?: string;
  notes?: string;
}

export interface CreatePaymentDto {
  invoiceId?: string;
  patientId?: string;
  amount?: number;
  paymentMethod?: PaymentMethod;
  referenceNumber?: string;
  notes?: string;
}

export interface CreateUpdateInvoiceDto {
  patientId?: string;
  dueDate?: string;
  discountAmount?: number;
  taxPercentage?: number;
  patientInsuranceId?: string;
  appointmentId?: string;
  notes?: string;
  items?: CreateUpdateInvoiceItemDto[];
}

export interface CreateUpdateInvoiceItemDto {
  serviceType?: ServiceType;
  serviceCode?: string;
  description?: string;
  quantity?: number;
  unitPrice?: number;
  discountPercentage?: number;
  discountAmount?: number;
  isCoveredByInsurance?: boolean;
  notes?: string;
}

export interface DeferredPaymentDto extends FullAuditedEntityDto<string> {
  patientId?: string;
  patientName?: string;
  invoiceId?: string;
  invoiceNumber?: string;
  deferredNumber?: string;
  totalAmount?: number;
  paidAmount?: number;
  remainingAmount?: number;
  createdDate?: string;
  dueDate?: string;
  numberOfInstallments?: number;
  installmentAmount?: number;
  status?: DeferredPaymentStatus;
  reason?: string;
  contactPhone?: string;
  notes?: string;
}

export interface GetDeferredPaymentsInput extends PagedAndSortedResultRequestDto {
  searchText?: string;
  patientId?: string;
  status?: DeferredPaymentStatus;
}

export interface GetInpatientDepositsInput extends PagedAndSortedResultRequestDto {
  patientId?: string;
  admissionId?: string;
  status?: DepositStatus;
}

export interface GetInvoicesInput extends PagedAndSortedResultRequestDto {
  searchText?: string;
  patientId?: string;
  status?: InvoiceStatus;
  fromDate?: string;
  toDate?: string;
}

export interface GetPaymentsInput extends PagedAndSortedResultRequestDto {
  searchText?: string;
  patientId?: string;
  invoiceId?: string;
  paymentMethod?: PaymentMethod;
  fromDate?: string;
  toDate?: string;
}

export interface InpatientDepositDto extends FullAuditedEntityDto<string> {
  patientId?: string;
  patientName?: string;
  admissionId?: string;
  receiptNumber?: string;
  depositDate?: string;
  amount?: number;
  paymentMethod?: PaymentMethod;
  referenceNumber?: string;
  journalEntryId?: string;
  receivedBy?: string;
  notes?: string;
  status?: DepositStatus;
}

export interface InvoiceDto extends FullAuditedEntityDto<string> {
  patientId?: string;
  patientName?: string;
  invoiceNumber?: string;
  invoiceDate?: string;
  dueDate?: string;
  totalAmount?: number;
  discountAmount?: number;
  taxPercentage?: number;
  taxAmount?: number;
  netAmount?: number;
  paidAmount?: number;
  dueAmount?: number;
  insuranceCoverage?: number;
  coPaymentAmount?: number;
  status?: InvoiceStatus;
  patientInsuranceId?: string;
  appointmentId?: string;
  notes?: string;
  items?: InvoiceItemDto[];
}

export interface InvoiceItemDto {
  id?: string;
  invoiceId?: string;
  serviceType?: ServiceType;
  serviceCode?: string;
  description?: string;
  quantity?: number;
  unitPrice?: number;
  discountPercentage?: number;
  discountAmount?: number;
  totalPrice?: number;
  isCoveredByInsurance?: boolean;
  notes?: string;
}

export interface PaymentDailyReportDto {
  date?: string;
  methods?: PaymentMethodSummaryDto[];
  totalAmount?: number;
}

export interface PaymentDto extends FullAuditedEntityDto<string> {
  invoiceId?: string;
  invoiceNumber?: string;
  patientId?: string;
  patientName?: string;
  paymentNumber?: string;
  paymentDate?: string;
  amount?: number;
  paymentMethod?: PaymentMethod;
  referenceNumber?: string;
  status?: PaymentStatus;
  receivedBy?: string;
  notes?: string;
}

export interface PaymentMethodSummaryDto {
  method?: PaymentMethod;
  methodName?: string;
  count?: number;
  total?: number;
}

export interface PaymentReceiptDto {
  paymentId?: string;
  paymentNumber?: string;
  paymentDate?: string;
  patientName?: string;
  patientFileNumber?: string;
  amount?: number;
  amountInWords?: string;
  paymentMethod?: string;
  referenceNumber?: string;
  receivedBy?: string;
  notes?: string;
  invoiceNumber?: string;
  items?: ReceiptItemDto[];
  hospitalName?: string;
  hospitalLogoUrl?: string;
}

export interface ReceiptItemDto {
  serviceName?: string;
  price?: number;
}
