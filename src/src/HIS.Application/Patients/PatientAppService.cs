using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HIS.ActivityLogs;
using Microsoft.AspNetCore.Http;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Microsoft.AspNetCore.Authorization;
using HIS.General;
using HIS.Permissions;
using HIS.Billing;

namespace HIS.Patients;

/// <summary>
/// خدمة تطبيق المرضى
/// </summary>
[Authorize(HISPermissions.Patients.Default)]
public class PatientAppService : ApplicationService, IPatientAppService
{
    private readonly IRepository<Patient, Guid> _patientRepository;
    private readonly IRepository<Nationality, Guid> _nationalityRepository;
    private readonly IRepository<Profession, Guid> _professionRepository;
    private readonly IRepository<Contract, Guid> _contractRepository;
    private readonly IRepository<HIS.General.PaymentMethod, Guid> _paymentMethodRepository;
    private readonly IRepository<ReferralSource, Guid> _referralSourceRepository;
    private readonly IRepository<Invoice, Guid> _invoiceRepository;
    private readonly IGuidGenerator _guidGenerator;
    private readonly ICurrentTenant _currentTenant;
    private readonly ActivityLogManager _activityLogManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _env;

    public PatientAppService(
        IRepository<Patient, Guid> patientRepository,
        IRepository<Nationality, Guid> nationalityRepository,
        IRepository<Profession, Guid> professionRepository,
        IRepository<Contract, Guid> contractRepository,
        IRepository<HIS.General.PaymentMethod, Guid> paymentMethodRepository,
        IRepository<ReferralSource, Guid> referralSourceRepository,
        IRepository<Invoice, Guid> invoiceRepository,
        IGuidGenerator guidGenerator,
        ICurrentTenant currentTenant,
        ActivityLogManager activityLogManager,
        IHttpContextAccessor httpContextAccessor,
        Microsoft.AspNetCore.Hosting.IWebHostEnvironment env)
    {
        _patientRepository = patientRepository;
        _nationalityRepository = nationalityRepository;
        _professionRepository = professionRepository;
        _contractRepository = contractRepository;
        _paymentMethodRepository = paymentMethodRepository;
        _referralSourceRepository = referralSourceRepository;
        _invoiceRepository = invoiceRepository;
        _guidGenerator = guidGenerator;
        _currentTenant = currentTenant;
        _activityLogManager = activityLogManager;
        _httpContextAccessor = httpContextAccessor;
        _env = env;
    }

    public async Task<PagedResultDto<PatientDto>> GetListAsync(GetPatientsInput input)
    {
        var patientQuery = await _patientRepository.GetQueryableAsync();
        var nationalityQuery = await _nationalityRepository.GetQueryableAsync();
        var professionQuery = await _professionRepository.GetQueryableAsync();
        var contractQuery = await _contractRepository.GetQueryableAsync();
        var paymentMethodQuery = await _paymentMethodRepository.GetQueryableAsync();
        var referralQuery = await _referralSourceRepository.GetQueryableAsync();

        // Apply filters to patients
        patientQuery = ApplyFilters(patientQuery, input);

        var totalCount = await AsyncExecuter.CountAsync(patientQuery);

        // Sorting, Paging...
        patientQuery = !string.IsNullOrEmpty(input.Sorting)
            ? ApplySorting(patientQuery, input.Sorting)
            : patientQuery.OrderByDescending(x => x.CreationTime);

        patientQuery = patientQuery.Skip(input.SkipCount).Take(input.MaxResultCount);

        // Join to get names
        var query = from patient in patientQuery
                    join nat in nationalityQuery on patient.NationalityId equals nat.Id into nats
                    from nat in nats.DefaultIfEmpty()
                    join prof in professionQuery on patient.ProfessionId equals prof.Id into profs
                    from prof in profs.DefaultIfEmpty()
                    join cont in contractQuery on patient.ContractId equals cont.Id into conts
                    from cont in conts.DefaultIfEmpty()
                    join pm in paymentMethodQuery on patient.PaymentMethodId equals pm.Id into pms
                    from pm in pms.DefaultIfEmpty()
                    join refSrc in referralQuery on patient.ReferralSourceId equals refSrc.Id into refs
                    from refSrc in refs.DefaultIfEmpty()
                    select new { patient, NationalityName = nat.NameAr, ProfessionName = prof.NameAr, ContractName = cont.NameAr, PaymentMethodName = pm.NameAr, ReferralName = refSrc.NameAr };

        var results = await AsyncExecuter.ToListAsync(query);

        var dtos = results.Select(x => {
            var dto = MapToDto(x.patient);
            dto.NationalityName = x.NationalityName;
            dto.ProfessionName = x.ProfessionName;
            dto.ContractName = x.ContractName;
            dto.PaymentMethodName = x.PaymentMethodName;
            dto.ReferralSourceName = x.ReferralName;
            return dto;
        }).ToList();

        return new PagedResultDto<PatientDto>(totalCount, dtos);
    }

