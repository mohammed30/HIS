using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

using Microsoft.AspNetCore.Authorization;
using HIS.Permissions;

using HIS.Accounting;
using Volo.Abp.Guids;

namespace HIS.Settings;

/// <summary>
/// خدمة تطبيق الأقسام
/// </summary>
[Authorize(HISPermissions.Settings.Default)]
public class DepartmentAppService : CrudAppService<Department, DepartmentDto, Guid, GetDepartmentsInput, CreateUpdateDepartmentDto>, IDepartmentAppService
{
    private readonly IRepository<Account, Guid> _accountRepository;
    private readonly IRepository<CostCenter, Guid> _costCenterRepository;

    public DepartmentAppService(
        IRepository<Department, Guid> repository,
        IRepository<Account, Guid> accountRepository,
        IRepository<CostCenter, Guid> costCenterRepository) : base(repository)
    {
        _accountRepository = accountRepository;
        _costCenterRepository = costCenterRepository;
    }

    private string GenerateNextAccountCode(string parentCode, List<string> existingChildCodes)
    {
        if (parentCode.Length == 4)
        {
            if (parentCode.EndsWith("000"))
            {
                int step = 100;
                int start = int.Parse(parentCode) + step;
                while (existingChildCodes.Contains(start.ToString()))
                {
                    start += step;
                }
                return start.ToString();
            }
            else if (parentCode.EndsWith("00"))
            {
                int step = 10;
                int start = int.Parse(parentCode) + step;
                while (existingChildCodes.Contains(start.ToString()))
                {
                    start += step;
                }
                return start.ToString();
            }
            else if (parentCode.EndsWith("0"))
            {
                int step = 1;
                int start = int.Parse(parentCode) + step;
                while (existingChildCodes.Contains(start.ToString()))
                {
                    start += step;
                }
                return start.ToString();
            }
            else
            {
                int start = int.Parse(parentCode) + 1;
                while (existingChildCodes.Contains(start.ToString()))
                {
                    start += 1;
                }
                return start.ToString();
            }
        }
        
        if (existingChildCodes.Any())
        {
            var maxCode = existingChildCodes.OrderByDescending(c => c).First();
            if (int.TryParse(maxCode, out int val))
            {
                return (val + 1).ToString();
            }
        }
        return parentCode + "1";
    }

