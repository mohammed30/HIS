import type { PaymentMethod } from './payment-method.enum';
import type { ServiceType } from './service-type.enum';
import type { FullAuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';
import type { DeferredPaymentStatus } from './deferred-payment-status.enum';
import type { DepositStatus } from './deposit-status.enum';
import type { InvoiceStatus } from './invoice-status.enum';
import type { PaymentStatus } from './payment-status.enum';

export interface CreateDeferredPaymentDto {
  patientId?: string;
  invoiceId?: string | null;
  totalAmount?: number;
  dueDate?: string;
  numberOfInstallments?: number;
  reason?: string | null;
  contactPhone?: string | null;
  notes?: string | null;
}

export interface CreateInpatientDepositDto {
  patientId?: string;
  admissionId?: string;
  amount?: number;
  paymentMethod?: PaymentMethod;
  referenceNumber?: string | null;
  notes?: string | null;
}

export interface CreatePaymentDto {
  invoiceId?: string | null;
  patientId?: string;
  amount?: number;
  paymentMethod?: PaymentMethod;
  referenceNumber?: string | null;
  notes?: string | null;
}

export interface CreateUpdateInvoiceDto {
  patientId?: string;
  dueDate?: string | null;
  discountAmount?: number;
  taxPercentage?: number;
  patientInsuranceId?: string | null;
  appointmentId?: string | null;
  notes?: string | null;
  items?: CreateUpdateInvoiceItemDto[] | null;
}

export interface CreateUpdateInvoiceItemDto {
  serviceType?: ServiceType;
  serviceCode?: string | null;
  description?: string;
  quantity?: number;
  unitPrice?: number;
  discountPercentage?: number;
  discountAmount?: number;
  isCoveredByInsurance?: boolean;
  insurancePercentage?: number;
  notes?: string | null;
}

export interface DeferredPaymentDto extends FullAuditedEntityDto<string> {
  patientId?: string;
  patientName?: string | null;
  invoiceId?: string | null;
  invoiceNumber?: string | null;
  deferredNumber?: string;
  totalAmount?: number;
  paidAmount?: number;
  remainingAmount?: number;
  createdDate?: string;
  dueDate?: string;
  numberOfInstallments?: number;
  installmentAmount?: number;
  status?: DeferredPaymentStatus;
  reason?: string | null;
  contactPhone?: string | null;
  notes?: string | null;
}

export interface GetDeferredPaymentsInput extends PagedAndSortedResultRequestDto {
  searchText?: string | null;
  patientId?: string | null;
  status?: DeferredPaymentStatus | null;
}

export interface GetInpatientDepositsInput extends PagedAndSortedResultRequestDto {
  patientId?: string | null;
  admissionId?: string | null;
  status?: DepositStatus | null;
}

export interface GetInvoicesInput extends PagedAndSortedResultRequestDto {
  searchText?: string | null;
  patientId?: string | null;
  status?: InvoiceStatus | null;
  fromDate?: string | null;
  toDate?: string | null;
}

export interface GetPaymentsInput extends PagedAndSortedResultRequestDto {
  searchText?: string | null;
  patientId?: string | null;
  invoiceId?: string | null;
  paymentMethod?: PaymentMethod | null;
  fromDate?: string | null;
  toDate?: string | null;
}

export interface InpatientDepositDto extends FullAuditedEntityDto<string> {
  patientId?: string;
  patientName?: string | null;
  admissionId?: string;
  receiptNumber?: string;
  depositDate?: string;
  amount?: number;
  paymentMethod?: PaymentMethod;
  referenceNumber?: string | null;
  journalEntryId?: string | null;
  receivedBy?: string | null;
  notes?: string | null;
  status?: DepositStatus;
}

export interface InvoiceDto extends FullAuditedEntityDto<string> {
  patientId?: string;
  patientName?: string | null;
  invoiceNumber?: string;
  invoiceDate?: string;
  dueDate?: string | null;
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
  patientInsuranceId?: string | null;
  appointmentId?: string | null;
  notes?: string | null;
  items?: InvoiceItemDto[] | null;
}

export interface InvoiceItemDto {
  id?: string;
  invoiceId?: string;
  serviceType?: ServiceType;
  serviceCode?: string | null;
  description?: string;
  quantity?: number;
  unitPrice?: number;
  discountPercentage?: number;
  discountAmount?: number;
  totalPrice?: number;
  isCoveredByInsurance?: boolean;
  insurancePercentage?: number;
  notes?: string | null;
}

export interface PaymentDailyReportDto {
  date?: string;
  methods?: PaymentMethodSummaryDto[];
  totalAmount?: number;
}

export interface PaymentDto extends FullAuditedEntityDto<string> {
  invoiceId?: string | null;
  invoiceNumber?: string | null;
  patientId?: string;
  patientName?: string | null;
  paymentNumber?: string;
  paymentDate?: string;
  amount?: number;
  paymentMethod?: PaymentMethod;
  referenceNumber?: string | null;
  status?: PaymentStatus;
  receivedBy?: string | null;
  notes?: string | null;
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
