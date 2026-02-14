import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PurchaseOrderListComponent } from './list/purchase-order-list.component';
import { PurchaseOrderDetailComponent } from './detail/purchase-order-detail.component';

const routes: Routes = [
    { path: '', component: PurchaseOrderListComponent },
    { path: 'create', component: PurchaseOrderDetailComponent },
    { path: 'edit/:id', component: PurchaseOrderDetailComponent }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class PurchaseOrdersRoutingModule { }
