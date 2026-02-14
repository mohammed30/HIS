import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdmissionService } from '@proxy/inpatient';
import { AdmissionDto } from '@proxy/inpatient/models';
import { CoreModule } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';

@Component({
    selector: 'app-admission-list',
    standalone: true,
    imports: [CommonModule, CoreModule, ThemeSharedModule],
    template: `
    <div class="container-fluid p-4">
      <div class="card shadow-sm">
        <div class="card-header d-flex justify-content-between align-items-center bg-transparent py-3">
          <h2 class="m-0 h4"><i class="fas fa-hospital-user me-2 text-primary"></i> Admissions</h2>
          <button class="btn btn-primary">
            <i class="fas fa-plus me-1"></i> New Admission
          </button>
        </div>
        
        <div class="card-body">
          <div class="table-responsive">
            <table class="table table-hover align-middle">
              <thead class="table-light">
                <tr>
                  <th>Patient</th>
                  <th>MRN</th>
                  <th>Room</th>
                  <th>Admission Date</th>
                  <th>Status</th>
                  <th class="text-end">Actions</th>
                </tr>
              </thead>
              <tbody>
                <tr *ngFor="let admission of admissions">
                  <td>
                    <div class="fw-bold">{{ admission.patientName }}</div>
                  </td>
                  <td><code class="text-secondary">{{ admission.patientFileNumber }}</code></td>
                  <td>
                    <span class="badge bg-light text-dark border">
                      <i class="fas fa-door-open me-1 text-muted"></i> {{ admission.roomNumber }}
                    </span>
                  </td>
                  <td class="small">{{ admission.admissionDate | date:'mediumDate' }}</td>
                  <td>
                    <span class="badge rounded-pill" [ngClass]="getStatusSeverityClass(admission.status)">
                      {{ getStatusName(admission.status) }}
                    </span>
                  </td>
                  <td class="text-end">
                    <div class="btn-group">
                      <button class="btn btn-sm btn-outline-info" title="View">
                        <i class="fas fa-eye"></i>
                      </button>
                      <button class="btn btn-sm btn-outline-primary" title="Edit">
                        <i class="fas fa-pencil-alt"></i>
                      </button>
                    </div>
                  </td>
                </tr>
                <tr *ngIf="admissions.length === 0">
                  <td colspan="6" class="text-center py-5 text-muted">
                    <i class="fas fa-inbox fa-3x mb-3 d-block opacity-25"></i>
                    No admissions found
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  `
})
export class AdmissionListComponent implements OnInit {
    private admissionService = inject(AdmissionService);
    admissions: AdmissionDto[] = [];

    ngOnInit() {
        this.loadAdmissions();
    }

    loadAdmissions() {
        this.admissionService.getList({ maxResultCount: 100 }).subscribe(result => {
            this.admissions = result.items || [];
        });
    }

    getStatusName(status: number | undefined): string {
        switch (status) {
            case 0: return 'Admitted';
            case 1: return 'Discharged';
            case 2: return 'Cancelled';
            default: return 'Unknown';
        }
    }

    getStatusSeverityClass(status: number | undefined): string {
        switch (status) {
            case 0: return 'bg-info text-dark';
            case 1: return 'bg-success';
            case 2: return 'bg-danger';
            default: return 'bg-secondary';
        }
    }
}
