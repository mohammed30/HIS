import { RestService } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import type { CreateUpdateReceiptVoucherDto, ReceiptVoucherDto } from './dtos/models';

@Injectable({
    providedIn: 'root',
})
export class ReceiptVoucherService {
    apiName = 'Default';

    create = (input: CreateUpdateReceiptVoucherDto) =>
        this.restService.request<any, ReceiptVoucherDto>({
            method: 'POST',
            url: '/api/app/receipt-voucher',
            body: input,
        },
            { apiName: this.apiName });

    update = (id: string, input: CreateUpdateReceiptVoucherDto) =>
        this.restService.request<any, ReceiptVoucherDto>({
            method: 'PUT',
            url: `/api/app/receipt-voucher/${id}`,
            body: input,
        },
            { apiName: this.apiName });

    delete = (id: string) =>
        this.restService.request<any, void>({
            method: 'DELETE',
            url: `/api/app/receipt-voucher/${id}`,
        },
            { apiName: this.apiName });

    get = (id: string) =>
        this.restService.request<any, ReceiptVoucherDto>({
            method: 'GET',
            url: `/api/app/receipt-voucher/${id}`,
        },
            { apiName: this.apiName });

    getList = (input: PagedAndSortedResultRequestDto) =>
        this.restService.request<any, PagedResultDto<ReceiptVoucherDto>>({
            method: 'GET',
            url: '/api/app/receipt-voucher',
            params: input,
        },
            { apiName: this.apiName });

    constructor(private restService: RestService) { }
}
