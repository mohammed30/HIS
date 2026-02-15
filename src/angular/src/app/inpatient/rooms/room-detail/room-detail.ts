import { Component, Input, OnInit, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { RoomService, RoomDto, RoomType, roomTypeOptions, RoomStatus, roomStatusOptions, BedDto } from '../../../proxy/rooms';
import { ToasterService } from '@abp/ng.theme.shared';

import { CoreModule } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { RoomBedListComponent } from '../room-bed-list/room-bed-list';

@Component({
  selector: 'app-room-detail',
  standalone: true,
  imports: [CoreModule, ThemeSharedModule, RoomBedListComponent],
  templateUrl: './room-detail.html'
})
export class RoomDetailComponent implements OnInit {
  activeModal = inject(NgbActiveModal);
  fb = inject(FormBuilder);
  roomService = inject(RoomService);
  toaster = inject(ToasterService);

  @Input() selectedId: string;

  form: FormGroup;
  roomTypes = roomTypeOptions;
  roomStatuses = roomStatusOptions;
  beds: BedDto[] = [];

  ngOnInit() {
    this.buildForm();
    if (this.selectedId) {
      this.roomService.get(this.selectedId).subscribe(res => {
        this.form.patchValue(res);
        this.beds = res.beds || [];
      });
    }
  }

  buildForm() {
    this.form = this.fb.group({
      roomNumber: ['', [Validators.required, Validators.maxLength(16)]],
      name: ['', [Validators.maxLength(64)]],
      type: [null, [Validators.required]],
      bedCount: [1, [Validators.required, Validators.min(1)]],
      dailyRate: [0, [Validators.required, Validators.min(0)]],
      floor: ['', [Validators.maxLength(16)]],
      status: [RoomStatus.Available, [Validators.required]],
      notes: ['', [Validators.maxLength(1024)]],
      amenities: ['', [Validators.maxLength(512)]],
    });
  }

  save() {
    if (this.form.invalid) return;

    const request = this.selectedId
      ? this.roomService.update(this.selectedId, this.form.value)
      : this.roomService.create(this.form.value);

    request.subscribe(() => {
      this.activeModal.close(true);
    });
  }
}
