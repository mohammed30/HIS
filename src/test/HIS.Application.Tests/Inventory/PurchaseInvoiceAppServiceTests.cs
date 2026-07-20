using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HIS.Accounting;
using HIS.Inventory.Dtos;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace HIS.Inventory.Tests;

public abstract class PurchaseInvoiceAppServiceTests<TStartupModule> : HISTestBase<TStartupModule>
    where TStartupModule : Volo.Abp.Modularity.IAbpModule
{
    private readonly IPurchaseInvoiceAppService _purchaseInvoiceAppService;
    private readonly IRepository<Supplier, Guid> _supplierRepository;
    private readonly IRepository<PurchaseOrder, Guid> _purchaseOrderRepository;
    private readonly IRepository<Warehouse, Guid> _warehouseRepository;
    private readonly IRepository<Account, Guid> _accountRepository;
    private readonly IRepository<AccountMapping, Guid> _accountMappingRepository;

    protected PurchaseInvoiceAppServiceTests()
    {
        _purchaseInvoiceAppService = GetRequiredService<IPurchaseInvoiceAppService>();
        _supplierRepository = GetRequiredService<IRepository<Supplier, Guid>>();
        _purchaseOrderRepository = GetRequiredService<IRepository<PurchaseOrder, Guid>>();
        _warehouseRepository = GetRequiredService<IRepository<Warehouse, Guid>>();
        _accountRepository = GetRequiredService<IRepository<Account, Guid>>();
        _accountMappingRepository = GetRequiredService<IRepository<AccountMapping, Guid>>();
    }
    
    private async Task SeedAccountingForTestAsync()
    {
        var invAcc = await _accountRepository.FirstOrDefaultAsync(a => a.Code == "1130");
        if (invAcc == null)
        {
            invAcc = new Account(Guid.NewGuid(), "1130", "Inventory", "المخزون", AccountType.Asset, null);
            await _accountRepository.InsertAsync(invAcc, autoSave: true);
        }

        var invMapping = await _accountMappingRepository.FirstOrDefaultAsync(m => m.MappingType == AccountMappingType.Inventory);
        if (invMapping == null)
            await _accountMappingRepository.InsertAsync(new AccountMapping(Guid.NewGuid(), AccountMappingType.Inventory, invAcc.Id, true), autoSave: true);
        else if (invMapping.AccountId == null)
        {
            invMapping.AccountId = invAcc.Id;
            await _accountMappingRepository.UpdateAsync(invMapping, autoSave: true);
        }

        var otherMappings = new[] { 
            AccountMappingType.SalesRevenue, AccountMappingType.CashAccount, 
            AccountMappingType.VATOutput, AccountMappingType.VATInput, 
            AccountMappingType.InsuranceReceivable, AccountMappingType.CardPaymentBank, 
            AccountMappingType.PatientsReceivable, AccountMappingType.COGS
        };
        foreach (var mapping in otherMappings)
        {
            var existingMapping = await _accountMappingRepository.FirstOrDefaultAsync(m => m.MappingType == mapping);
            if (existingMapping == null)
            {
                await _accountMappingRepository.InsertAsync(new AccountMapping(Guid.NewGuid(), mapping, invAcc.Id, true), autoSave: true);
            }
            else if (existingMapping.AccountId == null)
            {
                existingMapping.AccountId = invAcc.Id;
                await _accountMappingRepository.UpdateAsync(existingMapping, autoSave: true);
            }
        }
    }

    [Fact]
    public async Task CreateAsync_ShouldCreatePurchaseInvoice()
    {
        // Arrange
        Guid supplierId = Guid.NewGuid();
        Guid productId = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            await _supplierRepository.InsertAsync(new Supplier(supplierId, "Test Supplier 3", "contact", "000", "e@e.c", "addr", "1234567890"));
        });

        var input = new CreateUpdatePurchaseInvoiceDto
        {
            SupplierId = supplierId,
            InvoiceNumber = "INV-12345",
            InvoiceDate = DateTime.Now,
            Notes = "Test note",
            Lines = new List<CreateUpdatePurchaseInvoiceLineDto>
            {
                new CreateUpdatePurchaseInvoiceLineDto
                {
                    ProductId = productId,
                    Quantity = 20,
                    UnitCost = 50,
                    Discount = 0,
                    BatchNumber = "BATCH-01"
                }
            }
        };

        // Act
        PurchaseInvoiceDto result = null;
        await WithUnitOfWorkAsync(async () =>
        {
            result = await _purchaseInvoiceAppService.CreateAsync(input);
        });

        // Assert
        result.ShouldNotBeNull();
        result.SupplierId.ShouldBe(supplierId);
        result.InvoiceNumber.ShouldBe("INV-12345");
        result.Status.ShouldBe(PurchaseInvoiceStatus.Draft);
        result.TotalAmount.ShouldBeGreaterThan(0);
        result.Lines.Count.ShouldBe(1);
    }

    [Fact]
    public async Task PostInvoiceAsync_ShouldChangeStatusToPosted()
    {
        // Arrange
        Guid supplierId = Guid.NewGuid();
        Guid productId = Guid.NewGuid();
        Guid warehouseId = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            await _supplierRepository.InsertAsync(new Supplier(supplierId, "Test Supplier 4", "contact", "000", "e@e.c", "addr", "1234567890"));
            await _warehouseRepository.InsertAsync(new Warehouse(warehouseId, "WH 4", "Loc", "WH-4"));
            await SeedAccountingForTestAsync();
        });

        PurchaseInvoiceDto invoice = null;
        await WithUnitOfWorkAsync(async () =>
        {
            invoice = await _purchaseInvoiceAppService.CreateAsync(new CreateUpdatePurchaseInvoiceDto
            {
                SupplierId = supplierId,
                InvoiceNumber = "INV-54321",
                InvoiceDate = DateTime.Now,
                Notes = "Test note",
                Lines = new List<CreateUpdatePurchaseInvoiceLineDto>
                {
                    new CreateUpdatePurchaseInvoiceLineDto { ProductId = productId, Quantity = 10, UnitCost = 100, BatchNumber = "BATCH-02" }
                }
            });
        });

        // Act
        await WithUnitOfWorkAsync(async () =>
        {
            await _purchaseInvoiceAppService.PostInvoiceAsync(invoice.Id, warehouseId);
        });

        // Assert
        await WithUnitOfWorkAsync(async () =>
        {
            var postedInvoice = await _purchaseInvoiceAppService.GetAsync(invoice.Id);
            postedInvoice.ShouldNotBeNull();
            postedInvoice.Status.ShouldBe(PurchaseInvoiceStatus.Posted);
        });
    }
}
