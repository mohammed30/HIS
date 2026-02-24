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

    public decimal WorkingCapital => TotalAssets - TotalLiabilities;
    public decimal TotalLiabilitiesAndEquity => TotalLiabilities + TotalEquity;

    public class ReportLine
    {
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public decimal Amount { get; set; }
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
            page.Size(PageSizes.A4.Landscape());
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
            column.Item().Background(PrimaryBlue).Padding(12).Row(row =>
            {
                if (LogoBytes != null && LogoBytes.Length > 0)
                    row.ConstantItem(70).AlignMiddle().Image(LogoBytes).FitArea();
                else
                    row.ConstantItem(70);

                row.RelativeItem().AlignCenter().Column(col =>
                {
                    col.Item().Text("مستشفى آسيا").FontSize(22).Bold().FontColor(TextLight);
                    col.Item().Text("ASIA HOSPITAL").FontSize(12).FontColor(TextLight);
                    col.Item().PaddingTop(4).Text("القائمة العمومية / Balance Sheet")
                        .FontSize(11).FontColor(LightBlue);
                });

                row.ConstantItem(70);
            });

            column.Item().Background(LightBlue).Padding(8).AlignRight().Text(text =>
            {
                text.Span("في تاريخ: ").Bold().FontSize(10).FontColor(PrimaryBlue);
                text.Span($"{AsOfDate:yyyy/MM/dd}").FontSize(10);
            });
        });
    }

    void ComposeContent(IContainer container)
    {
        container.PaddingVertical(10).Column(column =>
        {
            // Side-by-side table
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.RelativeColumn(4); // Asset Name
                    cols.RelativeColumn(1); // Asset Code
                    cols.RelativeColumn(2); // Asset Amount
                    cols.RelativeColumn(4); // Liab/Equity Name
                    cols.RelativeColumn(1); // Liab Code
                    cols.RelativeColumn(2); // Liab Amount
                });

                // Header
                table.Header(header =>
                {
                    header.Cell().Element(HeaderCell).Text("الأصول / Assets").AlignRight();
                    header.Cell().Element(HeaderCell).Text("رمز");
                    header.Cell().Element(HeaderCell).Text("المبلغ");
                    header.Cell().Element(HeaderCell).Text("الخصوم وحقوق الملكية / Liabilities & Equity").AlignRight();
                    header.Cell().Element(HeaderCell).Text("رمز");
                    header.Cell().Element(HeaderCell).Text("المبلغ");
                });

                // Build combined liab+equity list
                var rightSide = new List<(string Name, string Code, decimal Amount, bool IsHeader)>();
                rightSide.Add(("الخصوم / Liabilities", "", 0, true));
                foreach (var l in LiabilityLines)
                    rightSide.Add((l.AccountName, l.AccountCode ?? "", l.Amount, false));
                rightSide.Add(("حقوق الملكية / Equity", "", 0, true));
                foreach (var e in EquityLines)
                    rightSide.Add((e.AccountName, e.AccountCode ?? "", e.Amount, false));

                int rows = Math.Max(AssetLines.Count, rightSide.Count);

                for (int i = 0; i < rows; i++)
                {
                    // Asset
                    if (i < AssetLines.Count)
                    {
                        table.Cell().Element(DataCellRight).Text(AssetLines[i].AccountName ?? "").AlignRight();
                        table.Cell().Element(DataCellCenter).Text(AssetLines[i].AccountCode ?? "");
                        table.Cell().Element(DataCellCenter).Text($"{AssetLines[i].Amount:N2}").FontColor(AccentGreen);
                    }
                    else
                    {
                        table.Cell().Element(DataCellRight).Text("");
                        table.Cell().Element(DataCellCenter).Text("");
                        table.Cell().Element(DataCellCenter).Text("");
                    }

                    // Liab/Equity
                    if (i < rightSide.Count)
                    {
                        var (name, code, amt, isHeader) = rightSide[i];
                        if (isHeader)
                        {
                            table.Cell().ColumnSpan(3).Background(LightBlue).Padding(5)
                                .Text(name).Bold().AlignRight().FontColor(PrimaryBlue).FontSize(9);
                        }
                        else
                        {
                            table.Cell().Element(DataCellRight).Text(name).AlignRight();
                            table.Cell().Element(DataCellCenter).Text(code);
                            table.Cell().Element(DataCellCenter).Text($"{amt:N2}").FontColor(AccentRed);
                        }
                    }
                    else
                    {
                        table.Cell().Element(DataCellRight).Text("");
                        table.Cell().Element(DataCellCenter).Text("");
                        table.Cell().Element(DataCellCenter).Text("");
                    }
                }

                // Totals row
                table.Cell().Background(PrimaryBlue).Padding(7).Text("إجمالي الأصول")
                    .Bold().AlignRight().FontColor(TextLight);
                table.Cell().Background(PrimaryBlue).Padding(7).Text("");
                table.Cell().Background(PrimaryBlue).Padding(7).Text($"{TotalAssets:N2}")
                    .Bold().FontColor(Colors.Green.Lighten3);
                table.Cell().Background(PrimaryBlue).Padding(7).Text("إجمالي الخصوم وحقوق الملكية")
                    .Bold().AlignRight().FontColor(TextLight);
                table.Cell().Background(PrimaryBlue).Padding(7).Text("");
                table.Cell().Background(PrimaryBlue).Padding(7).Text($"{TotalLiabilitiesAndEquity:N2}")
                    .Bold().FontColor(Colors.Yellow.Lighten3);
            });

            column.Item().Height(10);

            // Working Capital Highlight
            column.Item().Background(WorkingCapital >= 0 ? "#E8F5E9" : "#FFEBEE")
                .Border(2).BorderColor(WorkingCapital >= 0 ? AccentGreen : AccentRed)
                .Padding(12).Row(row =>
            {
                row.RelativeItem().AlignRight().Text("رأس المال العامل / Working Capital")
                    .Bold().FontSize(13).FontColor(PrimaryBlue);
                row.ConstantItem(200).AlignCenter().Text($"{WorkingCapital:N2} ج.م")
                    .Bold().FontSize(15)
                    .FontColor(WorkingCapital >= 0 ? AccentGreen : AccentRed);
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
