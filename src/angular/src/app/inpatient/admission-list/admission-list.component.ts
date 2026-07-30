import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CoreModule } from '@abp/ng.core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ThemeSharedModule, ToasterService } from '@abp/ng.theme.shared';
import { NgbModule, NgbModal } from '@ng-bootstrap/ng-bootstrap';

import { AdmissionService } from '@proxy/inpatient';
import { AdmissionDto } from '@proxy/inpatient/models';
import { roomTypeOptions, RoomService } from '@proxy/rooms';
import { RoomLookupDto, BedDto } from '@proxy/rooms/models';
import { InpatientDepositService } from '@proxy/billing';
import { PatientService } from '@proxy/patients';
import { PatientDto } from '@proxy/patients/models';
import { MedicalOrderService } from '@proxy/clinical';
import { OrderType } from '@proxy/clinical/order-type.enum';
import { ServiceItemService } from '@proxy/services';
import { PatientInsuranceService } from '@proxy/insurance';
import { PatientInsuranceDto } from '@proxy/insurance/models';

import { CreditLimitGaugeComponent } from '../../shared/components/credit-limit-gauge/credit-limit-gauge.component';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-admission-list',
  standalone: true,
  imports: [
    CommonModule,
    CoreModule,
    ThemeSharedModule,
    ReactiveFormsModule,
    NgbModule,
    CreditLimitGaugeComponent
  ],
  templateUrl: './admission-list.component.html',
  styleUrls: ['./admission-list.component.scss']
})
export class AdmissionListComponent implements OnInit {
  private admissionService = inject(AdmissionService);
  private roomService = inject(RoomService);
  private depositService = inject(InpatientDepositService);
  private patientService = inject(PatientService);
  private fb = inject(FormBuilder);
  private toaster = inject(ToasterService);
  private modalService = inject(NgbModal);
  private medicalOrderService = inject(MedicalOrderService);
  private serviceItemService = inject(ServiceItemService);
  private patientInsuranceService = inject(PatientInsuranceService);
  private http = inject(HttpClient);

  admissions: AdmissionDto[] = [];
  selectedAdmission: AdmissionDto | null = null;
  roomTypes = roomTypeOptions;
  provisionalInvoice: any = null;

  defaultCreditLimit: number = 1000;
  creditLimitSettingsLoaded: boolean = false;

  patients: PatientDto[] = [];
  filteredPatients: PatientDto[] = [];
  patientSearchTerm = '';

  availableRooms: RoomLookupDto[] = [];
  availableBedsList: BedDto[] = [];
  patientInsurances: PatientInsuranceDto[] = [];

  transferRooms: RoomLookupDto[] = [];
  transferBeds: BedDto[] = [];

  filterForm = this.fb.group({
    roomTypeId: [null as number | null],
    searchText: ['']
  });

  detailForm = this.fb.group({
    companionName: [''],
    companionPhone: [''],
    companionAddress: [''],
    insuranceCeiling: [0],
    isServicesStopped: [false],
    pharmacyPercentage: [0],
    purpose: [''],
    notes: [''],
    numberOfDays: [0],
    paidAmount: [0],
    patientInsuranceId: [null as string | null]
  });

  dischargeForm = this.fb.group({
    dischargeDate: [new Date().toISOString().substring(0, 16)], // "YYYY-MM-DDThh:mm" format for datetime-local
    notes: ['']
  });

  depositForm = this.fb.group({
    amount: [0],
    paymentMethod: [0], // Cash by default
    referenceNumber: [''],
    notes: ['']
  });

  transferForm = this.fb.group({
    toRoomTypeId: [null as number | null],
    toRoomId: [null as string | null],
    toBedId: [null as string | null],
    reason: ['']
  });

  newAdmissionForm = this.fb.group({
    patientId: [null as string | null, Validators.required],
    roomTypeId: [null as number | null, Validators.required],
    roomId: [null as string | null, Validators.required],
    bedId: [null as string | null, Validators.required],
    companionName: [''],
    companionPhone: [''],
    companionAddress: [''],
    insuranceCeiling: [0],
    isServicesStopped: [false],
    pharmacyPercentage: [0],
    purpose: [''],
    notes: [''],
    numberOfDays: [0],
    paidAmount: [0],
    patientInsuranceId: [null as string | null]
  });

