using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using HIS.Accounting.Dtos;

namespace HIS.Accounting;

public class AccountAppService : CrudAppService<Account, AccountDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateAccountDto>, IAccountAppService
{
    public AccountAppService(IRepository<Account, Guid> repository)
        : base(repository)
    {
    }

    public override async Task<AccountDto> CreateAsync(CreateUpdateAccountDto input)
    {
        // 1. Determine Parent Code
        string parentCode = "";
        if (input.ParentId.HasValue)
        {
            var parent = await Repository.GetAsync(input.ParentId.Value);
            parentCode = parent.Code;
        }

        // 2. Find max code among siblings
        var siblings = await Repository.GetListAsync(x => x.ParentId == input.ParentId);

        // Filter siblings to find proper sequence
        // Logic: 
        // If Parent is "1", Children are "11", "12"...
        // If Parent is "101", Children are "10101", "10102"?? OR "101.1"? 
        // The user request says "consistent with parent". Standard accounting is often strictly hierarchical numbers.
        // Let's assume a suffix of length 1 or 2. 
        // If siblings exist, take max code and increment.
        // If no siblings, take parent code + "1" (or "01").

        // Better Approach: 
        // If ParentId is null (Root), max code length 1 (1, 2, 3...)
        // If ParentId exists, append next number.

        string nextCode;

        if (siblings.Any())
        {
            var maxCode = siblings.Select(x => x.Code).OrderByDescending(x => x.Length).ThenByDescending(x => x).FirstOrDefault();
            // Simple increment logic - assuming numeric codes
            if (long.TryParse(maxCode, out long maxCodeVal))
            {
                nextCode = (maxCodeVal + 1).ToString();
            }
            else
            {
                // Fallback if non-numeric
                nextCode = parentCode + (siblings.Count + 1);
            }
        }
        else
        {
            // First child
            if (string.IsNullOrEmpty(parentCode))
            {
                // First Root ever? unlikely but...
                nextCode = "1";
            }
            else
            {
                // Parent "1" -> Child "11"
                // Parent "11" -> Child "111" ? 
                // USUALLY: 
                // Level 1: 1 digit (1 - Assets)
                // Level 2: 2 digits (11 - Current Assets)
                // Level 3: 4 digits (1101 - Cash) ??

                // Let's try simple concatenation of '1' for now, user can correct if schema differs.
                // Request: "consistent with parent"
                // If Parent "1", Child "11".
                nextCode = parentCode + "1";
            }
        }

        input.Code = nextCode;

        return await base.CreateAsync(input);
    }
}