import { Component, Input, OnInit } from '@angular/core';
import { ListService, PagedResultDto } from '@abp/ng.core';
import { PatientCareService } from '../../proxy/nursing/patient-care.service';
import {
    PainAssessmentDto, CreatePainAssessmentDto, PainLocation,
    FallRiskAssessmentDto, CreateFallRiskAssessmentDto, RiskLevel
} from '../../proxy/nursing/models';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ConfirmationService } from '@abp/ng.theme.shared';

@Component({
    selector: 'app-assessments',
    templateUrl: './assessments.component.html',
    styleUrls: ['./assessments.component.scss'],
    providers: [ListService],
    standalone: false
})
export class AssessmentsComponent implements OnInit {
    @Input() patientId: string = '';
    activeTab = 1;

    // Pain
    painAssessments = { items: [], totalCount: 0 } as PagedResultDto<PainAssessmentDto>;
    isPainModalOpen = false;
    painForm: FormGroup;
    painLocations = Object.keys(PainLocation).filter(k => !isNaN(Number(k))).map(k => ({ key: Number(k), value: PainLocation[k] }));

    // Fall Risk
    fallRiskAssessments = { items: [], totalCount: 0 } as PagedResultDto<FallRiskAssessmentDto>;
    isFallModalOpen = false;
    fallForm: FormGroup;
    riskLevels = RiskLevel;

    constructor(
        public readonly list: ListService,
        private service: PatientCareService,
        private fb: FormBuilder,
        private confirmation: ConfirmationService
    ) { }

    ngOnInit() {
        this.getPainAssessments();
        this.getFallRiskAssessments();
    }

    // --- Pain ---
    getPainAssessments() {
        this.service.getPainAssessments(this.patientId).subscribe(res => {
            this.painAssessments = res;
        });
    }

    createPain() {
        this.buildPainForm();
        this.isPainModalOpen = true;
    }

    buildPainForm() {
        this.painForm = this.fb.group({
            patientId: [this.patientId, Validators.required],
            painScore: [0, [Validators.required, Validators.min(0), Validators.max(10)]],
            location: [null, Validators.required],
            characteristics: [''],
            intervention: [''],
            assessmentTime: [new Date().toISOString(), Validators.required]
        });
    }

    savePain() {
        if (this.painForm.invalid) return;
        this.service.createPainAssessment(this.painForm.value).subscribe(() => {
            this.isPainModalOpen = false;
            this.getPainAssessments();
        });
    }

    // --- Fall Risk ---
    getFallRiskAssessments() {
        this.service.getFallRiskAssessments(this.patientId).subscribe(res => {
            this.fallRiskAssessments = res;
        });
    }

    createFall() {
        this.buildFallForm();
        this.isFallModalOpen = true;
    }

    buildFallForm() {
        this.fallForm = this.fb.group({
            patientId: [this.patientId, Validators.required],
            historyOfFalls: [false],
            secondaryDiagnosis: [false],
            ambulatoryAid: [false],
            iVTherapy: [false],
            gaitProblem: [false],
            mentalStatusIssue: [false],
            assessmentTime: [new Date().toISOString(), Validators.required]
        });
    }

    saveFall() {
        if (this.fallForm.invalid) return;
        this.service.createFallRiskAssessment(this.fallForm.value).subscribe(() => {
            this.isFallModalOpen = false;
            this.getFallRiskAssessments();
        });
    }
}
