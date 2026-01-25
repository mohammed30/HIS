using HIS.ActivityLogs;
using HIS.Patients;
using HIS.Settings;
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
    }
}

