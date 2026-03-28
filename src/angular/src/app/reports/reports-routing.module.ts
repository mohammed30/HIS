import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PaidTicketsComponent } from './paid-tickets/paid-tickets.component';
import { PharmacySalesComponent } from './pharmacy-sales/pharmacy-sales.component';
import { InsuranceReportsComponent } from './insurance-reports/insurance-reports.component';

const routes: Routes = [
  { path: 'paid-tickets', component: PaidTicketsComponent },
  { path: 'pharmacy-sales', component: PharmacySalesComponent },
  { path: 'insurance-reports', component: InsuranceReportsComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ReportsRoutingModule {}
