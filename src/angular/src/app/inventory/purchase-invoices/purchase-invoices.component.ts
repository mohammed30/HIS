import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, FormArray, Validators } from '@angular/forms';
import { CoreModule, PagedResultDto } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { finalize } from 'rxjs/operators';
import { PurchaseInvoiceService, PurchaseInvoiceStatus, SupplierService, InventoryService } from '../../proxy/inventory';
import { PurchaseInvoiceDto } from '../../proxy/inventory/dtos/models';
import { ServiceItemService } from '../../proxy/services';
import { DrugService } from '../../proxy/pharmacy';
import { forkJoin } from 'rxjs';

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
  products: any[] = [];

  constructor(
    private service: PurchaseInvoiceService,
    private supplierService: SupplierService,
    private inventoryService: InventoryService,
    private serviceItemService: ServiceItemService,
    private drugService: DrugService,
    private fb: FormBuilder
  ) {}

  ngOnInit(): void {
    this.getList();
    this.loadLookups();
  }

  loadLookups() {
    this.supplierService.getList({ maxResultCount: 100, skipCount: 0, sorting: '' }).subscribe(res => this.suppliers = res.items);
    this.inventoryService.getWarehouseList({ maxResultCount: 100, skipCount: 0, sorting: '' }).subscribe(res => this.warehouses = res.items);
    
    // Load both ServiceItems and Drugs, then combine them
    forkJoin({
      services: this.serviceItemService.getList({ maxResultCount: 1000, skipCount: 0, sorting: '' }),
      drugs: this.drugService.getList({ maxResultCount: 1000, skipCount: 0, sorting: '' })
    }).subscribe(res => {
      // For drugs, we might want to map brandName or scientificName as name, and barcode as code
      const mappedDrugs = res.drugs.items.map((d: any) => ({
        id: d.id, // Using drug.id because inventory tracks drugs by their drug ID
        name: (d.brandName || d.scientificName) + ' (دواء)',
        code: d.barcode || 'MED-' + d.id.substring(0,4).toUpperCase()
      }));
      
      const mappedServices = res.services.items.map((s: any) => ({
        id: s.id,
        name: s.name + ' (خدمة/مستلزم)',
        code: s.code
      }));

      this.products = [...mappedDrugs, ...mappedServices];
      // Sort alphabetically
      this.products.sort((a, b) => (a.name || '').localeCompare(b.name || ''));
    });
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
    const lineForm = this.fb.group({
      productDisplay: [''],
      productId: [null, Validators.required],
      quantity: [1, [Validators.required, Validators.min(1)]],
      unitCost: [0, [Validators.required, Validators.min(0)]],
      margin: [0, Validators.min(0)],
      salePrice: [{value: 0, disabled: true}],
      batchNumber: [''],
      expiryDate: [null]
    });

    lineForm.valueChanges.subscribe(val => {
      const cost = val.unitCost || 0;
      const margin = val.margin || 0;
      const calculatedSalePrice = cost + (cost * margin / 100);
      
      if (lineForm.get('salePrice').value !== calculatedSalePrice) {
        lineForm.patchValue({ salePrice: calculatedSalePrice }, { emitEvent: false });
      }
    });

    this.lines.push(lineForm);
  }

  removeLine(index: number) {
    this.lines.removeAt(index);
  }

  onProductSelect(event: any, index: number) {
    const value = event.target.value;
    const line = this.lines.at(index);
    const product = this.products.find(p => `${p.name} - ${p.code}` === value || p.name === value || p.code === value);
    if (product) {
      line.get('productId').setValue(product.id);
    } else {
      line.get('productId').setValue(null);
    }
  }

  createInvoice() {
    this.buildForm();
    this.addLine();
    this.isModalOpen = true;
  }

  save() {
    // Enable disabled fields before getting value so they are included
    this.form.enable();
    const request = this.form.value;

    this.service.create(request).subscribe(() => {
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
