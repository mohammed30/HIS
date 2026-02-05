import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PharmacyService } from '../pharmacy.service';
import { ToasterService } from '@abp/ng.theme.shared';
import { CommonModule } from '@angular/common';
import { ThemeSharedModule } from '@abp/ng.theme.shared';

@Component({
  selector: 'app-dispensing-workflow',
  standalone: true,
  imports: [CommonModule, ThemeSharedModule],
  template: `
    <div class="card shadow-sm hover-lift" *ngIf="order">
      <div class="card-header bg-success text-white">
        <h5 class="mb-0"><i class="fas fa-pills me-2"></i>Medication Dispensing (صرف الدواء)</h5>
      </div>
      <div class="card-body">
        
        <!-- Interaction Warnings -->
        <div *ngIf="warnings.length > 0" class="alert alert-danger shadow-sm mb-4">
            <h5 class="alert-heading fw-bold"><i class="fas fa-exclamation-triangle me-2"></i>Drug Interaction Warnings</h5>
            <ul class="mb-0">
                <li *ngFor="let w of warnings">{{ w }}</li>
            </ul>
        </div>

        <div class="row mb-4">
          <div class="col-md-6 border-end">
            <h6 class="text-primary fw-bold mb-3"><i class="fas fa-user-injured me-2"></i>Patient Information</h6>
            <p class="mb-1"><strong>Name:</strong> {{ order.patientName }}</p>
            <p class="mb-1"><strong>MRN:</strong> <code class="text-primary fw-bold">{{ order.patientMRN }}</code></p>
          </div>
          <div class="col-md-6 ps-4">
             <h6 class="text-success fw-bold mb-3"><i class="fas fa-prescription-bottle-alt me-2"></i>Prescription Details</h6>
             <p class="mb-1"><strong>Medication:</strong> <span class="badge bg-primary fs-6">{{ order.serviceName }}</span></p>
             <div class="d-flex flex-wrap gap-2 mb-2">
                 <span class="badge bg-light text-dark border">Qty: {{ order.quantity }}</span>
                 <span class="badge bg-light text-dark border">Dosage: {{ order.dosage || 'N/A' }}</span>
                 <span class="badge bg-light text-dark border">Route: {{ order.route || 'N/A' }}</span>
                 <span class="badge bg-light text-dark border">Freq: {{ order.frequency || 'N/A' }}</span>
                 <span class="badge bg-light text-dark border">Duration: {{ order.duration || 'N/A' }}</span>
             </div>
             <div class="p-2 bg-light rounded mt-2" *ngIf="order.instructions">
                <strong>Instructions:</strong> {{ order.instructions }}
             </div>
             <p class="mb-0 text-muted mt-2" *ngIf="order.clinicalNotes"><strong>MD Notes:</strong> {{ order.clinicalNotes }}</p>
          </div>
        </div>

        <div class="d-flex justify-content-end gap-2 border-top pt-3">
          <button class="btn btn-outline-secondary btn-premium" (click)="cancel()">
            <i class="fas fa-times me-1"></i> Cancel
          </button>
          <button class="btn btn-success btn-premium" (click)="confirmDispense()" [disabled]="processing">
            <i class="fas fa-check-circle me-1"></i> 
            {{ processing ? 'Processing...' : 'Confirm & Dispense Inventory' }}
          </button>
        </div>
      </div>
    </div>
  `
})
export class DispensingWorkflowComponent implements OnInit {
  orderId: string;
  order: any;
  warnings: string[] = [];
  processing = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private pharmacyService: PharmacyService,
    private toaster: ToasterService
  ) { }

  ngOnInit(): void {
    this.orderId = this.route.snapshot.params['id'];
    this.loadOrder();
  }

  loadOrder() {
    this.pharmacyService.getPrescription(this.orderId).subscribe(res => {
      this.order = res;
      // Check interactions
      this.pharmacyService.checkInteractions(this.order.patientId, this.order.serviceName).subscribe(w => {
        this.warnings = w;
      });
    });
  }

  confirmDispense() {
    if (this.warnings.length > 0 && !confirm('There are drug interactions. Are you sure you want to proceed?')) {
      return;
    }

    this.processing = true;
    const input = { medicalOrderId: this.orderId };

    this.pharmacyService.dispenseMedication(input).subscribe({
      next: () => {
        this.toaster.success('Dispensed successfully', 'Success');
        this.router.navigate(['/pharmacy']);
      },
      error: (err) => {
        this.processing = false;
        this.toaster.error(err.message || 'Error dispensing', 'Error');
      }
    });
  }

  cancel() {
    this.router.navigate(['/pharmacy']);
  }
}
