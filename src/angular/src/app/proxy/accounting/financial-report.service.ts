import type { BalanceSheetDto, CashFlowStatementDto, ChangesInEquityDto, DateRangeDto, IncomeStatementDto } from './dtos/models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class FinancialReportService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  getBalanceSheet = (input: DateRangeDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, BalanceSheetDto>({
      method: 'GET',
      url: '/api/app/financial-reports/balance-sheet',
      params: { startDate: input.startDate, endDate: input.endDate },
    },
    { apiName: this.apiName,...config });
  

  getCashFlowStatement = (input: DateRangeDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CashFlowStatementDto>({
      method: 'GET',
      url: '/api/app/financial-reports/cash-flow-statement',
      params: { startDate: input.startDate, endDate: input.endDate },
    },
    { apiName: this.apiName,...config });
  

  getChangesInEquity = (input: DateRangeDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ChangesInEquityDto>({
      method: 'GET',
      url: '/api/app/financial-reports/changes-in-equity',
      params: { startDate: input.startDate, endDate: input.endDate },
    },
    { apiName: this.apiName,...config });
  

  getIncomeStatement = (input: DateRangeDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, IncomeStatementDto>({
      method: 'GET',
      url: '/api/app/financial-reports/income-statement',
      params: { startDate: input.startDate, endDate: input.endDate },
    },
    { apiName: this.apiName,...config });
}