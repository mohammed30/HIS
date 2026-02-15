import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RoomListComponent } from './room-list/room-list';

const routes: Routes = [{ path: '', loadComponent: () => import('./room-list/room-list').then(m => m.RoomListComponent) }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RoomsRoutingModule { }
