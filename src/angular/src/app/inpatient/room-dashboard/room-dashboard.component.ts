import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { RoomService } from '@proxy/rooms';
import { RoomDto } from '@proxy/rooms/models';
import { AdmissionService } from '@proxy/inpatient';
import { AdmissionDto } from '@proxy/inpatient/models';
import { CoreModule } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';

@Component({
  selector: 'app-room-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, CoreModule, ThemeSharedModule],
  template: `
    <div class="container-fluid p-4" style="background-color: #fdfdfd; min-height: 100vh;">
      <!-- Header & Search -->
      <div class="d-flex flex-wrap justify-content-between align-items-center mb-4 gap-3">
        <h2 class="m-0 fw-bold" style="color: #1e293b;">
          <i class="fas fa-bed me-2" style="color: #3b82f6;"></i> {{ '::Menu:Inpatient' | abpLocalization }}
        </h2>
        
        <div class="d-flex gap-2 flex-grow-1" style="max-width: 400px;">
          <div class="input-group shadow-sm rounded-3">
            <span class="input-group-text bg-white border-end-0 text-muted"><i class="fas fa-search"></i></span>
            <input type="text" class="form-control border-start-0 ps-0" placeholder="بحث باسم المريض، رقم الغرفة، السرير..." [(ngModel)]="searchTerm">
          </div>
          <button class="btn btn-primary shadow-sm" (click)="loadData()" style="border-radius: 8px;">
             <i class="fas fa-sync-alt"></i>
          </button>
        </div>
      </div>

      <!-- Floors Loop -->
      <div *ngFor="let floor of getFilteredFloors()" class="mb-5">
        <h4 class="mb-3 text-muted fw-bold d-flex align-items-center">
          <i class="fas fa-layer-group me-2"></i> {{ floor.floorName }}
          <span class="badge bg-secondary ms-2" style="font-size: 0.75rem;">{{ floor.rooms.length }} غرف</span>
        </h4>
        
        <div class="row g-4">
          <div class="col-12 col-md-6 col-lg-4 col-xl-3" *ngFor="let room of floor.rooms">
            
            <div class="card h-100 shadow-sm border-0" 
                 style="border-radius: 16px; overflow: hidden; transition: transform 0.2s;" 
                 [ngStyle]="{'border-bottom': '6px solid ' + getRoomTypeColor(room.type)}">
              
              <!-- Occupancy Status Header -->
              <div class="card-header border-0 d-flex justify-content-between align-items-center p-3" 
                   [ngClass]="getOccupancyBgClass(room)">
                <span class="badge bg-white text-dark shadow-sm px-2 py-1" style="border-radius: 8px; font-weight: 700; font-size: 0.8rem;">
                  {{ room.roomNumber }}
                </span>
                <span class="fw-bold" style="font-size: 0.85rem; color: #fff;">
                  {{ getOccupancyText(room) }}
                </span>
              </div>
              
              <div class="card-body p-3">
                <div class="d-flex justify-content-between mb-3 align-items-center">
                  <span class="badge" [ngStyle]="{'background-color': getRoomTypeColor(room.type), 'color': '#fff'}">
                    {{ getRoomTypeName(room.type) }}
                  </span>
                  <span class="fw-bold text-muted small" style="letter-spacing: 0.5px;">
                    <i class="fas fa-procedures me-1"></i> أسرة: {{ room.availableBeds }} / {{ room.bedCount }}
                  </span>
                </div>

                <!-- Patients List -->
                <div *ngIf="room.patients && room.patients.length > 0" class="mt-2">
                  <p class="text-muted small mb-1 fw-bold">المرضى الحاليين:</p>
                  <div class="d-flex flex-column gap-1">
                    <div *ngFor="let p of room.patients" class="p-2 rounded-2" style="background-color: #f1f5f9; font-size: 0.85rem;">
                      <i class="fas fa-user-injured text-primary me-2"></i>
                      <a [routerLink]="['/patients', p.patientId, 'medical-record']" class="fw-bold text-primary text-decoration-none hover-underline">
                        {{ p.patientName }}
                      </a>
                      <small class="text-muted d-block ms-4">سرير: {{ p.bedNumber || 'غير محدد' }}</small>
                    </div>
                  </div>
                </div>
                
                <div *ngIf="!room.patients || room.patients.length === 0" class="text-center py-3">
                  <span class="text-muted small"><i class="fas fa-info-circle me-1"></i> الغرفة فارغة تماماً</span>
                </div>
              </div>

              <div class="card-footer d-flex gap-2 bg-transparent border-0 p-3 pt-0">
                <button [routerLink]="['/inpatient/rooms']" [queryParams]="{ searchText: room.roomNumber }" class="btn btn-light btn-sm w-100 shadow-sm fw-bold text-primary" style="border-radius: 8px;">التفاصيل</button>
                <button *abpPermission="'HIS.Inpatient.Admissions.Create'" 
                        [routerLink]="['/inpatient/admissions']"
                        [queryParams]="{ roomId: room.id }"
                        class="btn btn-primary btn-sm w-100 shadow-sm fw-bold" 
                        style="border-radius: 8px;"
                        [disabled]="room.availableBeds === 0">
                  تسكين مريض
                </button>
              </div>
            </div>

          </div>
        </div>
      </div>
      
      <!-- Empty State -->
      <div *ngIf="getFilteredFloors().length === 0" class="text-center py-5">
        <i class="fas fa-search fa-3x text-muted mb-3 opacity-50"></i>
        <h4 class="text-muted">لا توجد غرف تطابق بحثك</h4>
      </div>
      
    </div>
  `
})
export class RoomDashboardComponent implements OnInit {
  private roomService = inject(RoomService);
  private admissionService = inject(AdmissionService);
  
