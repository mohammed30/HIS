import type { PosApproveDto, PosInvoiceListDto, PosPartialRefundDto, PosProductDto, PosRefundResultDto, PosRejectDto, PosSaleDto } from './dtos/models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';
import type { InvoiceStatus } from '../billing/invoice-status.enum';

@Injectable({
  providedIn: 'root',
})
export class PosService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  approveAndPay = (invoiceId: string, input: PosApproveDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/pos/${invoiceId}/approve-and-pay`,
      body: input,
    },
    { apiName: this.apiName,...config });
  

  createDraft = (input: PosSaleDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, string>({
      method: 'POST',
      responseType: 'text',
      url: '/api/app/pos/create-draft',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  dispense = (invoiceId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/pos/${invoiceId}/dispense`,
    },
    { apiName: this.apiName,...config });
  

  getInvoiceDetails = (invoiceId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PosInvoiceListDto>({
      method: 'GET',
      url: `/api/app/pos/invoices/${invoiceId}`,
    },
    { apiName: this.apiName,...config });
  

  getInvoicePdf = (idOrNumber: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, Blob>({
      method: 'GET',
      responseType: 'blob',
      url: `/api/app/pos/generate-doc/${idOrNumber}`,
    },
    { apiName: this.apiName,...config });
  

  getPosInvoices = (status?: InvoiceStatus, filter?: string, fromDate?: string, toDate?: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PosInvoiceListDto[]>({
      method: 'GET',
      url: '/api/app/pos/invoices',
      params: { status, filter, fromDate, toDate },
    },
    { apiName: this.apiName,...config });
  

  getProductByBarcode = (barcode: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PosProductDto>({
      method: 'GET',
      url: '/api/app/pos/product-by-barcode',
      params: { barcode },
    },
    { apiName: this.apiName,...config });
  

  getProductById = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PosProductDto>({
      method: 'GET',
      url: `/api/app/pos/${id}/product-by-id`,
    },
    { apiName: this.apiName,...config });
  

  getReturnInvoicePdf = (refundInvoiceId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, Blob>({
      method: 'GET',
      responseType: 'blob',
      url: `/api/app/pos/return-doc/${refundInvoiceId}`,
    },
    { apiName: this.apiName,...config });
  

  partialRefund = (invoiceId: string, input: PosPartialRefundDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PosRefundResultDto>({
      method: 'POST',
      url: `/api/app/pos/${invoiceId}/partial-refund`,
      body: input,
    },
    { apiName: this.apiName,...config });
  

  processSale = (input: PosSaleDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, string>({
      method: 'POST',
      responseType: 'text',
      url: '/api/app/pos/process-sale',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  refundSale = (invoiceNumber: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/pos/refund-sale/${invoiceNumber}`,
    },
    { apiName: this.apiName,...config });
  

  reject = (invoiceId: string, input: PosRejectDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/pos/${invoiceId}/reject`,
      body: input,
    },
    { apiName: this.apiName,...config });
  

  searchProducts = (query: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PosProductDto[]>({
      method: 'POST',
      url: '/api/app/pos/search-products',
      params: { query },
    },
    { apiName: this.apiName,...config });
  

  submitForApproval = (invoiceId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/pos/${invoiceId}/submit-for-approval`,
    },
    { apiName: this.apiName,...config });
}