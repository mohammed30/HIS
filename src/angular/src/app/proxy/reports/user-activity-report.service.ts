import type { GetUserActivityFrequencyInput, UserActivityFrequencyDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class UserActivityReportService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  getList = (input: GetUserActivityFrequencyInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<UserActivityFrequencyDto>>({
      method: 'GET',
      url: '/api/app/user-activity-report',
      params: { userId: input.userId, module: input.module, startDate: input.startDate, endDate: input.endDate, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
}