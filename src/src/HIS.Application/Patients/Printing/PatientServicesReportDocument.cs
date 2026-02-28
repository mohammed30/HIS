using System;
using System.Linq;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HIS.Patients.Printing;

public class PatientServicesReportDocument : IDocument
{
    private static readonly string PrimaryBlue = "#003366";
    private static readonly string LightBlue = "#E6F2FF";
    private static readonly string AccentRed = "#DC3545";
    private static readonly string TextDark = "#333333";
    private static readonly string TextLight = "#FFFFFF";
    private static readonly string BorderGray = "#CCCCCC";
    private static readonly string SuccessGreen = "#28a745";

    public PatientServicesReportDto ReportData { get; set; }
    public byte[] LogoBytes { get; set; }

    public DocumentMetadata GetMetadata() => new DocumentMetadata
    {
        Title = $"تقرير خدمات المريض - {ReportData?.PatientName}",
        Author = "مستشفى آسيا"
    };

    public void Compose(IDocumentContainer container)
    {
        container
            .Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10).FontColor(TextDark)); // Reduced from 12 to 10
                page.ContentFromRightToLeft(); // RTL for Arabic

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });
    }

    void ComposeHeader(IContainer container)
    {
        container.Column(column =>
        {
            // Top header with blue background
            column.Item().Background(PrimaryBlue).PaddingVertical(6).PaddingHorizontal(12).Row(row =>
            {
                // Logo on the right (RTL)
                if (LogoBytes != null && LogoBytes.Length > 0)
                    row.ConstantItem(40).AlignMiddle().Image(LogoBytes).FitArea();
                else
                    row.ConstantItem(40);

                // Hospital info in the center
                row.RelativeItem().AlignCenter().Column(col =>
                {
                    col.Item().Text(text =>
                    {
                        text.Span("مستشفى آسيا  ").FontSize(16).Bold().FontColor(TextLight);
                        text.Span("ASIA HOSPITAL").FontSize(10).FontColor(TextLight);
                    });
                    
                    col.Item().PaddingTop(2).Text("تقرير خدمات مريض / PATIENT SERVICES REPORT")
                        .FontSize(10)
                        .FontColor(LightBlue);
                });

                row.ConstantItem(40); // Spacer
            });

            // Title Bar
            column.Item().Background(AccentRed).Padding(8).AlignCenter()
                .Text("تقرير خدمات المريض")
                .FontSize(16)
                .Bold()
                .FontColor(TextLight);
        });
    }

    void ComposeContent(IContainer container)
    {
        container.PaddingVertical(15).Column(column =>
        {
            // Patient Info Section
            column.Item().Row(row =>
            {
                // Right side: Patient Info
                row.RelativeItem(2).Element(c => ComposeSection(c, "بيانات المريض", comp =>
                {
                    comp.Item().PaddingBottom(5).Text(text => { text.Span("الاسم: ").Bold().FontSize(14); text.Span(ReportData?.PatientName ?? "-").FontSize(14); });
                    comp.Item().Text(text => { text.Span("رقم الملف: ").Bold().FontSize(12); text.Span(ReportData?.MRN ?? "-").FontSize(12); });
                }));
                
                row.ConstantItem(20);

                // Left side: Report Info
                row.RelativeItem(1).Element(c => ComposeSection(c, "معلومات التقرير", comp =>
                {
                    comp.Item().Text(text => { text.Span("تاريخ الطباعة: ").Bold(); text.Span(ReportData?.ReportDate.ToString("yyyy/MM/dd HH:mm") ?? "-"); });
                }));
            });

            column.Item().Height(15);
            
            // Totals Summary
            column.Item().Row(row =>
            {
                row.RelativeItem().Border(1).BorderColor(PrimaryBlue).Background(LightBlue).Padding(10).AlignCenter().Column(col =>
                {
                    col.Item().Text("إجمالي المطالبات").FontSize(12).FontColor(TextDark).Bold();
                    col.Item().Text($"{ReportData?.TotalAmountInvoiced:N2}").FontSize(16).FontColor(PrimaryBlue).Bold();
                });
                row.ConstantItem(15);
                row.RelativeItem().Border(1).BorderColor(SuccessGreen).Background("#f4fdf8").Padding(10).AlignCenter().Column(col =>
                {
                    col.Item().Text("إجمالي الدفعات").FontSize(12).FontColor(TextDark).Bold();
                    col.Item().Text($"{ReportData?.TotalAmountPaid:N2}").FontSize(16).FontColor(SuccessGreen).Bold();
                });
                row.ConstantItem(15);
                row.RelativeItem().Border(1).BorderColor(AccentRed).Background("#fff5f5").Padding(10).AlignCenter().Column(col =>
                {
                    col.Item().Text("الإجمالي المتبقي").FontSize(12).FontColor(TextDark).Bold();
                    col.Item().Text($"{ReportData?.TotalAmountDue:N2}").FontSize(16).FontColor(AccentRed).Bold();
                });
            });

            column.Item().Height(20);

            // Services Table
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(1.2f); // Date
                    columns.RelativeColumn(1.8f); // Invoice Number
                    columns.RelativeColumn(3.5f); // Service
                    columns.RelativeColumn(0.8f); // Quantity
                    columns.RelativeColumn(1.2f); // Total Price
                    columns.RelativeColumn(1f); // Status
                });

                table.Header(header =>
                {
                    header.Cell().Element(HeaderCell).Text("التاريخ").Bold();
                    header.Cell().Element(HeaderCell).Text("رقم الفاتورة").Bold();
                    header.Cell().Element(HeaderCell).Text("الخدمة / العملية").Bold();
                    header.Cell().Element(HeaderCell).Text("الكمية").Bold();
                    header.Cell().Element(HeaderCell).Text("السعر").Bold();
                    header.Cell().Element(HeaderCell).Text("الحالة").Bold();
                });

                if (ReportData?.Services == null || !ReportData.Services.Any())
                {
                    table.Cell().ColumnSpan(6).Element(DataCell).Padding(15).Text("لا توجد خدمات مسجلة").Italic().FontColor(Colors.Grey.Medium);
                }
                else
                {
                    foreach (var service in ReportData.Services)
                    {
                        table.Cell().Element(DataCell).Text(service.Date.ToString("yyyy/MM/dd"));
                        table.Cell().Element(DataCell).Text(service.InvoiceNumber);
                        table.Cell().Element(ServiceDataCell).Text(service.ServiceDescription);
                        table.Cell().Element(DataCell).Text(service.Quantity.ToString("0.##"));
                        table.Cell().Element(DataCell).Text($"{service.TotalPrice:N2}");
                        
                        var statusColor = service.IsPaid ? SuccessGreen : AccentRed;
                        var statusText = service.IsPaid ? "مدفوعة" : "غير مدفوعة";
                        table.Cell().Element(DataCell).Text(statusText).FontColor(statusColor).Bold();
                    }
                }
            });
        });
    }

    void ComposeSection(IContainer container, string title, Action<ColumnDescriptor> contentAction)
    {
        container.Border(1).BorderColor(BorderGray).Column(column =>
        {
            column.Item().Background(LightBlue).Padding(5).Text(title).Bold().FontColor(PrimaryBlue);
            column.Item().Padding(10).Column(contentAction);
        });
    }

    void ComposeFooter(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().LineHorizontal(2).LineColor(PrimaryBlue);
            column.Item().PaddingTop(10).AlignCenter().Text("شكراً لزيارتكم مستشفى آسيا - نتمنى لكم دوام الصحة والعافية").FontSize(10).Italic();
        });
    }

    static IContainer HeaderCell(IContainer container) => container.Background(PrimaryBlue).Padding(6).AlignCenter().AlignMiddle().DefaultTextStyle(x => x.FontColor(TextLight).Bold());
    static IContainer DataCell(IContainer container) => container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(6).PaddingHorizontal(4).AlignCenter().AlignMiddle();
    static IContainer ServiceDataCell(IContainer container) => container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(6).PaddingHorizontal(4).AlignRight().AlignMiddle();
}
