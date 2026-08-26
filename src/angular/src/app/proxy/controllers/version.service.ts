import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class VersionService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  getVersion = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, string>({
      method: 'GET',
      responseType: 'text',
      url: '/api/app/version',
    },
    { apiName: this.apiName,...config });
}