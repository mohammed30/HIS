import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { DoctorRevenueReportService } from '../../proxy/settings/doctor-revenue-report.service';

interface DoctorRevenueLine {
  doctorId: string;
  doctorName: string;
  doctorCode: string;
  doctorPercentage: number;
  hospitalPercentage: number;
  totalRevenue: number;
  doctorAmount: number;
  hospitalAmount: number;
  accountCode: string | null;
}

interface DoctorRevenueReport {
  fromDate: string;
  toDate: string;
  lines: DoctorRevenueLine[];
  totalRevenue: number;
  totalDoctorAmount: number;
  totalHospitalAmount: number;
}

@Component({
  selector: 'app-doctor-revenue-report',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="container-fluid py-4" dir="rtl">
      <!-- Header -->
      <div class="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h4 class="mb-0 fw-bold"><i class="fas fa-chart-pie me-2 text-primary"></i>تقرير حق الطبيب والمستشفى</h4>
          <small class="text-muted">توزيع الإيرادات بين الأطباء والمستشفى حسب النسب المعرّفة</small>
        </div>
        <div class="d-flex gap-2">
          <button class="btn btn-success" (click)="printReport(false)">
            <i class="fas fa-print me-1"></i> طباعة حق الطبيب
          </button>
          <button class="btn btn-primary" (click)="printReport(true)">
            <i class="fas fa-print me-1"></i> طباعة حق المستشفى
          </button>
        </div>
      </div>

      <!-- Filters -->
      <div class="card mb-4 shadow-sm">
        <div class="card-body">
          <div class="row g-3 align-items-end">
            <div class="col-md-3">
              <label class="form-label fw-semibold">الطبيب</label>
              <select class="form-select" [(ngModel)]="filter.doctorId">
                <option value="">جميع الأطباء</option>
                @for (d of doctors; track d.id) {
                  <option [value]="d.id">{{ d.name }}</option>
                }
              </select>
            </div>
            <div class="col-md-3">
              <label class="form-label fw-semibold">من تاريخ</label>
              <input type="date" class="form-control" [(ngModel)]="filter.fromDate">
            </div>
            <div class="col-md-3">
              <label class="form-label fw-semibold">إلى تاريخ</label>
              <input type="date" class="form-control" [(ngModel)]="filter.toDate">
            </div>
            <div class="col-md-3">
              <button class="btn btn-primary w-100" (click)="loadReport()">
                <i class="fas fa-search me-1"></i> عرض التقرير
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Loading -->
      @if (isLoading) {
        <div class="text-center py-5">
          <div class="spinner-border text-primary" role="status"></div>
          <div class="mt-2 text-muted">جاري تحميل التقرير...</div>
        </div>
      }

      <!-- Report -->
      @if (!isLoading && report) {
        <div id="printable-area">
          <!-- Print Header -->
          <div class="print-only text-center mb-4">
            <h4 class="fw-bold">تقرير حق الطبيب والمستشفى</h4>
            <p class="text-muted">من {{ report.fromDate | date:'yyyy/MM/dd' }} إلى {{ report.toDate | date:'yyyy/MM/dd' }}</p>
          </div>

          <!-- Summary Cards -->
          <div class="row g-3 mb-4 no-print">
            <div class="col-md-4">
              <div class="card text-white bg-primary shadow">
                <div class="card-body text-center">
                  <div class="h6 mb-1">إجمالي الإيرادات</div>
                  <div class="h4 fw-bold">{{ report.totalRevenue | number:'1.2-2' }}</div>
                </div>
              </div>
            </div>
            <div class="col-md-4">
              <div class="card text-white bg-success shadow">
                <div class="card-body text-center">
                  <div class="h6 mb-1">إجمالي حق الأطباء</div>
                  <div class="h4 fw-bold">{{ report.totalDoctorAmount | number:'1.2-2' }}</div>
                </div>
              </div>
            </div>
            <div class="col-md-4">
              <div class="card text-white bg-info shadow">
                <div class="card-body text-center">
                  <div class="h6 mb-1">إجمالي حق المستشفى</div>
                  <div class="h4 fw-bold">{{ report.totalHospitalAmount | number:'1.2-2' }}</div>
                </div>
              </div>
            </div>
          </div>

          <!-- Report Table -->
          <div class="card shadow-sm">
            <div class="card-header d-flex justify-content-between align-items-center">
              <span class="fw-bold"><i class="fas fa-table me-2"></i>تفاصيل التقرير</span>
              <span class="badge bg-secondary">{{ report.lines.length }} طبيب</span>
            </div>
            <div class="table-responsive">
              <table class="table table-striped table-hover mb-0" id="reportTable">
                <thead class="table-dark">
                  <tr class="text-center">
                    <th><i class="fas fa-list"></i></th>
                    <th>الكود</th>
                    <th>اسم الطبيب</th>
                    <th>الحساب المحاسبي</th>
                    <th>نسبة الطبيب %</th>
                    <th>نسبة المستشفى %</th>
                    <th>إجمالي الإيرادات</th>
                    <th class="text-success">حق الطبيب</th>
                    <th class="text-primary">حق المستشفى</th>
                    <th>طباعة</th>
                  </tr>
                </thead>
                <tbody>
                  @for (line of report.lines; track line.doctorId; let i = $index) {
                    <tr class="text-center align-middle" (click)="toggleRow(line.doctorId!)" style="cursor: pointer;" [class.table-active]="expandedRows.has(line.doctorId!)">
                      <td><i class="fas" [class.fa-chevron-down]="expandedRows.has(line.doctorId!)" [class.fa-chevron-left]="!expandedRows.has(line.doctorId!)"></i></td>
                      <td><code>{{ line.doctorCode }}</code></td>
                      <td class="fw-semibold text-end">{{ line.doctorName }}</td>
                      <td><span class="badge bg-light text-dark border">{{ line.accountCode || '-' }}</span></td>
                      <td>
                        <div class="d-flex align-items-center gap-1">
                          <div class="progress flex-grow-1" style="height:8px;">
                            <div class="progress-bar bg-success" [style.width.%]="line.doctorPercentage"></div>
                          </div>
                          <span class="text-success fw-bold small">{{ line.doctorPercentage }}%</span>
                        </div>
                      </td>
                      <td>
                        <div class="d-flex align-items-center gap-1">
                          <div class="progress flex-grow-1" style="height:8px;">
                            <div class="progress-bar bg-primary" [style.width.%]="line.hospitalPercentage"></div>
                          </div>
                          <span class="text-primary fw-bold small">{{ line.hospitalPercentage }}%</span>
                        </div>
                      </td>
                      <td class="fw-semibold">{{ line.totalRevenue | number:'1.2-2' }}</td>
                      <td class="text-success fw-bold">{{ line.doctorAmount | number:'1.2-2' }}</td>
                      <td class="text-primary fw-bold">{{ line.hospitalAmount | number:'1.2-2' }}</td>
                      <td>
                        <button class="btn btn-sm btn-outline-success border-0" (click)="$event.stopPropagation(); printReport(false, line.doctorId)" title="طباعة تفصيلي">
                          <i class="fas fa-print"></i>
                        </button>
                      </td>
                    </tr>
                    @if (expandedRows.has(line.doctorId!)) {
                      <tr class="table-light">
                        <td colspan="10" class="p-3 text-start">
                          <div class="card shadow-sm border-0">
                             <div class="card-body p-0">
                                <table class="table table-sm table-bordered mb-0 text-center">
                                  <thead class="table-secondary">
                                    <tr>
                                      <th>التاريخ</th>
                                      <th>رقم الفاتورة</th>
                                      <th>اسم المريض</th>
                                      <th>الخدمة</th>
                                      <th>سعر الخدمة</th>
                                      <th class="text-success">حق الطبيب</th>
                                    </tr>
                                  </thead>
                                  <tbody>
                                    @for (detail of line.details || []; track $index) {
                                      <tr>
                                        <td>{{ detail.date | date:'yyyy/MM/dd' }}</td>
                                        <td>{{ detail.invoiceNumber }}</td>
                                        <td>{{ detail.patientName }}</td>
                                        <td>{{ detail.serviceName }}</td>
                                        <td>{{ detail.servicePrice | number:'1.2-2' }}</td>
                                        <td class="fw-bold text-success">{{ detail.doctorAmount | number:'1.2-2' }}</td>
                                      </tr>
                                    } @empty {
                                      <tr><td colspan="6" class="text-muted">لا توجد تفاصيل</td></tr>
                                    }
                                  </tbody>
                                </table>
                             </div>
                          </div>
                        </td>
                      </tr>
                    }
                  } @empty {
                    <tr>
                      <td colspan="10" class="text-center text-muted py-4">
                        <i class="fas fa-inbox fa-2x mb-2 d-block"></i>
                        لا توجد بيانات للفترة المحددة
                      </td>
                    </tr>
                  }
                </tbody>
                <tfoot class="table-dark fw-bold text-center">
                  <tr>
                    <td colspan="6">الإجماليات</td>
                    <td>{{ report.totalRevenue | number:'1.2-2' }}</td>
                    <td class="text-success">{{ report.totalDoctorAmount | number:'1.2-2' }}</td>
                    <td class="text-info">{{ report.totalHospitalAmount | number:'1.2-2' }}</td>
                    <td></td>
                  </tr>
                </tfoot>
              </table>
            </div>
          </div>
        </div>
      }
    </div>
  `,
  styles: [`
    @media print {
      .no-print { display: none !important; }
      .print-only { display: block !important; }
    }
    .print-only { display: none; }
    .table th, .table td { vertical-align: middle; }
  `]
})
export class DoctorRevenueReportComponent implements OnInit {
  private http = inject(HttpClient);
  private apiUrl = environment.apis.default.url;

  doctors: { id: string; name: string }[] = [];
  report: DoctorRevenueReport | null = null;
  isLoading = false;

  filter = {
    doctorId: '',
    fromDate: new Date(new Date().getFullYear(), new Date().getMonth(), 1).toISOString().split('T')[0],
    toDate: new Date().toISOString().split('T')[0]
  };

  expandedRows = new Set<string>();

  toggleRow(doctorId: string) {
    if (this.expandedRows.has(doctorId)) {
      this.expandedRows.delete(doctorId);
    } else {
      this.expandedRows.add(doctorId);
    }
  }

  ngOnInit() {
    this.loadDoctors();
    this.loadReport();
  }

  loadDoctors() {
    this.http.get<{ id: string; name: string }[]>(this.apiUrl + '/api/app/doctor/lookup').subscribe({
      next: (res) => this.doctors = res,
      error: (err) => console.error(err)
    });
  }

  loadReport() {
    this.isLoading = true;
    let url = `${this.apiUrl}/api/app/doctor-revenue-report/report?fromDate=${this.filter.fromDate}&toDate=${this.filter.toDate}`;
    if (this.filter.doctorId) url += `&doctorId=${this.filter.doctorId}`;

    this.http.get<DoctorRevenueReport>(url).subscribe({
      next: (res) => { this.report = res; this.isLoading = false; },
      error: (err) => { console.error(err); this.isLoading = false; }
    });
  }

  private reportService = inject(DoctorRevenueReportService);

  printReport(isHospital: boolean, doctorId?: string) {
    const input = {
      doctorId: doctorId || this.filter.doctorId || null,
      fromDate: this.filter.fromDate,
      toDate: this.filter.toDate,
      isHospitalReport: isHospital
    };

    this.reportService.getReportPdf(input).subscribe({
      next: (blob: Blob) => {
        const url = window.URL.createObjectURL(blob);
        const iframe = document.createElement('iframe');
        iframe.style.display = 'none';
        iframe.src = url;
        document.body.appendChild(iframe);
        iframe.contentWindow?.print();

        setTimeout(() => {
          document.body.removeChild(iframe);
          window.URL.revokeObjectURL(url);
        }, 10000);
      },
      error: (err) => {
        console.error('Error generating PDF', err);
        alert('حدث خطأ أثناء إنشاء التقرير');
      }
    });
  }
}
