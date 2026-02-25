using System;
using System.Collections.Generic;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HIS.Accounting.Printing;

public class BalanceSheetDocument : IDocument
{
    private static readonly string PrimaryBlue = "#003366";
    private static readonly string LightBlue = "#E6F2FF";
    private static readonly string AccentGreen = "#28a745";
    private static readonly string AccentRed = "#DC3545";
    private static readonly string TextDark = "#333333";
    private static readonly string TextLight = "#FFFFFF";
    private static readonly string BorderGray = "#CCCCCC";

    public DateTime AsOfDate { get; set; }
    public string PrintedBy { get; set; }
    public DateTime PrintedAt { get; set; }
    public byte[] LogoBytes { get; set; }

    public List<ReportLine> AssetLines { get; set; } = new();
    public List<ReportLine> LiabilityLines { get; set; } = new();
    public List<ReportLine> EquityLines { get; set; } = new();

    public decimal TotalAssets { get; set; }
    public decimal TotalLiabilities { get; set; }
    public decimal TotalEquity { get; set; }
    
    public decimal TotalPreviousAssets { get; set; }
    public decimal TotalPreviousLiabilities { get; set; }
    public decimal TotalPreviousEquity { get; set; }

    public decimal WorkingCapital => TotalAssets - TotalLiabilities;
    public decimal WorkingCapitalPrevious => TotalPreviousAssets - TotalPreviousLiabilities;
    public decimal TotalLiabilitiesAndEquity => TotalLiabilities + TotalEquity;
    public decimal TotalPreviousLiabilitiesAndEquity => TotalPreviousLiabilities + TotalPreviousEquity;

    public class ReportLine
    {
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public decimal Amount { get; set; }
        public decimal PreviousAmount { get; set; }
    }

