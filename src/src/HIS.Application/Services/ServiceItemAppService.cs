using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HIS.Permissions;

namespace HIS.Services;

[Authorize(HISPermissions.Settings.Default)]
public class ServiceItemAppService : ApplicationService, IServiceItemAppService
{
    private readonly IRepository<ServiceItem, Guid> _serviceRepository;
    private readonly IRepository<RadiologyItem, Guid> _radiologyRepository;

    public ServiceItemAppService(
        IRepository<ServiceItem, Guid> serviceRepository,
        IRepository<RadiologyItem, Guid> radiologyRepository)
    {
        _serviceRepository = serviceRepository;
        _radiologyRepository = radiologyRepository;
    }

    // --- BASE SERVICE ITEM CRUD ---

    public async Task<PagedResultDto<ServiceItemDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        // Simple implementation, standard ABP CrudAppService handles this better usually, 
        // but explicit implementation serves specific needs.
        // For brevity in this task, forwarding to repository.
        var queryable = await _serviceRepository.GetQueryableAsync();
        var totalCount = await _serviceRepository.GetCountAsync();
        var items = await _serviceRepository.GetPagedListAsync(input.SkipCount, input.MaxResultCount, input.Sorting ?? nameof(ServiceItem.Name));

        return new PagedResultDto<ServiceItemDto>(
            totalCount,
            ObjectMapper.Map<List<ServiceItem>, List<ServiceItemDto>>(items)
        );
    }

    [HttpGet("{id:guid}")]
    public async Task<ServiceItemDto> GetAsync(Guid id)
    {
        var item = await _serviceRepository.GetAsync(id);
        return ObjectMapper.Map<ServiceItem, ServiceItemDto>(item);
    }

    public async Task<ServiceItemDto> CreateAsync(CreateUpdateServiceItemDto input)
    {
        // Auto-generate code if not provided
        var code = string.IsNullOrWhiteSpace(input.Code) 
            ? await GenerateServiceCodeAsync(input.Category)
            : input.Code;
            
        // Check code uniqueness
        var existing = await _serviceRepository.FirstOrDefaultAsync(x => x.Code == code);
        if (existing != null)
        {
            throw new Volo.Abp.UserFriendlyException($"Service Code {code} already exists.");
        }

        var item = new ServiceItem(GuidGenerator.Create(), code, input.Name, input.Category, input.DepartmentId)
        {
            IsActive = input.IsActive,
            Price = input.Price.GetValueOrDefault(),
            Unit = input.Unit,
            ReferenceRange = input.ReferenceRange,
            Instructions = input.Instructions
        };

        await _serviceRepository.InsertAsync(item);
        return ObjectMapper.Map<ServiceItem, ServiceItemDto>(item);
    }
    
    private async Task<string> GenerateServiceCodeAsync(ServiceCategory category)
    {
        var count = await _serviceRepository.CountAsync(x => x.Category == category);
        var prefix = category switch
        {
            ServiceCategory.LabTest => "LAB",
            ServiceCategory.Radiology => "RAD",
            ServiceCategory.Consultation => "CON",
            ServiceCategory.Procedure => "PRO",
            ServiceCategory.Surgery => "SUR",
            _ => "SVC"
        };
        return $"{prefix}-{(count + 1).ToString("D3")}";
    }

    public async Task<ServiceItemDto> UpdateAsync(Guid id, CreateUpdateServiceItemDto input)
    {
        var item = await _serviceRepository.GetAsync(id);
        item.Name = input.Name;
        item.Category = input.Category;
        item.DepartmentId = input.DepartmentId;
        item.IsActive = input.IsActive;
        item.Price = input.Price.GetValueOrDefault();
        item.Unit = input.Unit;
        item.ReferenceRange = input.ReferenceRange;
        item.Instructions = input.Instructions;
        // Code usually shouldn't change

        await _serviceRepository.UpdateAsync(item);
        return ObjectMapper.Map<ServiceItem, ServiceItemDto>(item);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _serviceRepository.DeleteAsync(id);
    }

    // --- RADIOLOGY SPECIFIC ---
    
    [HttpGet]
    [Route("/api/app/service-item/radiology")]
    public async Task<PagedResultDto<RadiologyItemDto>> GetRadiologyListAsync(PagedAndSortedResultRequestDto input)
    {
        var queryable = await _radiologyRepository.GetQueryableAsync();
        var count = await _radiologyRepository.GetCountAsync();
        var list = await _radiologyRepository.GetPagedListAsync(input.SkipCount, input.MaxResultCount, input.Sorting ?? nameof(RadiologyItem.Name));

        return new PagedResultDto<RadiologyItemDto>(
            count,
            ObjectMapper.Map<List<RadiologyItem>, List<RadiologyItemDto>>(list)
        );
    }

    [HttpPost]
    [Route("/api/app/service-item/radiology")]
    public async Task<RadiologyItemDto> CreateRadiologyAsync(CreateUpdateRadiologyItemDto input)
    {
         var existing = await _serviceRepository.FirstOrDefaultAsync(x => x.Code == input.Code);
        if (existing != null)
        {
            throw new Volo.Abp.UserFriendlyException($"Service Code {input.Code} already exists.");
        }

        var item = new RadiologyItem(
            GuidGenerator.Create(), 
            input.Code, 
            input.Name, 
            input.DepartmentId, 
            input.Modality, 
            input.BodyPart
        )
        {
            Price = input.Price.GetValueOrDefault(),
            Instructions = input.Instructions,
            IsActive = input.IsActive
        };

        await _radiologyRepository.InsertAsync(item);
        return ObjectMapper.Map<RadiologyItem, RadiologyItemDto>(item);
    }

    [HttpPut]
    [Route("/api/app/service-item/radiology/{id}")]
    public async Task<RadiologyItemDto> UpdateRadiologyAsync(Guid id, CreateUpdateRadiologyItemDto input)
    {
        var item = await _radiologyRepository.GetAsync(id);
        item.Name = input.Name;
        // item.Code = input.Code; // Prevent changing code for safety
        item.Category = input.Category; // Should remain Radiology
        item.DepartmentId = input.DepartmentId;
        item.IsActive = input.IsActive;
        item.Modality = input.Modality;
        item.BodyPart = input.BodyPart;
        item.Price = input.Price.GetValueOrDefault();
        item.Instructions = input.Instructions;

        await _radiologyRepository.UpdateAsync(item);
        return ObjectMapper.Map<RadiologyItem, RadiologyItemDto>(item);
    }
}
