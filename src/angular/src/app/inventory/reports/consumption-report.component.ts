import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CoreModule } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { InventoryService } from '@proxy/inventory';
import { RestService } from '@abp/ng.core';
import { DepartmentConsumptionReportDto } from '@proxy/inventory/dtos/models';

@Component({
  selector: 'app-consumption-report',
  standalone: true,
  imports: [CommonModule, FormsModule, CoreModule, ThemeSharedModule],
  template: `
    <div class="card">
      <div class="card-header d-flex justify-content-between align-items-center">
        <h4 class="card-title mb-0">معدل استهلاك المخزون (بالأقسام)</h4>
        <div class="d-flex align-items-center gap-2 flex-wrap">
            <div class="input-group" style="width: 200px;">
                <span class="input-group-text">من</span>
                <input type="date" class="form-control" [(ngModel)]="startDate">
            </div>
            <div class="input-group" style="width: 200px;">
                <span class="input-group-text">إلى</span>
                <input type="date" class="form-control" [(ngModel)]="endDate">
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
                <th>القسم / الإدارة</th>
                <th>{{ '::ProductName' | abpLocalization }}</th>
                <th>إجمالي الكمية المستهلكة</th>
                <th>إجمالي التكلفة</th>
              </tr>
            </thead>
            <tbody>
              <tr *ngIf="isLoading">
                <td colspan="4" class="text-center py-4">
                  <div class="spinner-border text-primary" role="status"></div>
                </td>
              </tr>
              <tr *ngIf="!isLoading && reportData.length === 0">
                <td colspan="4" class="text-center py-4 text-muted">
                    لا توجد بيانات استهلاك في هذه الفترة
                </td>
              </tr>
              <tr *ngFor="let item of reportData">
                <td class="fw-bold">{{ item.departmentName || 'اخرى' }}</td>
                <td>{{ item.productName }}</td>
                <td>{{ item.quantity | number:'1.0-2' }}</td>
                <td class="text-primary">{{ item.totalCost | number:'1.2-2' }}</td>
              </tr>
              <!-- Subtotals per department could be added conceptually here -->
            </tbody>
            <tfoot class="fw-bold" *ngIf="reportData.length > 0">
                <tr>
                    <td colspan="3" class="text-end">الإجمالي العام:</td>
                    <td class="text-primary">{{ getTotalCost() | number:'1.2-2' }}</td>
                </tr>
            </tfoot>
          </table>
        </div>
      </div>
    </div>
  `
})
export class ConsumptionReportComponent implements OnInit {
  inventoryService = inject(InventoryService);
  restService = inject(RestService);

  reportData: DepartmentConsumptionReportDto[] = [];
  isLoading = false;

  startDate: string;
  endDate: string;

  constructor() {
    const today = new Date();
    const firstDay = new Date(today.getFullYear(), today.getMonth(), 1);

    this.endDate = today.toISOString().split('T')[0];
    this.startDate = firstDay.toISOString().split('T')[0];
  }

  ngOnInit() {
    this.loadReport();
  }

  loadReport() {
    if (!this.startDate || !this.endDate) return;

    this.isLoading = true;
    this.inventoryService.getConsumptionReport({
      startDate: this.startDate,
      endDate: this.endDate
    }).subscribe({
      next: (data) => {
        this.reportData = data;
        this.isLoading = false;
      },
      error: () => this.isLoading = false
    });
  }

  getTotalCost(): number {
    return this.reportData.reduce((sum, item) => sum + (Number(item.totalCost) || 0), 0);
  }

  print() {
    this.isLoading = true;
    this.restService.request<any, Blob>({
      method: 'GET',
      url: '/api/app/inventory/reports/consumption/pdf',
      params: {
        startDate: this.startDate,
        endDate: this.endDate
      },
      responseType: 'blob'
    }).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `Consumption_Report_${new Date().getTime()}.pdf`;
        link.click();
        window.URL.revokeObjectURL(url);
        this.isLoading = false;
      },
      error: () => this.isLoading = false
    });
  }
}
