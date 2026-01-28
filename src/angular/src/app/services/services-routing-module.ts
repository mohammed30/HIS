import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ServicesComponent } from './services/services';
import { PriceListsComponent } from './price-lists/price-lists';
import { RadiologyComponent } from './radiology/radiology';

const routes: Routes = [
  { path: '', component: ServicesComponent },
  { path: 'price-lists', component: PriceListsComponent },
  { path: 'radiology', component: RadiologyComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ServicesRoutingModule { }
