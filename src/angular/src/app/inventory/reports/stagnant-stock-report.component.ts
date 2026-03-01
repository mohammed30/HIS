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
    <div class="card">
      <div class="card-header d-flex justify-content-between align-items-center">
        <h4 class="card-title mb-0">تقرير المخزون الراكد</h4>
        <div class="d-flex align-items-center gap-2">
            <select class="form-select" [(ngModel)]="warehouseId" (change)="loadReport()">
                <option [ngValue]="null">جميع المستودعات</option>
                <option *ngFor="let w of warehouses" [ngValue]="w.id">{{ w.name }}</option>
            </select>
            <div class="input-group" style="width: 200px;">
                <span class="input-group-text">أيام الركود</span>
                <input type="number" class="form-control" [(ngModel)]="thresholdDays" min="1">
            </div>
            <button class="btn btn-primary" (click)="loadReport()" [disabled]="isLoading">
              <i class="fa fa-sync" [class.fa-spin]="isLoading"></i> تحديث
            </button>
            <button class="btn btn-outline-secondary" (click)="print()">
                <i class="fa fa-print"></i> طباعة
            </button>
        </div>
      </div>
      <div class="card-body">
        <div class="table-responsive">
          <table class="table table-bordered table-striped">
            <thead>
              <tr>
                <th>{{ '::ProductName' | abpLocalization }}</th>
                <th>{{ '::Warehouse' | abpLocalization }}</th>
                <th>الكمية المتوفرة</th>
                <th>آخر حركة صرف</th>
                <th>أيام الركود</th>
              </tr>
            </thead>
            <tbody>
              <tr *ngIf="isLoading">
                <td colspan="5" class="text-center py-4">
                  <div class="spinner-border text-primary" role="status"></div>
                </td>
              </tr>
              <tr *ngIf="!isLoading && reportData.length === 0">
                <td colspan="5" class="text-center py-4 text-muted">
                    لا يوجد أصناف راكدة حسب المتطلبات الحالية
                </td>
              </tr>
              <tr *ngFor="let item of reportData">
                <td>{{ item.productName }}</td>
                <td>{{ item.warehouseName }}</td>
                <td>{{ item.currentQuantity | number:'1.0-2' }}</td>
                <td>{{ item.lastTransactionDate ? (item.lastTransactionDate | date:'shortDate') : 'لا يوجد' }}</td>
                <td class="text-danger fw-bold">{{ item.daysStagnant }} يوم</td>
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
