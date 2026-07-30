import type { CreateLabReceptionOrderDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class LabReceptionService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  createLabReceptionOrder = (input: CreateLabReceptionOrderDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, string>({
      method: 'POST',
      responseType: 'text',
      url: '/api/app/lab-reception/create-order',
      body: input,
    },
    { apiName: this.apiName,...config });
}