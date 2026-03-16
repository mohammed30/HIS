import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { EnvironmentService, RestService } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ReportService {
  apiName = 'Default';

  constructor(private restService: RestService, private environmentService: EnvironmentService) {}

  private getRootUrl(): string {
    return this.environmentService.getApiUrl(this.apiName);
  }

  getPaidTickets = (input: GetPaidTicketsInput) =>
    this.restService.request<any, PagedResultDto<PaidTicketDto>>({
      method: 'GET',
      url: '/api/app/report/paid-tickets',
      params: { 
        fromDate: input.fromDate, 
        toDate: input.toDate, 
        creatorUser: input.creatorUser,
        sorting: input.sorting,
        skipCount: input.skipCount,
        maxResultCount: input.maxResultCount
      },
    }, { apiName: this.apiName });

  refundTicket = (appointmentId: string) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/report/refund-ticket`,
      params: { appointmentId },
    }, { apiName: this.apiName });

  getPharmacySales = (input: GetPharmacySalesInput) =>
    this.restService.request<any, PagedResultDto<PharmacySalesDto>>({
      method: 'GET',
      url: '/api/app/report/pharmacy-sales',
      params: { 
        fromDate: input.fromDate, 
        toDate: input.toDate,
        sorting: input.sorting,
        skipCount: input.skipCount,
        maxResultCount: input.maxResultCount
      },
    }, { apiName: this.apiName });

  getPaidTicketsPdf = (input: GetPaidTicketsInput) =>
    `${this.getRootUrl()}/api/app/report/paid-tickets-pdf?fromDate=${input.fromDate || ''}&toDate=${input.toDate || ''}&creatorUser=${input.creatorUser || ''}`;

  getPharmacySalesPdf = (input: GetPharmacySalesInput) =>
    `${this.getRootUrl()}/api/app/report/pharmacy-sales-pdf?fromDate=${input.fromDate || ''}&toDate=${input.toDate || ''}`;
}

export interface PaidTicketDto {
  appointmentId: string;
  ticketNumber: string;
  patientName: string;
  clinicName: string;
  doctorName: string;
  serviceName: string;
  amount: number;
  appointmentDate: string;
  createdByUser: string;
  creationTime: string;
}

export interface GetPaidTicketsInput extends PagedAndSortedResultRequestDto {
  fromDate?: string;
  toDate?: string;
  creatorUser?: string;
}

export interface PharmacySalesDto {
  dispensingId: string;
  patientName: string;
  productName: string;
  quantity: number;
  unitPrice: number;
  totalAmount: number;
  withdrawalType: string;
  createdByUser: string;
  dispensingTime: string;
}

export interface GetPharmacySalesInput extends PagedAndSortedResultRequestDto {
  fromDate?: string;
  toDate?: string;
}
