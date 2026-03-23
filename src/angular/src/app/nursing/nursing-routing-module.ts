import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PatientListComponent } from './patient-list/patient-list.component';
import { PatientDashboardComponent } from './patient-dashboard/patient-dashboard.component';
import { PatientCareDashboardComponent } from './patient-care-dashboard/patient-care-dashboard.component';
import { ShiftHandoverComponent } from './shift-handover/shift-handover.component';
import { InternalRequestsComponent } from './internal-requests/internal-requests';

const routes: Routes = [
  { path: '', redirectTo: 'patient-list', pathMatch: 'full' },
  { path: 'patient-list', component: PatientListComponent },
  { path: 'patient/:patientId', component: PatientDashboardComponent },
  { path: 'patient-care/:patientId', component: PatientCareDashboardComponent },
  { path: 'shift-handover', component: ShiftHandoverComponent },
  { path: 'internal-requests', component: InternalRequestsComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class NursingRoutingModule { }
