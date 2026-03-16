import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PaidTicketsComponent } from './paid-tickets/paid-tickets.component';
import { PharmacySalesComponent } from './pharmacy-sales/pharmacy-sales.component';

const routes: Routes = [
  { path: 'paid-tickets', component: PaidTicketsComponent },
  { path: 'pharmacy-sales', component: PharmacySalesComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ReportsRoutingModule {}
