import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { permissionGuard } from '@abp/ng.core';
import { DoctorScheduleComponent } from './doctor-schedule/doctor-schedule';
import { WaitingListComponent } from './waiting-list/waiting-list';
import { BookingComponent } from './booking/booking';

const routes: Routes = [
  { path: '', redirectTo: 'booking', pathMatch: 'full' },
  {
    path: 'booking',
    loadComponent: () => import('./booking/booking').then(c => c.BookingComponent),
    canActivate: [permissionGuard],
    data: { requiredPolicy: 'HIS.Appointments.Create' }
  },
  {
    path: 'my-appointments',
    loadComponent: () => import('./my-appointments/my-appointments').then(c => c.MyAppointmentsComponent)
  },
  {
    path: 'doctor-schedule',
    loadComponent: () => import('./doctor-schedule/doctor-schedule').then(c => c.DoctorScheduleComponent)
  },
  {
    path: 'waiting-list',
    loadComponent: () => import('./waiting-list/waiting-list').then(c => c.WaitingListComponent)
  },
  {
    path: 'flow',
    loadComponent: () => import('./clinic-flow/clinic-flow.component').then(m => m.ClinicFlowComponent)
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AppointmentsRoutingModule { }
