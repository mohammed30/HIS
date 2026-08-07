import { Component, OnInit } from '@angular/core';
import { InternalRequestService, InternalRequestStatus } from '../../proxy/inventory';
import { InternalRequestDto, CreateUpdateInternalRequestDto, ReturnInternalRequestDto } from '../../proxy/inventory/dtos/models';
import { PagedResultDto, CoreModule } from '@abp/ng.core';
import { finalize, debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { ConfirmationService, Confirmation, ThemeSharedModule, ToasterService } from '@abp/ng.theme.shared';
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
import { LabService } from '../../proxy/laboratory/lab.service';
import { ServiceItemService } from '../../proxy/services/service-item.service';

export enum InternalRequestType {
  Medication = 0,
  Consumable = 1,
  Laboratory = 2,
  Radiology = 3,
  Other = 4
}


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
  
  filterFromDate: string;
  filterToDate: string;
  filterText: string = '';
  
  activeAdmissions: AdmissionLookupDto[] = [];
  warehouses: any[] = [];
  requestingDepartments: LookupDto[] = [];
  pharmacyWarehouseId: string | null = null;

  // Return modal
  isReturnModalOpen = false;
  selectedReturnRequest: InternalRequestDto | null = null;
  returnLines: Array<{ inventoryItemId: string; inventoryItemName: string; originalQuantity: number; returnQuantity: number }> = [];
  returnNotes = '';
  returnPermission = 'HIS.Nursing.InternalRequestReturn';
  
  // Dynamic search data
  availableSearchData: any[] = [];
  catalogSearchTerm: string = '';
  requestTypeEnum = InternalRequestType;


  
  // Dummy departments / warehouses for MVP
  myDepartmentId = '00000000-0000-0000-0000-000000000000'; // To be replaced in real
  mainStoreId = '00000000-0000-0000-0000-000000000000'; // To be replaced in real

  constructor(
    private internalRequestService: InternalRequestService,
    private inventoryService: InventoryService,
    private departmentService: DepartmentService,
    private admissionService: AdmissionService,
    private labService: LabService,
    private serviceItemService: ServiceItemService,
    private confirmation: ConfirmationService,
    private fb: FormBuilder,
    private toaster: ToasterService
  ) {}

  ngOnInit(): void {
    const today = new Date().toISOString().split('T')[0];
    this.filterFromDate = today;
    this.filterToDate = today;
    
    this.buildForm();
    this.getList();
    this.getWarehouses();
    this.getDepartments();
    this.getActiveAdmissions();
  }

  buildForm() {
    this.form = this.fb.group({
      requestingDepartmentId: [null, Validators.required],
      fulfilledByWarehouseId: [null], // No longer strictly required if Lab/Rad
      admissionId: [null, Validators.required], // Required as per request
      requestType: [InternalRequestType.Medication, Validators.required],
      requestDate: [new Date().toISOString(), Validators.required],
      notes: [''],
      lines: this.fb.array([])
    });

    // Handle type change
    this.form.get('requestType')?.valueChanges.subscribe(type => {
      this.lines.clear();
      this.loadLookupData(type);
    });


    // When fulfillment warehouse changes, load items
    this.form.get('fulfilledByWarehouseId')?.valueChanges.subscribe(id => {
      if (id && this.form.get('requestType')?.value == InternalRequestType.Medication) {
        this.loadInventoryItems(id);
      }
    });

    // Initial load
    this.loadLookupData(InternalRequestType.Medication);
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

  get filteredCatalog() {
    if (!this.catalogSearchTerm) return this.availableSearchData;
    return this.availableSearchData.filter(v => 
      v.name.toLowerCase().indexOf(this.catalogSearchTerm.toLowerCase()) > -1
    );
  }

  addItemToRequest(item: any) {
    // Check if already added
    const exists = this.lines.value.some(l => l.inventoryItemId === item.id);
    if (exists) return;

    this.lines.insert(0, this.fb.group({
      inventoryItemId: [item.id, Validators.required],
      selectedItem: [item],
      requestedQuantity: [1, [Validators.required, Validators.min(0.01)]],
      notes: ['']
    }));
  }


  getList() {
    this.isLoading = true;
    this.internalRequestService.getList({ 
        maxResultCount: 10, 
        skipCount: 0, 
        sorting: '',
        fromDate: this.filterFromDate,
        toDate: this.filterToDate,
        filterText: this.filterText 
    } as any)
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
    
    // Set default requesting department if only one
    if (this.requestingDepartments.length === 1) {
      this.form.get('requestingDepartmentId')?.setValue(this.requestingDepartments[0].id);
    }

    this.catalogSearchTerm = '';
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

  cancel(id: string) {
    this.confirmation.warn('هل أنت متأكد من إلغاء هذا الطلب؟ سيتم حذف أي تكاليف مالية مرتبطة به.', 'تأكيد الإلغاء').subscribe((status) => {
      if (status === Confirmation.Status.confirm) {
        this.internalRequestService.cancelRequest(id).subscribe(() => {
          this.getList();
        });
      }
    });
  }

  openReturnModal(row: InternalRequestDto) {
    this.selectedReturnRequest = row;
    this.returnNotes = '';
    this.returnLines = (row.lines || []).map(l => ({
      inventoryItemId: l.inventoryItemId,
      inventoryItemName: l.inventoryItemName,
      originalQuantity: l.approvedQuantity,
      returnQuantity: 0
    }));
    this.isReturnModalOpen = true;
  }

  submitReturn() {
    if (!this.selectedReturnRequest) return;
    const hasAny = this.returnLines.some(l => l.returnQuantity > 0);
    if (!hasAny) return;

    const hasInvalid = this.returnLines.some(l => l.returnQuantity > l.originalQuantity);
    if (hasInvalid) {
      this.confirmation.warn('كمية الإرجاع لا يمكن أن تكون أكبر من الكمية المعتمدة للصنف.', 'تنبيه');
      return;
    }

    const input: ReturnInternalRequestDto = {
      requestId: this.selectedReturnRequest.id,
      lines: this.returnLines.filter(l => l.returnQuantity > 0),
      notes: this.returnNotes
    };

    this.internalRequestService.returnItems(input).subscribe(() => {
      this.isReturnModalOpen = false;
      this.toaster.success('تم إرسال طلب المرتجع بنجاح وهو بانتظار موافقة الصيدلية', 'مرتجع جديد');
      this.getList();
    });
  }

  get hasReturnQuantity(): boolean {
    const hasAny = this.returnLines.some(l => l.returnQuantity > 0);
    const hasInvalid = this.returnLines.some(l => l.returnQuantity > l.originalQuantity || l.returnQuantity < 0);
    return hasAny && !hasInvalid;
  }

  delete(id: string) {
    this.confirmation.warn('::DeletionConfirmationMessage', '::AreYouSure').subscribe((status) => {
      if (status === Confirmation.Status.confirm) {
        this.internalRequestService.delete(id).subscribe(() => {
          this.getList();
        });
      }
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
      this.requestingDepartments = res.filter(d => 
        d.name.includes('رجال') || d.name.includes('نساء')
      );
    });
  }

  getActiveAdmissions() {
    this.admissionService.getActiveAdmissionsLookup().subscribe(res => {
      this.activeAdmissions = res;
    });
  }

  loadInventoryItems(warehouseId: string) {
    this.inventoryService.getStockLevels(warehouseId).subscribe(res => {
      this.availableSearchData = res.items.map(x => ({ id: x.id, name: x.productName, extra: x.quantity }));
    });
  }

  loadLookupData(type: InternalRequestType) {
    const warehouseId = this.form.get('fulfilledByWarehouseId')?.value;
    
    if (type === InternalRequestType.Medication || type === InternalRequestType.Consumable) {
      if (warehouseId) {
        this.loadInventoryItems(warehouseId);
      }
    } else if (type === InternalRequestType.Laboratory) {
      this.labService.getTests({ maxResultCount: 1000, skipCount: 0, sorting: '' }).subscribe(res => {
        this.availableSearchData = res.items.map(x => ({ id: x.id, name: x.name, extra: x.price }));
      });
    } else if (type === InternalRequestType.Radiology) {
      this.serviceItemService.getRadiologyList({ maxResultCount: 1000, skipCount: 0, sorting: '' }).subscribe(res => {
        this.availableSearchData = res.items.map(x => ({ id: x.id, name: x.name, extra: x.price }));
      });
    }
  }

  // Typeahead search function
  searchItems: OperatorFunction<string, readonly any[]> = (text$: Observable<string>) =>
    text$.pipe(
      debounceTime(200),
      distinctUntilChanged(),
      map(term => term.length < 2 ? []
        : this.availableSearchData.filter(v => v.name.toLowerCase().indexOf(term.toLowerCase()) > -1).slice(0, 10))
    );

  itemFormatter = (x: any) => x.name;

  onSelectItem(event: NgbTypeaheadSelectItemEvent, index: number) {
    const item = event.item;
    this.lines.at(index).get('inventoryItemId')?.setValue(item.id);
  }


  getStatusText(status: number) {
    switch (status) {
      case InternalRequestStatus.Draft: return 'مسودة';
      case InternalRequestStatus.Submitted: return 'مرسل';
      case InternalRequestStatus.Approved: return 'معتمد';
      case InternalRequestStatus.Received: return 'مستلم';
      case InternalRequestStatus.Rejected: return 'مرفوض';
      case InternalRequestStatus.Cancelled: return 'ملغي';
      default: return 'غير معروف';
    }
  }

  getRequestTypeText(type: number) {
    switch (type) {
      case InternalRequestType.Medication: return 'أدوية';
      case InternalRequestType.Consumable: return 'مستلزمات';
      case InternalRequestType.Laboratory: return 'تحاليل';
      case InternalRequestType.Radiology: return 'أشعة';
      case InternalRequestType.Other: return 'أخرى';
      default: return 'غير معروف';
    }
  }

  getRequestTypeClass(type: number) {
    switch (type) {
      case InternalRequestType.Medication: return 'bg-success';
      case InternalRequestType.Consumable: return 'bg-info';
      case InternalRequestType.Laboratory: return 'bg-primary';
      case InternalRequestType.Radiology: return 'bg-danger';
      default: return 'bg-secondary';
    }
  }
}
