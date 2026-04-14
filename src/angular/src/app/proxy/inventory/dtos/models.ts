import type { InternalRequestType } from '../internal-request-type.enum';
import type { AuditedEntityDto, EntityDto, FullAuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';
import type { PurchaseRequisitionStatus } from '../purchase-requisition-status.enum';
import type { InternalRequestStatus } from '../internal-request-status.enum';
import type { InventoryItemType } from '../inventory-item-type.enum';
import type { TransactionType } from '../transaction-type.enum';
import type { PurchaseInvoiceStatus } from '../purchase-invoice-status.enum';
import type { PurchaseOrderStatus } from '../purchase-order-status.enum';

export interface CreateUpdateInternalRequestDto {
  requestingDepartmentId?: string;
  fulfilledByWarehouseId?: string;
  admissionId?: string | null;
  requestDate?: string;
  requestType?: InternalRequestType;
  notes?: string;
  lines?: CreateUpdateInternalRequestLineDto[];
}

export interface CreateUpdateInternalRequestLineDto {
  inventoryItemId?: string;
  requestedQuantity?: number;
  notes?: string;
}

export interface CreateUpdatePurchaseInvoiceDto {
  invoiceNumber?: string;
  supplierId?: string;
  purchaseOrderId?: string | null;
  invoiceDate?: string;
  notes?: string;
  lines?: CreateUpdatePurchaseInvoiceLineDto[];
}

export interface CreateUpdatePurchaseInvoiceLineDto {
  productId?: string;
  quantity?: number;
  unitCost?: number;
  discount?: number;
  batchNumber?: string;
  expiryDate?: string | null;
}

export interface CreateUpdatePurchaseOrderDto {
  supplierId?: string;
  orderDate?: string;
  expectedDeliveryDate?: string | null;
  referenceNumber?: string;
  notes?: string;
  purchaseOrderLines?: CreateUpdatePurchaseOrderLineDto[];
}

export interface CreateUpdatePurchaseOrderLineDto {
  productId?: string;
  quantity?: number;
  unitPrice?: number;
  discount?: number;
  description?: string;
}

export interface CreateUpdatePurchaseRequisitionDto {
  departmentId?: string;
  requiredDate?: string;
  notes?: string | null;
  lines?: CreateUpdatePurchaseRequisitionLineDto[];
}

export interface CreateUpdatePurchaseRequisitionLineDto {
  productId?: string;
  quantity?: number;
  description?: string | null;
}

export interface CreateUpdateSupplierDto {
  name: string;
  contactPerson?: string;
  phone?: string;
  email?: string;
  address?: string;
  taxId?: string;
}

export interface CreateUpdateWarehouseDto {
  name?: string;
  location?: string;
}

export interface DepartmentConsumptionReportDto {
  departmentId?: string;
  departmentName?: string;
  productId?: string;
  productName?: string;
  quantity?: number;
  totalCost?: number;
}

export interface GetConsumptionReportInput {
  startDate?: string | null;
  endDate?: string | null;
  departmentId?: string | null;
}

export interface GetLowStockReportInput {
  warehouseId?: string | null;
}

export interface GetPurchaseRequisitionsInput extends PagedAndSortedResultRequestDto {
  filter?: string | null;
  status?: PurchaseRequisitionStatus | null;
  departmentId?: string | null;
}

export interface GetStagnantStockReportInput {
  warehouseId?: string | null;
  thresholdDays?: number;
}

export interface InternalRequestDto extends FullAuditedEntityDto<string> {
  requestNumber?: string;
  requestingDepartmentId?: string;
  requestingDepartmentName?: string;
  fulfilledByWarehouseId?: string;
  fulfilledByWarehouseName?: string;
  admissionId?: string | null;
  patientName?: string;
  requestDate?: string;
  requestType?: InternalRequestType;
  status?: InternalRequestStatus;
  notes?: string;
  lines?: InternalRequestLineDto[];
}

export interface InternalRequestLineDto extends FullAuditedEntityDto<string> {
  internalRequestId?: string;
  inventoryItemId?: string;
  inventoryItemName?: string;
  requestedQuantity?: number;
  approvedQuantity?: number;
  notes?: string;
}

export interface InventoryItemDto extends EntityDto<string> {
  warehouseId?: string;
  warehouseName?: string;
  productId?: string;
  productName?: string;
  type?: InventoryItemType;
  quantity?: number;
  averageCost?: number;
  minStockLevel?: number;
  reorderLevel?: number;
}

export interface InventoryTransactionDto extends EntityDto<string> {
  inventoryItemId?: string;
  transactionType?: TransactionType;
  quantity?: number;
  unitCost?: number;
  totalValue?: number;
  transactionDate?: string;
  referenceNumber?: string;
  departmentId?: string | null;
}

export interface IssueStockDto {
  warehouseId: string;
  productId: string;
  quantity: number;
  departmentId?: string | null;
  referenceNumber?: string;
}

export interface LowStockReportDto {
  productId?: string;
  productName?: string;
  warehouseName?: string;
  currentQuantity?: number;
  minStockLevel?: number;
  deficit?: number;
}

export interface PriceComparisonDto {
  supplierId?: string;
  supplierName?: string;
  unitPrice?: number;
  orderDate?: string;
  orderNumber?: string;
}

export interface PurchaseInvoiceDto extends FullAuditedEntityDto<string> {
  invoiceNumber?: string;
  supplierId?: string;
  supplierName?: string;
  purchaseOrderId?: string | null;
  purchaseOrderNumber?: string;
  invoiceDate?: string;
  totalAmount?: number;
  taxAmount?: number;
  discountAmount?: number;
  netAmount?: number;
  status?: PurchaseInvoiceStatus;
  notes?: string;
  lines?: PurchaseInvoiceLineDto[];
}

export interface PurchaseInvoiceLineDto extends EntityDto<string> {
  productId?: string;
  productName?: string;
  quantity?: number;
  unitCost?: number;
  discount?: number;
  totalLineAmount?: number;
  batchNumber?: string;
  expiryDate?: string | null;
}

export interface PurchaseOrderDto extends AuditedEntityDto<string> {
  orderNumber?: string;
  supplierId?: string;
  supplierName?: string;
  orderDate?: string;
  expectedDeliveryDate?: string | null;
  status?: PurchaseOrderStatus;
  referenceNumber?: string;
  notes?: string;
  totalAmount?: number;
  purchaseOrderLines?: PurchaseOrderLineDto[];
}

export interface PurchaseOrderLineDto extends EntityDto<string> {
  purchaseOrderId?: string;
  productId?: string;
  productName?: string;
  quantity?: number;
  unitPrice?: number;
  discount?: number;
  totalAmount?: number;
  description?: string;
}

export interface PurchaseRequisitionDto extends FullAuditedEntityDto<string> {
  requisitionNumber?: string;
  requestorId?: string;
  requestorName?: string;
  departmentId?: string;
  departmentName?: string;
  requiredDate?: string;
  status?: PurchaseRequisitionStatus;
  notes?: string | null;
  lines?: PurchaseRequisitionLineDto[];
}

export interface PurchaseRequisitionLineDto extends EntityDto<string> {
  productId?: string;
  productName?: string;
  quantity?: number;
  description?: string | null;
}

export interface ReceiveStockDto {
  warehouseId: string;
  productId: string;
  productName?: string;
  type?: InventoryItemType;
  quantity: number;
  unitCost: number;
  supplierId?: string | null;
  referenceNumber?: string;
}

export interface StagnantStockReportDto {
  productId?: string;
  productName?: string;
  warehouseName?: string;
  currentQuantity?: number;
  lastTransactionDate?: string | null;
  daysStagnant?: number;
}

export interface SupplierDto extends AuditedEntityDto<string> {
  name?: string;
  contactPerson?: string;
  phone?: string;
  email?: string;
  address?: string;
  taxId?: string;
}

export interface UpdateStockLevelsDto {
  minStockLevel?: number;
  reorderLevel?: number;
}

export interface WarehouseDto extends AuditedEntityDto<string> {
  name?: string;
  code?: string;
  location?: string;
}
