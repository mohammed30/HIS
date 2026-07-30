import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PaidTicketsComponent } from './paid-tickets/paid-tickets.component';
import { PharmacySalesComponent } from './pharmacy-sales/pharmacy-sales.component';
import { InsuranceReportsComponent } from './insurance-reports/insurance-reports.component';
import { UserFinancialReportComponent } from './user-financial-report/user-financial-report.component';
import { UserActivityReportComponent } from './user-activity-report/user-activity-report.component';

const routes: Routes = [
  { path: 'paid-tickets', component: PaidTicketsComponent },
  { path: 'pharmacy-sales', component: PharmacySalesComponent },
  { path: 'insurance-reports', component: InsuranceReportsComponent },
  { path: 'user-financial-report', component: UserFinancialReportComponent },
  { path: 'user-activity-report', component: UserActivityReportComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ReportsRoutingModule {}
