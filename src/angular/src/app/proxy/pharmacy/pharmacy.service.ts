import type { DispensingLabelDto } from './dtos/models';
import type { DispenseDto, PendingPrescriptionDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';
import type { InventoryItemDto } from '../inventory/dtos/models';

@Injectable({
  providedIn: 'root',
})
export class PharmacyService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  checkInteractions = (patientId: string, newDrugName: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, string[]>({
      method: 'POST',
      url: `/api/app/pharmacy/check-interactions/${patientId}`,
      params: { newDrugName },
    },
    { apiName: this.apiName,...config });
  

  dispenseMedication = (input: DispenseDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/pharmacy/dispense-medication',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  getLabel = (dispensingId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DispensingLabelDto>({
      method: 'GET',
      url: `/api/app/pharmacy/label/${dispensingId}`,
    },
    { apiName: this.apiName,...config });
  

  getPendingPrescriptions = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, PendingPrescriptionDto[]>({
      method: 'GET',
      url: '/api/app/pharmacy/pending-prescriptions',
    },
    { apiName: this.apiName,...config });
  

  getPharmacyStock = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, InventoryItemDto[]>({
      method: 'GET',
      url: '/api/app/pharmacy/pharmacy-stock',
    },
    { apiName: this.apiName,...config });
  

  getPrescription = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PendingPrescriptionDto>({
      method: 'GET',
      url: `/api/app/pharmacy/${id}/prescription`,
    },
    { apiName: this.apiName,...config });
}