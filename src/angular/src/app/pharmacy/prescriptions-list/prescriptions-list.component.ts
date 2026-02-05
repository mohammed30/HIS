import { Component, OnInit } from '@angular/core';
import { PharmacyService } from '../pharmacy.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-prescriptions-list',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="card shadow-sm hover-lift">
      <div class="card-header bg-primary text-white">
        <h5 class="card-title mb-0">
          <i class="fas fa-prescription me-2"></i>
          Pending Prescriptions (الطلبات المعلقة)
        </h5>
      </div>
      <div class="card-body">
        <div class="table-responsive">
          <table class="table table-striped table-hover align-middle">
            <thead>
              <tr>
                <th>Date</th>
                <th>Patient Name</th>
                <th>MRN</th>
                <th>Medication</th>
                <th>Qty</th>
                <th>Notes</th>
                <th class="text-center">Action</th>
              </tr>
            </thead>
            <tbody>
              <tr *ngFor="let item of list">
                <td>{{ item.creationTime | date:'short' }}</td>
                <td><strong>{{ item.patientName }}</strong></td>
                <td><code class="text-primary">{{ item.patientMRN }}</code></td>
                <td>{{ item.serviceName }}</td>
                <td><span class="badge bg-info text-dark">{{ item.quantity }}</span></td>
                <td><small class="text-muted">{{ item.clinicalNotes || '-' }}</small></td>
                <td class="text-center">
                  <button class="btn btn-primary btn-premium btn-sm" (click)="dispense(item.id)">
                    <i class="fas fa-capsules me-1"></i> Dispense
                  </button>
                </td>
              </tr>
              <tr *ngIf="list && list.length === 0">
                <td colspan="7" class="text-center py-4 text-muted">
                  <i class="fas fa-inbox fa-3x mb-2 d-block"></i>
                  No pending prescriptions found.
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  `
})
export class PrescriptionsListComponent implements OnInit {
  list: any[] = [];

  constructor(private pharmacyService: PharmacyService, private router: Router) { }

  ngOnInit(): void {
    this.loadData();
  }

  loadData() {
    this.pharmacyService.getPendingPrescriptions().subscribe(res => {
      this.list = res;
    });
  }

  dispense(id: string) {
    this.router.navigate(['/pharmacy/dispense', id]);
  }
}
