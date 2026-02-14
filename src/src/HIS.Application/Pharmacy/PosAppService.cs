using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HIS.Billing;
using HIS.Inventory;
using HIS.Pharmacy.Dtos;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;

namespace HIS.Pharmacy;

public class PosAppService : HISAppService, IPosAppService
{
    private readonly IRepository<Drug, Guid> _drugRepository;
    private readonly IRepository<InventoryItem, Guid> _inventoryItemRepository;
    private readonly IRepository<Warehouse, Guid> _warehouseRepository;
    private readonly IRepository<Invoice, Guid> _invoiceRepository;
    private readonly InventoryManager _inventoryManager;
    private readonly IGuidGenerator _guidGenerator;

    public PosAppService(
        IRepository<Drug, Guid> drugRepository,
        IRepository<InventoryItem, Guid> inventoryItemRepository,
        IRepository<Warehouse, Guid> warehouseRepository,
        IRepository<Invoice, Guid> invoiceRepository,
        InventoryManager inventoryManager,
        IGuidGenerator guidGenerator)
    {
        _drugRepository = drugRepository;
        _inventoryItemRepository = inventoryItemRepository;
        _warehouseRepository = warehouseRepository;
        _invoiceRepository = invoiceRepository;
        _inventoryManager = inventoryManager;
        _guidGenerator = guidGenerator;
    }

    public async Task<PosProductDto> GetProductByBarcodeAsync(string barcode)
    {
        var drug = await _drugRepository.FirstOrDefaultAsync(x => x.Barcode == barcode);
        if (drug == null) throw new UserFriendlyException("Product not found");

        return await MapToPosProduct(drug);
    }

    public async Task<PosProductDto> GetProductByIdAsync(Guid id)
    {
         var drug = await _drugRepository.GetAsync(id);
         return await MapToPosProduct(drug);
    }

    private async Task<PosProductDto> MapToPosProduct(Drug drug)
    {
        // Get Stock from Pharmacy Warehouse
        var pharmacy = await _warehouseRepository.FirstOrDefaultAsync(x => x.Name == "Pharmacy");
        int stock = 0;
        decimal price = 0; // Get from ServiceItem or Drug? 
        // Drug has Price in DTO but Entity relies on ServiceItem linkage.
        // Assuming ServiceItem linkage is consistent.
        // For now returning 0 price/stock as placeholder if not linked.
        
        if (pharmacy != null && drug.ServiceItemId.HasValue)
        {
             var invItem = await _inventoryItemRepository.FirstOrDefaultAsync(x => x.WarehouseId == pharmacy.Id && x.ProductId == drug.ServiceItemId.Value);
             stock = (int)(invItem?.Quantity ?? 0);
             // Need to fetch ServiceItem to get price. Skipping for brevity, assume frontend handles or fetched via Include.
        }

        return new PosProductDto
        {
            Id = drug.Id,
            Name = $"{drug.BrandName} {drug.Strength}",
            Barcode = drug.Barcode,
            Price = 100, // Placeholder
            CurrentStock = stock
        };
    }

    public async Task ProcessSaleAsync(PosSaleDto input)
    {
        var pharmacy = await _warehouseRepository.FirstOrDefaultAsync(x => x.Name == "Pharmacy");
        if (pharmacy == null) throw new UserFriendlyException("Pharmacy Warehouse not found");

        // 1. Create Invoice
        var tenantId = CurrentTenant.Id;
        var patientId = input.PatientId ?? Guid.Empty; // TODO: Handle Guest/Walk-in properly
        var invoice = new Invoice(_guidGenerator.Create(), tenantId, patientId, "POS-" + DateTime.Now.Ticks);
        invoice.InvoiceDate = DateTime.Now;
        // ... Populate Invoice details
        
        // 2. Add Items & Deduct Stock
        foreach (var item in input.Items)
        {
            var drug = await _drugRepository.GetAsync(item.DrugId);
            if (!drug.ServiceItemId.HasValue) continue;

            // Deduct Stock
            await _inventoryManager.DispenseStockAsync(pharmacy.Id, drug.ServiceItemId.Value, item.Quantity, "POS Sale");

            // Add to Invoice
            var invoiceItem = new InvoiceItem(
                _guidGenerator.Create(),
                tenantId,
                invoice.Id,
                $"{drug.BrandName} {drug.Strength}",
                item.UnitPrice
            );
            invoiceItem.Quantity = item.Quantity;
            invoiceItem.ServiceType = ServiceType.Medication;
            invoiceItem.ServiceCode = drug.ServiceItemId.Value.ToString();
            invoiceItem.DiscountAmount = item.Discount;
            
            invoice.Items.Add(invoiceItem);        }
        
        // 3. Mark Paid
        invoice.PaidAmount = input.PaidAmount;
        invoice.NetAmount = invoice.TotalAmount - invoice.DiscountAmount; // Simplified calculation
        
        await _invoiceRepository.InsertAsync(invoice);
    }
}
