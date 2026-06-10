import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgbNavModule, NgbDatepickerModule, NgbDateStruct, NgbCalendar } from '@ng-bootstrap/ng-bootstrap';
import { FinancialReportsService } from '../../proxy/accounting/financial-reports.service';
import { FinancialDashboardSummaryDto } from '../../proxy/accounting/models';
import { NgxEchartsModule, NGX_ECHARTS_CONFIG } from 'ngx-echarts';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { ThemeSharedModule } from '@abp/ng.theme.shared';

@Component({
  selector: 'app-financial-dashboard',
  standalone: true,
  imports: [
    CommonModule, 
    FormsModule, 
    ThemeSharedModule,
    NgbNavModule,
    NgbDatepickerModule,
    NgxEchartsModule
  ],
  providers: [
    {
      provide: NGX_ECHARTS_CONFIG,
      useFactory: () => ({ echarts: () => import('echarts') })
    }
  ],
  templateUrl: './financial-dashboard.html',
  styleUrls: ['./financial-dashboard.scss']
})
export class FinancialDashboardComponent implements OnInit, OnDestroy {
  summary: FinancialDashboardSummaryDto;
  isLoading = false;
  destroy$ = new Subject<void>();
  activeTab = 1;

  startDate: NgbDateStruct;
  endDate: NgbDateStruct;

  profitabilityChartOptions: any;

  constructor(
    private reportsService: FinancialReportsService,
    private calendar: NgbCalendar
  ) {
    this.endDate = this.calendar.getToday();
    this.startDate = { year: this.endDate.year, month: 1, day: 1 };
  }

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.isLoading = true;
    const start = new Date(this.startDate.year, this.startDate.month - 1, this.startDate.day).toISOString();
    const end = new Date(this.endDate.year, this.endDate.month - 1, this.endDate.day).toISOString();

    this.reportsService.getDashboardSummary(start, end)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res) => {
          this.summary = res;
          this.initChart();
          this.isLoading = false;
        },
        error: () => {
          this.isLoading = false;
        }
      });
  }

  initChart(): void {
    if (!this.summary || !this.summary.departmentProfitability) return;

    const data = this.summary.departmentProfitability.map(d => ({
      name: d.costCenterName || 'غير محدد',
      value: d.profit
    }));

    this.profitabilityChartOptions = {
      title: {
        text: 'أرباح مراكز التكلفة (الأقسام)',
        left: 'center',
        textStyle: { fontFamily: 'Tajawal, sans-serif' }
      },
      tooltip: {
        trigger: 'item',
        formatter: '{a} <br/>{b} : {c} ({d}%)'
      },
      legend: {
        orient: 'horizontal',
        bottom: 'bottom'
      },
      series: [
        {
          name: 'الربحية',
          type: 'pie',
          radius: '50%',
          data: data,
          emphasis: {
            itemStyle: {
              shadowBlur: 10,
              shadowOffsetX: 0,
              shadowColor: 'rgba(0, 0, 0, 0.5)'
            }
          }
        }
      ]
    };
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
