using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using HIS.Accounting.Dtos;
using System.Linq;

namespace HIS.Accounting
{
    public class AccountMappingAppService : HISAppService, IAccountMappingAppService
    {
        private readonly IRepository<AccountMapping, Guid> _accountMappingRepository;
        private readonly IRepository<Account, Guid> _accountRepository;

        public AccountMappingAppService(
            IRepository<AccountMapping, Guid> accountMappingRepository,
            IRepository<Account, Guid> accountRepository)
        {
            _accountMappingRepository = accountMappingRepository;
            _accountRepository = accountRepository;
        }

        public async Task<ListResultDto<AccountMappingDto>> GetListAsync()
        {
            var queryable = await _accountMappingRepository.GetQueryableAsync();
            // Include Account
            var mappings = await AsyncExecuter.ToListAsync(queryable);
            
            var dtos = new List<AccountMappingDto>();
            foreach (var m in mappings)
            {
                var dto = ObjectMapper.Map<AccountMapping, AccountMappingDto>(m);
                if (m.AccountId.HasValue)
                {
                    var account = await _accountRepository.FindAsync(m.AccountId.Value);
                    if (account != null)
                    {
                        dto.AccountCode = account.Code;
                        dto.AccountName = account.Name;
                        dto.AccountNameAr = account.NameAr;
                    }
                }
                
                dto.Description = GetDescriptionEn(m.MappingType);
                dto.DescriptionAr = GetDescriptionAr(m.MappingType);
                dtos.Add(dto);
            }

            return new ListResultDto<AccountMappingDto>(dtos);
        }

        public async Task<AccountMappingDto> UpdateAsync(Guid id, UpdateAccountMappingDto input)
        {
            var mapping = await _accountMappingRepository.GetAsync(id);
            mapping.AccountId = input.AccountId;
            
            await _accountMappingRepository.UpdateAsync(mapping);

            var dto = ObjectMapper.Map<AccountMapping, AccountMappingDto>(mapping);
            if (mapping.AccountId.HasValue)
            {
                var account = await _accountRepository.FindAsync(mapping.AccountId.Value);
                if (account != null)
                {
                    dto.AccountCode = account.Code;
                    dto.AccountName = account.Name;
                    dto.AccountNameAr = account.NameAr;
                }
            }

            dto.Description = GetDescriptionEn(mapping.MappingType);
            dto.DescriptionAr = GetDescriptionAr(mapping.MappingType);
            return dto;
        }

        private string GetDescriptionEn(AccountMappingType type)
        {
            return type switch
            {
                AccountMappingType.SalesRevenue => "Sales Revenue Account",
                AccountMappingType.CashAccount => "Default Cash Account",
                AccountMappingType.VATOutput => "VAT Output Tax Account",
                AccountMappingType.VATInput => "VAT Input Tax Account",
                AccountMappingType.Inventory => "Main Inventory Account",
                AccountMappingType.COGS => "Cost of Goods Sold Account",
                _ => type.ToString()
            };
        }

        private string GetDescriptionAr(AccountMappingType type)
        {
            return type switch
            {
                AccountMappingType.SalesRevenue => "حساب إيرادات المبيعات",
                AccountMappingType.CashAccount => "حساب الخزينة الافتراضي",
                AccountMappingType.VATOutput => "ضريبة مخرجات",
                AccountMappingType.VATInput => "ضريبة مدخلات",
                AccountMappingType.Inventory => "المخزون",
                AccountMappingType.COGS => "تكلفة المبيعات",
                _ => type.ToString()
            };
        }
    }
}