    public async Task<PatientDto> GetAsync(Guid id)
    {
        var patient = await _patientRepository.GetAsync(id);
        return MapToDto(patient);
    }

    [Authorize(HISPermissions.Patients.Create)]
    public async Task<PatientDto> CreateAsync(CreateUpdatePatientDto input)
    {
        // Validate Payment Method
        if (input.PaymentMethodId.HasValue)
        {
            var paymentMethodExists = await _paymentMethodRepository.AnyAsync(x => x.Id == input.PaymentMethodId.Value);
            if (!paymentMethodExists)
            {
                throw new UserFriendlyException("طريقة الدفع المختارة غير موجودة. يرجى تحديث الصفحة والمحاولة مرة أخرى.");
            }
        }

        // Validate duplicates
        if (!string.IsNullOrWhiteSpace(input.IdentityNumber))
        {
            var identityExists = await _patientRepository.AnyAsync(x => x.IdentityNumber == input.IdentityNumber);
            if (identityExists) throw new UserFriendlyException("يوجد مريض مسجل مسبقاً بنفس رقم الهوية.");
        }

        if (!string.IsNullOrWhiteSpace(input.MobileNumber))
        {
            var mobileExists = await _patientRepository.AnyAsync(x => x.MobileNumber == input.MobileNumber);
            if (mobileExists) throw new UserFriendlyException("يوجد مريض مسجل مسبقاً بنفس رقم الجوال.");
        }

        var mrn = await GenerateMrnAsync();

        var patient = new Patient(
            id: _guidGenerator.Create(),
            tenantId: _currentTenant.Id,
            mrn: mrn,
            firstNameAr: string.Empty, // Set via MapNames
            lastNameAr: string.Empty, // Set via MapNames
            dateOfBirth: input.DateOfBirth,
            gender: input.Gender,
            identityType: input.IdentityType,
            identityNumber: input.IdentityNumber,
            mobileNumber: input.MobileNumber
        );

        MapNames(patient, input);

        patient.MaritalStatus = input.MaritalStatus;
        patient.NationalityId = input.NationalityId;
        patient.ProfessionId = input.ProfessionId;
        patient.IdentityExpiryDate = input.IdentityExpiryDate;
        patient.IdentityIssueDate = input.IdentityIssueDate;
        patient.IdentityIssuePlace = input.IdentityIssuePlace;
        patient.PassportNumber = input.PassportNumber;
        patient.PassportIssueDate = input.PassportIssueDate;
        patient.PassportIssuePlace = input.PassportIssuePlace;
        patient.PassportExpiryDate = input.PassportExpiryDate;
        patient.VisaNumber = input.VisaNumber;
        patient.VisaIssueDate = input.VisaIssueDate;
        patient.VisaIssuePlace = input.VisaIssuePlace;
        patient.VisaExpiryDate = input.VisaExpiryDate;
        patient.PhoneNumber = input.PhoneNumber;
        patient.Email = input.Email;
        patient.Address = input.Address;
        patient.City = input.City;
        patient.SponsorName = input.SponsorName;
        patient.SponsorId = input.SponsorId;
        patient.EmergencyContactName = input.EmergencyContactName;
        patient.EmergencyContactRelation = input.EmergencyContactRelation;
        patient.EmergencyContactPhone = input.EmergencyContactPhone;
        patient.PaymentMethodId = input.PaymentMethodId;
        patient.ContractId = input.ContractId;
        patient.ReferralSourceId = input.ReferralSourceId;
        patient.CardNumber = input.CardNumber;
        patient.TaxFile = input.TaxFile;
        patient.BloodType = input.BloodType;
        patient.Allergies = input.Allergies;
        patient.Notes = input.Notes;
        patient.IsSocialSecurity = input.IsSocialSecurity;
        patient.IsActive = input.IsActive;

        await _patientRepository.InsertAsync(patient);

        // Log Activity
        await _activityLogManager.LogActivityAsync(
            module: "Patients",
            action: ActivityAction.Create,
            description: $"تم إنشاء مريض جديد: {patient.FullNameAr} (MRN: {patient.MRN})",
            entityType: "Patient",
            entityId: patient.Id.ToString(),
            newValues: new { patient.MRN, patient.FullNameAr, patient.MobileNumber },
            ipAddress: GetClientIp() ?? "",
            userAgent: GetUserAgent() ?? ""
        );

        return MapToDto(patient);
    }

