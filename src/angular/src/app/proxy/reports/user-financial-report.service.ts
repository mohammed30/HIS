import type { GetUserFinancialTransactionsInput, UserFinancialTransactionDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class UserFinancialReportService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  getList = (input: GetUserFinancialTransactionsInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<UserFinancialTransactionDto>>({
      method: 'GET',
      url: '/api/app/user-financial-report',
      params: { userId: input.userId, moduleName: input.moduleName, startDate: input.startDate, endDate: input.endDate, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
}