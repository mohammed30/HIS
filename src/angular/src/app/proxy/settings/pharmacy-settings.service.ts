import { RestService } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import type { PharmacySettingsDto } from './models';

@Injectable({
  providedIn: 'root',
})
export class PharmacySettingsService {
  apiName = 'Default';

  get = () =>
    this.restService.request<any, PharmacySettingsDto>({
      method: 'GET',
      url: '/api/app/pharmacy-settings',
    },
    { apiName: this.apiName });

  update = (input: PharmacySettingsDto) =>
    this.restService.request<any, void>({
      method: 'PUT',
      url: '/api/app/pharmacy-settings',
      body: input,
    },
    { apiName: this.apiName });

  constructor(private restService: RestService) {}
}
