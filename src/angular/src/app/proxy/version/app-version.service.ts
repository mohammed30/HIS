import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class AppVersionService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  getVersion = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, string>({
      method: 'GET',
      responseType: 'text',
      url: '/api/app/app-version/version',
    },
    { apiName: this.apiName,...config });
}