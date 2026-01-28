import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { ServicesRoutingModule } from './services-routing-module';
import { ServicesComponent } from './services/services';
import { PriceListsComponent } from './price-lists/price-lists';
import { RadiologyComponent } from './radiology/radiology';

@NgModule({
  declarations: [],
  imports: [
    ThemeSharedModule,
    ServicesRoutingModule,
    ServicesComponent,
    PriceListsComponent,
    RadiologyComponent
  ]
})
export class ServicesModule { }
