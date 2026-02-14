using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using HIS.Inventory.Dtos;

namespace HIS.Inventory;

public interface ISupplierAppService : ICrudAppService<SupplierDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateSupplierDto>
{
}
