import { Component, Input, OnInit } from '@angular/core';
import { ListService, PagedResultDto } from '@abp/ng.core';
import { FluidBalanceService } from '../../proxy/nursing/fluid-balance.service';
import {
    FluidBalanceDto, CreateFluidBalanceDto, FluidBalanceSummaryDto
} from '../../proxy/nursing/models';
import { FluidType, FluidMetric } from '../../proxy/nursing';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ConfirmationService } from '@abp/ng.theme.shared';

@Component({
    selector: 'app-fluid-balance',
    templateUrl: './fluid-balance.component.html',
    styleUrls: ['./fluid-balance.component.scss'],
    providers: [ListService],
    standalone: false
})
export class FluidBalanceComponent implements OnInit {
    @Input() patientId: string = '';

    records = { items: [], totalCount: 0 } as PagedResultDto<FluidBalanceDto>;
    summary: FluidBalanceSummaryDto = { totalInput: 0, totalOutput: 0, balance: 0 };

    isModalOpen = false;
    form: FormGroup;

    fluidTypes = FluidType;
    fluidMetrics = FluidMetric;

    // Helpers for dropdowns
    typeOptions = [
        { key: FluidType.Input, value: 'Input' },
        { key: FluidType.Output, value: 'Output' }
    ];

    metricOptions = [];

    constructor(
        public readonly list: ListService,
        private service: FluidBalanceService,
        private fb: FormBuilder,
        private confirmation: ConfirmationService
    ) { }

    ngOnInit() {
        this.refresh();
    }

    refresh() {
        this.list.hookToQuery(query => this.service.getList(this.patientId)).subscribe(res => {
            this.records = res;
            this.loadSummary();
        });
    }

    loadSummary() {
        // Summary for TODAY
        const today = new Date().toISOString();
        this.service.getSummary(this.patientId, today).subscribe(res => {
            this.summary = res;
        });
    }

    createRecord() {
        this.buildForm();
        this.updateMetricOptions(FluidType.Input); // Default
        this.isModalOpen = true;
    }

    buildForm() {
        this.form = this.fb.group({
            patientId: [this.patientId, Validators.required],
            type: [FluidType.Input, Validators.required],
            metric: [null, Validators.required],
            amount: [0, [Validators.required, Validators.min(1)]],
            entryTime: [new Date().toISOString(), Validators.required],
            notes: ['']
        });

        this.form.get('type').valueChanges.subscribe(val => {
            this.updateMetricOptions(val);
            this.form.get('metric').setValue(null);
        });
    }

    updateMetricOptions(type: FluidType) {
        if (type === FluidType.Input) {
            this.metricOptions = [
                { key: FluidMetric.Oral, value: 'Oral' },
                { key: FluidMetric.IV, value: 'IV' },
                { key: FluidMetric.TubeFeeding, value: 'Tube Feeding' }
            ];
        } else {
            this.metricOptions = [
                { key: FluidMetric.Urine, value: 'Urine' },
                { key: FluidMetric.Stool, value: 'Stool' },
                { key: FluidMetric.Vomit, value: 'Vomit' },
                { key: FluidMetric.Drain, value: 'Drain' },
                { key: FluidMetric.Sweat, value: 'Sweat' }
            ];
        }
    }

    save() {
        if (this.form.invalid) return;

        this.service.create(this.form.value).subscribe(() => {
            this.isModalOpen = false;
            this.list.get(); // Triggers refresh and loadSummary
        });
    }
}
