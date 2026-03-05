import { ListService, PagedResultDto, CoreModule } from '@abp/ng.core';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AttendanceRecordDto, EmployeeLookupDto } from '../../proxy/hr/models';
import { HRService } from '../../proxy/hr/hr.service';
import { ConfirmationService, Confirmation, ThemeSharedModule } from '@abp/ng.theme.shared';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-attendance',
  standalone: true,
  imports: [CoreModule, ThemeSharedModule, ReactiveFormsModule, CommonModule, NgbDropdownModule],
  templateUrl: './attendance.html',
  styleUrls: ['./attendance.scss'],
  providers: [ListService],
})
export class Attendance implements OnInit {
  records: PagedResultDto<AttendanceRecordDto> = { items: [], totalCount: 0 };
  selectedRecord = {} as AttendanceRecordDto;
  isModalOpen = false;
  form: FormGroup;

  // Lookups
  employeeLookup: EmployeeLookupDto[] = [];

  permitTypes = [
    { id: 'Late Arrival', name: '::HR:LateArrival' },
    { id: 'Early Departure', name: '::HR:EarlyDeparture' },
    { id: 'Personal Permit', name: '::HR:PersonalPermit' },
    { id: 'Work Mission', name: '::HR:WorkMission' },
  ];

  constructor(
    public readonly list: ListService,
    private hrService: HRService,
    private fb: FormBuilder,
    private confirmation: ConfirmationService
  ) { }

  ngOnInit() {
    this.loadLookups();

    const streamCreator = (query) => this.hrService.getAttendanceRecords(query);
    this.list.hookToQuery(streamCreator).subscribe((response) => {
      this.records = response;
    });
  }

  loadLookups() {
    this.hrService.getEmployeeLookup().subscribe((res) => this.employeeLookup = res);
  }

  create() {
    this.selectedRecord = {} as AttendanceRecordDto;
    this.buildForm();
    this.isModalOpen = true;
  }

  buildForm() {
    this.form = this.fb.group({
      employeeId: [this.selectedRecord.employeeId || null, Validators.required],
      permitType: [this.selectedRecord.permitType || null, Validators.required],
      date: [
        this.selectedRecord.date ? new Date(this.selectedRecord.date).toISOString().split('T')[0] : new Date().toISOString().split('T')[0],
        Validators.required
      ],
      hours: [this.selectedRecord.hours || 0, [Validators.min(0), Validators.max(24)]],
      minutes: [this.selectedRecord.minutes || 0, [Validators.min(0), Validators.max(59)]],
      reason: [this.selectedRecord.reason || ''],
      notes: [this.selectedRecord.notes || ''],
    });
  }

  save() {
    if (this.form.invalid) return;

    this.hrService.createAttendanceRecord(this.form.value).subscribe(() => {
      this.isModalOpen = false;
      this.list.get();
    });
  }

  delete(id: string) {
    this.confirmation.warn('::AreYouSureToDelete', '::AreYouSure').subscribe((status) => {
      if (status === Confirmation.Status.confirm) {
        this.hrService.deleteAttendanceRecord(id).subscribe(() => this.list.get());
      }
    });
  }
}