    public override async Task<DepartmentDto> CreateAsync(CreateUpdateDepartmentDto input)
    {
        if (string.IsNullOrWhiteSpace(input.Code))
        {
            input.Code = $"DEP-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }

        if (input.CreateCostCenterAccount && input.ParentAccountId.HasValue)
        {
            var parentAccount = await _accountRepository.FirstOrDefaultAsync(x => x.Id == input.ParentAccountId.Value);
            if (parentAccount != null)
            {
                var allAccounts = await _accountRepository.GetListAsync();
                var childCodes = allAccounts.Select(x => x.Code).ToList();
                
                string nextAccountCode = GenerateNextAccountCode(parentAccount.Code, childCodes);

                string prefixNameEn = parentAccount.Type == AccountType.Expense ? "Expense" : "Revenue";
                string prefixNameAr = parentAccount.Type == AccountType.Expense ? "مصروفات" : "إيرادات";

                var accountId = GuidGenerator.Create();
                var account = new Account(
                    accountId,
                    nextAccountCode,
                    $"{prefixNameEn} - {input.NameAr}",
                    $"{prefixNameAr} - {input.NameAr}",
                    parentAccount.Type,
                    parentAccount.Id
                );
                await _accountRepository.InsertAsync(account, autoSave: true);

                var costCenterId = GuidGenerator.Create();
                var costCenter = new CostCenter(
                    costCenterId,
                    input.Code,
                    input.NameAr,
                    input.NameEn ?? input.NameAr
                );
                await _costCenterRepository.InsertAsync(costCenter, autoSave: true);

                input.CostCenterId = costCenterId;
            }
        }

        return await base.CreateAsync(input);
    }

    public async Task<List<LookupDto>> GetLookupAsync()
    {
        var queryable = await Repository.GetQueryableAsync();
        var items = await AsyncExecuter.ToListAsync(
            queryable.Where(x => x.IsActive).OrderBy(x => x.SortOrder).ThenBy(x => x.NameAr));
        
        return items.Select(x => new LookupDto { Id = x.Id, Name = x.NameAr }).ToList();
    }

    /// <summary>
    /// يُرجع قائمة الأقسام الطبية فقط (للاستخدام في تعريف الأطباء)
    /// </summary>
    public async Task<List<LookupDto>> GetMedicalDepartmentsLookupAsync()
    {
        var queryable = await Repository.GetQueryableAsync();
        var items = await AsyncExecuter.ToListAsync(
            queryable.Where(x => x.IsActive && x.IsMedical).OrderBy(x => x.SortOrder).ThenBy(x => x.NameAr));
        
        return items.Select(x => new LookupDto { Id = x.Id, Name = $"{x.Code} - {x.NameAr}" }).ToList();
    }

    protected override async Task<IQueryable<Department>> CreateFilteredQueryAsync(GetDepartmentsInput input)
    {
        var queryable = await Repository.GetQueryableAsync();

        if (!string.IsNullOrEmpty(input.SearchText))
        {
            queryable = queryable.Where(x =>
                x.Code.Contains(input.SearchText) ||
                x.NameAr.Contains(input.SearchText) ||
                (x.NameEn != null && x.NameEn.Contains(input.SearchText)));
        }

        if (input.IsActive.HasValue)
            queryable = queryable.Where(x => x.IsActive == input.IsActive);

        if (input.IsMedical.HasValue)
            queryable = queryable.Where(x => x.IsMedical == input.IsMedical);

        return queryable;
    }

    protected override IQueryable<Department> ApplyDefaultSorting(IQueryable<Department> query)
    {
        return query.OrderBy(x => x.SortOrder).ThenBy(x => x.NameAr);
    }
}

/// <summary>
/// خدمة تطبيق التخصصات
/// </summary>
[Authorize(HISPermissions.Settings.Default)]
public class SpecialtyAppService : CrudAppService<Specialty, SpecialtyDto, Guid, GetSpecialtiesInput, CreateUpdateSpecialtyDto>, ISpecialtyAppService
{
    public SpecialtyAppService(IRepository<Specialty, Guid> repository) : base(repository)
    {
    }

    public override async Task<SpecialtyDto> CreateAsync(CreateUpdateSpecialtyDto input)
    {
        if (string.IsNullOrWhiteSpace(input.Code))
        {
            input.Code = $"SPC-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }
        return await base.CreateAsync(input);
    }

    public async Task<List<LookupDto>> GetLookupAsync()
    {
        var queryable = await Repository.GetQueryableAsync();
        var items = await AsyncExecuter.ToListAsync(
            queryable.Where(x => x.IsActive).OrderBy(x => x.SortOrder).ThenBy(x => x.NameAr));
        
        return items.Select(x => new LookupDto { Id = x.Id, Name = x.NameAr }).ToList();
    }

    protected override async Task<IQueryable<Specialty>> CreateFilteredQueryAsync(GetSpecialtiesInput input)
    {
        var queryable = await Repository.GetQueryableAsync();

        if (!string.IsNullOrEmpty(input.SearchText))
        {
            queryable = queryable.Where(x =>
                x.Code.Contains(input.SearchText) ||
                x.NameAr.Contains(input.SearchText) ||
                (x.NameEn != null && x.NameEn.Contains(input.SearchText)));
        }

        if (input.IsActive.HasValue)
            queryable = queryable.Where(x => x.IsActive == input.IsActive);

        return queryable;
    }

    protected override IQueryable<Specialty> ApplyDefaultSorting(IQueryable<Specialty> query)
    {
        return query.OrderBy(x => x.SortOrder).ThenBy(x => x.NameAr);
    }
}

/// <summary>
/// خدمة تطبيق العيادات
/// </summary>
[Authorize(HISPermissions.Settings.Default)]
public class ClinicAppService : CrudAppService<Clinic, ClinicDto, Guid, GetClinicsInput, CreateUpdateClinicDto>, IClinicAppService
{
    private readonly IRepository<Department, Guid> _departmentRepository;

