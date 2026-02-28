using System;
using System.Collections.Generic;
using System.Linq;
using HIS.Inventory.Dtos;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HIS.Inventory.Printing;

public class StagnantStockReportDocument : IDocument
{
    private static readonly string PrimaryBlue = "#003366";
    private static readonly string LightBlue = "#E6F2FF";
    private static readonly string AccentRed = "#DC3545";
    private static readonly string TextDark = "#333333";
    private static readonly string TextLight = "#FFFFFF";
    private static readonly string BorderGray = "#CCCCCC";

    public List<StagnantStockReportDto> Items { get; set; }
    public string WarehouseName { get; set; }
    public int ThresholdDays { get; set; }
    public DateTime ReportDate { get; set; } = DateTime.Now;

    public DocumentMetadata GetMetadata() => new DocumentMetadata
    {
        Title = "تقرير المخزون الراكد",
        Author = "مستشفى آسيا"
    };

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(1, Unit.Centimetre);
            page.PageColor(Colors.White);
            page.DefaultTextStyle(x => x.FontSize(10).FontColor(TextDark));
            page.ContentFromRightToLeft();

            page.Header().Element(ComposeHeader);
            page.Content().Element(ComposeContent);
            page.Footer().Element(ComposeFooter);
        });
    }

    void ComposeHeader(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().Background(PrimaryBlue).Padding(10).Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("مستشفى آسيا").FontSize(16).Bold().FontColor(TextLight);
                    col.Item().Text("ASIA HOSPITAL").FontSize(10).FontColor(TextLight);
                });

                row.RelativeItem().AlignRight().Column(col =>
                {
                    col.Item().Text("تقرير المخزون الراكد").FontSize(16).Bold().FontColor(TextLight);
                    col.Item().Text("STAGNANT STOCK REPORT").FontSize(10).FontColor(TextLight);
                });
            });

            column.Item().PaddingVertical(5).LineHorizontal(2).LineColor(AccentRed);
        });
    }

    void ComposeContent(IContainer container)
    {
        container.PaddingVertical(10).Column(column =>
        {
            column.Item().Row(row =>
            {
                row.RelativeItem().Text(text =>
                {
                    text.Span("المخزن: ").Bold();
                    text.Span(string.IsNullOrEmpty(WarehouseName) ? "الكل" : WarehouseName);
                    text.Span(" | ").Bold();
                    text.Span("المدة (يوم): ").Bold();
                    text.Span(ThresholdDays.ToString());
                });
                row.RelativeItem().AlignRight().Text(text =>
                {
                    text.Span("التاريخ: ").Bold();
                    text.Span(ReportDate.ToString("yyyy/MM/dd HH:mm"));
                });
            });

            column.Item().PaddingTop(10).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3); // Product
                    columns.RelativeColumn(2); // Warehouse
                    columns.RelativeColumn(1); // Current Qty
                    columns.RelativeColumn(1.5f); // Last Transaction
                    columns.RelativeColumn(1); // Stagnant Days
                });

                table.Header(header =>
                {
                    header.Cell().Element(HeaderCell).Text("الصنف").Bold();
                    header.Cell().Element(HeaderCell).Text("المخزن").Bold();
                    header.Cell().Element(HeaderCell).Text("الرصيد الحالي").Bold();
                    header.Cell().Element(HeaderCell).Text("آخر حركة").Bold();
                    header.Cell().Element(HeaderCell).Text("أيام الركود").Bold();
                });

                foreach (var item in Items)
                {
                    table.Cell().Element(DataCell).AlignRight().Text(item.ProductName);
                    table.Cell().Element(DataCell).AlignCenter().Text(item.WarehouseName);
                    table.Cell().Element(DataCell).AlignCenter().Text(item.CurrentQuantity.ToString("0.##"));
                    table.Cell().Element(DataCell).AlignCenter().Text(item.LastTransactionDate?.ToString("yyyy/MM/dd") ?? "-");
                    table.Cell().Element(DataCell).AlignCenter().Text(item.DaysStagnant.ToString()).FontColor(item.DaysStagnant >= ThresholdDays ? AccentRed : TextDark);
                }
            });
        });
    }

    void ComposeFooter(IContainer container)
    {
        container.AlignCenter().Text(x =>
        {
            x.Span("صفحة ");
            x.CurrentPageNumber();
            x.Span(" من ");
            x.TotalPages();
        });
    }

    static IContainer HeaderCell(IContainer container) => container.Background(PrimaryBlue).Padding(5).AlignCenter().AlignMiddle().DefaultTextStyle(x => x.FontColor(TextLight));
    static IContainer DataCell(IContainer container) => container.BorderBottom(1).BorderColor(BorderGray).Padding(5).AlignMiddle();
}
