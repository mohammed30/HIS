import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { PharmacyRoutingModule } from './pharmacy-routing.module';
import { PrescriptionsListComponent } from './prescriptions-list/prescriptions-list.component';
import { DispensingWorkflowComponent } from './dispensing-workflow/dispensing-workflow.component';
import { PharmacyStockComponent } from './pharmacy-stock/pharmacy-stock.component';

@NgModule({
    declarations: [],
    imports: [
        CommonModule,
        ThemeSharedModule,
        PharmacyRoutingModule,
        PrescriptionsListComponent,
        DispensingWorkflowComponent,
        PharmacyStockComponent
    ]
})
export class PharmacyModule { }
