import type { PagedAndSortedResultRequestDto } from '@abp/ng.core';

export interface VoucherFilterDto extends PagedAndSortedResultRequestDto {
  filter?: string | null;
}
