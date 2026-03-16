using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HIS.Billing;
using HIS.Inventory;
using HIS.Services;
using HIS.Pharmacy.Dtos;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using HIS.Accounting;
using Volo.Abp.Content;
using System.IO;
using QuestPDF.Fluent;
using HIS.Billing.Printing;
using HIS.Patients;

namespace HIS.Pharmacy;

public class PosAppService : HISAppService, IPosAppService
{
    private readonly IRepository<Drug, Guid> _drugRepository;
    private readonly IRepository<InventoryItem, Guid> _inventoryItemRepository;
    private readonly IRepository<Warehouse, Guid> _warehouseRepository;
    private readonly IRepository<Invoice, Guid> _invoiceRepository;
    private readonly IRepository<ServiceItem, Guid> _serviceItemRepository;
    private readonly IRepository<Account, Guid> _accountRepository;
    private readonly IRepository<Patient, Guid> _patientRepository;
    private readonly IRepository<InvoiceItem, Guid> _invoiceItemRepository;
    private readonly InventoryManager _inventoryManager;
    private readonly AccountingManager _accountingManager;
    private readonly IGuidGenerator _guidGenerator;

    public PosAppService(
        IRepository<Drug, Guid> drugRepository,
        IRepository<InventoryItem, Guid> inventoryItemRepository,
        IRepository<Warehouse, Guid> warehouseRepository,
        IRepository<Invoice, Guid> invoiceRepository,
        IRepository<ServiceItem, Guid> serviceItemRepository,
        IRepository<Account, Guid> accountRepository,
        IRepository<Patient, Guid> patientRepository,
        IRepository<InvoiceItem, Guid> invoiceItemRepository,
        InventoryManager inventoryManager,
        AccountingManager accountingManager,
        IGuidGenerator guidGenerator)
    {
        _drugRepository = drugRepository;
        _inventoryItemRepository = inventoryItemRepository;
        _warehouseRepository = warehouseRepository;
        _invoiceRepository = invoiceRepository;
        _serviceItemRepository = serviceItemRepository;
        _accountRepository = accountRepository;
        _patientRepository = patientRepository;
        _invoiceItemRepository = invoiceItemRepository;
        _inventoryManager = inventoryManager;
        _accountingManager = accountingManager;
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

    public async Task<List<PosProductDto>> SearchProductsAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return new List<PosProductDto>();

        var drugs = await _drugRepository.GetListAsync(x => 
            x.BrandName.Contains(query) || 
            x.ScientificName.Contains(query) || 
            x.Barcode.Contains(query));

        var dtos = new List<PosProductDto>();
        foreach (var drug in drugs)
        {
            dtos.Add(await MapToPosProduct(drug));
        }

        return dtos;
    }

    private async Task<PosProductDto> MapToPosProduct(Drug drug)
    {
        var pharmacy = await _warehouseRepository.FirstOrDefaultAsync(x => x.Name == "Pharmacy Warehouse");
        int stock = 0;
        decimal price = 0;

        if (drug.ServiceItemId.HasValue)
        {
            var serviceItem = await _serviceItemRepository.FirstOrDefaultAsync(x => x.Id == drug.ServiceItemId.Value);
            price = serviceItem?.Price ?? 0;

            if (pharmacy != null)
            {
                var invItem = await _inventoryItemRepository.FirstOrDefaultAsync(x => x.WarehouseId == pharmacy.Id && x.ProductId == drug.ServiceItemId.Value);
                stock = (int)(invItem?.Quantity ?? 0);
            }
        }

        return new PosProductDto
        {
            Id = drug.Id,
            Name = $"{drug.BrandName} {drug.Strength}",
            Barcode = drug.Barcode,
            Price = price,
            CurrentStock = stock
        };
    }

    public async Task<Guid> ProcessSaleAsync(PosSaleDto input)
    {
        var pharmacy = await _warehouseRepository.FirstOrDefaultAsync(x => x.Name == "Pharmacy Warehouse");
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
            invoiceItem.ServiceCode = drug.ServiceItemId.Value.ToString("N"); // Use "N" for 32 chars to fit in db
            invoiceItem.DiscountAmount = item.Discount;
            
            invoice.Items.Add(invoiceItem);
            await _invoiceItemRepository.InsertAsync(invoiceItem);
            
            invoice.TotalAmount += (item.Quantity * item.UnitPrice);
            invoice.DiscountAmount += item.Discount;
        }
        
        // 3. Mark Paid & Finalize
        invoice.PaidAmount = input.PaidAmount;
        invoice.NetAmount = invoice.TotalAmount - invoice.DiscountAmount;
        invoice.Status = Billing.InvoiceStatus.Paid; // Auto-pay in POS

        await _invoiceRepository.InsertAsync(invoice);

        // 4. Accounting Entry (Revenue Side)
        // Dr Cash (1110) / Cr Pharmacy Revenue (4200)
        var cashAccount = await _accountRepository.FirstOrDefaultAsync(x => x.Code == "1110");
        var revenueAccount = await _accountRepository.FirstOrDefaultAsync(x => x.Code == "4200");

