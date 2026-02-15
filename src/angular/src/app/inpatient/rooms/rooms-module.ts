import { NgModule } from '@angular/core';
import { CoreModule } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { NgbDropdownModule, NgbNavModule, NgbDatepickerModule } from '@ng-bootstrap/ng-bootstrap';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { RoomsRoutingModule } from './rooms-routing-module';
import { RoomListComponent } from './room-list/room-list';
import { RoomDetailComponent } from './room-detail/room-detail';
import { RoomBedListComponent } from './room-bed-list/room-bed-list';

@NgModule({
  declarations: [],
  imports: [
    CoreModule,
    ThemeSharedModule,
    NgbDropdownModule,
    NgbNavModule,
    NgbDatepickerModule,
    NgxDatatableModule,
    RoomsRoutingModule
  ]
})
export class RoomsModule { }