  paymentMethods = [
    { value: 0, key: 'Cash' },
    { value: 1, key: 'Card' },
    { value: 2, key: 'Bank Transfer' }
  ];

  consumableForm = this.fb.group({
    serviceItemId: [null as string | null, Validators.required],
    quantity: [1, [Validators.required, Validators.min(1)]],
    clinicalNotes: ['']
  });

  labForm = this.fb.group({
    serviceItemId: [null as string | null, Validators.required],
    clinicalNotes: ['']
  });

  radiologyForm = this.fb.group({
    serviceItemId: [null as string | null, Validators.required],
    clinicalNotes: ['']
  });

  consumableItems: any[] = [];
  filteredConsumableItems: any[] = [];
  consumableSearchTerm = '';

  labItems: any[] = [];
  filteredLabItems: any[] = [];
  labSearchTerm = '';

  radiologyItems: any[] = [];
  filteredRadiologyItems: any[] = [];
  radiologySearchTerm = '';

  ngOnInit() {
    this.loadAdmissions();
    
    // Load inpatient settings for credit limit
    this.http.get<any>(environment.apis.default.url + '/api/app/inpatient-settings').subscribe({
      next: (res) => {
        this.defaultCreditLimit = res.admissionDepositAmount || 1000;
        this.creditLimitSettingsLoaded = true;
      },
      error: () => {
        this.defaultCreditLimit = 1000;
        this.creditLimitSettingsLoaded = true;
      }
    });
  }

  loadAdmissions() {
    const filter = this.filterForm.value;
    this.admissionService.getList({
      maxResultCount: 100,
      roomTypeId: filter.roomTypeId ?? undefined,
      searchText: filter.searchText ?? undefined
    }).subscribe(result => {
      this.admissions = result.items || [];
      if (this.selectedAdmission) {
        const found = this.admissions.find(x => x.id === this.selectedAdmission.id);
        if (found) {
          this.selectAdmission(found);
        } else {
          this.selectedAdmission = null;
        }
      }
    });
  }

  selectAdmission(admission: AdmissionDto) {
    this.selectedAdmission = admission;
    this.detailForm.patchValue({
      companionName: admission.companionName || '',
      companionPhone: admission.companionPhone || '',
      companionAddress: admission.companionAddress || '',
      insuranceCeiling: admission.insuranceCeiling || 0,
      isServicesStopped: admission.isServicesStopped || false,
      pharmacyPercentage: admission.pharmacyPercentage || 0,
      purpose: admission.purpose || '',
      notes: admission.notes || '',
      patientInsuranceId: admission.patientInsuranceId || null
    });
    this.detailForm.markAsPristine();
    if (admission.patientId) {
      this.patientInsuranceService.getByPatient(admission.patientId).subscribe(res => {
        this.patientInsurances = res || [];
      });
    }
  }

  saveDetails() {
    if (!this.selectedAdmission) return;

    const formVal = this.detailForm.value;
    const updateDto = {
      ...this.selectedAdmission,
      ...formVal,
      patientId: this.selectedAdmission.patientId,
      roomId: this.selectedAdmission.roomId,
      bedId: this.selectedAdmission.bedId
    } as any;

    this.admissionService.update(this.selectedAdmission.id, updateDto).subscribe(updated => {
      this.toaster.success('::SuccessfullySaved');
      this.selectAdmission(updated);
      const index = this.admissions.findIndex(x => x.id === updated.id);
      if (index > -1) {
        this.admissions[index] = updated;
      }
    });
  }

  openDischargeModal(content: any) {
    if (!this.selectedAdmission) return;
    this.dischargeForm.reset({
      dischargeDate: new Date().toISOString().substring(0, 16),
      notes: ''
    });
    this.admissionService.getProvisionalInvoice(this.selectedAdmission.id).subscribe((res) => {
      this.provisionalInvoice = res;
      this.modalService.open(content, { centered: true });
    });
  }

