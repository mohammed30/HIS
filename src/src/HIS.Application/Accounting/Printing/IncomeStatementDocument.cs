using System;
using System.Collections.Generic;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HIS.Accounting.Printing;

public class IncomeStatementDocument : IDocument
{
    private static readonly string PrimaryBlue = "#003366";
    private static readonly string LightBlue = "#E6F2FF";
    private static readonly string AccentGreen = "#28a745";
    private static readonly string AccentRed = "#DC3545";
    private static readonly string TextDark = "#333333";
    private static readonly string TextLight = "#FFFFFF";
    private static readonly string BorderGray = "#CCCCCC";

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string PrintedBy { get; set; }
    public DateTime PrintedAt { get; set; }
    public byte[] LogoBytes { get; set; }

    // Revenue
    public List<ReportLine> RevenueLines { get; set; } = new();
    public decimal TotalRevenue { get; set; }

    // Cost of Sales
    public List<ReportLine> CostOfSalesLines { get; set; } = new();
    public decimal TotalCostOfSales { get; set; }

    // G&A Expenses
    public List<ReportLine> GaExpenseLines { get; set; } = new();
    public decimal TotalGaExpenses { get; set; }

    // Other revenue/expenses
    public List<ReportLine> OtherRevenueLines { get; set; } = new();
    public List<ReportLine> OtherExpenseLines { get; set; } = new();
    public decimal TotalOtherRevenues { get; set; }
    public decimal TotalOtherExpenses { get; set; }

    // Calculated
    public decimal GrossProfit => TotalRevenue - TotalCostOfSales;
    public decimal OperatingIncome => GrossProfit - TotalGaExpenses;
    public decimal NetIncome => OperatingIncome + TotalOtherRevenues - TotalOtherExpenses;

    public class ReportLine
    {
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public decimal Amount { get; set; }
    }

