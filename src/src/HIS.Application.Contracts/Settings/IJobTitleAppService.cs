using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using HIS.Settings.Dtos;

namespace HIS.Settings;

public interface IJobTitleAppService : ICrudAppService<JobTitleDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateJobTitleDto>
{
}
