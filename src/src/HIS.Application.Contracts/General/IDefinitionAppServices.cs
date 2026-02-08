using System;
using Volo.Abp.Application.Services;

namespace HIS.General;

public interface INationalityAppService : 
    ICrudAppService<NationalityDto, Guid, Volo.Abp.Application.Dtos.PagedAndSortedResultRequestDto, CreateUpdateNationalityDto>
{
}

public interface IProfessionAppService : 
    ICrudAppService<ProfessionDto, Guid, Volo.Abp.Application.Dtos.PagedAndSortedResultRequestDto, CreateUpdateProfessionDto>
{
}

public interface IContractAppService : 
    ICrudAppService<ContractDto, Guid, Volo.Abp.Application.Dtos.PagedAndSortedResultRequestDto, CreateUpdateContractDto>
{
}

public interface IPatientCategoryAppService : 
    ICrudAppService<PatientCategoryDto, Guid, Volo.Abp.Application.Dtos.PagedAndSortedResultRequestDto, CreateUpdatePatientCategoryDto>
{
}

public interface IReferralSourceAppService : 
    ICrudAppService<ReferralSourceDto, Guid, Volo.Abp.Application.Dtos.PagedAndSortedResultRequestDto, CreateUpdateReferralSourceDto>
{
}
