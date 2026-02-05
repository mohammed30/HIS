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
        <div class="row align-items-center mb-4">
          <div class="col-md-7">
            <h6 class="text-primary fw-bold mb-3"><i class="fas fa-user-injured me-2"></i>Patient Information</h6>
            <p class="mb-1"><strong>Name:</strong> {{ order.patientName }}</p>
            <p class="mb-1"><strong>MRN:</strong> <code class="text-primary fw-bold">{{ order.patientMRN }}</code></p>
            <hr>
            <h6 class="text-success fw-bold mb-3"><i class="fas fa-prescription-bottle-alt me-2"></i>Order Details</h6>
            <p class="mb-1"><strong>Medication:</strong> <span class="badge bg-light text-dark fs-6">{{ order.serviceName }}</span></p>
            <p class="mb-1"><strong>Requested Qty:</strong> <span class="badge bg-info text-dark">{{ order.quantity }}</span></p>
            <p class="mb-0 text-muted"><strong>Notes:</strong> {{ order.clinicalNotes || 'No specific clinical notes provided.' }}</p>
          </div>
          <div class="col-md-5">
             <div class="alert alert-info border-0 shadow-sm">
               <div class="d-flex">
                 <i class="fas fa-info-circle fa-2x me-3"></i>
                 <div>
                   <h6 class="alert-heading fw-bold">LIFO Strategy</h6>
                   <p class="mb-0 small">Items will be automatically dispensed from the <strong>Pharmacy Warehouse</strong> using the Last-In-First-Out (LIFO) accounting method.</p>
                 </div>
               </div>
             </div>
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
    });
  }

  confirmDispense() {
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
