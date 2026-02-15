import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ReservationListComponent } from './reservation-list/reservation-list';

const routes: Routes = [{ path: '', loadComponent: () => import('./reservation-list/reservation-list').then(m => m.ReservationListComponent) }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ReservationsRoutingModule { }
