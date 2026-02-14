using AutoMapper;
using HIS.Settings;
using HIS.Settings.Dtos;

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

        CreateMap<Doctor, DoctorDto>();
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
        CreateMap<Accounting.Dtos.CreateUpdateAccountDto, Accounting.Account>();
        
        CreateMap<Accounting.JournalEntry, Accounting.Dtos.JournalEntryDto>();
        CreateMap<Accounting.JournalEntryLine, Accounting.Dtos.JournalEntryLineDto>();

        // Inventory
        CreateMap<Inventory.Warehouse, Inventory.Dtos.WarehouseDto>();
        CreateMap<Inventory.Dtos.CreateUpdateWarehouseDto, Inventory.Warehouse>();
        
        CreateMap<Inventory.InventoryItem, Inventory.Dtos.InventoryItemDto>();
        
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

        // Billing
        CreateMap<Billing.Invoice, Billing.InvoiceDto>();
        CreateMap<Billing.CreateUpdateInvoiceDto, Billing.Invoice>();
        
        CreateMap<Billing.InvoiceItem, Billing.InvoiceItemDto>();
        CreateMap<Billing.CreateUpdateInvoiceItemDto, Billing.InvoiceItem>();
        
        CreateMap<Billing.Payment, Billing.PaymentDto>();
        CreateMap<Billing.CreatePaymentDto, Billing.Payment>();
        
        CreateMap<Billing.DeferredPayment, Billing.DeferredPaymentDto>();
        CreateMap<Billing.CreateDeferredPaymentDto, Billing.DeferredPayment>();

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

        // Services & Radiology
        CreateMap<Services.ServiceItem, Services.ServiceItemDto>();
        CreateMap<Services.CreateUpdateServiceItemDto, Services.ServiceItem>();
        
        CreateMap<Services.RadiologyItem, Services.RadiologyItemDto>();
        CreateMap<Services.CreateUpdateRadiologyItemDto, Services.RadiologyItem>();

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

        // Inpatient (Admissions)
        CreateMap<Inpatient.Admission, Inpatient.AdmissionDto>()
            .ForMember(dest => dest.PatientName, opt => opt.Ignore())
            .ForMember(dest => dest.PatientFileNumber, opt => opt.Ignore())
            .ForMember(dest => dest.RoomNumber, opt => opt.Ignore())
            .ForMember(dest => dest.RoomTypeName, opt => opt.Ignore());
        CreateMap<Inpatient.CreateUpdateAdmissionDto, Inpatient.Admission>();

        // Operations (Surgery)
        CreateMap<Operations.SurgicalOperation, Operations.SurgicalOperationDto>()
            .ForMember(dest => dest.PatientName, opt => opt.Ignore())
            .ForMember(dest => dest.DoctorName, opt => opt.Ignore());
        CreateMap<Operations.CreateUpdateSurgicalOperationDto, Operations.SurgicalOperation>();
    }
}
