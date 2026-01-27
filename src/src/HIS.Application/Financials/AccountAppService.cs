using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HIS.Financials.Dtos;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace HIS.Financials;

[Authorize]
public class AccountAppService : ApplicationService, IAccountAppService
{
    private readonly IRepository<Account, Guid> _accountRepository;

    public AccountAppService(IRepository<Account, Guid> accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<List<AccountDto>> GetListAsync()
    {
        var accounts = await _accountRepository.GetListAsync();
        return ObjectMapper.Map<List<Account>, List<AccountDto>>(accounts);
    }

    public async Task<AccountDto> GetAsync(Guid id)
    {
        var account = await _accountRepository.GetAsync(id);
        return ObjectMapper.Map<Account, AccountDto>(account);
    }

    public async Task<AccountDto> CreateAsync(CreateUpdateAccountDto input)
    {
        var level = 0;
        string code = "";

        if (input.ParentId.HasValue)
        {
            var parent = await _accountRepository.GetAsync(input.ParentId.Value);
            level = parent.Level + 1;
            
            // Generate Code based on Parent
            // Get last child of parent
            var siblings = await _accountRepository.GetListAsync(x => x.ParentId == input.ParentId.Value);
            var maxCode = siblings.Select(x => x.Code).OrderByDescending(x => x).FirstOrDefault();

            if (maxCode != null)
            {
                // Increment last segment
                long lastCode = long.Parse(maxCode); // Assuming numeric codes for simplicity, might need smarter logic
                code = (lastCode + 1).ToString();
            }
            else
            {
                code = parent.Code + "01"; // Simple concatenation for example: 101 -> 10101
            }

            // Update parent IsLeaf
            if (parent.IsLeaf)
            {
                parent.IsLeaf = false;
                await _accountRepository.UpdateAsync(parent);
            }
        }
        else
        {
            // Root Node Logic
            var roots = await _accountRepository.GetListAsync(x => x.ParentId == null);
            code = ((int)input.Type * 10).ToString(); // e.g., Assets(1) -> 10, Liabilities(2) -> 20
        }

        var account = new Account(
            GuidGenerator.Create(),
            CurrentTenant.Id,
            code,
            input.NameAr,
            input.NameEn,
            input.Type,
            input.ParentId,
            level
        );

        await _accountRepository.InsertAsync(account);

        return ObjectMapper.Map<Account, AccountDto>(account);
    }

    public async Task<AccountDto> UpdateAsync(Guid id, CreateUpdateAccountDto input)
    {
        var account = await _accountRepository.GetAsync(id);
        
        account.NameAr = input.NameAr;
        account.NameEn = input.NameEn;
        account.Type = input.Type;
        
        // Changing parent is complex (needs code regeneration), skipping for MVP or blocking
        // if (input.ParentId != account.ParentId) ...

        await _accountRepository.UpdateAsync(account);
        return ObjectMapper.Map<Account, AccountDto>(account);
    }

    public async Task DeleteAsync(Guid id)
    {
        var hasChildren = await _accountRepository.AnyAsync(x => x.ParentId == id);
        if (hasChildren)
        {
            throw new UserFriendlyException("Cannot delete account with children.");
        }

        var account = await _accountRepository.GetAsync(id);
        
        await _accountRepository.DeleteAsync(id);

        if (account.ParentId.HasValue)
        {
            // Check if parent has other children
            var siblings = await _accountRepository.AnyAsync(x => x.ParentId == account.ParentId.Value && x.Id != id);
            if (!siblings)
            {
                var parent = await _accountRepository.GetAsync(account.ParentId.Value);
                parent.IsLeaf = true;
                await _accountRepository.UpdateAsync(parent);
            }
        }
    }

    public async Task<List<AccountDto>> GetTreeAsync()
    {
        var accounts = await _accountRepository.GetListAsync();
        return ObjectMapper.Map<List<Account>, List<AccountDto>>(accounts.OrderBy(x => x.Code).ToList());
    }
}
