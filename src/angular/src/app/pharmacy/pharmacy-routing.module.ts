import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PrescriptionsListComponent } from './prescriptions-list/prescriptions-list.component';
import { DispensingWorkflowComponent } from './dispensing-workflow/dispensing-workflow.component';
import { PharmacyStockComponent } from './pharmacy-stock/pharmacy-stock.component';
import { DrugsComponent } from './drugs/drugs.component';
import { StockDashboardComponent } from './stock-dashboard/stock-dashboard.component';
import { DispensingComponent } from './dispensing/dispensing.component';
import { PharmacyPosComponent } from './pos/pharmacy-pos.component';

const routes: Routes = [
    { path: '', component: PrescriptionsListComponent },
    { path: 'dispense/:id', component: DispensingWorkflowComponent },
    { path: 'stock', component: StockDashboardComponent },
    { path: 'dispensing', component: DispensingComponent },
    { path: 'pos', component: PharmacyPosComponent },
    { path: 'drugs', component: DrugsComponent },
    { path: 'inpatient-requests', loadComponent: () => import('../inventory/internal-requests/internal-requests').then(c => c.InternalRequestsComponent) }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class PharmacyRoutingModule { }
