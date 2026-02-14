import type { MedicalOrderDto } from '../clinical/models';

export interface DispenseDto {
  medicalOrderId: string;
}

export interface PendingPrescriptionDto extends MedicalOrderDto {
  patientName?: string;
  patientMRN?: string;
  dosage?: string;
  frequency?: string;
  route?: string;
  duration?: string;
  instructions?: string;
}

export interface VerificationStatus {
  // Enum map if needed, or just numbers
}

export interface VerifyPrescriptionDto {
  medicalOrderId: string;
  isApproved: boolean;
  safetyCheckComments?: string;
}

export interface DispensingVerificationDto {
  id: string;
  medicalOrderId: string;
  pharmacistId: string;
  verificationTime: string;
  isApproved: boolean;
  safetyCheckComments?: string;
  status: number;
}

export interface StockTransferDto {
  id: string;
  transferNumber: string;
  fromWarehouseId: string;
  fromWarehouseName?: string;
  toWarehouseId: string;
  toWarehouseName?: string;
  status: number;
  transferDate?: string;
  items: StockTransferItemDto[];
}

export interface StockTransferItemDto {
  drugId: string;
  quantity: number;
  batchNumber?: string;
  expiryDate?: string;
}

export interface InventoryItemDto {
  id: string;
  warehouseId: string;
  productId: string;
  productName: string;
  quantity: number;
  averageCost: number;
}

export interface CreateStockTransferDto {
  fromWarehouseId: string;
  toWarehouseId: string;
  notes?: string;
  items: CreateStockTransferItemDto[];
}

export interface CreateStockTransferItemDto {
  drugId: string;
  quantity: number;
}

export interface PosProductDto {
  id: string;
  name: string;
  barcode: string;
  price: number;
  currentStock: number;
}

export interface PosSaleDto {
  patientId?: string;
  items: PosSaleItemDto[];
  totalAmount: number;
  paidAmount: number;
  paymentMethod: number;
}

export interface PosSaleItemDto {
  drugId: string;
  quantity: number;
  unitPrice: number;
  discount: number;
}
