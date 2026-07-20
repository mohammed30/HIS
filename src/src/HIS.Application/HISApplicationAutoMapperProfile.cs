using AutoMapper;
using HIS.Notifications;
using HIS.Settings;
using HIS.Settings.Dtos;
using Volo.Abp.AutoMapper;

namespace HIS;

public class HISApplicationAutoMapperProfile : Profile
{
    public HISApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        CreateMap<Specialty, SpecialtyDto>();
        CreateMap<CreateUpdateSpecialtyDto, Specialty>();

        CreateMap<Department, DepartmentDto>();
        CreateMap<CreateUpdateDepartmentDto, Department>();

        CreateMap<JobTitle, JobTitleDto>();
        CreateMap<CreateUpdateJobTitleDto, JobTitle>();

        CreateMap<Clinic, ClinicDto>();
        CreateMap<CreateUpdateClinicDto, Clinic>();

        CreateMap<Doctor, DoctorDto>()
            .ForMember(x => x.ClinicName, map => map.MapFrom(s => s.ClinicId != null ? "Clinic Name" : null)); // Will be populated in AppService if needed, or via Include
        CreateMap<CreateUpdateDoctorDto, Doctor>();
        
        CreateMap<Appointments.Appointment, Appointments.Dtos.AppointmentDto>()
             .ForMember(x => x.PatientName, map => map.MapFrom(s => "Patient Name")) // Ideally join with Patient Repository or lookup
             .ForMember(x => x.DoctorName, map => map.MapFrom(s => "Doctor Name"))
             .ForMember(x => x.ClinicName, map => map.MapFrom(s => "Clinic Name"));
             
        CreateMap<Appointments.Dtos.CreateAppointmentDto, Appointments.Appointment>();

        CreateMap<Appointments.WaitingList, Appointments.Dtos.WaitingListDto>();
        CreateMap<Appointments.Dtos.CreateUpdateWaitingListDto, Appointments.WaitingList>(); // Corrected type mapping
        
        CreateMap<HIS.Settings.Laboratory, HIS.Settings.LaboratoryDto>();
        CreateMap<HIS.Settings.CreateUpdateLaboratoryDto, HIS.Settings.Laboratory>();

