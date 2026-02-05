import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { PharmacyRoutingModule } from './pharmacy-routing.module';
import { PrescriptionsListComponent } from './prescriptions-list/prescriptions-list.component';
import { DispensingWorkflowComponent } from './dispensing-workflow/dispensing-workflow.component';
import { PharmacyStockComponent } from './pharmacy-stock/pharmacy-stock.component';
import { DrugsComponent } from './drugs/drugs.component';
import { DrugDialogComponent } from './drugs/drug-dialog/drug-dialog.component';

@NgModule({
    declarations: [],
    imports: [
        CommonModule,
        ThemeSharedModule,
        PharmacyRoutingModule,
        PrescriptionsListComponent,
        PharmacyRoutingModule,
        PrescriptionsListComponent,
        DispensingWorkflowComponent,
        PharmacyStockComponent,
        DrugsComponent,
        DrugDialogComponent
    ]
})
export class PharmacyModule { }
