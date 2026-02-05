using System;
using System.Threading.Tasks;
using HIS.Services;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace HIS.Clinical;

public class MedicalOrderAppService : CrudAppService<MedicalOrder, MedicalOrderDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateMedicalOrderDto>, IMedicalOrderAppService
{
    private readonly IRepository<ServiceItem, Guid> _serviceItemRepository;

    public MedicalOrderAppService(IRepository<MedicalOrder, Guid> repository, IRepository<ServiceItem, Guid> serviceItemRepository) 
        : base(repository)
    {
        _serviceItemRepository = serviceItemRepository;
    }

    public override async Task<MedicalOrderDto> CreateAsync(CreateUpdateMedicalOrderDto input)
    {
        var serviceItem = await _serviceItemRepository.GetAsync(input.ServiceItemId);

        var entity = new MedicalOrder(
            GuidGenerator.Create(),
            input.PatientId,
            input.Type,
            input.ServiceItemId,
            serviceItem.Name,
            serviceItem.Price
        );
        entity.ClinicalNotes = input.ClinicalNotes;
        entity.Details = input.Details;
        
        // If doctor identification is needed, you can insert CurrentUser.Id here
        if (CurrentUser.Id.HasValue)
        {
            entity.DoctorId = CurrentUser.Id.Value;
        }

        await Repository.InsertAsync(entity);

        return await MapToGetOutputDtoAsync(entity);
    }
}
