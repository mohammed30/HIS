import { Component, OnInit } from '@angular/core';
import { PharmacyService } from '../pharmacy.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-pharmacy-stock',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="card shadow-sm hover-lift">
      <div class="card-header bg-primary text-white d-flex justify-content-between align-items-center">
        <h5 class="mb-0"><i class="fas fa-warehouse me-2"></i>Pharmacy Stock</h5>
        <div class="d-flex align-items-center">
            <label class="me-2 text-white mb-0">Warehouse:</label>
            <select class="form-select form-select-sm" style="width: auto;" [(ngModel)]="selectedWarehouseId" (change)="loadData()">
                <option *ngFor="let w of warehouses" [value]="w.id">{{ w.name }}</option>
            </select>
        </div>
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
                <td colspan="3" class="text-center py-4 text-muted">No stock items found. Please select a valid warehouse.</td>
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
  warehouses: any[] = [];
  selectedWarehouseId: string = '';

  constructor(private pharmacyService: PharmacyService, private http: HttpClient) { }

  ngOnInit(): void {
    this.loadWarehouses();
  }

  loadWarehouses() {
    this.http.get<any[]>('/api/app/inventory/warehouse-lookup').subscribe(res => {
      this.warehouses = res;
      // Pre-select the pharmacy warehouse if possible
      const defaultWh = this.warehouses.find(w => w.name.toLowerCase().includes('pharmacy') || w.name.includes('صيدلية'));
      if (defaultWh) {
        this.selectedWarehouseId = defaultWh.id;
      } else if (this.warehouses.length > 0) {
        this.selectedWarehouseId = this.warehouses[0].id;
      }
      if (this.selectedWarehouseId) {
        this.loadData();
      }
    });
  }

  loadData() {
    if (!this.selectedWarehouseId) return;
    this.pharmacyService.getPharmacyStock(this.selectedWarehouseId).subscribe(res => {
      this.items = res;
    });
  }
}