  openProvisionalInvoiceModal(content: any) {
    if (!this.selectedAdmission) return;
    this.admissionService.getProvisionalInvoice(this.selectedAdmission.id).subscribe((res) => {
      this.provisionalInvoice = res;
      this.modalService.open(content, { centered: true, size: 'lg' });
    });
  }

  confirmDischarge() {
    if (!this.selectedAdmission || this.dischargeForm.invalid) return;

    const formVal = this.dischargeForm.value;
    this.admissionService.discharge(this.selectedAdmission.id, {
      dischargeDate: formVal.dischargeDate as string,
      notes: formVal.notes || ''
    }).subscribe(updated => {
      this.toaster.success('::SuccessfullyDischarged');
      this.modalService.dismissAll();
      this.selectAdmission(updated);
      const index = this.admissions.findIndex(x => x.id === updated.id);
      if (index > -1) {
        this.admissions[index] = updated;
      }
      
      // Auto-print invoice if required or just show notification
      if (updated.invoiceId) {
        this.toaster.info('تم إنشاء الفاتورة النهائية للرقم المرجعي ' + updated.invoiceId, 'تنويه');
      }
    });
  }

  openDepositModal(content: any) {
    if (!this.selectedAdmission) return;
    this.depositForm.reset({
      amount: 0,
      paymentMethod: 0,
      referenceNumber: '',
      notes: ''
    });
    this.modalService.open(content, { centered: true });
  }

  confirmDeposit() {
    if (!this.selectedAdmission || this.depositForm.invalid) return;

    const formVal = this.depositForm.value;
    const input: any = {
      patientId: this.selectedAdmission.patientId,
      admissionId: this.selectedAdmission.id,
      amount: formVal.amount || 0,
      paymentMethod: formVal.paymentMethod || 0,
      referenceNumber: formVal.referenceNumber || '',
      notes: formVal.notes || ''
    };

    this.depositService.create(input).subscribe(() => {
      this.toaster.success('::SuccessfullyDeposited');
      this.modalService.dismissAll();
      this.loadAdmissions(); // Refresh to show new PaidAmount
    });
  }

  openTransferModal(content: any) {
    if (!this.selectedAdmission) return;
    this.transferForm.reset();
    this.transferRooms = [];
    this.transferBeds = [];
    this.modalService.open(content, { centered: true });
  }

  onTransferRoomTypeChange() {
    this.transferForm.patchValue({ toRoomId: null, toBedId: null });
    this.transferRooms = [];
    this.transferBeds = [];

    const type = this.transferForm.value.toRoomTypeId;
    if (type !== null && type !== undefined) {
      this.roomService.getAvailableRooms(type as any).subscribe(res => {
        this.transferRooms = res;
      });
    }
  }

  onTransferRoomChange() {
    this.transferForm.patchValue({ toBedId: null });
    this.transferBeds = [];

    const roomId = this.transferForm.value.toRoomId;
    if (roomId) {
      this.roomService.get(roomId).subscribe(room => {
        this.transferBeds = room.beds?.filter(b => b.status === 0) || []; // Available = 0
      });
    }
  }

  confirmTransfer() {
    if (!this.selectedAdmission || this.transferForm.invalid) return;

    const formVal = this.transferForm.value;
    if (!formVal.toRoomId || !formVal.toBedId) return;

    this.admissionService.transferPatient(this.selectedAdmission.id, {
      toRoomId: formVal.toRoomId,
      toBedId: formVal.toBedId,
      reason: formVal.reason || ''
    }).subscribe(updated => {
      this.toaster.success('::SuccessfullyTransferred');
      this.modalService.dismissAll();
      this.selectAdmission(updated);
      const index = this.admissions.findIndex(x => x.id === updated.id);
      if (index > -1) {
        this.admissions[index] = updated;
      }
    });
  }

  getStatusName(status: number): string {
    switch (status) {
      case 0: return '::Enum:AdmissionStatus.0';
      case 1: return '::Enum:AdmissionStatus.1';
      case 2: return '::Enum:AdmissionStatus.2';
      default: return '::Unknown';
    }
  }