    public DocumentMetadata GetMetadata() => new DocumentMetadata
    {
        Title = "قائمة الدخل - Income Statement",
        Author = PrintedBy ?? "System"
    };

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(1.5f, Unit.Centimetre);
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
            // Blue header bar with logo + hospital name
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
                    col.Item().Text("قائمة الدخل / Income Statement")
                        .FontSize(9).FontColor(LightBlue);
                });

                row.ConstantItem(40);
            });

            // Report period bar
            column.Item().Background(LightBlue).Padding(8).Row(row =>
            {
                row.RelativeItem().AlignRight().Text(text =>
                {
                    text.Span("الفترة: ").Bold().FontSize(10).FontColor(PrimaryBlue);
                    text.Span($"{StartDate:yyyy/MM/dd}");
                    text.Span("  →  ").FontColor(PrimaryBlue);
                    text.Span($"{EndDate:yyyy/MM/dd}");
                });
            });
        });
    }

    void ComposeContent(IContainer container)
    {
        container.PaddingVertical(10).Column(column =>
        {
            // Revenue section
            if (RevenueLines.Count > 0)
            {
                column.Item().Element(c => ComposeSectionHeader(c, "الإيرادات / Revenue", AccentGreen));
                column.Item().Element(c => ComposeLineTable(c, RevenueLines, TotalRevenue, "إجمالي الإيرادات", AccentGreen, false));
                column.Item().Height(6);
            }

            // Cost of Sales
            if (CostOfSalesLines.Count > 0)
            {
                column.Item().Element(c => ComposeSectionHeader(c, "تكلفة البضاعة المباعة / COGS", AccentRed));
                column.Item().Element(c => ComposeLineTable(c, CostOfSalesLines, TotalCostOfSales, "إجمالي التكلفة", AccentRed, true));
                column.Item().Height(6);
            }

            // Gross Profit
            column.Item().Element(c => ComposeSummaryRow(c, "مجمل الربح / Gross Profit", GrossProfit));
            column.Item().Height(6);

            // G&A Expenses
            if (GaExpenseLines.Count > 0)
            {
                column.Item().Element(c => ComposeSectionHeader(c, "المصروفات العمومية والإدارية / G&A Expenses", AccentRed));
                column.Item().Element(c => ComposeLineTable(c, GaExpenseLines, TotalGaExpenses, "إجمالي المصروفات", AccentRed, true));
                column.Item().Height(6);
            }

            // Operating Income
            column.Item().Element(c => ComposeSummaryRow(c, "الدخل من العمليات / Operating Income", OperatingIncome));
            column.Item().Height(6);

            // Other Revenue & Expenses
            if (OtherRevenueLines.Count > 0 || OtherExpenseLines.Count > 0)
            {
                column.Item().Element(c => ComposeSectionHeader(c, "البنود الأخرى / Other Items", PrimaryBlue));
                
                if (OtherRevenueLines.Count > 0)
                    column.Item().Element(c => ComposeLineTable(c, OtherRevenueLines, TotalOtherRevenues, "إجمالي الإيرادات الأخرى", AccentGreen, false));
                
                if (OtherExpenseLines.Count > 0)
                    column.Item().Element(c => ComposeLineTable(c, OtherExpenseLines, TotalOtherExpenses, "إجمالي المصروفات الأخرى", AccentRed, true));
                
                column.Item().Height(6);
            }

            // Net Income (prominent row)
            column.Item().Background(PrimaryBlue).Padding(10).Row(row =>
            {
                row.RelativeItem().AlignRight().Text("صافي الربح / Net Income")
                    .FontSize(13).Bold().FontColor(TextLight);
                row.ConstantItem(150).AlignLeft().Text($"{NetIncome:N2} ج.م")
                    .FontSize(14).Bold().FontColor(NetIncome >= 0 ? Colors.Green.Lighten3 : Colors.Red.Lighten3);
            });
        });
    }

    void ComposeSectionHeader(IContainer container, string title, string color)
    {
        container.Background(color).Padding(7).Text(title)
            .Bold().FontSize(10).FontColor(TextLight);
    }

    void ComposeLineTable(IContainer container, List<ReportLine> lines, decimal total, string totalLabel, string totalColor, bool isNegative)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(cols =>
            {
                cols.RelativeColumn(1);   // Code
                cols.RelativeColumn(4);   // Name
                cols.RelativeColumn(2);   // Amount
            });

            // Header
            table.Header(header =>
            {
                header.Cell().Element(HeaderCell).Text("رمز الحساب");
                header.Cell().Element(HeaderCell).Text("اسم الحساب");
                header.Cell().Element(HeaderCell).Text("المبلغ");
            });

            // Lines
            foreach (var line in lines)
            {
                table.Cell().Element(DataCell).Text(line.AccountCode ?? "");
                table.Cell().Element(DataCellRight).Text(line.AccountName ?? "");
                table.Cell().Element(DataCell).Text($"{line.Amount:N2}");
            }

            // Total row
            table.Cell().ColumnSpan(2).Background(LightBlue).Padding(6).AlignRight()
                .Text(totalLabel).Bold().FontSize(10).FontColor(PrimaryBlue);
            table.Cell().Background(LightBlue).Padding(6).AlignCenter()
                .Text(isNegative ? $"({total:N2})" : $"{total:N2}")
                .Bold().FontSize(10).FontColor(totalColor);
        });
    }

    void ComposeSummaryRow(IContainer container, string label, decimal amount)
    {
        container.Background("#F0F4FF").Border(1).BorderColor(BorderGray).Padding(8).Row(row =>
        {
            row.RelativeItem().AlignRight().Text(label).Bold().FontSize(11).FontColor(PrimaryBlue);
            row.ConstantItem(150).AlignCenter().Text($"{amount:N2} ج.م")
                .Bold().FontSize(11).FontColor(amount >= 0 ? AccentGreen : AccentRed);
        });
    }

    void ComposeFooter(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().LineHorizontal(1).LineColor(PrimaryBlue);
            column.Item().PaddingTop(6).Row(row =>
            {
                row.RelativeItem().AlignRight().Text(text =>
                {
                    text.Span("طُبع بواسطة: ").Bold().FontSize(8).FontColor(PrimaryBlue);
                    text.Span(PrintedBy ?? "System").FontSize(8);
                });
                row.RelativeItem().AlignCenter().Text("مستشفى آسيا")
                    .FontSize(8).Italic().FontColor(BorderGray);
                row.RelativeItem().AlignLeft().Text(text =>
                {
                    text.Span("تاريخ الطباعة: ").Bold().FontSize(8).FontColor(PrimaryBlue);
                    text.Span($"{PrintedAt:yyyy/MM/dd HH:mm}").FontSize(8);
                });
            });
        });
    }

    static IContainer HeaderCell(IContainer container) =>
        container.Background(PrimaryBlue).Padding(6).AlignCenter()
            .DefaultTextStyle(x => x.FontColor(TextLight).Bold().FontSize(9));

    static IContainer DataCell(IContainer container) =>
        container.BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(6).AlignCenter();

    static IContainer DataCellRight(IContainer container) =>
        container.BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(6).AlignRight();
}
