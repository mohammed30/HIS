using System;
using System.Threading.Tasks;
using HIS.Pharmacy.Dtos;
using HIS.Services;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

using HIS.Permissions;

namespace HIS.Pharmacy;

public class DrugAppService : CrudAppService<Drug, DrugDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateDrugDto>, IDrugAppService
{
    protected override string GetPolicyName { get; set; } = HISPermissions.Pharmacy.Drugs;
    protected override string GetListPolicyName { get; set; } = HISPermissions.Pharmacy.Drugs;
    protected override string CreatePolicyName { get; set; } = HISPermissions.Pharmacy.DrugsCreate;
    protected override string UpdatePolicyName { get; set; } = HISPermissions.Pharmacy.DrugsEdit;
    protected override string DeletePolicyName { get; set; } = HISPermissions.Pharmacy.DrugsDelete;

    private readonly IRepository<ServiceItem, Guid> _serviceItemRepository;

    public DrugAppService(
        IRepository<Drug, Guid> repository,
        IRepository<ServiceItem, Guid> serviceItemRepository) 
        : base(repository)
    {
        _serviceItemRepository = serviceItemRepository;
    }

    public override async Task<DrugDto> CreateAsync(CreateUpdateDrugDto input)
    {
        // 1. Create corresponding Service Item so it can be ordered
        var serviceItem = new ServiceItem(
            GuidGenerator.Create(),
            input.Barcode, // Use barcode as code
            $"{input.BrandName} {input.Strength} - {input.Form}", // Descriptive Name
            ServiceCategory.Pharmacy
        );
        serviceItem.Price = input.Price;
        
        await _serviceItemRepository.InsertAsync(serviceItem);

        // 2. Create Drug
        var drug = ObjectMapper.Map<CreateUpdateDrugDto, Drug>(input);
        drug.ServiceItemId = serviceItem.Id;

        await Repository.InsertAsync(drug);

        return ObjectMapper.Map<Drug, DrugDto>(drug);
    }
}
