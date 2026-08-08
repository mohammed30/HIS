import { Component, OnInit, inject, ViewChild, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CoreModule, LocalizationService, ConfigStateService } from '@abp/ng.core';
import { ThemeSharedModule, ConfirmationService, Confirmation } from '@abp/ng.theme.shared';
import { HttpClient } from '@angular/common/http';
import { forkJoin, of } from 'rxjs';
import { map, switchMap, catchError } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { NationalityService } from '../../proxy/general/nationality.service';
import { ProfessionService } from '../../proxy/general/profession.service';
import { ContractService } from '../../proxy/general/contract.service';
import { PaymentMethodService } from '../../proxy/general/payment-method.service';
import { PaymentService } from '../../proxy/billing/payment.service';
import { ReferralSourceService } from '../../proxy/general/referral-source.service';
import { NationalityDto, ProfessionDto, ContractDto, ReferralSourceDto } from '../../proxy/general/models';
import { PaymentMethodDto } from '../../proxy/general/dtos/models';

import { DoctorService } from '../../proxy/settings/doctor.service';
import { ClinicService } from '../../proxy/settings/clinic.service';
import { PatientService } from '../../proxy/patients/patient.service';
import { ServiceItemService } from '../../proxy/services/service-item.service';
import { InvoiceService } from '../../proxy/billing/invoice.service';
import { AppointmentService } from '../../proxy/appointments/appointment.service';
import { ServiceCategory } from '../../proxy/services/service-category.enum';
import { ServiceType } from '../../proxy/billing/service-type.enum';
import { ToasterService } from '@abp/ng.theme.shared';

