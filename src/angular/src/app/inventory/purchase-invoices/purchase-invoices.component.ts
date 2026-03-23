import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, FormArray, Validators } from '@angular/forms';
import { CoreModule, PagedResultDto } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { finalize } from 'rxjs/operators';
import { PurchaseInvoiceService, PurchaseInvoiceStatus, SupplierService, InventoryService } from '../../proxy/inventory';
import { PurchaseInvoiceDto } from '../../proxy/inventory/dtos/models';

@Component({
  selector: 'app-purchase-invoices',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, CoreModule, ThemeSharedModule],
  templateUrl: './purchase-invoices.component.html',
  styleUrl: './purchase-invoices.component.scss'
})
export class PurchaseInvoicesComponent implements OnInit {
  data: PagedResultDto<PurchaseInvoiceDto> = { items: [], totalCount: 0 };
  isLoading = false;
  isModalOpen = false;
  form: FormGroup;
  statusEnum = PurchaseInvoiceStatus;
  
  suppliers: any[] = [];
  warehouses: any[] = [];

  constructor(
    private service: PurchaseInvoiceService,
    private supplierService: SupplierService,
    private inventoryService: InventoryService,
    private fb: FormBuilder
  ) {}

  ngOnInit(): void {
    this.getList();
    this.loadLookups();
  }

  loadLookups() {
    this.supplierService.getList({ maxResultCount: 100, skipCount: 0, sorting: '' }).subscribe(res => this.suppliers = res.items);
    this.inventoryService.getWarehouseList({ maxResultCount: 100, skipCount: 0, sorting: '' }).subscribe(res => this.warehouses = res.items);
  }

  getList() {
    this.isLoading = true;
    this.service.getList({ maxResultCount: 10, skipCount: 0, sorting: '' })
      .pipe(finalize(() => this.isLoading = false))
      .subscribe(res => this.data = res);
  }

  buildForm() {
    this.form = this.fb.group({
      invoiceNumber: ['', Validators.required],
      supplierId: [null, Validators.required],
      invoiceDate: [new Date().toISOString().substring(0, 10), Validators.required],
      notes: [''],
      lines: this.fb.array([])
    });
  }

  get lines() {
    return this.form.get('lines') as FormArray;
  }

  addLine() {
    this.lines.push(this.fb.group({
      productId: [null, Validators.required],
      quantity: [1, [Validators.required, Validators.min(0.01)]],
      unitCost: [0, [Validators.required, Validators.min(0)]],
      discount: [0],
      batchNumber: [''],
      expiryDate: [null]
    }));
  }

  removeLine(index: number) {
    this.lines.removeAt(index);
  }

  createInvoice() {
    this.buildForm();
    this.addLine();
    this.isModalOpen = true;
  }

  save() {
    if (this.form.invalid) return;
    this.service.create(this.form.value).subscribe(() => {
      this.isModalOpen = false;
      this.getList();
    });
  }

  post(id: string) {
    // For demo, we use the first warehouse or ask user
    const warehouseId = this.warehouses[0]?.id; 
    if (!warehouseId) {
        return;
    }
    this.service.postInvoice(id, warehouseId).subscribe(() => {
      this.getList();
    });
  }

  getStatusText(status: number) {
    switch (status) {
      case PurchaseInvoiceStatus.Draft: return 'مسودة';
      case PurchaseInvoiceStatus.Posted: return 'مُرحلة (مُستلمة)';
      case PurchaseInvoiceStatus.Cancelled: return 'ملغاة';
      default: return 'غير معروف';
    }
  }
}
