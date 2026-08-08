import type { DoctorRevenueReportDto, DoctorRevenueReportInput } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class DoctorRevenueReportService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  getReport = (input: DoctorRevenueReportInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DoctorRevenueReportDto>({
      method: 'GET',
      url: '/api/app/doctor-revenue-report/report',
      params: { doctorId: input.doctorId, fromDate: input.fromDate, toDate: input.toDate },
    },
    { apiName: this.apiName,...config });
}