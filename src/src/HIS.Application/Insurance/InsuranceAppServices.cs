using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace HIS.Insurance;

/// <summary>
/// خدمة شركات التأمين
/// </summary>
public class InsuranceCompanyAppService : CrudAppService<InsuranceCompany, InsuranceCompanyDto, Guid, GetInsuranceCompaniesInput, CreateUpdateInsuranceCompanyDto>, IInsuranceCompanyAppService
{
    public InsuranceCompanyAppService(IRepository<InsuranceCompany, Guid> repository) : base(repository)
    {
    }

    public override async Task<InsuranceCompanyDto> CreateAsync(CreateUpdateInsuranceCompanyDto input)
    {
        if (string.IsNullOrWhiteSpace(input.Code))
        {
            input.Code = $"INS-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}";
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

    protected override async Task<IQueryable<InsuranceCompany>> CreateFilteredQueryAsync(GetInsuranceCompaniesInput input)
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

    protected override IQueryable<InsuranceCompany> ApplyDefaultSorting(IQueryable<InsuranceCompany> query)
    {
        return query.OrderBy(x => x.SortOrder).ThenBy(x => x.NameAr);
    }
}

/// <summary>
/// خدمة خطط التأمين
/// </summary>
public class InsurancePlanAppService : CrudAppService<InsurancePlan, InsurancePlanDto, Guid, GetInsurancePlansInput, CreateUpdateInsurancePlanDto>, IInsurancePlanAppService
{
    private readonly IRepository<InsuranceCompany, Guid> _companyRepository;

    public InsurancePlanAppService(
        IRepository<InsurancePlan, Guid> repository,
        IRepository<InsuranceCompany, Guid> companyRepository) : base(repository)
    {
        _companyRepository = companyRepository;
    }

    public override async Task<InsurancePlanDto> CreateAsync(CreateUpdateInsurancePlanDto input)
    {
        if (string.IsNullOrWhiteSpace(input.Code))
        {
            input.Code = $"PLN-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}";
        }
        return await base.CreateAsync(input);
    }

    public async Task<List<LookupDto>> GetLookupAsync(Guid? companyId = null)
    {
        var queryable = await Repository.GetQueryableAsync();
        
        if (companyId.HasValue)
            queryable = queryable.Where(x => x.InsuranceCompanyId == companyId);
            
        var items = await AsyncExecuter.ToListAsync(
            queryable.Where(x => x.IsActive).OrderBy(x => x.SortOrder).ThenBy(x => x.NameAr));
        
        return items.Select(x => new LookupDto { Id = x.Id, Name = x.NameAr }).ToList();
    }

    public override async Task<InsurancePlanDto> GetAsync(Guid id)
    {
        var entity = await Repository.GetAsync(id);
        var dto = ObjectMapper.Map<InsurancePlan, InsurancePlanDto>(entity);
        
        if (entity.InsuranceCompanyId != Guid.Empty)
        {
            var company = await _companyRepository.FindAsync(entity.InsuranceCompanyId);
            dto.InsuranceCompanyName = company?.NameAr;
        }
        
        return dto;
    }

    protected override async Task<IQueryable<InsurancePlan>> CreateFilteredQueryAsync(GetInsurancePlansInput input)
    {
        var queryable = await Repository.GetQueryableAsync();

        if (!string.IsNullOrEmpty(input.SearchText))
        {
            queryable = queryable.Where(x =>
                x.Code.Contains(input.SearchText) ||
                x.NameAr.Contains(input.SearchText) ||
                (x.NameEn != null && x.NameEn.Contains(input.SearchText)));
        }

        if (input.InsuranceCompanyId.HasValue)
            queryable = queryable.Where(x => x.InsuranceCompanyId == input.InsuranceCompanyId);

        if (input.IsActive.HasValue)
            queryable = queryable.Where(x => x.IsActive == input.IsActive);

        return queryable;
    }

    protected override IQueryable<InsurancePlan> ApplyDefaultSorting(IQueryable<InsurancePlan> query)
    {
        return query.OrderBy(x => x.SortOrder).ThenBy(x => x.NameAr);
    }
}

/// <summary>
/// خدمة تأمين المرضى
/// </summary>
public class PatientInsuranceAppService : CrudAppService<PatientInsurance, PatientInsuranceDto, Guid, GetPatientInsurancesInput, CreateUpdatePatientInsuranceDto>, IPatientInsuranceAppService
{
    private readonly IRepository<InsurancePlan, Guid> _planRepository;
    private readonly IRepository<InsuranceCompany, Guid> _companyRepository;

    public PatientInsuranceAppService(
        IRepository<PatientInsurance, Guid> repository,
        IRepository<InsurancePlan, Guid> planRepository,
        IRepository<InsuranceCompany, Guid> companyRepository) : base(repository)
    {
        _planRepository = planRepository;
        _companyRepository = companyRepository;
    }

    public async Task<List<PatientInsuranceDto>> GetByPatientAsync(Guid patientId)
    {
        var queryable = await Repository.GetQueryableAsync();
        var items = await AsyncExecuter.ToListAsync(
            queryable.Where(x => x.PatientId == patientId).OrderByDescending(x => x.IsPrimary));
        
        var dtos = ObjectMapper.Map<List<PatientInsurance>, List<PatientInsuranceDto>>(items);
        
        // Populate company and plan names
        foreach (var dto in dtos)
        {
            var plan = await _planRepository.FindAsync(dto.InsurancePlanId);
            if (plan != null)
            {
                dto.InsurancePlanName = plan.NameAr;
                var company = await _companyRepository.FindAsync(plan.InsuranceCompanyId);
                dto.InsuranceCompanyName = company?.NameAr;
            }
        }
        
        return dtos;
    }

    protected override async Task<IQueryable<PatientInsurance>> CreateFilteredQueryAsync(GetPatientInsurancesInput input)
    {
        var queryable = await Repository.GetQueryableAsync();

        if (input.PatientId.HasValue)
            queryable = queryable.Where(x => x.PatientId == input.PatientId);

        if (input.InsurancePlanId.HasValue)
            queryable = queryable.Where(x => x.InsurancePlanId == input.InsurancePlanId);

        if (input.Status.HasValue)
            queryable = queryable.Where(x => x.Status == input.Status);

        return queryable;
    }

    protected override IQueryable<PatientInsurance> ApplyDefaultSorting(IQueryable<PatientInsurance> query)
    {
        return query.OrderByDescending(x => x.IsPrimary).ThenByDescending(x => x.StartDate);
    }
}



/// <summary>
/// خدمة تسعير الخدمات لشركات التأمين
/// </summary>
public class InsuranceServicePriceAppService : CrudAppService<InsuranceServicePrice, InsuranceServicePriceDto, Guid, GetInsuranceServicePricesInput, CreateUpdateInsuranceServicePriceDto>, IInsuranceServicePriceAppService
{
    private readonly IRepository<InsurancePlan, Guid> _planRepository;
    private readonly IRepository<HIS.Services.ServiceItem, Guid> _serviceItemRepository;

    public InsuranceServicePriceAppService(
        IRepository<InsuranceServicePrice, Guid> repository,
        IRepository<InsurancePlan, Guid> planRepository,
        IRepository<HIS.Services.ServiceItem, Guid> serviceItemRepository) : base(repository)
    {
        _planRepository = planRepository;
        _serviceItemRepository = serviceItemRepository;
    }

    public override async Task<InsuranceServicePriceDto> GetAsync(Guid id)
    {
        var entity = await Repository.GetAsync(id);
        var dto = ObjectMapper.Map<InsuranceServicePrice, InsuranceServicePriceDto>(entity);
        
        var plan = await _planRepository.FindAsync(entity.InsurancePlanId);
        dto.InsurancePlanName = plan?.NameAr;
        
        var service = await _serviceItemRepository.FindAsync(entity.ServiceItemId);
        dto.ServiceItemName = service?.Name;
        dto.ServiceItemCode = service?.Code;
        
        return dto;
    }

    protected override async Task<IQueryable<InsuranceServicePrice>> CreateFilteredQueryAsync(GetInsuranceServicePricesInput input)
    {
        var queryable = await Repository.GetQueryableAsync();

        if (input.InsurancePlanId.HasValue)
            queryable = queryable.Where(x => x.InsurancePlanId == input.InsurancePlanId);

        if (input.ServiceItemId.HasValue)
            queryable = queryable.Where(x => x.ServiceItemId == input.ServiceItemId);

        return queryable;
    }
}

#region Interfaces
public interface IInsuranceCompanyAppService : ICrudAppService<InsuranceCompanyDto, Guid, GetInsuranceCompaniesInput, CreateUpdateInsuranceCompanyDto>
{
    Task<List<LookupDto>> GetLookupAsync();
}

public interface IInsurancePlanAppService : ICrudAppService<InsurancePlanDto, Guid, GetInsurancePlansInput, CreateUpdateInsurancePlanDto>
{
    Task<List<LookupDto>> GetLookupAsync(Guid? companyId = null);
}

public interface IPatientInsuranceAppService : ICrudAppService<PatientInsuranceDto, Guid, GetPatientInsurancesInput, CreateUpdatePatientInsuranceDto>
{
    Task<List<PatientInsuranceDto>> GetByPatientAsync(Guid patientId);
}

public interface IInsuranceServicePriceAppService : ICrudAppService<InsuranceServicePriceDto, Guid, GetInsuranceServicePricesInput, CreateUpdateInsuranceServicePriceDto>
{
}

public class LookupDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
#endregion
