import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CoreModule } from '@abp/ng.core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { ThemeSharedModule, ToasterService } from '@abp/ng.theme.shared';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { AdmissionService } from '@proxy/inpatient';
import { AdmissionDto } from '@proxy/inpatient/models';
import { roomTypeOptions } from '@proxy/rooms';

@Component({
  selector: 'app-admission-list',
  standalone: true,
  imports: [
    CommonModule,
    CoreModule,
    ThemeSharedModule,
    ReactiveFormsModule,
    NgbModule
  ],
  templateUrl: './admission-list.component.html',
  styleUrls: ['./admission-list.component.scss']
})
export class AdmissionListComponent implements OnInit {
  private admissionService = inject(AdmissionService);
  private fb = inject(FormBuilder);
  private toaster = inject(ToasterService);

  admissions: AdmissionDto[] = [];
  selectedAdmission: AdmissionDto | null = null;
  roomTypes = roomTypeOptions;

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
    notes: ['']
  });

  ngOnInit() {
    this.loadAdmissions();
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
      notes: admission.notes || ''
    });
    this.detailForm.markAsPristine();
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

  getStatusName(status: number | undefined): string {
    switch (status) {
      case 0: return '::Enum:AdmissionStatus.0';
      case 1: return '::Enum:AdmissionStatus.1';
      case 2: return '::Enum:AdmissionStatus.2';
      default: return '::Unknown';
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
}