    [Authorize(HISPermissions.Patients.Edit)]
    public async Task<PatientDto> UpdateAsync(Guid id, CreateUpdatePatientDto input)
    {
        var patient = await _patientRepository.GetAsync(id);

        // Validate duplicates
        if (!string.IsNullOrWhiteSpace(input.IdentityNumber))
        {
            var identityExists = await _patientRepository.AnyAsync(x => x.IdentityNumber == input.IdentityNumber && x.Id != id);
            if (identityExists) throw new UserFriendlyException("يوجد مريض آخر مسجل بنفس رقم الهوية.");
        }

        if (!string.IsNullOrWhiteSpace(input.MobileNumber))
        {
            var mobileExists = await _patientRepository.AnyAsync(x => x.MobileNumber == input.MobileNumber && x.Id != id);
            if (mobileExists) throw new UserFriendlyException("يوجد مريض آخر مسجل بنفس رقم الجوال.");
        }

        // Store old values for logging
        var oldValues = new { patient.FirstNameAr, patient.LastNameAr, patient.MobileNumber, patient.Email };

        MapNames(patient, input);

        patient.DateOfBirth = input.DateOfBirth;
        patient.Gender = input.Gender;
        patient.MaritalStatus = input.MaritalStatus;
        patient.NationalityId = input.NationalityId;
        patient.ProfessionId = input.ProfessionId;
        patient.IdentityType = input.IdentityType;
        patient.IdentityNumber = input.IdentityNumber;
        patient.IdentityExpiryDate = input.IdentityExpiryDate;
        patient.IdentityIssueDate = input.IdentityIssueDate;
        patient.IdentityIssuePlace = input.IdentityIssuePlace;
        patient.PassportNumber = input.PassportNumber;
        patient.PassportIssueDate = input.PassportIssueDate;
        patient.PassportIssuePlace = input.PassportIssuePlace;
        patient.PassportExpiryDate = input.PassportExpiryDate;
        patient.VisaNumber = input.VisaNumber;
        patient.VisaIssueDate = input.VisaIssueDate;
        patient.VisaIssuePlace = input.VisaIssuePlace;
        patient.VisaExpiryDate = input.VisaExpiryDate;
        patient.MobileNumber = input.MobileNumber;
        patient.PhoneNumber = input.PhoneNumber;
        patient.Email = input.Email;
        patient.Address = input.Address;
        patient.City = input.City;
        patient.SponsorName = input.SponsorName;
        patient.SponsorId = input.SponsorId;
        patient.EmergencyContactName = input.EmergencyContactName;
        patient.EmergencyContactRelation = input.EmergencyContactRelation;
        patient.EmergencyContactPhone = input.EmergencyContactPhone;
        patient.PaymentMethodId = input.PaymentMethodId;
        patient.ContractId = input.ContractId;
        patient.ReferralSourceId = input.ReferralSourceId;
        patient.CardNumber = input.CardNumber;
        patient.TaxFile = input.TaxFile;
        patient.BloodType = input.BloodType;
        patient.Allergies = input.Allergies;
        patient.Notes = input.Notes;
        patient.IsSocialSecurity = input.IsSocialSecurity;
        patient.IsActive = input.IsActive;

        await _patientRepository.UpdateAsync(patient);

        // Log Activity
        await _activityLogManager.LogActivityAsync(
            module: "Patients",
            action: ActivityAction.Update,
            description: $"تم تعديل بيانات المريض: {patient.FullNameAr} (MRN: {patient.MRN})",
            entityType: "Patient",
            entityId: patient.Id.ToString(),
            oldValues: oldValues,
            newValues: new { patient.FirstNameAr, patient.LastNameAr, patient.MobileNumber, patient.Email },
            ipAddress: GetClientIp(),
            userAgent: GetUserAgent()
        );

        return MapToDto(patient);
    }

