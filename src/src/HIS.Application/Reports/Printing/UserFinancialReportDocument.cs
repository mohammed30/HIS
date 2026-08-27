using System;
using System.Linq;
using HIS.Reports;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HIS.Reports.Printing;

public class UserFinancialReportDocument : IDocument
{
    private static readonly string PrimaryBlue = "#003366";
    private static readonly string LightBlue = "#E6F2FF";
    private static readonly string TextDark = "#333333";
    private static readonly string TextLight = "#FFFFFF";
    private static readonly string BorderGray = "#CCCCCC";

    public UserFinancialReportPrintDataDto ReportData { get; set; }
    public byte[] LogoBytes { get; set; }

    public DocumentMetadata GetMetadata() => new DocumentMetadata
    {
        Title = $"تقرير عهدة المستخدم - {ReportData?.UserName ?? "الكل"}",
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
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial").FontColor(TextDark)); 
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
            column.Item().Background(PrimaryBlue).PaddingVertical(10).PaddingHorizontal(15).Row(row =>
            {
                if (LogoBytes != null && LogoBytes.Length > 0)
                    row.ConstantItem(50).AlignMiddle().Image(LogoBytes).FitArea();
                else
                    row.ConstantItem(50);

                row.RelativeItem().AlignCenter().Column(col =>
                {
                    col.Item().Text(text =>
                    {
                        text.Span("مستشفى آسيا  ").FontSize(18).Bold().FontColor(TextLight);
                        text.Span("ASIA HOSPITAL").FontSize(12).FontColor(TextLight);
                    });
                    
                    col.Item().PaddingTop(2).Text("تقرير تسليم عهدة مستخدم / USER CUSTODY REPORT")
                        .FontSize(12)
                        .FontColor(LightBlue);
                });

                row.ConstantItem(50);
            });

            column.Item().PaddingTop(15).PaddingBottom(10).Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text(text => { text.Span("المستخدم: ").Bold(); text.Span(ReportData?.UserName ?? "الكل"); });
                    col.Item().Text(text => { text.Span("من تاريخ: ").Bold(); text.Span(ReportData?.StartDate?.ToString("yyyy/MM/dd") ?? "-"); });
                    col.Item().Text(text => { text.Span("إلى تاريخ: ").Bold(); text.Span(ReportData?.EndDate?.ToString("yyyy/MM/dd") ?? "-"); });
                });
            });
        });
    }

    void ComposeContent(IContainer container)
    {
        var cashTransactions = ReportData.Transactions.Where(x => x.PaymentCategory == "Cash").ToList();
        var bankTransactions = ReportData.Transactions.Where(x => x.PaymentCategory == "Bank").ToList();

        container.PaddingVertical(10).Column(column =>
        {
            if (cashTransactions.Any())
            {
                column.Item().PaddingBottom(5).Text("نقداً (Cash)").FontSize(14).Bold().FontColor(PrimaryBlue);
                column.Item().PaddingBottom(15).Element(c => ComposeSummaryTable(c, cashTransactions));
            }

            if (bankTransactions.Any())
            {
                column.Item().PaddingBottom(5).Text("بنك / شبكة (POS)").FontSize(14).Bold().FontColor(PrimaryBlue);
                column.Item().PaddingBottom(15).Element(c => ComposeSummaryTable(c, bankTransactions));
            }

            ComposeGrandTotals(column.Item(), cashTransactions, bankTransactions);

            ComposeSignatures(column.Item());
        });
    }

    void ComposeSummaryTable(IContainer container, System.Collections.Generic.List<UserFinancialTransactionDto> transactions)
    {
        var grouped = transactions
            .GroupBy(x => x.TransactionType ?? "أخرى")
            .Select(g => new { Module = g.Key, Total = g.Sum(x => x.Amount) })
            .ToList();

        var totalIncoming = transactions.Where(x => x.Amount > 0).Sum(x => x.Amount);
        var totalOutgoing = Math.Abs(transactions.Where(x => x.Amount < 0).Sum(x => x.Amount));
        var netTotal = totalIncoming - totalOutgoing;

        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn();
                columns.ConstantColumn(120);
            });

            table.Header(header =>
            {
                header.Cell().Element(CellStyle).Text("الخدمة / الموديول").Bold();
                header.Cell().Element(CellStyle).AlignRight().Text("الإجمالي").Bold();
                
                static IContainer CellStyle(IContainer container)
                {
                    return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(BorderGray);
                }
            });

            foreach (var item in grouped)
            {
                table.Cell().Element(CellStyle).Text(item.Module);
                table.Cell().Element(CellStyle).AlignRight().Text($"{item.Total:N2}");
            }

            table.Cell().Element(FooterStyle).Text("إجمالي المقبوضات").Bold();
            table.Cell().Element(FooterStyle).AlignRight().Text($"{totalIncoming:N2}").Bold();

            table.Cell().Element(FooterStyle).Text("إجمالي المدفوعات/المسترد").Bold();
            table.Cell().Element(FooterStyle).AlignRight().Text($"{totalOutgoing:N2}").FontColor(Colors.Red.Darken2).Bold();

            table.Cell().Element(FooterStyle).Text("الصافي").Bold();
            table.Cell().Element(FooterStyle).AlignRight().Text($"{netTotal:N2}").Bold();

            static IContainer CellStyle(IContainer container)
            {
                return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(4);
            }
            static IContainer FooterStyle(IContainer container)
            {
                return container.Background(Colors.Grey.Lighten4).BorderBottom(1).BorderColor(BorderGray).PaddingVertical(5);
            }
        });
    }

    void ComposeGrandTotals(IContainer container, System.Collections.Generic.List<UserFinancialTransactionDto> cashTransactions, System.Collections.Generic.List<UserFinancialTransactionDto> bankTransactions)
    {
        var allNet = cashTransactions.Sum(x => x.Amount) + bankTransactions.Sum(x => x.Amount);

        container.PaddingTop(20).Row(row =>
        {
            row.RelativeItem();
            row.ConstantItem(250).Border(1).BorderColor(PrimaryBlue).Padding(10).Column(col =>
            {
                col.Item().Row(r =>
                {
                    r.RelativeItem().Text("إجمالي الوردية (الكل):").Bold();
                    r.RelativeItem().AlignRight().Text($"{allNet:N2}").Bold().FontSize(12);
                });
            });
        });
    }

    void ComposeSignatures(IContainer container)
    {
        container.PaddingTop(50).Row(row =>
        {
            row.RelativeItem().AlignCenter().Column(col =>
            {
                col.Item().Text("توقيع المستلم (المحاسب)").Bold();
                col.Item().PaddingTop(20).Text("___________________________");
            });

            row.RelativeItem().AlignCenter().Column(col =>
            {
                col.Item().Text("توقيع المُسلّم (المستخدم)").Bold();
                col.Item().PaddingTop(20).Text("___________________________");
            });
        });
    }

    void ComposeFooter(IContainer container)
    {
        container.AlignCenter().Text(x =>
        {
            x.Span("تاريخ الطباعة: ").FontSize(9);
            x.Span(ReportData.PrintDate.ToString("yyyy/MM/dd hh:mm tt")).FontSize(9);
            x.Span(" - مستشفى آسيا").FontSize(9);
        });
    }
}
