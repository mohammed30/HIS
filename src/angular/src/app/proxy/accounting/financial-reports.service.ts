import type { DashboardBalanceSheetDto, DashboardIncomeStatementDto, FinancialDashboardSummaryDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class FinancialReportsService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  getBalanceSheet = (asOfDate: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DashboardBalanceSheetDto>({
      method: 'GET',
      url: '/api/app/financial-reports/balance-sheet',
      params: { asOfDate },
    },
    { apiName: this.apiName,...config });
  

  getDashboardSummary = (startDate: string, endDate: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, FinancialDashboardSummaryDto>({
      method: 'GET',
      url: '/api/app/financial-reports/dashboard-summary',
      params: { startDate, endDate },
    },
    { apiName: this.apiName,...config });
  

  getDepartmentProfitabilityReport = (startDate: string, endDate: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, number[]>({
      method: 'GET',
      url: '/api/app/financial-reports/department-profitability-report',
      params: { startDate, endDate },
    },
    { apiName: this.apiName,...config });
  

  getIncomeStatement = (startDate: string, endDate: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DashboardIncomeStatementDto>({
      method: 'GET',
      url: '/api/app/financial-reports/income-statement',
      params: { startDate, endDate },
    },
    { apiName: this.apiName,...config });
}
