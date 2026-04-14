import type { PosProductDto, PosSaleDto } from './dtos/models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class PosService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  getInvoicePdf = (idOrNumber: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, Blob>({
      method: 'GET',
      responseType: 'blob',
      url: `/api/app/pos/generate-doc/${idOrNumber}`,
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
  

  searchProducts = (query: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PosProductDto[]>({
      method: 'POST',
      url: '/api/app/pos/search-products',
      params: { query },
    },
    { apiName: this.apiName,...config });
}