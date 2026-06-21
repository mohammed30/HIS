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
  private themeObserver: MutationObserver;

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
    this.observeThemeChanges();
  }

  observeThemeChanges(): void {
    if (typeof MutationObserver === 'undefined') return;

    this.themeObserver = new MutationObserver(() => {
      if (this.summary) {
        this.initChart();
      }
    });

    this.themeObserver.observe(document.documentElement, {
      attributes: true,
      attributeFilter: ['data-theme', 'class']
    });
    this.themeObserver.observe(document.body, {
      attributes: true,
      attributeFilter: ['data-theme', 'class']
    });
  }

  loadData(): void {
    this.isLoading = true;
    const start = `${this.startDate.year}-${String(this.startDate.month).padStart(2, '0')}-${String(this.startDate.day).padStart(2, '0')}`;
    const end = `${this.endDate.year}-${String(this.endDate.month).padStart(2, '0')}-${String(this.endDate.day).padStart(2, '0')}`;

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

  isDownloading = false;
  downloadProfitabilityReport(): void {
    this.isDownloading = true;
    const start = `${this.startDate.year}-${String(this.startDate.month).padStart(2, '0')}-${String(this.startDate.day).padStart(2, '0')}`;
    const end = `${this.endDate.year}-${String(this.endDate.month).padStart(2, '0')}-${String(this.endDate.day).padStart(2, '0')}`;

    this.reportsService.getDepartmentProfitabilityReport(start, end)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (blob) => {
          const url = window.URL.createObjectURL(blob);
          const link = document.createElement('a');
          link.href = url;
          link.download = `تقرير_ربحية_الأقسام.pdf`;
          link.click();
          window.URL.revokeObjectURL(url);
          this.isDownloading = false;
        },
        error: () => {
          this.isDownloading = false;
        }
      });
  }

  checkIsDark(): boolean {
    if (typeof window === 'undefined') return false;
    
    // 1. Check data-theme attributes
    const docTheme = document.documentElement.getAttribute('data-theme');
    const bodyTheme = document.body.getAttribute('data-theme');
    if (docTheme === 'dark' || bodyTheme === 'dark') return true;
    if (docTheme === 'light' || bodyTheme === 'light') return false;
    
    // 2. Check classes
    const darkClasses = ['dark', 'lpx-theme-dark', 'theme-dark'];
    const hasDarkClass = darkClasses.some(cls => 
      document.documentElement.classList.contains(cls) || 
      document.body.classList.contains(cls)
    );
    if (hasDarkClass) return true;

    // 3. Fallback to computed styles
    try {
      const bodyStyles = window.getComputedStyle(document.body);
      const bgColor = bodyStyles.backgroundColor;
      const textColor = bodyStyles.color;

      if (textColor.includes('255') || textColor.includes('248') || textColor.includes('250')) {
        return true;
      }

      const rgb = bgColor.match(/\d+/g);
      if (rgb && rgb.length >= 3) {
        const r = parseInt(rgb[0], 10);
        const g = parseInt(rgb[1], 10);
        const b = parseInt(rgb[2], 10);
        const brightness = (r * 299 + g * 587 + b * 114) / 1000;
        return brightness < 128;
      }
    } catch (e) {
      // Ignore styles check if failed
    }

    return false;
  }

  chartInstance: any;
  onChartInit(ec: any): void {
    this.chartInstance = ec;
  }

  initChart(): void {
    if (!this.summary || !this.summary.departmentProfitability) return;

    const isDark = this.checkIsDark();
    const textColor = isDark ? '#ffffff' : '#212529';
    const subTextColor = isDark ? '#ced4da' : '#6c757d';

    const data = this.summary.departmentProfitability.map(d => ({
      name: d.costCenterName || 'غير محدد',
      value: d.profit
    }));

    this.profitabilityChartOptions = {
      backgroundColor: 'transparent',
      title: {
        text: 'أرباح مراكز التكلفة (الأقسام)',
        left: 'center',
        textStyle: { 
          fontFamily: 'Tajawal, sans-serif',
          color: textColor
        }
      },
      tooltip: {
        trigger: 'item',
        formatter: '{a} <br/>{b} : {c} ({d}%)'
      },
      legend: {
        orient: 'horizontal',
        bottom: 'bottom',
        textStyle: {
          color: subTextColor,
          fontFamily: 'Tajawal, sans-serif'
        }
      },
      series: [
        {
          name: 'الربحية',
          type: 'pie',
          radius: '50%',
          data: data,
          label: {
            show: true,
            color: textColor,
            fontFamily: 'Tajawal, sans-serif'
          },
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

    if (this.chartInstance) {
      this.chartInstance.setOption(this.profitabilityChartOptions, true);
    }
  }

  ngOnDestroy(): void {
    if (this.themeObserver) {
      this.themeObserver.disconnect();
    }
    this.destroy$.next();
    this.destroy$.complete();
  }
}
