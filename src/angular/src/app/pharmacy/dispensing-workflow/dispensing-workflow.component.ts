import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PharmacyService } from '../pharmacy.service';
import { ToasterService } from '@abp/ng.theme.shared';
import { CommonModule } from '@angular/common';
import { ThemeSharedModule, ConfirmationService, Confirmation } from '@abp/ng.theme.shared';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-dispensing-workflow',
  standalone: true,
  imports: [CommonModule, ThemeSharedModule, FormsModule],
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

        <div class="row mb-4">
            <div class="col-12">
                <label class="form-label fw-bold"><i class="fas fa-comment-medical me-2"></i>Counseling Notes (ملاحظات الصيدلي للمريض)</label>
                <textarea class="form-control" [(ngModel)]="counselingNotes" rows="3" placeholder="Enter instructions for the patient..."></textarea>
            </div>
        </div>

        <div class="d-flex justify-content-between border-top pt-3">
          <div>
            <button *ngIf="dispensingId" class="btn btn-primary btn-premium" (click)="printLabel()">
                <i class="fas fa-print me-1"></i> Print Label
            </button>
          </div>
          <div class="d-flex gap-2">
            <button class="btn btn-outline-secondary btn-premium" (click)="cancel()">
                <i class="fas fa-times me-1"></i> Cancel
            </button>
            <button class="btn btn-success btn-premium" (click)="confirmDispense()" [disabled]="processing || dispensingId">
                <i class="fas fa-check-circle me-1"></i> 
                {{ processing ? 'Processing...' : (dispensingId ? 'Dispensed' : 'Confirm & Dispense Inventory') }}
            </button>
          </div>
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
  counselingNotes = '';
  dispensingId: string = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private pharmacyService: PharmacyService,
    private toaster: ToasterService,
    private confirmation: ConfirmationService
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
    if (this.warnings.length > 0) {
      this.confirmation.warn('There are drug interactions. Are you sure you want to proceed?', 'Interaction Warning').subscribe(status => {
        if (status === Confirmation.Status.confirm) {
          this.executeDispense();
        }
      });
      return;
    }
    this.executeDispense();
  }

  private executeDispense() {
    const input = {
      medicalOrderId: this.orderId,
      counselingNotes: this.counselingNotes
    };

    this.pharmacyService.dispenseMedication(input).subscribe({
      next: (res: any) => {
        this.toaster.success('Dispensed successfully', 'Success');
        // If the backend returned an ID, we could use it for label
        // For now, we'll just wait or navigate
        this.processing = false;
        // In a real flow, we might stay to print label
        // this.dispensingId = res.id; 
      },
      error: (err) => {
        this.processing = false;
        this.toaster.error(err.message || 'Error dispensing', 'Error');
      }
    });
  }

  printLabel() {
    // Implementation for opening a print-friendly view or PDF
    this.toaster.info('Generating label...', 'Info');
  }

  cancel() {
    this.router.navigate(['/pharmacy']);
  }
}
