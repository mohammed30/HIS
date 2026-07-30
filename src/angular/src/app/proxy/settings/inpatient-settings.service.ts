import type { InpatientSettingsDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class InpatientSettingsService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  get = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, InpatientSettingsDto>({
      method: 'GET',
      url: '/api/app/inpatient-settings',
    },
    { apiName: this.apiName,...config });
  

  update = (input: InpatientSettingsDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'PUT',
      url: '/api/app/inpatient-settings',
      body: input,
    },
    { apiName: this.apiName,...config });
}