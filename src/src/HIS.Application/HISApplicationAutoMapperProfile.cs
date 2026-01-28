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
        
        CreateMap<Laboratory, LaboratoryDto>();
        CreateMap<CreateUpdateLaboratoryDto, Laboratory>();

        CreateMap<Appointments.DoctorSchedule, Appointments.DoctorScheduleDto>();
        CreateMap<Appointments.CreateUpdateDoctorScheduleDto, Appointments.DoctorSchedule>();

        CreateMap<Financials.Account, Financials.Dtos.AccountDto>();
        CreateMap<Financials.Dtos.CreateUpdateAccountDto, Financials.Account>();

        // Activity Logs
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
    }
}
