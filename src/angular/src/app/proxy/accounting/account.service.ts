import { Injectable } from '@angular/core';
import { RestService, PagedResultDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';
import { AccountDto, CreateUpdateAccountDto } from './models';

@Injectable({
    providedIn: 'root',
})
export class AccountService {
    apiName = 'Default';

    constructor(private restService: RestService) { }

    getList(input: PagedAndSortedResultRequestDto) {
        return this.restService.request<any, PagedResultDto<AccountDto>>({
            method: 'GET',
            url: '/api/app/account',
            params: input,
        },
            { apiName: this.apiName });
    }

    create(input: CreateUpdateAccountDto) {
        return this.restService.request<any, AccountDto>({
            method: 'POST',
            url: '/api/app/account',
            body: input,
        },
            { apiName: this.apiName });
    }

    update(id: string, input: CreateUpdateAccountDto) {
        return this.restService.request<any, AccountDto>({
            method: 'PUT',
            url: `/api/app/account/${id}`,
            body: input,
        },
            { apiName: this.apiName });
    }

    delete(id: string) {
        return this.restService.request<any, void>({
            method: 'DELETE',
            url: `/api/app/account/${id}`,
        },
            { apiName: this.apiName });
    }

    get(id: string) {
        return this.restService.request<any, AccountDto>({
            method: 'GET',
            url: `/api/app/account/${id}`,
        },
            { apiName: this.apiName });
    }
}