  getRoomTypeName(typeValue: any): string {
    const val = Number(typeValue);
    switch (val) {
      case 0: return 'عادي';
      case 1: return 'خاص';
      case 2: return 'عناية مركزة';
      case 3: return 'جناح';
      case 4: return 'عزل';
      default: return 'غير معروف';
    }
  }

  getStatusSeverityClass(status: number | undefined): string {
    switch (status) {
      case 0: return 'bg-info text-dark';
      case 1: return 'bg-secondary';
      case 2: return 'bg-danger';
      default: return 'bg-light';
    }
  }

  // New Admission Methods
  openNewAdmissionModal(content: any) {
    this.newAdmissionForm.reset({
      patientId: null,
      roomTypeId: null,
      roomId: null,
      bedId: null,
      companionName: '',
      companionPhone: '',
      companionAddress: '',
      insuranceCeiling: 0,
      isServicesStopped: false,
      pharmacyPercentage: 0,
      purpose: '',
      notes: '',
      numberOfDays: 0,
      paidAmount: this.defaultCreditLimit
    });
    this.availableRooms = [];
    this.availableBedsList = [];
    this.patientSearchTerm = '';

    // Load patients for selection
    this.patientService.getList({ maxResultCount: 1000 }).subscribe(res => {
      this.patients = res.items || [];
      this.filteredPatients = [...this.patients];
    });

    this.modalService.open(content, { centered: true, size: 'lg' });
  }

  filterPatients() {
    const term = this.patientSearchTerm.toLowerCase();
    if (!term) {
      this.filteredPatients = [...this.patients];
      return;
    }
    this.filteredPatients = this.patients.filter(p =>
      (p.fullNameAr && p.fullNameAr.toLowerCase().includes(term)) ||
      (p.mrn && p.mrn.toLowerCase().includes(term))
    );
  }

  onPatientSelect(patient: PatientDto) {
    this.newAdmissionForm.get('patientId')?.setValue(patient.id);
    this.patientSearchTerm = patient.fullNameAr || '';
    this.patientInsurances = [];
    this.patientInsuranceService.getByPatient(patient.id).subscribe(res => {
      this.patientInsurances = res || [];
      if (this.patientInsurances.length > 0) {
          const primary = this.patientInsurances.find(x => x.isPrimary) || this.patientInsurances[0];
          this.newAdmissionForm.get('patientInsuranceId')?.setValue(primary.id);
      }
    });
  }

  onNewAdmissionRoomTypeChange() {
    this.newAdmissionForm.patchValue({ roomId: null, bedId: null });
    this.availableRooms = [];
    this.availableBedsList = [];

    const type = this.newAdmissionForm.value.roomTypeId;
    if (type !== null && type !== undefined) {
      this.roomService.getAvailableRooms(type as any).subscribe(res => {
        this.availableRooms = res;
      });
    }
  }

  onNewAdmissionRoomChange() {
    this.newAdmissionForm.patchValue({ bedId: null });
    this.availableBedsList = [];

    const roomId = this.newAdmissionForm.value.roomId;
    if (roomId) {
      this.roomService.get(roomId).subscribe(room => {
        this.availableBedsList = room.beds?.filter(b => b.status === 0) || [];
      });
    }
  }

  confirmNewAdmission() {
    if (this.newAdmissionForm.invalid) return;

    this.admissionService.create(this.newAdmissionForm.value as any).subscribe(() => {
      this.toaster.success('::SuccessfullySaved');
      this.modalService.dismissAll();
      this.loadAdmissions();
    });
  }

  // Consumables Ordering
  openConsumableModal(content: any) {
    if (!this.selectedAdmission) return;
    this.consumableForm.reset({ serviceItemId: null, quantity: 1, clinicalNotes: '' });
    this.consumableSearchTerm = '';
    this.filteredConsumableItems = [];

    // Load consumable service items (category = 7 = Consumable)
    this.serviceItemService.getList({ maxResultCount: 500 } as any).subscribe(res => {
      this.consumableItems = (res.items || []).filter((item: any) => item.category === 7);
      this.filteredConsumableItems = [...this.consumableItems];
    });

    this.modalService.open(content, { centered: true });
  }

