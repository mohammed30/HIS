import { Component, OnInit, inject, ViewChild, ElementRef } from '@angular/core';
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
import { ServiceItemService } from '../../proxy/services/service-item.service';
import { InvoiceService } from '../../proxy/billing/invoice.service';
import { AppointmentService } from '../../proxy/appointments/appointment.service';
import { ServiceCategory } from '../../proxy/services/service-category.enum';
import { ServiceType } from '../../proxy/billing/service-type.enum';
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
    private serviceItemService = inject(ServiceItemService);
    private invoiceService = inject(InvoiceService);
    private appointmentService = inject(AppointmentService);

    @ViewChild('testSearchInput') testSearchInput!: ElementRef;

    // Master Data Lists
    nationalities: NationalityDto[] = [];
    professions: ProfessionDto[] = [];
    contracts: ContractDto[] = [];
    patientCategories: PatientCategoryDto[] = [];
    referralSources: ReferralSourceDto[] = [];

    // Tab State
    activeTab: string = 'lab';
    activeSubTab: string = 'billing';

    // Clinic Booking
    clinics: any[] = [];
    doctors: any[] = [];
    services: any[] = []; // Clinic Services

    booking: any = {
        clinicId: '',
        doctorId: '',
        serviceItemId: '',
        appointmentDate: new Date().toISOString().slice(0, 16), // datetime-local format
        cardType: 'percent', // percent or amount
        paymentMethod: 'Cash',
        payAmount: 0,
        discount: 0,
        createInvoice: true
    };

    printTicketChecked: boolean = true;

    // Date Filters
    fromDate: string = new Date().toISOString().split('T')[0];
    toDate: string = new Date().toISOString().split('T')[0];

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
    availableTests: any[] = [];
    displayTests: any[] = [];
    selectedTests: any[] = [];
    testSearchText: string = '';
    ticketCount: number = 1;

    // Patient Search
    searchResults: any[] = [];

    constructor() { }

    ngOnInit() {
        this.loadLabTests();
        this.loadMasterData();
        this.loadClinicData();
    }

    loadMasterData() {
        this.nationalityService.getList({ maxResultCount: 1000 } as any).subscribe(res => this.nationalities = res.items || []);
        this.professionService.getList({ maxResultCount: 1000 } as any).subscribe(res => this.professions = res.items || []);
        this.contractService.getList({ maxResultCount: 1000 } as any).subscribe(res => this.contracts = res.items || []);
        this.patientCategoryService.getList({ maxResultCount: 1000 } as any).subscribe(res => this.patientCategories = res.items || []);
        this.referralSourceService.getList({ maxResultCount: 1000 } as any).subscribe(res => this.referralSources = res.items || []);
    }

    loadLabTests() {
        this.serviceItemService.getList({ maxResultCount: 1000 } as any).subscribe(res => {
            // Filter only Lab Tests
            this.availableTests = (res.items || []).filter(x => x.category === ServiceCategory.LabTest);
            this.displayTests = [...this.availableTests];
        });
    }

    filterTests() {
        if (!this.testSearchText) {
            this.displayTests = [...this.availableTests];
            return;
        }
        const lower = this.testSearchText.toLowerCase();
        this.displayTests = this.availableTests.filter(t =>
            (t.name && t.name.toLowerCase().includes(lower)) ||
            (t.code && t.code.toLowerCase().includes(lower))
        );
    }

    newPatient() {
        this.patientInfo = this.getEmptyPatient();
        this.resetAge();
        this.searchResults = [];
    }

    resetAge() {
        this.ageYears = 0;
        this.ageMonths = 0;
        this.ageDays = 0;
    }

    calculateAge() {
        if (!this.patientInfo.dateOfBirth) {
            this.resetAge();
            return;
        }

        const birthDate = new Date(this.patientInfo.dateOfBirth);
        const today = new Date();

        let years = today.getFullYear() - birthDate.getFullYear();
        let months = today.getMonth() - birthDate.getMonth();
        let days = today.getDate() - birthDate.getDate();

        if (days < 0) {
            months--;
            const prevMonth = new Date(today.getFullYear(), today.getMonth(), 0);
            days += prevMonth.getDate();
        }

        if (months < 0) {
            years--;
            months += 12;
        }

        this.ageYears = years < 0 ? 0 : years;
        this.ageMonths = months < 0 ? 0 : months;
        this.ageDays = days < 0 ? 0 : days;
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
                this.calculateAge();
                this.toaster.success('تم حفظ بيانات المريض بنجاح', 'نجاح');
            },
            error: (err) => {
                console.error(err);
                this.toaster.error('حدث خطأ أثناء حفظ البيانات', 'خطأ');
            }
        });
    }

    searchPatient() {
        const searchText = this.patientInfo.fullNameAr;
        if (!searchText) {
            this.toaster.warn('الرجاء إدخال اسم للبحث', 'تنبيه');
            return;
        }

        this.patientService.search(searchText).subscribe({
            next: (res) => {
                if (res.length === 0) {
                    this.toaster.info('لا توجد نتائج مطابقة', 'بحث');
                    this.searchResults = [];
                } else if (res.length === 1) {
                    this.selectPatient(res[0].id);
                } else {
                    this.searchResults = res;
                    this.toaster.info(`تم العثور على ${res.length} نتائج`, 'بحث');
                }
            },
            error: (err) => {
                console.error(err);
                this.toaster.error('حدث خطأ أثناء البحث', 'خطأ');
            }
        });
    }

    selectPatient(id: string) {
        this.patientService.get(id).subscribe({
            next: (res) => {
                this.patientInfo = res;
                this.calculateAge();
                this.searchResults = [];
                this.toaster.success('تم تحميل بيانات المريض', 'نجاح');
            },
            error: (err) => {
                console.error(err);
                this.toaster.error('حدث خطأ أثناء تحميل بيانات المريض', 'خطأ');
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

    focusSearch() {
        this.testSearchInput?.nativeElement?.focus();
    }

    closeSearch() {
        setTimeout(() => {
            this.searchResults = [];
        }, 200);
    }

    saveInvoice() {
        if (!this.patientInfo.id) {
            this.toaster.error('يجب حفظ بيانات المريض أولاً', 'خطأ');
            return;
        }

        if (this.selectedTests.length === 0) {
            this.toaster.warn('يجب اختيار فحص واحد على الأقل', 'تنبيه');
            return;
        }

        const invoice = {
            patientId: this.patientInfo.id,
            dueDate: new Date().toISOString(),
            notes: 'Lab Request',
            items: this.selectedTests.map(test => ({
                serviceType: ServiceType.Laboratory,
                serviceCode: test.code,
                description: test.name,
                quantity: 1,
                unitPrice: test.price,
                discountPercentage: 0,
                isCoveredByInsurance: false, // Default
                notes: ''
            }))
        };

        this.invoiceService.create(invoice).subscribe({
            next: (res) => {
                this.toaster.success('تم حفظ الفاتورة بنجاح', 'نجاح');
                this.selectedTests = [];
                // Could navigate to billing or show print dialog
            },
            error: (err) => {
                console.error(err);
                this.toaster.error('حدث خطأ أثناء حفظ الفاتورة', 'خطأ');
            }
        });
    }

    // --- Clinic Booking Methods ---

    loadClinicData() {
        this.appointmentService.getClinicLookup().subscribe(res => {
            this.clinics = (res as any[]) || [];
        });

        // Load Services (Clinic Services)
        this.serviceItemService.getList({ maxResultCount: 1000 } as any).subscribe(res => {
            this.services = (res.items || []).filter(x => x.category === ServiceCategory.Consultation || x.category === ServiceCategory.Procedure);
        });
    }

    onClinicChange() {
        this.booking.doctorId = '';
        if (this.booking.clinicId) {
            this.appointmentService.getDoctorLookup(this.booking.clinicId).subscribe(res => {
                this.doctors = (res as any[]) || [];
            });
        } else {
            this.doctors = [];
        }
    }

    bookAppointment() {
        if (!this.patientInfo.id) {
            this.toaster.error('يجب اختيار مريض أولاً', 'خطأ');
            return;
        }
        if (!this.booking.clinicId || !this.booking.doctorId || !this.booking.appointmentDate) {
            this.toaster.warn('يرجى تعبئة جميع الحقول المطلوبة (العيادة، الطبيب، التاريخ)', 'تنبيه');
            return;
        }

        const input = {
            patientId: this.patientInfo.id,
            clinicId: this.booking.clinicId,
            doctorId: this.booking.doctorId,
            serviceItemId: this.booking.serviceItemId || null,
            appointmentDate: this.booking.appointmentDate,
            createInvoice: true, // Always create invoice for now as per UI "Book/Bond" (Hajz/Sanad)
            paymentMethod: this.booking.paymentMethod,
            paidAmount: this.booking.payAmount,
            discount: this.booking.discount
        };

        this.appointmentService.bookClinicAppointment(input as any).subscribe({
            next: (res) => {
                this.toaster.success('تم حجز الموعد بنجاح', 'نجاح');
                if (this.printTicketChecked) {
                    this.printTicket(res.id);
                }
                // Reset booking form or navigate
            },
            error: (err) => {
                console.error(err);
                this.toaster.error('حدث خطأ أثناء حجز الموعد', 'خطأ');
            }
        });
    }
    printTicket(appointmentId: string) {
        this.appointmentService.getTicketPdf(appointmentId).subscribe({
            next: (blob: Blob) => {
                const url = window.URL.createObjectURL(blob);
                const iframe = document.createElement('iframe');
                iframe.style.display = 'none';
                iframe.src = url;
                document.body.appendChild(iframe);
                iframe.contentWindow?.print();

                // Cleanup
                setTimeout(() => {
                    document.body.removeChild(iframe);
                    window.URL.revokeObjectURL(url);
                }, 10000);
            },
            error: (err) => {
                console.error(err);
                this.toaster.error('فشل طباعة التذكرة', 'خطأ');
            }
        });
    }
}
