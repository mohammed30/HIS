import type { PharmacySettingsDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class PharmacySettingsService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  get = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, PharmacySettingsDto>({
      method: 'GET',
      url: '/api/app/pharmacy-settings',
    },
    { apiName: this.apiName,...config });
  

  update = (input: PharmacySettingsDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'PUT',
      url: '/api/app/pharmacy-settings',
      body: input,
    },
    { apiName: this.apiName,...config });
}