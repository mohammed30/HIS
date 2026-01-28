import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common'; // Usually SharedModule covers this
import { SharedModule } from '../shared/shared.module';
import { ServicesRoutingModule } from './services-routing-module';
import { ServicesComponent } from './services/services';
import { PriceListsComponent } from './price-lists/price-lists';
import { RadiologyComponent } from './radiology/radiology';

@NgModule({
  declarations: [
    ServicesComponent,
    PriceListsComponent,
    RadiologyComponent
  ],
  imports: [
    SharedModule,
    ServicesRoutingModule
  ]
})
export class ServicesModule { }
