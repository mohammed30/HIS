using HIS.General;
using HIS.Nursing;
using HIS.HR;
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
    public DbSet<HIS.Settings.Laboratory> Laboratories { get; set; }
    public DbSet<JobTitle> JobTitles { get; set; }

    // General Master Data (Definitions)
    public DbSet<Nationality> Nationalities { get; set; }
    public DbSet<Profession> Professions { get; set; }
    public DbSet<Contract> Contracts { get; set; }
    public DbSet<PatientCategory> PatientCategories { get; set; }
    public DbSet<ReferralSource> ReferralSources { get; set; }
    public DbSet<PaymentMethod> PaymentMethods { get; set; }

    // Appointments
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<DoctorSchedule> DoctorSchedules { get; set; }
    public DbSet<WaitingList> WaitingLists { get; set; }

    // Laboratory Module (New)
    public DbSet<HIS.Laboratory.LabTestCategory> LabTestCategories { get; set; }
    public DbSet<HIS.Laboratory.LabTest> LabTests { get; set; }
    public DbSet<HIS.Laboratory.LabRequest> LabRequests { get; set; }
    public DbSet<HIS.Laboratory.LabAppointment> LabAppointments { get; set; }

    // Emergency Module (New)
    public DbSet<HIS.Emergency.EmergencyVisit> EmergencyVisits { get; set; }

    // Insurance
    public DbSet<HIS.Insurance.InsuranceCompany> InsuranceCompanies { get; set; }
    public DbSet<HIS.Insurance.InsurancePlan> InsurancePlans { get; set; }
    public DbSet<HIS.Insurance.PatientInsurance> PatientInsurances { get; set; }

    // Billing
    public DbSet<HIS.Billing.Invoice> Invoices { get; set; }
    public DbSet<HIS.Billing.InvoiceItem> InvoiceItems { get; set; }
    public DbSet<HIS.Billing.Payment> Payments { get; set; }
    public DbSet<HIS.Billing.DeferredPayment> DeferredPayments { get; set; }
    public DbSet<HIS.Billing.InpatientDeposit> InpatientDeposits { get; set; }

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

    public DbSet<HIS.Accounting.PaymentVoucher> PaymentVouchers { get; set; }
    public DbSet<HIS.Accounting.PaymentVoucherLine> PaymentVoucherLines { get; set; }
    public DbSet<HIS.Accounting.ReceiptVoucher> ReceiptVouchers { get; set; }
    public DbSet<HIS.Accounting.ReceiptVoucherLine> ReceiptVoucherLines { get; set; }
    public DbSet<HIS.Accounting.ContractClaim> ContractClaims { get; set; }
    public DbSet<HIS.Accounting.BankTransaction> BankTransactions { get; set; }
    
    public DbSet<HIS.Inventory.Warehouse> Warehouses { get; set; }
    public DbSet<HIS.Inventory.Supplier> Suppliers { get; set; }
    public DbSet<HIS.Inventory.InventoryItem> InventoryItems { get; set; }
    public DbSet<HIS.Inventory.InventoryTransaction> InventoryTransactions { get; set; }
    public DbSet<HIS.Inventory.InventoryBatch> InventoryBatches { get; set; }
    
    public DbSet<HIS.Inventory.PurchaseOrder> PurchaseOrders { get; set; }
    public DbSet<HIS.Inventory.PurchaseOrderLine> PurchaseOrderLines { get; set; }
    
    public DbSet<HIS.Inventory.InternalRequest> InternalRequests { get; set; }
    public DbSet<HIS.Inventory.InternalRequestLine> InternalRequestLines { get; set; }

    public DbSet<HIS.Inventory.PurchaseInvoice> PurchaseInvoices { get; set; }
    public DbSet<HIS.Inventory.PurchaseInvoiceLine> PurchaseInvoiceLines { get; set; }

    // Clinical
    public DbSet<HIS.Clinical.MedicalOrder> MedicalOrders { get; set; }
    public DbSet<HIS.Pharmacy.Dispensing> Dispensings { get; set; }
    
    // Pharmacy (Master Data)
    public DbSet<HIS.Pharmacy.Drug> Drugs { get; set; }
    
    // Phase 3: Stock & POS
    public DbSet<HIS.Pharmacy.StockTransfer> StockTransfers { get; set; }
    public DbSet<HIS.Pharmacy.DispensingVerification> DispensingVerifications { get; set; }

    // Rooms & Inpatient
    public DbSet<HIS.Rooms.Room> Rooms { get; set; }
    public DbSet<HIS.Inpatient.Admission> Admissions { get; set; }
    public DbSet<HIS.Rooms.Bed> Beds { get; set; }
    public DbSet<HIS.Inpatient.Reservation> Reservations { get; set; }
    public DbSet<HIS.Inpatient.PatientTransfer> PatientTransfers { get; set; }

    // Operations (Surgery)
    public DbSet<HIS.Operations.SurgicalOperation> SurgicalOperations { get; set; }

    // Insurance
    public DbSet<HIS.Insurance.InsuranceServicePrice> InsuranceServicePrices { get; set; }
    
    // Nursing Services
    public DbSet<MedicationAdministration> MedicationAdministrations { get; set; }
    public DbSet<CarePlan> CarePlans { get; set; }
    
    // Phase 2
    public DbSet<PatientRound> PatientRounds { get; set; }
    public DbSet<PainAssessment> PainAssessments { get; set; }
    public DbSet<FallRiskAssessment> FallRiskAssessments { get; set; }
    public DbSet<WoundCare> WoundCares { get; set; }
    public DbSet<FluidBalance> FluidBalances { get; set; }
    public DbSet<ShiftHandover> ShiftHandovers { get; set; }

    // HR (شؤون العاملين)
    public DbSet<Employee> Employees { get; set; }
    public DbSet<JobGrade> JobGrades { get; set; }
    public DbSet<CompensationItem> CompensationItems { get; set; }
    public DbSet<LeaveType> LeaveTypes { get; set; }
    public DbSet<EmployeeLeave> EmployeeLeaves { get; set; }
    public DbSet<EmployeeLoan> EmployeeLoans { get; set; }
    public DbSet<SalarySetup> SalarySetups { get; set; }
    public DbSet<PayrollRun> PayrollRuns { get; set; }
    public DbSet<PayrollLine> PayrollLines { get; set; }
    public DbSet<Penalty> Penalties { get; set; }
    public DbSet<AttendanceRecord> AttendanceRecords { get; set; }
    public DbSet<DailyAttendance> DailyAttendances { get; set; }

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
            b.Property(x => x.IdentityIssuePlace).HasMaxLength(128);
            
            b.Property(x => x.PassportNumber).HasMaxLength(32);
            b.Property(x => x.PassportIssuePlace).HasMaxLength(128);
            b.Property(x => x.VisaNumber).HasMaxLength(32);
            b.Property(x => x.VisaIssuePlace).HasMaxLength(128);
            
            b.Property(x => x.MobileNumber).HasMaxLength(20).IsRequired();
            b.Property(x => x.PhoneNumber).HasMaxLength(20);
            b.Property(x => x.Email).HasMaxLength(256);
            b.Property(x => x.Address).HasMaxLength(512);
            b.Property(x => x.City).HasMaxLength(128);
            
            b.Property(x => x.SponsorName).HasMaxLength(256);
            b.Property(x => x.SponsorId).HasMaxLength(32);
            
            b.Property(x => x.EmergencyContactName).HasMaxLength(128);
            b.Property(x => x.EmergencyContactRelation).HasMaxLength(64);
            b.Property(x => x.EmergencyContactPhone).HasMaxLength(20);
            b.Property(x => x.BloodType).HasMaxLength(8);
            b.Property(x => x.Allergies).HasMaxLength(1024);
            b.Property(x => x.Notes).HasMaxLength(2048);
            b.Property(x => x.PhotoUrl).HasMaxLength(512);
            b.Property(x => x.CardNumber).HasMaxLength(64);
            b.Property(x => x.TaxFile).HasMaxLength(64);
            b.Property(x => x.IsSocialSecurity).IsRequired();
            
            b.HasIndex(x => x.MRN).IsUnique();
            b.HasIndex(x => x.IdentityNumber);
            b.HasIndex(x => x.MobileNumber);
            b.HasIndex(x => x.FirstNameAr);
            b.HasIndex(x => x.LastNameAr);

            // Relationships
            b.HasOne<Nationality>().WithMany().HasForeignKey(x => x.NationalityId);
            b.HasOne<Profession>().WithMany().HasForeignKey(x => x.ProfessionId);
            b.HasOne<Contract>().WithMany().HasForeignKey(x => x.ContractId);
            b.HasOne<PaymentMethod>().WithMany().HasForeignKey(x => x.PaymentMethodId);
            b.HasOne<ReferralSource>().WithMany().HasForeignKey(x => x.ReferralSourceId);
        });

        // Nationality
        builder.Entity<Nationality>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "Nationalities", HISConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.NameAr).HasMaxLength(128).IsRequired();
            b.Property(x => x.NameEn).HasMaxLength(128).IsRequired();
            b.Property(x => x.Code).HasMaxLength(32);
        });

        // Profession
        builder.Entity<Profession>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "Professions", HISConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.NameAr).HasMaxLength(128).IsRequired();
            b.Property(x => x.NameEn).HasMaxLength(128).IsRequired();
            b.Property(x => x.Code).HasMaxLength(32);
        });

        // Contract
        builder.Entity<Contract>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "Contracts", HISConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.NameAr).HasMaxLength(128).IsRequired();
            b.Property(x => x.NameEn).HasMaxLength(128).IsRequired();
            b.Property(x => x.Code).HasMaxLength(32);
        });

        // PatientCategory
        builder.Entity<PatientCategory>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "PatientCategories", HISConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.NameAr).HasMaxLength(128).IsRequired();
            b.Property(x => x.NameEn).HasMaxLength(128).IsRequired();
            b.Property(x => x.Code).HasMaxLength(32);
        });

        // ReferralSource
        builder.Entity<ReferralSource>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "ReferralSources", HISConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.NameAr).HasMaxLength(128).IsRequired();
            b.Property(x => x.NameEn).HasMaxLength(128).IsRequired();
            b.Property(x => x.Code).HasMaxLength(32);
        });

        // PaymentMethod
        builder.Entity<PaymentMethod>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "PaymentMethods", HISConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.NameAr).HasMaxLength(128).IsRequired();
            b.Property(x => x.NameEn).HasMaxLength(128).IsRequired();
            b.Property(x => x.Code).HasMaxLength(32);
            b.Property(x => x.IsDefault).HasDefaultValue(false);
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
            b.Property(x => x.MorningConsultationFee).HasPrecision(18, 2);
            b.Property(x => x.EveningConsultationFee).HasPrecision(18, 2);
            b.Property(x => x.FollowUpFee).HasPrecision(18, 2);
            b.Property(x => x.PhotoUrl).HasMaxLength(512);
            b.Property(x => x.Bio).HasMaxLength(2048);
            
            b.HasIndex(x => x.Code).IsUnique();
            b.HasIndex(x => x.SpecialtyId);
            b.HasIndex(x => x.DepartmentId);
        });

        // Laboratory (Settings)
        builder.Entity<HIS.Settings.Laboratory>(b =>
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

        // --- Laboratory Module ---

        builder.Entity<HIS.Laboratory.LabTestCategory>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "LabTestCategories", HISConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Code).HasMaxLength(32).IsRequired();
            b.Property(x => x.Name).HasMaxLength(128).IsRequired();
            b.HasIndex(x => x.Code).IsUnique();
            b.HasIndex(x => x.ParentId);
        });

        builder.Entity<HIS.Laboratory.LabTest>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "LabTests", HISConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Code).HasMaxLength(32).IsRequired();
            b.Property(x => x.Name).HasMaxLength(128).IsRequired();
            b.Property(x => x.Price).HasPrecision(18, 2);
            b.Property(x => x.Instructions).HasMaxLength(1024);
            b.HasIndex(x => x.Code).IsUnique();
            b.HasIndex(x => x.CategoryId);
        });

        builder.Entity<HIS.Laboratory.LabRequest>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "LabRequests", HISConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Result).HasMaxLength(2048);
            b.Property(x => x.Notes).HasMaxLength(1024);
            
            b.HasIndex(x => x.PatientId);
            b.HasIndex(x => x.RequestDate);
            b.HasIndex(x => x.Status);
        });

        builder.Entity<HIS.Laboratory.LabAppointment>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "LabAppointments", HISConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Notes).HasMaxLength(1024);
            b.Property(x => x.PreparationInstructions).HasMaxLength(2048);
            
            b.HasIndex(x => x.PatientId);
            b.HasIndex(x => x.AppointmentDate);
            b.HasIndex(x => x.Status);
            b.HasIndex(x => x.ServiceItemId);
        });

        // --- Emergency Module ---

        builder.Entity<HIS.Emergency.EmergencyVisit>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "EmergencyVisits", HISConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.ChiefComplaint).HasMaxLength(1024).IsRequired();
            b.Property(x => x.BloodPressure).HasMaxLength(16);
            b.Property(x => x.Temperature).HasPrecision(4, 1);
            b.Property(x => x.Notes).HasMaxLength(2048);

            b.HasIndex(x => x.PatientId);
            b.HasIndex(x => x.ArrivalTime);
            b.HasIndex(x => x.Severity);
            b.HasIndex(x => x.Status);
        });

        // Appointment
        builder.Entity<Appointment>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "Appointments", HISConsts.DbSchema);
            b.ConfigureByConvention();
            
            b.Property(x => x.ConsultationFee).HasPrecision(18, 2);
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
            
            b.Property(x => x.ServiceCode).HasMaxLength(64);
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

        // Inpatient Deposit
        builder.Entity<HIS.Billing.InpatientDeposit>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "InpatientDeposits", HISConsts.DbSchema);
            b.ConfigureByConvention();
            
            b.Property(x => x.ReceiptNumber).HasMaxLength(32).IsRequired();
            b.Property(x => x.Amount).HasPrecision(18, 2);
            b.Property(x => x.ReferenceNumber).HasMaxLength(64);
            b.Property(x => x.ReceivedBy).HasMaxLength(128);
            b.Property(x => x.Notes).HasMaxLength(1024);
            
            b.HasIndex(x => x.ReceiptNumber).IsUnique();
            b.HasIndex(x => x.AdmissionId);
            b.HasIndex(x => x.PatientId);
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
            b.Property(x => x.Price).HasPrecision(18, 2);
            
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
            b.Property(x => x.NameAr).HasMaxLength(128);
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

        builder.Entity<HIS.Accounting.PaymentVoucher>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "PaymentVouchers", HISConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Amount).HasPrecision(18, 2);
            b.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.PaymentVoucherId).IsRequired();
        });

        builder.Entity<HIS.Accounting.PaymentVoucherLine>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "PaymentVoucherLines", HISConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Amount).HasPrecision(18, 2);
        });

        builder.Entity<HIS.Accounting.ReceiptVoucher>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "ReceiptVouchers", HISConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Amount).HasPrecision(18, 2);
            b.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.ReceiptVoucherId).IsRequired();
        });

        builder.Entity<HIS.Accounting.ReceiptVoucherLine>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "ReceiptVoucherLines", HISConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Amount).HasPrecision(18, 2);
        });

        builder.Entity<HIS.Accounting.ContractClaim>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "ContractClaims", HISConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Amount).HasPrecision(18, 2);
        });

        builder.Entity<HIS.Accounting.BankTransaction>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "BankTransactions", HISConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Amount).HasPrecision(18, 2);
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
            b.Property(x => x.TaxId).HasMaxLength(50);
        });

        builder.Entity<HIS.Settings.JobTitle>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "JobTitles", HISConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.NameAr).HasMaxLength(128).IsRequired();
            b.Property(x => x.NameEn).HasMaxLength(128);
            b.Property(x => x.Description).HasMaxLength(512);
            b.HasIndex(x => x.DepartmentId);
        });

        builder.Entity<HIS.Inventory.InventoryItem>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "InventoryItems", HISConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Quantity).HasPrecision(18, 4);
            b.Property(x => x.AverageCost).HasPrecision(18, 4);
            b.Property(x => x.MinStockLevel).HasPrecision(18, 4);
            b.Property(x => x.ReorderLevel).HasPrecision(18, 4);
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

        builder.Entity<HIS.Inventory.InventoryBatch>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "InventoryBatches", HISConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Quantity).HasPrecision(18, 4);
            b.Property(x => x.OriginalQuantity).HasPrecision(18, 4);
            b.Property(x => x.UnitCost).HasPrecision(18, 4);
            b.Property(x => x.BatchNumber).HasMaxLength(64);
            b.Property(x => x.ReferenceNumber).HasMaxLength(64);
            b.HasIndex(x => x.InventoryItemId);
            b.HasIndex(x => x.ReceivedDate);
        });

        builder.Entity<HIS.Inventory.PurchaseOrder>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "PurchaseOrders", HISConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.OrderNumber).HasMaxLength(32).IsRequired();
            b.Property(x => x.ReferenceNumber).HasMaxLength(64);
            b.Property(x => x.Notes).HasMaxLength(2048);
            b.Property(x => x.TotalAmount).HasPrecision(18, 2);
            
            b.HasIndex(x => x.OrderNumber).IsUnique();
            b.HasIndex(x => x.SupplierId);
        });

        builder.Entity<HIS.Inventory.PurchaseOrderLine>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "PurchaseOrderLines", HISConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Quantity).HasPrecision(18, 4);
            b.Property(x => x.UnitPrice).HasPrecision(18, 2);
            b.Property(x => x.Discount).HasPrecision(18, 2);
            b.Property(x => x.TotalAmount).HasPrecision(18, 2);
            b.Property(x => x.Description).HasMaxLength(256);
            
            b.HasIndex(x => x.PurchaseOrderId);
            b.HasIndex(x => x.ProductId);
            
            b.HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId).IsRequired(false);
        });

        builder.Entity<HIS.Inventory.InternalRequest>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "InternalRequests", HISConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.RequestNumber).HasMaxLength(32).IsRequired();
            b.Property(x => x.Notes).HasMaxLength(2048);
            
            b.HasIndex(x => x.RequestNumber).IsUnique();
            b.HasIndex(x => x.RequestingDepartmentId);
            b.HasIndex(x => x.FulfilledByWarehouseId);
            b.HasIndex(x => x.Status);
            b.HasIndex(x => x.RequestDate);
        });

        builder.Entity<HIS.Inventory.InternalRequestLine>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "InternalRequestLines", HISConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.RequestedQuantity).HasPrecision(18, 4);
            b.Property(x => x.ApprovedQuantity).HasPrecision(18, 4);
            b.Property(x => x.Notes).HasMaxLength(512);

            b.HasIndex(x => x.InternalRequestId);
            b.HasIndex(x => x.InventoryItemId);
        });

        builder.Entity<HIS.Inventory.PurchaseInvoice>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "PurchaseInvoices", HISConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.InvoiceNumber).HasMaxLength(32).IsRequired();
            b.Property(x => x.Notes).HasMaxLength(2048);
            b.Property(x => x.TotalAmount).HasPrecision(18, 4);
            b.Property(x => x.TaxAmount).HasPrecision(18, 4);
            b.Property(x => x.DiscountAmount).HasPrecision(18, 4);
            b.Property(x => x.NetAmount).HasPrecision(18, 4);

            b.HasIndex(x => x.InvoiceNumber);
            b.HasIndex(x => x.SupplierId);
            b.HasIndex(x => x.Status);
        });

        builder.Entity<HIS.Inventory.PurchaseInvoiceLine>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "PurchaseInvoiceLines", HISConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Quantity).HasPrecision(18, 4);
            b.Property(x => x.UnitCost).HasPrecision(18, 4);
            b.Property(x => x.Discount).HasPrecision(18, 4);
            b.Property(x => x.TotalLineAmount).HasPrecision(18, 4);
            b.Property(x => x.BatchNumber).HasMaxLength(64);

            b.HasIndex(x => x.PurchaseInvoiceId);
            b.HasIndex(x => x.ProductId);
        });

        // Medical Order
        builder.Entity<HIS.Clinical.MedicalOrder>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "MedicalOrders", HISConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Details).HasMaxLength(1024);
            b.Property(x => x.ClinicalNotes).HasMaxLength(2048);
            b.Property(x => x.ServiceName).HasMaxLength(256).IsRequired();
            b.Property(x => x.Price).HasPrecision(18, 2);
            b.Property(x => x.Quantity).HasPrecision(18, 2);
            
            b.HasIndex(x => x.PatientId);
            b.HasIndex(x => x.ServiceItemId);
            b.HasIndex(x => x.Status);
        });

        // Pharmacy Dispensing
        builder.Entity<HIS.Pharmacy.Dispensing>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "Dispensings", HISConsts.DbSchema);
            b.ConfigureByConvention();
            
            b.HasIndex(x => x.MedicalOrderId);
            b.HasIndex(x => x.PatientId);

            b.OwnsMany(x => x.Items, a =>
            {
                a.ToTable(HISConsts.DbTablePrefix + "DispensedItems", HISConsts.DbSchema);
                a.WithOwner().HasForeignKey(x => x.DispensingId);
                a.Property(x => x.Quantity).HasPrecision(18, 2);
                a.Property(x => x.UnitCost).HasPrecision(18, 2);
                a.Property(x => x.BatchNumber).HasMaxLength(64);
                a.Property(x => x.BatchNumber).HasMaxLength(64);
            });
        });

        // Pharmacy Drug
        builder.Entity<HIS.Pharmacy.Drug>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "Drugs", HISConsts.DbSchema);
            b.ConfigureByConvention();
            
            b.Property(x => x.Barcode).HasMaxLength(64).IsRequired();
            b.Property(x => x.BrandName).HasMaxLength(128).IsRequired();
            b.Property(x => x.ScientificName).HasMaxLength(128);
            b.Property(x => x.Strength).HasMaxLength(64);
            b.Property(x => x.Form).HasMaxLength(64);
            b.Property(x => x.Manufacturer).HasMaxLength(128);
            b.Property(x => x.BatchNumberPrefix).HasMaxLength(32);
            
            b.HasIndex(x => x.Barcode).IsUnique();
        });

        // Rooms
        builder.Entity<HIS.Rooms.Room>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "Rooms", HISConsts.DbSchema);
            b.ConfigureByConvention();
            
            b.Property(x => x.RoomNumber).HasMaxLength(32).IsRequired();
            b.Property(x => x.Name).HasMaxLength(128);
            b.Property(x => x.Floor).HasMaxLength(32);
            b.Property(x => x.DailyRate).HasColumnType("decimal(18,2)");
            b.Property(x => x.Notes).HasMaxLength(500);
            
            b.HasIndex(x => x.RoomNumber).IsUnique();
        });

        // Admissions (Inpatient)
        builder.Entity<HIS.Inpatient.Admission>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "Admissions", HISConsts.DbSchema);
            b.ConfigureByConvention();
            
            b.Property(x => x.CompanionName).HasMaxLength(128);
            b.Property(x => x.CompanionPhone).HasMaxLength(32);
            b.Property(x => x.CompanionAddress).HasMaxLength(256);
            b.Property(x => x.Purpose).HasMaxLength(256);
            b.Property(x => x.Notes).HasMaxLength(500);
            b.Property(x => x.InsuranceCeiling).HasColumnType("decimal(18,2)");
            b.Property(x => x.TotalAmount).HasColumnType("decimal(18,2)");
            b.Property(x => x.PaidAmount).HasColumnType("decimal(18,2)");
            b.Property(x => x.InsuranceAmount).HasColumnType("decimal(18,2)");
            b.Property(x => x.PharmacyPercentage).HasColumnType("decimal(5,2)");
            b.Property(x => x.AccumulatedRoomCharges).HasColumnType("decimal(18,2)");
            
            b.Ignore(x => x.DueAmount);
            
            b.HasIndex(x => x.PatientId);
            b.HasIndex(x => x.RoomId);
            b.HasIndex(x => x.Status);
        });

        // Beds
        builder.Entity<HIS.Rooms.Bed>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "Beds", HISConsts.DbSchema);
            b.ConfigureByConvention();
            
            b.Property(x => x.BedNumber).HasMaxLength(32).IsRequired();
            
            b.HasIndex(x => x.RoomId);
            b.HasIndex(x => x.Status);
            b.HasIndex(x => new { x.RoomId, x.BedNumber }).IsUnique();
        });

        // Reservations
        builder.Entity<HIS.Inpatient.Reservation>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "Reservations", HISConsts.DbSchema);
            b.ConfigureByConvention();
            
            b.Property(x => x.Notes).HasMaxLength(1024);
            
            b.HasIndex(x => x.PatientId);
            b.HasIndex(x => x.RoomId);
            b.HasIndex(x => x.BedId);
            b.HasIndex(x => x.StartDate);
            b.HasIndex(x => x.Status);
        });
        // Patient Transfer
        builder.Entity<HIS.Inpatient.PatientTransfer>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "PatientTransfers", HISConsts.DbSchema);
            b.ConfigureByConvention();
            
            b.Property(x => x.PreviousRoomDailyRate).HasColumnType("decimal(18,2)");
            b.Property(x => x.PreviousRoomTotalAmount).HasColumnType("decimal(18,2)");
            
            b.HasIndex(x => x.AdmissionId);
            b.HasIndex(x => x.FromRoomId);
            b.HasIndex(x => x.ToRoomId);
        });

        // Surgical Operations
        builder.Entity<HIS.Operations.SurgicalOperation>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "SurgicalOperations", HISConsts.DbSchema);
            b.ConfigureByConvention();
            
            b.Property(x => x.OperationName).HasMaxLength(256).IsRequired();
            b.Property(x => x.Details).HasMaxLength(1000);
            b.Property(x => x.Notes).HasMaxLength(500);
            b.Property(x => x.TotalAmount).HasColumnType("decimal(18,2)");
            b.Property(x => x.CompanyShare).HasColumnType("decimal(18,2)");
            b.Property(x => x.PatientShare).HasColumnType("decimal(18,2)");
            b.Property(x => x.InsuranceTotal).HasColumnType("decimal(18,2)");
            b.Property(x => x.AnesthesiologistFeeAmount).HasColumnType("decimal(18,2)");
            b.Property(x => x.AnesthesiologistFeePercentage).HasColumnType("decimal(5,2)");
            b.Property(x => x.HospitalShareAmount).HasColumnType("decimal(18,2)");
            b.Property(x => x.SurgeonFeeAmount).HasColumnType("decimal(18,2)");
            b.Property(x => x.SurgeonFeePercentage).HasColumnType("decimal(5,2)");
            
            b.HasIndex(x => x.DoctorId);
            b.HasIndex(x => x.Status);
        });

        // Insurance Service Prices
        builder.Entity<HIS.Insurance.InsuranceServicePrice>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "InsuranceServicePrices", HISConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.CustomPrice).HasPrecision(18, 2);
            b.Property(x => x.Notes).HasMaxLength(1024);
            b.HasIndex(x => x.InsurancePlanId);
            b.HasIndex(x => x.ServiceItemId);
        });

        // --- Nursing Module ---

        builder.Entity<MedicationAdministration>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "MedicationAdministrations", HISConsts.DbSchema);
            b.ConfigureByConvention();
            
            b.Property(x => x.DrugName).HasMaxLength(256);
            b.Property(x => x.Dosage).HasMaxLength(128);
            b.Property(x => x.Notes).HasMaxLength(500);

            b.HasIndex(x => x.PatientId);
            b.HasIndex(x => x.MedicalOrderId);
            b.HasIndex(x => x.AdministrationTime);
        });

        builder.Entity<CarePlan>(b =>
        {
            b.ToTable(HISConsts.DbTablePrefix + "CarePlans", HISConsts.DbSchema);
            b.ConfigureByConvention();
            
            b.Property(x => x.Diagnosis).HasMaxLength(512).IsRequired();
            b.Property(x => x.Goal).HasMaxLength(1024).IsRequired();
            b.Property(x => x.Interventions).HasMaxLength(2048);
            b.Property(x => x.Evaluation).HasMaxLength(1024);
            
            b.HasIndex(x => x.PatientId);
            b.HasIndex(x => x.Status);
        });

            // Phase 2 Configurations

            builder.Entity<PatientRound>(b =>
            {
                b.ToTable(HISConsts.DbTablePrefix + "PatientRounds", HISConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Note).HasMaxLength(2048).IsRequired();
                b.HasIndex(x => x.PatientId);
            });

            builder.Entity<PainAssessment>(b =>
            {
                b.ToTable(HISConsts.DbTablePrefix + "PainAssessments", HISConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Characteristics).HasMaxLength(512);
                b.Property(x => x.Intervention).HasMaxLength(512);
                b.HasIndex(x => x.PatientId);
            });

            builder.Entity<FallRiskAssessment>(b =>
            {
                b.ToTable(HISConsts.DbTablePrefix + "FallRiskAssessments", HISConsts.DbSchema);
                b.ConfigureByConvention();
                b.HasIndex(x => x.PatientId);
            });

            builder.Entity<WoundCare>(b =>
            {
                b.ToTable(HISConsts.DbTablePrefix + "WoundCares", HISConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Location).HasMaxLength(128);
                b.Property(x => x.Exudate).HasMaxLength(128);
                b.Property(x => x.Treatment).HasMaxLength(512);
                b.Property(x => x.Notes).HasMaxLength(1024);
                b.HasIndex(x => x.PatientId);
            });

            builder.Entity<FluidBalance>(b =>
            {
                b.ToTable(HISConsts.DbTablePrefix + "FluidBalances", HISConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Notes).HasMaxLength(512);
                b.HasIndex(x => x.PatientId);
                b.HasIndex(x => x.EntryTime);
            });

            builder.Entity<ShiftHandover>(b =>
            {
                b.ToTable(HISConsts.DbTablePrefix + "ShiftHandovers", HISConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Notes).HasMaxLength(4096);
                b.HasIndex(x => x.HandoverTime);
            });

            // ===== HR (شؤون العاملين) =====

            builder.Entity<JobGrade>(b =>
            {
                b.ToTable(HISConsts.DbTablePrefix + "JobGrades", HISConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Code).HasMaxLength(32).IsRequired();
                b.Property(x => x.NameAr).HasMaxLength(256).IsRequired();
                b.Property(x => x.NameEn).HasMaxLength(256);
                b.Property(x => x.BaseSalary).HasColumnType("decimal(18,2)");
                b.HasIndex(x => x.Code);
            });

            builder.Entity<Employee>(b =>
            {
                b.ToTable(HISConsts.DbTablePrefix + "Employees", HISConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.EmployeeNumber).HasMaxLength(64).IsRequired();
                b.Property(x => x.NameAr).HasMaxLength(256).IsRequired();
                b.Property(x => x.NameEn).HasMaxLength(256);
                b.Property(x => x.Address).HasMaxLength(512);
                b.Property(x => x.Phone).HasMaxLength(64);
                b.Property(x => x.Qualification).HasMaxLength(256);
                b.Property(x => x.IdentityNumber).HasMaxLength(128);
                b.Property(x => x.InsuranceNumber).HasMaxLength(128);
                b.Property(x => x.BankName).HasMaxLength(256);
                b.Property(x => x.BankAccountNumber).HasMaxLength(128);
                b.Property(x => x.SectionName).HasMaxLength(256);
                b.Property(x => x.JobTitle).HasMaxLength(256);
                b.Property(x => x.EmploymentClassification).HasMaxLength(128);
                b.Property(x => x.PhotoUrl).HasMaxLength(1024);
                b.Property(x => x.BasicSalary).HasColumnType("decimal(18,2)");
                b.HasIndex(x => x.EmployeeNumber);
                b.HasIndex(x => x.DepartmentId);
                b.HasIndex(x => x.JobGradeId);
            });

            builder.Entity<CompensationItem>(b =>
            {
                b.ToTable(HISConsts.DbTablePrefix + "CompensationItems", HISConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.NameAr).HasMaxLength(256).IsRequired();
                b.Property(x => x.DisplayName).HasMaxLength(256);
                b.Property(x => x.FormulaExpression).HasMaxLength(2048);
                b.HasIndex(x => x.AccountId);
            });

            builder.Entity<LeaveType>(b =>
            {
                b.ToTable(HISConsts.DbTablePrefix + "LeaveTypes", HISConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.NameAr).HasMaxLength(256).IsRequired();
                b.Property(x => x.EmployeeClass).HasMaxLength(128);
            });

            builder.Entity<EmployeeLeave>(b =>
            {
                b.ToTable(HISConsts.DbTablePrefix + "EmployeeLeaves", HISConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Notes).HasMaxLength(1024);
                b.HasIndex(x => x.EmployeeId);
                b.HasIndex(x => x.LeaveTypeId);
            });

            builder.Entity<EmployeeLoan>(b =>
            {
                b.ToTable(HISConsts.DbTablePrefix + "EmployeeLoans", HISConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Amount).HasColumnType("decimal(18,2)");
                b.Property(x => x.PaidAmount).HasColumnType("decimal(18,2)");
                b.Property(x => x.Notes).HasMaxLength(1024);
                b.HasIndex(x => x.EmployeeId);
            });

            builder.Entity<SalarySetup>(b =>
            {
                b.ToTable(HISConsts.DbTablePrefix + "SalarySetups", HISConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Amount).HasColumnType("decimal(18,2)");
                b.HasIndex(x => x.EmployeeId);
                b.HasIndex(x => x.CompensationItemId);
            });

            builder.Entity<PayrollRun>(b =>
            {
                b.ToTable(HISConsts.DbTablePrefix + "PayrollRuns", HISConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.TotalEarnings).HasColumnType("decimal(18,2)");
                b.Property(x => x.TotalDeductions).HasColumnType("decimal(18,2)");
                b.Property(x => x.NetSalary).HasColumnType("decimal(18,2)");
                b.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.PayrollRunId);
                b.HasIndex(x => x.DepartmentId);
                b.HasIndex(x => x.JournalEntryId);
            });

            builder.Entity<PayrollLine>(b =>
            {
                b.ToTable(HISConsts.DbTablePrefix + "PayrollLines", HISConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Amount).HasColumnType("decimal(18,2)");
                b.HasIndex(x => x.PayrollRunId);
                b.HasIndex(x => x.EmployeeId);
            });

            builder.Entity<Penalty>(b =>
            {
                b.ToTable(HISConsts.DbTablePrefix + "Penalties", HISConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Description).HasMaxLength(1024);
                b.Property(x => x.Amount).HasColumnType("decimal(18,2)");
                b.Property(x => x.Notes).HasMaxLength(1024);
                b.HasIndex(x => x.EmployeeId);
                b.HasIndex(x => x.Date);
            });

            builder.Entity<AttendanceRecord>(b =>
            {
                b.ToTable(HISConsts.DbTablePrefix + "AttendanceRecords", HISConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.PermitType).HasMaxLength(128);
                b.Property(x => x.Reason).HasMaxLength(1024);
                b.Property(x => x.Notes).HasMaxLength(1024);
                b.HasIndex(x => x.EmployeeId);
                b.HasIndex(x => x.Date);
            });

            builder.Entity<DailyAttendance>(b =>
            {
                b.ToTable("DailyAttendances", HISConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.WorkedHours).HasColumnType("decimal(18,2)");
                b.Property(x => x.Notes).HasMaxLength(1024);
                b.HasIndex(x => x.EmployeeId);
                b.HasIndex(x => x.Date);
            });
    }
}