    public ClinicAppService(
        IRepository<Clinic, Guid> repository,
        IRepository<Department, Guid> departmentRepository) : base(repository)
    {
        _departmentRepository = departmentRepository;
    }

    protected override async Task<ClinicDto> MapToGetOutputDtoAsync(Clinic entity)
    {
        var dto = await base.MapToGetOutputDtoAsync(entity);
        var dept = await _departmentRepository.FindAsync(entity.DepartmentId);
        dto.DepartmentName = dept?.NameAr;
        return dto;
    }

    protected override async Task<ClinicDto> MapToGetListOutputDtoAsync(Clinic entity)
    {
        return await MapToGetOutputDtoAsync(entity);
    }

    public override async Task<ClinicDto> CreateAsync(CreateUpdateClinicDto input)
    {
        if (string.IsNullOrWhiteSpace(input.Code))
        {
            input.Code = $"CLN-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }
        return await base.CreateAsync(input);
    }

    public async Task<List<LookupDto>> GetLookupAsync()
    {
        var queryable = await Repository.GetQueryableAsync();
        var items = await AsyncExecuter.ToListAsync(
            queryable.Where(x => x.IsActive).OrderBy(x => x.SortOrder).ThenBy(x => x.NameAr));
        
        return items.Select(x => new LookupDto { Id = x.Id, Name = x.NameAr }).ToList();
    }

    public async Task<List<ClinicDto>> GetByDepartmentAsync(Guid departmentId)
    {
        var queryable = await Repository.GetQueryableAsync();
        var items = await AsyncExecuter.ToListAsync(
            queryable.Where(x => x.DepartmentId == departmentId && x.IsActive)
                     .OrderBy(x => x.SortOrder).ThenBy(x => x.NameAr));
        
        return ObjectMapper.Map<List<Clinic>, List<ClinicDto>>(items);
    }

    protected override async Task<IQueryable<Clinic>> CreateFilteredQueryAsync(GetClinicsInput input)
    {
        var queryable = await Repository.GetQueryableAsync();

        if (!string.IsNullOrEmpty(input.SearchText))
        {
            queryable = queryable.Where(x =>
                x.Code.Contains(input.SearchText) ||
                x.NameAr.Contains(input.SearchText) ||
                (x.NameEn != null && x.NameEn.Contains(input.SearchText)));
        }

        if (input.DepartmentId.HasValue)
            queryable = queryable.Where(x => x.DepartmentId == input.DepartmentId);

        if (input.IsActive.HasValue)
            queryable = queryable.Where(x => x.IsActive == input.IsActive);

        return queryable;
    }

    protected override IQueryable<Clinic> ApplyDefaultSorting(IQueryable<Clinic> query)
    {
        return query.OrderBy(x => x.SortOrder).ThenBy(x => x.NameAr);
    }
}

/// <summary>
/// خدمة تطبيق الأطباء
/// </summary>
[Authorize(HISPermissions.Settings.Default)]
public class DoctorAppService : CrudAppService<Doctor, DoctorDto, Guid, GetDoctorsInput, CreateUpdateDoctorDto>, IDoctorAppService
{
    private readonly IRepository<Clinic, Guid> _clinicRepository;
    private readonly IRepository<Department, Guid> _departmentRepository;
    private readonly IRepository<Specialty, Guid> _specialtyRepository;
    private readonly IRepository<Account, Guid> _accountRepository;

