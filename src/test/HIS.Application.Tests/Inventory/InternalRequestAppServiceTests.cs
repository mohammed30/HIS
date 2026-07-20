using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HIS.Accounting;
using HIS.Inventory;
using HIS.Inventory.Dtos;
using HIS.Services;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;
using Xunit;
using HIS.Settings;
using HIS.Patients;
using HIS.Inpatient;

namespace HIS.Inventory.Tests;

public abstract class InternalRequestAppServiceTests<TStartupModule> : HISTestBase<TStartupModule>
    where TStartupModule : Volo.Abp.Modularity.IAbpModule
{
    private readonly IInternalRequestAppService _internalRequestAppService;
    private readonly IRepository<Warehouse, Guid> _warehouseRepository;
    private readonly IRepository<InventoryItem, Guid> _inventoryItemRepository;
    private readonly IRepository<InventoryBatch, Guid> _batchRepository;
    private readonly IRepository<ServiceItem, Guid> _serviceItemRepository;
    private readonly IRepository<InternalRequest, Guid> _internalRequestRepository;
    private readonly IRepository<Account, Guid> _accountRepository;
    private readonly IRepository<AccountMapping, Guid> _accountMappingRepository;
    private readonly IRepository<Department, Guid> _departmentRepository;
    private readonly IRepository<Patient, Guid> _patientRepository;
    private readonly IRepository<Admission, Guid> _admissionRepository;

    protected InternalRequestAppServiceTests()
    {
        _internalRequestAppService = GetRequiredService<IInternalRequestAppService>();
        _warehouseRepository = GetRequiredService<IRepository<Warehouse, Guid>>();
        _inventoryItemRepository = GetRequiredService<IRepository<InventoryItem, Guid>>();
        _batchRepository = GetRequiredService<IRepository<InventoryBatch, Guid>>();
        _serviceItemRepository = GetRequiredService<IRepository<ServiceItem, Guid>>();
        _internalRequestRepository = GetRequiredService<IRepository<InternalRequest, Guid>>();
        _accountRepository = GetRequiredService<IRepository<Account, Guid>>();
        _accountMappingRepository = GetRequiredService<IRepository<AccountMapping, Guid>>();
        _departmentRepository = GetRequiredService<IRepository<Department, Guid>>();
        _patientRepository = GetRequiredService<IRepository<Patient, Guid>>();
        _admissionRepository = GetRequiredService<IRepository<Admission, Guid>>();
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
    public async Task CreateAsync_Should_Create_InternalRequest_Draft()
    {
        Guid reqDeptId = Guid.NewGuid();
        Guid srcWhId = Guid.NewGuid();
        Guid serviceItemId = Guid.NewGuid();
        Guid invItemId = Guid.NewGuid();
        Guid patientId = Guid.NewGuid();
        Guid admissionId = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            await _departmentRepository.InsertAsync(new Department(reqDeptId, null, "DEP01", "Pharmacy Dep"));
            await _warehouseRepository.InsertAsync(new Warehouse(srcWhId, "Main Store", "Location", "MAIN"));
            
            var pt = new Patient(patientId, null, "PT-01", "Ahmed", "Ali", new DateTime(1990, 1, 1), Gender.Male, IdentityType.NationalId, "123", "050");
            await _patientRepository.InsertAsync(pt);
            
            var admission = new Admission(admissionId, null, patientId, Guid.NewGuid(), Guid.NewGuid());
            await _admissionRepository.InsertAsync(admission);

            var sItem = new ServiceItem(serviceItemId, "S001", "Gauze", ServiceCategory.Pharmacy);
            await _serviceItemRepository.InsertAsync(sItem);

            var item = new InventoryItem(invItemId, srcWhId, serviceItemId, "Gauze Roll", InventoryItemType.Consumable, 100m, 5m);
            await _inventoryItemRepository.InsertAsync(item);
        });

        InternalRequestDto result = null;

        await WithUnitOfWorkAsync(async () =>
        {
            var dto = new CreateUpdateInternalRequestDto
            {
                RequestingDepartmentId = reqDeptId,
                FulfilledByWarehouseId = srcWhId,
                AdmissionId = admissionId,
                RequestType = InternalRequestType.Consumable,
                Notes = "Need supplies",
                RequestDate = DateTime.Now,
                Lines = new List<CreateUpdateInternalRequestLineDto>
                {
                    new CreateUpdateInternalRequestLineDto
                    {
                        InventoryItemId = invItemId,
                        RequestedQuantity = 10m
                    }
                }
            };
            result = await _internalRequestAppService.CreateAsync(dto);
        });

        result.ShouldNotBeNull();
        result.Status.ShouldBe(InternalRequestStatus.Draft);
        result.Lines.Count.ShouldBe(1);
        result.Lines[0].RequestedQuantity.ShouldBe(10m);
    }

    [Fact]
    public async Task SubmitRequestAsync_Should_Change_Status_To_Submitted()
    {
        Guid reqDeptId = Guid.NewGuid();
        Guid srcWhId = Guid.NewGuid();
        Guid serviceItemId = Guid.NewGuid();
        Guid invItemId = Guid.NewGuid();
        Guid requestId = Guid.NewGuid();
        Guid patientId = Guid.NewGuid();
        Guid admissionId = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            await _departmentRepository.InsertAsync(new Department(reqDeptId, null, "DEP01", "Pharmacy Dep"));
            await _warehouseRepository.InsertAsync(new Warehouse(srcWhId, "Main Store", "Location", "MAIN"));
            
            var pt = new Patient(patientId, null, "PT-01", "Ahmed", "Ali", new DateTime(1990, 1, 1), Gender.Male, IdentityType.NationalId, "123", "050");
            await _patientRepository.InsertAsync(pt);
            
            var admission = new Admission(admissionId, null, patientId, Guid.NewGuid(), Guid.NewGuid());
            await _admissionRepository.InsertAsync(admission);

            var req = new InternalRequest(requestId, "REQ-01", reqDeptId, srcWhId, DateTime.Now) { AdmissionId = admissionId };
            await _internalRequestRepository.InsertAsync(req);
        });

        await WithUnitOfWorkAsync(async () =>
        {
            var res = await _internalRequestAppService.SubmitRequestAsync(requestId);
            res.Status.ShouldBe(InternalRequestStatus.Submitted);
        });

        await WithUnitOfWorkAsync(async () =>
        {
            var updated = await _internalRequestRepository.GetAsync(requestId);
            updated.Status.ShouldBe(InternalRequestStatus.Submitted);
        });
    }

    [Fact]
    public async Task ApproveAndFulfillAsync_Should_Fulfill_And_Deduct_SourceStock()
    {
        Guid reqDeptId = Guid.NewGuid();
        Guid srcWhId = Guid.NewGuid();
        Guid serviceItemId = Guid.NewGuid();
        Guid invItemId = Guid.NewGuid();
        Guid requestId = Guid.NewGuid();
        Guid patientId = Guid.NewGuid();
        Guid admissionId = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            await EnsureAccountMappingsAreFilledAsync();

            await _departmentRepository.InsertAsync(new Department(reqDeptId, null, "DEP01", "Pharmacy Dep"));
            await _warehouseRepository.InsertAsync(new Warehouse(srcWhId, "Main Store", "Location", "MAIN"));
            
            var pt = new Patient(patientId, null, "PT-01", "Ahmed", "Ali", new DateTime(1990, 1, 1), Gender.Male, IdentityType.NationalId, "123", "050");
            await _patientRepository.InsertAsync(pt);
            
            var admission = new Admission(admissionId, null, patientId, Guid.NewGuid(), Guid.NewGuid());
            await _admissionRepository.InsertAsync(admission);
            
            var sItem = new ServiceItem(serviceItemId, "S001", "Gauze", ServiceCategory.Pharmacy);
            await _serviceItemRepository.InsertAsync(sItem);

            var item = new InventoryItem(invItemId, srcWhId, serviceItemId, "Gauze Roll", InventoryItemType.Consumable, 100m, 5m);
            await _inventoryItemRepository.InsertAsync(item);

            await _batchRepository.InsertAsync(new InventoryBatch(Guid.NewGuid(), invItemId, "B-001", 100m, 5m, DateTime.Now.AddDays(-10), "PO-001"));

            var req = new InternalRequest(requestId, "REQ-01", reqDeptId, srcWhId, DateTime.Now) { AdmissionId = admissionId };
            req.Status = InternalRequestStatus.Submitted;
            req.RequestType = InternalRequestType.Consumable;
            
            var line = new InternalRequestLine(Guid.NewGuid(), requestId, invItemId, 10m);
            req.Lines = new List<InternalRequestLine> { line };
            
            await _internalRequestRepository.InsertAsync(req);
        });

        await WithUnitOfWorkAsync(async () =>
        {
            var res = await _internalRequestAppService.ApproveAndFulfillAsync(requestId);
            res.Status.ShouldBe(InternalRequestStatus.Approved);
        });

        await WithUnitOfWorkAsync(async () =>
        {
            var updatedReq = await _internalRequestRepository.GetAsync(requestId);
            updatedReq.Status.ShouldBe(InternalRequestStatus.Approved);

            var updatedItem = await _inventoryItemRepository.GetAsync(invItemId);
            updatedItem.Quantity.ShouldBe(90m); // 100 - 10
        });
    }

    [Fact]
    public async Task ConfirmReceiptAsync_Should_Finalize_And_Transfer_Stock()
    {
        Guid reqDeptId = Guid.NewGuid();
        Guid srcWhId = Guid.NewGuid();
        Guid serviceItemId = Guid.NewGuid();
        Guid srcInvItemId = Guid.NewGuid();
        Guid destInvItemId = Guid.NewGuid();
        Guid requestId = Guid.NewGuid();
        Guid patientId = Guid.NewGuid();
        Guid admissionId = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            await EnsureAccountMappingsAreFilledAsync();

            await _departmentRepository.InsertAsync(new Department(reqDeptId, null, "DEP01", "Pharmacy Dep"));
            await _warehouseRepository.InsertAsync(new Warehouse(srcWhId, "Main Store", "Location", "MAIN"));
            
            var pt = new Patient(patientId, null, "PT-01", "Ahmed", "Ali", new DateTime(1990, 1, 1), Gender.Male, IdentityType.NationalId, "123", "050");
            await _patientRepository.InsertAsync(pt);
            
            var admission = new Admission(admissionId, null, patientId, Guid.NewGuid(), Guid.NewGuid());
            await _admissionRepository.InsertAsync(admission);
            
            var sItem = new ServiceItem(serviceItemId, "S001", "Gauze", ServiceCategory.Pharmacy);
            await _serviceItemRepository.InsertAsync(sItem);

            // Source Item
            var srcItem = new InventoryItem(srcInvItemId, srcWhId, serviceItemId, "Gauze Roll", InventoryItemType.Consumable, 90m, 5m);
            await _inventoryItemRepository.InsertAsync(srcItem);

            var req = new InternalRequest(requestId, "REQ-01", reqDeptId, srcWhId, DateTime.Now) { AdmissionId = admissionId };
            req.Status = InternalRequestStatus.Approved; // Already fulfilled
            req.RequestType = InternalRequestType.Consumable;
            
            var line = new InternalRequestLine(Guid.NewGuid(), requestId, srcInvItemId, 10m);
            line.ApprovedQuantity = 10m; // Was approved during fulfillment
            req.Lines = new List<InternalRequestLine> { line };
            
            await _internalRequestRepository.InsertAsync(req);
        });

        await WithUnitOfWorkAsync(async () =>
        {
            var res = await _internalRequestAppService.ConfirmReceiptAsync(requestId);
            res.Status.ShouldBe(InternalRequestStatus.Received);
        });

        await WithUnitOfWorkAsync(async () =>
        {
            var updatedReq = await _internalRequestRepository.GetAsync(requestId);
            updatedReq.Status.ShouldBe(InternalRequestStatus.Received);
        });
    }
}
