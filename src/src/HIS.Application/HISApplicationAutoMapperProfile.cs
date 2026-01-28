using AutoMapper;
using HIS.Settings;

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
        
        CreateMap<Laboratory, LaboratoryDto>();
        CreateMap<CreateUpdateLaboratoryDto, Laboratory>();

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
    }
}