    public DoctorAppService(
        IRepository<Doctor, Guid> repository,
        IRepository<Clinic, Guid> clinicRepository,
        IRepository<Department, Guid> departmentRepository,
        IRepository<Specialty, Guid> specialtyRepository,
        IRepository<Account, Guid> accountRepository) : base(repository)
    {
        _clinicRepository = clinicRepository;
        _departmentRepository = departmentRepository;
        _specialtyRepository = specialtyRepository;
        _accountRepository = accountRepository;
    }

    public override async Task<DoctorDto> CreateAsync(CreateUpdateDoctorDto input)
    {
        if (string.IsNullOrWhiteSpace(input.Code))
        {
            input.Code = $"DOC-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }
        var dto = await base.CreateAsync(input);

        // Auto-create accounting account under 2150 (Doctors Payable)
        await CreateDoctorAccountAsync(dto.Id, dto.Code, dto.NameAr);

        return await GetAsync(dto.Id);
    }

    public async Task SyncOldDoctorsAccountsAsync()
    {
        var doctors = await Repository.GetListAsync();
        
        var creditorsParent = await _accountRepository.FirstOrDefaultAsync(x => x.Code == "2110");
        if (creditorsParent == null) return;

        foreach (var doctor in doctors)
        {
            if (doctor.AccountId == null)
            {
                await CreateDoctorAccountAsync(doctor.Id, doctor.Code, doctor.NameAr);
            }
            else
            {
                // Fix existing account
                var account = await _accountRepository.FindAsync(doctor.AccountId.Value);
                if (account != null)
                {
                    bool needUpdate = false;
                    
                    if (account.ParentId != creditorsParent.Id)
                    {
                        account.ParentId = creditorsParent.Id;
                        account.Type = AccountType.Liability;
                        string docCode = doctor.Id.ToString().Substring(0, 4).ToUpper();
                        account.Code = creditorsParent.Code + "-DR-" + docCode;
                        needUpdate = true;
                    }

                    var expectedNameAr = $"مستحقات - {doctor.NameAr ?? doctor.NameEn}";
                    var expectedName = $"Dr. {doctor.NameAr ?? doctor.NameEn} Dues";
                    
                    if (account.NameAr != expectedNameAr || account.Name != expectedName)
                    {
                        account.NameAr = expectedNameAr;
                        account.Name = expectedName;
                        needUpdate = true;
                    }

                    if (needUpdate)
                    {
                        await _accountRepository.UpdateAsync(account);
                    }
                }
            }
        }
    }

    private async Task CreateDoctorAccountAsync(Guid doctorId, string doctorCode, string doctorNameAr)
    {
        try
        {
            var creditorsParent = await _accountRepository.FirstOrDefaultAsync(x => x.Code == "2110");
            if (creditorsParent == null) return;

            string docCode = doctorId.ToString().Substring(0, 4).ToUpper();
            var expectedNameAr = $"مستحقات - {doctorNameAr}";
            var expectedName = $"Dr. {doctorNameAr} Dues";

            var account = new Account(
                GuidGenerator.Create(),
                creditorsParent.Code + "-DR-" + docCode,
                expectedName,
                expectedNameAr,
                AccountType.Liability,
                creditorsParent.Id
            );
            await _accountRepository.InsertAsync(account, autoSave: true);

            // Link back to doctor
            var doctor = await Repository.GetAsync(doctorId);
            doctor.AccountId = account.Id;
            await Repository.UpdateAsync(doctor, autoSave: true);
        }
        catch (Exception)
        {
            // Non-critical - don't fail doctor creation if account creation fails
        }
    }

    public async Task<List<LookupDto>> GetLookupAsync()
    {
        var queryable = await Repository.GetQueryableAsync();
        var items = await AsyncExecuter.ToListAsync(
            queryable.Where(x => x.IsActive).OrderBy(x => x.SortOrder).ThenBy(x => x.NameAr));
        
        return items.Select(x => new LookupDto { Id = x.Id, Name = x.NameAr }).ToList();
    }

    public async Task<List<DoctorDto>> GetBySpecialtyAsync(Guid specialtyId)
    {
        var queryable = await Repository.GetQueryableAsync();
        var items = await AsyncExecuter.ToListAsync(
            queryable.Where(x => x.SpecialtyId == specialtyId && x.IsActive)
                     .OrderBy(x => x.SortOrder).ThenBy(x => x.NameAr));
        
        return ObjectMapper.Map<List<Doctor>, List<DoctorDto>>(items);
    }

    public async Task<List<DoctorDto>> GetByDepartmentAsync(Guid departmentId)
    {
        var queryable = await Repository.GetQueryableAsync();
        var items = await AsyncExecuter.ToListAsync(
            queryable.Where(x => x.DepartmentId == departmentId && x.IsActive)
                     .OrderBy(x => x.SortOrder).ThenBy(x => x.NameAr));
        
        return ObjectMapper.Map<List<Doctor>, List<DoctorDto>>(items);
    }

    protected override async Task<DoctorDto> MapToGetListOutputDtoAsync(Doctor entity)
    {
        return await MapToGetOutputDtoAsync(entity);
    }

    protected override async Task<DoctorDto> MapToGetOutputDtoAsync(Doctor entity)
    {
        var dto = await base.MapToGetOutputDtoAsync(entity);
        if (entity.ClinicId.HasValue)
        {
            var clinic = await _clinicRepository.FindAsync(entity.ClinicId.Value);
            dto.ClinicName = clinic?.NameAr;
        }
        
        var dept = await _departmentRepository.FindAsync(entity.DepartmentId);
        dto.DepartmentName = dept?.NameAr;

        var specialty = await _specialtyRepository.FindAsync(entity.SpecialtyId);
        dto.SpecialtyName = specialty?.NameAr;

        dto.HospitalPercentage = 100 - entity.DoctorPercentage;

        if (entity.AccountId.HasValue)
        {
            var account = await _accountRepository.FindAsync(entity.AccountId.Value);
            dto.AccountId = account?.Id;
        }

        return dto;
    }

    protected override async Task<IQueryable<Doctor>> CreateFilteredQueryAsync(GetDoctorsInput input)
    {
        var queryable = await Repository.GetQueryableAsync();

        if (!string.IsNullOrEmpty(input.SearchText))
        {
            queryable = queryable.Where(x =>
                x.Code.Contains(input.SearchText) ||
                x.NameAr.Contains(input.SearchText) ||
                (x.NameEn != null && x.NameEn.Contains(input.SearchText)));
        }

        if (input.SpecialtyId.HasValue)
            queryable = queryable.Where(x => x.SpecialtyId == input.SpecialtyId);

        if (input.DepartmentId.HasValue)
            queryable = queryable.Where(x => x.DepartmentId == input.DepartmentId);

        if (input.IsActive.HasValue)
            queryable = queryable.Where(x => x.IsActive == input.IsActive);

        return queryable;
    }

    protected override IQueryable<Doctor> ApplyDefaultSorting(IQueryable<Doctor> query)
    {
        return query.OrderBy(x => x.SortOrder).ThenBy(x => x.NameAr);
    }
}

/// <summary>
/// تقرير حق الطبيب والمستشفى
/// </summary>
[Authorize(HISPermissions.Settings.Default)]
public class DoctorRevenueReportAppService : ApplicationService, IDoctorRevenueReportAppService
{
    private readonly IRepository<Doctor, Guid> _doctorRepository;
    private readonly IRepository<Account, Guid> _accountRepository;
    private readonly IRepository<JournalEntry, Guid> _journalEntryRepository;
    private readonly IRepository<JournalEntryLine, Guid> _journalEntryLineRepository;

