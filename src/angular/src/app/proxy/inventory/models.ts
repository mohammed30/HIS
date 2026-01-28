import { AuditedEntityDto, PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';

export interface WarehouseDto extends AuditedEntityDto<string> {
    name: string;
    location: string;
}

export interface CreateUpdateWarehouseDto {
    name: string;
    location: string;
}

export enum InventoryItemType {
    Medication = 0,
    Consumable = 1,
    Asset = 2,
    Reagent = 3,
    Other = 4
}

export interface InventoryItemDto {
    id: string; // The ID of the item
    warehouseId: string;
    warehouseName: string;
    productId: string;
    productName: string;
    type: InventoryItemType;
    quantity: number;
    averageCost: number;
}

export interface ReceiveStockDto {
    warehouseId: string;
    productId: string;
    productName: string;
    type: InventoryItemType;
    quantity: number;
    unitCost: number;
    supplierId?: string;
    referenceNumber: string;
}

export interface IssueStockDto {
    warehouseId: string;
    productId: string;
    quantity: number;
    departmentId?: string;
    referenceNumber: string;
}

export enum TransactionType {
    Received = 0,
    Issued = 1,
    Adjustment = 2
}

export interface InventoryTransactionDto {
    id: string;
    inventoryItemId: string;
    transactionType: TransactionType;
    quantity: number;
    unitCost: number;
    totalValue: number;
    transactionDate: string;
    referenceNumber: string;
}
