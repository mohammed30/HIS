using System;
using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using HIS.Inventory.Dtos;
using HIS.Accounting;
using System.Threading.Tasks;

namespace HIS.Inventory;

public class SupplierAppService : CrudAppService<Supplier, SupplierDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateSupplierDto>, ISupplierAppService
{
    private readonly IRepository<Account, Guid> _accountRepository;

    public SupplierAppService(
        IRepository<Supplier, Guid> repository,
        IRepository<Account, Guid> accountRepository) 
        : base(repository)
    {
        _accountRepository = accountRepository;
    }

    public override async Task<SupplierDto> CreateAsync(CreateUpdateSupplierDto input)
    {
        var result = await base.CreateAsync(input);

        var payableAccount = await _accountRepository.FirstOrDefaultAsync(x => x.Code == "2110");
        if (payableAccount != null)
        {
            var suppAccount = await _accountRepository.FirstOrDefaultAsync(x => x.ParentId == payableAccount.Id && x.NameAr == result.Name);
            if (suppAccount == null)
            {
                var codeSuffix = result.Name.Replace(" ", "");
                if (codeSuffix.Length > 3) codeSuffix = codeSuffix.Substring(0, 3);
                var newAccount = new Account(
                    GuidGenerator.Create(), 
                    payableAccount.Code + "-" + codeSuffix, 
                    result.Name, 
                    result.Name, 
                    payableAccount.Type, 
                    payableAccount.Id
                );
                await _accountRepository.InsertAsync(newAccount);
            }
        }

        return result;
    }
}
