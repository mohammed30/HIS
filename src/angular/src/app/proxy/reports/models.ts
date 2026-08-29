import type { PagedAndSortedResultRequestDto } from '@abp/ng.core';

export interface GetInsuranceClaimsInput extends PagedAndSortedResultRequestDto {
  insuranceCompanyId?: string | null;
  startDate?: string | null;
  endDate?: string | null;
  serviceType?: string | null;
  patientType?: number | null;
}

export interface GetPaidTicketsInput extends PagedAndSortedResultRequestDto {
  fromDate?: string | null;
  toDate?: string | null;
  creatorUser?: string | null;
}

export interface GetPharmacySalesInput extends PagedAndSortedResultRequestDto {
  fromDate?: string | null;
  toDate?: string | null;
}

export interface GetUserActivityFrequencyInput extends PagedAndSortedResultRequestDto {
  userId?: string | null;
  module?: string | null;
  startDate?: string | null;
  endDate?: string | null;
}

export interface GetUserFinancialTransactionsInput extends PagedAndSortedResultRequestDto {
  userId?: string | null;
  moduleName?: string | null;
  startDate?: string | null;
  endDate?: string | null;
}

export interface InsuranceClaimItemDto {
  invoiceItemId?: string;
  departmentName?: string;
  serviceCode?: string;
  serviceName?: string;
  diagnosisCode?: string;
  approvalNumber?: string;
  doctorName?: string;
  totalPrice?: number;
  patientCoPay?: number;
  insuranceCoverage?: number;
}

export interface InsuranceClaimReportDto {
  invoiceId?: string;
  invoiceNumber?: string;
  invoiceDate?: string;
  patientId?: string;
  patientName?: string;
  patientFileNumber?: string;
  insuranceCompanyId?: string | null;
  insuranceCompanyName?: string;
  policyNumber?: string;
  sponsorName?: string;
  items?: InsuranceClaimItemDto[];
  totalPatientAmount?: number;
  totalInsuranceAmount?: number;
  totalInvoiceAmount?: number;
}

export interface PaidTicketDto {
  appointmentId?: string;
  ticketNumber?: string;
  patientName?: string;
  clinicName?: string;
  doctorName?: string;
  serviceName?: string;
  amount?: number;
  appointmentDate?: string;
  createdByUser?: string;
  creationTime?: string;
}

export interface PharmacySalesDto {
  dispensingId?: string;
  patientName?: string;
  productName?: string;
  quantity?: number;
  unitPrice?: number;
  totalAmount?: number;
  withdrawalType?: string;
  createdByUser?: string;
  dispensingTime?: string;
}

export interface UserActivityFrequencyDto {
  userId?: string | null;
  userName?: string | null;
  module?: string | null;
  entityType?: string | null;
  action?: string | null;
  date?: string;
  lastAccessTime?: string;
  frequencyCount?: number;
}

export interface UserFinancialTransactionDto {
  transactionId?: string;
  userId?: string | null;
  userName?: string | null;
  moduleName?: string | null;
  transactionType?: string | null;
  amount?: number;
  transactionDate?: string;
  description?: string | null;
  referenceNumber?: string | null;
  paymentCategory?: string | null;
}
