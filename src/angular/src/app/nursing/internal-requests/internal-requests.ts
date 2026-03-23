import { Component, OnInit } from '@angular/core';
import { InternalRequestService, InternalRequestStatus } from '../../proxy/inventory';
import { InternalRequestDto, CreateUpdateInternalRequestDto } from '../../proxy/inventory/dtos/models';
import { PagedResultDto, CoreModule } from '@abp/ng.core';
import { finalize } from 'rxjs/operators';
import { ConfirmationService, Confirmation, ThemeSharedModule } from '@abp/ng.theme.shared';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, FormArray, Validators } from '@angular/forms';
import { InventoryService } from '../../proxy/inventory/inventory.service';
import { InventoryItemDto } from '../../proxy/inventory/dtos/models';
@Component({
  selector: 'app-nursing-internal-requests',
  imports: [CommonModule, FormsModule, ReactiveFormsModule, CoreModule, ThemeSharedModule],
  templateUrl: './internal-requests.html',
  styleUrl: './internal-requests.scss'
})
export class InternalRequestsComponent implements OnInit {
  data: PagedResultDto<InternalRequestDto> = { items: [], totalCount: 0 };
  isLoading = false;
  isModalOpen = false;
  form: FormGroup;
  statusEnum = InternalRequestStatus;
  
  availableItems: InventoryItemDto[] = [];
  warehouses: any[] = [];
  
  // Dummy departments / warehouses for MVP
  myDepartmentId = '00000000-0000-0000-0000-000000000000'; // To be replaced in real
  mainStoreId = '00000000-0000-0000-0000-000000000000'; // To be replaced in real

  constructor(
    private internalRequestService: InternalRequestService,
    private inventoryService: InventoryService,
    private confirmation: ConfirmationService,
    private fb: FormBuilder
  ) {}

  ngOnInit(): void {
    this.buildForm();
    this.getList();
    this.getWarehouses();
  }

  buildForm() {
    this.form = this.fb.group({
      requestingDepartmentId: [null, Validators.required],
      fulfilledByWarehouseId: [null, Validators.required],
      requestDate: [new Date().toISOString(), Validators.required],
      notes: [''],
      lines: this.fb.array([])
    });

    // When fulfillment warehouse changes, load items
    this.form.get('fulfilledByWarehouseId')?.valueChanges.subscribe(id => {
      if (id) this.loadInventoryItems(id);
    });
  }

  get lines() {
    return this.form.get('lines') as FormArray;
  }

  addLine() {
    this.lines.push(this.fb.group({
      inventoryItemId: [null, Validators.required],
      requestedQuantity: [1, [Validators.required, Validators.min(0.01)]],
      notes: ['']
    }));
  }

  removeLine(index: number) {
    this.lines.removeAt(index);
  }

  getList() {
    this.isLoading = true;
    // Real implementation would filter by requestingDepartmentId
    this.internalRequestService.getList({ maxResultCount: 10, skipCount: 0, sorting: '' })
      .pipe(finalize(() => this.isLoading = false))
      .subscribe((res) => {
        this.data = res;
      });
  }

  createRequest() {
    this.buildForm();
    this.addLine();
    this.isModalOpen = true;
  }

  save() {
    if (this.form.invalid) return;

    this.internalRequestService.create(this.form.value).subscribe(() => {
      this.isModalOpen = false;
      this.getList();
    });
  }

  submit(id: string) {
    this.internalRequestService.submitRequest(id).subscribe(() => {
      this.getList();
    });
  }

  receive(id: string) {
    this.internalRequestService.confirmReceipt(id).subscribe(() => {
      this.getList();
    });
  }

  getWarehouses() {
    this.inventoryService.getWarehouseList({ maxResultCount: 100, skipCount: 0, sorting: '' }).subscribe(res => {
      this.warehouses = res.items;
    });
  }

  loadInventoryItems(warehouseId: string) {
    this.inventoryService.getStockLevels(warehouseId).subscribe(res => {
      this.availableItems = res.items;
    });
  }

  getStatusText(status: number) {
    switch (status) {
      case InternalRequestStatus.Draft: return 'مسودة';
      case InternalRequestStatus.Submitted: return 'مرسل';
      case InternalRequestStatus.Approved: return 'معتمد';
      case InternalRequestStatus.Received: return 'مستلم';
      case InternalRequestStatus.Rejected: return 'مرفوض';
      default: return 'غير معروف';
    }
  }
}
