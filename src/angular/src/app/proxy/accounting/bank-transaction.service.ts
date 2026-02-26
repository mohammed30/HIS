import type { BankTransactionDto, CreateUpdateBankTransactionDto } from './dtos/models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class BankTransactionService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateUpdateBankTransactionDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, BankTransactionDto>({
      method: 'POST',
      url: '/api/app/bank-transaction',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/bank-transaction/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, BankTransactionDto>({
      method: 'GET',
      url: `/api/app/bank-transaction/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<BankTransactionDto>>({
      method: 'GET',
      url: '/api/app/bank-transaction',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateBankTransactionDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, BankTransactionDto>({
      method: 'PUT',
      url: `/api/app/bank-transaction/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}