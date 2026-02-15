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

import { DoctorService } from '../../proxy/settings/doctor.service';
import { PatientService } from '../../proxy/patients/patient.service';
import { ServiceItemService } from '../../proxy/services/service-item.service';
import { InvoiceService } from '../../proxy/billing/invoice.service';
import { AppointmentService } from '../../proxy/appointments/appointment.service';
import { ServiceCategory } from '../../proxy/services/service-category.enum';
import { ServiceType } from '../../proxy/billing/service-type.enum';
import { ToasterService } from '@abp/ng.theme.shared';

// New Imports
import { AdmissionService } from '../../proxy/inpatient/admission.service';
import { RoomService } from '../../proxy/rooms/room.service';
import { SurgicalOperationService } from '../../proxy/operations/surgical-operation.service';
import { RoomType } from '../../proxy/rooms/room-type.enum';
import { AdmissionStatus } from '../../proxy/inpatient/admission-status.enum';
import { OperationStatus } from '../../proxy/operations/operation-status.enum';

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
    private admissionService = inject(AdmissionService);
    private roomService = inject(RoomService);
    private operationService = inject(SurgicalOperationService);
    private doctorService = inject(DoctorService);

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

    // Admission Model
    admission: any = {
        companionName: '',
        companionPhone: '',
        companionAddress: '',
        purpose: '',
        pharmacyPercentage: 0,
        insuranceCeiling: 0,
        admissionDate: new Date().toISOString().slice(0, 16),
        roomType: null,
        roomId: null,
        numberOfDays: 0,
        notes: '',
        paidAmount: 0,
        isServicesStopped: false
    };

    // Operation Model
    operation: any = {
        operationTypeId: '',
        operationDate: new Date().toISOString().slice(0, 16),
        doctorId: '',
        totalAmount: 0,
        companyShare: 0,
        patientShare: 0,
        details: '',
        notes: ''
    };

    roomTypes = [
        { id: RoomType.Standard, name: 'Standard / عادي' },
        { id: RoomType.Private, name: 'Private / خاص' },
        { id: RoomType.ICU, name: 'ICU / عناية مركزة' },
        { id: RoomType.Suite, name: 'Suite / جناح' },
        { id: RoomType.Isolation, name: 'Isolation / عزل' }
    ];

    availableRooms: any[] = [];
    operationTypes: any[] = [];
    inpatientList: any[] = [];
    operationsList: any[] = [];

    // Medical Services
    medicalServiceCategories = [
        { id: ServiceCategory.Consultation, name: 'Consultation / كشف عيادة' },
        { id: ServiceCategory.Procedure, name: 'Procedure / إجراء طبي' },
        { id: ServiceCategory.Radiology, name: 'Radiology / أشعة' },
        { id: ServiceCategory.Other, name: 'Other / خدمات أخرى' }
    ];
    selectedCategory: ServiceCategory | null = null;
    filteredServiceItems: any[] = [];
    selectedServiceId: string = '';
    requestedByDoctorId: string = '';
    selectedMedicalServices: any[] = [];
    medicalServicesTotal = 0;
    medicalServicesPatientShare = 0;
    medicalServicesInsuranceShare = 0;

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
        this.loadOperationTypes();
        this.loadAllDoctors();
    }

    loadMasterData() {
        this.nationalityService.getList({ maxResultCount: 1000 } as any).subscribe(res => this.nationalities = res.items || []);
        this.professionService.getList({ maxResultCount: 1000 } as any).subscribe(res => this.professions = res.items || []);
        this.contractService.getList({ maxResultCount: 1000 } as any).subscribe(res => this.contracts = res.items || []);
        this.patientCategoryService.getList({ maxResultCount: 1000 } as any).subscribe(res => this.patientCategories = res.items || []);
        this.referralSourceService.getList({ maxResultCount: 1000 } as any).subscribe(res => this.referralSources = res.items || []);
    }

    loadAllDoctors() {
        this.doctorService.getLookup().subscribe(res => {
            this.doctors = (res as any[]) || [];
        });
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
                this.loadInpatientList();
                this.loadOperationsList();
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

    // --- Inpatient Methods ---

    loadAvailableRooms() {
        if (this.admission.roomType === null) {
            this.availableRooms = [];
            return;
        }
        this.roomService.getAvailableRooms(this.admission.roomType).subscribe(res => {
            this.availableRooms = res || [];
        });
    }

    saveAdmission() {
        if (!this.patientInfo.id) {
            this.toaster.error('يجب اختيار مريض أولاً', 'خطأ');
            return;
        }
        if (!this.admission.roomId) {
            this.toaster.warn('يرجى اختيار غرفة', 'تنبيه');
            return;
        }

        const input = {
            patientId: this.patientInfo.id,
            roomId: this.admission.roomId,
            insuranceCeiling: this.admission.insuranceCeiling,
            companionName: this.admission.companionName,
            companionPhone: this.admission.companionPhone,
            companionAddress: this.admission.companionAddress,
            purpose: this.admission.purpose,
            pharmacyPercentage: this.admission.pharmacyPercentage,
            isServicesStopped: this.admission.isServicesStopped,
            notes: this.admission.notes
        };

        this.admissionService.create(input).subscribe({
            next: () => {
                this.toaster.success('تم تسجيل التنويم بنجاح', 'نجاح');
                this.loadInpatientList();
            },
            error: (err) => {
                console.error(err);
                this.toaster.error('حدث خطأ أثناء حفظ بيانات التنويم', 'خطأ');
            }
        });
    }

    loadInpatientList() {
        if (!this.patientInfo.id) return;
        this.admissionService.getList({ patientId: this.patientInfo.id } as any).subscribe(res => {
            this.inpatientList = res.items || [];
        });
    }

    // --- Operations Methods ---

    loadOperationTypes() {
        this.serviceItemService.getList({ maxResultCount: 1000 } as any).subscribe(res => {
            this.operationTypes = (res.items || []).filter(x => x.category === ServiceCategory.Surgery);
        });
    }

    saveOperation() {
        if (!this.patientInfo.id) {
            this.toaster.error('يجب اختيار مريض أولاً', 'خطأ');
            return;
        }
        if (!this.operation.operationTypeId || !this.operation.doctorId) {
            this.toaster.warn('يرجى اختيار العملية والطبيب', 'تنبيه');
            return;
        }

        const input = {
            patientId: this.patientInfo.id,
            doctorId: this.operation.doctorId,
            operationTypeId: this.operation.operationTypeId,
            operationDate: this.operation.operationDate,
            totalAmount: this.operation.totalAmount,
            companyShare: this.operation.companyShare,
            patientShare: this.operation.patientShare,
            details: this.operation.details,
            notes: this.operation.notes,
            status: OperationStatus.Scheduled
        };

        this.operationService.create(input).subscribe({
            next: (res) => { // Use 'res' to get the created operation
                this.toaster.success('تم حفظ بيانات العملية بنجاح', 'نجاح');
                this.loadOperationsList();
                if (this.printTicketChecked) {
                    this.printOperationTicket(res.id);
                }
                // Optional: clear form
                this.operation = {
                    operationTypeId: '',
                    operationDate: new Date().toISOString().slice(0, 16),
                    doctorId: '',
                    totalAmount: 0,
                    companyShare: 0,
                    patientShare: 0,
                    details: '',
                    notes: ''
                };
            },
            error: (err) => {
                console.error(err);
                this.toaster.error('حدث خطأ أثناء حفظ بيانات العملية', 'خطأ');
            }
        });
    }

    printOperationTicket(operationId: string) {
        this.operationService.getOperationTicketPdf(operationId).subscribe({
            next: (blob: Blob) => {
                const url = window.URL.createObjectURL(blob);
                const iframe = document.createElement('iframe');
                iframe.style.display = 'none';
                iframe.src = url;
                document.body.appendChild(iframe);
                iframe.contentWindow?.print();

                setTimeout(() => {
                    document.body.removeChild(iframe);
                    window.URL.revokeObjectURL(url);
                }, 10000);
            },
            error: (err) => {
                console.error(err);
                this.toaster.error('فشل طباعة تذكرة العملية', 'خطأ');
            }
        });
    }

    loadOperationsList() {
        if (!this.patientInfo.id) return;
        this.operationService.getList({ patientId: this.patientInfo.id } as any).subscribe(res => {
            this.operationsList = res.items || [];
        });
    }

    // --- Medical Services Methods ---

    onCategoryChange() {
        if (this.selectedCategory === null) {
            this.filteredServiceItems = [];
            return;
        }
        this.serviceItemService.getList({
            maxResultCount: 1000,
            category: this.selectedCategory
        } as any).subscribe(res => {
            this.filteredServiceItems = res.items || [];
        });
    }

    addMedicalServiceToList() {
        const item = this.filteredServiceItems.find(x => x.id === this.selectedServiceId);
        if (!item) return;

        const isDuplicate = this.selectedMedicalServices.some(x => x.id === item.id);
        if (isDuplicate) {
            this.toaster.warn('هذه الخدمة مضافة بالفعل', 'تنبيه');
            return;
        }

        const price = item.price || 0;
        this.selectedMedicalServices.push({
            id: item.id,
            name: item.name,
            code: item.code,
            price: price,
            patientShare: price,
            insuranceShare: 0,
            serviceType: this.mapCategoryToType(this.selectedCategory),
            status: 'Pending'
        });

        this.updateMedicalServicesTotals();
    }

    mapCategoryToType(category: ServiceCategory | null): ServiceType {
        if (category === null) return ServiceType.Other;
        switch (category) {
            case ServiceCategory.Consultation: return ServiceType.Consultation;
            case ServiceCategory.Procedure: return ServiceType.Procedure;
            case ServiceCategory.Radiology: return ServiceType.Radiology;
            case ServiceCategory.LabTest: return ServiceType.Laboratory;
            default: return ServiceType.Other;
        }
    }

    removeMedicalService(index: number) {
        this.selectedMedicalServices.splice(index, 1);
        this.updateMedicalServicesTotals();
    }

    updateMedicalServicesTotals() {
        this.medicalServicesTotal = this.selectedMedicalServices.reduce((acc, curr) => acc + curr.price, 0);
        this.medicalServicesPatientShare = this.selectedMedicalServices.reduce((acc, curr) => acc + curr.patientShare, 0);
        this.medicalServicesInsuranceShare = this.selectedMedicalServices.reduce((acc, curr) => acc + curr.insuranceShare, 0);
    }

    saveMedicalServicesInvoice() {
        if (!this.patientInfo.id) {
            this.toaster.error('يجب اختيار مريض أولاً', 'خطأ');
            return;
        }
        if (this.selectedMedicalServices.length === 0) {
            this.toaster.warn('يرجى إضافة خدمات أولاً', 'تنبيه');
            return;
        }

        const invoiceInput: any = {
            patientId: this.patientInfo.id,
            dueDate: new Date().toISOString(),
            items: this.selectedMedicalServices.map(x => ({
                serviceItemId: x.id,
                serviceType: x.serviceType,
                description: x.name,
                quantity: 1,
                unitPrice: x.price,
                discountPercentage: 0
            }))
        };

        this.invoiceService.create(invoiceInput).subscribe({
            next: (invoice) => {
                this.toaster.success('تم حفظ فاتورة الخدمات الطبية بنجاح', 'نجاح');
                this.selectedMedicalServices = [];
                this.updateMedicalServicesTotals();
                // Optionally print ticket or redirect to payment
            },
            error: (err) => {
                console.error(err);
                this.toaster.error('حدث خطأ أثناء حفظ الفاتورة', 'خطأ');
            }
        });
    }
}
