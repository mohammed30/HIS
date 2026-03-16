using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HIS.Accounting;
using HIS.Appointments;
using HIS.Billing;
using HIS.Clinical;
using HIS.Inventory;
using HIS.Patients;
using HIS.Pharmacy;
using HIS.Services;
using HIS.Settings;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;
using System.Linq.Dynamic.Core;
using Volo.Abp.Content;
using System.IO;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Volo.Abp.Identity;
using HIS.Reports;
using Microsoft.AspNetCore.Hosting;

namespace HIS.Reports;

public class ReportAppService : ApplicationService, IReportAppService
{
    private readonly IRepository<Appointment, Guid> _appointmentRepository;
    private readonly IRepository<Invoice, Guid> _invoiceRepository;
    private readonly IRepository<Patient, Guid> _patientRepository;
    private readonly IRepository<Clinic, Guid> _clinicRepository;
    private readonly IRepository<Doctor, Guid> _doctorRepository;
    private readonly IRepository<ServiceItem, Guid> _serviceRepository;
    private readonly IRepository<Dispensing, Guid> _dispensingRepository;
    private readonly IRepository<MedicalOrder, Guid> _medicalOrderRepository;
    private readonly IRepository<InventoryItem, Guid> _inventoryItemRepository;
    private readonly IRepository<JournalEntry, Guid> _journalEntryRepository;
    private readonly IRepository<InvoiceItem, Guid> _invoiceItemRepository;
    private readonly AccountingManager _accountingManager;
    private readonly IIdentityUserRepository _userRepository;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ReportAppService(
        IRepository<Appointment, Guid> appointmentRepository,
        IRepository<Invoice, Guid> invoiceRepository,
        IRepository<Patient, Guid> patientRepository,
        IRepository<Clinic, Guid> clinicRepository,
        IRepository<Doctor, Guid> doctorRepository,
        IRepository<ServiceItem, Guid> serviceRepository,
        IRepository<Dispensing, Guid> dispensingRepository,
        IRepository<MedicalOrder, Guid> medicalOrderRepository,
        IRepository<InventoryItem, Guid> inventoryItemRepository,
        IRepository<JournalEntry, Guid> journalEntryRepository,
        IRepository<InvoiceItem, Guid> invoiceItemRepository,
        AccountingManager accountingManager,
        IIdentityUserRepository userRepository,
        IWebHostEnvironment webHostEnvironment)
    {
        _appointmentRepository = appointmentRepository;
        _invoiceRepository = invoiceRepository;
        _patientRepository = patientRepository;
        _clinicRepository = clinicRepository;
        _doctorRepository = doctorRepository;
        _serviceRepository = serviceRepository;
        _dispensingRepository = dispensingRepository;
        _medicalOrderRepository = medicalOrderRepository;
        _inventoryItemRepository = inventoryItemRepository;
        _journalEntryRepository = journalEntryRepository;
        _invoiceItemRepository = invoiceItemRepository;
        _accountingManager = accountingManager;
        _userRepository = userRepository;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<PagedResultDto<PaidTicketDto>> GetPaidTicketsAsync(GetPaidTicketsInput input)
    {
        var appointments = await _appointmentRepository.GetQueryableAsync();
        var invoices = await _invoiceRepository.GetQueryableAsync();
        
        var query = from appt in appointments
                   join inv in invoices on appt.Id equals inv.AppointmentId
                   where inv.Status == InvoiceStatus.Paid || inv.Status == InvoiceStatus.PartiallyPaid
                   select new { appt, inv };

        if (input.FromDate.HasValue)
            query = query.Where(x => x.appt.AppointmentDate >= input.FromDate.Value);
        
        if (input.ToDate.HasValue)
            query = query.Where(x => x.appt.AppointmentDate < input.ToDate.Value.Date.AddDays(1));

        var totalCount = await AsyncExecuter.CountAsync(query);
        var items = await AsyncExecuter.ToListAsync(query.PageBy(input));

        var dtos = new List<PaidTicketDto>();
        foreach (var item in items)
        {
            var patient = await _patientRepository.FindAsync(item.appt.PatientId);
            var clinic = await _clinicRepository.FindAsync(item.appt.ClinicId);
            var doctor = await _doctorRepository.FindAsync(item.appt.DoctorId);
            var creator = await _userRepository.FindAsync(item.appt.CreatorId ?? Guid.Empty);

            string serviceName = "N/A";
            if (item.appt.ServiceItemId.HasValue)
            {
                var service = await _serviceRepository.FindAsync(item.appt.ServiceItemId.Value);
                serviceName = service?.Name ?? "N/A";
            }

            dtos.Add(new PaidTicketDto
            {
                AppointmentId = item.appt.Id,
                TicketNumber = item.appt.Id.ToString().Substring(0, 8).ToUpper(),
                PatientName = patient?.FullNameAr ?? "Unknown",
                ClinicName = clinic?.NameAr ?? "Unknown",
                DoctorName = doctor?.NameAr ?? "Unknown",
                ServiceName = serviceName,
                Amount = item.inv.TotalAmount,
                AppointmentDate = item.appt.AppointmentDate,
                CreatedByUser = creator?.UserName ?? "system",
                CreationTime = item.appt.CreationTime
            });
        }

        if (!string.IsNullOrEmpty(input.CreatorUser))
        {
            dtos = dtos.Where(x => x.CreatedByUser.Contains(input.CreatorUser, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        return new PagedResultDto<PaidTicketDto>(totalCount, dtos);
    }

    public async Task RefundTicketAsync(Guid appointmentId)
    {
        var appt = await _appointmentRepository.GetAsync(appointmentId);
        var invoice = await _invoiceRepository.FirstOrDefaultAsync(x => x.AppointmentId == appointmentId);

        if (invoice == null || invoice.Status != InvoiceStatus.Paid)
        {
            throw new Volo.Abp.UserFriendlyException("Cannot refund. Ticket is not paid.");
        }

        // 1. Create Reverse Accounting Entry
        var originalEntries = await _journalEntryRepository.GetListAsync(x => x.ReferenceNumber.Contains(invoice.InvoiceNumber));
        foreach (var original in originalEntries)
        {
            var reverseEntry = await _accountingManager.CreateEntryAsync(
                DateTime.Now, 
                $"REFUND-{invoice.InvoiceNumber}", 
                $"إرجاع تذكرة: {invoice.InvoiceNumber}"
            );

            foreach (var line in original.Lines)
            {
                // Reverse Debit/Credit
                reverseEntry.AddLine(GuidGenerator, line.AccountId, line.Credit, line.Debit);
            }
            await _accountingManager.PostEntryAsync(reverseEntry);
        }

        // 2. Update Invoice Status
        invoice.Status = InvoiceStatus.Cancelled;
        await _invoiceRepository.UpdateAsync(invoice);

        // 3. Cancel Appointment
        appt.Status = AppointmentStatus.Cancelled;
        await _appointmentRepository.UpdateAsync(appt);
    }

    public async Task<PagedResultDto<PharmacySalesDto>> GetPharmacySalesAsync(GetPharmacySalesInput input)
    {
        var fromDate = input.FromDate ?? DateTime.MinValue;
        var toDate = input.ToDate?.Date.AddDays(1) ?? DateTime.MaxValue;

        // 1. Fetch Ward Dispensing (Dispensing Entity)
        var dispensings = await _dispensingRepository.WithDetailsAsync(x => x.Items);
        var dispensingQuery = dispensings
            .Where(x => x.CreationTime >= fromDate && x.CreationTime < toDate);

        var dispensingItems = await AsyncExecuter.ToListAsync(dispensingQuery);

        // 2. Fetch POS Sales (InvoiceItem Entity where ServiceType is Medication)
        var invoiceQueryable = await _invoiceRepository.GetQueryableAsync();
        var invoiceItemsQueryable = await _invoiceItemRepository.GetQueryableAsync();

        var posQuery = from invItem in invoiceItemsQueryable
                       join inv in invoiceQueryable on invItem.InvoiceId equals inv.Id
                       where invItem.ServiceType == ServiceType.Medication
                       && inv.InvoiceDate >= fromDate && inv.InvoiceDate < toDate
                       && (inv.Status == InvoiceStatus.Paid || inv.Status == InvoiceStatus.Refunded)
                       select new { invItem, inv };

        var posItems = await AsyncExecuter.ToListAsync(posQuery);

        var dtos = new List<PharmacySalesDto>();

        // Map Ward Dispensing
        foreach (var dispensing in dispensingItems)
        {
            var patient = await _patientRepository.FindAsync(dispensing.PatientId);
            var creator = await _userRepository.FindAsync(dispensing.CreatorId ?? Guid.Empty);

            foreach (var item in dispensing.Items)
            {
                var inventoryItem = await _inventoryItemRepository.FindAsync(item.InventoryItemId);
                
                dtos.Add(new PharmacySalesDto
                {
                    DispensingId = dispensing.Id,
                    PatientName = patient?.FullNameAr ?? "Unknown",
                    ProductName = inventoryItem?.ProductName ?? "Unknown Product",
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitCost,
                    TotalAmount = item.Quantity * item.UnitCost,
                    WithdrawalType = "صرف مخزني / Stock Issue",
                    CreatedByUser = creator?.UserName ?? "pharmacist",
                    DispensingTime = dispensing.CreationTime
                });
            }
        }

        // Map POS Sales
        foreach (var pos in posItems)
        {
            var patient = await _patientRepository.FindAsync(pos.inv.PatientId);
            var creator = await _userRepository.FindAsync(pos.inv.CreatorId ?? Guid.Empty);

            bool isRefund = pos.inv.Status == InvoiceStatus.Refunded;
            dtos.Add(new PharmacySalesDto
            {
                DispensingId = pos.inv.Id, // Use Invoice Id as Dispensing Id for the DTO
                PatientName = patient?.FullNameAr ?? "عميل نقدي / Guest",
                ProductName = pos.invItem.Description,
                Quantity = isRefund ? -pos.invItem.Quantity : pos.invItem.Quantity,
                UnitPrice = pos.invItem.UnitPrice,
                TotalAmount = isRefund ? -(pos.invItem.Quantity * pos.invItem.UnitPrice) : (pos.invItem.Quantity * pos.invItem.UnitPrice),
                WithdrawalType = isRefund ? "مرتجع مبيعات / Refund" : "مبيعات نقطة البيع / POS",
                CreatedByUser = creator?.UserName ?? "cashier",
                DispensingTime = pos.inv.InvoiceDate
            });
        }

        // Apply sorting and paging manually on the merged list
        var resultDtos = dtos.OrderByDescending(x => x.DispensingTime)
                             .Skip(input.SkipCount)
                             .Take(input.MaxResultCount)
                             .ToList();

        return new PagedResultDto<PharmacySalesDto>(dtos.Count, resultDtos);
    }

    [Microsoft.AspNetCore.Mvc.HttpGet]
    [Microsoft.AspNetCore.Mvc.Route("api/app/report/paid-tickets-pdf")]
    public async Task<IRemoteStreamContent> GetPaidTicketsPdfAsync([Microsoft.AspNetCore.Mvc.FromQuery] GetPaidTicketsInput input)
    {
        var data = await GetPaidTicketsAsync(input);
        
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(1, Unit.Centimetre);
                page.Content().Column(col =>
                {
                    col.Item().Text("تقرير التذاكر المدفوعة").FontSize(20).Bold().AlignCenter();
                    col.Item().PaddingTop(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("التاريخ");
                            header.Cell().Text("المريض");
                            header.Cell().Text("الخدمة");
                            header.Cell().Text("المبلغ");
                            header.Cell().Text("المستخدم");
                        });

                        foreach (var row in data.Items)
                        {
                            table.Cell().Text(row.AppointmentDate.ToShortDateString());
                            table.Cell().Text(row.PatientName);
                            table.Cell().Text(row.ServiceName);
                            table.Cell().Text(row.Amount.ToString("N2"));
                            table.Cell().Text(row.CreatedByUser);
                        }
                    });
                });
            });
        });

        using (var ms = new MemoryStream())
        {
            document.GeneratePdf(ms);
            return new RemoteStreamContent(new MemoryStream(ms.ToArray()), "PaidTicketsReport.pdf", "application/pdf");
        }
    }

    [Microsoft.AspNetCore.Mvc.HttpGet]
    [Microsoft.AspNetCore.Mvc.Route("api/app/report/pharmacy-sales-pdf")]
    public async Task<IRemoteStreamContent> GetPharmacySalesPdfAsync([Microsoft.AspNetCore.Mvc.FromQuery] GetPharmacySalesInput input)
    {
        var data = await GetPharmacySalesAsync(input);
        var logoPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "logo", "Dark.png");
        var userName = CurrentUser.UserName ?? "admin";
        var printDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                // Header
                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("نظام مستشفى آسيا").FontSize(10).FontColor(Colors.Grey.Medium);
                        col.Item().Text($"المستخدم: {userName}").FontSize(9);
                        col.Item().Text($"تاريخ الطباعة: {printDateTime}").FontSize(9);
                    });

                    row.RelativeItem().AlignCenter().Column(col =>
                    {
                        if (File.Exists(logoPath))
                        {
                            col.Item().Height(40).Image(logoPath);
                        }
                        col.Item().Text("مستشفى آسيا").FontSize(16).Bold().FontColor(Colors.Green.Darken3);
                    });

                    row.RelativeItem().AlignRight().Column(col =>
                    {
                        col.Item().Text("تقرير مبيعات الصيدلية").FontSize(16).Bold().FontColor(Colors.Green.Darken1);
                        col.Item().Text($"تاريخ البدء: {input.FromDate?.ToShortDateString() ?? "N/A"}").FontSize(9);
                        col.Item().Text($"تاريخ الانتهاء: {input.ToDate?.ToShortDateString() ?? "N/A"}").FontSize(9);
                    });
                });

                // Content
                page.Content().PaddingVertical(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(1.2f); // Time
                        columns.RelativeColumn(1.8f); // Patient
                        columns.RelativeColumn(2.5f); // Product
                        columns.RelativeColumn(0.7f); // Qty
                        columns.RelativeColumn(0.8f); // Price
                        columns.RelativeColumn(1.2f); // Total
                        columns.RelativeColumn(1.5f); // Withdrawal Type
                        columns.RelativeColumn(1.2f); // User
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("وقت الصرف");
                        header.Cell().Element(CellStyle).Text("المريض");
                        header.Cell().Element(CellStyle).Text("اسم الصنف");
                        header.Cell().Element(CellStyle).Text("الكمية");
                        header.Cell().Element(CellStyle).Text("السعر");
                        header.Cell().Element(CellStyle).Text("الإجمالي");
                        header.Cell().Element(CellStyle).Text("نوع السحب");
                        header.Cell().Element(CellStyle).Text("المستخدم");

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.DefaultTextStyle(x => x.SemiBold().FontColor(Colors.White))
                                            .PaddingVertical(5)
                                            .BorderBottom(1)
                                            .BorderColor(Colors.Black)
                                            .Background(Colors.Green.Darken2)
                                            .AlignCenter();
                        }
                    });

                    foreach (var item in data.Items)
                    {
                        table.Cell().Element(ContentStyle).Text(item.DispensingTime.ToString("yyyy-MM-dd HH:mm"));
                        table.Cell().Element(ContentStyle).Text(item.PatientName);
                        table.Cell().Element(ContentStyle).Text(item.ProductName);
                        table.Cell().Element(ContentStyle).Text(item.Quantity.ToString());
                        table.Cell().Element(ContentStyle).Text(item.UnitPrice.ToString("N2"));
                        table.Cell().Element(ContentStyle).Text(item.TotalAmount.ToString("N2"));
                        table.Cell().Element(ContentStyle).Text(item.WithdrawalType);
                        table.Cell().Element(ContentStyle).Text(item.CreatedByUser);

                        static IContainer ContentStyle(IContainer container)
                        {
                            return container.BorderBottom(1)
                                            .BorderColor(Colors.Grey.Lighten2)
                                            .PaddingVertical(5)
                                            .AlignCenter();
                        }
                    }

                    // Total Row
                    table.Footer(footer =>
                    {
                        footer.Cell().ColumnSpan(5).PaddingRight(10).AlignRight().Text("الإجمالي الكلي / Total Sum").Bold();
                        footer.Cell().Background(Colors.Grey.Lighten3).AlignCenter().Text(data.Items.Sum(x => x.TotalAmount).ToString("N2")).Bold().FontColor(Colors.Green.Darken3);
                        footer.Cell().ColumnSpan(2);
                    });
                });

                // Footer
                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("صفحة ");
                    x.CurrentPageNumber();
                    x.Span(" من ");
                    x.TotalPages();
                });
            });
        });

        using (var ms = new MemoryStream())
        {
            document.GeneratePdf(ms);
            return new RemoteStreamContent(new MemoryStream(ms.ToArray()), "PharmacySalesReport.pdf", "application/pdf");
        }
    }
}
