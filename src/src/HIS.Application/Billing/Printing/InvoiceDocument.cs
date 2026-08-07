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
    
    /// <summary>هل هي فاتورة مرتجع؟</summary>
    public bool IsReturn { get; set; } = false;
    /// <summary>رقم الفاتورة الأصلية (لفواتير المرتجع)</summary>
    public string? OriginalInvoiceNumber { get; set; }
    /// <summary>طباعة نسختين (أصل وصورة)</summary>
    public bool PrintTwoCopies { get; set; } = false;
    
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
        // Page 1 (Original - الأصل)
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(1.5f, Unit.Centimetre);
            page.PageColor(Colors.White);
            page.DefaultTextStyle(x => x.FontSize(12).FontColor(TextDark));
            page.ContentFromRightToLeft();

            page.Header().Element(c => ComposeHeader(c, "أصل / ORIGINAL"));
            page.Content().Element(ComposeContent);
            page.Footer().Element(ComposeFooter);
        });

        // Page 2 (Copy - الصورة) - only for return invoices
        if (PrintTwoCopies)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12).FontColor(TextDark));
                page.ContentFromRightToLeft();

                page.Header().Element(c => ComposeHeader(c, "صورة / COPY"));
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });
        }
    }

    void ComposeHeader(IContainer container, string copyLabel = "")
    {
        var titleAr = IsReturn ? "مرتجع / RETURN" : "فاتورة / INVOICE";
        var titleColor = IsReturn ? "#DC3545" : AccentRed;

        container.Column(column =>
        {
            // Top header with blue background
            column.Item().Background(PrimaryBlue).PaddingVertical(6).PaddingHorizontal(12).Row(row =>
            {
                if (LogoBytes != null && LogoBytes.Length > 0)
                    row.ConstantItem(40).AlignMiddle().Image(LogoBytes).FitArea();
                else
                    row.ConstantItem(40);

                row.RelativeItem().AlignCenter().Column(col =>
                {
                    col.Item().Text(text =>
                    {
                        text.Span("مستشفى آسيا  ").FontSize(16).Bold().FontColor(TextLight);
                        text.Span("ASIA HOSPITAL").FontSize(10).FontColor(TextLight);
                    });
                    if (Tax > 0)
                        col.Item().PaddingTop(2).Text("فاتورة ضريبية / TAX INVOICE").FontSize(10).FontColor(LightBlue);
                    else
                        col.Item().PaddingTop(2).Text("نظام معلومات المستشفيات / HIS").FontSize(10).FontColor(LightBlue);
                });

                // Copy label on the left
                if (!string.IsNullOrEmpty(copyLabel))
                    row.ConstantItem(60).AlignMiddle().AlignCenter()
                        .Text(copyLabel).FontSize(9).Bold().FontColor(Colors.Yellow.Medium);
                else
                    row.ConstantItem(60);
            });

            // Invoice Title Bar
            column.Item().Background(titleColor).Padding(8).AlignCenter()
                .Text(titleAr)
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
                    comp.Item().PaddingBottom(5).Text(text => { text.Span("الاسم: ").Bold().FontSize(16); text.Span(PatientName ?? "-").FontSize(16); });
                    comp.Item().Text(text => { text.Span("رقم الملف: ").Bold().FontSize(13); text.Span(PatientNumber ?? "-").FontSize(13); });
                }));
                
                row.ConstantItem(20);

                // Left side: Invoice Info
                row.RelativeItem().Element(c => ComposeSection(c, IsReturn ? "بيانات المرتجع" : "بيانات الفاتورة", comp =>
                {
                    comp.Item().Text(text => { text.Span(IsReturn ? "رقم المرتجع: " : "رقم الفاتورة: ").Bold(); text.Span(InvoiceNumber ?? "-"); });
                    comp.Item().Text(text => { text.Span("التاريخ: ").Bold(); text.Span(Date.ToString("yyyy/MM/dd - HH:mm")); });
                    if (IsReturn && !string.IsNullOrEmpty(OriginalInvoiceNumber))
                        comp.Item().Text(text => { text.Span("فاتورة الأصل: ").Bold(); text.Span(OriginalInvoiceNumber); });
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

                if (Tax > 0)
                {
                    table.Cell().ColumnSpan(3).Element(TotalLabelCell).Text("ضريبة القيمة المضافة (15%):");
                    table.Cell().Element(DataCell).Text($"{Tax:N2}");
                }

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
