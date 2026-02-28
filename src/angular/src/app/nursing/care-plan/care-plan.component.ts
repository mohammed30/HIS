import { Component, Input, OnInit } from '@angular/core';
import { ListService, PagedResultDto } from '@abp/ng.core';
import { NursingService } from '../../proxy/nursing/nursing.service';
import { CarePlanDto, CreateCarePlanDto } from '../../proxy/nursing/models';
import { CarePlanStatus } from '../../proxy/nursing';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Confirmation, ConfirmationService } from '@abp/ng.theme.shared';

@Component({
    selector: 'app-care-plan',
    templateUrl: './care-plan.component.html',
    styleUrls: ['./care-plan.component.scss'],
    providers: [ListService],
    standalone: false
})
export class CarePlanComponent implements OnInit {
    @Input() patientId: string = '';

    carePlans = { items: [], totalCount: 0 } as PagedResultDto<CarePlanDto>; // Service returns array, need to handle
    isModalOpen = false;
    form: FormGroup;
    statuses = CarePlanStatus;
    statusOptions = [
        { key: CarePlanStatus.Active, value: 'Active' },
        { key: CarePlanStatus.Resolved, value: 'Resolved' },
        { key: CarePlanStatus.Discontinued, value: 'Discontinued' }
    ];

    selectedPlan: CarePlanDto;

    constructor(
        public readonly list: ListService,
        private service: NursingService,
        private fb: FormBuilder,
        private confirmation: ConfirmationService
    ) { }

    ngOnInit() {
        this.getCarePlans();
    }

    getCarePlans() {
        // service returns CarePlanDto[] not PagedResultDto
        this.service.getCarePlans(this.patientId).subscribe(res => {
            this.carePlans = { items: res || [], totalCount: (res || []).length };
        });
    }

    createPlan() {
        this.selectedPlan = null;
        this.buildForm();
        this.isModalOpen = true;
    }

    editPlan(plan: CarePlanDto) {
        this.selectedPlan = plan;
        this.buildForm();
        this.isModalOpen = true;
    }

    buildForm() {
        this.form = this.fb.group({
            patientId: [this.patientId, Validators.required],
            diagnosis: [this.selectedPlan?.diagnosis || '', Validators.required],
            goal: [this.selectedPlan?.goal || '', Validators.required],
            interventions: [this.selectedPlan?.interventions || ''],
            status: [this.selectedPlan?.status ?? CarePlanStatus.Active, Validators.required]
        });
    }

    save() {
        if (this.form.invalid) return;

        const request = this.selectedPlan
            ? this.service.updateCarePlan(this.selectedPlan.id, this.form.value)
            : this.service.createCarePlan(this.form.value);

        request.subscribe(() => {
            this.isModalOpen = false;
            this.getCarePlans();
        });
    }

    delete(id: string) {
        this.confirmation.warn('::AreYouSureToDelete', '::AreYouSure').subscribe((status) => {
            if (status === Confirmation.Status.confirm) {
                this.service.deleteCarePlan(id).subscribe(() => this.getCarePlans());
            }
        });
    }
}
