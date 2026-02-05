using System;
using HIS.Pharmacy.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace HIS.Pharmacy;

public interface IDrugAppService : ICrudAppService<
    DrugDto, 
    Guid, 
    PagedAndSortedResultRequestDto, 
    CreateUpdateDrugDto>
{
}
