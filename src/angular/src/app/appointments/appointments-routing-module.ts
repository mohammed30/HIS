import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DoctorScheduleComponent } from './doctor-schedule/doctor-schedule';
import { WaitingListComponent } from './waiting-list/waiting-list';
import { BookingComponent } from './booking/booking';

const routes: Routes = [
  { path: '', redirectTo: 'doctor-schedule', pathMatch: 'full' },
  {
    path: 'doctor-schedule',
    component: DoctorScheduleComponent
  },
  {
    path: 'waiting-list',
    component: WaitingListComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AppointmentsRoutingModule { }
