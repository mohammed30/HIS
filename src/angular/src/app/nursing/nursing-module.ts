import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { NursingRoutingModule } from './nursing-routing-module';


import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { PatientListComponent } from './patient-list/patient-list.component';
import { PatientDashboardComponent } from './patient-dashboard/patient-dashboard.component';
import { VitalSignsComponent } from './vital-signs/vital-signs.component';
import { MedicationAdministrationComponent } from './medication-administration/medication-administration.component';

import { PatientCareDashboardComponent } from './patient-care-dashboard/patient-care-dashboard.component';
import { CarePlanComponent } from './care-plan/care-plan.component';
import { PatientRoundComponent } from './patient-round/patient-round.component';
import { AssessmentsComponent } from './assessments/assessments.component';
import { FluidBalanceComponent } from './fluid-balance/fluid-balance.component';
import { ShiftHandoverComponent } from './shift-handover/shift-handover.component';

import { CoreModule } from '@abp/ng.core';
import { NgbNavModule } from '@ng-bootstrap/ng-bootstrap';

@NgModule({
  declarations: [
    PatientListComponent,
    PatientDashboardComponent,
    VitalSignsComponent,
    MedicationAdministrationComponent,
    PatientCareDashboardComponent,
    CarePlanComponent,
    PatientRoundComponent,
    AssessmentsComponent,
    FluidBalanceComponent,
    ShiftHandoverComponent
  ],
  imports: [
    CoreModule,
    ThemeSharedModule,
    NursingRoutingModule,
    NgbNavModule
  ]
})
export class NursingModule { }
