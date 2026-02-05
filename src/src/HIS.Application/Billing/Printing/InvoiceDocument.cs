using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HIS.Billing.Printing;

public class InvoiceDocument : IDocument
{
    // Primary brand colors
    private static readonly string PrimaryBlue = "#003366";
    private static readonly string LightBlue = "#E6F2FF";
    private static readonly string AccentRed = "#DC3545";
    private static readonly string TextDark = "#333333";
    private static readonly string TextLight = "#FFFFFF";
    private static readonly string BorderGray = "#CCCCCC";

    public string InvoiceNumber { get; set; }
    public DateTime Date { get; set; }
    public DateTime? DueDate { get; set; }
    public string PatientName { get; set; }
    public string PatientNumber { get; set; }
    public string Status { get; set; }
    public byte[] LogoBytes { get; set; }
    
    public List<InvoiceItemModel> Items { get; set; } = new();
    
    public decimal SubTotal { get; set; }
    public decimal Discount { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }

    public class InvoiceItemModel
    {
        public string Service { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
    }

    public DocumentMetadata GetMetadata() => new DocumentMetadata
    {
        Title = $"Invoice {InvoiceNumber}",
        Author = "Asia Hospital"
    };

    public void Compose(IDocumentContainer container)
    {
        container
            .Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11).FontColor(TextDark));
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
            column.Item().Background(PrimaryBlue).Padding(15).Row(row =>
            {
                // Logo on the right (RTL)
                if (LogoBytes != null && LogoBytes.Length > 0)
                {
                    row.ConstantItem(80).AlignMiddle().Image(LogoBytes).FitArea();
                }
                else
                {
                    row.ConstantItem(80);
                }

                // Hospital info in the center
                row.RelativeItem().AlignCenter().Column(col =>
                {
                    col.Item().Text("مستشفى آسيا")
                        .FontSize(24)
                        .Bold()
                        .FontColor(TextLight);
                    col.Item().Text("ASIA HOSPITAL")
                        .FontSize(14)
                        .FontColor(TextLight);
                    col.Item().PaddingTop(5).Text("فاتورة ضريبية / TAX INVOICE")
                        .FontSize(12)
                        .FontColor(LightBlue);
                });

                row.ConstantItem(80); // Spacer for balance
            });

            // Invoice Title Bar
            column.Item().Background(AccentRed).Padding(8).AlignCenter()
                .Text("فاتورة / INVOICE")
                .FontSize(16)
                .Bold()
                .FontColor(TextLight);
        });
    }

    void ComposeContent(IContainer container)
    {
        container.PaddingVertical(15).Column(column =>
        {
            // Invoice & Patient Info Section
            column.Item().Row(row =>
            {
                // Right side: Patient Info
                row.RelativeItem().Element(c => ComposeSection(c, "بيانات المريض", comp =>
                {
                    comp.Item().Text(text => { text.Span("الاسم: ").Bold(); text.Span(PatientName ?? "-"); });
                    comp.Item().Text(text => { text.Span("رقم الملف: ").Bold(); text.Span(PatientNumber ?? "-"); });
                }));
                
                row.ConstantItem(20);

                // Left side: Invoice Info
                row.RelativeItem().Element(c => ComposeSection(c, "بيانات الفاتورة", comp =>
                {
                    comp.Item().Text(text => { text.Span("رقم الفاتورة: ").Bold(); text.Span(InvoiceNumber ?? "-"); });
                    comp.Item().Text(text => { text.Span("التاريخ: ").Bold(); text.Span(Date.ToString("yyyy/MM/dd")); });
                    comp.Item().Text(text => { text.Span("الحالة: ").Bold(); text.Span(Status ?? "-"); });
                }));
            });

            column.Item().Height(20);

            // Lines Table
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3); // Service Description
                    columns.RelativeColumn(1); // Qty
                    columns.RelativeColumn(1); // Unit Price
                    columns.RelativeColumn(1); // Total
                });

                table.Header(header =>
                {
                    header.Cell().Element(HeaderCell).Text("الخدمة").Bold();
                    header.Cell().Element(HeaderCell).Text("الكمية").Bold();
                    header.Cell().Element(HeaderCell).Text("السعر").Bold();
                    header.Cell().Element(HeaderCell).Text("الاجمالي").Bold();
                });

                foreach (var item in Items)
                {
                    table.Cell().Element(DataCell).Text(item.Service);
                    table.Cell().Element(DataCell).Text(item.Quantity.ToString());
                    table.Cell().Element(DataCell).Text($"{item.UnitPrice:N2}");
                    table.Cell().Element(DataCell).Text($"{item.Total:N2}");
                }

                // Footers for Totals
                table.Cell().ColumnSpan(3).Element(TotalLabelCell).Text("المجموع الفرعي:");
                table.Cell().Element(DataCell).Text($"{SubTotal:N2}");

                if (Discount > 0)
                {
                    table.Cell().ColumnSpan(3).Element(TotalLabelCell).Text("الخصم:");
                    table.Cell().Element(DataCell).Text($"{Discount:N2}");
                }

                table.Cell().ColumnSpan(3).Element(TotalLabelCell).Text("ضريبة القيمة المضافة (15%):");
                table.Cell().Element(DataCell).Text($"{Tax:N2}");

                table.Cell().ColumnSpan(3).Element(TotalLabelCell).Text("الاجمالي النهائي:");
                table.Cell().Element(DataCellHighlight).Text(t => t.Span($"{Total:N2} ج.م").Bold());
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

    static IContainer HeaderCell(IContainer container) => container.Background(PrimaryBlue).Padding(8).AlignCenter().DefaultTextStyle(x => x.FontColor(TextLight).Bold());
    static IContainer DataCell(IContainer container) => container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(8).AlignCenter();
    static IContainer DataCellHighlight(IContainer container) => container.Background(Colors.Yellow.Lighten4).Border(1).BorderColor(BorderGray).Padding(8).AlignCenter();
    static IContainer TotalLabelCell(IContainer container) => container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(8).AlignRight().DefaultTextStyle(x => x.Bold());
}