    public DocumentMetadata GetMetadata() => new DocumentMetadata
    {
        Title = "القائمة العمومية - Balance Sheet",
        Author = PrintedBy ?? "System"
    };

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4.Portrait());
            page.Margin(1.5f, Unit.Centimetre);
            page.PageColor(Colors.White);
            page.DefaultTextStyle(x => x.FontSize(9).FontColor(TextDark));
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
                    col.Item().Text("القائمة العمومية / Balance Sheet")
                        .FontSize(9).FontColor(LightBlue);
                });

                row.ConstantItem(40);
            });

            column.Item().Background(LightBlue).PaddingVertical(4).PaddingHorizontal(8).AlignRight().Text(text =>
            {
                text.Span("في تاريخ: ").Bold().FontSize(9).FontColor(PrimaryBlue);
                text.Span($"{AsOfDate:yyyy/MM/dd}").FontSize(9);
            });
        });
    }

    void ComposeContent(IContainer container)
    {
        container.PaddingVertical(10).Column(column =>
        {
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.RelativeColumn(5); // Description
                    cols.RelativeColumn(2); // Account Code
                    cols.RelativeColumn(3); // Current Amount
                    cols.RelativeColumn(3); // Previous Amount
                });

                table.Header(header =>
                {
                    header.Cell().Element(HeaderCell).Text("البيان / Description").AlignRight();
                    header.Cell().Element(HeaderCell).Text("الرمز / Code");
                    header.Cell().Element(HeaderCell).Text("الرصيد الحالي").AlignRight();
                    header.Cell().Element(HeaderCell).Text("الرصيد السابق").AlignRight();
                });

                // --- Assets Section ---
                table.Cell().ColumnSpan(4).Background(PrimaryBlue).Padding(5).Text("الأصول / Assets").Bold().AlignRight().FontColor(TextLight).FontSize(10);
                foreach (var line in AssetLines)
                {
                    table.Cell().Element(DataCellRight).Text(line.AccountName ?? "").AlignRight();
                    table.Cell().Element(DataCellCenter).Text(line.AccountCode ?? "");
                    table.Cell().Element(DataCellRight).Text($"{line.Amount:N2}").FontColor(AccentGreen).Bold();
                    table.Cell().Element(DataCellRight).Text($"{line.PreviousAmount:N2}").FontColor(AccentGreen);
                }
                table.Cell().ColumnSpan(2).Background(LightBlue).Padding(6).AlignRight().Text("إجمالي الأصول").Bold();
                table.Cell().Background(LightBlue).Padding(6).AlignRight().Text($"{TotalAssets:N2}").Bold().FontColor(AccentGreen);
                table.Cell().Background(LightBlue).Padding(6).AlignRight().Text($"{TotalPreviousAssets:N2}").Bold().FontColor(AccentGreen);

                // --- Liabilities Section ---
                table.Cell().ColumnSpan(4).Background(PrimaryBlue).Padding(5).Text("الخصوم / Liabilities").Bold().AlignRight().FontColor(TextLight).FontSize(10);
                foreach (var line in LiabilityLines)
                {
                    table.Cell().Element(DataCellRight).Text(line.AccountName ?? "").AlignRight();
                    table.Cell().Element(DataCellCenter).Text(line.AccountCode ?? "");
                    table.Cell().Element(DataCellRight).Text($"{line.Amount:N2}").FontColor(AccentRed).Bold();
                    table.Cell().Element(DataCellRight).Text($"{line.PreviousAmount:N2}").FontColor(AccentRed);
                }
                table.Cell().ColumnSpan(2).Background(LightBlue).Padding(6).AlignRight().Text("إجمالي الخصوم").Bold();
                table.Cell().Background(LightBlue).Padding(6).AlignRight().Text($"{TotalLiabilities:N2}").Bold().FontColor(AccentRed);
                table.Cell().Background(LightBlue).Padding(6).AlignRight().Text($"{TotalPreviousLiabilities:N2}").Bold().FontColor(AccentRed);

                // --- Equity Section ---
                table.Cell().ColumnSpan(4).Background(PrimaryBlue).Padding(5).Text("حقوق الملكية / Equity").Bold().AlignRight().FontColor(TextLight).FontSize(10);
                foreach (var line in EquityLines)
                {
                    table.Cell().Element(DataCellRight).Text(line.AccountName ?? "").AlignRight();
                    table.Cell().Element(DataCellCenter).Text(line.AccountCode ?? "");
                    table.Cell().Element(DataCellRight).Text($"{line.Amount:N2}").FontColor(Colors.Orange.Medium).Bold();
                    table.Cell().Element(DataCellRight).Text($"{line.PreviousAmount:N2}").FontColor(Colors.Orange.Medium);
                }
                table.Cell().ColumnSpan(2).Background(LightBlue).Padding(6).AlignRight().Text("إجمالي حقوق الملكية").Bold();
                table.Cell().Background(LightBlue).Padding(6).AlignRight().Text($"{TotalEquity:N2}").Bold().FontColor(Colors.Orange.Medium);
                table.Cell().Background(LightBlue).Padding(6).AlignRight().Text($"{TotalPreviousEquity:N2}").Bold().FontColor(Colors.Orange.Medium);

                // --- Grand Totals ---
                table.Cell().ColumnSpan(2).Background(PrimaryBlue).Padding(8).AlignRight().Text("إجمالي الخصوم وحقوق الملكية").Bold().FontColor(TextLight);
                table.Cell().Background(PrimaryBlue).Padding(8).AlignRight().Text($"{TotalLiabilitiesAndEquity:N2}").Bold().FontColor(Colors.Yellow.Medium);
                table.Cell().Background(PrimaryBlue).Padding(8).AlignRight().Text($"{TotalPreviousLiabilitiesAndEquity:N2}").Bold().FontColor(Colors.Yellow.Medium);
            });

            column.Item().Height(10);

            // Working Capital Highlight
            column.Item().Border(1).BorderColor(PrimaryBlue).Padding(10).Row(row =>
            {
                row.RelativeItem().AlignRight().Column(c => {
                    c.Item().Text("رأس المال العامل / Working Capital").Bold().FontSize(12).FontColor(PrimaryBlue);
                    c.Item().Text("(أصول − خصوم)").FontSize(8).FontColor(Colors.Grey.Medium);
                });
                
                row.RelativeItem().AlignCenter().Column(c => {
                    c.Item().Text("الحالي").AlignCenter().FontSize(8).FontColor(PrimaryBlue);
                    c.Item().Text($"{WorkingCapital:N2} ج.م").Bold().FontSize(13).FontColor(WorkingCapital >= 0 ? AccentGreen : AccentRed);
                });

                row.RelativeItem().AlignLeft().Column(c => {
                    c.Item().Text("السابق").AlignCenter().FontSize(8).FontColor(PrimaryBlue);
                    c.Item().Text($"{WorkingCapitalPrevious:N2} ج.م").Bold().FontSize(13).FontColor(WorkingCapitalPrevious >= 0 ? AccentGreen : AccentRed);
                });
            });
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

    static IContainer HeaderCell(IContainer c) =>
        c.Background(PrimaryBlue).Padding(7).AlignCenter()
         .DefaultTextStyle(x => x.FontColor(TextLight).Bold().FontSize(9));

    static IContainer DataCellRight(IContainer c) =>
        c.BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(6).AlignRight();

    static IContainer DataCellCenter(IContainer c) =>
        c.BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(6).AlignCenter();
}
