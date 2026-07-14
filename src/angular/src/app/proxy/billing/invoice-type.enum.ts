import { mapEnumToOptions } from '@abp/ng.core';

export enum InvoiceType {
  Sale = 0,
  Return = 1,
}

export const invoiceTypeOptions = mapEnumToOptions(InvoiceType);
