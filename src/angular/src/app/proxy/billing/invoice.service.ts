import type { InvoiceStatus } from './invoice-status.enum';
import type { CreateUpdateInvoiceDto, GetInvoicesInput, InvoiceDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class InvoiceService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  approveInvoice = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, InvoiceDto>({
      method: 'POST',
      url: `/api/app/invoice/${id}/approve-invoice`,
    },
    { apiName: this.apiName,...config });
  

  cancel = (id: string, reason: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, InvoiceDto>({
      method: 'POST',
      url: `/api/app/invoice/${id}/cancel`,
      params: { reason },
    },
    { apiName: this.apiName,...config });
  

  create = (input: CreateUpdateInvoiceDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, InvoiceDto>({
      method: 'POST',
      url: '/api/app/invoice',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/invoice/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, InvoiceDto>({
      method: 'GET',
      url: `/api/app/invoice/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getInvoicePdf = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, Blob>({
      method: 'GET',
      responseType: 'blob',
      url: `/api/app/billing/generate-doc/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: GetInvoicesInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<InvoiceDto>>({
      method: 'GET',
      url: '/api/app/invoice',
      params: { searchText: input.searchText, patientId: input.patientId, status: input.status, fromDate: input.fromDate, toDate: input.toDate, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getPendingApprovals = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, InvoiceDto[]>({
      method: 'GET',
      url: '/api/app/invoice/pending-approvals',
    },
    { apiName: this.apiName,...config });
  

  getWithItems = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, InvoiceDto>({
      method: 'GET',
      url: `/api/app/invoice/${id}/with-items`,
    },
    { apiName: this.apiName,...config });
  

  rejectInvoice = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, InvoiceDto>({
      method: 'POST',
      url: `/api/app/invoice/${id}/reject-invoice`,
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateInvoiceDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, InvoiceDto>({
      method: 'PUT',
      url: `/api/app/invoice/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
  

  updateStatus = (id: string, status: InvoiceStatus, config?: Partial<Rest.Config>) =>
    this.restService.request<any, InvoiceDto>({
      method: 'PUT',
      url: `/api/app/invoice/${id}/status`,
      params: { status },
    },
    { apiName: this.apiName,...config });
}