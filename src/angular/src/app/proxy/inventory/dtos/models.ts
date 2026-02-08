import type { AuditedEntityDto, EntityDto } from '@abp/ng.core';
import type { InventoryItemType } from '../inventory-item-type.enum';
import type { TransactionType } from '../transaction-type.enum';

export interface CreateUpdateWarehouseDto {
  name?: string;
  location?: string;
}

export interface InventoryItemDto extends EntityDto<string> {
  warehouseId?: string;
  warehouseName?: string;
  productId?: string;
  productName?: string;
  type?: InventoryItemType;
  quantity?: number;
  averageCost?: number;
}

export interface InventoryTransactionDto extends EntityDto<string> {
  inventoryItemId?: string;
  transactionType?: TransactionType;
  quantity?: number;
  unitCost?: number;
  totalValue?: number;
  transactionDate?: string;
  referenceNumber?: string;
}

export interface IssueStockDto {
  warehouseId: string;
  productId: string;
  quantity: number;
  departmentId?: string;
  referenceNumber?: string;
}

export interface ReceiveStockDto {
  warehouseId: string;
  productId: string;
  productName?: string;
  type?: InventoryItemType;
  quantity: number;
  unitCost: number;
  supplierId?: string;
  referenceNumber?: string;
}

export interface WarehouseDto extends AuditedEntityDto<string> {
  name?: string;
  code?: string;
  location?: string;
}
