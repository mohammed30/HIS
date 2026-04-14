import type { FullAuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';
import type { InventoryCountStatus } from './inventory-count-status.enum';

export interface CreateInventoryCountDto {
  warehouseId?: string;
  countDate?: string;
  notes?: string | null;
}

export interface GetInventoryCountsInput extends PagedAndSortedResultRequestDto {
  warehouseId?: string | null;
  status?: InventoryCountStatus | null;
  fromDate?: string | null;
  toDate?: string | null;
}

export interface InventoryCountDto extends FullAuditedEntityDto<string> {
  warehouseId?: string;
  warehouseName?: string;
  countDate?: string;
  status?: InventoryCountStatus;
  notes?: string | null;
  items?: InventoryCountItemDto[];
}

export interface InventoryCountItemDto extends FullAuditedEntityDto<string> {
  inventoryItemId?: string;
  productName?: string;
  systemQuantity?: number;
  countedQuantity?: number;
  difference?: number;
  notes?: string | null;
}

export interface UpdateInventoryCountItemDto {
  id?: string;
  countedQuantity?: number;
  notes?: string | null;
}
