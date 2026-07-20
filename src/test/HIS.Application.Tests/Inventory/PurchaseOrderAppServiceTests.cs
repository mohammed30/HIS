using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HIS.Inventory.Dtos;
using Shouldly;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace HIS.Inventory.Tests;

public abstract class PurchaseOrderAppServiceTests<TStartupModule> : HISTestBase<TStartupModule>
    where TStartupModule : Volo.Abp.Modularity.IAbpModule
{
    private readonly IPurchaseOrderAppService _purchaseOrderAppService;
    private readonly IRepository<Supplier, Guid> _supplierRepository;
    private readonly IRepository<Warehouse, Guid> _warehouseRepository;
    private readonly IRepository<InventoryItem, Guid> _inventoryItemRepository;

    protected PurchaseOrderAppServiceTests()
    {
        _purchaseOrderAppService = GetRequiredService<IPurchaseOrderAppService>();
        _supplierRepository = GetRequiredService<IRepository<Supplier, Guid>>();
        _warehouseRepository = GetRequiredService<IRepository<Warehouse, Guid>>();
        _inventoryItemRepository = GetRequiredService<IRepository<InventoryItem, Guid>>();
    }

    [Fact]
    public async Task CreateAsync_ShouldCreatePurchaseOrder()
    {
        // Arrange
        Guid supplierId = Guid.NewGuid();
        Guid productId = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            await _supplierRepository.InsertAsync(new Supplier(supplierId, "Test Supplier", "contact", "000", "e@e.c", "addr", "1234567890"));
            var serviceItemRepo = GetRequiredService<IRepository<HIS.Services.ServiceItem, Guid>>();
            await serviceItemRepo.InsertAsync(new HIS.Services.ServiceItem(productId, "T-PROD", "Test Product", HIS.Services.ServiceCategory.Consumable, null));
        });

        var input = new CreateUpdatePurchaseOrderDto
        {
            SupplierId = supplierId,
            OrderDate = DateTime.Now,
            ReferenceNumber = "PO-REF-123",
            Notes = "Test note",
            PurchaseOrderLines = new List<CreateUpdatePurchaseOrderLineDto>
            {
                new CreateUpdatePurchaseOrderLineDto
                {
                    ProductId = productId,
                    Quantity = 10,
                    UnitPrice = 100,
                    Discount = 5
                }
            }
        };

        // Act
        PurchaseOrderDto result = null;
        try
        {
            await WithUnitOfWorkAsync(async () =>
            {
                result = await _purchaseOrderAppService.CreateAsync(input);
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex}");
            var allPos = await _purchaseOrderAppService.GetListAsync(new PagedAndSortedResultRequestDto());
            foreach (var po in allPos.Items)
            {
                Console.WriteLine($"IN DB -> PO: Id={po.Id}, OrderNum={po.OrderNumber}");
            }
            throw;
        }

        // Assert
        result.ShouldNotBeNull();
        result.SupplierId.ShouldBe(supplierId);
        result.Status.ShouldBe(PurchaseOrderStatus.Draft);
        result.TotalAmount.ShouldBe(10 * 100 - 5);
        result.PurchaseOrderLines.Count.ShouldBe(1);
    }

    [Fact]
    public async Task ConfirmOrderAsync_ShouldChangeStatusToConfirmed()
    {
        // Arrange
        Guid supplierId = Guid.NewGuid();
        Guid productId = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            await _supplierRepository.InsertAsync(new Supplier(supplierId, "Test Supplier 2", "contact", "000", "e@e.c", "addr", "1234567890"));
            var serviceItemRepo = GetRequiredService<IRepository<HIS.Services.ServiceItem, Guid>>();
            await serviceItemRepo.InsertAsync(new HIS.Services.ServiceItem(productId, "T-PROD2", "Test Product 2", HIS.Services.ServiceCategory.Consumable, null));
        });

        PurchaseOrderDto order = null;
        await WithUnitOfWorkAsync(async () =>
        {
            order = await _purchaseOrderAppService.CreateAsync(new CreateUpdatePurchaseOrderDto
            {
                SupplierId = supplierId,
                OrderDate = DateTime.Now,
                ReferenceNumber = "PO-REF-123",
                Notes = "Test note",
                PurchaseOrderLines = new List<CreateUpdatePurchaseOrderLineDto>
                {
                    new CreateUpdatePurchaseOrderLineDto { ProductId = productId, Quantity = 5, UnitPrice = 50 }
                }
            });
        });

        // Act
        PurchaseOrderDto confirmedOrder = null;
        await WithUnitOfWorkAsync(async () =>
        {
            confirmedOrder = await _purchaseOrderAppService.ConfirmOrderAsync(order.Id);
        });

        // Assert
        confirmedOrder.ShouldNotBeNull();
        confirmedOrder.Status.ShouldBe(PurchaseOrderStatus.Confirmed);
    }
}
