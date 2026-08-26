import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class UserSettingsService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  setTheme = (theme: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/user-settings/set-theme',
      params: { theme },
    },
    { apiName: this.apiName,...config });
}