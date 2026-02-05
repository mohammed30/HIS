import type { CreatePaymentDto, GetPaymentsInput, PaymentDailyReportDto, PaymentDto, PaymentReceiptDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class PaymentService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreatePaymentDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PaymentDto>({
      method: 'POST',
      url: '/api/app/payment',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/payment/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PaymentDto>({
      method: 'GET',
      url: `/api/app/payment/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getDailyReport = (date: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PaymentDailyReportDto>({
      method: 'GET',
      url: '/api/app/payment/daily-report',
      params: { date },
    },
    { apiName: this.apiName,...config });
  

  getList = (input: GetPaymentsInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<PaymentDto>>({
      method: 'GET',
      url: '/api/app/payment',
      params: { searchText: input.searchText, patientId: input.patientId, invoiceId: input.invoiceId, paymentMethod: input.paymentMethod, fromDate: input.fromDate, toDate: input.toDate, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getReceiptData = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PaymentReceiptDto>({
      method: 'GET',
      url: `/api/app/payment/${id}/receipt-data`,
    },
    { apiName: this.apiName,...config });
  

  getTotalByDateRange = (from: string, to: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, number>({
      method: 'GET',
      url: '/api/app/payment/total-by-date-range',
      params: { from, to },
    },
    { apiName: this.apiName,...config });
  

  refund = (id: string, reason: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PaymentDto>({
      method: 'POST',
      url: `/api/app/payment/${id}/refund`,
      params: { reason },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreatePaymentDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PaymentDto>({
      method: 'PUT',
      url: `/api/app/payment/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}