    [Authorize(HISPermissions.Patients.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        var patient = await _patientRepository.GetAsync(id);
        var patientInfo = new { patient.MRN, patient.FullNameAr };

        await _patientRepository.DeleteAsync(id);

        // Log Activity
        await _activityLogManager.LogActivityAsync(
            module: "Patients",
            action: ActivityAction.Delete,
            description: $"تم حذف المريض: {patient.FullNameAr} (MRN: {patient.MRN})",
            entityType: "Patient",
            entityId: id.ToString(),
            oldValues: patientInfo,
            ipAddress: GetClientIp(),
            userAgent: GetUserAgent()
        );
    }

    public async Task<List<PatientLookupDto>> SearchAsync(string searchText)
    {
        var queryable = await _patientRepository.GetQueryableAsync();

        if (!string.IsNullOrEmpty(searchText))
        {
            var words = searchText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in words)
            {
                queryable = queryable.Where(x =>
                    x.MRN.Contains(word) ||
                    x.FirstNameAr.Contains(word) ||
                    (x.MiddleNameAr != null && x.MiddleNameAr.Contains(word)) ||
                    x.LastNameAr.Contains(word) ||
                    (x.FirstNameEn != null && x.FirstNameEn.Contains(word)) ||
                    (x.MiddleNameEn != null && x.MiddleNameEn.Contains(word)) ||
                    (x.LastNameEn != null && x.LastNameEn.Contains(word)) ||
                    x.IdentityNumber.Contains(word) ||
                    x.MobileNumber.Contains(word));
            }
        }

        queryable = queryable.Where(x => x.IsActive).Take(20);

        var patients = await AsyncExecuter.ToListAsync(queryable);

        return patients.Select(p => new PatientLookupDto
        {
            Id = p.Id,
            MRN = p.MRN,
            FullNameAr = p.FullNameAr,
            MobileNumber = p.MobileNumber
        }).ToList();
    }

    public async Task<PatientDto?> GetByMRNAsync(string mrn)
    {
        var queryable = await _patientRepository.GetQueryableAsync();
        var patient = await AsyncExecuter.FirstOrDefaultAsync(queryable.Where(x => x.MRN == mrn));
        return patient == null ? null : MapToDto(patient);
    }

    public async Task<PatientDto?> GetByIdentityNumberAsync(string identityNumber)
    {
        var queryable = await _patientRepository.GetQueryableAsync();
        var patient = await AsyncExecuter.FirstOrDefaultAsync(queryable.Where(x => x.IdentityNumber == identityNumber));
        return patient == null ? null : MapToDto(patient);
    }

