import { AuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';

export enum AccountType {
    Asset = 0,
    Liability = 1,
    Equity = 2,
    Revenue = 3,
    Expense = 4
}

export interface AccountDto extends AuditedEntityDto<string> {
    code: string;
    name: string;
    nameAr?: string;
    type: AccountType;
    parentId?: string;
    parentName?: string;
    isActive: boolean;
}

export interface CreateUpdateAccountDto {
    code: string;
    name: string;
    nameAr?: string;
    type: AccountType;
    parentId?: string;
}

export interface JournalEntryLineDto {
    id: string;
    accountId: string;
    accountName: string;
    debit: number;
    credit: number;
}

export interface JournalEntryDto extends AuditedEntityDto<string> {
    date: string;
    referenceNumber: string;
    description: string;
    isPosted: boolean;
    lines: JournalEntryLineDto[];
}
