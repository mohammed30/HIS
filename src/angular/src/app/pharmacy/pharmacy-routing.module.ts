import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PrescriptionsListComponent } from './prescriptions-list/prescriptions-list.component';
import { DispensingWorkflowComponent } from './dispensing-workflow/dispensing-workflow.component';
import { PharmacyStockComponent } from './pharmacy-stock/pharmacy-stock.component';

const routes: Routes = [
    { path: '', component: PrescriptionsListComponent },
    { path: 'dispense/:id', component: DispensingWorkflowComponent },
    { path: 'stock', component: PharmacyStockComponent }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class PharmacyRoutingModule { }
