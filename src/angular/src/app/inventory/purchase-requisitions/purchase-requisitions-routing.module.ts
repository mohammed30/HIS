import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PurchaseRequisitionListComponent } from './list/purchase-requisition-list.component';
import { PurchaseRequisitionDetailComponent } from './detail/purchase-requisition-detail.component';

const routes: Routes = [
    { path: '', component: PurchaseRequisitionListComponent },
    { path: 'create', component: PurchaseRequisitionDetailComponent },
    { path: 'edit/:id', component: PurchaseRequisitionDetailComponent }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class PurchaseRequisitionsRoutingModule { }
