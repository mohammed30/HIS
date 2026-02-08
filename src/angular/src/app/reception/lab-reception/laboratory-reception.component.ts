import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CoreModule, LocalizationService } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { HttpClient } from '@angular/common/http';
import { NationalityService } from '../../proxy/general/nationality.service';
import { ProfessionService } from '../../proxy/general/profession.service';
import { ContractService } from '../../proxy/general/contract.service';
import { PatientCategoryService } from '../../proxy/general/patient-category.service';
import { ReferralSourceService } from '../../proxy/general/referral-source.service';
import { NationalityDto, ProfessionDto, ContractDto, PatientCategoryDto, ReferralSourceDto } from '../../proxy/general/models';

import { PatientService } from '../../proxy/patients/patient.service';
import { ToasterService } from '@abp/ng.theme.shared';

@Component({
    selector: 'app-laboratory-reception',
    standalone: true,
    imports: [CommonModule, CoreModule, ThemeSharedModule, FormsModule, ReactiveFormsModule],
    templateUrl: './laboratory-reception.component.html',
    styleUrls: ['./laboratory-reception.component.scss']
})
export class LaboratoryReceptionComponent implements OnInit {
    private http = inject(HttpClient);
    private localization = inject(LocalizationService);
    private toaster = inject(ToasterService);
    private nationalityService = inject(NationalityService);
    private professionService = inject(ProfessionService);
    private contractService = inject(ContractService);
    private patientCategoryService = inject(PatientCategoryService);
    private referralSourceService = inject(ReferralSourceService);
    private patientService = inject(PatientService);

    // Master Data Lists
    nationalities: NationalityDto[] = [];
    professions: ProfessionDto[] = [];
    contracts: ContractDto[] = [];
    patientCategories: PatientCategoryDto[] = [];
    referralSources: ReferralSourceDto[] = [];

    // Tab State
    activeTab: string = 'lab';

    // Patient Info
    patientInfo: any = this.getEmptyPatient();

    getEmptyPatient() {
        return {
            id: null,
            mrn: '',
            fullNameAr: '',
            fullNameEn: '',
            gender: 0,
            nationalityId: null,
            professionId: null,
            dateOfBirth: '',
            identityNumber: '',
            identityType: 0,
            idExpiryDate: '',
            idIssuePlace: '',
            mobileNumber: '',
            phoneNumber: '',
            address: '',
            email: '',
            passportNumber: '',
            passportIssueDate: '',
            passportExpiryDate: '',
            passportIssuePlace: '',
            visaNumber: '',
            visaIssueDate: '',
            visaExpiryDate: '',
            visaIssuePlace: '',
            sponsorName: '',
            sponsorId: '',
            emergencyContactName: '',
            emergencyContactPhone: '',
            emergencyContactRelation: '',
            taxFile: '',
            contractId: null,
            patientCategoryId: null,
            cardNumber: '',
            referralSourceId: null,
            isSocialSecurity: false,
            isActive: true
        };
    }

    ageYears: number = 0;
    ageMonths: number = 0;
    ageDays: number = 0;

    // Laboratory Tests
    availableTests: any[] = [
        { id: '1', name: 'CBC', price: 100 },
        { id: '2', name: 'STOOL', price: 50 },
        { id: '3', name: 'TWBCs+Diff', price: 80 },
        { id: '4', name: 'Urine General', price: 40 },
        { id: '5', name: 'Virology', price: 200 },
        { id: '6', name: 'LFT', price: 150 },
        { id: '7', name: 'RFT', price: 150 },
        { id: '8', name: 'Lipid Profile', price: 180 },
        { id: '9', name: 'Blood Glucose', price: 30 }
    ];

    selectedTests: any[] = [];
    ticketCount: number = 1;

    constructor() { }

    ngOnInit() {
        this.loadLabTests();
        this.loadMasterData();
    }

    loadMasterData() {
        this.nationalityService.getList({ maxResultCount: 1000 } as any).subscribe(res => this.nationalities = res.items || []);
        this.professionService.getList({ maxResultCount: 1000 } as any).subscribe(res => this.professions = res.items || []);
        this.contractService.getList({ maxResultCount: 1000 } as any).subscribe(res => this.contracts = res.items || []);
        this.patientCategoryService.getList({ maxResultCount: 1000 } as any).subscribe(res => this.patientCategories = res.items || []);
        this.referralSourceService.getList({ maxResultCount: 1000 } as any).subscribe(res => this.referralSources = res.items || []);
    }

    loadLabTests() {
        // In a real scenario, we would fetch from the backend
        // this.http.get<any>(`${this.apiUrl}/api/app/lab/tests`).subscribe(res => {
        //   this.availableTests = res.items;
        // });
    }

    newPatient() {
        this.patientInfo = this.getEmptyPatient();
    }

    savePatient() {
        if (!this.patientInfo.fullNameAr || !this.patientInfo.mobileNumber) {
            this.toaster.warn('يرجى إكمال البيانات المطلوبة (الاسم والموبايل)', 'بيانات ناقصة');
            return;
        }

        const request = this.patientInfo.id
            ? this.patientService.update(this.patientInfo.id, this.patientInfo)
            : this.patientService.create(this.patientInfo);

        request.subscribe({
            next: (res) => {
                this.patientInfo = { ...this.patientInfo, ...res };
                this.toaster.success('تم حفظ بيانات المريض بنجاح', 'نجاح');
            },
            error: (err) => {
                console.error(err);
                this.toaster.error('حدث خطأ أثناء حفظ البيانات', 'خطأ');
            }
        });
    }

    addTestToOrder(test: any) {
        if (!this.selectedTests.find(t => t.id === test.id)) {
            this.selectedTests.push({
                ...test,
                contractPrice: test.price // Default to list price, would be calculated based on contract
            });
        }
    }

    removeTest(index: number) {
        this.selectedTests.splice(index, 1);
    }

    // Helper to calculate age would go here
}
