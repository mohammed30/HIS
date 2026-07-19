using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HIS.Accounting;
using HIS.Billing;
using HIS.Billing.Printing;
using HIS.Inventory;
using HIS.Patients;
using HIS.Pharmacy.Dtos;
using HIS.Services;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using Volo.Abp;
using Volo.Abp.Content;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;

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
    private readonly IRepository<AccountMapping, Guid> _accountMappingRepository;
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
        IRepository<AccountMapping, Guid> accountMappingRepository,
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
        _accountMappingRepository = accountMappingRepository;
        _inventoryManager = inventoryManager;
        _accountingManager = accountingManager;
        _guidGenerator = guidGenerator;
    }

    // ─────────────────────────────────────────────────────────
    //  Product Lookup
    // ─────────────────────────────────────────────────────────

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
            dtos.Add(await MapToPosProduct(drug));

        return dtos;
    }

    private async Task<PosProductDto> MapToPosProduct(Drug drug)
    {
        var pharmacy = await _warehouseRepository.FirstOrDefaultAsync(x => x.Name == "Pharmacy Warehouse" || x.Name == "مستودع الصيدلية");
        int stock = 0;
        decimal price = 0;

        if (drug.ServiceItemId.HasValue)
        {
            var serviceItem = await _serviceItemRepository.FirstOrDefaultAsync(x => x.Id == drug.ServiceItemId.Value);
            price = serviceItem?.Price ?? 0;

            if (pharmacy != null)
            {
                var invItem = await _inventoryItemRepository.FirstOrDefaultAsync(
                    x => x.WarehouseId == pharmacy.Id && x.ProductId == drug.ServiceItemId.Value);
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

    // ─────────────────────────────────────────────────────────
    //  Step 1: Create Draft
    // ─────────────────────────────────────────────────────────

    [HttpPost]
    [Route("api/app/pos/create-draft")]
    public async Task<Guid> CreateDraftAsync(PosSaleDto input)
    {
        var tenantId = CurrentTenant.Id;
        var patientId = input.PatientId ?? Guid.Empty;
        var invoiceNumber = "POS-" + DateTime.Now.Ticks;

        var invoice = new Invoice(_guidGenerator.Create(), tenantId, patientId, invoiceNumber);
        invoice.InvoiceDate = DateTime.Now;
        invoice.Status = InvoiceStatus.Draft;
        invoice.InvoiceType = InvoiceType.Sale;
        invoice.Notes = input.Notes;
        invoice.PaymentMethod = input.PaymentMethod;

        foreach (var item in input.Items)
        {
            var drug = await _drugRepository.GetAsync(item.DrugId);
            if (!drug.ServiceItemId.HasValue) continue;

            var invoiceItem = new InvoiceItem(
                _guidGenerator.Create(), tenantId, invoice.Id,
                $"{drug.BrandName} {drug.Strength}", item.UnitPrice)
            {
                Quantity = item.Quantity,
                ServiceType = ServiceType.Medication,
                ServiceCode = drug.ServiceItemId.Value.ToString("N"),
                DiscountAmount = item.Discount
            };

            invoice.Items.Add(invoiceItem);
            await _invoiceItemRepository.InsertAsync(invoiceItem);

            invoice.TotalAmount += item.Quantity * item.UnitPrice;
            invoice.DiscountAmount += item.Discount;
        }

        invoice.NetAmount = invoice.TotalAmount - invoice.DiscountAmount;
        await _invoiceRepository.InsertAsync(invoice, autoSave: true);

        return invoice.Id;
    }

    // ─────────────────────────────────────────────────────────
    //  Step 3: Submit for Approval (Pharmacist → Accountant)
    // ─────────────────────────────────────────────────────────

    [HttpPost]
    [Route("api/app/pos/{invoiceId}/submit-for-approval")]
    public async Task SubmitForApprovalAsync(Guid invoiceId)
    {
        var invoice = await _invoiceRepository.GetAsync(invoiceId);

        if (invoice.Status != InvoiceStatus.Draft && invoice.Status != InvoiceStatus.Rejected)
            throw new UserFriendlyException("يمكن إرسال الفاتورة فقط عندما تكون في حالة مسودة أو مرفوضة");

        invoice.Status = InvoiceStatus.PendingApproval;
        invoice.RejectionReason = null; // Clear previous rejection reason
        await _invoiceRepository.UpdateAsync(invoice);

        // Notify Subscribers
        var notificationRepo = LazyServiceProvider.LazyGetRequiredService<IRepository<HIS.Notifications.Notification, Guid>>();
        var notificationSender = LazyServiceProvider.LazyGetRequiredService<HIS.Notifications.NotificationSender>();
        var settingProvider = LazyServiceProvider.LazyGetRequiredService<Volo.Abp.Settings.ISettingProvider>();

        var subscribersStr = await settingProvider.GetOrNullAsync("Notifications.Subscribers.Pharmacy");
        var subscriberIds = string.IsNullOrEmpty(subscribersStr) ? new List<Guid>() : subscribersStr.Split(',').Select(Guid.Parse).ToList();

        if (subscriberIds.Any())
        {
            var notifications = subscriberIds.Select(id => new HIS.Notifications.Notification(
                _guidGenerator.Create(),
                id,
                "فاتورة صيدلية بانتظار الاعتماد",
                $"تم إرسال فاتورة مبيعات جديدة رقم {invoice.InvoiceNumber} بانتظار الاعتماد.",
                HIS.Notifications.NotificationTypes.Pharmacy,
                $"/pharmacy/pos?invoiceId={invoice.Id}",
                invoice.Id.ToString(),
                CurrentUser.UserName ?? "النظام"
            )).ToList();

            await notificationRepo.InsertManyAsync(notifications);
            foreach (var notif in notifications)
            {
                var dto = ObjectMapper.Map<HIS.Notifications.Notification, HIS.Notifications.NotificationDto>(notif);
                await notificationSender.SendToUserAsync(notif.UserId, dto);
            }
        }
    }

    // ─────────────────────────────────────────────────────────
    //  Step 4 (Reject): Accountant Rejects Invoice
    // ─────────────────────────────────────────────────────────

    [HttpPost]
    [Route("api/app/pos/{invoiceId}/reject")]
    public async Task RejectAsync(Guid invoiceId, PosRejectDto input)
    {
        var invoice = await _invoiceRepository.GetAsync(invoiceId);

        if (invoice.Status != InvoiceStatus.PendingApproval)
            throw new UserFriendlyException("يمكن رفض الفاتورة فقط عندما تكون في انتظار الاعتماد");

        invoice.Status = InvoiceStatus.Rejected;
        invoice.RejectionReason = input.RejectionReason;
        await _invoiceRepository.UpdateAsync(invoice);

        // Notify Subscribers
        var notificationRepo = LazyServiceProvider.LazyGetRequiredService<IRepository<HIS.Notifications.Notification, Guid>>();
        var notificationSender = LazyServiceProvider.LazyGetRequiredService<HIS.Notifications.NotificationSender>();
        var settingProvider = LazyServiceProvider.LazyGetRequiredService<Volo.Abp.Settings.ISettingProvider>();

        var subscribersStr = await settingProvider.GetOrNullAsync("Notifications.Subscribers.Pharmacy");
        var subscriberIds = string.IsNullOrEmpty(subscribersStr) ? new List<Guid>() : subscribersStr.Split(',').Select(Guid.Parse).ToList();

        if (subscriberIds.Any())
        {
            var notifications = subscriberIds.Select(id => new HIS.Notifications.Notification(
                _guidGenerator.Create(),
                id,
                "فاتورة صيدلية مرفوضة",
                $"تم رفض فاتورة مبيعات الصيدلية رقم {invoice.InvoiceNumber}. السبب: {input.RejectionReason}",
                HIS.Notifications.NotificationTypes.Pharmacy,
                $"/pharmacy/pos?invoiceId={invoice.Id}",
                invoice.Id.ToString(),
                CurrentUser.UserName ?? "النظام"
            )).ToList();

            await notificationRepo.InsertManyAsync(notifications);
            foreach (var notif in notifications)
            {
                var dto = ObjectMapper.Map<HIS.Notifications.Notification, HIS.Notifications.NotificationDto>(notif);
                await notificationSender.SendToUserAsync(notif.UserId, dto);
            }
        }
    }

    // ─────────────────────────────────────────────────────────
    //  Step 5: Approve & Pay (Accountant)
    // ─────────────────────────────────────────────────────────

    [HttpPost]
    [Route("api/app/pos/{invoiceId}/approve-and-pay")]
    public async Task ApproveAndPayAsync(Guid invoiceId, PosApproveDto input)
    {
        var invoice = await _invoiceRepository.GetAsync(invoiceId);

        if (invoice.Status != InvoiceStatus.PendingApproval)
            throw new UserFriendlyException("يمكن اعتماد الفاتورة فقط عندما تكون في انتظار الاعتماد");

        invoice.Status = InvoiceStatus.Paid;
        invoice.PaidAmount = input.PaidAmount;
        invoice.PaymentMethod = input.PaymentMethod;
        if (!string.IsNullOrWhiteSpace(input.Notes))
            invoice.Notes = (invoice.Notes ?? "") + " | " + input.Notes;

        await _invoiceRepository.UpdateAsync(invoice, autoSave: true);

        // Accounting Entry: Dr Cash / Cr Revenue
        await CreateSaleAccountingEntryAsync(invoice);

        // Notify Subscribers
        var notificationRepo = LazyServiceProvider.LazyGetRequiredService<IRepository<HIS.Notifications.Notification, Guid>>();
        var notificationSender = LazyServiceProvider.LazyGetRequiredService<HIS.Notifications.NotificationSender>();
        var settingProvider = LazyServiceProvider.LazyGetRequiredService<Volo.Abp.Settings.ISettingProvider>();

        var subscribersStr = await settingProvider.GetOrNullAsync("Notifications.Subscribers.Pharmacy");
        var subscriberIds = string.IsNullOrEmpty(subscribersStr) ? new List<Guid>() : subscribersStr.Split(',').Select(Guid.Parse).ToList();

        if (subscriberIds.Any())
        {
            var notifications = subscriberIds.Select(id => new HIS.Notifications.Notification(
                _guidGenerator.Create(),
                id,
                "اعتماد فاتورة صيدلية",
                $"تم اعتماد ودفع فاتورة مبيعات الصيدلية رقم {invoice.InvoiceNumber}.",
                HIS.Notifications.NotificationTypes.Pharmacy,
                $"/pharmacy/pos?invoiceId={invoice.Id}",
                invoice.Id.ToString(),
                CurrentUser.UserName ?? "النظام"
            )).ToList();

            await notificationRepo.InsertManyAsync(notifications);
            foreach (var notif in notifications)
            {
                var dto = ObjectMapper.Map<HIS.Notifications.Notification, HIS.Notifications.NotificationDto>(notif);
                await notificationSender.SendToUserAsync(notif.UserId, dto);
            }
        }
    }

    // ─────────────────────────────────────────────────────────
    //  Step 7: Dispense Items (Pharmacist)
    // ─────────────────────────────────────────────────────────

    [HttpPost]
    [Route("api/app/pos/{invoiceId}/dispense")]
    public async Task DispenseAsync(Guid invoiceId)
    {
        var invoice = await _invoiceRepository.GetAsync(invoiceId);

        if (invoice.Status != InvoiceStatus.Paid)
            throw new UserFriendlyException("يمكن صرف الأصناف فقط للفواتير المدفوعة");

        var pharmacy = await _warehouseRepository.FirstOrDefaultAsync(x => x.Name == "Pharmacy Warehouse" || x.Name == "مستودع الصيدلية");
        if (pharmacy == null) throw new UserFriendlyException("Pharmacy Warehouse not found");

        var items = await _invoiceItemRepository.GetListAsync(x => x.InvoiceId == invoice.Id);

        foreach (var item in items)
        {
            if (Guid.TryParse(item.ServiceCode, out Guid serviceItemId))
            {
                await _inventoryManager.DispenseStockAsync(pharmacy.Id, serviceItemId, item.Quantity, invoice.InvoiceNumber);
            }
        }

        invoice.Status = InvoiceStatus.Dispensed;
        await _invoiceRepository.UpdateAsync(invoice);
    }

    // ─────────────────────────────────────────────────────────
    //  Partial Refund
    // ─────────────────────────────────────────────────────────

    [HttpPost]
    [Route("api/app/pos/{invoiceId}/partial-refund")]
    public async Task<PosRefundResultDto> PartialRefundAsync(Guid invoiceId, PosPartialRefundDto input)
    {
        var originalInvoice = await _invoiceRepository.GetAsync(invoiceId);

        if (originalInvoice.Status != InvoiceStatus.Paid && originalInvoice.Status != InvoiceStatus.Dispensed)
            throw new UserFriendlyException("لا يمكن ارتجاع فاتورة غير مكتملة الدفع");

        if (originalInvoice.InvoiceType == InvoiceType.Return)
            throw new UserFriendlyException("لا يمكن ارتجاع فاتورة مرتجع");

        var pharmacy = await _warehouseRepository.FirstOrDefaultAsync(x => x.Name == "Pharmacy Warehouse" || x.Name == "مستودع الصيدلية");
        if (pharmacy == null) throw new UserFriendlyException("Pharmacy Warehouse not found");

        var originalItems = await _invoiceItemRepository.GetListAsync(x => x.InvoiceId == originalInvoice.Id);

        var tenantId = CurrentTenant.Id;
        var refundNumber = "RET-" + DateTime.Now.Ticks;
        var refundInvoice = new Invoice(
            _guidGenerator.Create(), tenantId, originalInvoice.PatientId, refundNumber)
        {
            InvoiceDate = DateTime.Now,
            Status = InvoiceStatus.Refunded,
            InvoiceType = InvoiceType.Return,
            OriginalInvoiceId = originalInvoice.Id,
            OriginalInvoiceNumber = originalInvoice.InvoiceNumber,
            PaymentMethod = originalInvoice.PaymentMethod
        };

        decimal refundTotal = 0;

        foreach (var refundItem in input.Items)
        {
            var originalItem = originalItems.FirstOrDefault(x => x.Id == refundItem.InvoiceItemId);
            if (originalItem == null)
                throw new UserFriendlyException($"بند الفاتورة غير موجود: {refundItem.InvoiceItemId}");

            if (refundItem.ReturnQuantity <= 0 || refundItem.ReturnQuantity > originalItem.Quantity)
                throw new UserFriendlyException(
                    $"كمية الارتجاع غير صالحة للبند: {originalItem.Description}. الكمية الأصلية: {originalItem.Quantity}");

            var invoiceItem = new InvoiceItem(
                _guidGenerator.Create(), tenantId, refundInvoice.Id,
                originalItem.Description, originalItem.UnitPrice)
            {
                Quantity = refundItem.ReturnQuantity,
                ServiceType = originalItem.ServiceType,
                ServiceCode = originalItem.ServiceCode,
                DiscountAmount = 0
            };

            refundInvoice.Items.Add(invoiceItem);
            await _invoiceItemRepository.InsertAsync(invoiceItem);

            refundTotal += refundItem.ReturnQuantity * originalItem.UnitPrice;

            // Return stock to pharmacy
            if (Guid.TryParse(originalItem.ServiceCode, out Guid serviceItemId))
            {
                await _inventoryManager.ReturnStockAsync(
                    pharmacy.Id, serviceItemId, refundItem.ReturnQuantity, refundNumber);
            }
        }

        refundInvoice.TotalAmount = refundTotal;
        refundInvoice.NetAmount = refundTotal;
        refundInvoice.PaidAmount = refundTotal; // Refund amount to return to customer

        await _invoiceRepository.InsertAsync(refundInvoice);

        // If all items are returned, mark original as fully refunded
        bool allReturned = input.Items.All(ri =>
        {
            var orig = originalItems.FirstOrDefault(x => x.Id == ri.InvoiceItemId);
            return orig != null && ri.ReturnQuantity == orig.Quantity;
        }) && input.Items.Count == originalItems.Count;

        if (allReturned)
        {
            originalInvoice.Status = InvoiceStatus.Refunded;
            await _invoiceRepository.UpdateAsync(originalInvoice);
        }

        // Accounting Entry: Dr Revenue / Cr Cash (reversal)
        await CreateRefundAccountingEntryAsync(refundInvoice, refundTotal);

        return new PosRefundResultDto
        {
            RefundInvoiceId = refundInvoice.Id,
            RefundInvoiceNumber = refundInvoice.InvoiceNumber,
            RefundAmount = refundTotal
        };
    }

    // ─────────────────────────────────────────────────────────
    //  Queries
    // ─────────────────────────────────────────────────────────

    [HttpGet]
    [Route("api/app/pos/invoices")]
    public async Task<List<PosInvoiceListDto>> GetPosInvoicesAsync(InvoiceStatus? status = null, string? filter = null, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var invoices = await _invoiceRepository.GetListAsync(x =>
            (status == null || x.Status == status) &&
            (status != InvoiceStatus.Refunded || x.InvoiceType == InvoiceType.Return) &&
            (x.InvoiceNumber.StartsWith("POS-") || x.InvoiceNumber.StartsWith("RET-")) &&
            (string.IsNullOrEmpty(filter) || x.InvoiceNumber.Contains(filter) || x.OriginalInvoiceNumber.Contains(filter)) &&
            (!fromDate.HasValue || x.InvoiceDate >= fromDate.Value) &&
            (!toDate.HasValue || x.InvoiceDate <= toDate.Value));

        invoices = invoices.OrderByDescending(x => x.InvoiceDate).ToList();

        var result = new List<PosInvoiceListDto>();
        foreach (var inv in invoices)
        {
            var patient = await _patientRepository.FirstOrDefaultAsync(x => x.Id == inv.PatientId);
            var items = await _invoiceItemRepository.GetListAsync(x => x.InvoiceId == inv.Id);

            result.Add(new PosInvoiceListDto
            {
                Id = inv.Id,
                InvoiceNumber = inv.InvoiceNumber,
                InvoiceDate = inv.InvoiceDate,
                PatientName = patient?.FullNameAr ?? "عميل نقدي",
                TotalAmount = inv.TotalAmount,
                PaidAmount = inv.PaidAmount,
                Status = inv.Status,
                InvoiceType = inv.InvoiceType,
                RejectionReason = inv.RejectionReason,
                OriginalInvoiceNumber = inv.OriginalInvoiceNumber,
                Items = items.Select(i => new PosInvoiceItemDto
                {
                    Id = i.Id,
                    Description = i.Description,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    TotalPrice = i.TotalPrice,
                    ServiceCode = i.ServiceCode
                }).ToList()
            });
        }

        return result;
    }

    [HttpGet]
    [Route("api/app/pos/invoices/{invoiceId}")]
    public async Task<PosInvoiceListDto> GetInvoiceDetailsAsync(Guid invoiceId)
    {
        var inv = await _invoiceRepository.GetAsync(invoiceId);
        var patient = await _patientRepository.FirstOrDefaultAsync(x => x.Id == inv.PatientId);
        var items = await _invoiceItemRepository.GetListAsync(x => x.InvoiceId == inv.Id);

        return new PosInvoiceListDto
        {
            Id = inv.Id,
            InvoiceNumber = inv.InvoiceNumber,
            InvoiceDate = inv.InvoiceDate,
            PatientName = patient?.FullNameAr ?? "عميل نقدي",
            TotalAmount = inv.TotalAmount,
            PaidAmount = inv.PaidAmount,
            Status = inv.Status,
            InvoiceType = inv.InvoiceType,
            RejectionReason = inv.RejectionReason,
            OriginalInvoiceNumber = inv.OriginalInvoiceNumber,
            Items = items.Select(i => new PosInvoiceItemDto
            {
                Id = i.Id,
                Description = i.Description,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalPrice = i.TotalPrice,
                ServiceCode = i.ServiceCode
            }).ToList()
        };
    }

    // ─────────────────────────────────────────────────────────
    //  Printing
    // ─────────────────────────────────────────────────────────

    [HttpGet]
    [Route("api/app/pos/generate-doc/{idOrNumber}")]
    public async Task<IRemoteStreamContent> GetInvoicePdfAsync(string idOrNumber)
    {
        Invoice invoice;
        if (Guid.TryParse(idOrNumber, out Guid invoiceId))
        {
            invoice = await _invoiceRepository.GetAsync(invoiceId);
            if (invoice != null)
            {
                var items = await _invoiceItemRepository.GetListAsync(x => x.InvoiceId == invoice.Id);
                foreach (var item in items) invoice.Items.Add(item);
            }
        }
        else
        {
            invoice = await _invoiceRepository.FirstOrDefaultAsync(x => x.InvoiceNumber == idOrNumber);
            if (invoice != null)
            {
                var items = await _invoiceItemRepository.GetListAsync(x => x.InvoiceId == invoice.Id);
                foreach (var item in items) invoice.Items.Add(item);
            }
        }

        if (invoice == null) throw new UserFriendlyException("Invoice not found: " + idOrNumber);
        return await BuildInvoicePdfAsync(invoice, singleCopy: true);
    }

    [HttpGet]
    [Route("api/app/pos/return-doc/{refundInvoiceId}")]
    public async Task<IRemoteStreamContent> GetReturnInvoicePdfAsync(Guid refundInvoiceId)
    {
        var invoice = await _invoiceRepository.GetAsync(refundInvoiceId);
        if (invoice.InvoiceType != InvoiceType.Return)
            throw new UserFriendlyException("هذه ليست فاتورة مرتجع");

        var items = await _invoiceItemRepository.GetListAsync(x => x.InvoiceId == invoice.Id);
        foreach (var item in items) invoice.Items.Add(item);

        return await BuildInvoicePdfAsync(invoice, singleCopy: false); // Two copies
    }

    private async Task<IRemoteStreamContent> BuildInvoicePdfAsync(Invoice invoice, bool singleCopy)
    {
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
            IsReturn = invoice.InvoiceType == InvoiceType.Return,
            OriginalInvoiceNumber = invoice.OriginalInvoiceNumber,
            PrintTwoCopies = !singleCopy,
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
            var filename = invoice.InvoiceType == InvoiceType.Return
                ? $"Return_{invoice.InvoiceNumber}.pdf"
                : $"Invoice_{invoice.InvoiceNumber}.pdf";
            return new RemoteStreamContent(new MemoryStream(ms.ToArray()), filename, "application/pdf");
        }
    }

    // ─────────────────────────────────────────────────────────
    //  Legacy Compatibility
    // ─────────────────────────────────────────────────────────

    [HttpPost]
    [Route("api/app/pos/process-sale")]
    public async Task<Guid> ProcessSaleAsync(PosSaleDto input)
    {
        // Legacy: Create draft + submit + approve in one shot (backward compat)
        var invoiceId = await CreateDraftAsync(input);

        var invoice = await _invoiceRepository.GetAsync(invoiceId);
        invoice.Status = InvoiceStatus.Paid;
        invoice.PaidAmount = input.PaidAmount;
        await _invoiceRepository.UpdateAsync(invoice);

        await CreateSaleAccountingEntryAsync(invoice);

        // Deduct from stock automatically in the same process
        await DispenseAsync(invoiceId);

        return invoiceId;
    }

    [HttpPost]
    [Route("api/app/pos/refund-sale/{invoiceNumber}")]
    public async Task RefundSaleAsync(string invoiceNumber)
    {
        var invoice = await _invoiceRepository.FirstOrDefaultAsync(x => x.InvoiceNumber == invoiceNumber);
        if (invoice == null) throw new UserFriendlyException("Invoice not found: " + invoiceNumber);

        var items = await _invoiceItemRepository.GetListAsync(x => x.InvoiceId == invoice.Id);
        if (invoice.Status == InvoiceStatus.Refunded)
            throw new UserFriendlyException("Invoice already refunded");

        var allItems = items.Select(i => new PosRefundItemDto
        {
            InvoiceItemId = i.Id,
            ReturnQuantity = i.Quantity
        }).ToList();

        await PartialRefundAsync(invoice.Id, new PosPartialRefundDto { Items = allItems });
    }

    // ─────────────────────────────────────────────────────────
    //  Helpers
    // ─────────────────────────────────────────────────────────

    private async Task CreateSaleAccountingEntryAsync(Invoice invoice)
    {
        var (cashAccount, revenueAccount) = await GetAccountingAccountsAsync();
        if (cashAccount == null || revenueAccount == null) return;

        var entry = await _accountingManager.CreateEntryAsync(
            DateTime.Now, invoice.InvoiceNumber,
            $"مبيعات صيدلية: {invoice.InvoiceNumber}", isAutomatic: true);

        entry.AddLine(_guidGenerator, cashAccount.Id, invoice.TotalAmount, 0);
        entry.AddLine(_guidGenerator, revenueAccount.Id, 0, invoice.TotalAmount);
        await _accountingManager.PostEntryAsync(entry);
    }

    private async Task CreateRefundAccountingEntryAsync(Invoice refundInvoice, decimal amount)
    {
        var (cashAccount, revenueAccount) = await GetAccountingAccountsAsync();
        if (cashAccount == null || revenueAccount == null) return;

        var entry = await _accountingManager.CreateEntryAsync(
            DateTime.Now, refundInvoice.InvoiceNumber,
            $"مرتجع مبيعات صيدلية: {refundInvoice.OriginalInvoiceNumber} → {refundInvoice.InvoiceNumber}",
            isAutomatic: true);

        entry.AddLine(_guidGenerator, revenueAccount.Id, amount, 0); // Dr Revenue (reversal)
        entry.AddLine(_guidGenerator, cashAccount.Id, 0, amount);    // Cr Cash (reversal)
        await _accountingManager.PostEntryAsync(entry);
    }

    private async Task<(Account cashAccount, Account revenueAccount)> GetAccountingAccountsAsync()
    {
        var cashMapping = await _accountMappingRepository.FirstOrDefaultAsync(
            x => x.MappingType == AccountMappingType.CashAccount);
        var cashAccount = cashMapping?.AccountId.HasValue == true
            ? await _accountRepository.FirstOrDefaultAsync(x => x.Id == cashMapping.AccountId.Value)
            : await _accountRepository.FirstOrDefaultAsync(x => x.Code == "1110");

        var revenueMapping = await _accountMappingRepository.FirstOrDefaultAsync(
            x => x.MappingType == AccountMappingType.SalesRevenue);
        var revenueAccount = revenueMapping?.AccountId.HasValue == true
            ? await _accountRepository.FirstOrDefaultAsync(x => x.Id == revenueMapping.AccountId.Value)
            : await _accountRepository.FirstOrDefaultAsync(x => x.Code == "4200");

        cashAccount = await GetLeafAccountAsync(cashAccount);
        revenueAccount = await GetLeafAccountAsync(revenueAccount);

        return (cashAccount, revenueAccount);
    }

    private async Task<Account> GetLeafAccountAsync(Account account)
    {
        if (account == null) return null;
        var hasChildren = await _accountRepository.AnyAsync(x => x.ParentId == account.Id && x.IsActive);
        if (!hasChildren) return account;

        var children = await _accountRepository.GetListAsync(x => x.ParentId == account.Id && x.IsActive);
        foreach (var child in children.OrderBy(x => x.Code))
        {
            var leaf = await GetLeafAccountAsync(child);
            if (leaf != null) return leaf;
        }
        return account;
    }
}

