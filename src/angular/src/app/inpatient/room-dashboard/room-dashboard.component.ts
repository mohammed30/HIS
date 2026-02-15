import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RoomService } from '@proxy/rooms';
import { RoomDto } from '@proxy/rooms/models';
import { CoreModule } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';

@Component({
  selector: 'app-room-dashboard',
  standalone: true,
  imports: [CommonModule, CoreModule, ThemeSharedModule],
  template: `
    <div class="container-fluid p-4">
      <div class="d-flex justify-content-between align-items-center mb-4">
        <h2 class="m-0"><i class="fas fa-bed me-2"></i> {{ '::Menu:Inpatient' | abpLocalization }}</h2>
        <button class="btn btn-primary" (click)="loadRooms()">
           <i class="fas fa-sync-alt"></i>
        </button>
      </div>

      <div class="row g-4">
        <div class="col-12 col-md-6 col-lg-3" *ngFor="let room of rooms">
          <div class="card h-100 shadow-sm">
            <div class="card-header d-flex justify-content-between align-items-center">
              <h5 class="m-0">{{ room.roomNumber }}</h5>
              <span class="badge" [ngClass]="room.status === 0 ? 'bg-success' : 'bg-danger'">
                {{ room.status === 0 ? 'Available' : 'Occupied' }}
              </span>
            </div>
            
            <div class="card-body">
              <div class="d-flex justify-content-between mb-2">
                <span class="text-muted small">Type:</span>
                <span class="fw-bold">{{ getRoomTypeName(room.type) }}</span>
              </div>
              <div class="d-flex justify-content-between mb-2">
                <span class="text-muted small">Beds:</span>
                <span class="fw-bold">{{ room.availableBeds }} / {{ room.bedCount }}</span>
              </div>
              <div class="d-flex justify-content-between">
                <span class="text-muted small">Rate:</span>
                <span class="fw-bold text-primary">{{ room.dailyRate | currency }}</span>
              </div>
            </div>

            <div class="card-footer d-flex gap-2 bg-transparent">
              <button class="btn btn-outline-primary btn-sm w-100">Details</button>
              <button *abpPermission="'HIS.Inpatient.Admissions.Create'" class="btn btn-primary btn-sm w-100" [disabled]="room.availableBeds === 0">Assign</button>
            </div>
          </div>
        </div>
      </div>
    </div>
  `
})
export class RoomDashboardComponent implements OnInit {
  private roomService = inject(RoomService);
  rooms: RoomDto[] = [];

  ngOnInit() {
    this.loadRooms();
  }

  loadRooms() {
    this.roomService.getList({ maxResultCount: 100 }).subscribe(result => {
      this.rooms = result.items || [];
    });
  }

  getRoomTypeName(type: number): string {
    // Basic mapping for now, should ideally use localization
    switch (type) {
      case 0: return 'Private';
      case 1: return 'Semi-Private';
      case 2: return 'Ward';
      case 3: return 'ICU';
      default: return 'Other';
    }
  }
}
