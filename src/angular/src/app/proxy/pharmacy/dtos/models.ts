import type { AuditedEntityDto, EntityDto, FullAuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';
import type { VerificationStatus } from '../verification-status.enum';
import type { PaymentMethod } from '../../billing/payment-method.enum';
import type { InvoiceStatus } from '../../billing/invoice-status.enum';
import type { InvoiceType } from '../../billing/invoice-type.enum';
import type { TransferStatus } from '../transfer-status.enum';

export interface CreateDispensedItemDto {
  inventoryItemId?: string;
  inventoryBatchId?: string;
  quantity?: number;
}

export interface CreateDispensingDto {
  medicalOrderId?: string;
  patientId?: string;
  counselingNotes?: string | null;
  items?: CreateDispensedItemDto[];
}

export interface CreateStockTransferDto {
  fromWarehouseId?: string;
  toWarehouseId?: string;
  notes?: string | null;
  items?: CreateStockTransferItemDto[];
}

export interface CreateStockTransferItemDto {
  drugId?: string;
  quantity?: number;
  batchNumber?: string | null;
  expiryDate?: string | null;
}

export interface CreateUpdateDrugDto {
  barcode: string;
  brandName: string;
  scientificName: string;
  strength?: string | null;
  form?: string | null;
  manufacturer?: string | null;
  batchNumberPrefix?: string | null;
  minimumStockLevel?: number;
  reorderLevel?: number;
  binLocation?: string | null;
  isControlled?: boolean;
  legalCategory?: string | null;
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
  pharmacistId?: string | null;
  pharmacistName?: string | null;
  verificationTime?: string;
  isApproved?: boolean;
  safetyCheckComments?: string | null;
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
  serviceItemId?: string | null;
  serviceItemName?: string;
  minimumStockLevel?: number;
  reorderLevel?: number;
  binLocation?: string;
  isControlled?: boolean;
  legalCategory?: string | null;
}

export interface GetDrugListDto extends PagedAndSortedResultRequestDto {
  searchText?: string | null;
}

export interface PosApproveDto {
  paidAmount?: number;
  paymentMethod?: PaymentMethod;
  notes?: string | null;
}

export interface PosInvoiceItemDto {
  id?: string;
  description?: string;
  quantity?: number;
  unitPrice?: number;
  totalPrice?: number;
  serviceCode?: string;
}

export interface PosInvoiceListDto {
  id?: string;
  invoiceNumber?: string;
  invoiceDate?: string;
  patientName?: string;
  totalAmount?: number;
  paidAmount?: number;
  status?: InvoiceStatus;
  invoiceType?: InvoiceType;
  rejectionReason?: string | null;
  originalInvoiceNumber?: string | null;
  items?: PosInvoiceItemDto[];
}

export interface PosPartialRefundDto {
  items?: PosRefundItemDto[];
}

export interface PosProductDto {
  id?: string;
  name?: string;
  barcode?: string;
  price?: number;
  currentStock?: number;
}

export interface PosRefundItemDto {
  invoiceItemId?: string;
  returnQuantity?: number;
}

export interface PosRefundResultDto {
  refundInvoiceId?: string;
  refundInvoiceNumber?: string;
  refundAmount?: number;
}

export interface PosRejectDto {
  rejectionReason?: string;
}

export interface PosSaleDto {
  patientId?: string | null;
  items?: PosSaleItemDto[];
  totalAmount?: number;
  paidAmount?: number;
  paymentMethod?: PaymentMethod;
  notes?: string | null;
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
  transferDate?: string | null;
  notes?: string | null;
  items?: StockTransferItemDto[];
}

export interface StockTransferItemDto extends EntityDto<string> {
  stockTransferId?: string;
  drugId?: string;
  drugName?: string;
  quantity?: number;
  batchNumber?: string | null;
  expiryDate?: string | null;
}

export interface VerifyPrescriptionDto {
  medicalOrderId?: string;
  isApproved?: boolean;
  safetyCheckComments?: string | null;
}
