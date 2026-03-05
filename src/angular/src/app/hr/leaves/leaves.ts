import { ListService, PagedResultDto, CoreModule } from '@abp/ng.core';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { LeaveTypeDto, EmployeeLeaveDto, EmployeeLookupDto } from '../../proxy/hr/models';
import { HRService } from '../../proxy/hr/hr.service';
import { ConfirmationService, Confirmation, ThemeSharedModule } from '@abp/ng.theme.shared';
import { NgbDropdownModule, NgbNavModule } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-leaves',
  standalone: true,
  imports: [CoreModule, ThemeSharedModule, ReactiveFormsModule, CommonModule, NgbDropdownModule, NgbNavModule],
  templateUrl: './leaves.html',
  styleUrls: ['./leaves.scss'],
  providers: [ListService],
})
export class Leaves implements OnInit {
  // Types List
  leaveTypes: LeaveTypeDto[] = [];
  selectedType = {} as LeaveTypeDto;
  isTypeModalOpen = false;
  typeForm: FormGroup;

  // Requests Paged List
  employeeLeaves: PagedResultDto<EmployeeLeaveDto> = { items: [], totalCount: 0 };
  selectedLeave = {} as EmployeeLeaveDto;
  isLeaveModalOpen = false;
  leaveForm: FormGroup;

  // Lookups
  employeeLookup: EmployeeLookupDto[] = [];

  activeTab = 1; // 1 for requests, 2 for types

  constructor(
    public readonly list: ListService,
    private hrService: HRService,
    private fb: FormBuilder,
    private confirmation: ConfirmationService
  ) { }

  ngOnInit() {
    this.loadLeaveTypes();
    this.loadEmployeeLookup();

    const streamCreator = (query) => this.hrService.getEmployeeLeaves(query);
    this.list.hookToQuery(streamCreator).subscribe((response) => {
      this.employeeLeaves = response;
    });
  }

  loadLeaveTypes() {
    this.hrService.getLeaveTypes().subscribe((result) => {
      this.leaveTypes = result;
    });
  }

  loadEmployeeLookup() {
    this.hrService.getEmployeeLookup().subscribe((result) => {
      this.employeeLookup = result;
    });
  }

  // ===== Leave Type Methods =====

  createType() {
    this.selectedType = {} as LeaveTypeDto;
    this.buildTypeForm();
    this.isTypeModalOpen = true;
  }

  editType(id: string) {
    const type = this.leaveTypes.find((x) => x.id === id);
    if (type) {
      this.selectedType = { ...type };
      this.buildTypeForm();
      this.isTypeModalOpen = true;
    }
  }

  buildTypeForm() {
    this.typeForm = this.fb.group({
      nameAr: [this.selectedType.nameAr || '', Validators.required],
      duration: [this.selectedType.duration || 0, Validators.required],
      employeeClass: [this.selectedType.employeeClass || ''],
      affectsSalary: [this.selectedType.affectsSalary ?? true],
      isBalance: [this.selectedType.isBalance ?? true],
      isPublicHoliday: [this.selectedType.isPublicHoliday ?? false],
      isActive: [this.selectedType.isActive ?? true],
    });
  }

  saveType() {
    if (this.typeForm.invalid) return;

    const request = this.selectedType.id
      ? this.hrService.updateLeaveType(this.selectedType.id, this.typeForm.value)
      : this.hrService.createLeaveType(this.typeForm.value);

    request.subscribe(() => {
      this.isTypeModalOpen = false;
      this.loadLeaveTypes();
    });
  }

  deleteType(id: string) {
    this.confirmation.warn('::AreYouSureToDelete', '::AreYouSure').subscribe((status) => {
      if (status === Confirmation.Status.confirm) {
        this.hrService.deleteLeaveType(id).subscribe(() => this.loadLeaveTypes());
      }
    });
  }

  // ===== Leave Request Methods =====

  createLeaveRequest() {
    this.selectedLeave = {} as EmployeeLeaveDto;
    this.buildLeaveForm();
    this.isLeaveModalOpen = true;
  }

  buildLeaveForm() {
    this.leaveForm = this.fb.group({
      employeeId: [this.selectedLeave.employeeId || null, Validators.required],
      leaveTypeId: [this.selectedLeave.leaveTypeId || null, Validators.required],
      startDate: [
        this.selectedLeave.startDate ? new Date(this.selectedLeave.startDate).toISOString().split('T')[0] : null,
        Validators.required
      ],
      endDate: [
        this.selectedLeave.endDate ? new Date(this.selectedLeave.endDate).toISOString().split('T')[0] : null,
        Validators.required
      ],
      duration: [this.selectedLeave.duration || 1, [Validators.required, Validators.min(1)]],
      notes: [this.selectedLeave.notes || ''],
    });
  }

  saveLeaveRequest() {
    if (this.leaveForm.invalid) return;

    this.hrService.createEmployeeLeave(this.leaveForm.value).subscribe(() => {
      this.isLeaveModalOpen = false;
      this.list.get();
    });
  }

  deleteLeaveRequest(id: string) {
    this.confirmation.warn('::AreYouSureToDelete', '::AreYouSure').subscribe((status) => {
      if (status === Confirmation.Status.confirm) {
        this.hrService.deleteEmployeeLeave(id).subscribe(() => this.list.get());
      }
    });
  }
}
