using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using HIS.Inventory.Dtos;
using HIS.Settings;
using Volo.Abp.Identity;
using Volo.Abp.Domain.Entities;


namespace HIS.Inventory;

[Authorize]
public class PurchaseRequisitionAppService : CrudAppService<
    PurchaseRequisition, 
    PurchaseRequisitionDto, 
    Guid, 
    GetPurchaseRequisitionsInput, 
    CreateUpdatePurchaseRequisitionDto>, IPurchaseRequisitionAppService
{
    private readonly IRepository<Department, Guid> _departmentRepository;
    private readonly IIdentityUserRepository _userRepository;

    public PurchaseRequisitionAppService(
        IRepository<PurchaseRequisition, Guid> repository,
        IRepository<Department, Guid> departmentRepository,
        IIdentityUserRepository userRepository) : base(repository)
    {
        _departmentRepository = departmentRepository;
        _userRepository = userRepository;
    }

    public override async Task<PurchaseRequisitionDto> CreateAsync(CreateUpdatePurchaseRequisitionDto input)
    {
        var requisitionNumber = $"PR-{DateTime.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}";
        var entity = new PurchaseRequisition(
            GuidGenerator.Create(),
            requisitionNumber,
            CurrentUser.Id.GetValueOrDefault(),
            input.DepartmentId,
            input.RequiredDate
        )
        {
            Notes = input.Notes
        };

        foreach (var lineDto in input.Lines)
        {
            entity.Lines.Add(new PurchaseRequisitionLine(
                GuidGenerator.Create(),
                entity.Id,
                lineDto.ProductId,
                lineDto.Quantity
            )
            {
                Description = lineDto.Description
            });
        }

        await Repository.InsertAsync(entity);
        return await MapToGetOutputDtoAsync(entity);
    }

    public override async Task<PurchaseRequisitionDto> UpdateAsync(Guid id, CreateUpdatePurchaseRequisitionDto input)
    {
        var entity = await GetEntityByIdAsync(id);
        
        entity.DepartmentId = input.DepartmentId;
        entity.RequiredDate = input.RequiredDate;
        entity.Notes = input.Notes;
        
        entity.Lines.Clear();
        foreach (var lineDto in input.Lines)
        {
            entity.Lines.Add(new PurchaseRequisitionLine(
                GuidGenerator.Create(),
                entity.Id,
                lineDto.ProductId,
                lineDto.Quantity
            )
            {
                Description = lineDto.Description
            });
        }
        
        await Repository.UpdateAsync(entity);
        return await MapToGetOutputDtoAsync(entity);
    }

    public async Task UpdateStatusAsync(Guid id, PurchaseRequisitionStatus status)
    {
        var entity = await Repository.GetAsync(id);
        entity.Status = status;
        await Repository.UpdateAsync(entity);
    }

    protected override async Task<PurchaseRequisitionDto> MapToGetListOutputDtoAsync(PurchaseRequisition entity)
    {
        return await MapToGetOutputDtoAsync(entity);
    }

    protected override async Task<PurchaseRequisitionDto> MapToGetOutputDtoAsync(PurchaseRequisition entity)
    {
        var dto = await base.MapToGetOutputDtoAsync(entity);
        
        if (entity.DepartmentId != Guid.Empty)
        {
            var dept = await _departmentRepository.FindAsync(entity.DepartmentId);
            dto.DepartmentName = dept?.NameAr ?? dept?.NameEn ?? "Unknown";
        }
        else
        {
            dto.DepartmentName = "None";
        }

        if (entity.RequestorId != Guid.Empty)
        {
            var user = await _userRepository.FindAsync(entity.RequestorId);
            dto.RequestorName = user?.UserName ?? "Unknown";
        }
        else
        {
            dto.RequestorName = "System";
        }

        return dto;
    }

    protected override async Task<PurchaseRequisition> GetEntityByIdAsync(Guid id)
    {
        var query = await Repository.WithDetailsAsync(x => x.Lines);
        var entity = await AsyncExecuter.FirstOrDefaultAsync(query.Where(x => x.Id == id));
        if (entity == null)
        {
            throw new EntityNotFoundException(typeof(PurchaseRequisition), id);
        }
        return entity;
    }
}

public interface IPurchaseRequisitionAppService : ICrudAppService<
    PurchaseRequisitionDto, 
    Guid, 
    GetPurchaseRequisitionsInput, 
    CreateUpdatePurchaseRequisitionDto>
{
    Task UpdateStatusAsync(Guid id, PurchaseRequisitionStatus status);
}
