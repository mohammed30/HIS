using HIS.ActivityLogs;
using HIS.Patients;
using HIS.Settings;
using HIS.Appointments;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace HIS.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class HISDbContext :
    AbpDbContext<HISDbContext>,
    ITenantManagementDbContext,
    IIdentityDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */

    // Activity Logs
    public DbSet<ActivityLog> ActivityLogs { get; set; }

    // Patients
    public DbSet<Patient> Patients { get; set; }

    // Settings
    public DbSet<Department> Departments { get; set; }
    public DbSet<Specialty> Specialties { get; set; }
    public DbSet<Clinic> Clinics { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Laboratory> Laboratories { get; set; }

    // Appointments
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<DoctorSchedule> DoctorSchedules { get; set; }
    public DbSet<WaitingList> WaitingLists { get; set; }

    // Insurance
    public DbSet<HIS.Insurance.InsuranceCompany> InsuranceCompanies { get; set; }
    public DbSet<HIS.Insurance.InsurancePlan> InsurancePlans { get; set; }
    public DbSet<HIS.Insurance.PatientInsurance> PatientInsurances { get; set; }

    // Billing
    public DbSet<HIS.Billing.Invoice> Invoices { get; set; }
    public DbSet<HIS.Billing.InvoiceItem> InvoiceItems { get; set; }
    public DbSet<HIS.Billing.Payment> Payments { get; set; }
    public DbSet<HIS.Billing.DeferredPayment> DeferredPayments { get; set; }

    // Medical Records
    public DbSet<HIS.MedicalRecords.MedicalHistory> MedicalHistories { get; set; }
    public DbSet<HIS.MedicalRecords.Diagnosis> Diagnoses { get; set; }
    public DbSet<HIS.MedicalRecords.VitalSign> VitalSigns { get; set; }
    public DbSet<HIS.MedicalRecords.Allergy> Allergies { get; set; }
    public DbSet<HIS.MedicalRecords.PatientNote> PatientNotes { get; set; }

    // Services
    public DbSet<HIS.Services.ServiceItem> ServiceItems { get; set; }
    public DbSet<HIS.Services.RadiologyItem> RadiologyItems { get; set; }

    // Pricing
    public DbSet<HIS.Pricing.PriceList> PriceLists { get; set; }
    public DbSet<HIS.Pricing.ServicePrice> ServicePrices { get; set; }

    // Financial & Inventory
    public DbSet<HIS.Accounting.Account> Accounts { get; set; }
    public DbSet<HIS.Accounting.JournalEntry> JournalEntries { get; set; }
    public DbSet<HIS.Accounting.JournalEntryLine> JournalEntryLines { get; set; }
    
    public DbSet<HIS.Inventory.Warehouse> Warehouses { get; set; }
    public DbSet<HIS.Inventory.Supplier> Suppliers { get; set; }
    public DbSet<HIS.Inventory.InventoryItem> InventoryItems { get; set; }
    public DbSet<HIS.Inventory.InventoryTransaction> InventoryTransactions { get; set; }

    #region Entities from the modules

    // Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }

    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    #endregion

    public HISDbContext(DbContextOptions<HISDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureFeatureManagement();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureTenantManagement();
        builder.ConfigureBlobStoring();
        
        /* Configure your own tables/entities inside here */

        // ActivityLog
        builder.Entity<ActivityLog>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "ActivityLogs", HISConsts.DbSchema);
            b.ConfigureByConvention();
            
            b.Property(x => x.Module).HasMaxLength(128).IsRequired();
            b.Property(x => x.UserName).HasMaxLength(256);
            b.Property(x => x.EntityType).HasMaxLength(256);
            b.Property(x => x.EntityId).HasMaxLength(128);
            b.Property(x => x.Description).HasMaxLength(2048);
            b.Property(x => x.IpAddress).HasMaxLength(64);
            b.Property(x => x.UserAgent).HasMaxLength(512);
            
            b.HasIndex(x => x.UserId);
            b.HasIndex(x => x.Module);
            b.HasIndex(x => x.Action);
            b.HasIndex(x => x.Timestamp);
            b.HasIndex(x => new { x.EntityType, x.EntityId });
        });

        // Patient
        builder.Entity<Patient>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "Patients", HISConsts.DbSchema);
            b.ConfigureByConvention();
            
            b.Property(x => x.MRN).HasMaxLength(32).IsRequired();
            b.Property(x => x.FirstNameAr).HasMaxLength(128).IsRequired();
            b.Property(x => x.MiddleNameAr).HasMaxLength(128);
            b.Property(x => x.LastNameAr).HasMaxLength(128).IsRequired();
            b.Property(x => x.FirstNameEn).HasMaxLength(128);
            b.Property(x => x.MiddleNameEn).HasMaxLength(128);
            b.Property(x => x.LastNameEn).HasMaxLength(128);
            b.Property(x => x.IdentityNumber).HasMaxLength(32).IsRequired();
            b.Property(x => x.Nationality).HasMaxLength(64);
            b.Property(x => x.MobileNumber).HasMaxLength(20).IsRequired();
            b.Property(x => x.PhoneNumber).HasMaxLength(20);
            b.Property(x => x.Email).HasMaxLength(256);
            b.Property(x => x.Address).HasMaxLength(512);
            b.Property(x => x.City).HasMaxLength(128);
            b.Property(x => x.EmergencyContactName).HasMaxLength(128);
            b.Property(x => x.EmergencyContactRelation).HasMaxLength(64);
            b.Property(x => x.EmergencyContactPhone).HasMaxLength(20);
            b.Property(x => x.BloodType).HasMaxLength(8);
            b.Property(x => x.Allergies).HasMaxLength(1024);
            b.Property(x => x.Notes).HasMaxLength(2048);
            b.Property(x => x.PhotoUrl).HasMaxLength(512);
            
            b.HasIndex(x => x.MRN).IsUnique();
            b.HasIndex(x => x.IdentityNumber);
            b.HasIndex(x => x.MobileNumber);
            b.HasIndex(x => x.FirstNameAr);
            b.HasIndex(x => x.LastNameAr);
        });

        // Department
        builder.Entity<Department>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "Departments", HISConsts.DbSchema);
            b.ConfigureByConvention();
            
            b.Property(x => x.Code).HasMaxLength(32).IsRequired();
            b.Property(x => x.NameAr).HasMaxLength(128).IsRequired();
            b.Property(x => x.NameEn).HasMaxLength(128);
            b.Property(x => x.Description).HasMaxLength(512);
            b.Property(x => x.Location).HasMaxLength(256);
            b.Property(x => x.ExtensionNumber).HasMaxLength(16);
            
            b.HasIndex(x => x.Code).IsUnique();
        });

        // Specialty
        builder.Entity<Specialty>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "Specialties", HISConsts.DbSchema);
            b.ConfigureByConvention();
            
            b.Property(x => x.Code).HasMaxLength(32).IsRequired();
            b.Property(x => x.NameAr).HasMaxLength(128).IsRequired();
            b.Property(x => x.NameEn).HasMaxLength(128);
            b.Property(x => x.Description).HasMaxLength(512);
            
            b.HasIndex(x => x.Code).IsUnique();
        });

        // Clinic
        builder.Entity<Clinic>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "Clinics", HISConsts.DbSchema);
            b.ConfigureByConvention();
            
            b.Property(x => x.Code).HasMaxLength(32).IsRequired();
            b.Property(x => x.NameAr).HasMaxLength(128).IsRequired();
            b.Property(x => x.NameEn).HasMaxLength(128);
            b.Property(x => x.Location).HasMaxLength(256);
            b.Property(x => x.RoomNumber).HasMaxLength(32);
            b.Property(x => x.ExtensionNumber).HasMaxLength(16);
            b.Property(x => x.ConsultationFee).HasPrecision(18, 2);
            
            b.HasIndex(x => x.Code).IsUnique();
            b.HasIndex(x => x.DepartmentId);
        });

        // Doctor
        builder.Entity<Doctor>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "Doctors", HISConsts.DbSchema);
            b.ConfigureByConvention();
            
            b.Property(x => x.Code).HasMaxLength(32).IsRequired();
            b.Property(x => x.NameAr).HasMaxLength(128).IsRequired();
            b.Property(x => x.NameEn).HasMaxLength(128);
            b.Property(x => x.LicenseNumber).HasMaxLength(64);
            b.Property(x => x.MobileNumber).HasMaxLength(20);
            b.Property(x => x.Email).HasMaxLength(256);
            b.Property(x => x.Degree).HasMaxLength(128);
            b.Property(x => x.ConsultationFee).HasPrecision(18, 2);
            b.Property(x => x.FollowUpFee).HasPrecision(18, 2);
            b.Property(x => x.PhotoUrl).HasMaxLength(512);
            b.Property(x => x.Bio).HasMaxLength(2048);
            
            b.HasIndex(x => x.Code).IsUnique();
            b.HasIndex(x => x.SpecialtyId);
            b.HasIndex(x => x.DepartmentId);
        });

        // Laboratory
        builder.Entity<Laboratory>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "Laboratories", HISConsts.DbSchema);
            b.ConfigureByConvention();
            
            b.Property(x => x.Code).HasMaxLength(32).IsRequired();
            b.Property(x => x.NameAr).HasMaxLength(128).IsRequired();
            b.Property(x => x.NameEn).HasMaxLength(128);
            b.Property(x => x.Description).HasMaxLength(512);
            b.Property(x => x.Location).HasMaxLength(256);
            b.Property(x => x.ExtensionNumber).HasMaxLength(16);
            b.Property(x => x.WorkingHours).HasMaxLength(128);
            
            b.HasIndex(x => x.Code).IsUnique();
        });

        // Appointment
        builder.Entity<Appointment>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "Appointments", HISConsts.DbSchema);
            b.ConfigureByConvention();
            
            b.Property(x => x.Notes).HasMaxLength(2048);
            
            b.HasIndex(x => x.PatientId);
            b.HasIndex(x => x.DoctorId);
            b.HasIndex(x => x.ClinicId);
            b.HasIndex(x => x.AppointmentDate);
        });

        // DoctorSchedule
        builder.Entity<DoctorSchedule>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "DoctorSchedules", HISConsts.DbSchema);
            b.ConfigureByConvention();
            
            b.HasIndex(x => new { x.DoctorId, x.DayOfWeek }).IsUnique();
        });

        // WaitingList
        builder.Entity<WaitingList>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "WaitingLists", HISConsts.DbSchema);
            b.ConfigureByConvention();
            
            b.HasIndex(x => x.PatientId);
            b.HasIndex(x => x.DepartmentId);
            b.HasIndex(x => x.RequestDate);
        });

        // Insurance Company
        builder.Entity<HIS.Insurance.InsuranceCompany>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "InsuranceCompanies", HISConsts.DbSchema);
            b.ConfigureByConvention();
            
            b.Property(x => x.Code).HasMaxLength(32).IsRequired();
            b.Property(x => x.NameAr).HasMaxLength(256).IsRequired();
            b.Property(x => x.NameEn).HasMaxLength(256);
            b.Property(x => x.Phone).HasMaxLength(32);
            b.Property(x => x.Email).HasMaxLength(256);
            b.Property(x => x.Address).HasMaxLength(512);
            b.Property(x => x.ContactPerson).HasMaxLength(128);
            b.Property(x => x.ContactPhone).HasMaxLength(32);
            b.Property(x => x.Website).HasMaxLength(256);
            
            b.HasIndex(x => x.Code).IsUnique();
        });

        // Insurance Plan
        builder.Entity<HIS.Insurance.InsurancePlan>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "InsurancePlans", HISConsts.DbSchema);
            b.ConfigureByConvention();
            
            b.Property(x => x.Code).HasMaxLength(32).IsRequired();
            b.Property(x => x.NameAr).HasMaxLength(256).IsRequired();
            b.Property(x => x.NameEn).HasMaxLength(256);
            b.Property(x => x.CoveragePercentage).HasPrecision(5, 2);
            b.Property(x => x.MaxCoverageAmount).HasPrecision(18, 2);
            b.Property(x => x.CoPaymentPercentage).HasPrecision(5, 2);
            b.Property(x => x.DeductibleAmount).HasPrecision(18, 2);
            
            b.HasIndex(x => x.Code).IsUnique();
            b.HasIndex(x => x.InsuranceCompanyId);
        });

        // Patient Insurance
        builder.Entity<HIS.Insurance.PatientInsurance>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "PatientInsurances", HISConsts.DbSchema);
            b.ConfigureByConvention();
            
            b.Property(x => x.PolicyNumber).HasMaxLength(64).IsRequired();
            b.Property(x => x.CardNumber).HasMaxLength(64);
            b.Property(x => x.SubscriberName).HasMaxLength(256);
            b.Property(x => x.RelationToSubscriber).HasMaxLength(64);
            b.Property(x => x.EmployerName).HasMaxLength(256);
            
            b.HasIndex(x => x.PatientId);
            b.HasIndex(x => x.InsurancePlanId);
            b.HasIndex(x => x.PolicyNumber);
        });

        // Invoice
        builder.Entity<HIS.Billing.Invoice>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "Invoices", HISConsts.DbSchema);
            b.ConfigureByConvention();
            
            b.Property(x => x.InvoiceNumber).HasMaxLength(32).IsRequired();
            b.Property(x => x.TotalAmount).HasPrecision(18, 2);
            b.Property(x => x.DiscountAmount).HasPrecision(18, 2);
            b.Property(x => x.TaxPercentage).HasPrecision(5, 2);
            b.Property(x => x.TaxAmount).HasPrecision(18, 2);
            b.Property(x => x.NetAmount).HasPrecision(18, 2);
            b.Property(x => x.PaidAmount).HasPrecision(18, 2);
            b.Property(x => x.InsuranceCoverage).HasPrecision(18, 2);
            b.Property(x => x.CoPaymentAmount).HasPrecision(18, 2);
            b.Ignore(x => x.DueAmount);
            
            b.HasIndex(x => x.InvoiceNumber).IsUnique();
            b.HasIndex(x => x.PatientId);
            b.HasIndex(x => x.InvoiceDate);
        });

        // Invoice Item
        builder.Entity<HIS.Billing.InvoiceItem>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "InvoiceItems", HISConsts.DbSchema);
            b.ConfigureByConvention();
            
            b.Property(x => x.ServiceCode).HasMaxLength(32);
            b.Property(x => x.Description).HasMaxLength(512).IsRequired();
            b.Property(x => x.Quantity).HasPrecision(10, 2);
            b.Property(x => x.UnitPrice).HasPrecision(18, 2);
            b.Property(x => x.DiscountPercentage).HasPrecision(5, 2);
            b.Property(x => x.DiscountAmount).HasPrecision(18, 2);
            b.Ignore(x => x.TotalPrice);
            
            b.HasIndex(x => x.InvoiceId);
        });

        // Payment
        builder.Entity<HIS.Billing.Payment>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "Payments", HISConsts.DbSchema);
            b.ConfigureByConvention();
            
            b.Property(x => x.PaymentNumber).HasMaxLength(32).IsRequired();
            b.Property(x => x.Amount).HasPrecision(18, 2);
            b.Property(x => x.ReferenceNumber).HasMaxLength(64);
            b.Property(x => x.ReceivedBy).HasMaxLength(128);
            
            b.HasIndex(x => x.PaymentNumber).IsUnique();
            b.HasIndex(x => x.PatientId);
            b.HasIndex(x => x.InvoiceId);
            b.HasIndex(x => x.PaymentDate);
        });

        // Deferred Payment
        builder.Entity<HIS.Billing.DeferredPayment>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "DeferredPayments", HISConsts.DbSchema);
            b.ConfigureByConvention();
            
            b.Property(x => x.DeferredNumber).HasMaxLength(32).IsRequired();
            b.Property(x => x.TotalAmount).HasPrecision(18, 2);
            b.Property(x => x.PaidAmount).HasPrecision(18, 2);
            b.Property(x => x.InstallmentAmount).HasPrecision(18, 2);
            b.Property(x => x.Reason).HasMaxLength(512);
            b.Property(x => x.ContactPhone).HasMaxLength(32);
            b.Ignore(x => x.RemainingAmount);
            
            b.HasIndex(x => x.DeferredNumber).IsUnique();
            b.HasIndex(x => x.PatientId);
            b.HasIndex(x => x.DueDate);
        });

        // Medical History
        builder.Entity<HIS.MedicalRecords.MedicalHistory>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "MedicalHistories", HISConsts.DbSchema);
            b.ConfigureByConvention();
            
            b.Property(x => x.ConditionAr).HasMaxLength(256).IsRequired();
            b.Property(x => x.ConditionEn).HasMaxLength(256);
            b.Property(x => x.ICD10Code).HasMaxLength(16);
            b.Property(x => x.Notes).HasMaxLength(1024);
            
            b.HasIndex(x => x.PatientId);
        });

        // Diagnosis
        builder.Entity<HIS.MedicalRecords.Diagnosis>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "Diagnoses", HISConsts.DbSchema);
            b.ConfigureByConvention();
            
            b.Property(x => x.ICD10Code).HasMaxLength(16);
            b.Property(x => x.DiagnosisNameAr).HasMaxLength(256).IsRequired();
            b.Property(x => x.DiagnosisNameEn).HasMaxLength(256);
            b.Property(x => x.DiagnosedByName).HasMaxLength(128);
            b.Property(x => x.Notes).HasMaxLength(1024);
            
            b.HasIndex(x => x.PatientId);
            b.HasIndex(x => x.VisitId);
            b.HasIndex(x => x.DiagnosisDate);
        });

        // Vital Sign
        builder.Entity<HIS.MedicalRecords.VitalSign>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "VitalSigns", HISConsts.DbSchema);
            b.ConfigureByConvention();
            
            b.Property(x => x.Temperature).HasPrecision(4, 1);
            b.Property(x => x.OxygenSaturation).HasPrecision(5, 2);
            b.Property(x => x.Weight).HasPrecision(5, 2);
            b.Property(x => x.Height).HasPrecision(5, 2);
            b.Property(x => x.RecordedByName).HasMaxLength(128);
            b.Property(x => x.Notes).HasMaxLength(512);
            b.Ignore(x => x.BMI);
            
            b.HasIndex(x => x.PatientId);
            b.HasIndex(x => x.RecordedAt);
        });

        // Allergy
        builder.Entity<HIS.MedicalRecords.Allergy>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "Allergies", HISConsts.DbSchema);
            b.ConfigureByConvention();
            
            b.Property(x => x.AllergenNameAr).HasMaxLength(256).IsRequired();
            b.Property(x => x.AllergenNameEn).HasMaxLength(256);
            b.Property(x => x.Reaction).HasMaxLength(512);
            b.Property(x => x.Notes).HasMaxLength(512);
            
            b.HasIndex(x => x.PatientId);
        });

        // Patient Note
        builder.Entity<HIS.MedicalRecords.PatientNote>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "PatientNotes", HISConsts.DbSchema);
            b.ConfigureByConvention();
            
            b.Property(x => x.Title).HasMaxLength(256).IsRequired();
            b.Property(x => x.Content).IsRequired();
            b.Property(x => x.CreatedByName).HasMaxLength(128);
            
            b.HasIndex(x => x.PatientId);
            b.HasIndex(x => x.VisitId);
        });

        // Services
        builder.Entity<HIS.Services.ServiceItem>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "ServiceItems", HISConsts.DbSchema);
            b.ConfigureByConvention();
            
            b.Property(x => x.Code).HasMaxLength(64).IsRequired();
            b.Property(x => x.Name).HasMaxLength(256).IsRequired();
            
            b.HasIndex(x => x.Code).IsUnique();
        });

        // Radiology Item (TPH Inheritance)
        builder.Entity<HIS.Services.RadiologyItem>(b =>
        {
            b.HasBaseType<HIS.Services.ServiceItem>();
            b.Property(x => x.Modality).HasMaxLength(64);
            b.Property(x => x.BodyPart).HasMaxLength(128);
            b.Property(x => x.Instructions).HasMaxLength(1024);
        });

        // Price List
        builder.Entity<HIS.Pricing.PriceList>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "PriceLists", HISConsts.DbSchema);
            b.ConfigureByConvention();
            
            b.Property(x => x.Name).HasMaxLength(128).IsRequired();
        });

        // Service Price
        builder.Entity<HIS.Pricing.ServicePrice>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "ServicePrices", HISConsts.DbSchema);
            b.ConfigureByConvention();
            
            b.Property(x => x.Amount).HasPrecision(18, 2);
            b.Property(x => x.CoPayAmount).HasPrecision(18, 2);
            
            b.HasIndex(x => x.PriceListId);
            b.HasIndex(x => new { x.PriceListId, x.ServiceItemId }).IsUnique();
        });

        // --- Financial & Inventory ---

        builder.Entity<HIS.Accounting.Account>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "Accounts", HISConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Code).HasMaxLength(32).IsRequired();
            b.Property(x => x.Name).HasMaxLength(128).IsRequired();
            b.HasIndex(x => x.Code).IsUnique();
        });

        builder.Entity<HIS.Accounting.JournalEntry>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "JournalEntries", HISConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.ReferenceNumber).HasMaxLength(64);
            b.Property(x => x.Description).HasMaxLength(512);
        });

        builder.Entity<HIS.Accounting.JournalEntryLine>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "JournalEntryLines", HISConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Debit).HasPrecision(18, 2);
            b.Property(x => x.Credit).HasPrecision(18, 2);
            b.HasIndex(x => x.JournalEntryId);
            b.HasIndex(x => x.AccountId);
        });

        builder.Entity<HIS.Inventory.Warehouse>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "Warehouses", HISConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Name).HasMaxLength(128).IsRequired();
            b.Property(x => x.Location).HasMaxLength(256);
        });

        builder.Entity<HIS.Inventory.Supplier>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "Suppliers", HISConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Name).HasMaxLength(128).IsRequired();
            b.Property(x => x.ContactPerson).HasMaxLength(128);
            b.Property(x => x.Phone).HasMaxLength(32);
            b.Property(x => x.Email).HasMaxLength(128);
        });

        builder.Entity<HIS.Inventory.InventoryItem>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "InventoryItems", HISConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Quantity).HasPrecision(18, 4);
            b.Property(x => x.AverageCost).HasPrecision(18, 4);
            b.HasIndex(x => new { x.WarehouseId, x.ProductId }).IsUnique();
        });

        builder.Entity<HIS.Inventory.InventoryTransaction>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "InventoryTransactions", HISConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Quantity).HasPrecision(18, 4);
            b.Property(x => x.UnitCost).HasPrecision(18, 4);
            b.Property(x => x.ReferenceNumber).HasMaxLength(64);
            b.HasIndex(x => x.InventoryItemId);
            b.HasIndex(x => x.TransactionDate);
        });
    }
}


