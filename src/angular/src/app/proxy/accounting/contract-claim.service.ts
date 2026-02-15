import { RestService } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import type { ContractClaimDto, CreateUpdateContractClaimDto } from './dtos/models';

@Injectable({
    providedIn: 'root',
})
export class ContractClaimService {
    apiName = 'Default';

    create = (input: CreateUpdateContractClaimDto) =>
        this.restService.request<any, ContractClaimDto>({
            method: 'POST',
            url: '/api/app/contract-claim',
            body: input,
        },
            { apiName: this.apiName });

    update = (id: string, input: CreateUpdateContractClaimDto) =>
        this.restService.request<any, ContractClaimDto>({
            method: 'PUT',
            url: `/api/app/contract-claim/${id}`,
            body: input,
        },
            { apiName: this.apiName });

    delete = (id: string) =>
        this.restService.request<any, void>({
            method: 'DELETE',
            url: `/api/app/contract-claim/${id}`,
        },
            { apiName: this.apiName });

    get = (id: string) =>
        this.restService.request<any, ContractClaimDto>({
            method: 'GET',
            url: `/api/app/contract-claim/${id}`,
        },
            { apiName: this.apiName });

    getList = (input: PagedAndSortedResultRequestDto) =>
        this.restService.request<any, PagedResultDto<ContractClaimDto>>({
            method: 'GET',
            url: '/api/app/contract-claim',
            params: input,
        },
            { apiName: this.apiName });

    constructor(private restService: RestService) { }
}
