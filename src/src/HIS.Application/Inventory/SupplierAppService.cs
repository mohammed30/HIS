using System;
using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using HIS.Inventory.Dtos;

namespace HIS.Inventory;

public class SupplierAppService : CrudAppService<Supplier, SupplierDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateSupplierDto>, ISupplierAppService
{
    public SupplierAppService(IRepository<Supplier, Guid> repository) 
        : base(repository)
    {
    }
}
