import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CoreModule, ListService, PagedResultDto } from '@abp/ng.core';
import { ThemeSharedModule, ConfirmationService, Confirmation } from '@abp/ng.theme.shared';
import { SurgicalOperationService } from '../../proxy/operations/surgical-operation.service';
import { SurgicalOperationDto, CreateUpdateSurgicalOperationDto } from '../../proxy/operations/models';
import { PatientService } from '../../proxy/patients/patient.service';
import { PatientDto } from '../../proxy/patients/models';
import { DoctorService } from '../../proxy/settings/doctor.service';
import { DoctorDto } from '../../proxy/settings/models';
import { NgbDatepickerModule, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { OperationStatus } from '../../proxy/operations/operation-status.enum';

@Component({
  selector: 'app-surgical-operations',
  standalone: true,
  imports: [
    CoreModule,
    CommonModule,
    ReactiveFormsModule,
    ThemeSharedModule,
    NgbDatepickerModule
  ],
  providers: [ListService],
  templateUrl: './surgical-operations.html',
  styleUrls: ['./surgical-operations.scss']
})
export class SurgicalOperations implements OnInit {
  operations: SurgicalOperationDto[] = [];
  patients: PatientDto[] = [];
  filteredPatients: PatientDto[] = [];
  doctors: DoctorDto[] = [];
  form: FormGroup;
  selectedTask = {} as SurgicalOperationDto;
  isModalOpen = false;
  patientSearchTerm = '';
  listSearchTerm = '';

  operationStatusOptions = [
    { value: OperationStatus.Scheduled, key: '::Enum:OperationStatus.0' },
    { value: OperationStatus.InProgress, key: '::Enum:OperationStatus.1' },
    { value: OperationStatus.Completed, key: '::Enum:OperationStatus.2' },
    { value: OperationStatus.Cancelled, key: '::Enum:OperationStatus.3' }
  ];

  constructor(
    public readonly list: ListService<SurgicalOperationInput>,
    private operationService: SurgicalOperationService,
    private patientService: PatientService,
    private doctorService: DoctorService,
    private fb: FormBuilder,
    private confirmation: ConfirmationService,
    private modalService: NgbModal
  ) { }

  ngOnInit() {
    this.loadDropdowns();
    const streamCreator = (query) => this.operationService.getList(query);
    this.list.hookToQuery(streamCreator).subscribe((res) => {
      this.operations = res.items;
      this.applyFilter();
    });
  }

  filteredOperations: SurgicalOperationDto[] = [];
  
  applyFilter() {
    if (!this.listSearchTerm) {
      this.filteredOperations = [...this.operations];
      return;
    }
    const term = this.listSearchTerm.toLowerCase();
    this.filteredOperations = this.operations.filter(op => 
      op.patientName?.toLowerCase().includes(term) || 
      op.operationName?.toLowerCase().includes(term) ||
      op.doctorName?.toLowerCase().includes(term)
    );
  }

  onSearch(event: any) {
    this.listSearchTerm = event.target.value;
    this.applyFilter();
  }

  loadDropdowns() {
    this.patientService.getList({ maxResultCount: 1000 }).subscribe(res => {
      this.patients = res.items || [];
      this.filteredPatients = [...this.patients];
    });
    this.doctorService.getList({ maxResultCount: 1000 }).subscribe(res => {
      this.doctors = res.items || [];
    });
  }

  filterPatients(term: string) {
    this.patientSearchTerm = term;
    if (!term) {
      this.filteredPatients = [...this.patients];
      return;
    }
    const lowerTerm = term.toLowerCase();
    this.filteredPatients = this.patients.filter(p =>
      (p.fullNameAr?.toLowerCase().includes(lowerTerm)) ||
      (p.mrn?.toLowerCase().includes(lowerTerm))
    );
  }

  createOperation(modalHtml: any) {
    this.selectedTask = {} as SurgicalOperationDto;
    this.buildForm();
    this.modalService.open(modalHtml, { size: 'lg' });
  }

  editOperation(id: string, modalHtml: any) {
    this.operationService.get(id).subscribe((res) => {
      this.selectedTask = res;
      this.buildForm();
      this.modalService.open(modalHtml, { size: 'lg' });
    });
  }

  buildForm() {
    const isEditing = !!this.selectedTask.id;
    this.form = this.fb.group({
      patientId: [this.selectedTask.patientId || null, Validators.required],
      doctorId: [this.selectedTask.doctorId || null, Validators.required],
      operationName: [this.selectedTask.operationName || '', Validators.required],
      operationDate: [this.selectedTask.operationDate ? this.selectedTask.operationDate.substring(0, 16) : '', Validators.required],
      status: [this.selectedTask.status || OperationStatus.Scheduled, Validators.required],
      surgeonFeePercentage: [this.selectedTask.surgeonFeePercentage || 0],
      anesthesiologistFeePercentage: [this.selectedTask.anesthesiologistFeePercentage || 0],
      totalAmount: [this.selectedTask.totalAmount || 0, [Validators.required, Validators.min(0)]],
      notes: [this.selectedTask.notes || '']
    });
  }

  save(modal: any) {
    if (this.form.invalid) {
      return;
    }

    const val = this.form.value;

    const request = this.selectedTask.id
      ? this.operationService.update(this.selectedTask.id, val)
      : this.operationService.create(val);

    request.subscribe(() => {
      this.list.get();
      modal.close();
    });
  }

  deleteOperation(id: string) {
    this.confirmation.warn('::OperationDeletionConfirmationMessage', '::AreYouSure').subscribe((status) => {
      if (status === Confirmation.Status.confirm) {
        this.operationService.delete(id).subscribe(() => this.list.get());
      }
    });
  }
}

interface SurgicalOperationInput {
  maxResultCount?: number;
  skipCount?: number;
  sorting?: string;
}
