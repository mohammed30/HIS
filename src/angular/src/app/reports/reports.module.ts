import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReportsRoutingModule } from './reports-routing.module';
import { PaidTicketsComponent } from './paid-tickets/paid-tickets.component';
import { PharmacySalesComponent } from './pharmacy-sales/pharmacy-sales.component';
import { InsuranceReportsComponent } from './insurance-reports/insurance-reports.component';

@NgModule({
  imports: [
    CommonModule,
    ReportsRoutingModule,
    PaidTicketsComponent,
    PharmacySalesComponent,
    InsuranceReportsComponent,
  ],
})
export class ReportsModule {}
