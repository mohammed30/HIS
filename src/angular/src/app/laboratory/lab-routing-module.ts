import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LabCatalogComponent } from './catalog/lab-catalog.component';
import { LabRequestsComponent } from './requests/lab-requests.component';
import { LabAppointmentsComponent } from './appointments/lab-appointments.component';

const routes: Routes = [
    { path: '', redirectTo: 'catalog', pathMatch: 'full' },
    { path: 'catalog', component: LabCatalogComponent },
    { path: 'requests', component: LabRequestsComponent },
    { path: 'appointments', component: LabAppointmentsComponent },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class LabRoutingModule { }

