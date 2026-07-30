import type { PagedAndSortedResultRequestDto } from '@abp/ng.core';

export interface GetPaidTicketsInput extends PagedAndSortedResultRequestDto {
  fromDate?: string | null;
  toDate?: string | null;
  creatorUser?: string | null;
}

export interface GetPharmacySalesInput extends PagedAndSortedResultRequestDto {
  fromDate?: string | null;
  toDate?: string | null;
}

export interface GetUserFinancialTransactionsInput extends PagedAndSortedResultRequestDto {
  userId?: string | null;
  moduleName?: string;
  startDate?: string | null;
  endDate?: string | null;
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

export interface UserFinancialTransactionDto {
  transactionId?: string;
  userId?: string | null;
  userName?: string;
  moduleName?: string;
  transactionType?: string;
  amount?: number;
  transactionDate?: string;
  description?: string;
  referenceNumber?: string;
}
