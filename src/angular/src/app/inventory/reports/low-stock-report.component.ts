import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CoreModule } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { InventoryService } from '@proxy/inventory';
import { LowStockReportDto, WarehouseDto } from '@proxy/inventory/dtos/models';

@Component({
  selector: 'app-low-stock-report',
  standalone: true,
  imports: [CommonModule, FormsModule, CoreModule, ThemeSharedModule],
  template: `
    <div class="card">
      <div class="card-header d-flex justify-content-between align-items-center">
        <h4 class="card-title mb-0">{{ '::LowStockReport' | abpLocalization }}</h4>
        <div class="d-flex align-items-center gap-2">
            <select class="form-select" [(ngModel)]="warehouseId" (change)="loadReport()">
                <option [ngValue]="null">جميع المستودعات</option>
                <option *ngFor="let w of warehouses" [ngValue]="w.id">{{ w.name }}</option>
            </select>
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
            <thead class="bg-light">
              <tr>
                <th>{{ '::ProductName' | abpLocalization }}</th>
                <th>{{ '::Warehouse' | abpLocalization }}</th>
                <th>الكمية الحالية</th>
                <th>الحد الأدنى للطلب</th>
                <th>مقدار النقص</th>
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
                    لا يوجد نواقص في المخزون
                </td>
              </tr>
              <tr *ngFor="let item of reportData">
                <td>{{ item.productName }}</td>
                <td>{{ item.warehouseName }}</td>
                <td class="text-danger fw-bold">{{ item.currentQuantity | number:'1.0-2' }}</td>
                <td>{{ item.minStockLevel | number:'1.0-2' }}</td>
                <td class="text-danger">{{ item.deficit | number:'1.0-2' }}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  `
})
export class LowStockReportComponent implements OnInit {
  inventoryService = inject(InventoryService);

  warehouses: WarehouseDto[] = [];
  warehouseId: string | null = null;
  reportData: LowStockReportDto[] = [];
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
    this.inventoryService.getLowStockReport({ warehouseId: this.warehouseId as any }).subscribe({
      next: (data) => {
        this.reportData = data;
        this.isLoading = false;
      },
      error: () => this.isLoading = false
    });
  }

  print() {
    window.print();
  }
}
