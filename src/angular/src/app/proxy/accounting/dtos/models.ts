import type { AuditedEntityDto, EntityDto, FullAuditedEntityDto } from '@abp/ng.core';
import type { AccountType } from '../account-type.enum';

export interface AccountDto extends AuditedEntityDto<string> {
  code?: string;
  name?: string;
  nameAr?: string;
  type?: AccountType;
  parentId?: string;
  parentName?: string;
  isActive?: boolean;
}

export interface CreateUpdateAccountDto {
  code: string;
  name: string;
  nameAr?: string;
  type?: AccountType;
  parentId?: string;
}

export interface JournalEntryDto extends FullAuditedEntityDto<string> {
  date?: string;
  referenceNumber?: string;
  description?: string;
  isPosted?: boolean;
  lines?: JournalEntryLineDto[];
}

export interface JournalEntryLineDto extends EntityDto<string> {
  accountId?: string;
  accountName?: string;
  debit?: number;
  credit?: number;
}
