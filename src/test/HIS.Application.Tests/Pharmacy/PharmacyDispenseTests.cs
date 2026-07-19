using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HIS.Accounting;
using HIS.Services;
using HIS.Clinical;
using HIS.Inventory;
using HIS.Patients;
using HIS.Pharmacy;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Volo.Abp.Uow;
using Xunit;

namespace HIS.Pharmacy.Tests;

/// <summary>
/// اختبارات وحدة لسيناريوهات صرف الأدوية من الصيدلية (abstract base)
///
/// البق الموثق: PharmacyAppService كانت تبحث عن المستودع باسم "Pharmacy"
/// بدلاً من "Pharmacy Warehouse" فيعود null ولا يحدث خصم من المخزون أبداً.
/// الإصلاح: تغيير اسم المستودع في PharmacyAppService.DispenseMedicationAsync.
///
/// ملاحظة بنية الاختبار:
/// - الاختبارات تشترك في نفس قاعدة بيانات SQLite (Collection Fixture).
/// - يجب استخدام مستودع واحد اسمه "Pharmacy Warehouse" لأن PharmacyAppService
///   تبحث بالاسم دائماً → إذا أُنشئ مستودع جديد في كل اختبار قد يُعيد FirstOrDefault
///   المستودع الخاطئ وتفشل عملية إيجاد المخزون.
/// - كل اختبار يستخدم productId مختلفاً لتجنب التعارض.
/// </summary>
public abstract class PharmacyDispenseTests<TStartupModule> : HISApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IPharmacyAppService _pharmacyAppService;
    private readonly IRepository<Warehouse, Guid> _warehouseRepository;
    private readonly IRepository<InventoryItem, Guid> _inventoryItemRepository;
    private readonly IRepository<InventoryBatch, Guid> _batchRepository;
    private readonly IRepository<MedicalOrder, Guid> _medicalOrderRepository;
    private readonly IRepository<Patient, Guid> _patientRepository;
    private readonly IRepository<Dispensing, Guid> _dispensingRepository;
    private readonly IRepository<Account, Guid> _accountRepository;
    private readonly IRepository<AccountMapping, Guid> _accountMappingRepository;

    protected PharmacyDispenseTests()
    {
        _pharmacyAppService       = GetRequiredService<IPharmacyAppService>();
        _warehouseRepository      = GetRequiredService<IRepository<Warehouse, Guid>>();
        _inventoryItemRepository  = GetRequiredService<IRepository<InventoryItem, Guid>>();
        _batchRepository          = GetRequiredService<IRepository<InventoryBatch, Guid>>();
        _medicalOrderRepository   = GetRequiredService<IRepository<MedicalOrder, Guid>>();
        _patientRepository        = GetRequiredService<IRepository<Patient, Guid>>();
        _dispensingRepository     = GetRequiredService<IRepository<Dispensing, Guid>>();
        _accountRepository        = GetRequiredService<IRepository<Account, Guid>>();
        _accountMappingRepository = GetRequiredService<IRepository<AccountMapping, Guid>>();
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  Helper 1: الحصول على مستودع الصيدلية (أو إنشاؤه)
    //  يجب أن يكون مستودع واحد مشترك لأن PharmacyAppService تبحث بالاسم.
    // ══════════════════════════════════════════════════════════════════════════
    private async Task<Guid> GetOrCreatePharmacyWarehouseIdAsync()
    {
        var wh = await _warehouseRepository
            .FirstOrDefaultAsync(x => x.Name == "Pharmacy Warehouse");
        if (wh != null) return wh.Id;

        var newWh = new Warehouse(
            Guid.NewGuid(), "Pharmacy Warehouse", "Building B - 1st Floor", "WH-TEST");
        await _warehouseRepository.InsertAsync(newWh);
        return newWh.Id;
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  Helper 2: إصلاح الـ AccountMappings الإلزامية
    //  المشكلة: FinancialDataSeedContributor يُنشئ AccountMappings بـ AccountId=null
    //  لأن الحسابات غير موجودة في بيئة الاختبار → PostEntryAsync تفشل.
    //  الحل: إنشاء الحسابات وتحديث الـ mappings لتشير إليها.
    // ══════════════════════════════════════════════════════════════════════════
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

    // ══════════════════════════════════════════════════════════════════════════
    //  سيناريو 1: Panadol Advance — خصم 10 من رصيد 995 (البق الأصلي)
    // ══════════════════════════════════════════════════════════════════════════
    [Fact]
    public async Task DispenseMedication_PanadolAdvance_ShouldDeductCorrectQuantity()
    {
        Guid productId = Guid.NewGuid();
        Guid drugId = Guid.NewGuid();
        Guid itemId    = Guid.Empty;
        Guid orderId   = Guid.Empty;
                Guid whId      = Guid.Empty;
        const decimal initialQty  = 995m;
        const decimal dispenseQty = 10m;

        await WithUnitOfWorkAsync(async () =>
        {
            await EnsureAccountMappingsAreFilledAsync();
            whId = await GetOrCreatePharmacyWarehouseIdAsync();

            var serviceItem = new ServiceItem(productId, "M001", "Panadol", ServiceCategory.Pharmacy) { Price = 10m };
            await GetRequiredService<IRepository<ServiceItem, Guid>>().InsertAsync(serviceItem);

            var drug = new Drug(drugId, "123", "Med", "Med", "500", "Tab", "Manuf") { ServiceItemId = productId };
            await GetRequiredService<IRepository<Drug, Guid>>().InsertAsync(drug);

            var item = new InventoryItem(Guid.NewGuid(), whId, productId,
                "Panadol Advance", InventoryItemType.Medication, initialQty, 5m);
            await _inventoryItemRepository.InsertAsync(item);
            itemId = item.Id;

            await _batchRepository.InsertAsync(new InventoryBatch(
                Guid.NewGuid(), itemId, "B-PAN-01", initialQty, 5m,
                DateTime.Now.AddDays(-10), "PO-PAN-01"));

            var pt = new Patient(Guid.NewGuid(), null, "TST-PA1",
                "مريض", "أ", new DateTime(1990, 1, 1), Gender.Male,
                IdentityType.NationalId, Guid.NewGuid().ToString("N")[..10], "0500000001");
            await _patientRepository.InsertAsync(pt);

            var ord = new MedicalOrder(Guid.NewGuid(), pt.Id,
                OrderType.Medication, productId, "Panadol Advance", 50m)
                { Quantity = dispenseQty };
            await _medicalOrderRepository.InsertAsync(ord);
            orderId = ord.Id;
        });

        await WithUnitOfWorkAsync(async () =>
        {
            await _pharmacyAppService.DispenseMedicationAsync(
                new DispenseDto { MedicalOrderId = orderId, WarehouseId = whId, CounselingNotes = "تناول بعد الأكل" });
        });

        await WithUnitOfWorkAsync(async () =>
        {
            var updated = await _inventoryItemRepository.GetAsync(itemId);
            updated.Quantity.ShouldBe(initialQty - dispenseQty,
                $"Panadol: يجب أن ينخفض من {initialQty} إلى {initialQty - dispenseQty}");
        });
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  سيناريو 2: Lipitor — خصم 10 من رصيد 1000 (البق الثاني)
    // ══════════════════════════════════════════════════════════════════════════
    [Fact]
    public async Task DispenseMedication_Lipitor_ShouldDeductCorrectQuantity()
    {
        Guid productId = Guid.NewGuid();
        Guid drugId = Guid.NewGuid();
        Guid itemId    = Guid.Empty;
        Guid orderId   = Guid.Empty;
                Guid whId      = Guid.Empty;
        const decimal initialQty  = 1000m;
        const decimal dispenseQty = 10m;

        await WithUnitOfWorkAsync(async () =>
        {
            await EnsureAccountMappingsAreFilledAsync();
            whId = await GetOrCreatePharmacyWarehouseIdAsync();

            var serviceItem = new ServiceItem(productId, "M001", "Panadol", ServiceCategory.Pharmacy) { Price = 10m };
            await GetRequiredService<IRepository<ServiceItem, Guid>>().InsertAsync(serviceItem);

            var drug = new Drug(drugId, "123", "Med", "Med", "500", "Tab", "Manuf") { ServiceItemId = productId };
            await GetRequiredService<IRepository<Drug, Guid>>().InsertAsync(drug);

            var item = new InventoryItem(Guid.NewGuid(), whId, productId,
                "Lipitor 20mg", InventoryItemType.Medication, initialQty, 20m);
            await _inventoryItemRepository.InsertAsync(item);
            itemId = item.Id;

            await _batchRepository.InsertAsync(new InventoryBatch(
                Guid.NewGuid(), itemId, "B-LIP-01", initialQty, 20m,
                DateTime.Now.AddDays(-5), "PO-LIP-01"));

            var pt = new Patient(Guid.NewGuid(), null, "TST-LI1",
                "مريض", "ب", new DateTime(1985, 3, 15), Gender.Female,
                IdentityType.NationalId, Guid.NewGuid().ToString("N")[..10], "0500000002");
            await _patientRepository.InsertAsync(pt);

            var ord = new MedicalOrder(Guid.NewGuid(), pt.Id,
                OrderType.Medication, productId, "Lipitor 20mg", 100m)
                { Quantity = dispenseQty };
            await _medicalOrderRepository.InsertAsync(ord);
            orderId = ord.Id;
        });

        await WithUnitOfWorkAsync(async () =>
        {
            await _pharmacyAppService.DispenseMedicationAsync(
                new DispenseDto { MedicalOrderId = orderId, WarehouseId = whId });
        });

        await WithUnitOfWorkAsync(async () =>
        {
            var updated = await _inventoryItemRepository.GetAsync(itemId);
            updated.Quantity.ShouldBe(initialQty - dispenseQty,
                "Lipitor: يجب أن يصبح 990 بعد صرف 10");
        });
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  سيناريو 3: إنشاء سجل Dispensing في قاعدة البيانات
    // ══════════════════════════════════════════════════════════════════════════
    [Fact]
    public async Task DispenseMedication_ShouldCreateDispensingRecord_InDatabase()
    {
        Guid productId = Guid.NewGuid();
        Guid drugId = Guid.NewGuid();
        Guid itemId    = Guid.Empty;
        Guid orderId   = Guid.Empty;
                Guid whId      = Guid.Empty;
        long countBefore = 0;

        await WithUnitOfWorkAsync(async () =>
        {
            await EnsureAccountMappingsAreFilledAsync();
            whId = await GetOrCreatePharmacyWarehouseIdAsync();

            var serviceItem = new ServiceItem(productId, "M001", "Panadol", ServiceCategory.Pharmacy) { Price = 10m };
            await GetRequiredService<IRepository<ServiceItem, Guid>>().InsertAsync(serviceItem);

            var drug = new Drug(drugId, "123", "Med", "Med", "500", "Tab", "Manuf") { ServiceItemId = productId };
            await GetRequiredService<IRepository<Drug, Guid>>().InsertAsync(drug);

            var item = new InventoryItem(Guid.NewGuid(), whId, productId,
                "Amoxicillin 250mg", InventoryItemType.Medication, 500m, 8m);
            await _inventoryItemRepository.InsertAsync(item);
            itemId = item.Id;

            await _batchRepository.InsertAsync(new InventoryBatch(
                Guid.NewGuid(), itemId, "B-AMX-01", 500m, 8m,
                DateTime.Now.AddDays(-7), "PO-AMX-01"));

            var pt = new Patient(Guid.NewGuid(), null, "TST-AM1",
                "مريض", "ج", new DateTime(2000, 6, 1), Gender.Male,
                IdentityType.NationalId, Guid.NewGuid().ToString("N")[..10], "0500000003");
            await _patientRepository.InsertAsync(pt);

            var ord = new MedicalOrder(Guid.NewGuid(), pt.Id,
                OrderType.Medication, productId, "Amoxicillin 250mg", 40m)
                { Quantity = 7m };
            await _medicalOrderRepository.InsertAsync(ord);
            orderId = ord.Id;

            countBefore = await _dispensingRepository.GetCountAsync();
        });

        await WithUnitOfWorkAsync(async () =>
        {
            await _pharmacyAppService.DispenseMedicationAsync(
                new DispenseDto { MedicalOrderId = orderId, WarehouseId = whId, CounselingNotes = "مرة يومياً" });
        });

        await WithUnitOfWorkAsync(async () =>
        {
            var countAfter = await _dispensingRepository.GetCountAsync();
            countAfter.ShouldBe(countBefore + 1, "يجب أن يُنشأ سجل صرف جديد");
        });
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  سيناريو 4: تغيير حالة الطلب إلى Completed
    // ══════════════════════════════════════════════════════════════════════════
    [Fact]
    public async Task DispenseMedication_ShouldChangeOrderStatus_ToCompleted()
    {
        Guid productId = Guid.NewGuid();
        Guid drugId = Guid.NewGuid();
        Guid itemId    = Guid.Empty;
        Guid orderId   = Guid.Empty;
                Guid whId      = Guid.Empty;

        await WithUnitOfWorkAsync(async () =>
        {
            await EnsureAccountMappingsAreFilledAsync();
            whId = await GetOrCreatePharmacyWarehouseIdAsync();

            var serviceItem = new ServiceItem(productId, "M001", "Panadol", ServiceCategory.Pharmacy) { Price = 10m };
            await GetRequiredService<IRepository<ServiceItem, Guid>>().InsertAsync(serviceItem);

            var drug = new Drug(drugId, "123", "Med", "Med", "500", "Tab", "Manuf") { ServiceItemId = productId };
            await GetRequiredService<IRepository<Drug, Guid>>().InsertAsync(drug);

            var item = new InventoryItem(Guid.NewGuid(), whId, productId,
                "Omeprazole 20mg", InventoryItemType.Medication, 300m, 12m);
            await _inventoryItemRepository.InsertAsync(item);
            itemId = item.Id;

            await _batchRepository.InsertAsync(new InventoryBatch(
                Guid.NewGuid(), itemId, "B-OME-01", 300m, 12m,
                DateTime.Now.AddDays(-3), "PO-OME-01"));

            var pt = new Patient(Guid.NewGuid(), null, "TST-OM1",
                "مريض", "د", new DateTime(1975, 9, 20), Gender.Male,
                IdentityType.NationalId, Guid.NewGuid().ToString("N")[..10], "0500000004");
            await _patientRepository.InsertAsync(pt);

            var ord = new MedicalOrder(Guid.NewGuid(), pt.Id,
                OrderType.Medication, productId, "Omeprazole 20mg", 60m)
                { Quantity = 14m };
            await _medicalOrderRepository.InsertAsync(ord);
            orderId = ord.Id;
        });

        await WithUnitOfWorkAsync(async () =>
        {
            await _pharmacyAppService.DispenseMedicationAsync(
                new DispenseDto { MedicalOrderId = orderId, WarehouseId = whId });
        });

        await WithUnitOfWorkAsync(async () =>
        {
            var updated = await _medicalOrderRepository.GetAsync(orderId);
            updated.Status.ShouldBe(OrderStatus.Completed,
                "حالة الطلب يجب أن تتغير إلى Completed");
        });
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  سيناريو 5: رفض الصرف المزدوج
    // ══════════════════════════════════════════════════════════════════════════
    [Fact]
    public async Task DispenseMedication_WhenAlreadyDispensed_ShouldThrowUserFriendlyException()
    {
        Guid productId = Guid.NewGuid();
        Guid drugId = Guid.NewGuid();
        Guid itemId    = Guid.Empty;
        Guid orderId   = Guid.Empty;
                Guid whId      = Guid.Empty;

        await WithUnitOfWorkAsync(async () =>
        {
            await EnsureAccountMappingsAreFilledAsync();
            whId = await GetOrCreatePharmacyWarehouseIdAsync();

            var serviceItem = new ServiceItem(productId, "M001", "Panadol", ServiceCategory.Pharmacy) { Price = 10m };
            await GetRequiredService<IRepository<ServiceItem, Guid>>().InsertAsync(serviceItem);

            var drug = new Drug(drugId, "123", "Med", "Med", "500", "Tab", "Manuf") { ServiceItemId = productId };
            await GetRequiredService<IRepository<Drug, Guid>>().InsertAsync(drug);

            var item = new InventoryItem(Guid.NewGuid(), whId, productId,
                "Ibuprofen 400mg", InventoryItemType.Medication, 200m, 6m);
            await _inventoryItemRepository.InsertAsync(item);
            itemId = item.Id;

            await _batchRepository.InsertAsync(new InventoryBatch(
                Guid.NewGuid(), itemId, "B-IBU-01", 200m, 6m,
                DateTime.Now.AddDays(-1), "PO-IBU-01"));

            var pt = new Patient(Guid.NewGuid(), null, "TST-IB1",
                "مريض", "هـ", new DateTime(1995, 2, 10), Gender.Female,
                IdentityType.NationalId, Guid.NewGuid().ToString("N")[..10], "0500000005");
            await _patientRepository.InsertAsync(pt);

            var ord = new MedicalOrder(Guid.NewGuid(), pt.Id,
                OrderType.Medication, productId, "Ibuprofen 400mg", 30m)
                { Quantity = 2m };
            await _medicalOrderRepository.InsertAsync(ord);
            orderId = ord.Id;
        });

        await WithUnitOfWorkAsync(async () =>
        {
            await _pharmacyAppService.DispenseMedicationAsync(
                new DispenseDto { MedicalOrderId = orderId, WarehouseId = whId });
        });

        await Should.ThrowAsync<UserFriendlyException>(async () =>
        {
            await WithUnitOfWorkAsync(async () =>
            {
                await _pharmacyAppService.DispenseMedicationAsync(
                    new DispenseDto { MedicalOrderId = orderId, WarehouseId = whId });
            });
        });
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  سيناريو 6: رفض الصرف عند نقص المخزون
    // ══════════════════════════════════════════════════════════════════════════
    [Fact]
    public async Task DispenseMedication_WhenInsufficientStock_ShouldThrowBusinessException()
    {
        Guid productId = Guid.NewGuid();
        Guid drugId = Guid.NewGuid();
        Guid itemId    = Guid.Empty;
        Guid orderId   = Guid.Empty;
                Guid whId      = Guid.Empty;

        await WithUnitOfWorkAsync(async () =>
        {
            whId = await GetOrCreatePharmacyWarehouseIdAsync();

            var serviceItem = new ServiceItem(productId, "M001", "Panadol", ServiceCategory.Pharmacy) { Price = 10m };
            await GetRequiredService<IRepository<ServiceItem, Guid>>().InsertAsync(serviceItem);

            var drug = new Drug(drugId, "123", "Med", "Med", "500", "Tab", "Manuf") { ServiceItemId = productId };
            await GetRequiredService<IRepository<Drug, Guid>>().InsertAsync(drug);

            var item = new InventoryItem(Guid.NewGuid(), whId, productId,
                "Insulin Pen", InventoryItemType.Medication, 5m, 50m);
            await _inventoryItemRepository.InsertAsync(item);
            itemId = item.Id;

            await _batchRepository.InsertAsync(new InventoryBatch(
                Guid.NewGuid(), itemId, "B-INS-01", 5m, 50m,
                DateTime.Now.AddDays(-2), "PO-INS-01"));

            var pt = new Patient(Guid.NewGuid(), null, "TST-IN1",
                "مريض", "و", new DateTime(1960, 11, 5), Gender.Male,
                IdentityType.NationalId, Guid.NewGuid().ToString("N")[..10], "0500000006");
            await _patientRepository.InsertAsync(pt);

            var ord = new MedicalOrder(Guid.NewGuid(), pt.Id,
                OrderType.Medication, productId, "Insulin Pen", 250m)
                { Quantity = 20m }; // أكثر من المتاح (5)
            await _medicalOrderRepository.InsertAsync(ord);
            orderId = ord.Id;
        });

        await Should.ThrowAsync<Volo.Abp.BusinessException>(async () =>
        {
            await WithUnitOfWorkAsync(async () =>
            {
                await _pharmacyAppService.DispenseMedicationAsync(
                    new DispenseDto { MedicalOrderId = orderId, WarehouseId = whId });
            });
        });
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  سيناريو 7: خصم صحيح من الدفعة (Batch)
    // ══════════════════════════════════════════════════════════════════════════
    [Fact]
    public async Task DispenseMedication_ShouldDeductFromBatch_Correctly()
    {
        Guid productId = Guid.NewGuid();
        Guid drugId = Guid.NewGuid();
        Guid itemId    = Guid.Empty;
        Guid batchId   = Guid.Empty;
        Guid orderId   = Guid.Empty;
                Guid whId      = Guid.Empty;
        const decimal batchQty    = 100m;
        const decimal dispenseQty = 30m;

        await WithUnitOfWorkAsync(async () =>
        {
            await EnsureAccountMappingsAreFilledAsync();
            whId = await GetOrCreatePharmacyWarehouseIdAsync();

            var serviceItem = new ServiceItem(productId, "M001", "Panadol", ServiceCategory.Pharmacy) { Price = 10m };
            await GetRequiredService<IRepository<ServiceItem, Guid>>().InsertAsync(serviceItem);

            var drug = new Drug(drugId, "123", "Med", "Med", "500", "Tab", "Manuf") { ServiceItemId = productId };
            await GetRequiredService<IRepository<Drug, Guid>>().InsertAsync(drug);

            var item = new InventoryItem(Guid.NewGuid(), whId, productId,
                "Metformin 500mg", InventoryItemType.Medication, batchQty, 8m);
            await _inventoryItemRepository.InsertAsync(item);
            itemId = item.Id;

            var batch = new InventoryBatch(Guid.NewGuid(), itemId, "B-MET-01",
                batchQty, 8m, DateTime.Now.AddDays(-10), "PO-MET-01");
            await _batchRepository.InsertAsync(batch);
            batchId = batch.Id;

            var pt = new Patient(Guid.NewGuid(), null, "TST-ME1",
                "مريض", "ز", new DateTime(1970, 7, 7), Gender.Male,
                IdentityType.NationalId, Guid.NewGuid().ToString("N")[..10], "0500000007");
            await _patientRepository.InsertAsync(pt);

            var ord = new MedicalOrder(Guid.NewGuid(), pt.Id,
                OrderType.Medication, productId, "Metformin 500mg", 40m)
                { Quantity = dispenseQty };
            await _medicalOrderRepository.InsertAsync(ord);
            orderId = ord.Id;
        });

        await WithUnitOfWorkAsync(async () =>
        {
            await _pharmacyAppService.DispenseMedicationAsync(
                new DispenseDto { MedicalOrderId = orderId, WarehouseId = whId });
        });

        await WithUnitOfWorkAsync(async () =>
        {
            var updatedItem  = await _inventoryItemRepository.GetAsync(itemId);
            var updatedBatch = await _batchRepository.GetAsync(batchId);
            updatedItem.Quantity.ShouldBe(batchQty - dispenseQty, "كمية الصنف الإجمالية يجب أن تنقص");
            updatedBatch.Quantity.ShouldBe(batchQty - dispenseQty, "كمية الدفعة يجب أن تنقص");
        });
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  سيناريو 8: LIFO — الأحدث يُستهلك أولاً
    // ══════════════════════════════════════════════════════════════════════════
    [Fact]
    public async Task DispenseMedication_MultipleBatches_ShouldFollowLIFO_Order()
    {
        Guid productId    = Guid.NewGuid();
        Guid itemId       = Guid.Empty;
        Guid olderBatchId = Guid.Empty;
        Guid newerBatchId = Guid.Empty;
        Guid orderId      = Guid.Empty;
        Guid whId         = Guid.Empty;
        Guid drugId = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            await EnsureAccountMappingsAreFilledAsync();
            whId = await GetOrCreatePharmacyWarehouseIdAsync();

            var serviceItem = new ServiceItem(productId, "M001", "Panadol", ServiceCategory.Pharmacy) { Price = 10m };
            await GetRequiredService<IRepository<ServiceItem, Guid>>().InsertAsync(serviceItem);

            var drug = new Drug(drugId, "123", "Med", "Med", "500", "Tab", "Manuf") { ServiceItemId = productId };
            await GetRequiredService<IRepository<Drug, Guid>>().InsertAsync(drug);

            var item = new InventoryItem(Guid.NewGuid(), whId, productId,
                "Atorvastatin 20mg", InventoryItemType.Medication, 15m, 12m);
            await _inventoryItemRepository.InsertAsync(item);
            itemId = item.Id;

            var older = new InventoryBatch(Guid.NewGuid(), itemId, "BATCH-OLD",
                10m, 12m, DateTime.Now.AddDays(-20), "PO-OLD");
            await _batchRepository.InsertAsync(older);
            olderBatchId = older.Id;

            var newer = new InventoryBatch(Guid.NewGuid(), itemId, "BATCH-NEW",
                5m, 13m, DateTime.Now.AddDays(-2), "PO-NEW");
            await _batchRepository.InsertAsync(newer);
            newerBatchId = newer.Id;

            var pt = new Patient(Guid.NewGuid(), null, "TST-AT1",
                "مريض", "ح", new DateTime(1980, 4, 12), Gender.Male,
                IdentityType.NationalId, Guid.NewGuid().ToString("N")[..10], "0500000008");
            await _patientRepository.InsertAsync(pt);

            var ord = new MedicalOrder(Guid.NewGuid(), pt.Id,
                OrderType.Medication, productId, "Atorvastatin 20mg", 60m)
                { Quantity = 12m };
            await _medicalOrderRepository.InsertAsync(ord);
            orderId = ord.Id;
        });

        await WithUnitOfWorkAsync(async () =>
        {
            await _pharmacyAppService.DispenseMedicationAsync(
                new DispenseDto { MedicalOrderId = orderId, WarehouseId = whId });
        });

        await WithUnitOfWorkAsync(async () =>
        {
            var updatedItem       = await _inventoryItemRepository.GetAsync(itemId);
            var updatedNewerBatch = await _batchRepository.GetAsync(newerBatchId);
            var updatedOlderBatch = await _batchRepository.GetAsync(olderBatchId);
            updatedItem.Quantity.ShouldBe(3m,       "الإجمالي: 15 - 12 = 3");
            updatedNewerBatch.Quantity.ShouldBe(0m, "الدفعة الأحدث تُستنفد أولاً (LIFO)");
            updatedOlderBatch.Quantity.ShouldBe(3m, "الدفعة القديمة: 10 - 7 = 3");
        });
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  سيناريو 9: GetPendingPrescriptions يُرجع المعلقة فقط
    // ══════════════════════════════════════════════════════════════════════════
    [Fact]
    public async Task GetPendingPrescriptions_ShouldReturnOnly_PendingMedicationOrders()
    {
        Guid pendingOrderId   = Guid.Empty;
        Guid completedOrderId = Guid.Empty;
        Guid drugId = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            var pt = new Patient(Guid.NewGuid(), null, "TST-PN1",
                "مريض", "ط", new DateTime(1992, 8, 18), Gender.Male,
                IdentityType.NationalId, Guid.NewGuid().ToString("N")[..10], "0500000009");
            await _patientRepository.InsertAsync(pt);

            var pending = new MedicalOrder(Guid.NewGuid(), pt.Id,
                OrderType.Medication, Guid.NewGuid(), "Cough Syrup", 15m)
                { Quantity = 1m };
            await _medicalOrderRepository.InsertAsync(pending);
            pendingOrderId = pending.Id;

            var completed = new MedicalOrder(Guid.NewGuid(), pt.Id,
                OrderType.Medication, Guid.NewGuid(), "Old Medicine", 30m)
                { Quantity = 7m, Status = OrderStatus.Completed };
            await _medicalOrderRepository.InsertAsync(completed);
            completedOrderId = completed.Id;
        });

        await WithUnitOfWorkAsync(async () =>
        {
            var result = await _pharmacyAppService.GetPendingPrescriptionsAsync();
            result.ShouldContain(p => p.Id == pendingOrderId,
                "الوصفة المعلقة يجب أن تظهر");
            result.ShouldNotContain(p => p.Id == completedOrderId,
                "الوصفة المكتملة يجب ألا تظهر");
        });
    }
        [Fact]
    public async Task GetPharmacyStockAsync_WithSpecificWarehouseId_ShouldReturnItems()
    {
        Guid testWarehouseId = Guid.NewGuid();
        Guid drugId = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            var warehouse = new Warehouse(testWarehouseId, "Test Pharmacy Warehouse", "Location", "CODE");
            await _warehouseRepository.InsertAsync(warehouse);

            var invItem = new InventoryItem(Guid.NewGuid(), testWarehouseId, drugId, "TEST", InventoryItemType.Medication, 50, 10);
            await _inventoryItemRepository.InsertAsync(invItem);
        });

        await WithUnitOfWorkAsync(async () =>
        {
            var stock = await _pharmacyAppService.GetPharmacyStockAsync(testWarehouseId);
            stock.ShouldNotBeNull();
            stock.Count.ShouldBe(1);
            stock[0].Quantity.ShouldBe(50);
        });
    }
}