// New Imports
import { InpatientDepositService } from '../../proxy/billing/inpatient-deposit.service';
import { AdmissionService } from '../../proxy/inpatient/admission.service';
import { RoomService } from '../../proxy/rooms/room.service';
import { SurgicalOperationService } from '../../proxy/operations/surgical-operation.service';
import { RoomType } from '../../proxy/rooms/room-type.enum';
import { AdmissionStatus } from '../../proxy/inpatient/admission-status.enum';
import { OperationStatus } from '../../proxy/operations/operation-status.enum';
import { BedDto } from '../../proxy/rooms/models';
import { BedStatus } from '../../proxy/rooms/bed-status.enum';
import { LabService } from '../../proxy/laboratory/lab.service';
import { InsuranceCompanyService } from '../../proxy/insurance/insurance-company.service';
import { InsurancePlanService } from '../../proxy/insurance/insurance-plan.service';
import { PatientBalanceGuardService } from '../../shared/patient-balance-guard.service';

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
    private paymentMethodService = inject(PaymentMethodService);
    private paymentService = inject(PaymentService);
    private inpatientDepositService = inject(InpatientDepositService);
    private referralSourceService = inject(ReferralSourceService);
    private patientService = inject(PatientService);
    private serviceItemService = inject(ServiceItemService);
    private invoiceService = inject(InvoiceService);
    private appointmentService = inject(AppointmentService);
    private admissionService = inject(AdmissionService);
    private roomService = inject(RoomService);
    private operationService = inject(SurgicalOperationService);
    private insuranceCompanyService = inject(InsuranceCompanyService);
    private insurancePlanService = inject(InsurancePlanService);
    private doctorService = inject(DoctorService);
    private clinicService = inject(ClinicService);
    private labService = inject(LabService);
    public patientBalanceGuard = inject(PatientBalanceGuardService);
    private confirmation = inject(ConfirmationService);
    private configState = inject(ConfigStateService);

    @ViewChild('testSearchInput') testSearchInput!: ElementRef;

    // Master Data Lists
    nationalities: NationalityDto[] = [];
    professions: ProfessionDto[] = [];
    contracts: ContractDto[] = [];
    paymentMethods: PaymentMethodDto[] = [];
    referralSources: ReferralSourceDto[] = [];
    insuranceCompanies: any[] = [];
    insurancePlans: any[] = [];
    
    // Tab Insurance Percentages
    labInsurancePercentage = 0;
    clinicsInsurancePercentage = 0;
    medicalServicesInsurancePercentage = 0;
    operationsInsurancePercentage = 0;
    inpatientInsurancePercentage = 0;

    // Role-Based Access
    isAdminOrAdminStaff: boolean = false;

    // Tab State
    activeTab: string = 'lab';
    activeSubTab: string = 'statement';

    // Clinic Booking
    clinics: any[] = [];
    doctors: any[] = [];
    services: any[] = []; // Clinic Services
    departments: any[] = [];
    selectedDepartmentId: string = '';

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
    printBarcodeChecked: boolean = false;
    printWorkOrderChecked: boolean = false;

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

        isServicesStopped: false,
        bedId: null
    };

    isSavingAdmission: boolean = false;

    // Billing Model
    billingDetails: any = {
        cash: 0,
        card: 0,
        transfer: 0,
        clientBalance: 0,
        paidAmount: 0,
        remainingAmount: 0,
        discount: 0,
        total: 0,
        grandTotal: 0,
        tax: 0,
        applyTax: false // Default unchecked as per user request
    };

    availableBedsList: BedDto[] = [];

    // Patient Statement
    patientStatement: any[] = [];
    statementSummary = { totalDebit: 0, totalCredit: 0, balance: 0 };

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

    medicalServicesPayment = {
        amountPaid: 0,
        remainingAmount: 0,
        discount: 0,
        paymentMethod: 0 // Cash
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
    // Medical Services Model
    // The original `selectedCategory` and `selectedServiceId` are already defined above.
    // The instruction seems to be redefining them or moving them.
    // I will assume the instruction intends to update the types and add the new property.
    // I will keep the first definition of `selectedCategory` and `filteredServiceItems`
    // and update the types for `medicalServicesTotal`, `medicalServicesPatientShare`, `medicalServicesInsuranceShare`,
    // and add `printMedicalServicesTicket`.
    // The `selectedMedicalServices` property was removed as per the instruction.
    selectedServiceId: string = '';
    requestedByDoctorId: string = '';
    selectedMedicalServices: any[] = [];
    medicalServicesTotal: number = 0;
    medicalServicesPatientShare: number = 0;
    medicalServicesInsuranceShare: number = 0;
    printMedicalServicesTicket: boolean = true;

    getEmptyPatient() {
        return {
            id: null,
            mrn: '',
            fullNameAr: '',
            fullNameEn: '',
            gender: 0,
            nationalityId: null,
            professionId: null,
            dateOfBirth: null,
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
            insuranceCompanyId: null,
            insurancePlanId: null,
            paymentMethodId: null,
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
    searchMode: 'code' | 'name' = 'code';
    requestingDoctorId: string = '';
    ticketCount: number = 1;

    // Modal State
    showAdmissionDaysModal: boolean = false;
    showRefundModal: boolean = false;
    newAdmissionDays: number = 0;
    refundReason: string = '';
    selectedRefundItem: any = null;

    // Statement Details
    selectedStatementItem: any = null;
    showStatementDetailsModal: boolean = false;

    // Patient Search
    searchResults: any[] = [];
    selectedSearchIndex: number = -1;

    constructor() { }

    ngOnInit() {
        // Check user roles for admin access
        const currentUser = this.configState.getDeep('currentUser');
        const roles: string[] = currentUser?.roles || [];
        this.isAdminOrAdminStaff = roles.some((r: string) => {
            const normalized = r.toLowerCase().replace(/\s/g, '');
            return normalized === 'admin' ||
                   normalized === 'adminstaff' ||
                   normalized === 'administrators' ||
                   normalized === 'his.admin';
        });

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
        this.paymentMethodService.getList({ maxResultCount: 1000 } as any).subscribe(res => {
            this.paymentMethods = (res.items || []).sort((a, b) => (b.isDefault ? 1 : 0) - (a.isDefault ? 1 : 0));
            // Auto-select default logic
            const defaultMethod = this.paymentMethods.find(m => m.isDefault);
            if (defaultMethod && !this.patientInfo.paymentMethodId) {
                this.patientInfo.paymentMethodId = defaultMethod.id;
                this.onPaymentMethodChange(defaultMethod.id);
            }
        });
        this.referralSourceService.getList({ maxResultCount: 1000 } as any).subscribe(res => this.referralSources = res.items || []);
        this.insuranceCompanyService.getList({ maxResultCount: 1000 } as any).subscribe(res => this.insuranceCompanies = res.items || []);
    }

    loadAllDoctors() {
        this.doctorService.getList({ maxResultCount: 1000 }).subscribe(res => {
            this.doctors = (res.items || []).map(x => ({ 
                ...x, 
                name: x.nameAr || x.nameEn || (x as any).name 
            })).sort((a, b) => (a.name || '').localeCompare(b.name || ''));
        });
    }

    onInsuranceCompanyChange(companyId: string) {
        if (!companyId) {
            this.insurancePlans = [];
            this.patientInfo.insurancePlanId = null;
            return;
        }
        this.insurancePlanService.getList({ insuranceCompanyId: companyId, maxResultCount: 1000 } as any).subscribe(res => {
            this.insurancePlans = res.items || [];
        });
    }

    onInsurancePlanChange(planId: string) {
        const plan = this.insurancePlans.find(p => p.id === planId);
        if (plan) {
            this.labInsurancePercentage = plan.labCoveragePercentage || 0;
            this.clinicsInsurancePercentage = plan.consultationCoveragePercentage || 0;
            this.medicalServicesInsurancePercentage = plan.medicalServiceCoveragePercentage || 0;
            this.operationsInsurancePercentage = plan.operationsCoveragePercentage || 0;
            this.inpatientInsurancePercentage = plan.inpatientCoveragePercentage || 0;
        } else {
            this.labInsurancePercentage = 0;
            this.clinicsInsurancePercentage = 0;
            this.medicalServicesInsurancePercentage = 0;
            this.operationsInsurancePercentage = 0;
            this.inpatientInsurancePercentage = 0;
        }
    }

    currentDoctorSharePercent: number = 0;

    getDoctorShare(): number {
        if (!this.booking.doctorId) return 0;
        const amount = (this.booking.payAmount || 0) - (this.booking.discount || 0);
        return (amount * this.currentDoctorSharePercent) / 100;
    }

    getHospitalShare(): number {
        const amount = (this.booking.payAmount || 0) - (this.booking.discount || 0);
        return amount - this.getDoctorShare();
    }

    getClinicCompanyShare(): number {
        if (this.clinicsInsurancePercentage > 0) {
            const amount = (this.booking.payAmount || 0) - (this.booking.discount || 0);
            return Math.round(amount * (this.clinicsInsurancePercentage / 100));
        }
        return 0;
    }

    getClinicPatientShare(): number {
        const amount = (this.booking.payAmount || 0) - (this.booking.discount || 0);
        return amount - this.getClinicCompanyShare();
    }

    updateConsultationFee() {
        // If a service item is selected, it should have priority for the fee
        if (this.booking.serviceItemId) {
            const service = this.services.find(s => s.id === this.booking.serviceItemId);
            if (service) {
                this.booking.payAmount = service.price || 0;
            }
        }

        if (!this.booking.doctorId) {
            if (!this.booking.serviceItemId) {
                this.booking.payAmount = 0;
            }
            this.currentDoctorSharePercent = 0;
            return;
        }

        const doctor = this.doctors.find(d => d.id === this.booking.doctorId);
        if (!doctor) {
            this.currentDoctorSharePercent = 0;
            return;
        }

        const fillDeptAndClinic = (deptId?: string, clinicId?: string, doctorPercentage?: number) => {
            this.currentDoctorSharePercent = doctorPercentage || 0;
            
            if (deptId) {
                const matchingDept = this.departments.find(d => d.id.toLowerCase() === deptId.toLowerCase());
                if (matchingDept && this.selectedDepartmentId !== matchingDept.id) {
                    this.selectedDepartmentId = matchingDept.id;
                    // Fetch clinics for this department without clearing doctor
                    this.clinicService.getByDepartment(matchingDept.id).subscribe(res => {
                        this.clinics = (res || []).map(x => ({ ...x, name: x.nameAr || x.nameEn || (x as any).name }))
                            .sort((a, b) => a.name.localeCompare(b.name));
                        
                        if (clinicId) {
                            const matchingClinic = this.clinics.find(c => c.id.toLowerCase() === clinicId.toLowerCase());
                            if (matchingClinic) {
                                this.booking.clinicId = matchingClinic.id;
                            }
                        } else if (this.clinics.length === 1) {
                            this.booking.clinicId = this.clinics[0].id;
                        }
                    });
                } else if (matchingDept && this.selectedDepartmentId === matchingDept.id) {
                    // Department is already correct
                    if (clinicId) {
                        const matchingClinic = this.clinics.find(c => c.id.toLowerCase() === clinicId.toLowerCase());
                        if (matchingClinic) {
                            this.booking.clinicId = matchingClinic.id;
                        }
                    } else if (this.clinics.length === 1) {
                        this.booking.clinicId = this.clinics[0].id;
                    }
                }
            } else if (clinicId) {
                const matchingClinic = this.clinics.find(c => c.id.toLowerCase() === clinicId.toLowerCase());
                if (matchingClinic && this.booking.clinicId !== matchingClinic.id) {
                    this.booking.clinicId = matchingClinic.id;
                }
            }
        };

        if (doctor.departmentId !== undefined) {
            fillDeptAndClinic(doctor.departmentId, doctor.clinicId, doctor.doctorPercentage);
        } else {
            // Auto-fill department and clinic by fetching full doctor details to be absolutely sure
            this.doctorService.get(this.booking.doctorId).subscribe(doc => {
                fillDeptAndClinic(doc.departmentId, doc.clinicId, doc.doctorPercentage);
            });
        }

        const appointmentDate = new Date(this.booking.appointmentDate);
        const hour = appointmentDate.getHours();

        let fee = doctor.consultationFee || 0;

        if (hour < 14) { // Morning: Before 2 PM
            fee = doctor.morningConsultationFee || doctor.consultationFee || 0;
        } else { // Evening: 2 PM or later
            fee = doctor.eveningConsultationFee || doctor.consultationFee || 0;
        }

        if (!this.booking.serviceItemId) {
            this.booking.payAmount = fee;
        }
    }

    loadLabTests() {
        this.labService.getTests({ maxResultCount: 1000 } as any).subscribe(res => {
            this.availableTests = res.items || [];
            this.filterTests(); // Apply any existing filter after loading
        });
    }

    filterTests() {
        if (!this.testSearchText || !this.testSearchText.trim()) {
            this.displayTests = [...this.availableTests];
            return;
        }
        const lower = this.testSearchText.trim().toLowerCase();
        
        this.displayTests = this.availableTests.filter(t =>
            (t.code && String(t.code).toLowerCase().includes(lower)) ||
            (t.name && String(t.name).toLowerCase().includes(lower)) ||
            (t['nameAr'] && String(t['nameAr']).toLowerCase().includes(lower))
        );
    }

    newPatient() {
        this.patientInfo = this.getEmptyPatient();
        // Auto-select default payment method for new patient
        const defaultMethod = this.paymentMethods.find(m => m.isDefault);
        if (defaultMethod) {
            this.patientInfo.paymentMethodId = defaultMethod.id;
            this.onPaymentMethodChange(defaultMethod.id);
        }
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
        const missingFields = [];
        if (!this.patientInfo.fullNameAr) missingFields.push('الاسم');
        if (!this.patientInfo.mobileNumber) missingFields.push('الموبايل');

        if (!this.patientInfo.paymentMethodId) missingFields.push('طريقة الدفع');

        if (missingFields.length > 0) {
            this.toaster.warn('يرجى إكمال البيانات المطلوبة: ' + missingFields.join('، '), 'بيانات ناقصة');
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
            this.searchResults = [];
            this.selectedSearchIndex = -1;
            return;
        }

        this.patientService.search(searchText).subscribe({
            next: (res) => {
                this.searchResults = res;
                this.selectedSearchIndex = -1;
            },
            error: (err) => {
                console.error(err);
                this.toaster.error('حدث خطأ أثناء البحث', 'خطأ');
            }
        });
    }

    onSearchKeyDown(event: KeyboardEvent) {
        if (!this.searchResults || this.searchResults.length === 0) return;

        if (event.key === 'ArrowDown') {
            event.preventDefault();
            this.selectedSearchIndex = Math.min(this.selectedSearchIndex + 1, this.searchResults.length - 1);
        } else if (event.key === 'ArrowUp') {
            event.preventDefault();
            this.selectedSearchIndex = Math.max(this.selectedSearchIndex - 1, 0);
        } else if (event.key === 'Enter') {
            event.preventDefault();
            if (this.selectedSearchIndex >= 0 && this.selectedSearchIndex < this.searchResults.length) {
                this.selectPatient(this.searchResults[this.selectedSearchIndex].id);
                this.searchResults = [];
                this.selectedSearchIndex = -1;
            }
        }
    }

    selectPatient(id: string) {
        this.patientService.get(id).subscribe({
            next: (res) => {
                this.patientInfo = res;
                this.calculateAge();
                this.searchResults = [];
                
                // Track patient balance/status if admitted
                this.patientBalanceGuard.checkPatient(id);
                
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
            this.calculateBillingTotals();
        }
    }

    removeTest(index: number) {
        this.selectedTests.splice(index, 1);
        this.calculateBillingTotals();
    }

    focusSearch() {
        this.testSearchInput?.nativeElement?.focus();
    }

    closeSearch() {
        setTimeout(() => {
            this.searchResults = [];
            this.selectedSearchIndex = -1;
        }, 200);
    }

    saveInvoice() {
        if (!this.patientInfo.id) {
            this.toaster.error('يجب حفظ بيانات المريض أولاً', 'خطأ');
            return;
        }

        if (!this.requestingDoctorId) {
            this.toaster.warn('يجب اختيار الطبيب المعالج', 'تنبيه');
            return;
        }

        if (this.selectedTests.length === 0) {
            this.toaster.warn('يجب اختيار فحص واحد على الأقل', 'تنبيه');
            return;
        }

        if (!this.patientBalanceGuard.canProceedWithService(this.billingDetails.patientShare)) {
            return;
        }

        const invoice = {
            patientId: this.patientInfo.id,
            dueDate: new Date().toISOString(),
            notes: 'Lab Request',
            patientInsuranceId: this.patientInfo.insurancePlanId, // pass plan id 
            items: this.selectedTests.map(test => ({
                serviceType: ServiceType.Laboratory,
                serviceCode: test.code,
                description: test.name,
                quantity: 1,
                unitPrice: test.price,
                discountPercentage: 0,
                isCoveredByInsurance: (this.labInsurancePercentage > 0) ? true : false,
                insurancePercentage: (this.labInsurancePercentage > 0) ? this.labInsurancePercentage : 0,
                notes: ''
            }))
        };

        this.invoiceService.create(invoice).subscribe({
            next: (res) => {
                this.toaster.success('تم حفظ الفاتورة بنجاح', 'نجاح');

                // Create Lab Requests for the Lab Module
                this.selectedTests.forEach(test => {
                    this.labService.createRequest({
                        patientId: this.patientInfo.id,
                        doctorId: this.requestingDoctorId,
                        serviceItemId: test.id,
                        notes: 'Created from Reception screen'
                    }).subscribe({
                        error: (err) => console.error('Error creating LabRequest', err)
                    });
                });

                // Create Payment if Paid Amount > 0
                if (this.billingDetails.paidAmount > 0) {
                    const paymentInput: any = {
                        patientId: this.patientInfo.id,
                        invoiceId: res.id,
                        amount: this.billingDetails.paidAmount,
                        paymentMethod: this.mapPaymentMethod(this.activePaymentType),
                        paymentDate: new Date().toISOString(),
                        referenceNumber: '',
                        notes: 'Lab Services Payment'
                    };

                    this.paymentService.create(paymentInput).subscribe({
                        next: () => {
                            this.toaster.success('تم حفظ الدفع بنجاح', 'نجاح');
                        },
                        error: (err) => {
                            console.error('Payment Error', err);
                            this.toaster.error('فشل حفظ الدفع', 'خطأ');
                        }
                    });
                }

                if (this.printTicketChecked) {
                    this.printInvoice(res.id);
                }
                
                // Clear state
                this.selectedTests = [];
                this.requestingDoctorId = '';
                this.billingDetails = {
                    total: 0,
                    discount: 0,
                    tax: 0,
                    grandTotal: 0,
                    insuranceShare: 0,
                    patientShare: 0,
                    cash: 0,
                    card: 0,
                    transfer: 0,
                    clientBalance: 0,
                    paidAmount: 0,
                    remainingAmount: 0,
                    applyTax: false
                };
            },
            error: (err) => {
                console.error(err);
                this.toaster.error('حدث خطأ أثناء حفظ الفاتورة', 'خطأ');
            }
        });
    }

    private mapPaymentMethod(type: string): number {
        switch (type) {
            case 'Cash': return 0;
            case 'Card': return 1;
            case 'Transfer': return 2;
            case 'ClientBalance': return 3;
            default: return 0;
        }
    }

    // --- Clinic Booking Methods ---

    loadClinicData() {
        this.appointmentService.getClinicLookup().subscribe(res => {
            this.clinics = ((res as any[]) || []).map(x => ({ ...x, name: x.name || x.nameAr || x.nameEn }))
                .sort((a, b) => a.name.localeCompare(b.name));
        });

        // Load Departments
        this.http.get<any[]>(environment.apis.default.url + '/api/app/department/medical-departments-lookup').subscribe(res => {
            this.departments = (res || []).sort((a, b) => a.name.localeCompare(b.name));
        });

        // Load Services (Clinic Services)
        this.serviceItemService.getList({ maxResultCount: 1000 } as any).subscribe(res => {
            let items = (res.items || []).filter(x => x.category === ServiceCategory.Consultation || x.category === ServiceCategory.Procedure);
            
            // Sort items alphabetically by name first
            items.sort((a, b) => (a.name || '').localeCompare(b.name || ''));

            // Find "General Consultation" (كشف عام)
            const generalConsultation = items.find(x => x.name?.includes('كشف عام'));
            if (generalConsultation) {
                // Move it to the top
                items = items.filter(x => x.id !== generalConsultation.id);
                items.unshift(generalConsultation);
                
                // Set as default if not already set
                if (!this.booking.serviceItemId) {
                    this.booking.serviceItemId = generalConsultation.id;
                    this.updateConsultationFee();
                }
            }
            
            this.services = items;
        });
    }

    onDepartmentChange() {
        this.booking.clinicId = '';
        this.booking.doctorId = '';
        this.doctors = [];
        this.clinics = [];

        if (this.selectedDepartmentId) {
            // Filter Clinics by Department using Proxy Service
            this.clinicService.getByDepartment(this.selectedDepartmentId).subscribe(res => {
                this.clinics = (res || []).map(x => ({ ...x, name: x.nameAr || x.nameEn || (x as any).name }))
                    .sort((a, b) => a.name.localeCompare(b.name));
            });

            // Filter Doctors by Department (directly) using getList for full DTOs
            this.doctorService.getList({ departmentId: this.selectedDepartmentId, maxResultCount: 1000 } as any).subscribe(res => {
                this.doctors = (res.items || []).map(x => ({ ...x, name: x.nameAr || x.nameEn || (x as any).name }))
                    .sort((a, b) => (a.name || '').localeCompare(b.name || ''));
            });
        } else {
            // Load all if no department selected
            this.appointmentService.getClinicLookup().subscribe(res => {
                this.clinics = ((res as any[]) || []).map(x => ({ ...x, name: x.name || x.nameAr || x.nameEn }))
                    .sort((a, b) => a.name.localeCompare(b.name));
            });
            this.doctors = [];
        }
    }

    onClinicChange() {
        this.booking.doctorId = '';
        if (this.booking.clinicId) {
            // Pass both clinic and selectedDepartmentId to narrow down using getList for full DTOs
            this.doctorService.getList({ clinicId: this.booking.clinicId, departmentId: this.selectedDepartmentId || undefined, maxResultCount: 1000 } as any).subscribe(res => {
                this.doctors = (res.items || []).map(x => ({ 
                    ...x, 
                    name: x.nameAr || x.nameEn || (x as any).name 
                })).sort((a, b) => (a.name || '').localeCompare(b.name || ''));
            });
        } else if (this.selectedDepartmentId) {
            // Fallback to department doctors if clinic is cleared
            this.doctorService.getList({ departmentId: this.selectedDepartmentId, maxResultCount: 1000 } as any).subscribe(res => {
                this.doctors = (res.items || []).map(x => ({ ...x, name: x.nameAr || x.nameEn || (x as any).name }))
                    .sort((a, b) => (a.name || '').localeCompare(b.name || ''));
            });
        } else {
            this.doctors = [];
        }
    }

    bookAppointment(type: 'statement' | 'bond' | 'followup') {
        if (!this.patientInfo.id) {
            this.toaster.error('يجب اختيار مريض أولاً', 'خطأ');
            return;
        }
        if (!this.booking.clinicId || !this.booking.doctorId || !this.booking.appointmentDate) {
            this.toaster.warn('يرجى تعبئة جميع الحقول المطلوبة (العيادة، الطبيب، التاريخ)', 'تنبيه');
            return;
        }

        if (!this.patientBalanceGuard.canProceedWithService(this.getClinicPatientShare())) {
            return;
        }

        const input: any = {
            patientId: this.patientInfo.id,
            clinicId: this.booking.clinicId,
            doctorId: this.booking.doctorId,
            serviceItemId: (this.booking.serviceItemId && this.booking.serviceItemId !== '') ? this.booking.serviceItemId : null,
            appointmentDate: this.booking.appointmentDate,
            createInvoice: type !== 'followup', // Don't create invoice for simple follow-up unless specified
            paymentMethod: this.booking.paymentMethod,
            discount: this.booking.discount,
            insurancePercentage: this.clinicsInsurancePercentage,
            patientInsuranceId: this.patientInfo.insurancePlanId
        };

        // Payment Logic
        // Always pass the paid amount if the user entered it, regardless of button clicked.
        // The difference between statement and bond is primarily what gets printed/reported.
        if (type === 'followup') {
            // Follow-up: usually free or pre-paid
            input.paidAmount = 0;
            // You might want to flag this as a follow-up in the backend if the API supports it
        } else {
            // For both 'bond' and 'statement', process the entered payment amount
            input.paidAmount = this.booking.payAmount || 0;
        }

        this.appointmentService.bookClinicAppointment(input).subscribe({
            next: (res) => {
                const msg = type === 'followup' ? 'تم حجز المتابعة بنجاح' : 'تم حجز الموعد وإنشاء الفاتورة (' + type + ')';
                this.toaster.success(msg, 'نجاح');

                if (this.printTicketChecked) {
                    this.printTicket(res.id);
                }
                // Reset/Navigation logic here if needed
            },
            error: (err) => {
                console.error(err);
                this.toaster.error('حدث خطأ أثناء حجز الموعد', 'خطأ');
            }
        });
    }

    printRegistrationForm() {
        if (!this.patientInfo.id) {
            this.toaster.warn('يرجى اختيار مريض', 'تنبيه');
            return;
        }
        // Placeholder for future implementation
        this.toaster.info('جاري طباعة استمارة التسجيل...', 'طباعة');
        // Logic to fetch PDF or window.print() would go here
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
            this.admission.roomId = null;
            this.onRoomChange();
            return;
        }
        this.roomService.getAvailableRooms(this.admission.roomType).subscribe(res => {
            this.availableRooms = res || [];
            this.admission.roomId = null;
            this.onRoomChange();
        });
    }

    onRoomChange() {
        if (!this.admission.roomId) {
            this.availableBedsList = [];
            this.admission.bedId = null;
            return;
        }

        this.roomService.get(this.admission.roomId).subscribe(res => {
            this.availableBedsList = (res.beds || []).filter(b => b.status === BedStatus.Available);
            this.admission.bedId = null;
        });
    }

    saveAdmission() {
        if (!this.patientInfo.id) {
            this.toaster.error('يجب اختيار مريض أولاً', 'خطأ');
            return;
        }
        if (!this.admission.roomId || !this.admission.bedId) {
            this.toaster.warn('يرجى اختيار غرفة وسرير', 'تنبيه');
            return;
        }

        const input = {
            patientId: this.patientInfo.id,
            roomId: this.admission.roomId,
            bedId: this.admission.bedId,
            insuranceCeiling: this.admission.insuranceCeiling,
            companionName: this.admission.companionName,
            companionPhone: this.admission.companionPhone,
            companionAddress: this.admission.companionAddress,
            purpose: this.admission.purpose,
            pharmacyPercentage: this.admission.pharmacyPercentage,
            isServicesStopped: this.admission.isServicesStopped,
            notes: this.admission.notes,
            numberOfDays: this.admission.numberOfDays || 0,
            paidAmount: this.admission.paidAmount || 0
        };

        this.isSavingAdmission = true;

        this.admissionService.create(input).subscribe({
            next: () => {
                this.isSavingAdmission = false;
                this.toaster.success('تم تسجيل التنويم بنجاح', 'نجاح');
                this.loadInpatientList();
                this.admission = {
                    roomType: null,
                    roomId: null,
                    bedId: null,
                    insuranceCeiling: 0,
                    companionName: '',
                    companionPhone: '',
                    companionAddress: '',
                    purpose: '',
                    pharmacyPercentage: 0,
                    isServicesStopped: false,
                    notes: '',
                    numberOfDays: 0,
                    paidAmount: 0
                };
                this.availableRooms = [];
                this.availableBedsList = [];
            },
            error: (err) => {
                this.isSavingAdmission = false;
                console.error(err);
                this.toaster.error('حدث خطأ أثناء حفظ بيانات التنويم', 'خطأ');
            }
        });
    }

    loadInpatientList() {
        if (!this.patientInfo.id) return;
        this.admissionService.getList({ patientId: this.patientInfo.id } as any).subscribe(res => {
            this.inpatientList = res.items || [];
            this.selectedAdmission = null;
        });
    }

    selectedAdmission: any = null;

    selectAdmission(item: any) {
        this.selectedAdmission = item;
        
        // Find roomType ID from roomTypeName since backend returns roomTypeName
        let rtId = null;
        if (item.roomTypeName) {
            const rtObj = this.roomTypes.find(rt => rt.name === item.roomTypeName);
            if (rtObj) rtId = rtObj.id;
        }

        // Populate the form fields with existing admission data
        this.admission = {
            roomType: rtId,
            roomId: item.roomId ?? null,
            bedId: item.bedId ?? null,
            insuranceCeiling: item.insuranceCeiling ?? 0,
            companionName: item.companionName ?? '',
            companionPhone: item.companionPhone ?? '',
            companionAddress: item.companionAddress ?? '',
            purpose: item.purpose ?? '',
            pharmacyPercentage: item.pharmacyPercentage ?? 0,
            isServicesStopped: item.isServicesStopped ?? false,
            notes: item.notes ?? '',
            numberOfDays: item.numberOfDays ?? 0,
            paidAmount: item.paidAmount ?? 0,
            admissionDate: item.admissionDate ? new Date(item.admissionDate).toISOString().slice(0, 16) : new Date().toISOString().slice(0, 16)
        };
        // Load rooms for selected type so dropdowns are populated
        if (rtId !== null) {
            this.roomService.getAvailableRooms(rtId).subscribe(res => {
                this.availableRooms = res || [];
                if (item.roomId) {
                    this.admission.roomId = item.roomId;
                    this.roomService.get(item.roomId).subscribe(roomRes => {
                        // Include both occupied and available beds so current bed shows
                        this.availableBedsList = (roomRes.beds || []).filter((b: any) =>
                            b.status === BedStatus.Available || b.id === item.bedId
                        );
                        this.admission.bedId = item.bedId ?? null;
                    });
                }
            });
        }
    }

    updateAdmission() {
        if (!this.selectedAdmission) return;
        this.isSavingAdmission = true;
        const updateDto = {
            ...this.selectedAdmission,
            companionName: this.admission.companionName,
            companionPhone: this.admission.companionPhone,
            companionAddress: this.admission.companionAddress,
            purpose: this.admission.purpose,
            pharmacyPercentage: this.admission.pharmacyPercentage,
            isServicesStopped: this.admission.isServicesStopped,
            notes: this.admission.notes,
            numberOfDays: this.admission.numberOfDays,
            paidAmount: this.admission.paidAmount,
            patientInsuranceId: this.selectedAdmission.patientInsuranceId
        };
        this.admissionService.update(this.selectedAdmission.id, updateDto).subscribe({
            next: (updated) => {
                this.isSavingAdmission = false;
                this.toaster.success('تم التعديل بنجاح', 'نجاح');
                this.selectedAdmission = updated;
                this.patientBalanceGuard.checkPatient(this.patientInfo.id);
                this.loadInpatientList();
            },
            error: (err) => {
                this.isSavingAdmission = false;
                console.error(err);
                this.toaster.error('فشل التعديل', 'خطأ');
            }
        });
    }

    clearAdmissionSelection() {
        this.selectedAdmission = null;
        this.admission = {
            roomType: null,
            roomId: null,
            bedId: null,
            insuranceCeiling: 0,
            companionName: '',
            companionPhone: '',
            companionAddress: '',
            purpose: '',
            pharmacyPercentage: 0,
            isServicesStopped: false,
            notes: '',
            numberOfDays: 0,
            paidAmount: 0
        };
        this.availableRooms = [];
        this.availableBedsList = [];
    }

    printAdmissionInvoice() {
        if (!this.selectedAdmission) {
            this.toaster.warn('يرجى اختيار التنويم من القائمة أولاً', 'تنبيه');
            return;
        }

        if (!this.selectedAdmission.invoiceId) {
            this.toaster.info('لم يتم إصدار فاتورة لهذا التنويم بعد', 'تنبيه');
            return;
        }

        this.invoiceService.getInvoicePdf(this.selectedAdmission.invoiceId).subscribe({
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
                this.toaster.error('فشل طباعة الفاتورة', 'خطأ');
            }
        });
    }

    updateAdmissionDays() {
        if (!this.selectedAdmission) {
            this.toaster.warn('يرجى اختيار التنويم من القائمة أولاً', 'تنبيه');
            return;
        }
        this.newAdmissionDays = this.selectedAdmission.numberOfDays;
        this.showAdmissionDaysModal = true;
    }

    confirmUpdateAdmissionDays() {
        if (this.newAdmissionDays >= 0) {
            this.admissionService.updateDays(this.selectedAdmission.id, this.newAdmissionDays).subscribe({
                next: () => {
                    this.toaster.success('تم التعديل بنجاح', 'نجاح');
                    this.showAdmissionDaysModal = false;
                    this.loadInpatientList();
                },
                error: (err) => {
                    console.error(err);
                    this.toaster.error('فشل تعديل عدد الأيام', 'خطأ');
                }
            });
        } else {
            this.toaster.warn('الرجاء إدخال رقم صحيح', 'تنبيه');
        }
    }

    dischargeAdmission() {
        if (!this.selectedAdmission) {
            this.toaster.warn('يرجى اختيار التنويم من القائمة أولاً', 'تنبيه');
            return;
        }
        this.confirmation.warn('هل أنت متأكد من إصدار إذن الخروج لهذا المريض؟', 'تأكيد').subscribe(status => {
            if (status === Confirmation.Status.confirm) {
                const input = {
                    dischargeDate: new Date().toISOString(),
                    notes: ''
                };
                this.admissionService.discharge(this.selectedAdmission.id, input).subscribe({
                    next: () => {
                        this.toaster.success('تم إصدار إذن الخروج بنجاح', 'نجاح');
                        this.loadInpatientList();
                        this.selectedAdmission = null;
                    },
                    error: (err) => {
                        console.error(err);
                        this.toaster.error('حدث خطأ أثناء إصدار إذن الخروج', 'خطأ');
                    }
                });
            }
        });
    }

    getAdmissionStatusName(status: number): string {
        const statusMap: { [key: number]: string } = {
            0: 'نشط',
            1: 'خرج',
            2: 'نقل',
            3: 'ملغي'
        };
        return statusMap[status] || 'غير معروف';
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

    onMedicalCategoryChange() {
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

        if (this.medicalServicesInsurancePercentage > 0) {
            this.selectedMedicalServices.forEach(curr => {
                curr.insuranceShare = Math.round(curr.price * (this.medicalServicesInsurancePercentage / 100));
                curr.patientShare = curr.price - curr.insuranceShare;
            });
        } else {
            this.selectedMedicalServices.forEach(curr => {
                curr.insuranceShare = 0;
                curr.patientShare = curr.price;
            });
        }

        this.medicalServicesPatientShare = this.selectedMedicalServices.reduce((acc, curr) => acc + curr.patientShare, 0);
        this.medicalServicesInsuranceShare = this.selectedMedicalServices.reduce((acc, curr) => acc + curr.insuranceShare, 0);

        // Calculate Net and Remaining
        const netTotal = this.medicalServicesTotal - this.medicalServicesPayment.discount;
        this.medicalServicesPayment.remainingAmount = netTotal - this.medicalServicesPayment.amountPaid;
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

        const patientTotalShare = this.selectedMedicalServices.reduce((acc, curr) => acc + (curr.patientShare || curr.price), 0);
        if (!this.patientBalanceGuard.canProceedWithService(patientTotalShare)) {
            return;
        }

        const invoiceInput: any = {
            patientId: this.patientInfo.id,
            dueDate: new Date().toISOString(),
            discountAmount: this.medicalServicesPayment.discount,
            taxPercentage: 0, // Should be from config
            patientInsuranceId: this.patientInfo.insurancePlanId,
            items: this.selectedMedicalServices.map(x => ({
                serviceItemId: x.id,
                serviceType: x.serviceType,
                description: x.name,
                quantity: 1,
                unitPrice: x.price,
                discountPercentage: 0, // Line item discount?
                isCoveredByInsurance: (this.medicalServicesInsurancePercentage > 0) ? true : false,
                insurancePercentage: (this.medicalServicesInsurancePercentage > 0) ? this.medicalServicesInsurancePercentage : 0
            }))
        };

        this.invoiceService.create(invoiceInput).subscribe({
            next: (invoice) => {
                this.toaster.success('تم حفظ فاتورة الخدمات الطبية بنجاح', 'نجاح');

                // Create Payment if Amount Paid > 0
                if (this.medicalServicesPayment.amountPaid > 0) {
                    const paymentInput: any = {
                        patientId: this.patientInfo.id,
                        invoiceId: invoice.id,
                        amount: this.medicalServicesPayment.amountPaid,
                        paymentMethod: this.mapPaymentMethod(this.activePaymentType),
                        paymentDate: new Date().toISOString(),
                        referenceNumber: '',
                        notes: 'Medical Services Ticket Payment'
                    };

                    this.paymentService.create(paymentInput).subscribe({
                        next: () => {
                            this.toaster.success('تم حفظ الدفع بنجاح', 'نجاح');
                        },
                        error: (err) => {
                            console.error('Payment Error', err);
                            this.toaster.error('فشل حفظ الدفع', 'خطأ');
                        }
                    });
                }

                // Reset and Print
                this.selectedMedicalServices = [];
                this.updateMedicalServicesTotals();
                this.medicalServicesPayment = { amountPaid: 0, remainingAmount: 0, discount: 0, paymentMethod: 0 };

                if (this.printMedicalServicesTicket) {
                    this.printInvoice(invoice.id);
                }
            },
            error: (err) => {
                console.error(err);
                this.toaster.error('حدث خطأ أثناء حفظ الفاتورة', 'خطأ');
            }
        });
    }

    printInvoice(invoiceId: string) {
        this.invoiceService.getInvoicePdf(invoiceId).subscribe({
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
                this.toaster.error('فشل طباعة الفاتورة', 'خطأ');
            }
        });
    }

    activePaymentType: string = 'Cash'; // Default

    onPaymentMethodChange(methodId: string) {
        if (!methodId) {
            this.activePaymentType = '';
            return;
        }
        const method = this.paymentMethods.find(m => m.id === methodId);
        if (method) {
            // Determine type
            const name = ((method.nameAr || '') + (method.nameEn || '')).toLowerCase();
            if (name.includes('نقدا') || name.includes('cash') || name.includes('نقدي')) {
                this.activePaymentType = 'Cash';
                this.booking.paymentMethod = 'Cash';
            } else if (name.includes('شبكة') || name.includes('network') || name.includes('card')) {
                this.activePaymentType = 'Card';
                this.booking.paymentMethod = 'Card';
            } else if (name.includes('تحويل') || name.includes('transfer')) {
                this.activePaymentType = 'Transfer';
            } else if (name.includes('رصيد') || name.includes('balance') || name.includes('client')) {
                this.activePaymentType = 'ClientBalance';
            } else {
                this.activePaymentType = 'Other';
            }

            // Auto-fill the selected method with the remaining amount (or full total if starting fresh)
            // For simplicity, let's put the full Grand Total into the selected method
            this.billingDetails.cash = 0;
            this.billingDetails.card = 0;
            this.billingDetails.transfer = 0;
            this.billingDetails.clientBalance = 0;

            if (this.activePaymentType === 'Cash') this.billingDetails.cash = this.billingDetails.grandTotal;
            if (this.activePaymentType === 'Card') this.billingDetails.card = this.billingDetails.grandTotal;
            if (this.activePaymentType === 'Transfer') this.billingDetails.transfer = this.billingDetails.grandTotal;
            if (this.activePaymentType === 'ClientBalance') this.billingDetails.clientBalance = this.billingDetails.grandTotal;

            this.calculateBillingTotals();
        }
    }

    calculateBillingTotals() {
        // Calculate Total from Services
        const testsTotal = this.selectedTests.reduce((sum, item) => sum + (item.price || 0), 0);
        this.billingDetails.total = testsTotal;

        // Calculate Tax (14% of the Total if checked)
        if (this.billingDetails.applyTax) {
            this.billingDetails.tax = Math.round(this.billingDetails.total * 0.14 * 100) / 100;
        } else {
            this.billingDetails.tax = 0;
        }

        // Logic: Grand Total = Total - Discount + Tax
        this.billingDetails.grandTotal = (this.billingDetails.total - (this.billingDetails.discount || 0)) + this.billingDetails.tax;

        // Insurance Calculation using the percentage
        if (this.labInsurancePercentage > 0) {
            // Apply percentage on the Total (before discount/tax)
            this.billingDetails.insuranceShare = Math.round(this.billingDetails.total * (this.labInsurancePercentage / 100));
        } else {
            this.billingDetails.insuranceShare = 0;
        }

        this.billingDetails.patientShare = this.billingDetails.grandTotal - this.billingDetails.insuranceShare;

        // Recalculate Active Payment Type Amount if needed (to keep it in sync with new Grand Total)
        // If we are currently editing one, maybe update it? 
        // Better: Update the active payment method amount to match the new Grand Total IF it was matching before or just force update it?
        // User behavior: if I check tax, total increases, I expect the payment field to increase automatically if I haven't manually split it.
        // For simplicity: Update the active payment method with remaining amount.

        // Calculate Paid Amount from inputs
        this.billingDetails.paidAmount =
            (this.billingDetails.cash || 0) +
            (this.billingDetails.card || 0) +
            (this.billingDetails.transfer || 0) +
            (this.billingDetails.clientBalance || 0);

        // Calculate Remaining
        this.billingDetails.remainingAmount = this.billingDetails.grandTotal - this.billingDetails.paidAmount;

        // Auto-update active payment method if there is a remaining amount and we are in "auto-fill" mode 
        // (implied by previous logic where we set full amount to active method)
        // Let's allow manual edit, but if method is switched, it auto-fills. 
        // Integrating tax update: if tax changes, remaining changes. User will see non-zero remaining and can adjust.
        // OR: we can auto-update the active field. Let's try to auto-update active field if it's the only one being used (common case).
        if (this.activePaymentType) {
            // If active type is Cash, and others are 0, update Cash to match GrandTotal
            // Check if others are zero
            const otherSum = this.billingDetails.paidAmount - (this.billingDetails[this.activePaymentType.toLowerCase()] || (this.activePaymentType === 'ClientBalance' ? this.billingDetails.clientBalance : 0));
            if (otherSum === 0) {
                if (this.activePaymentType === 'Cash') this.billingDetails.cash = this.billingDetails.grandTotal;
                else if (this.activePaymentType === 'Card') this.billingDetails.card = this.billingDetails.grandTotal;
                else if (this.activePaymentType === 'Transfer') this.billingDetails.transfer = this.billingDetails.grandTotal;
                else if (this.activePaymentType === 'ClientBalance') this.billingDetails.clientBalance = this.billingDetails.grandTotal;

                // Re-calculate paid amount after auto-update
                this.billingDetails.paidAmount = this.billingDetails.grandTotal;
                this.billingDetails.remainingAmount = 0;
            }
        }
    }

    // --- Patient Statement ---
    getPatientStatement() {
        if (!this.patientInfo.id) {
            this.toaster.warn('يرجى اختيار مريض أولاً', 'تنبيه');
            return;
        }

        const from = this.fromDate ? new Date(this.fromDate).toISOString() : undefined;
        const to = this.toDate ? new Date(this.toDate).toISOString() : undefined;

        const invoices$ = this.invoiceService.getList({
            patientId: this.patientInfo.id,
            fromDate: from,
            toDate: to,
            maxResultCount: 1000
        } as any).pipe(
            switchMap(invRes => {
                const items = invRes.items || [];
                if (items.length === 0) return of(invRes);
                
                const requests = items.map(i => this.invoiceService.getWithItems(i.id).pipe(
                    catchError(() => of(i)) // fallback to basic info on error
                ));
                return forkJoin(requests).pipe(
                    map(detailedInvoices => {
                        return { items: detailedInvoices, totalCount: invRes.totalCount };
                    })
                );
            })
        );

        const payments$ = this.paymentService.getList({
            patientId: this.patientInfo.id,
            fromDate: from,
            toDate: to,
            maxResultCount: 1000
        } as any);

        const deposits$ = this.inpatientDepositService.getList({
            patientId: this.patientInfo.id,
            fromDate: from,
            toDate: to,
            maxResultCount: 1000
        } as any);

        console.log('Fetching Patient Statement:', { patientId: this.patientInfo.id, from, to });

        forkJoin([invoices$, payments$, deposits$]).subscribe({
            next: ([invRes, payRes, depRes]) => {
                console.log('Statement Results:', { invoices: invRes.items?.length, payments: payRes.items?.length, deposits: depRes.items?.length });
                const invoices = (invRes.items || []).map(i => ({
                    id: i.id,
                    date: i.invoiceDate,
                    type: 'Invoice / فاتورة',
                    reference: i.invoiceNumber,
                    debit: i.totalAmount || 0,
                    credit: 0,
                    balance: 0,
                    status: i.status,
                    notes: i.items?.map(x => x.serviceCode).join(', '),
                    serviceName: i.items?.map(x => x.description || x.serviceCode).join(' + '),
                    originalItem: i
                }));

                const payments = (payRes.items || []).map(p => ({
                    id: p.id,
                    date: p.paymentDate,
                    type: 'Payment / سند قبض',
                    reference: p.paymentNumber,
                    debit: 0,
                    credit: p.amount || 0,
                    balance: 0,
                    status: p.status,
                    notes: p.paymentMethod + (p.referenceNumber ? ' - ' + p.referenceNumber : ''),
                    serviceName: '-',
                    originalItem: p
                }));

                const deposits = (depRes.items || []).map(d => ({
                    id: d.id,
                    date: d.depositDate,
                    type: 'Deposit / دفعة تنويم',
                    reference: d.receiptNumber,
                    debit: 0,
                    credit: d.amount || 0,
                    balance: 0,
                    status: d.status,
                    notes: 'دفعة مقدمة - تنويم',
                    serviceName: '-',
                    originalItem: d
                }));

                // Merge and Sort
                let runningBalance = 0;
                this.patientStatement = [];
                const combined = [...invoices, ...payments, ...deposits].sort((a, b) => {
                    const dateA = a.date ? new Date(a.date).getTime() : 0;
                    const dateB = b.date ? new Date(b.date).getTime() : 0;
                    return dateA - dateB;
                });

                combined.forEach(item => {
                    runningBalance += (item.debit - item.credit);
                    item.balance = runningBalance;
                    this.patientStatement.push(item);
                });

                // Calculate Summary
                this.statementSummary.totalDebit = this.patientStatement.reduce((sum, item) => sum + item.debit, 0);
                this.statementSummary.totalCredit = this.patientStatement.reduce((sum, item) => sum + item.credit, 0);
                this.statementSummary.balance = this.statementSummary.totalDebit - this.statementSummary.totalCredit;
            },
            error: (err) => {
                console.error('Error fetching statement:', err);
                this.toaster.error('حدث خطأ أثناء تحميل كشف الحساب', 'خطأ');
            }
        });
    }

    viewStatementDetails(item: any) {
        this.selectedStatementItem = item;
        // If it's an invoice and items are missing, we might need to fetch full invoice details
        if (item.type.includes('Invoice') && (!item.originalItem.items || item.originalItem.items.length === 0)) {
            this.invoiceService.getWithItems(item.id).subscribe({
                next: (res) => {
                    this.selectedStatementItem.originalItem = res;
                    this.selectedStatementItem.serviceName = res.items?.map(x => x.description || x.serviceCode).join(' + ') || '-';
                    // Update in list as well
                    const listItem = this.patientStatement.find(x => x.id === item.id);
                    if (listItem) {
                        listItem.originalItem = res;
                        listItem.serviceName = this.selectedStatementItem.serviceName;
                    }
                    this.showStatementDetailsModal = true;
                },
                error: (err) => {
                    console.error(err);
                    this.toaster.error('حدث خطأ أثناء جلب تفاصيل الفاتورة', 'خطأ');
                }
            });
        } else {
            this.showStatementDetailsModal = true;
        }
    }

    cancelInvoice(item: any) {
        this.confirmation.warn(
            `هل أنت متأكد من إلغاء الفاتورة رقم ${item.reference}؟ سيتم عكس القيود المحاسبية واسترداد المدفوعات.`,
            'تأكيد الإلغاء'
        ).subscribe((status) => {
            if (status === Confirmation.Status.confirm) {
                this.http.post(`${environment.apis.default.url}/api/app/invoice/${item.id}/cancel`, {}).subscribe({
                    next: () => {
                        this.toaster.success('تم إلغاء الفاتورة بنجاح');
                        this.getPatientStatement();
                    },
                    error: (err) => {
                        console.error(err);
                        this.toaster.error('حدث خطأ أثناء إلغاء الفاتورة');
                    }
                });
            }
        });
    }

    refundPayment(item: any) {
        this.selectedRefundItem = item;
        this.refundReason = '';
        this.showRefundModal = true;
    }

    confirmRefundPayment() {
        if (!this.refundReason.trim()) {
            this.toaster.warn('يرجى إدخال سبب الاسترداد', 'تنبيه');
            return;
        }
        this.http.post(`${environment.apis.default.url}/api/app/payment/${this.selectedRefundItem.id}/refund?reason=${this.refundReason}`, {}).subscribe({
            next: () => {
                this.toaster.success('تم استرداد المبلغ بنجاح');
                this.showRefundModal = false;
                this.getPatientStatement();
            },
            error: (err) => {
                console.error(err);
                this.toaster.error('حدث خطأ أثناء استرداد المبلغ');
            }
        });
    }
}
