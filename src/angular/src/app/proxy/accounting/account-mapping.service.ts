import type { AccountMappingDto, UpdateAccountMappingDto } from './dtos/models';
import { RestService, Rest } from '@abp/ng.core';
import type { ListResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class AccountMappingService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  getList = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, ListResultDto<AccountMappingDto>>({
      method: 'GET',
      url: '/api/app/account-mapping',
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: UpdateAccountMappingDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, AccountMappingDto>({
      method: 'PUT',
      url: `/api/app/account-mapping/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}