import { Component, Input, OnInit } from '@angular/core';
import { ListService, PagedResultDto } from '@abp/ng.core';
import { PatientCareService } from '../../proxy/nursing/patient-care.service';
import { PatientRoundDto, CreatePatientRoundDto } from '../../proxy/nursing/models';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ConfirmationService } from '@abp/ng.theme.shared';

@Component({
    selector: 'app-patient-round',
    templateUrl: './patient-round.component.html',
    styleUrls: ['./patient-round.component.scss'],
    providers: [ListService],
    standalone: false
})
export class PatientRoundComponent implements OnInit {
    @Input() patientId: string;

    rounds = { items: [], totalCount: 0 } as PagedResultDto<PatientRoundDto>;
    isModalOpen = false;
    form: FormGroup;

    constructor(
        public readonly list: ListService,
        private service: PatientCareService,
        private fb: FormBuilder,
        private confirmation: ConfirmationService
    ) { }

    ngOnInit() {
        this.list.hookToQuery(query => this.service.getPatientRounds(this.patientId)).subscribe(res => {
            this.rounds = res;
        });
    }

    createRound() {
        this.buildForm();
        this.isModalOpen = true;
    }

    buildForm() {
        this.form = this.fb.group({
            patientId: [this.patientId, Validators.required],
            note: ['', [Validators.required, Validators.maxLength(2000)]],
        });
    }

    save() {
        if (this.form.invalid) {
            return;
        }

        this.service.createPatientRound(this.form.value).subscribe(() => {
            this.isModalOpen = false;
            this.form.reset();
            this.list.get();
        });
    }
}
