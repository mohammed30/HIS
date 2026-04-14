import type { GetPaidTicketsInput, GetPharmacySalesInput, PaidTicketDto, PharmacySalesDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ReportService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  getPaidTickets = (input: GetPaidTicketsInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<PaidTicketDto>>({
      method: 'GET',
      url: '/api/app/report/paid-tickets',
      params: { fromDate: input.fromDate, toDate: input.toDate, creatorUser: input.creatorUser, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getPaidTicketsPdf = (input: GetPaidTicketsInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, Blob>({
      method: 'GET',
      responseType: 'blob',
      url: '/api/app/report/paid-tickets-pdf',
      params: { fromDate: input.fromDate, toDate: input.toDate, creatorUser: input.creatorUser, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getPharmacySales = (input: GetPharmacySalesInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<PharmacySalesDto>>({
      method: 'GET',
      url: '/api/app/report/pharmacy-sales',
      params: { fromDate: input.fromDate, toDate: input.toDate, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getPharmacySalesPdf = (input: GetPharmacySalesInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, Blob>({
      method: 'GET',
      responseType: 'blob',
      url: '/api/app/report/pharmacy-sales-pdf',
      params: { fromDate: input.fromDate, toDate: input.toDate, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  refundTicket = (appointmentId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/report/refund-ticket/${appointmentId}`,
    },
    { apiName: this.apiName,...config });
}