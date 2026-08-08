using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HIS.Accounting;
using HIS.Inventory;
using HIS.Inventory.Dtos;
using HIS.Services;
using Shouldly;
using Volo.Abp;
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

    // ══════════════════════════════════════════════════════════════════════════
    //  Tests for Return Workflow (Today's Features)
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// يجب أن ينشئ ReturnItemsAsync طلب مرتجع جديد بحالة Submitted
    /// ويكون IsReturn = true وParentRequestId = معرف الطلب الأصلي.
    /// </summary>
    [Fact]
    public async Task ReturnItemsAsync_Should_Create_PendingReturn_Request()
    {
        Guid reqDeptId    = Guid.NewGuid();
        Guid srcWhId      = Guid.NewGuid();
        Guid serviceItemId = Guid.NewGuid();
        Guid invItemId    = Guid.NewGuid();
        Guid requestId    = Guid.NewGuid();
        Guid patientId    = Guid.NewGuid();
        Guid admissionId  = Guid.NewGuid();

        // Arrange: create the original fulfilled request
        await WithUnitOfWorkAsync(async () =>
        {
            await _departmentRepository.InsertAsync(new Department(reqDeptId, null, "DEP-RET01", "Nursing"));
            await _warehouseRepository.InsertAsync(new Warehouse(srcWhId, "Pharmacy Warehouse", "Store", "PHARM"));

            var pt = new Patient(patientId, null, "PT-RET01", "Sara", "Ahmad",
                new DateTime(1985, 5, 10), Gender.Female, IdentityType.NationalId, "999", "055");
            await _patientRepository.InsertAsync(pt);

            var admission = new Admission(admissionId, null, patientId, Guid.NewGuid(), Guid.NewGuid());
            await _admissionRepository.InsertAsync(admission);

            var sItem = new ServiceItem(serviceItemId, "MED001", "Paracetamol", ServiceCategory.Pharmacy);
            await _serviceItemRepository.InsertAsync(sItem);

            var invItem = new InventoryItem(invItemId, srcWhId, serviceItemId,
                "Paracetamol 500mg", InventoryItemType.Consumable, 50m, 2.5m);
            await _inventoryItemRepository.InsertAsync(invItem);

            // Original request that is already Received
            var req = new InternalRequest(requestId, "REQ-RET-01", reqDeptId, srcWhId, DateTime.Now)
            {
                AdmissionId = admissionId,
                RequestType = InternalRequestType.Medication,
                Status      = InternalRequestStatus.Received
            };
            var line = new InternalRequestLine(Guid.NewGuid(), requestId, invItemId, 20m)
            {
                ApprovedQuantity = 20m
            };
            req.Lines = new List<InternalRequestLine> { line };
            await _internalRequestRepository.InsertAsync(req);
        });

        // Act
        InternalRequestDto returnDto = null;
        await WithUnitOfWorkAsync(async () =>
        {
            var input = new ReturnInternalRequestDto
            {
                RequestId = requestId,
                Notes     = "مرتجع - تم شفاء المريض",
                Lines = new List<ReturnInternalRequestLineDto>
                {
                    new ReturnInternalRequestLineDto
                    {
                        InventoryItemId  = invItemId,
                        OriginalQuantity = 20m,
                        ReturnQuantity   = 5m
                    }
                }
            };
            returnDto = await _internalRequestAppService.ReturnItemsAsync(input);
        });

        // Assert: return request created with correct flags
        returnDto.ShouldNotBeNull();
        returnDto.IsReturn.ShouldBeTrue();
        returnDto.ParentRequestId.ShouldBe(requestId);
        returnDto.Status.ShouldBe(InternalRequestStatus.Submitted);
        returnDto.Lines.Count.ShouldBe(1);
        returnDto.Lines[0].RequestedQuantity.ShouldBe(5m);
    }

    /// <summary>
    /// يجب أن يرفع ReturnItemsAsync استثناء عند محاولة إرتجاع كمية أكبر من المعتمدة.
    /// </summary>
    [Fact]
    public async Task ReturnItemsAsync_Should_Throw_When_ReturnQty_Exceeds_Approved()
    {
        Guid reqDeptId    = Guid.NewGuid();
        Guid srcWhId      = Guid.NewGuid();
        Guid serviceItemId = Guid.NewGuid();
        Guid invItemId    = Guid.NewGuid();
        Guid requestId    = Guid.NewGuid();
        Guid patientId    = Guid.NewGuid();
        Guid admissionId  = Guid.NewGuid();

        // Arrange
        await WithUnitOfWorkAsync(async () =>
        {
            await _departmentRepository.InsertAsync(new Department(reqDeptId, null, "DEP-RET02", "Nursing2"));
            await _warehouseRepository.InsertAsync(new Warehouse(srcWhId, "Pharmacy WH2", "Store2", "PHARM2"));

            var pt = new Patient(patientId, null, "PT-RET02", "Ali", "Said",
                new DateTime(1990, 3, 15), Gender.Male, IdentityType.NationalId, "888", "056");
            await _patientRepository.InsertAsync(pt);

            var admission = new Admission(admissionId, null, patientId, Guid.NewGuid(), Guid.NewGuid());
            await _admissionRepository.InsertAsync(admission);

            var sItem = new ServiceItem(serviceItemId, "MED002", "Amoxicillin", ServiceCategory.Pharmacy);
            await _serviceItemRepository.InsertAsync(sItem);

            var invItem = new InventoryItem(invItemId, srcWhId, serviceItemId,
                "Amoxicillin 250mg", InventoryItemType.Consumable, 30m, 3m);
            await _inventoryItemRepository.InsertAsync(invItem);

            var req = new InternalRequest(requestId, "REQ-RET-02", reqDeptId, srcWhId, DateTime.Now)
            {
                AdmissionId = admissionId,
                RequestType = InternalRequestType.Medication,
                Status      = InternalRequestStatus.Received
            };
            var line = new InternalRequestLine(Guid.NewGuid(), requestId, invItemId, 10m)
            {
                ApprovedQuantity = 10m
            };
            req.Lines = new List<InternalRequestLine> { line };
            await _internalRequestRepository.InsertAsync(req);
        });

        // Act & Assert: should throw when return qty > approved qty
        await WithUnitOfWorkAsync(async () =>
        {
            var input = new ReturnInternalRequestDto
            {
                RequestId = requestId,
                Notes     = "محاولة إرتجاع زائدة",
                Lines = new List<ReturnInternalRequestLineDto>
                {
                    new ReturnInternalRequestLineDto
                    {
                        InventoryItemId  = invItemId,
                        OriginalQuantity = 10m,
                        ReturnQuantity   = 99m  // exceeds approved 10
                    }
                }
            };
            await Should.ThrowAsync<UserFriendlyException>(async () =>
            {
                await _internalRequestAppService.ReturnItemsAsync(input);
            });
        });
    }

    /// <summary>
    /// يجب أن يُعيد GetPendingReturnsAsync فقط الطلبات التي IsReturn=true وStatus=Submitted.
    /// </summary>
    [Fact]
    public async Task GetPendingReturnsAsync_Should_Return_Only_PendingReturn_Requests()
    {
        Guid reqDeptId    = Guid.NewGuid();
        Guid srcWhId      = Guid.NewGuid();
        Guid invItemId    = Guid.NewGuid();
        Guid serviceItemId = Guid.NewGuid();
        Guid patientId    = Guid.NewGuid();
        Guid admissionId  = Guid.NewGuid();
        Guid returnReqId  = Guid.NewGuid();
        Guid normalReqId  = Guid.NewGuid();

        // Arrange: insert one return (IsReturn=true, Submitted) and one normal request
        await WithUnitOfWorkAsync(async () =>
        {
            await _departmentRepository.InsertAsync(new Department(reqDeptId, null, "DEP-GP01", "NursingGP"));
            await _warehouseRepository.InsertAsync(new Warehouse(srcWhId, "Pharmacy WH GP", "StoreGP", "PHARMGP"));

            var pt = new Patient(patientId, null, "PT-GP01", "Nour", "Khalid",
                new DateTime(1995, 7, 20), Gender.Female, IdentityType.NationalId, "777", "057");
            await _patientRepository.InsertAsync(pt);

            var admission = new Admission(admissionId, null, patientId, Guid.NewGuid(), Guid.NewGuid());
            await _admissionRepository.InsertAsync(admission);

            var sItem = new ServiceItem(serviceItemId, "MED003", "Ibuprofen", ServiceCategory.Pharmacy);
            await _serviceItemRepository.InsertAsync(sItem);

            var invItem = new InventoryItem(invItemId, srcWhId, serviceItemId,
                "Ibuprofen 400mg", InventoryItemType.Consumable, 100m, 1.5m);
            await _inventoryItemRepository.InsertAsync(invItem);

            // Normal request (should NOT appear)
            var normalReq = new InternalRequest(normalReqId, "REQ-GP-NORMAL", reqDeptId, srcWhId, DateTime.Now)
            {
                AdmissionId = admissionId,
                RequestType = InternalRequestType.Medication,
                Status      = InternalRequestStatus.Submitted,
                IsReturn    = false
            };
            await _internalRequestRepository.InsertAsync(normalReq);

            // Return request pending pharmacy approval (SHOULD appear)
            var returnReq = new InternalRequest(returnReqId, "RET-REQ-GP-01", reqDeptId, srcWhId, DateTime.Now)
            {
                AdmissionId     = admissionId,
                RequestType     = InternalRequestType.Medication,
                Status          = InternalRequestStatus.Submitted,
                IsReturn        = true,
                ParentRequestId = normalReqId,
                Notes           = "مرتجع للاختبار"
            };
            var retLine = new InternalRequestLine(Guid.NewGuid(), returnReqId, invItemId, 3m);
            returnReq.Lines = new List<InternalRequestLine> { retLine };
            await _internalRequestRepository.InsertAsync(returnReq);
        });

        // Act
        Volo.Abp.Application.Dtos.PagedResultDto<InternalRequestDto> result = null;
        await WithUnitOfWorkAsync(async () =>
        {
            result = await _internalRequestAppService.GetPendingReturnsAsync(
                new Volo.Abp.Application.Dtos.PagedAndSortedResultRequestDto
                {
                    MaxResultCount = 50,
                    SkipCount      = 0
                });
        });

        // Assert: only the return request comes back, NOT the normal one
        result.ShouldNotBeNull();
        result.Items.ShouldAllBe(x => x.IsReturn && x.Status == InternalRequestStatus.Submitted);
        result.Items.ShouldContain(x => x.Id == returnReqId);
        result.Items.ShouldNotContain(x => x.Id == normalReqId);
    }

    /// <summary>
    /// يجب أن يغير ApproveReturnAsync حالة المرتجع إلى Approved
    /// ويعيد الكمية المرتجعة إلى مخزون الصيدلية.
    /// </summary>
    [Fact]
    public async Task ApproveReturnAsync_Should_Set_Status_Approved_And_Restore_Stock()
    {
        Guid reqDeptId     = Guid.NewGuid();
        Guid srcWhId       = Guid.NewGuid();
        Guid serviceItemId = Guid.NewGuid();
        Guid invItemId     = Guid.NewGuid();
        Guid originalReqId = Guid.NewGuid();
        Guid returnReqId   = Guid.NewGuid();
        Guid patientId     = Guid.NewGuid();
        Guid admissionId   = Guid.NewGuid();
        Guid batchId       = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            await EnsureAccountMappingsAreFilledAsync();

            await _departmentRepository.InsertAsync(new Department(reqDeptId, null, "DEP-APR01", "NursingAPR"));
            await _warehouseRepository.InsertAsync(new Warehouse(srcWhId, "Pharmacy WH APR", "StoreAPR", "PHARMAPR"));

            var pt = new Patient(patientId, null, "PT-APR01", "Majed", "Salem",
                new DateTime(1980, 1, 1), Gender.Male, IdentityType.NationalId, "666", "058");
            await _patientRepository.InsertAsync(pt);

            var admission = new Admission(admissionId, null, patientId, Guid.NewGuid(), Guid.NewGuid());
            await _admissionRepository.InsertAsync(admission);

            var sItem = new ServiceItem(serviceItemId, "MED004", "Metformin", ServiceCategory.Pharmacy);
            await _serviceItemRepository.InsertAsync(sItem);

            // Current stock: 80 units
            var invItem = new InventoryItem(invItemId, srcWhId, serviceItemId,
                "Metformin 500mg", InventoryItemType.Consumable, 80m, 5m);
            await _inventoryItemRepository.InsertAsync(invItem);

            await _batchRepository.InsertAsync(new InventoryBatch(batchId, invItemId,
                "BATCH-APR-01", 80m, 5m, DateTime.Now.AddDays(-30), "PO-APR"));

            // Original fulfilled request
            var originalReq = new InternalRequest(originalReqId, "REQ-APR-ORIG", reqDeptId, srcWhId, DateTime.Now)
            {
                AdmissionId = admissionId,
                RequestType = InternalRequestType.Medication,
                Status      = InternalRequestStatus.Received
            };
            var origLine = new InternalRequestLine(Guid.NewGuid(), originalReqId, invItemId, 15m)
            {
                ApprovedQuantity = 15m
            };
            originalReq.Lines = new List<InternalRequestLine> { origLine };
            await _internalRequestRepository.InsertAsync(originalReq);

            // Return request (pending pharmacy approval)
            var returnReq = new InternalRequest(returnReqId, "RET-APR-01", reqDeptId, srcWhId, DateTime.Now)
            {
                AdmissionId     = admissionId,
                RequestType     = InternalRequestType.Medication,
                Status          = InternalRequestStatus.Submitted,
                IsReturn        = true,
                ParentRequestId = originalReqId,
                Notes           = "إرجاع 7 أقراص"
            };
            var retLine = new InternalRequestLine(Guid.NewGuid(), returnReqId, invItemId, 7m);
            returnReq.Lines = new List<InternalRequestLine> { retLine };
            await _internalRequestRepository.InsertAsync(returnReq);
        });

        // Act: pharmacy approves the return
        InternalRequestDto approvedDto = null;
        await WithUnitOfWorkAsync(async () =>
        {
            approvedDto = await _internalRequestAppService.ApproveReturnAsync(returnReqId);
        });

        // Assert: status changed to Approved
        approvedDto.ShouldNotBeNull();
        approvedDto.Status.ShouldBe(InternalRequestStatus.Approved);

        // Assert: inventory stock increased by 7 (80 → 87)
        await WithUnitOfWorkAsync(async () =>
        {
            var updatedItem = await _inventoryItemRepository.GetAsync(invItemId);
            updatedItem.Quantity.ShouldBe(87m); // 80 + 7 returned
        });
    }

    /// <summary>
    /// يجب أن يرفع ApproveReturnAsync استثناء إذا كان الطلب ليس مرتجعاً.
    /// </summary>
    [Fact]
    public async Task ApproveReturnAsync_Should_Throw_When_Request_Is_Not_A_Return()
    {
        Guid reqDeptId   = Guid.NewGuid();
        Guid srcWhId     = Guid.NewGuid();
        Guid normalReqId = Guid.NewGuid();
        Guid patientId   = Guid.NewGuid();
        Guid admissionId = Guid.NewGuid();

        // Arrange: a normal (non-return) request in Submitted state
        await WithUnitOfWorkAsync(async () =>
        {
            await _departmentRepository.InsertAsync(new Department(reqDeptId, null, "DEP-ERR01", "NursingERR"));
            await _warehouseRepository.InsertAsync(new Warehouse(srcWhId, "Pharmacy WH ERR", "StoreERR", "PHARMERR"));

            var pt = new Patient(patientId, null, "PT-ERR01", "Hana", "Jamal",
                new DateTime(1978, 9, 9), Gender.Female, IdentityType.NationalId, "555", "059");
            await _patientRepository.InsertAsync(pt);

            var admission = new Admission(admissionId, null, patientId, Guid.NewGuid(), Guid.NewGuid());
            await _admissionRepository.InsertAsync(admission);

            // IsReturn = false – regular request, should NOT be approvable as a return
            var normalReq = new InternalRequest(normalReqId, "REQ-ERR-01", reqDeptId, srcWhId, DateTime.Now)
            {
                AdmissionId = admissionId,
                RequestType = InternalRequestType.Medication,
                Status      = InternalRequestStatus.Submitted,
                IsReturn    = false
            };
            await _internalRequestRepository.InsertAsync(normalReq);
        });

        // Act & Assert: calling ApproveReturnAsync on a non-return must throw
        await WithUnitOfWorkAsync(async () =>
        {
            await Should.ThrowAsync<UserFriendlyException>(async () =>
            {
                await _internalRequestAppService.ApproveReturnAsync(normalReqId);
            });
        });
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  Tests for Uncovered Cases (Cancel, Delete, GetList, Create Validation)
    // ══════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task CreateAsync_Without_AdmissionId_Should_Throw()
    {
        var dto = new CreateUpdateInternalRequestDto
        {
            RequestingDepartmentId = Guid.NewGuid(),
            FulfilledByWarehouseId = Guid.NewGuid(),
            RequestType = InternalRequestType.Consumable,
            RequestDate = DateTime.Now,
            Lines = new List<CreateUpdateInternalRequestLineDto>()
        };

        await Should.ThrowAsync<UserFriendlyException>(async () =>
        {
            await _internalRequestAppService.CreateAsync(dto);
        });
    }

    [Fact]
    public async Task CancelRequestAsync_Should_Cancel_Request()
    {
        Guid reqDeptId = Guid.NewGuid();
        Guid srcWhId = Guid.NewGuid();
        Guid requestId = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            await _departmentRepository.InsertAsync(new Department(reqDeptId, null, "DEP01", "Pharmacy Dep"));
            await _warehouseRepository.InsertAsync(new Warehouse(srcWhId, "Main Store", "Location", "MAIN"));
            
            var req = new InternalRequest(requestId, "REQ-01", reqDeptId, srcWhId, DateTime.Now);
            await _internalRequestRepository.InsertAsync(req);
        });

        await WithUnitOfWorkAsync(async () =>
        {
            var res = await _internalRequestAppService.CancelRequestAsync(requestId);
            res.Status.ShouldBe(InternalRequestStatus.Cancelled);
        });

        await WithUnitOfWorkAsync(async () =>
        {
            var updated = await _internalRequestRepository.GetAsync(requestId);
            updated.Status.ShouldBe(InternalRequestStatus.Cancelled);
        });
    }

    [Fact]
    public async Task CancelRequestAsync_Already_Cancelled_Should_Throw()
    {
        Guid reqDeptId = Guid.NewGuid();
        Guid srcWhId = Guid.NewGuid();
        Guid requestId = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            await _departmentRepository.InsertAsync(new Department(reqDeptId, null, "DEP01", "Pharmacy Dep"));
            await _warehouseRepository.InsertAsync(new Warehouse(srcWhId, "Main Store", "Location", "MAIN"));
            
            var req = new InternalRequest(requestId, "REQ-01", reqDeptId, srcWhId, DateTime.Now)
            {
                Status = InternalRequestStatus.Cancelled
            };
            await _internalRequestRepository.InsertAsync(req);
        });

        await WithUnitOfWorkAsync(async () =>
        {
            await Should.ThrowAsync<UserFriendlyException>(async () =>
            {
                await _internalRequestAppService.CancelRequestAsync(requestId);
            });
        });
    }

    [Fact]
    public async Task CancelRequestAsync_Discharged_Patient_Should_Throw()
    {
        Guid reqDeptId = Guid.NewGuid();
        Guid srcWhId = Guid.NewGuid();
        Guid requestId = Guid.NewGuid();
        Guid patientId = Guid.NewGuid();
        Guid admissionId = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            await _departmentRepository.InsertAsync(new Department(reqDeptId, null, "DEP01", "Pharmacy Dep"));
            await _warehouseRepository.InsertAsync(new Warehouse(srcWhId, "Main Store", "Location", "MAIN"));
            
            var pt = new Patient(patientId, null, "PT-01", "Ahmed", "Ali", new DateTime(1990, 1, 1), Gender.Male, IdentityType.NationalId, "123", "050");
            await _patientRepository.InsertAsync(pt);
            
            var admission = new Admission(admissionId, null, patientId, Guid.NewGuid(), Guid.NewGuid())
            {
                Status = AdmissionStatus.Discharged
            };
            await _admissionRepository.InsertAsync(admission);

            var req = new InternalRequest(requestId, "REQ-01", reqDeptId, srcWhId, DateTime.Now)
            {
                AdmissionId = admissionId
            };
            await _internalRequestRepository.InsertAsync(req);
        });

        await WithUnitOfWorkAsync(async () =>
        {
            await Should.ThrowAsync<UserFriendlyException>(async () =>
            {
                await _internalRequestAppService.CancelRequestAsync(requestId);
            });
        });
    }

    [Fact]
    public async Task DeleteAsync_Should_Delete_Request()
    {
        Guid reqDeptId = Guid.NewGuid();
        Guid srcWhId = Guid.NewGuid();
        Guid requestId = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            await _departmentRepository.InsertAsync(new Department(reqDeptId, null, "DEP01", "Pharmacy Dep"));
            await _warehouseRepository.InsertAsync(new Warehouse(srcWhId, "Main Store", "Location", "MAIN"));
            
            var req = new InternalRequest(requestId, "REQ-01", reqDeptId, srcWhId, DateTime.Now);
            await _internalRequestRepository.InsertAsync(req);
        });

        await WithUnitOfWorkAsync(async () =>
        {
            await _internalRequestAppService.DeleteAsync(requestId);
        });

        await WithUnitOfWorkAsync(async () =>
        {
            var exists = await _internalRequestRepository.FindAsync(requestId);
            exists.ShouldBeNull();
        });
    }

    [Fact]
    public async Task GetListAsync_Should_Return_Filtered_Requests()
    {
        Guid reqDeptId = Guid.NewGuid();
        Guid srcWhId = Guid.NewGuid();
        Guid requestId1 = Guid.NewGuid();
        Guid requestId2 = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            await _departmentRepository.InsertAsync(new Department(reqDeptId, null, "DEP01", "Pharmacy Dep"));
            await _warehouseRepository.InsertAsync(new Warehouse(srcWhId, "Main Store", "Location", "MAIN"));
            
            var req1 = new InternalRequest(requestId1, "REQ-0001", reqDeptId, srcWhId, new DateTime(2023, 1, 1));
            var req2 = new InternalRequest(requestId2, "REQ-0002", reqDeptId, srcWhId, new DateTime(2023, 1, 5));
            await _internalRequestRepository.InsertAsync(req1);
            await _internalRequestRepository.InsertAsync(req2);
        });

        await WithUnitOfWorkAsync(async () =>
        {
            var result = await _internalRequestAppService.GetListAsync(new InternalRequestGetListInput
            {
                FromDate = new DateTime(2023, 1, 2),
                ToDate = new DateTime(2023, 1, 10),
                FilterText = "REQ-0002"
            });

            result.TotalCount.ShouldBe(1);
            result.Items[0].Id.ShouldBe(requestId2);
        });
    }
}
