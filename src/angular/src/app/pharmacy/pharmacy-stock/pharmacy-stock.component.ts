import { Component, OnInit } from '@angular/core';
import { PharmacyService } from '../pharmacy.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-pharmacy-stock',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="card shadow-sm hover-lift">
      <div class="card-header bg-primary text-white">
        <h5 class="mb-0"><i class="fas fa-warehouse me-2"></i>Pharmacy Stock</h5>
      </div>
      <div class="card-body p-0">
        <div class="table-responsive">
          <table class="table table-hover mb-0">
            <thead class="table-dark">
              <tr>
                <th>Product Name</th>
                <th class="text-center">Quantity</th>
                <th class="text-end">Avg Cost</th>
              </tr>
            </thead>
            <tbody>
              <tr *ngFor="let item of items">
                <td><strong>{{ item.productName }}</strong></td>
                <td class="text-center">
                  <span class="badge" [ngClass]="item.quantity > 50 ? 'bg-success' : 'bg-warning'">
                    {{ item.quantity }}
                  </span>
                </td>
                <td class="text-end text-success fw-bold">{{ item.averageCost | currency:'EGP':'symbol-narrow' }}</td>
              </tr>
              <tr *ngIf="items && items.length === 0">
                <td colspan="3" class="text-center py-4 text-muted">No stock items found in Pharmacy.</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  `
})
export class PharmacyStockComponent implements OnInit {
  items: any[] = [];

  constructor(private pharmacyService: PharmacyService) { }

  ngOnInit(): void {
    this.loadData();
  }

  loadData() {
    this.pharmacyService.getPharmacyStock().subscribe(res => {
      this.items = res;
    });
  }
}
