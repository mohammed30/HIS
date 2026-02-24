import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService, LocalizationModule, LocalizationService } from '@abp/ng.core';
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
export class HomeComponent {
  private authService = inject(AuthService);
  private localizationService = inject(LocalizationService);

  get hasLoggedIn(): boolean {
    return this.authService.isAuthenticated;
  }

  // Dashboard Summary Data
  totalDoctors = 45;
  totalPatients = 1250;
  totalRooms = 120;
  occupancyRate = 75;

  visitsChartOption: any;
  roomsChartOption: any;

  constructor() {
    this.initCharts();
  }

  private initCharts() {
    const months = [
      'Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun',
      'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'
    ].map(m => this.localizationService.instant(`::Month:${m}`));

    this.visitsChartOption = {
      tooltip: { trigger: 'axis' },
      xAxis: {
        type: 'category',
        data: months,
      },
      yAxis: { type: 'value' },
      series: [
        {
          data: [120, 150, 180, 130, 210, 250, 230, 190, 260, 290, 220, 310],
          type: 'bar',
          itemStyle: { color: '#0d6efd' },
          name: this.localizationService.instant('::Visits')
        },
      ],
    };

    this.roomsChartOption = {
      tooltip: { trigger: 'item' },
      legend: { top: 'bottom' },
      series: [
        {
          name: this.localizationService.instant('::Rooms'),
          type: 'pie',
          radius: ['40%', '70%'],
          avoidLabelOverlap: false,
          itemStyle: {
            borderRadius: 10,
            borderColor: '#fff',
            borderWidth: 2
          },
          label: { show: false, position: 'center' },
          emphasis: {
            label: { show: true, fontSize: 20, fontWeight: 'bold' }
          },
          labelLine: { show: false },
          data: [
            { value: 90, name: this.localizationService.instant('::Occupied'), itemStyle: { color: '#dc3545' } },
            { value: 25, name: this.localizationService.instant('::Available'), itemStyle: { color: '#198754' } },
            { value: 5, name: this.localizationService.instant('::Maintenance'), itemStyle: { color: '#ffc107' } }
          ]
        }
      ]
    };
  }

  login() {
    this.authService.navigateToLogin();
  }
}

