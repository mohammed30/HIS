import type { CreateDispensingDto, DispensingLabelDto, DispensingVerificationDto, VerifyPrescriptionDto } from './dtos/models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class DispensingService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  dispense = (input: CreateDispensingDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/dispensing/dispense',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  getLabel = (dispensingId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DispensingLabelDto>({
      method: 'GET',
      url: `/api/app/dispensing/label/${dispensingId}`,
    },
    { apiName: this.apiName,...config });
  

  getVerification = (medicalOrderId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DispensingVerificationDto>({
      method: 'GET',
      url: `/api/app/dispensing/verification/${medicalOrderId}`,
    },
    { apiName: this.apiName,...config });
  

  verifyPrescription = (input: VerifyPrescriptionDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/dispensing/verify-prescription',
      body: input,
    },
    { apiName: this.apiName,...config });
}