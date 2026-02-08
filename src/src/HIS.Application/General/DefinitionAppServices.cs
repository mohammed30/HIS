using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace HIS.General;

public class NationalityAppService : 
    CrudAppService<Nationality, NationalityDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateNationalityDto>,
    INationalityAppService
{
    public NationalityAppService(IRepository<Nationality, Guid> repository) : base(repository)
    {
    }

    public override async Task<NationalityDto> CreateAsync(CreateUpdateNationalityDto input)
    {
        if (string.IsNullOrEmpty(input.Code))
        {
            var count = await Repository.GetCountAsync();
            input.Code = $"NAT-{(count + 1):D3}";
        }
        return await base.CreateAsync(input);
    }
}

public class ProfessionAppService : 
    CrudAppService<Profession, ProfessionDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateProfessionDto>,
    IProfessionAppService
{
    public ProfessionAppService(IRepository<Profession, Guid> repository) : base(repository)
    {
    }

    public override async Task<ProfessionDto> CreateAsync(CreateUpdateProfessionDto input)
    {
        if (string.IsNullOrEmpty(input.Code))
        {
            var count = await Repository.GetCountAsync();
            input.Code = $"PRO-{(count + 1):D3}";
        }
        return await base.CreateAsync(input);
    }
}

public class ContractAppService : 
    CrudAppService<Contract, ContractDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateContractDto>,
    IContractAppService
{
    public ContractAppService(IRepository<Contract, Guid> repository) : base(repository)
    {
    }

    public override async Task<ContractDto> CreateAsync(CreateUpdateContractDto input)
    {
        if (string.IsNullOrEmpty(input.Code))
        {
            var count = await Repository.GetCountAsync();
            input.Code = $"CON-{(count + 1):D3}";
        }
        return await base.CreateAsync(input);
    }
}

public class PatientCategoryAppService : 
    CrudAppService<PatientCategory, PatientCategoryDto, Guid, PagedAndSortedResultRequestDto, CreateUpdatePatientCategoryDto>,
    IPatientCategoryAppService
{
    public PatientCategoryAppService(IRepository<PatientCategory, Guid> repository) : base(repository)
    {
    }

    public override async Task<PatientCategoryDto> CreateAsync(CreateUpdatePatientCategoryDto input)
    {
        if (string.IsNullOrEmpty(input.Code))
        {
            var count = await Repository.GetCountAsync();
            input.Code = $"CAT-{(count + 1):D3}";
        }
        return await base.CreateAsync(input);
    }
}

public class ReferralSourceAppService : 
    CrudAppService<ReferralSource, ReferralSourceDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateReferralSourceDto>,
    IReferralSourceAppService
{
    public ReferralSourceAppService(IRepository<ReferralSource, Guid> repository) : base(repository)
    {
    }

    public override async Task<ReferralSourceDto> CreateAsync(CreateUpdateReferralSourceDto input)
    {
        if (string.IsNullOrEmpty(input.Code))
        {
            var count = await Repository.GetCountAsync();
            input.Code = $"REF-{(count + 1):D3}";
        }
        return await base.CreateAsync(input);
    }
}
