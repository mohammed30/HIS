using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HIS.Accounting;
using HIS.Inventory;
using HIS.Patients;
using HIS.Pharmacy;
using HIS.Pharmacy.Dtos;
using HIS.Services;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Volo.Abp.Uow;
using Xunit;

namespace HIS.Pharmacy.Tests;

/// <summary>
/// اختبارات وحدة لسيناريوهات البيع والصرف من نقطة البيع POS
/// </summary>
public abstract class PosAppServiceTests<TStartupModule> : HISApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IPosAppService _posAppService;
    private readonly IRepository<Warehouse, Guid> _warehouseRepository;
    private readonly IRepository<InventoryItem, Guid> _inventoryItemRepository;
    private readonly IRepository<InventoryBatch, Guid> _batchRepository;
    private readonly IRepository<Drug, Guid> _drugRepository;
    private readonly IRepository<ServiceItem, Guid> _serviceItemRepository;
    private readonly IRepository<Account, Guid> _accountRepository;
    private readonly IRepository<AccountMapping, Guid> _accountMappingRepository;
    private readonly IRepository<HIS.Billing.Invoice, Guid> _invoiceRepository;

    protected PosAppServiceTests()
    {
        _posAppService            = GetRequiredService<IPosAppService>();
        _warehouseRepository      = GetRequiredService<IRepository<Warehouse, Guid>>();
        _inventoryItemRepository  = GetRequiredService<IRepository<InventoryItem, Guid>>();
        _batchRepository          = GetRequiredService<IRepository<InventoryBatch, Guid>>();
        _drugRepository           = GetRequiredService<IRepository<Drug, Guid>>();
        _serviceItemRepository    = GetRequiredService<IRepository<ServiceItem, Guid>>();
        _accountRepository        = GetRequiredService<IRepository<Account, Guid>>();
        _accountMappingRepository = GetRequiredService<IRepository<AccountMapping, Guid>>();
        _invoiceRepository        = GetRequiredService<IRepository<HIS.Billing.Invoice, Guid>>();
    }

    private async Task<Guid> GetOrCreatePharmacyWarehouseIdAsync()
    {
        var wh = await _warehouseRepository.FirstOrDefaultAsync(x => x.Name == "Pharmacy Warehouse" || x.Name == "مستودع الصيدلية");
        if (wh != null) return wh.Id;

        var newWh = new Warehouse(Guid.NewGuid(), "Pharmacy Warehouse", "Building B - 1st Floor", "WH-TEST");
        await _warehouseRepository.InsertAsync(newWh);
        return newWh.Id;
    }

    private async Task EnsureAccountMappingsAreFilledAsync()
    {
        var required = new[]
        {
            (Code: "4200", Name: "Sales Revenue",    NameAr: "إيرادات المبيعات",  Type: AccountType.Revenue,   Map: AccountMappingType.SalesRevenue),
            (Code: "1110", Name: "Cash",             NameAr: "الخزينة",           Type: AccountType.Asset,     Map: AccountMappingType.CashAccount),
            (Code: "1110", Name: "Cash",             NameAr: "الخزينة",           Type: AccountType.Asset,     Map: AccountMappingType.CardPaymentBank),
            (Code: "2200", Name: "VAT Output",       NameAr: "ضريبة مخرجات",      Type: AccountType.Liability, Map: AccountMappingType.VATOutput),
            (Code: "1120", Name: "Receivables/VAT",  NameAr: "مدينون ومدخلات",    Type: AccountType.Asset,     Map: AccountMappingType.VATInput),
            (Code: "1120", Name: "Receivables/VAT",  NameAr: "مدينون ومدخلات",    Type: AccountType.Asset,     Map: AccountMappingType.PatientsReceivable),
            (Code: "1120", Name: "Receivables/VAT",  NameAr: "مدينون ومدخلات",    Type: AccountType.Asset,     Map: AccountMappingType.InsuranceReceivable),
            (Code: "1130", Name: "Inventory",        NameAr: "المخزون",           Type: AccountType.Asset,     Map: AccountMappingType.Inventory),
            (Code: "5200", Name: "COGS",             NameAr: "تكلفة المبيعات",     Type: AccountType.Expense,   Map: AccountMappingType.COGS),
        };

        var accountCache = new Dictionary<string, Guid>();

        foreach (var r in required)
        {
            if (!accountCache.ContainsKey(r.Code))
            {
                var existing = await _accountRepository.FirstOrDefaultAsync(x => x.Code == r.Code);
                if (existing == null)
                {
                    var acc = new Account(Guid.NewGuid(), r.Code, r.Name, r.NameAr, r.Type);
                    await _accountRepository.InsertAsync(acc);
                    accountCache[r.Code] = acc.Id;
                }
                else
                {
                    accountCache[r.Code] = existing.Id;
                }
            }

            var accountId = accountCache[r.Code];
            var mapping   = await _accountMappingRepository.FirstOrDefaultAsync(x => x.MappingType == r.Map);
            if (mapping == null)
            {
                await _accountMappingRepository.InsertAsync(
                    new AccountMapping(Guid.NewGuid(), r.Map, accountId, isMandatory: true));
            }
            else if (mapping.AccountId == null)
            {
                mapping.AccountId = accountId;
                await _accountMappingRepository.UpdateAsync(mapping);
            }
        }
    }

    [Fact]
    public async Task ProcessSale_ShouldDeductStock_Correctly()
    {
        Guid drugId = Guid.NewGuid();
        Guid serviceItemId = Guid.NewGuid();
        Guid whId = Guid.Empty;
        Guid itemId = Guid.Empty;
        const decimal initialQty = 100m;
        const int dispenseQty = 5;

        await WithUnitOfWorkAsync(async () =>
        {
            await EnsureAccountMappingsAreFilledAsync();
            whId = await GetOrCreatePharmacyWarehouseIdAsync();

            var serviceItem = new ServiceItem(serviceItemId, "M001", "Panadol", ServiceCategory.Pharmacy) { Price = 10m };
            await _serviceItemRepository.InsertAsync(serviceItem);

            var drug = new Drug(drugId, "123", "Panadol", "Paracetamol", "500", "Tab", "Manuf") { ServiceItemId = serviceItemId };
            await _drugRepository.InsertAsync(drug);

            var item = new InventoryItem(Guid.NewGuid(), whId, serviceItemId,
                "Panadol", InventoryItemType.Medication, initialQty, 5m);
            await _inventoryItemRepository.InsertAsync(item);
            itemId = item.Id;

            await _batchRepository.InsertAsync(new InventoryBatch(
                Guid.NewGuid(), itemId, "B-POS-01", initialQty, 5m,
                DateTime.Now.AddDays(-10), "PO-POS-01"));
        });

        Guid invoiceId = Guid.Empty;

        await WithUnitOfWorkAsync(async () =>
        {
            var dto = new PosSaleDto
            {
                PaymentMethod = HIS.Billing.PaymentMethod.Cash,
                PaidAmount = 50m,
                Items = new List<PosSaleItemDto>
                {
                    new PosSaleItemDto { DrugId = drugId, Quantity = dispenseQty, UnitPrice = 10m },
                      new PosSaleItemDto { DrugId = drugId, Quantity = dispenseQty, UnitPrice = 10m }
                }
            };

            invoiceId = await _posAppService.ProcessSaleAsync(dto);
        });

        // DispenseAsync is now called internally by ProcessSaleAsync
        await WithUnitOfWorkAsync(async () =>
        {
            var updated = await _inventoryItemRepository.GetAsync(itemId);
            updated.Quantity.ShouldBe(initialQty - (dispenseQty * 2), "يجب أن يتم خصم الكمية من نقطة البيع");
            
            var inv = await _invoiceRepository.GetAsync(invoiceId);
            inv.Status.ShouldBe(HIS.Billing.InvoiceStatus.Dispensed);
        });
    }
}