    public DoctorRevenueReportAppService(
        IRepository<Doctor, Guid> doctorRepository,
        IRepository<Account, Guid> accountRepository,
        IRepository<JournalEntry, Guid> journalEntryRepository,
        IRepository<JournalEntryLine, Guid> journalEntryLineRepository)
    {
        _doctorRepository = doctorRepository;
        _accountRepository = accountRepository;
        _journalEntryRepository = journalEntryRepository;
        _journalEntryLineRepository = journalEntryLineRepository;
    }

    public async Task<DoctorRevenueReportDto> GetReportAsync(DoctorRevenueReportInput input)
    {
        var fromDate = input.FromDate ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        var toDate = input.ToDate ?? DateTime.Now;
        toDate = toDate.Date.AddDays(1).AddTicks(-1);

        var doctors = await _doctorRepository.GetListAsync(x => x.IsActive && x.DoctorPercentage > 0);
        if (input.DoctorId.HasValue)
            doctors = doctors.Where(x => x.Id == input.DoctorId.Value).ToList();

        var report = new DoctorRevenueReportDto
        {
            FromDate = fromDate,
            ToDate = toDate
        };

        var invoiceRepo = LazyServiceProvider.LazyGetRequiredService<Volo.Abp.Domain.Repositories.IRepository<HIS.Billing.Invoice, Guid>>();
        var patientRepo = LazyServiceProvider.LazyGetRequiredService<Volo.Abp.Domain.Repositories.IRepository<HIS.Patients.Patient, Guid>>();
        var invoiceItemRepo = LazyServiceProvider.LazyGetRequiredService<Volo.Abp.Domain.Repositories.IRepository<HIS.Billing.InvoiceItem, Guid>>();

        foreach (var doctor in doctors.OrderBy(x => x.NameAr))
        {
            // Get doctor account code
            string? accountCode = null;
            if (doctor.AccountId.HasValue)
            {
                var account = await _accountRepository.FindAsync(doctor.AccountId.Value);
                accountCode = account?.Code;
            }

            var line = new DoctorRevenueLineDto
            {
                DoctorId = doctor.Id,
                DoctorName = doctor.NameAr,
                DoctorCode = doctor.Code,
                DoctorPercentage = doctor.DoctorPercentage,
                HospitalPercentage = 100 - doctor.DoctorPercentage,
                AccountCode = accountCode
            };

            // Calculate revenue from journal entries credited to doctor's account
            if (doctor.AccountId.HasValue)
            {
                var jLines = await _journalEntryLineRepository.GetListAsync(x => x.AccountId == doctor.AccountId.Value);
                var journalIds = jLines.Select(x => x.JournalEntryId).ToList();
                var journals = await _journalEntryRepository.GetListAsync(x =>
                    journalIds.Contains(x.Id) && x.Date >= fromDate && x.Date <= toDate);
                
                var filteredJournalIds = journals.Select(x => x.Id).ToHashSet();
                var doctorJLines = jLines.Where(x => filteredJournalIds.Contains(x.JournalEntryId) && x.Credit > 0).ToList();
                
                line.DoctorAmount = doctorJLines.Sum(x => x.Credit);
                line.TotalRevenue = doctor.DoctorPercentage > 0
                    ? line.DoctorAmount / (doctor.DoctorPercentage / 100m)
                    : 0;
                line.HospitalAmount = line.TotalRevenue - line.DoctorAmount;

                // Details
                foreach (var jl in doctorJLines)
                {
                    var je = journals.First(x => x.Id == jl.JournalEntryId);
                    var invoice = await invoiceRepo.FirstOrDefaultAsync(x => x.InvoiceNumber == je.ReferenceNumber);
                    
                    if (invoice != null)
                    {
                        var patient = await patientRepo.FindAsync(invoice.PatientId);
                        var items = await invoiceItemRepo.GetListAsync(x => x.InvoiceId == invoice.Id);
                        
                        foreach (var item in items)
                        {
                            var itemRev = (item.Quantity * item.UnitPrice) - item.DiscountAmount;
                            var itemDocShare = Math.Round(itemRev * (doctor.DoctorPercentage / 100m), 2);
                            if (itemDocShare > 0)
                            {
                                line.Details.Add(new DoctorRevenueServiceDetailDto
                                {
                                    Date = je.Date,
                                    InvoiceNumber = invoice.InvoiceNumber,
                                    PatientName = patient?.FullNameAr ?? "مريض",
                                    ServiceName = item.Description ?? "خدمة",
                                    ServicePrice = itemRev,
                                    DoctorAmount = itemDocShare
                                });
                            }
                        }
                    }
                    else
                    {
                        line.Details.Add(new DoctorRevenueServiceDetailDto
                        {
                            Date = je.Date,
                            InvoiceNumber = je.ReferenceNumber,
                            PatientName = "-",
                            ServiceName = je.Description,
                            ServicePrice = doctor.DoctorPercentage > 0 ? jl.Credit / (doctor.DoctorPercentage / 100m) : 0,
                            DoctorAmount = jl.Credit
                        });
                    }
                }
            }

            report.Lines.Add(line);
            report.TotalRevenue += line.TotalRevenue;
            report.TotalDoctorAmount += line.DoctorAmount;
            report.TotalHospitalAmount += line.HospitalAmount;
        }

        return report;
    }

