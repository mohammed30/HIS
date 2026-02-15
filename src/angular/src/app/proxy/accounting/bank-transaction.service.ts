import { RestService } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import type { BankTransactionDto, CreateUpdateBankTransactionDto } from './dtos/models';

@Injectable({
    providedIn: 'root',
})
export class BankTransactionService {
    apiName = 'Default';

    create = (input: CreateUpdateBankTransactionDto) =>
        this.restService.request<any, BankTransactionDto>({
            method: 'POST',
            url: '/api/app/bank-transaction',
            body: input,
        },
            { apiName: this.apiName });

    update = (id: string, input: CreateUpdateBankTransactionDto) =>
        this.restService.request<any, BankTransactionDto>({
            method: 'PUT',
            url: `/api/app/bank-transaction/${id}`,
            body: input,
        },
            { apiName: this.apiName });

    delete = (id: string) =>
        this.restService.request<any, void>({
            method: 'DELETE',
            url: `/api/app/bank-transaction/${id}`,
        },
            { apiName: this.apiName });

    get = (id: string) =>
        this.restService.request<any, BankTransactionDto>({
            method: 'GET',
            url: `/api/app/bank-transaction/${id}`,
        },
            { apiName: this.apiName });

    getList = (input: PagedAndSortedResultRequestDto) =>
        this.restService.request<any, PagedResultDto<BankTransactionDto>>({
            method: 'GET',
            url: '/api/app/bank-transaction',
            params: input,
        },
            { apiName: this.apiName });

    constructor(private restService: RestService) { }
}
