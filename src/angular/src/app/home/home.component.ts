import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService, LocalizationModule, LocalizationService, RestService } from '@abp/ng.core';
import { NgxEchartsDirective, provideEchartsCore } from 'ngx-echarts';

@Component({
  standalone: true,
  imports: [
    CommonModule,
    LocalizationModule,
    NgxEchartsDirective,
  ],
  providers: [
    provideEchartsCore({ echarts: () => import('echarts') }),
  ],
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent implements OnInit {
  private authService = inject(AuthService);
  private localizationService = inject(LocalizationService);
  private restService = inject(RestService);

  get hasLoggedIn(): boolean {
    return this.authService.isAuthenticated;
  }

  // Dashboard Summary Data
  totalDoctors = 0;
  totalPatients = 0;
  totalRooms = 0;
  occupancyRate = 0;

  visitsChartOption: any;
  roomsChartOption: any;

  constructor() {
    this.initEmptyCharts();
  }

  ngOnInit() {
    if (this.hasLoggedIn) {
      this.loadDashboardData();
    }
  }

  private loadDashboardData() {
    this.restService.request<any, any>({
      method: 'GET',
      url: '/api/app/dashboard/summary'
    }).subscribe((data: any) => {
      this.totalDoctors = data.totalDoctors;
      this.totalPatients = data.totalPatients;
      this.totalRooms = data.totalRooms;
      this.occupancyRate = data.occupancyRate;

      this.updateCharts(data);
    });
  }

  private initEmptyCharts() {
    const months = [
      'Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun',
      'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'
    ].map(m => this.localizationService.instant(`::Month:${m}`));

    this.visitsChartOption = {
      tooltip: { 
        trigger: 'axis', 
        backgroundColor: '#ffffff', 
        textStyle: { color: '#111827', fontWeight: 'bold' },
        extraCssText: 'box-shadow: 0 4px 12px rgba(0,0,0,0.1); border: 1px solid #e5e7eb; border-radius: 8px;'
      },
      xAxis: { type: 'category', data: months, axisLine: { lineStyle: { color: '#94a3b8' } } },
      yAxis: { type: 'value', splitLine: { lineStyle: { color: 'rgba(148, 163, 184, 0.2)' } }, axisLabel: { color: '#94a3b8' } },
      series: [
        {
          data: [],
          type: 'bar',
          barWidth: '40%',
          itemStyle: { 
            color: '#0ea5e9',
            borderRadius: [6, 6, 0, 0]
          },
          name: this.localizationService.instant('::Visits')
        }
      ]
    };

    this.roomsChartOption = {
      tooltip: { 
        trigger: 'item', 
        backgroundColor: '#ffffff', 
        textStyle: { color: '#111827', fontWeight: 'bold' },
        extraCssText: 'box-shadow: 0 4px 12px rgba(0,0,0,0.1); border: 1px solid #e5e7eb; border-radius: 8px;'
      },
      legend: { top: 'bottom', textStyle: { color: '#94a3b8' } },
      series: [
        {
          name: this.localizationService.instant('::Rooms'),
          type: 'pie',
          radius: ['55%', '80%'],
          avoidLabelOverlap: false,
          itemStyle: {
            borderRadius: 8,
            borderColor: '#ffffff',
            borderWidth: 2
          },
          label: { show: false, position: 'center' },
          emphasis: {
            label: { show: true, fontSize: 24, fontWeight: 'bold', color: 'inherit' }
          },
          labelLine: { show: false },
          data: []
        }
      ]
    };
  }

  private updateCharts(data: any) {
    // Process monthly visits
    const visitCounts = data.monthlyVisits.map((x: any) => x.count);
    this.visitsChartOption = {
      ...this.visitsChartOption,
      series: [{
        ...this.visitsChartOption.series[0],
        data: visitCounts
      }]
    };

    // Process room status
    this.roomsChartOption = {
      ...this.roomsChartOption,
      series: [{
        ...this.roomsChartOption.series[0],
        data: [
          { value: data.roomStatus.occupied, name: this.localizationService.instant('::Occupied'), itemStyle: { color: '#f43f5e' } },
          { value: data.roomStatus.available, name: this.localizationService.instant('::Available'), itemStyle: { color: '#10b981' } },
          { value: data.roomStatus.maintenance, name: this.localizationService.instant('::Maintenance'), itemStyle: { color: '#f59e0b' } }
        ]
      }]
    };
  }

  login() {
    this.authService.navigateToLogin();
  }
}