  allRooms: (RoomDto & { patients?: AdmissionDto[] })[] = [];
  searchTerm: string = '';

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    // Load Rooms
    this.roomService.getList({ maxResultCount: 1000 }).subscribe(roomResult => {
      let rooms = roomResult.items || [];
      
      // Load Active Admissions (Status 0) to get patient names
      this.admissionService.getList({ status: 0, maxResultCount: 1000 }).subscribe(admResult => {
        const admissions = admResult.items || [];
        
        // Map admissions to rooms
        this.allRooms = rooms.map(room => {
          const roomPatients = admissions.filter(a => a.roomId === room.id);
          return { ...room, patients: roomPatients };
        });
      });
    });
  }

  getFilteredFloors() {
    let filteredRooms = this.allRooms;
    
    if (this.searchTerm && this.searchTerm.trim() !== '') {
      const term = this.searchTerm.toLowerCase();
      filteredRooms = this.allRooms.filter(r => 
        (r.roomNumber && r.roomNumber.toLowerCase().includes(term)) ||
        (r.patients && r.patients.some(p => 
          (p.patientName && p.patientName.toLowerCase().includes(term)) || 
          (p.bedNumber && p.bedNumber.toLowerCase().includes(term))
        ))
      );
    }

    // Group by floor
    const floorsMap = new Map<string, typeof filteredRooms>();
    filteredRooms.forEach(r => {
      const floorName = r.floor || 'أخرى (غير محدد)';
      if (!floorsMap.has(floorName)) {
        floorsMap.set(floorName, []);
      }
      floorsMap.get(floorName)!.push(r);
    });

    // Convert map to array and sort floors
    return Array.from(floorsMap.entries()).map(([floorName, rooms]) => ({
      floorName,
      rooms: rooms.sort((a, b) => (a.roomNumber || '').localeCompare(b.roomNumber || ''))
    })).sort((a, b) => a.floorName.localeCompare(b.floorName));
  }

  getOccupancyBgClass(room: RoomDto): string {
    if (room.availableBeds === 0) return 'bg-danger bg-gradient'; // Full
    if (room.availableBeds > 0 && room.availableBeds < room.bedCount) return 'bg-warning bg-gradient'; // Partially Full
    return 'bg-success bg-gradient'; // Empty / Available
  }

  getOccupancyText(room: RoomDto): string {
    if (room.availableBeds === 0) return 'مشغولة بالكامل';
    if (room.availableBeds > 0 && room.availableBeds < room.bedCount) return 'مشغولة جزئياً';
    return 'متاحة بالكامل';
  }

  getRoomTypeName(type: number): string {
    switch (type) {
      case 0: return 'عادية (Standard)';
      case 1: return 'خاصة (Private)';
      case 2: return 'عناية مركزة (ICU)';
      case 3: return 'جناح (Suite)';
      case 4: return 'عزل (Isolation)';
      default: return 'أخرى';
    }
  }

  getRoomTypeColor(type: number): string {
    switch (type) {
      case 0: return '#64748b'; // Slate (Standard)
      case 1: return '#8b5cf6'; // Violet (Private)
      case 2: return '#3b82f6'; // Blue (ICU)
      case 3: return '#f59e0b'; // Amber/Gold (Suite)
      case 4: return '#ec4899'; // Pink (Isolation)
      default: return '#94a3b8';
    }
  }
}
