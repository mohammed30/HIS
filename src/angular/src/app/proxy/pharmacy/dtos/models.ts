import type { AuditedEntityDto, EntityDto, FullAuditedEntityDto } from '@abp/ng.core';
import type { VerificationStatus } from '../verification-status.enum';
import type { PaymentMethod } from '../../billing/payment-method.enum';
import type { TransferStatus } from '../transfer-status.enum';

export interface CreateDispensedItemDto {
  inventoryItemId?: string;
  inventoryBatchId?: string;
  quantity?: number;
}

export interface CreateDispensingDto {
  medicalOrderId?: string;
  patientId?: string;
  counselingNotes?: string;
  items?: CreateDispensedItemDto[];
}

export interface CreateStockTransferDto {
  fromWarehouseId?: string;
  toWarehouseId?: string;
  notes?: string;
  items?: CreateStockTransferItemDto[];
}

export interface CreateStockTransferItemDto {
  drugId?: string;
  quantity?: number;
  batchNumber?: string;
  expiryDate?: string;
}

export interface CreateUpdateDrugDto {
  barcode: string;
  brandName: string;
  scientificName: string;
  strength?: string;
  form?: string;
  manufacturer?: string;
  batchNumberPrefix?: string;
  minimumStockLevel?: number;
  reorderLevel?: number;
  binLocation?: string;
  isControlled?: boolean;
  legalCategory?: string;
  price?: number;
}

export interface DispensingLabelDto {
  patientName?: string;
  mrn?: string;
  drugName?: string;
  dosageInstructions?: string;
  dispensedDate?: string;
  expiryDate?: string;
  pharmacistName?: string;
}

export interface DispensingVerificationDto extends FullAuditedEntityDto<string> {
  medicalOrderId?: string;
  pharmacistId?: string;
  pharmacistName?: string;
  verificationTime?: string;
  isApproved?: boolean;
  safetyCheckComments?: string;
  status?: VerificationStatus;
}

export interface DrugDto extends AuditedEntityDto<string> {
  barcode?: string;
  brandName?: string;
  scientificName?: string;
  strength?: string;
  form?: string;
  manufacturer?: string;
  batchNumberPrefix?: string;
  serviceItemId?: string;
  serviceItemName?: string;
  minimumStockLevel?: number;
  reorderLevel?: number;
  binLocation?: string;
  isControlled?: boolean;
  legalCategory?: string;
}

export interface PosProductDto {
  id?: string;
  name?: string;
  barcode?: string;
  price?: number;
  currentStock?: number;
}

export interface PosSaleDto {
  patientId?: string;
  items?: PosSaleItemDto[];
  totalAmount?: number;
  paidAmount?: number;
  paymentMethod?: PaymentMethod;
}

export interface PosSaleItemDto {
  drugId?: string;
  quantity?: number;
  unitPrice?: number;
  discount?: number;
}

export interface StockTransferDto extends FullAuditedEntityDto<string> {
  transferNumber?: string;
  fromWarehouseId?: string;
  fromWarehouseName?: string;
  toWarehouseId?: string;
  toWarehouseName?: string;
  status?: TransferStatus;
  transferDate?: string;
  notes?: string;
  items?: StockTransferItemDto[];
}

export interface StockTransferItemDto extends EntityDto<string> {
  stockTransferId?: string;
  drugId?: string;
  drugName?: string;
  quantity?: number;
  batchNumber?: string;
  expiryDate?: string;
}

export interface VerifyPrescriptionDto {
  medicalOrderId?: string;
  isApproved?: boolean;
  safetyCheckComments?: string;
}
