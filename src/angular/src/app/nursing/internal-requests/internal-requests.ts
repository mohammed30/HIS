import { Component, OnInit } from '@angular/core';
import { InternalRequestService, InternalRequestStatus } from '../../proxy/inventory';
import { InternalRequestDto, CreateUpdateInternalRequestDto } from '../../proxy/inventory/dtos/models';
import { PagedResultDto, CoreModule } from '@abp/ng.core';
import { finalize, debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { ConfirmationService, Confirmation, ThemeSharedModule } from '@abp/ng.theme.shared';
import { Observable, of, OperatorFunction } from 'rxjs';
import { NgbTypeaheadModule, NgbTypeaheadSelectItemEvent, NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { CommonModule } from '@angular/common';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, FormArray, Validators } from '@angular/forms';
import { InventoryService } from '../../proxy/inventory/inventory.service';
import { InventoryItemDto } from '../../proxy/inventory/dtos/models';
import { DepartmentService } from '../../proxy/settings/department.service';
import { LookupDto } from '../../proxy/settings/models';
import { AdmissionService } from '../../proxy/inpatient/admission.service';
import { AdmissionLookupDto } from '../../proxy/inpatient/models';

@Component({
  selector: 'app-nursing-internal-requests',
  imports: [CommonModule, FormsModule, ReactiveFormsModule, CoreModule, ThemeSharedModule, NgbTypeaheadModule, NgxDatatableModule, NgbDropdownModule],
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
  requestingDepartments: LookupDto[] = [];
  activeAdmissions: AdmissionLookupDto[] = [];
  pharmacyWarehouseId: string | null = null;
  
  // Dummy departments / warehouses for MVP
  myDepartmentId = '00000000-0000-0000-0000-000000000000'; // To be replaced in real
  mainStoreId = '00000000-0000-0000-0000-000000000000'; // To be replaced in real

  constructor(
    private internalRequestService: InternalRequestService,
    private inventoryService: InventoryService,
    private departmentService: DepartmentService,
    private admissionService: AdmissionService,
    private confirmation: ConfirmationService,
    private fb: FormBuilder
  ) {}

  ngOnInit(): void {
    this.buildForm();
    this.getList();
    this.getWarehouses();
    this.getDepartments();
    this.getActiveAdmissions();
  }

  buildForm() {
    this.form = this.fb.group({
      requestingDepartmentId: [null, Validators.required],
      fulfilledByWarehouseId: [null, Validators.required],
      admissionId: [null],
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
      selectedItem: [null], // Temporary to hold the object for typeahead
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
    if (this.pharmacyWarehouseId) {
      this.form.get('fulfilledByWarehouseId')?.setValue(this.pharmacyWarehouseId);
    }
    this.addLine();
    this.isModalOpen = true;
  }

  save() {
    if (this.form.invalid) return;

    // Clean data before sending (remove selectedItem helper)
    const requestData = { ...this.form.value };
    requestData.lines = this.lines.value.map(line => {
      const { selectedItem, ...rest } = line;
      return rest;
    });

    this.internalRequestService.create(requestData).subscribe(() => {
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
      
      const pharmacy = res.items.find(w => 
        w.name.toLowerCase().includes('pharmacy') || 
        w.name.includes('صيدلية')
      );

      if (pharmacy) {
        this.pharmacyWarehouseId = pharmacy.id;
        if (this.isModalOpen) {
          this.form.get('fulfilledByWarehouseId')?.setValue(pharmacy.id);
        }
      }
    });
  }

  getDepartments() {
    this.departmentService.getMedicalDepartmentsLookup().subscribe(res => {
      this.requestingDepartments = res;
    });
  }

  getActiveAdmissions() {
    this.admissionService.getActiveAdmissionsLookup().subscribe(res => {
      this.activeAdmissions = res;
    });
  }

  loadInventoryItems(warehouseId: string) {
    this.inventoryService.getStockLevels(warehouseId).subscribe(res => {
      this.availableItems = res.items;
    });
  }

  // Typeahead search function
  searchItems: OperatorFunction<string, readonly InventoryItemDto[]> = (text$: Observable<string>) =>
    text$.pipe(
      debounceTime(200),
      distinctUntilChanged(),
      map(term => term.length < 2 ? []
        : this.availableItems.filter(v => v.productName.toLowerCase().indexOf(term.toLowerCase()) > -1).slice(0, 10))
    );

  itemFormatter = (x: InventoryItemDto) => x.productName;

  onSelectItem(event: NgbTypeaheadSelectItemEvent, index: number) {
    const item = event.item as InventoryItemDto;
    this.lines.at(index).get('inventoryItemId')?.setValue(item.id);
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
