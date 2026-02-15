import { FullAuditedEntityDto } from '@abp/ng.core';

export interface PurchaseRequisitionDto extends FullAuditedEntityDto<string> {
    requisitionNumber: string;
    requestorId: string;
    requestorName: string;
    departmentId: string;
    departmentName: string;
    requiredDate: string;
    status: number;
    notes?: string;
    lines: PurchaseRequisitionLineDto[];
}

export interface PurchaseRequisitionLineDto {
    id: string;
    productId: string;
    productName: string;
    quantity: number;
    description?: string;
}

export interface CreateUpdatePurchaseRequisitionDto {
    departmentId: string;
    requiredDate: string;
    notes?: string;
    lines: CreateUpdatePurchaseRequisitionLineDto[];
}

export interface CreateUpdatePurchaseRequisitionLineDto {
    productId: string;
    quantity: number;
    description?: string;
}