    public async Task<Volo.Abp.Content.IRemoteStreamContent> GetReportPdfAsync(DoctorRevenueReportInput input)
    {
        var reportData = await GetReportAsync(input);

        QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
        
        var document = new HIS.Settings.Printing.DoctorRevenueReportDocument
        {
            ReportData = reportData,
            IsHospitalReport = input.IsHospitalReport
        };

        var pdfBytes = QuestPDF.Fluent.GenerateExtensions.GeneratePdf(document);

        return new Volo.Abp.Content.RemoteStreamContent(
            new System.IO.MemoryStream(pdfBytes),
            "DoctorRevenueReport.pdf",
            "application/pdf"
        );
    }
}

/// <summary>
/// خدمة تطبيق المعامل
/// </summary>
[Authorize(HISPermissions.Laboratory.Default)]
public class LaboratoryAppService : CrudAppService<Laboratory, LaboratoryDto, Guid, GetLaboratoriesInput, CreateUpdateLaboratoryDto>, ILaboratoryAppService
{
    public LaboratoryAppService(IRepository<Laboratory, Guid> repository) : base(repository)
    {
    }

    public override async Task<LaboratoryDto> CreateAsync(CreateUpdateLaboratoryDto input)
    {
        if (string.IsNullOrWhiteSpace(input.Code))
        {
            input.Code = $"LAB-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }
        return await base.CreateAsync(input);
    }
    
    public async Task<List<LookupDto>> GetLookupAsync()
    {
        var queryable = await Repository.GetQueryableAsync();
        var items = await AsyncExecuter.ToListAsync(
            queryable.Where(x => x.IsActive).OrderBy(x => x.SortOrder).ThenBy(x => x.NameAr));
        
        return items.Select(x => new LookupDto { Id = x.Id, Name = x.NameAr }).ToList();
    }

    protected override async Task<IQueryable<Laboratory>> CreateFilteredQueryAsync(GetLaboratoriesInput input)
    {
        var queryable = await Repository.GetQueryableAsync();

        if (!string.IsNullOrEmpty(input.SearchText))
        {
            queryable = queryable.Where(x =>
                x.Code.Contains(input.SearchText) ||
                x.NameAr.Contains(input.SearchText) ||
                (x.NameEn != null && x.NameEn.Contains(input.SearchText)));
        }

        if (input.IsActive.HasValue)
            queryable = queryable.Where(x => x.IsActive == input.IsActive);

        return queryable;
    }

    protected override IQueryable<Laboratory> ApplyDefaultSorting(IQueryable<Laboratory> query)
    {
        return query.OrderBy(x => x.SortOrder).ThenBy(x => x.NameAr);
    }
}