        if (cashAccount != null && revenueAccount != null)
        {
            var entry = await _accountingManager.CreateEntryAsync(DateTime.Now, invoice.InvoiceNumber, $"مبيعات صيدلية: {invoice.InvoiceNumber}");
            entry.AddLine(_guidGenerator, cashAccount.Id, invoice.TotalAmount, 0);
            entry.AddLine(_guidGenerator, revenueAccount.Id, 0, invoice.TotalAmount);
            await _accountingManager.PostEntryAsync(entry);
        }

        return invoice.Id;
    }

    [Microsoft.AspNetCore.Mvc.HttpPost]
    [Microsoft.AspNetCore.Mvc.Route("api/app/pos/refund-sale/{invoiceNumber}")]
    public async Task RefundSaleAsync(string invoiceNumber)
    {
        var invoice = await _invoiceRepository.FirstOrDefaultAsync(x => x.InvoiceNumber == invoiceNumber);
        if (invoice == null) throw new UserFriendlyException("Invoice not found: " + invoiceNumber);
        
        // Include items for reversal
        var items = await _invoiceItemRepository.GetListAsync(x => x.InvoiceId == invoice.Id);
        
        if (invoice.Status == Billing.InvoiceStatus.Refunded) throw new UserFriendlyException("Invoice already refunded");

        var pharmacy = await _warehouseRepository.FirstOrDefaultAsync(x => x.Name == "Pharmacy Warehouse");
        if (pharmacy == null) throw new UserFriendlyException("Pharmacy Warehouse not found");

        // 1. Mark as Refunded
        invoice.Status = Billing.InvoiceStatus.Refunded;
        await _invoiceRepository.UpdateAsync(invoice);

        // 2. Reverse Inventory (Stock & COGS)
        foreach (var item in items)
        {
            // Resolve product (DrugId was used in POS, but ServiceCode holds Drug's ServiceItemId)
            if (Guid.TryParse(item.ServiceCode, out Guid productId))
            {
                await _inventoryManager.ReturnStockAsync(pharmacy.Id, productId, item.Quantity, invoice.InvoiceNumber);
            }
        }

        // 3. Accounting Entry (Revenue Reversal)
        // Dr Pharmacy Revenue (4200) / Cr Cash (1110)
        var cashAccount = await _accountRepository.FirstOrDefaultAsync(x => x.Code == "1110");
        var revenueAccount = await _accountRepository.FirstOrDefaultAsync(x => x.Code == "4200");

        if (cashAccount != null && revenueAccount != null)
        {
            var entry = await _accountingManager.CreateEntryAsync(DateTime.Now, invoice.InvoiceNumber, $"مرتجع مبيعات صيدلية: {invoice.InvoiceNumber}");
            entry.AddLine(_guidGenerator, revenueAccount.Id, invoice.TotalAmount, 0); // Reverse Revenue
            entry.AddLine(_guidGenerator, cashAccount.Id, 0, invoice.TotalAmount); // Reverse Cash
            await _accountingManager.PostEntryAsync(entry);
        }
    }

    [Microsoft.AspNetCore.Mvc.HttpGet]
    [Microsoft.AspNetCore.Mvc.Route("api/app/pos/generate-doc/{idOrNumber}")]
    public async Task<IRemoteStreamContent> GetInvoicePdfAsync(string idOrNumber)
    {
        Invoice invoice;
        if (Guid.TryParse(idOrNumber, out Guid invoiceId))
        {
            invoice = await _invoiceRepository.GetAsync(invoiceId, true);
        }
        else
        {
            invoice = await _invoiceRepository.FirstOrDefaultAsync(x => x.InvoiceNumber == idOrNumber);
            if (invoice != null)
            {
                // Explicitly load items if found by number
                var items = await _invoiceItemRepository.GetListAsync(x => x.InvoiceId == invoice.Id);
                foreach (var item in items) invoice.Items.Add(item);
            }
        }

        if (invoice == null) throw new UserFriendlyException("Invoice not found: " + idOrNumber);
        var patient = await _patientRepository.FirstOrDefaultAsync(x => x.Id == invoice.PatientId);

        var model = new InvoiceDocument
        {
            InvoiceNumber = invoice.InvoiceNumber,
            Date = invoice.InvoiceDate,
            PatientName = patient?.FullNameAr ?? "عميل نقدي / Guest",
            PatientNumber = patient?.MRN ?? "-",
            Status = L[invoice.Status.ToString()],
            SubTotal = invoice.TotalAmount,
            Discount = invoice.DiscountAmount,
            Tax = invoice.TaxAmount,
            Total = invoice.NetAmount,
            Items = invoice.Items.Select(i => new InvoiceDocument.InvoiceItemModel
            {
                Service = i.Description,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                Total = i.TotalPrice
            }).ToList()
        };

        using (var ms = new MemoryStream())
        {
            model.GeneratePdf(ms);
            return new RemoteStreamContent(new MemoryStream(ms.ToArray()), $"Invoice_{invoice.InvoiceNumber}.pdf", "application/pdf");
        }
    }
}
