using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using HIS.Settings.Dtos;

namespace HIS.Settings;

public class JobTitleAppService : CrudAppService<JobTitle, JobTitleDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateJobTitleDto>, IJobTitleAppService
{
    private readonly IRepository<Department, Guid> _departmentRepository;

    public JobTitleAppService(IRepository<JobTitle, Guid> repository, IRepository<Department, Guid> departmentRepository) 
        : base(repository)
    {
        _departmentRepository = departmentRepository;
    }

    public override async Task<PagedResultDto<JobTitleDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        var query = await Repository.GetQueryableAsync();
        
        // Apply sorting
        if (!input.Sorting.IsNullOrWhiteSpace())
        {
            query = query.OrderBy(input.Sorting);
        }
        
        var totalCount = await AsyncExecuter.CountAsync(query);
        
        query = query.PageBy(input);
        
        var entities = await AsyncExecuter.ToListAsync(query);
        var dtos = ObjectMapper.Map<List<JobTitle>, List<JobTitleDto>>(entities);

        // Populate Department Names manually to avoid N+1 if not eager loaded, or use Include
        // Since we don't have WithDetails accessible easily in generic repo without overriding Query, let's just fetch deps
        var deptIds = dtos.Where(x => x.DepartmentId.HasValue).Select(x => x.DepartmentId.Value).Distinct().ToList();
        var departments = await _departmentRepository.GetListAsync(x => deptIds.Contains(x.Id));
        var deptDict = departments.ToDictionary(x => x.Id, x => x.NameAr);

        foreach (var dto in dtos)
        {
            if (dto.DepartmentId.HasValue && deptDict.ContainsKey(dto.DepartmentId.Value))
            {
                dto.DepartmentName = deptDict[dto.DepartmentId.Value];
            }
        }

        return new PagedResultDto<JobTitleDto>(totalCount, dtos);
    }
}