  filterConsumables() {
    const term = this.consumableSearchTerm.toLowerCase();
    if (!term) {
      this.filteredConsumableItems = [...this.consumableItems];
      return;
    }
    this.filteredConsumableItems = this.consumableItems.filter((item: any) =>
      (item.name && item.name.toLowerCase().includes(term)) ||
      (item.code && item.code.toLowerCase().includes(term))
    );
  }

  confirmConsumableOrder() {
    if (!this.selectedAdmission || this.consumableForm.invalid) return;

    const formVal = this.consumableForm.value;
    this.medicalOrderService.create({
      patientId: this.selectedAdmission.patientId,
      admissionId: this.selectedAdmission.id,
      serviceItemId: formVal.serviceItemId!,
      type: OrderType.Consumable,
      quantity: formVal.quantity || 1,
      clinicalNotes: formVal.clinicalNotes || ''
    } as any).subscribe(() => {
      this.toaster.success('::SuccessfullySaved');
      this.modalService.dismissAll();
    });
  }

  // Lab Ordering
  openLabModal(content: any) {
    if (!this.selectedAdmission) return;
    this.labForm.reset({ serviceItemId: null, clinicalNotes: '' });
    this.labSearchTerm = '';
    this.filteredLabItems = [];

    // Load Lab service items (category = 2 = LabTest)
    this.serviceItemService.getList({ maxResultCount: 500 } as any).subscribe(res => {
      this.labItems = (res.items || []).filter((item: any) => item.category === 2);
      this.filteredLabItems = [...this.labItems];
    });

    this.modalService.open(content, { centered: true });
  }

  filterLab() {
    const term = this.labSearchTerm.toLowerCase();
    if (!term) {
      this.filteredLabItems = [...this.labItems];
      return;
    }
    this.filteredLabItems = this.labItems.filter((item: any) =>
      (item.name && item.name.toLowerCase().includes(term)) ||
      (item.code && item.code.toLowerCase().includes(term))
    );
  }

  confirmLabOrder() {
    if (!this.selectedAdmission || this.labForm.invalid) return;

    const formVal = this.labForm.value;
    this.medicalOrderService.create({
      patientId: this.selectedAdmission.patientId,
      admissionId: this.selectedAdmission.id,
      serviceItemId: formVal.serviceItemId!,
      type: OrderType.Lab,
      quantity: 1,
      clinicalNotes: formVal.clinicalNotes || ''
    } as any).subscribe(() => {
      this.toaster.success('::SuccessfullySaved');
      this.modalService.dismissAll();
    });
  }

  // Radiology Ordering
  openRadiologyModal(content: any) {
    if (!this.selectedAdmission) return;
    this.radiologyForm.reset({ serviceItemId: null, clinicalNotes: '' });
    this.radiologySearchTerm = '';
    this.filteredRadiologyItems = [];

    // Load Radiology service items (category = 3 = Radiology)
    this.serviceItemService.getList({ maxResultCount: 500 } as any).subscribe(res => {
      this.radiologyItems = (res.items || []).filter((item: any) => item.category === 3);
      this.filteredRadiologyItems = [...this.radiologyItems];
    });

    this.modalService.open(content, { centered: true });
  }

  filterRadiology() {
    const term = this.radiologySearchTerm.toLowerCase();
    if (!term) {
      this.filteredRadiologyItems = [...this.radiologyItems];
      return;
    }
    this.filteredRadiologyItems = this.radiologyItems.filter((item: any) =>
      (item.name && item.name.toLowerCase().includes(term)) ||
      (item.code && item.code.toLowerCase().includes(term))
    );
  }

  confirmRadiologyOrder() {
    if (!this.selectedAdmission || this.radiologyForm.invalid) return;

    const formVal = this.radiologyForm.value;
    this.medicalOrderService.create({
      patientId: this.selectedAdmission.patientId,
      admissionId: this.selectedAdmission.id,
      serviceItemId: formVal.serviceItemId!,
      type: OrderType.Radiology,
      quantity: 1,
      clinicalNotes: formVal.clinicalNotes || ''
    } as any).subscribe(() => {
      this.toaster.success('::SuccessfullySaved');
      this.modalService.dismissAll();
    });
  }
}