    private async Task<string> GenerateMrnAsync()
    {
        var todayPrefix = $"MRN{DateTime.Now:yyyyMMdd}";
        var queryable = await _patientRepository.GetQueryableAsync();
        
        // We need to order by length first then by string to handle numeric ordering correctly if lengths differed,
        // but MRN format is fixed length (MRN + 8 date + 4 seq = 15 chars).
        // However, standard string sort is fine for fixed length.
        var lastMrn = await AsyncExecuter.FirstOrDefaultAsync(
            queryable
                .Where(p => p.MRN.StartsWith(todayPrefix))
                .OrderByDescending(p => p.MRN)
                .Select(p => p.MRN)
        );

        int nextNumber = 1000;
        if (!string.IsNullOrEmpty(lastMrn) && lastMrn.Length >= 4)
        {
            // Extract last 4 digits
            if (int.TryParse(lastMrn.Substring(lastMrn.Length - 4), out int lastNumber))
            {
                nextNumber = lastNumber + 1;
            }
        }

        return $"{todayPrefix}{nextNumber:D4}";
    }

    private static PatientDto MapToDto(Patient patient)
    {
        return new PatientDto
        {
            Id = patient.Id,
            MRN = patient.MRN,
            FirstNameAr = patient.FirstNameAr,
            MiddleNameAr = patient.MiddleNameAr,
            LastNameAr = patient.LastNameAr,
            FirstNameEn = patient.FirstNameEn,
            MiddleNameEn = patient.MiddleNameEn,
            LastNameEn = patient.LastNameEn,
            FullNameAr = patient.FullNameAr,
            FullNameEn = patient.FullNameEn,
            DateOfBirth = patient.DateOfBirth,
            Age = patient.Age,
            Gender = patient.Gender,
            MaritalStatus = patient.MaritalStatus,
            NationalityId = patient.NationalityId,
            ProfessionId = patient.ProfessionId,
            IdentityType = patient.IdentityType,
            IdentityNumber = patient.IdentityNumber,
            IdentityExpiryDate = patient.IdentityExpiryDate,
            IdentityIssueDate = patient.IdentityIssueDate,
            IdentityIssuePlace = patient.IdentityIssuePlace,
            PassportNumber = patient.PassportNumber,
            PassportIssueDate = patient.PassportIssueDate,
            PassportIssuePlace = patient.PassportIssuePlace,
            PassportExpiryDate = patient.PassportExpiryDate,
            VisaNumber = patient.VisaNumber,
            VisaIssueDate = patient.VisaIssueDate,
            VisaIssuePlace = patient.VisaIssuePlace,
            VisaExpiryDate = patient.VisaExpiryDate,
            MobileNumber = patient.MobileNumber,
            PhoneNumber = patient.PhoneNumber,
            Email = patient.Email,
            Address = patient.Address,
            City = patient.City,
            SponsorName = patient.SponsorName,
            SponsorId = patient.SponsorId,
            EmergencyContactName = patient.EmergencyContactName,
            EmergencyContactRelation = patient.EmergencyContactRelation,
            EmergencyContactPhone = patient.EmergencyContactPhone,
            PaymentMethodId = patient.PaymentMethodId,
            ContractId = patient.ContractId,
            ReferralSourceId = patient.ReferralSourceId,
            CardNumber = patient.CardNumber,
            TaxFile = patient.TaxFile,
            BloodType = patient.BloodType,
            Allergies = patient.Allergies,
            Notes = patient.Notes,
            IsSocialSecurity = patient.IsSocialSecurity,
            PhotoUrl = patient.PhotoUrl,
            IsActive = patient.IsActive,
            CreationTime = patient.CreationTime,
            LastModificationTime = patient.LastModificationTime
        };
    }

