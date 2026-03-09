import { ListService, PagedResultDto, CoreModule } from '@abp/ng.core';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { DailyAttendanceDto, CreateUpdateDailyAttendanceDto, EmployeeLookupDto } from '../../proxy/hr/models';
import { HRService } from '../../proxy/hr/hr.service';
import { ConfirmationService, Confirmation, ThemeSharedModule } from '@abp/ng.theme.shared';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';

@Component({
    selector: 'app-daily-attendance',
    standalone: true,
    imports: [CoreModule, ThemeSharedModule, ReactiveFormsModule, CommonModule, NgbDropdownModule],
    templateUrl: './daily-attendance.html',
    styleUrls: ['./daily-attendance.scss'],
    providers: [ListService],
})
export class DailyAttendanceComponent implements OnInit {
    records: PagedResultDto<DailyAttendanceDto> = { items: [], totalCount: 0 };
    selectedRecord = {} as DailyAttendanceDto;
    isModalOpen = false;
    isEditing = false;
    form: FormGroup;

    employeeLookup: EmployeeLookupDto[] = [];

    statusOptions = [
        { value: 0, label: 'حاضر' },
        { value: 1, label: 'غائب' },
        { value: 2, label: 'متأخر' },
        { value: 3, label: 'انصراف مبكر' },
        { value: 4, label: 'إجازة' },
    ];

    constructor(
        public readonly list: ListService,
        private hrService: HRService,
        private fb: FormBuilder,
        private confirmation: ConfirmationService
    ) { }

    ngOnInit() {
        this.loadLookups();

        const streamCreator = (query) => this.hrService.getDailyAttendance(query);
        this.list.hookToQuery(streamCreator).subscribe((response) => {
            this.records = response;
        });
    }

    loadLookups() {
        this.hrService.getEmployeeLookup().subscribe((res) => this.employeeLookup = res);
    }

    getStatusLabel(status: number): string {
        const opt = this.statusOptions.find(s => s.value === status);
        return opt ? opt.label : '';
    }

    getStatusClass(status: number): string {
        switch (status) {
            case 0: return 'badge bg-success';
            case 1: return 'badge bg-danger';
            case 2: return 'badge bg-warning text-dark';
            case 3: return 'badge bg-info';
            case 4: return 'badge bg-secondary';
            default: return 'badge bg-light';
        }
    }

    formatTime(dateStr: string | null | undefined): string {
        if (!dateStr) return '---';
        const d = new Date(dateStr);
        return d.toLocaleTimeString('ar-EG', { hour: '2-digit', minute: '2-digit' });
    }

    create() {
        this.selectedRecord = {} as DailyAttendanceDto;
        this.isEditing = false;
        this.buildForm();
        this.isModalOpen = true;
    }

    edit(record: DailyAttendanceDto) {
        this.selectedRecord = record;
        this.isEditing = true;
        this.buildForm();
        this.isModalOpen = true;
    }

    buildForm() {
        const now = new Date();
        const todayStr = now.toISOString().split('T')[0];
        const nowTimeStr = now.toTimeString().slice(0, 5);

        this.form = this.fb.group({
            employeeId: [this.selectedRecord.employeeId || null, Validators.required],
            date: [
                this.selectedRecord.date ? new Date(this.selectedRecord.date).toISOString().split('T')[0] : todayStr,
                Validators.required
            ],
            checkInTime: [this.selectedRecord.checkInTime ? new Date(this.selectedRecord.checkInTime).toTimeString().slice(0, 5) : nowTimeStr],
            checkOutTime: [this.selectedRecord.checkOutTime ? new Date(this.selectedRecord.checkOutTime).toTimeString().slice(0, 5) : ''],
            status: [this.selectedRecord.status ?? 0, Validators.required],
            notes: [this.selectedRecord.notes || ''],
        });
    }

    save() {
        if (this.form.invalid) return;

        const formVal = this.form.value;
        const dateStr = formVal.date;

        const dto: CreateUpdateDailyAttendanceDto = {
            employeeId: formVal.employeeId,
            date: dateStr,
            checkInTime: formVal.checkInTime ? `${dateStr}T${formVal.checkInTime}:00` : null,
            checkOutTime: formVal.checkOutTime ? `${dateStr}T${formVal.checkOutTime}:00` : null,
            status: formVal.status,
            notes: formVal.notes,
        };

        if (this.isEditing && this.selectedRecord.id) {
            this.hrService.updateDailyAttendance(this.selectedRecord.id, dto).subscribe(() => {
                this.isModalOpen = false;
                this.list.get();
            });
        } else {
            this.hrService.createDailyAttendance(dto).subscribe(() => {
                this.isModalOpen = false;
                this.list.get();
            });
        }
    }

    delete(id: string) {
        this.confirmation.warn('::AreYouSureToDelete', '::AreYouSure').subscribe((status) => {
            if (status === Confirmation.Status.confirm) {
                this.hrService.deleteDailyAttendance(id).subscribe(() => this.list.get());
            }
        });
    }
}
