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
            var mappings = await AsyncExecuter.ToListAsync(queryable);
            
            // Self-healing: Ensure all enum values exist in the database
            var allTypes = Enum.GetValues<AccountMappingType>();
            var missingTypes = allTypes.Where(t => !mappings.Any(m => m.MappingType == t)).ToList();
            
            if (missingTypes.Any())
            {
                var defaults = new Dictionary<AccountMappingType, (string Code, bool IsMandatory)>
                {
                    { AccountMappingType.SalesRevenue, ("4200", true) },
                    { AccountMappingType.CashAccount, ("1110", true) },
                    { AccountMappingType.VATOutput, ("2200", true) },
                    { AccountMappingType.VATInput, ("1120", true) },
                    { AccountMappingType.Inventory, ("1130", true) },
                    { AccountMappingType.COGS, ("5200", true) },
                    { AccountMappingType.PatientsReceivable, ("1120", true) },
                    { AccountMappingType.InsuranceReceivable, ("1120", true) },
                    { AccountMappingType.InsuranceDiscounts, ("5400", false) },
                    { AccountMappingType.InventoryAdjustment, ("5200", false) },
                    { AccountMappingType.AccruedInventory, ("2110", false) },
                    { AccountMappingType.CardPaymentBank, ("1110", true) },
                    { AccountMappingType.PatientDeposits, ("2110", false) }
                };

                foreach (var type in missingTypes)
                {
                    Guid? accountId = null;
                    if (defaults.TryGetValue(type, out var def))
                    {
                        if (!string.IsNullOrEmpty(def.Code))
                        {
                            var account = await _accountRepository.FirstOrDefaultAsync(x => x.Code == def.Code);
                            accountId = account?.Id;
                        }
                        var mapping = new AccountMapping(GuidGenerator.Create(), type, accountId, def.IsMandatory);
                        await _accountMappingRepository.InsertAsync(mapping, autoSave: true);
                    }
                }
                // Re-query after inserting
                queryable = await _accountMappingRepository.GetQueryableAsync();
                mappings = await AsyncExecuter.ToListAsync(queryable);
            }
            
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
                AccountMappingType.PatientsReceivable => "Patients Receivable Account",
                AccountMappingType.InsuranceReceivable => "Insurance Companies Receivable Account",
                AccountMappingType.InsuranceDiscounts => "Insurance Discounts & Differences",
                AccountMappingType.InventoryAdjustment => "Inventory Deficit & Surplus Adjustment",
                AccountMappingType.AccruedInventory => "Goods Received Not Invoiced (GRNI)",
                AccountMappingType.CardPaymentBank => "POS Network Bank Account",
                AccountMappingType.PatientDeposits => "Patient Deposits & Advance Payments",
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
                AccountMappingType.PatientsReceivable => "حساب ذمم العملاء / المرضى",
                AccountMappingType.InsuranceReceivable => "حساب ذمم شركات التأمين",
                AccountMappingType.InsuranceDiscounts => "خصومات وفروقات التأمين",
                AccountMappingType.InventoryAdjustment => "تسوية عجز وزيادة المخزون",
                AccountMappingType.AccruedInventory => "البضاعة المستلمة غير المفوترة",
                AccountMappingType.CardPaymentBank => "حساب البنك لشبكة نقاط البيع",
                AccountMappingType.PatientDeposits => "أمانات ودفعات مقدمة للمرضى",
                _ => type.ToString()
            };
        }
    }
}
