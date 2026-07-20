using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HIS.Accounting;
using Volo.Abp.Domain.Repositories;

namespace HIS.Billing.Tests;

public abstract class BillingTestBase<TStartupModule> : HISApplicationTestBase<TStartupModule>
    where TStartupModule : Volo.Abp.Modularity.IAbpModule
{
    protected readonly IRepository<Account, Guid> AccountRepository;
    protected readonly IRepository<AccountMapping, Guid> AccountMappingRepository;

    protected BillingTestBase()
    {
        AccountRepository = GetRequiredService<IRepository<Account, Guid>>();
        AccountMappingRepository = GetRequiredService<IRepository<AccountMapping, Guid>>();
    }

    protected virtual async Task EnsureAccountMappingsAreFilledAsync()
    {
        var required = new[]
        {
            (Code: "4200", Name: "Sales Revenue",    NameAr: "إيرادات المبيعات",  Type: AccountType.Revenue,   Map: AccountMappingType.SalesRevenue),
            (Code: "1110", Name: "Cash",             NameAr: "الخزينة",           Type: AccountType.Asset,     Map: AccountMappingType.CashAccount),
            (Code: "1111", Name: "Bank",             NameAr: "البنك",             Type: AccountType.Asset,     Map: AccountMappingType.CardPaymentBank),
            (Code: "2200", Name: "VAT Output",       NameAr: "ضريبة مخرجات",      Type: AccountType.Liability, Map: AccountMappingType.VATOutput),
            (Code: "1120", Name: "VAT Input",        NameAr: "ضريبة مدخلات",      Type: AccountType.Asset,     Map: AccountMappingType.VATInput),
            (Code: "1121", Name: "Patients Recv",    NameAr: "ذمم مرضى",          Type: AccountType.Asset,     Map: AccountMappingType.PatientsReceivable),
            (Code: "1122", Name: "Insurance Recv",   NameAr: "ذمم تأمين",         Type: AccountType.Asset,     Map: AccountMappingType.InsuranceReceivable),
            (Code: "1130", Name: "Inventory",        NameAr: "المخزون",           Type: AccountType.Asset,     Map: AccountMappingType.Inventory),
            (Code: "5200", Name: "COGS",             NameAr: "تكلفة المبيعات",     Type: AccountType.Expense,   Map: AccountMappingType.COGS),
        };

        var accountCache = new Dictionary<string, Guid>();
        foreach (var r in required)
        {
            if (!accountCache.ContainsKey(r.Code))
            {
                var existing = await AccountRepository.FirstOrDefaultAsync(x => x.Code == r.Code);
                if (existing == null)
                {
                    var acc = new Account(Guid.NewGuid(), r.Code, r.Name, r.NameAr, r.Type);
                    await AccountRepository.InsertAsync(acc);
                    accountCache[r.Code] = acc.Id;
                }
                else
                {
                    accountCache[r.Code] = existing.Id;
                }
            }

            var accountId = accountCache[r.Code];
            var mapping   = await AccountMappingRepository.FirstOrDefaultAsync(x => x.MappingType == r.Map);
            if (mapping == null)
            {
                await AccountMappingRepository.InsertAsync(
                    new AccountMapping(Guid.NewGuid(), r.Map, accountId, isMandatory: true));
            }
            else if (mapping.AccountId == null)
            {
                mapping.AccountId = accountId;
                await AccountMappingRepository.UpdateAsync(mapping);
            }
        }
    }
}