    private static IQueryable<Patient> ApplyFilters(IQueryable<Patient> queryable, GetPatientsInput input)
    {
        if (!string.IsNullOrEmpty(input.SearchText))
        {
            var words = input.SearchText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in words)
            {
                queryable = queryable.Where(x =>
                    x.MRN.Contains(word) ||
                    x.FirstNameAr.Contains(word) ||
                    (x.MiddleNameAr != null && x.MiddleNameAr.Contains(word)) ||
                    x.LastNameAr.Contains(word) ||
                    (x.FirstNameEn != null && x.FirstNameEn.Contains(word)) ||
                    (x.MiddleNameEn != null && x.MiddleNameEn.Contains(word)) ||
                    (x.LastNameEn != null && x.LastNameEn.Contains(word)) ||
                    x.IdentityNumber.Contains(word) ||
                    x.MobileNumber.Contains(word));
            }
        }

        if (!string.IsNullOrEmpty(input.MRN))
            queryable = queryable.Where(x => x.MRN == input.MRN);

        if (!string.IsNullOrEmpty(input.IdentityNumber))
            queryable = queryable.Where(x => x.IdentityNumber == input.IdentityNumber);

        if (!string.IsNullOrEmpty(input.MobileNumber))
            queryable = queryable.Where(x => x.MobileNumber == input.MobileNumber);

        if (input.Gender.HasValue)
            queryable = queryable.Where(x => x.Gender == input.Gender);

        if (input.PaymentMethodId.HasValue)
            queryable = queryable.Where(x => x.PaymentMethodId == input.PaymentMethodId);

        if (input.IsActive.HasValue)
            queryable = queryable.Where(x => x.IsActive == input.IsActive);

