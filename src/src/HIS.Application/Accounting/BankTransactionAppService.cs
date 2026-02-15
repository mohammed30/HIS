using System;
using HIS.Accounting.Dtos;
using HIS.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace HIS.Accounting
{
    [Authorize(HISPermissions.Billing.JournalEntries)]
    public class BankTransactionAppService : 
        CrudAppService<
            BankTransaction, 
            BankTransactionDto, 
            Guid, 
            PagedAndSortedResultRequestDto, 
            CreateUpdateBankTransactionDto>, 
        IBankTransactionAppService
    {
        public BankTransactionAppService(IRepository<BankTransaction, Guid> repository) 
            : base(repository)
        {
            
        }
    }
}
