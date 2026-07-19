using System;
using System.Threading.Tasks;
using HIS.Pharmacy.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace HIS.Pharmacy;

public interface IDrugAppService : ICrudAppService<
    DrugDto, 
    Guid, 
    GetDrugListDto, 
    CreateUpdateDrugDto>
{
    Task<Volo.Abp.Content.IRemoteStreamContent> GetImportTemplateAsync();
    Task ImportExcelAsync(Volo.Abp.Content.IRemoteStreamContent input);
}
