using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace HIS.Clinical;

public interface IMedicalOrderAppService : ICrudAppService<MedicalOrderDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateMedicalOrderDto>
{
}
