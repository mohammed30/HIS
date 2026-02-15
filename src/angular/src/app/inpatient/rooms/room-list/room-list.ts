import { ListService, PagedResultDto } from '@abp/ng.core';
import { Component, OnInit, inject } from '@angular/core';
import { RoomService, RoomDto, RoomType, roomTypeOptions, RoomStatus, roomStatusOptions } from '../../../proxy/rooms';
import { ConfirmationService, Confirmation } from '@abp/ng.theme.shared';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { RoomDetailComponent } from '../room-detail/room-detail';
import { ToasterService } from '@abp/ng.theme.shared';

import { CoreModule } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';

@Component({
  selector: 'app-room-list',
  standalone: true,
  imports: [CoreModule, ThemeSharedModule, NgbDropdownModule, NgxDatatableModule],
  templateUrl: './room-list.html',
  styleUrls: ['./room-list.scss'],
  providers: [ListService],
})
export class RoomListComponent implements OnInit {
  roomService = inject(RoomService);
  confirmation = inject(ConfirmationService);
  modalService = inject(NgbModal);
  list = inject(ListService);
  toaster = inject(ToasterService);

  data: PagedResultDto<RoomDto> = { items: [], totalCount: 0 };
  roomTypes = roomTypeOptions;
  roomStatuses = roomStatusOptions;

  ngOnInit() {
    this.list.hookToQuery(query => this.roomService.getList(query)).subscribe(res => {
      this.data = res;
    });
  }

  createRoom() {
    const modal = this.modalService.open(RoomDetailComponent, { size: 'lg' });
    modal.result.then((result) => {
      if (result) {
        this.list.get();
        this.toaster.success('Successfully Created');
      }
    }, () => { });
  }

  editRoom(id: string) {
    const modal = this.modalService.open(RoomDetailComponent, { size: 'lg' });
    modal.componentInstance.selectedId = id;
    modal.result.then((result) => {
      if (result) {
        this.list.get();
        this.toaster.success('Successfully Updated');
      }
    }, () => { });
  }

  deleteRoom(id: string) {
    this.confirmation.warn('::AreYouSureToDelete', '::AreYouSure').subscribe((status) => {
      if (status === Confirmation.Status.confirm) {
        this.roomService.delete(id).subscribe(() => {
          this.list.get();
          this.toaster.success('Successfully Deleted');
        });
      }
    });
  }

  getRoomType(value: number): string {
    return this.roomTypes.find(x => x.value === value)?.key || '';
  }

  getRoomStatus(value: number): string {
    return this.roomStatuses.find(x => x.value === value)?.key || '';
  }
}
