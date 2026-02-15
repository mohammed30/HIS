import { RestService } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import type { CreateUpdatePaymentVoucherDto, PaymentVoucherDto } from './dtos/models';

@Injectable({
    providedIn: 'root',
})
export class PaymentVoucherService {
    apiName = 'Default';

    create = (input: CreateUpdatePaymentVoucherDto) =>
        this.restService.request<any, PaymentVoucherDto>({
            method: 'POST',
            url: '/api/app/payment-voucher',
            body: input,
        },
            { apiName: this.apiName });

    update = (id: string, input: CreateUpdatePaymentVoucherDto) =>
        this.restService.request<any, PaymentVoucherDto>({
            method: 'PUT',
            url: `/api/app/payment-voucher/${id}`,
            body: input,
        },
            { apiName: this.apiName });

    delete = (id: string) =>
        this.restService.request<any, void>({
            method: 'DELETE',
            url: `/api/app/payment-voucher/${id}`,
        },
            { apiName: this.apiName });

    get = (id: string) =>
        this.restService.request<any, PaymentVoucherDto>({
            method: 'GET',
            url: `/api/app/payment-voucher/${id}`,
        },
            { apiName: this.apiName });

    getList = (input: PagedAndSortedResultRequestDto) =>
        this.restService.request<any, PagedResultDto<PaymentVoucherDto>>({
            method: 'GET',
            url: '/api/app/payment-voucher',
            params: input,
        },
            { apiName: this.apiName });

    constructor(private restService: RestService) { }
}
