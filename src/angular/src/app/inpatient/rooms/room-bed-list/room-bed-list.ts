import { Component, Input } from '@angular/core';
import { BedDto } from '../../../proxy/rooms';

import { CoreModule } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-room-bed-list',
  standalone: true,
  imports: [CoreModule, ThemeSharedModule, CommonModule],
  templateUrl: './room-bed-list.html',
  styleUrls: ['./room-bed-list.scss']
})
export class RoomBedListComponent {
  @Input() beds: BedDto[] = [];
}
