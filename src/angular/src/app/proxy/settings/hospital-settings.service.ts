import type { HospitalSettingsDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class HospitalSettingsService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  get = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, HospitalSettingsDto>({
      method: 'GET',
      url: '/api/app/hospital-settings',
    },
    { apiName: this.apiName,...config });
  

  update = (input: HospitalSettingsDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'PUT',
      url: '/api/app/hospital-settings',
      body: input,
    },
    { apiName: this.apiName,...config });
}