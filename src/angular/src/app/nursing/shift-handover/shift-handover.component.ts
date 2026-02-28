import { Component, OnInit } from '@angular/core';
import { ListService, PagedResultDto, RestService } from '@abp/ng.core';
import { PatientCareService } from '../../proxy/nursing/patient-care.service';
import {
    ShiftHandoverDto, CreateShiftHandoverDto
} from '../../proxy/nursing/models';
import { ShiftType } from '../../proxy/nursing';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ConfirmationService } from '@abp/ng.theme.shared';

@Component({
    selector: 'app-shift-handover',
    templateUrl: './shift-handover.component.html',
    styleUrls: ['./shift-handover.component.scss'],
    providers: [ListService],
    standalone: false
})
export class ShiftHandoverComponent implements OnInit {
    records = { items: [], totalCount: 0 } as PagedResultDto<ShiftHandoverDto>;
    isModalOpen = false;
    form: FormGroup;

    users: any[] = [];
    shiftTypes = ShiftType;
    shiftOptions = [
        { key: ShiftType.Morning, value: 'Morning' },
        { key: ShiftType.Evening, value: 'Evening' },
        { key: ShiftType.Night, value: 'Night' }
    ];

    constructor(
        public readonly list: ListService,
        private service: PatientCareService,
        private restService: RestService,
        private fb: FormBuilder,
        private confirmation: ConfirmationService
    ) { }

    ngOnInit() {
        this.list.hookToQuery(query => this.service.getShiftHandovers(query)).subscribe(res => {
            this.records = res;
        });
        this.getUsers();
    }

    getUsers() {
        // Direct call to Identity API if proxy missing
        this.restService.request<any, PagedResultDto<any>>({
            method: 'GET',
            url: '/api/identity/users',
            params: { maxResultCount: 100 } // Should be enough for demo
        }).subscribe(res => {
            this.users = res.items;
        });
    }

    createHandover() {
        this.buildForm();
        this.isModalOpen = true;
    }

    buildForm() {
        this.form = this.fb.group({
            shift: [ShiftType.Morning, Validators.required],
            notes: ['', Validators.required],
            incomingNurseId: [null, Validators.required]
        });
    }

    save() {
        if (this.form.invalid) return;

        this.service.createShiftHandover(this.form.value).subscribe(() => {
            this.isModalOpen = false;
            this.list.get();
        });
    }
}
