import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CoreModule } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { InventoryService } from '@proxy/inventory';
import { RestService } from '@abp/ng.core';
import { StagnantStockReportDto, WarehouseDto } from '@proxy/inventory/dtos/models';

@Component({
  selector: 'app-stagnant-stock-report',
  standalone: true,
  imports: [CommonModule, FormsModule, CoreModule, ThemeSharedModule],
  template: `
    <div class="card shadow-sm border-0">
      <div class="card-header bg-transparent border-bottom py-4 px-4">
        <div class="row align-items-end g-3">
          <!-- Title Section -->
          <div class="col-md-auto">
            <h4 class="mb-0 text-primary d-flex align-items-center">
              <i class="fa fa-boxes-stacked me-2 fs-3"></i>
              <span class="fw-bold fs-5">{{ '::StagnantStockReport' | abpLocalization }}</span>
            </h4>
          </div>
          
          <!-- Filters and Actions Section -->
          <div class="col-md">
            <div class="d-flex flex-wrap justify-content-end align-items-end gap-3">
              <!-- Warehouse Selector -->
              <div style="min-width: 240px;" class="form-group mb-0">
                <label class="form-label small text-muted fw-bold mb-2">
                  <i class="fa fa-warehouse me-1 small"></i> {{ '::Warehouse' | abpLocalization }}
                </label>
                <select class="form-select border-0 shadow-sm bg-light" [(ngModel)]="warehouseId" (change)="loadReport()">
                  <option [ngValue]="null">{{ '::AllWarehouses' | abpLocalization }}</option>
                  <option *ngFor="let w of warehouses" [ngValue]="w.id">{{ w.name }}</option>
                </select>
              </div>

              <!-- Stagnancy Threshold -->
              <div style="width: 160px;" class="form-group mb-0">
                <label class="form-label small text-muted fw-bold mb-2">
                  <i class="fa fa-clock me-1 small"></i> {{ '::DaysStagnant' | abpLocalization }}
                </label>
                <div class="input-group shadow-sm">
                  <input type="number" class="form-control border-0 bg-light text-center" [(ngModel)]="thresholdDays" min="1">
                  <span class="input-group-text border-0 bg-light small text-muted">{{ '::Days' | abpLocalization }}</span>
                </div>
              </div>

              <!-- Actions -->
              <div class="d-flex gap-2">
                <button class="btn btn-primary d-flex align-items-center gap-2 px-4 shadow-sm h-100" style="min-height: 38px;" (click)="loadReport()" [disabled]="isLoading">
                  <i class="fa fa-sync-alt" [class.fa-spin]="isLoading"></i>
                  <span class="d-none d-lg-inline">{{ '::Refresh' | abpLocalization }}</span>
                </button>
                <button class="btn btn-outline-primary d-flex align-items-center gap-2 px-4 shadow-sm h-100 border-2" style="min-height: 38px;" (click)="print()" [disabled]="isLoading">
                  <i class="fa fa-print"></i>
                  <span class="d-none d-lg-inline">{{ '::Print' | abpLocalization }}</span>
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div class="card-body p-0">
        <div class="table-responsive">
          <table class="table table-hover align-middle mb-0">
            <thead class="bg-light">
              <tr>
                <th class="ps-4">{{ '::ProductName' | abpLocalization }}</th>
                <th>{{ '::Warehouse' | abpLocalization }}</th>
                <th class="text-center">{{ '::AvailableQuantity' | abpLocalization }}</th>
                <th class="text-center">{{ '::LastTransactionDate' | abpLocalization }}</th>
                <th class="text-center">{{ '::DaysStagnant' | abpLocalization }}</th>
              </tr>
            </thead>
            <tbody>
              <tr *ngIf="isLoading">
                <td colspan="5" class="text-center py-5">
                  <div class="spinner-border text-primary" role="status"></div>
                </td>
              </tr>
              <tr *ngIf="!isLoading && reportData.length === 0">
                <td colspan="5" class="text-center py-5 text-muted">
                    <i class="fa fa-info-circle me-2"></i>
                    {{ '::NoStagnantItems' | abpLocalization }}
                </td>
              </tr>
              <tr *ngFor="let item of reportData">
                <td class="ps-4 fw-bold text-dark">{{ item.productName }}</td>
                <td>
                    <span class="badge bg-light text-dark border">{{ item.warehouseName }}</span>
                </td>
                <td class="text-center fw-bold">{{ item.currentQuantity | number:'1.0-2' }}</td>
                <td class="text-center text-muted">
                    {{ item.lastTransactionDate ? (item.lastTransactionDate | date:'dd/MM/yyyy') : ('::NoData' | abpLocalization) }}
                </td>
                <td class="text-center">
                    <span class="badge bg-danger px-3 py-2">{{ item.daysStagnant }} {{ '::Days' | abpLocalization }}</span>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  `
})
export class StagnantStockReportComponent implements OnInit {
  inventoryService = inject(InventoryService);
  restService = inject(RestService);

  warehouses: WarehouseDto[] = [];
  warehouseId: string | null = null;
  reportData: StagnantStockReportDto[] = [];
  thresholdDays: number = 30;
  isLoading = false;

  ngOnInit() {
    this.loadWarehouses();
    this.loadReport();
  }

  loadWarehouses() {
    this.inventoryService.getWarehouseList({ maxResultCount: 100 }).subscribe(res => {
      this.warehouses = res.items;
    });
  }

  loadReport() {
    this.isLoading = true;
    this.inventoryService.getStagnantStockReport({
      warehouseId: this.warehouseId as any,
      thresholdDays: this.thresholdDays
    }).subscribe({
      next: (data) => {
        this.reportData = data;
        this.isLoading = false;
      },
      error: () => this.isLoading = false
    });
  }

  print() {
    this.isLoading = true;
    this.restService.request<any, Blob>({
      method: 'GET',
      url: '/api/app/inventory/reports/stagnant-stock/pdf',
      params: {
        warehouseId: this.warehouseId,
        thresholdDays: this.thresholdDays
      },
      responseType: 'blob'
    }).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `Stagnant_Stock_Report_${new Date().getTime()}.pdf`;
        link.click();
        window.URL.revokeObjectURL(url);
        this.isLoading = false;
      },
      error: () => this.isLoading = false
    });
  }
}
