import type {
  PosProductDto, PosSaleDto, PosInvoiceListDto,
  PosApproveDto, PosRejectDto, PosPartialRefundDto, PosRefundResultDto
} from './dtos/models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class PosService {
  private restService = inject(RestService);
  apiName = 'Default';

  // ── Product Lookup ──────────────────────────────────────────

  getProductByBarcode = (barcode: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PosProductDto>({
      method: 'GET',
      url: '/api/app/pos/product-by-barcode',
      params: { barcode },
    }, { apiName: this.apiName, ...config });

  getProductById = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PosProductDto>({
      method: 'GET',
      url: `/api/app/pos/${id}/product-by-id`,
    }, { apiName: this.apiName, ...config });

  searchProducts = (query: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PosProductDto[]>({
      method: 'POST',
      url: '/api/app/pos/search-products',
      params: { query },
    }, { apiName: this.apiName, ...config });

  // ── Sales Workflow ──────────────────────────────────────────

  /** Step 1: Create Draft (Pharmacist) */
  createDraft = (input: PosSaleDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, string>({
      method: 'POST',
      responseType: 'text',
      url: '/api/app/pos/create-draft',
      body: input,
    }, { apiName: this.apiName, ...config });

  /** Step 3: Submit for Approval (Pharmacist → Accountant) */
  submitForApproval = (invoiceId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/pos/${invoiceId}/submit-for-approval`,
    }, { apiName: this.apiName, ...config });

  /** Step 4 Reject: Accountant Rejects Invoice */
  reject = (invoiceId: string, input: PosRejectDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/pos/${invoiceId}/reject`,
      body: input,
    }, { apiName: this.apiName, ...config });

  /** Step 5: Accountant Approves & Pays */
  approveAndPay = (invoiceId: string, input: PosApproveDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/pos/${invoiceId}/approve-and-pay`,
      body: input,
    }, { apiName: this.apiName, ...config });

  /** Step 7: Pharmacist Dispenses Items */
  dispense = (invoiceId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/pos/${invoiceId}/dispense`,
    }, { apiName: this.apiName, ...config });

  // ── Return / Refund ─────────────────────────────────────────

  partialRefund = (invoiceId: string, input: PosPartialRefundDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PosRefundResultDto>({
      method: 'POST',
      url: `/api/app/pos/${invoiceId}/partial-refund`,
      body: input,
    }, { apiName: this.apiName, ...config });

  // ── Queries ─────────────────────────────────────────────────

  getPosInvoices = (status?: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PosInvoiceListDto[]>({
      method: 'GET',
      url: '/api/app/pos/invoices',
      params: status !== undefined ? { status } : {},
    }, { apiName: this.apiName, ...config });

  getInvoiceDetails = (invoiceId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PosInvoiceListDto>({
      method: 'GET',
      url: `/api/app/pos/invoices/${invoiceId}`,
    }, { apiName: this.apiName, ...config });

  // ── Printing ────────────────────────────────────────────────

  getInvoicePdf = (idOrNumber: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, Blob>({
      method: 'GET',
      responseType: 'blob',
      url: `/api/app/pos/generate-doc/${idOrNumber}`,
    }, { apiName: this.apiName, ...config });

  getReturnInvoicePdf = (refundInvoiceId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, Blob>({
      method: 'GET',
      responseType: 'blob',
      url: `/api/app/pos/return-doc/${refundInvoiceId}`,
    }, { apiName: this.apiName, ...config });

  // ── Legacy ──────────────────────────────────────────────────

  processSale = (input: PosSaleDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, string>({
      method: 'POST',
      responseType: 'text',
      url: '/api/app/pos/process-sale',
      body: input,
    }, { apiName: this.apiName, ...config });

  refundSale = (invoiceNumber: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/pos/refund-sale/${invoiceNumber}`,
    }, { apiName: this.apiName, ...config });
}