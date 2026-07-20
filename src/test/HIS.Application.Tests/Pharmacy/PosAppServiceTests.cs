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

    [Fact]
    public async Task ComplexSale_WithApproval_Dispense_And_PartialReturn_ShouldTrackStock_Accurately()
    {
        // 1. إعداد البيانات: دوائين في مستودع الصيدلية
        Guid drug1Id = Guid.NewGuid();
        Guid drug2Id = Guid.NewGuid();
        Guid serviceItem1Id = Guid.NewGuid();
        Guid serviceItem2Id = Guid.NewGuid();
        Guid whId = Guid.Empty;
        Guid item1Id = Guid.Empty;
        Guid item2Id = Guid.Empty;
        const decimal initialQty = 100m;

        await WithUnitOfWorkAsync(async () =>
        {
            await EnsureAccountMappingsAreFilledAsync();
            whId = await GetOrCreatePharmacyWarehouseIdAsync();

            // Drug 1
            var serviceItem1 = new ServiceItem(serviceItem1Id, "M100", "Aspirin", ServiceCategory.Pharmacy) { Price = 20m };
            await _serviceItemRepository.InsertAsync(serviceItem1);
            var drug1 = new Drug(drug1Id, "B100", "Aspirin", "Aspirin", "100", "Tab", "Manuf") { ServiceItemId = serviceItem1Id };
            await _drugRepository.InsertAsync(drug1);

            var item1 = new InventoryItem(Guid.NewGuid(), whId, serviceItem1Id, "Aspirin", InventoryItemType.Medication, initialQty, 10m);
            await _inventoryItemRepository.InsertAsync(item1);
            item1Id = item1.Id;

            await _batchRepository.InsertAsync(new InventoryBatch(Guid.NewGuid(), item1Id, "B-ASP", initialQty, 10m, DateTime.Now.AddDays(-5), "PO-1"));

            // Drug 2
            var serviceItem2 = new ServiceItem(serviceItem2Id, "M200", "Ibuprofen", ServiceCategory.Pharmacy) { Price = 30m };
            await _serviceItemRepository.InsertAsync(serviceItem2);
            var drug2 = new Drug(drug2Id, "B200", "Ibuprofen", "Ibuprofen", "400", "Tab", "Manuf") { ServiceItemId = serviceItem2Id };
            await _drugRepository.InsertAsync(drug2);

            var item2 = new InventoryItem(Guid.NewGuid(), whId, serviceItem2Id, "Ibuprofen", InventoryItemType.Medication, initialQty, 15m);
            await _inventoryItemRepository.InsertAsync(item2);
            item2Id = item2.Id;

            await _batchRepository.InsertAsync(new InventoryBatch(Guid.NewGuid(), item2Id, "B-IBU", initialQty, 15m, DateTime.Now.AddDays(-5), "PO-2"));
        });

        Guid invoiceId = Guid.Empty;

        // 2. إنشاء الفاتورة كمودة (Draft) من الصيدلي
        await WithUnitOfWorkAsync(async () =>
        {
            var dto = new PosSaleDto
            {
                PaymentMethod = HIS.Billing.PaymentMethod.Cash,
                TotalAmount = 190m, // (5 * 20) + (3 * 30) = 100 + 90
                PaidAmount = 0m,
                Items = new List<PosSaleItemDto>
                {
                    new PosSaleItemDto { DrugId = drug1Id, Quantity = 5, UnitPrice = 20m }, // Aspirin
                    new PosSaleItemDto { DrugId = drug2Id, Quantity = 3, UnitPrice = 30m }  // Ibuprofen
                }
            };
            invoiceId = await _posAppService.CreateDraftAsync(dto);
        });

        // 3. تقديم للاعتماد والموافقة والدفع
        await WithUnitOfWorkAsync(async () =>
        {
            await _posAppService.SubmitForApprovalAsync(invoiceId);
            
            var approveDto = new PosApproveDto { PaidAmount = 190m, PaymentMethod = HIS.Billing.PaymentMethod.Cash };
            await _posAppService.ApproveAndPayAsync(invoiceId, approveDto);
        });

        // التأكد من أن الكمية لم تتغير قبل الصرف
        await WithUnitOfWorkAsync(async () =>
        {
            var stock1 = await _inventoryItemRepository.GetAsync(item1Id);
            var stock2 = await _inventoryItemRepository.GetAsync(item2Id);
            stock1.Quantity.ShouldBe(100m, "الكمية يجب أن لا تُخصم قبل الصرف");
            stock2.Quantity.ShouldBe(100m, "الكمية يجب أن لا تُخصم قبل الصرف");
        });

        // 4. صرف الأدوية
        await WithUnitOfWorkAsync(async () =>
        {
            await _posAppService.DispenseAsync(invoiceId);
        });

        // التأكد من الخصم من المستودع بعد الصرف
        await WithUnitOfWorkAsync(async () =>
        {
            var stock1 = await _inventoryItemRepository.GetAsync(item1Id);
            var stock2 = await _inventoryItemRepository.GetAsync(item2Id);
            stock1.Quantity.ShouldBe(95m, "تم خصم 5 حبات أسبرين");
            stock2.Quantity.ShouldBe(97m, "تم خصم 3 حبات إيبوبروفين");
        });

        // 5. عمل مرتجع جزئي (إرجاع حبتين أسبرين)
        await WithUnitOfWorkAsync(async () =>
        {
            var invoiceDetails = await _posAppService.GetInvoiceDetailsAsync(invoiceId);
            var aspirinItem = invoiceDetails.Items.First(x => x.ServiceCode == serviceItem1Id.ToString("N"));

            var refundDto = new PosPartialRefundDto
            {
                Items = new List<PosRefundItemDto>
                {
                    new PosRefundItemDto { InvoiceItemId = aspirinItem.Id, ReturnQuantity = 2m }
                }
            };

            await _posAppService.PartialRefundAsync(invoiceId, refundDto);
        });

        // 6. التحقق من الكميات بعد الارتجاع الجزئي
        await WithUnitOfWorkAsync(async () =>
        {
            var stock1 = await _inventoryItemRepository.GetAsync(item1Id);
            var stock2 = await _inventoryItemRepository.GetAsync(item2Id);
            stock1.Quantity.ShouldBe(97m, "عادت حبتين أسبرين للمستودع (95 + 2 = 97)");
            stock2.Quantity.ShouldBe(97m, "الإيبوبروفين لم يتأثر بالمرتجع (بقي 97)");
        });
    }
}