        return queryable;
    }

    private static IQueryable<Patient> ApplySorting(IQueryable<Patient> queryable, string sorting)
    {
        return sorting.ToLower() switch
        {
            "mrn" => queryable.OrderBy(x => x.MRN),
            "mrn desc" => queryable.OrderByDescending(x => x.MRN),
            "firstnamear" => queryable.OrderBy(x => x.FirstNameAr),
            "firstnamear desc" => queryable.OrderByDescending(x => x.FirstNameAr),
            "creationtime" => queryable.OrderBy(x => x.CreationTime),
            "creationtime desc" => queryable.OrderByDescending(x => x.CreationTime),
            _ => queryable.OrderByDescending(x => x.CreationTime)
        };
    }

    private string? GetClientIp() => _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
    private string? GetUserAgent() => _httpContextAccessor.HttpContext?.Request?.Headers["User-Agent"].ToString();

    private void MapNames(Patient patient, CreateUpdatePatientDto input)
    {
        // Arabic Name
        if (!string.IsNullOrEmpty(input.FullNameAr))
        {
            var parts = input.FullNameAr.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 0) patient.FirstNameAr = parts[0];
            if (parts.Length > 2)
            {
                patient.MiddleNameAr = string.Join(" ", parts.Skip(1).Take(parts.Length - 2));
                patient.LastNameAr = parts.Last();
            }
            else if (parts.Length == 2)
            {
                patient.LastNameAr = parts[1];
            }
        }
        else
        {
            patient.FirstNameAr = input.FirstNameAr;
            patient.MiddleNameAr = input.MiddleNameAr;
            patient.LastNameAr = input.LastNameAr;
        }

        // English Name
        if (!string.IsNullOrEmpty(input.FullNameEn))
        {
            var parts = input.FullNameEn.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 0) patient.FirstNameEn = parts[0];
            if (parts.Length > 2)
            {
                patient.MiddleNameEn = string.Join(" ", parts.Skip(1).Take(parts.Length - 2));
                patient.LastNameEn = parts.Last();
            }
            else if (parts.Length == 2)
            {
                patient.LastNameEn = parts[1];
            }
        }
        else
        {
            patient.FirstNameEn = input.FirstNameEn;
            patient.MiddleNameEn = input.MiddleNameEn;
            patient.LastNameEn = input.LastNameEn;
        }
    }

    public async Task<PatientServicesReportDto> GetPatientServicesReportAsync(Guid patientId, bool showUnpaidOnly = false)
    {
        var patient = await _patientRepository.GetAsync(patientId);
        
        var queryable = await _invoiceRepository.WithDetailsAsync(x => x.Items);
        var invoicesQuery = queryable.Where(x => x.PatientId == patientId && x.Status != InvoiceStatus.Draft && x.Status != InvoiceStatus.Cancelled);
        
        var invoices = await AsyncExecuter.ToListAsync(invoicesQuery);

        var report = new PatientServicesReportDto
        {
            PatientId = patient.Id,
            MRN = patient.MRN,
            PatientName = patient.FullNameAr,
            ReportDate = DateTime.Now,
            Services = new List<PatientServiceItemDto>(),
            TotalAmountInvoiced = 0,
            TotalAmountPaid = 0,
            TotalAmountDue = 0
        };

        foreach (var invoice in invoices.OrderBy(x => x.InvoiceDate))
        {
            if (showUnpaidOnly && invoice.DueAmount <= 0)
            {
                continue;
            }

            report.TotalAmountInvoiced += invoice.NetAmount;
            report.TotalAmountPaid += invoice.PaidAmount;
            report.TotalAmountDue += invoice.DueAmount;

            bool isPaid = invoice.DueAmount <= 0;
            string statusStr = isPaid ? "مدفوعة" : "غير مدفوعة";

            foreach (var item in invoice.Items)
            {
                var serviceDto = new PatientServiceItemDto
                {
                    Date = invoice.InvoiceDate,
                    InvoiceNumber = invoice.InvoiceNumber,
                    ServiceDescription = !string.IsNullOrWhiteSpace(item.Description) 
                        ? item.Description 
                        : (!string.IsNullOrWhiteSpace(item.Notes) 
                            ? item.Notes 
                            : (!string.IsNullOrWhiteSpace(item.ServiceCode) 
                                ? $"{item.ServiceType} - {item.ServiceCode}" 
                                : item.ServiceType.ToString())),
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.TotalPrice,
                    Status = statusStr,
                    IsPaid = isPaid
                };
                
                report.Services.Add(serviceDto);
            }
        }

        return report;
    }

    [Microsoft.AspNetCore.Mvc.HttpGet]
    [Microsoft.AspNetCore.Mvc.Route("api/app/patient/patient-services-report-pdf/{patientId}")]
    public async Task<Volo.Abp.Content.IRemoteStreamContent> GetPatientServicesReportPdfAsync(Guid patientId, bool showUnpaidOnly = false)
    {
        var data = await GetPatientServicesReportAsync(patientId, showUnpaidOnly);

        QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

        byte[] logoBytes = null;
        var logoPath = System.IO.Path.Combine(_env.WebRootPath ?? "", "images", "logo", "Dark.png");
        
        if (!System.IO.File.Exists(logoPath))
        {
            var devPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot", "images", "logo", "Dark.png");
            if (System.IO.File.Exists(devPath)) logoPath = devPath;
        }

        if (System.IO.File.Exists(logoPath)) logoBytes = await System.IO.File.ReadAllBytesAsync(logoPath);

        var document = new HIS.Patients.Printing.PatientServicesReportDocument
        {
            ReportData = data,
            LogoBytes = logoBytes
        };

        var pdfBytes = QuestPDF.Fluent.GenerateExtensions.GeneratePdf(document);
        var stream = new System.IO.MemoryStream(pdfBytes);
        var printTime = Clock.Now;
        var fileName = $"تقرير_الخدمات_{printTime:yyyy-MM-dd_HH-mm-ss}.pdf";
        return new Volo.Abp.Content.RemoteStreamContent(stream, fileName, "application/pdf");
    }
}
