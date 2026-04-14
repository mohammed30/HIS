import type { AccountDto, AccountLookupDto, AccountStatementDto, AccountStatementInputDto, AccountSummaryDto, BalanceSheetDto, CashFlowStatementDto, ChangesInEquityDto, CreateUpdateAccountDto, CustomerDebtsReportDto, DailyAccountsReportDto, DateRangeDto, DiscountsReportDto, IncomeStatementDto } from './dtos/models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateUpdateAccountDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, AccountDto>({
      method: 'POST',
      url: '/api/app/account',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/account/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, AccountDto>({
      method: 'GET',
      url: `/api/app/account/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getAccountStatement = (input: AccountStatementInputDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, AccountStatementDto>({
      method: 'GET',
      url: '/api/app/account/account-statement',
      params: { accountId: input.accountId, startDate: input.startDate, endDate: input.endDate },
    },
    { apiName: this.apiName,...config });
  

  getAccountSummary = (input: DateRangeDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, AccountSummaryDto[]>({
      method: 'GET',
      url: '/api/app/account/account-summary',
      params: { startDate: input.startDate, endDate: input.endDate },
    },
    { apiName: this.apiName,...config });
  

  getBalanceSheet = (input: DateRangeDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, BalanceSheetDto>({
      method: 'GET',
      url: '/api/app/account/balance-sheet',
      params: { startDate: input.startDate, endDate: input.endDate },
    },
    { apiName: this.apiName,...config });
  

  getBalanceSheetPdf = (startDate: string, endDate: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, Blob>({
      method: 'GET',
      responseType: 'blob',
      url: '/api/app/account/balance-sheet-pdf',
      params: { startDate, endDate },
    },
    { apiName: this.apiName,...config });
  

  getCashFlowStatement = (input: DateRangeDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CashFlowStatementDto>({
      method: 'GET',
      url: '/api/app/account/cash-flow-statement',
      params: { startDate: input.startDate, endDate: input.endDate },
    },
    { apiName: this.apiName,...config });
  

  getChangesInEquity = (input: DateRangeDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ChangesInEquityDto>({
      method: 'GET',
      url: '/api/app/account/changes-in-equity',
      params: { startDate: input.startDate, endDate: input.endDate },
    },
    { apiName: this.apiName,...config });
  

  getCustomerDebtsReport = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, CustomerDebtsReportDto>({
      method: 'GET',
      url: '/api/app/account/customer-debts-report',
    },
    { apiName: this.apiName,...config });
  

  getDailyAccountsReport = (input: DateRangeDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DailyAccountsReportDto>({
      method: 'GET',
      url: '/api/app/account/daily-accounts-report',
      params: { startDate: input.startDate, endDate: input.endDate },
    },
    { apiName: this.apiName,...config });
  

  getDiscountsReport = (input: DateRangeDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DiscountsReportDto>({
      method: 'GET',
      url: '/api/app/account/discounts-report',
      params: { startDate: input.startDate, endDate: input.endDate },
    },
    { apiName: this.apiName,...config });
  

  getIncomeStatement = (input: DateRangeDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, IncomeStatementDto>({
      method: 'GET',
      url: '/api/app/account/income-statement',
      params: { startDate: input.startDate, endDate: input.endDate },
    },
    { apiName: this.apiName,...config });
  

  getIncomeStatementPdf = (startDate: string, endDate: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, Blob>({
      method: 'GET',
      responseType: 'blob',
      url: '/api/app/account/income-statement-pdf',
      params: { startDate, endDate },
    },
    { apiName: this.apiName,...config });
  

  getList = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<AccountDto>>({
      method: 'GET',
      url: '/api/app/account',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getLookup = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, AccountLookupDto[]>({
      method: 'GET',
      url: '/api/app/account/lookup',
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateAccountDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, AccountDto>({
      method: 'PUT',
      url: `/api/app/account/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}