import { Component, OnInit, ViewChild, TemplateRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { CoreModule, PagedResultDto } from '@abp/ng.core';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { NgbModal, NgbNavModule } from '@ng-bootstrap/ng-bootstrap';
import { PharmacyService } from '../../proxy/pharmacy/pharmacy.service';
import { DispensingService } from '../../proxy/pharmacy/dispensing.service';
import { VerifyPrescriptionDto, PendingPrescriptionDto } from '../../proxy/pharmacy/models';
import { FormsModule } from '@angular/forms';

@Component({
    selector: 'app-dispensing',
    standalone: true,
    imports: [CommonModule, ThemeSharedModule, NgxDatatableModule, CoreModule, FormsModule, NgbNavModule],
    template: `
    <div class="card">
      <div class="card-header">
        <h5 class="card-title mb-0">
          <i class="fas fa-check-double me-2"></i> {{ '::DispensingVerification' | abpLocalization }}
        </h5>
      </div>
      <div class="card-body">
        <ul ngbNav #nav="ngbNav" [(activeId)]="activeTab" class="nav-tabs">
            <li [ngbNavItem]="1">
                <a ngbNavLink>{{ '::PendingVerification' | abpLocalization }}</a>
                <ng-template ngbNavContent>
                    <ngx-datatable [rows]="pendingPrescriptions" [columnMode]="'force'" [headerHeight]="50" [footerHeight]="50" [rowHeight]="'auto'">
                        <ngx-datatable-column [name]="'::PrescriptionDate' | abpLocalization" prop="creationTime">
                            <ng-template let-row="row" ngx-datatable-cell-template>
                                {{ row.creationTime | date:'short' }}
                            </ng-template>
                        </ngx-datatable-column>
                        <ngx-datatable-column [name]="'::Patient' | abpLocalization" prop="patientName"></ngx-datatable-column>
                        <ngx-datatable-column [name]="'::Drug' | abpLocalization" prop="serviceName"></ngx-datatable-column>
                        <ngx-datatable-column [name]="'::Doctor' | abpLocalization" prop="doctorName"></ngx-datatable-column>
                        <ngx-datatable-column [name]="'::Actions' | abpLocalization" sortable="false">
                            <ng-template let-row="row" ngx-datatable-cell-template>
                                <button class="btn btn-sm btn-primary" (click)="openVerification(row)">
                                    <i class="fas fa-check-circle me-1"></i> {{ '::Verify' | abpLocalization }}
                                </button>
                            </ng-template>
                        </ngx-datatable-column>
                    </ngx-datatable>
                </ng-template>
            </li>
        </ul>
        <div [ngbNavOutlet]="nav" class="mt-3"></div>
      </div>
    </div>

    <!-- Verification Modal -->
    <ng-template #verificationModal let-modal>
        <div class="modal-header">
            <h5 class="modal-title">{{ '::VerifyPrescription' | abpLocalization }}</h5>
            <button type="button" class="btn-close" (click)="modal.dismiss()"></button>
        </div>
        <div class="modal-body" *ngIf="selectedPrescription">
            <dl class="row">
                <dt class="col-sm-4">{{ '::Patient' | abpLocalization }}</dt>
                <dd class="col-sm-8">{{ selectedPrescription.patientName }}</dd>

                <dt class="col-sm-4">{{ '::Drug' | abpLocalization }}</dt>
                <dd class="col-sm-8">{{ selectedPrescription.serviceName }}</dd>

                <dt class="col-sm-4">{{ '::Instructions' | abpLocalization }}</dt>
                <dd class="col-sm-8">{{ selectedPrescription.instructions }}</dd>
            </dl>
            
            <div class="mb-3">
                <label class="form-label">{{ '::SafetyCheckComments' | abpLocalization }}</label>
                <textarea class="form-control" rows="3" [(ngModel)]="verificationComments" placeholder="e.g. Drug Interaction checked, Dosage confirmed"></textarea>
            </div>
        </div>
        <div class="modal-footer">
            <button type="button" class="btn btn-danger" (click)="submitVerification(false)">{{ '::Reject' | abpLocalization }}</button>
            <button type="button" class="btn btn-success" (click)="submitVerification(true)">{{ '::Approve' | abpLocalization }}</button>
        </div>
    </ng-template>
  `
})
export class DispensingComponent implements OnInit {
    @ViewChild('verificationModal') verificationModal: TemplateRef<any>;
    activeTab = 1;
    pendingPrescriptions: PendingPrescriptionDto[] = [];
    selectedPrescription: PendingPrescriptionDto | null = null;
    verificationComments = '';

    constructor(
        private pharmacyService: PharmacyService,
        private dispensingService: DispensingService,
        private modalService: NgbModal
    ) { }

    ngOnInit() {
        this.loadPending();
    }

    loadPending() {
        this.pharmacyService.getPendingPrescriptions().subscribe(res => {
            this.pendingPrescriptions = res;
        });
    }

    openVerification(row: PendingPrescriptionDto) {
        this.selectedPrescription = row;
        this.verificationComments = ''; // Reset comments
        this.modalService.open(this.verificationModal, { size: 'lg' });
    }

    submitVerification(isApproved: boolean) {
        if (!this.selectedPrescription) return;

        const input: VerifyPrescriptionDto = {
            medicalOrderId: this.selectedPrescription.id,
            isApproved: isApproved,
            safetyCheckComments: this.verificationComments
        };

        this.dispensingService.verifyPrescription(input).subscribe(() => {
            this.modalService.dismissAll();
            this.loadPending(); // Refresh list
        });
    }
}