        CreateMap<Appointments.DoctorSchedule, Appointments.DoctorScheduleDto>();
        // Financials (Accounting)
        CreateMap<Accounting.Account, Accounting.Dtos.AccountDto>();
        CreateMap<Accounting.Dtos.CreateUpdateAccountDto, Accounting.Account>()
            .ForMember(x => x.Code, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Code)));
        
        CreateMap<Accounting.JournalEntry, Accounting.Dtos.JournalEntryDto>();
        CreateMap<Accounting.JournalEntryLine, Accounting.Dtos.JournalEntryLineDto>();

        // Vouchers & Claims
        CreateMap<Accounting.PaymentVoucher, Accounting.Dtos.PaymentVoucherDto>()
             .ForMember(x => x.SupplierName, opt => opt.Ignore()) // Populate in AppService or via Include
             .ForMember(x => x.PaymentMethodName, opt => opt.Ignore());
        CreateMap<Accounting.PaymentVoucherLine, Accounting.Dtos.PaymentVoucherLineDto>()
             .ForMember(x => x.AccountName, opt => opt.Ignore());
        CreateMap<Accounting.Dtos.CreateUpdatePaymentVoucherDto, Accounting.PaymentVoucher>();
        CreateMap<Accounting.Dtos.CreateUpdatePaymentVoucherLineDto, Accounting.PaymentVoucherLine>();

        CreateMap<Accounting.ReceiptVoucher, Accounting.Dtos.ReceiptVoucherDto>()
             .ForMember(x => x.PatientName, opt => opt.Ignore())
             .ForMember(x => x.PaymentMethodName, opt => opt.Ignore());
        CreateMap<Accounting.ReceiptVoucherLine, Accounting.Dtos.ReceiptVoucherLineDto>()
             .ForMember(x => x.AccountName, opt => opt.Ignore());
        CreateMap<Accounting.Dtos.CreateUpdateReceiptVoucherDto, Accounting.ReceiptVoucher>();
        CreateMap<Accounting.Dtos.CreateUpdateReceiptVoucherLineDto, Accounting.ReceiptVoucherLine>();

        CreateMap<Accounting.ContractClaim, Accounting.Dtos.ContractClaimDto>()
             .ForMember(x => x.ContractName, opt => opt.Ignore());
        CreateMap<Accounting.Dtos.CreateUpdateContractClaimDto, Accounting.ContractClaim>();

        CreateMap<Accounting.BankTransaction, Accounting.Dtos.BankTransactionDto>();
        CreateMap<Accounting.Dtos.CreateUpdateBankTransactionDto, Accounting.BankTransaction>();

        CreateMap<Accounting.AccountMapping, Accounting.Dtos.AccountMappingDto>()
            .ForMember(x => x.AccountCode, opt => opt.MapFrom(src => src.Account != null ? src.Account.Code : null))
            .ForMember(x => x.AccountName, opt => opt.MapFrom(src => src.Account != null ? src.Account.Name : null))
            .ForMember(x => x.AccountNameAr, opt => opt.MapFrom(src => src.Account != null ? src.Account.NameAr : null));

        // Inventory
        CreateMap<Inventory.Warehouse, Inventory.Dtos.WarehouseDto>();
        CreateMap<Inventory.Dtos.CreateUpdateWarehouseDto, Inventory.Warehouse>();
        
        CreateMap<Inventory.InventoryItem, Inventory.Dtos.InventoryItemDto>();
        CreateMap<Inventory.InventoryTransaction, Inventory.Dtos.InventoryTransactionDto>();
        
        CreateMap<Inventory.InventoryCount, Inventory.InventoryCountDto>()
            .ForMember(x => x.WarehouseName, opt => opt.Ignore());
        CreateMap<Inventory.InventoryCountItem, Inventory.InventoryCountItemDto>()
            .ForMember(x => x.ProductName, opt => opt.Ignore());

        CreateMap<Inventory.Supplier, Inventory.Dtos.SupplierDto>();
        CreateMap<Inventory.Dtos.CreateUpdateSupplierDto, Inventory.Supplier>();

        CreateMap<Inventory.PurchaseOrder, Inventory.Dtos.PurchaseOrderDto>()
            .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier.Name));
        
        CreateMap<Inventory.PurchaseOrderLine, Inventory.Dtos.PurchaseOrderLineDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));
        CreateMap<Inventory.Dtos.CreateUpdatePurchaseOrderDto, Inventory.PurchaseOrder>()
            .ForMember(x => x.PurchaseOrderLines, opt => opt.Ignore());

        CreateMap<ActivityLogs.ActivityLog, ActivityLogs.ActivityLogDto>();

        // Insurance
        CreateMap<Insurance.InsuranceCompany, Insurance.InsuranceCompanyDto>();
        CreateMap<Insurance.CreateUpdateInsuranceCompanyDto, Insurance.InsuranceCompany>();
        
        CreateMap<Insurance.InsurancePlan, Insurance.InsurancePlanDto>();
        CreateMap<Insurance.CreateUpdateInsurancePlanDto, Insurance.InsurancePlan>();
        
        CreateMap<Insurance.PatientInsurance, Insurance.PatientInsuranceDto>();
        CreateMap<Insurance.CreateUpdatePatientInsuranceDto, Insurance.PatientInsurance>();

        CreateMap<Insurance.InsuranceServicePrice, Insurance.InsuranceServicePriceDto>();
        CreateMap<Insurance.CreateUpdateInsuranceServicePriceDto, Insurance.InsuranceServicePrice>();

        // Billing
        CreateMap<Billing.Invoice, Billing.InvoiceDto>();
        CreateMap<Billing.CreateUpdateInvoiceDto, Billing.Invoice>();
        
        CreateMap<Billing.InvoiceItem, Billing.InvoiceItemDto>();
        CreateMap<Billing.CreateUpdateInvoiceItemDto, Billing.InvoiceItem>();
        
        CreateMap<Billing.Payment, Billing.PaymentDto>();
        CreateMap<Billing.CreatePaymentDto, Billing.Payment>();
        
        CreateMap<Billing.DeferredPayment, Billing.DeferredPaymentDto>();
        CreateMap<Billing.CreateDeferredPaymentDto, Billing.DeferredPayment>();

        CreateMap<Billing.InpatientDeposit, Billing.InpatientDepositDto>()
             .ForMember(x => x.PatientName, opt => opt.Ignore());
        CreateMap<Billing.CreateInpatientDepositDto, Billing.InpatientDeposit>();

        // Medical Records
        CreateMap<MedicalRecords.MedicalHistory, MedicalRecords.MedicalHistoryDto>();
        CreateMap<MedicalRecords.CreateUpdateMedicalHistoryDto, MedicalRecords.MedicalHistory>();
        
        CreateMap<MedicalRecords.Diagnosis, MedicalRecords.DiagnosisDto>();
        CreateMap<MedicalRecords.CreateUpdateDiagnosisDto, MedicalRecords.Diagnosis>();
        
        CreateMap<MedicalRecords.VitalSign, MedicalRecords.VitalSignDto>();
        CreateMap<MedicalRecords.CreateUpdateVitalSignDto, MedicalRecords.VitalSign>();
        
        CreateMap<MedicalRecords.Allergy, MedicalRecords.AllergyDto>();
        CreateMap<MedicalRecords.CreateUpdateAllergyDto, MedicalRecords.Allergy>();
        
        CreateMap<MedicalRecords.PatientNote, MedicalRecords.PatientNoteDto>();
        CreateMap<MedicalRecords.CreateUpdatePatientNoteDto, MedicalRecords.PatientNote>();

        // Patients
        CreateMap<Patients.Patient, Patients.PatientDto>();
        CreateMap<Patients.CreateUpdatePatientDto, Patients.Patient>();

        // Services & Radiology
        CreateMap<Services.ServiceItem, Services.ServiceItemDto>();
        CreateMap<Services.CreateUpdateServiceItemDto, Services.ServiceItem>();
        
        CreateMap<Services.RadiologyItem, Services.RadiologyItemDto>();
        CreateMap<Services.CreateUpdateRadiologyItemDto, Services.RadiologyItem>();

        CreateMap<Radiology.RadiologyRequest, Radiology.RadiologyRequestDto>();
        CreateMap<Radiology.CreateUpdateRadiologyRequestDto, Radiology.RadiologyRequest>();

        // Pricing
        CreateMap<Pricing.PriceList, Pricing.PriceListDto>();
        CreateMap<Pricing.CreateUpdatePriceListDto, Pricing.PriceList>();
        
        CreateMap<Pricing.ServicePrice, Pricing.ServicePriceDto>();
        CreateMap<Pricing.CreateUpdateServicePriceDto, Pricing.ServicePrice>();

        // Laboratory (Module)
        CreateMap<HIS.Laboratory.LabTest, HIS.Laboratory.Dtos.LabTestDto>();
        CreateMap<HIS.Laboratory.Dtos.CreateUpdateLabTestDto, HIS.Laboratory.LabTest>();
        
        CreateMap<HIS.Laboratory.LabRequest, HIS.Laboratory.Dtos.LabRequestDto>()
            .ForMember(dest => dest.PatientName, opt => opt.Ignore())
            .ForMember(dest => dest.DoctorName, opt => opt.Ignore())
            .ForMember(dest => dest.TestName, opt => opt.Ignore())
            .ForMember(dest => dest.TestCode, opt => opt.Ignore());
            
        CreateMap<HIS.Laboratory.Dtos.CreateLabRequestDto, HIS.Laboratory.LabRequest>();

        // Emergency
        CreateMap<Emergency.EmergencyVisit, Emergency.Dtos.EmergencyVisitDto>()
             .ForMember(dest => dest.PatientName, opt => opt.Ignore());

        // Lab Appointments
        CreateMap<HIS.Laboratory.LabAppointment, HIS.Laboratory.Dtos.LabAppointmentDto>()
            .ForMember(dest => dest.PatientName, opt => opt.Ignore())
            .ForMember(dest => dest.TestName, opt => opt.Ignore())
            .ForMember(dest => dest.TestCode, opt => opt.Ignore());

        // Clinical (Medical Order)
        CreateMap<Clinical.MedicalOrder, Clinical.MedicalOrderDto>();
        CreateMap<Clinical.CreateUpdateMedicalOrderDto, Clinical.MedicalOrder>();
        
        // Pharmacy
        CreateMap<Clinical.MedicalOrder, Pharmacy.PendingPrescriptionDto>();
        CreateMap<Pharmacy.Drug, Pharmacy.Dtos.DrugDto>()
            .ForMember(x => x.ServiceItemName, opt => opt.Ignore()); // Can populate later if needed
        CreateMap<Pharmacy.Dtos.CreateUpdateDrugDto, Pharmacy.Drug>();

        CreateMap<Pharmacy.Dispensing, Pharmacy.Dtos.DispensingDto>()
            .ForMember(x => x.PatientName, opt => opt.Ignore());
        CreateMap<Pharmacy.DispensedItem, Pharmacy.Dtos.DispensedItemDto>()
            .ForMember(x => x.ProductName, opt => opt.Ignore());

        CreateMap<Pharmacy.StockTransfer, Pharmacy.Dtos.StockTransferDto>()
            .ForMember(x => x.FromWarehouseName, opt => opt.Ignore())
            .ForMember(x => x.ToWarehouseName, opt => opt.Ignore());
        CreateMap<Pharmacy.StockTransferItem, Pharmacy.Dtos.StockTransferItemDto>()
            .ForMember(x => x.DrugName, opt => opt.Ignore());

        // Procurement (Inventory)
        CreateMap<Inventory.PurchaseRequisition, Inventory.Dtos.PurchaseRequisitionDto>()
            .ForMember(x => x.RequestorName, opt => opt.Ignore())
            .ForMember(x => x.DepartmentName, opt => opt.Ignore());
        CreateMap<Inventory.PurchaseRequisitionLine, Inventory.Dtos.PurchaseRequisitionLineDto>()
            .ForMember(x => x.ProductName, opt => opt.Ignore());
        CreateMap<Inventory.Dtos.CreateUpdatePurchaseRequisitionDto, Inventory.PurchaseRequisition>();
        CreateMap<Inventory.Dtos.CreateUpdatePurchaseRequisitionLineDto, Inventory.PurchaseRequisitionLine>();

        CreateMap<Inventory.InternalRequest, Inventory.Dtos.InternalRequestDto>()
            .ForMember(x => x.RequestingDepartmentName, opt => opt.Ignore())
            .ForMember(x => x.FulfilledByWarehouseName, opt => opt.Ignore());
        CreateMap<Inventory.InternalRequestLine, Inventory.Dtos.InternalRequestLineDto>()
            .ForMember(x => x.InventoryItemName, opt => opt.Ignore());
        CreateMap<Inventory.Dtos.CreateUpdateInternalRequestDto, Inventory.InternalRequest>();
        CreateMap<Inventory.Dtos.CreateUpdateInternalRequestLineDto, Inventory.InternalRequestLine>();

        CreateMap<Inventory.PurchaseInvoice, Inventory.Dtos.PurchaseInvoiceDto>()
            .ForMember(x => x.SupplierName, opt => opt.Ignore())
            .ForMember(x => x.PurchaseOrderNumber, opt => opt.Ignore());
        CreateMap<Inventory.PurchaseInvoiceLine, Inventory.Dtos.PurchaseInvoiceLineDto>()
            .ForMember(x => x.ProductName, opt => opt.Ignore());
        CreateMap<Inventory.Dtos.CreateUpdatePurchaseInvoiceDto, Inventory.PurchaseInvoice>()
             .IgnoreFullAuditedObjectProperties();
        CreateMap<Inventory.Dtos.CreateUpdatePurchaseInvoiceLineDto, Inventory.PurchaseInvoiceLine>();

        // General Master Data (Definitions)
        CreateMap<General.Nationality, General.NationalityDto>();
        CreateMap<General.CreateUpdateNationalityDto, General.Nationality>();

        CreateMap<General.Profession, General.ProfessionDto>();
        CreateMap<General.CreateUpdateProfessionDto, General.Profession>();

        CreateMap<General.Contract, General.ContractDto>();
        CreateMap<General.CreateUpdateContractDto, General.Contract>();

        CreateMap<General.PatientCategory, General.PatientCategoryDto>();
        CreateMap<General.CreateUpdatePatientCategoryDto, General.PatientCategory>();

        CreateMap<General.ReferralSource, General.ReferralSourceDto>();
        CreateMap<General.CreateUpdateReferralSourceDto, General.ReferralSource>();

        CreateMap<General.PaymentMethod, General.Dtos.PaymentMethodDto>();
        CreateMap<General.Dtos.CreateUpdatePaymentMethodDto, General.PaymentMethod>();

        // Rooms
        CreateMap<Rooms.Room, Rooms.RoomDto>();
        CreateMap<Rooms.CreateUpdateRoomDto, Rooms.Room>();
        CreateMap<Rooms.Room, Rooms.RoomLookupDto>();

        CreateMap<Rooms.Bed, Rooms.BedDto>();

        // Inpatient (Admissions)
        CreateMap<Inpatient.Admission, Inpatient.AdmissionDto>()
            .ForMember(dest => dest.PatientName, opt => opt.Ignore())
            .ForMember(dest => dest.PatientFileNumber, opt => opt.Ignore())
            .ForMember(dest => dest.RoomNumber, opt => opt.Ignore())
            .ForMember(dest => dest.RoomTypeName, opt => opt.Ignore());
        CreateMap<Inpatient.CreateUpdateAdmissionDto, Inpatient.Admission>();

        CreateMap<Inpatient.Reservation, Inpatient.ReservationDto>();
        CreateMap<Inpatient.CreateUpdateReservationDto, Inpatient.Reservation>();

        // Operations (Surgery)
        CreateMap<Operations.SurgicalOperation, Operations.SurgicalOperationDto>()
            .ForMember(dest => dest.PatientName, opt => opt.Ignore())
            .ForMember(dest => dest.DoctorName, opt => opt.Ignore());
        CreateMap<Operations.CreateUpdateSurgicalOperationDto, Operations.SurgicalOperation>();

        // Nursing
        CreateMap<Nursing.MedicationAdministration, Nursing.MedicationAdministrationDto>()
             .ForMember(x => x.PatientName, opt => opt.Ignore());
        CreateMap<Nursing.CreateMedicationAdministrationDto, Nursing.MedicationAdministration>();

        // Phase 2
        CreateMap<Nursing.PatientRound, Nursing.PatientRoundDto>();
        CreateMap<Nursing.CreatePatientRoundDto, Nursing.PatientRound>();
        
        CreateMap<Nursing.PainAssessment, Nursing.PainAssessmentDto>();
        CreateMap<Nursing.CreatePainAssessmentDto, Nursing.PainAssessment>();
        
        CreateMap<Nursing.FallRiskAssessment, Nursing.FallRiskAssessmentDto>();
        CreateMap<Nursing.CreateFallRiskAssessmentDto, Nursing.FallRiskAssessment>();
        
        CreateMap<Nursing.WoundCare, Nursing.WoundCareDto>();
        CreateMap<Nursing.CreateWoundCareDto, Nursing.WoundCare>();
        
        CreateMap<Nursing.FluidBalance, Nursing.FluidBalanceDto>();
        CreateMap<Nursing.CreateFluidBalanceDto, Nursing.FluidBalance>();
        
        CreateMap<Nursing.ShiftHandover, Nursing.ShiftHandoverDto>();
        CreateMap<Nursing.CreateShiftHandoverDto, Nursing.ShiftHandover>();

        CreateMap<Nursing.CarePlan, Nursing.CarePlanDto>()
             .ForMember(x => x.PatientName, opt => opt.Ignore());
        CreateMap<Nursing.CreateCarePlanDto, Nursing.CarePlan>();

        CreateMap<Clinical.MedicalOrder, Nursing.DueMedicationDto>()
             .ForMember(x => x.DrugName, map => map.MapFrom(s => s.ServiceName))
             .ForMember(x => x.OrderDate, map => map.MapFrom(s => s.CreationTime));

        // HR (شؤون العاملين)
        CreateMap<HR.Employee, HR.EmployeeDto>();
        CreateMap<HR.CreateUpdateEmployeeDto, HR.Employee>();
        CreateMap<HR.JobGrade, HR.JobGradeDto>();
        CreateMap<HR.CreateUpdateJobGradeDto, HR.JobGrade>();
        CreateMap<HR.CompensationItem, HR.CompensationItemDto>();
        CreateMap<HR.CreateUpdateCompensationItemDto, HR.CompensationItem>();
        CreateMap<HR.LeaveType, HR.LeaveTypeDto>();
        CreateMap<HR.CreateUpdateLeaveTypeDto, HR.LeaveType>();
        CreateMap<HR.EmployeeLeave, HR.EmployeeLeaveDto>();
        CreateMap<HR.CreateUpdateEmployeeLeaveDto, HR.EmployeeLeave>();
        CreateMap<HR.EmployeeLoan, HR.EmployeeLoanDto>();
        CreateMap<HR.CreateUpdateEmployeeLoanDto, HR.EmployeeLoan>();
        CreateMap<HR.SalarySetup, HR.SalarySetupDto>();
        CreateMap<HR.CreateUpdateSalarySetupDto, HR.SalarySetup>();
        CreateMap<HR.PayrollRun, HR.PayrollRunDto>();
        CreateMap<HR.PayrollLine, HR.PaySlipLineDto>()
             .ForMember(x => x.ItemName, opt => opt.Ignore());
        CreateMap<HR.Penalty, HR.PenaltyDto>();
        CreateMap<HR.CreateUpdatePenaltyDto, HR.Penalty>();
        CreateMap<HR.AttendanceRecord, HR.AttendanceRecordDto>();
        CreateMap<HR.CreateUpdateAttendanceRecordDto, HR.AttendanceRecord>();
        CreateMap<HR.DailyAttendance, HR.DailyAttendanceDto>();
        CreateMap<HR.CreateUpdateDailyAttendanceDto, HR.DailyAttendance>();

        // Notifications
        CreateMap<Notification, NotificationDto>();
    }
}


