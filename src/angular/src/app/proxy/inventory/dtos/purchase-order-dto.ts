import { AuditedEntityDto } from '@abp/ng.core';

export enum PurchaseOrderStatus {
    Draft = 0,
    Confirmed = 1,
    Received = 2,
    Cancelled = 3
}

export interface PurchaseOrderDto extends AuditedEntityDto<string> {
    orderNumber: string;
    supplierId: string;
    supplierName?: string;
    orderDate: string;
    expectedDeliveryDate?: string;
    status: PurchaseOrderStatus;
    referenceNumber?: string;
    notes?: string;
    totalAmount: number;
    purchaseOrderLines: PurchaseOrderLineDto[];
}

export interface PurchaseOrderLineDto {
    id: string;
    purchaseOrderId: string;
    productId: string;
    productName?: string;
    quantity: number;
    unitPrice: number;
    discount: number;
    totalAmount: number;
    description?: string;
}

export interface CreateUpdatePurchaseOrderDto {
    supplierId: string;
    orderDate: string;
    expectedDeliveryDate?: string;
    referenceNumber?: string;
    notes?: string;
    purchaseOrderLines: CreateUpdatePurchaseOrderLineDto[];
}

export interface CreateUpdatePurchaseOrderLineDto {
    productId: string;
    quantity: number;
    unitPrice: number;
    discount: number;
    description?: string;
}
