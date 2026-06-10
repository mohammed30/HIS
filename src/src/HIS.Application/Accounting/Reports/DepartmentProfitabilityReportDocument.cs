using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HIS.Accounting.Reports;

public class DepartmentProfitabilityReportDocument : IDocument
{
    private static readonly string PrimaryBlue = "#003366";
    private static readonly string LightBlue = "#E6F2FF";
    private static readonly string BorderGray = "#CCCCCC";
    
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string UserName { get; set; }
    public List<DepartmentProfitabilityDto> Items { get; set; } = new();

    public DocumentMetadata GetMetadata() => new DocumentMetadata
    {
        Title = "تقرير ربحية الأقسام",
        Author = "نظام المستشفى"
    };

    public void Compose(IDocumentContainer container)
    {
        container
            .Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11).FontColor(Colors.Black).FontFamily("Arial"));
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
            column.Item().Text("تقرير أرباح وخسائر الأقسام (Department P&L)")
                .FontSize(20).SemiBold().FontColor(PrimaryBlue).AlignCenter();
                
            column.Item().PaddingTop(5).Text($"الفترة من: {StartDate:yyyy/MM/dd} إلى: {EndDate:yyyy/MM/dd}")
                .FontSize(12).AlignCenter();
                
            column.Item().PaddingTop(10).LineHorizontal(1).LineColor(BorderGray);
        });
    }

    void ComposeContent(IContainer container)
    {
        container.PaddingVertical(10).Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(2); // Center Name
                columns.RelativeColumn(1); // Revenue
                columns.RelativeColumn(1); // Expense
                columns.RelativeColumn(1); // Net Profit
            });

            table.Header(header =>
            {
                header.Cell().Element(HeaderCellStyle).Text("مركز التكلفة / القسم");
                header.Cell().Element(HeaderCellStyle).Text("إجمالي الإيرادات");
                header.Cell().Element(HeaderCellStyle).Text("إجمالي المصروفات");
                header.Cell().Element(HeaderCellStyle).Text("صافي الربح / الخسارة");
            });

            decimal totalRev = 0, totalExp = 0, totalNet = 0;

            foreach (var item in Items.OrderByDescending(x => x.NetProfit))
            {
                var netProfitColor = item.NetProfit >= 0 ? Colors.Green.Darken2 : Colors.Red.Darken2;
                
                table.Cell().Element(CellStyle).Text(item.CostCenterName);
                table.Cell().Element(CellStyle).Text(item.TotalRevenue.ToString("N2"));
                table.Cell().Element(CellStyle).Text(item.TotalExpense.ToString("N2"));
                table.Cell().Element(CellStyle).Text(item.NetProfit.ToString("N2")).FontColor(netProfitColor).SemiBold();

                totalRev += item.TotalRevenue;
                totalExp += item.TotalExpense;
                totalNet += item.NetProfit;
            }

            // Totals row
            var totalColor = totalNet >= 0 ? Colors.Green.Darken2 : Colors.Red.Darken2;
            table.Cell().Element(TotalCellStyle).Text("الإجمالي الكلي").SemiBold();
            table.Cell().Element(TotalCellStyle).Text(totalRev.ToString("N2")).SemiBold();
            table.Cell().Element(TotalCellStyle).Text(totalExp.ToString("N2")).SemiBold();
            table.Cell().Element(TotalCellStyle).Text(totalNet.ToString("N2")).FontColor(totalColor).Bold();
        });
    }

    void ComposeFooter(IContainer container)
    {
        container.AlignCenter().Text(x =>
        {
            x.Span("تاريخ الطباعة: ").FontSize(10);
            x.Span(DateTime.Now.ToString("yyyy/MM/dd HH:mm")).FontSize(10);
            x.Span(" | المستخدم: ").FontSize(10);
            x.Span(UserName).FontSize(10);
            x.Span(" | صفحة ").FontSize(10);
            x.CurrentPageNumber().FontSize(10);
            x.Span(" من ").FontSize(10);
            x.TotalPages().FontSize(10);
        });
    }

    static IContainer HeaderCellStyle(IContainer container)
    {
        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
    }
    
    static IContainer CellStyle(IContainer container)
    {
        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
    }

    static IContainer TotalCellStyle(IContainer container)
    {
        return container.Background(LightBlue).BorderTop(1).BorderColor(Colors.Black).PaddingVertical(5);
    }
}
