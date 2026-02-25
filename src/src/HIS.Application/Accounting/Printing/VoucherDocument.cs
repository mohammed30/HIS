using System;
using System.Collections.Generic;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HIS.Accounting.Printing;

public class VoucherDocument : IDocument
{
    // Primary brand colors
    private static readonly string PrimaryBlue = "#003366";
    private static readonly string LightBlue = "#E6F2FF";
    private static readonly string AccentRed = "#DC3545";
    private static readonly string TextDark = "#333333";
    private static readonly string TextLight = "#FFFFFF";
    private static readonly string BorderGray = "#CCCCCC";

    public bool IsReceipt { get; set; } // True for Receipt, False for Payment
    public string VoucherTitle => IsReceipt ? "سند قبض / RECEIPT VOUCHER" : "سند صرف / PAYMENT VOUCHER";
    public string HeaderTitleBase => IsReceipt ? "سند قبض" : "سند صرف";

    public string VoucherNumber { get; set; }
    public DateTime Date { get; set; }
    public string PartyName { get; set; } // Patient/Payer or Supplier/Payee
    public string PartyLabel => IsReceipt ? "استلمنا من السيد/ة / Received From:" : "يُصرف للسيد/ة / Pay To:";
    
    public decimal TotalAmount { get; set; }
    public string AmountInWords { get; set; }
    public string Description { get; set; }
    public string PaymentMethodName { get; set; }

    public byte[] LogoBytes { get; set; }
    
    public List<VoucherLineModel> Lines { get; set; } = new();

    public class VoucherLineModel
    {
        public string AccountName { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }

    public DocumentMetadata GetMetadata() => new DocumentMetadata
    {
        Title = $"{HeaderTitleBase} {VoucherNumber}",
        Author = "Asia Hospital"
    };

    public void Compose(IDocumentContainer container)
    {
        container
            .Page(page =>
            {
                page.Size(PageSizes.A5.Landscape()); // Often vouchers are A5 Landscape
                page.Margin(1, Unit.Centimetre);
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
                });

                row.ConstantItem(40); // Spacer for balance
            });

            // Voucher Title Bar
            column.Item().Background(AccentRed).Padding(5).AlignCenter()
                .Text(VoucherTitle)
                .FontSize(14)
                .Bold()
                .FontColor(TextLight);
        });
    }

    void ComposeContent(IContainer container)
    {
        container.PaddingVertical(10).Column(column =>
        {
            // Info Row: Number, Date, Method
            column.Item().Row(row =>
            {
                row.RelativeItem().Text(t => { t.Span("رقم السند: ").Bold(); t.Span(VoucherNumber ?? "-"); });
                row.RelativeItem().Text(t => { t.Span("التاريخ: ").Bold(); t.Span(Date.ToString("yyyy/MM/dd")); });
                row.RelativeItem().Text(t => { t.Span("طريقة الدفع: ").Bold(); t.Span(PaymentMethodName ?? "-"); });
            });

            column.Item().PaddingVertical(10).LineHorizontal(1).LineColor(BorderGray);

            // Party and Amount
            column.Item().Row(row =>
            {
                row.RelativeItem().Column(col => 
                {
                    col.Item().Text(t => { t.Span(PartyLabel).Bold().FontSize(12); });
                    col.Item().PaddingTop(2).Text(PartyName ?? "-").FontSize(14).Bold().FontColor(PrimaryBlue);
                });
                
                row.ConstantItem(150).Background(LightBlue).Border(1).BorderColor(PrimaryBlue).Padding(10).AlignCenter().Column(col =>
                {
                    col.Item().Text("المبلغ / Amount").Bold().FontSize(10);
                    col.Item().Text($"{TotalAmount:N2} ج.م").Bold().FontSize(16).FontColor(PrimaryBlue);
                });
            });

            column.Item().PaddingTop(10).Text(t => { t.Span("مبلغ وقدره (فقط): ").Bold(); t.Span(AmountInWords ?? "-"); });
            column.Item().PaddingTop(5).Text(t => { t.Span("وذلك عن: ").Bold(); t.Span(Description ?? "-"); });

            column.Item().PaddingVertical(10).LineHorizontal(1).LineColor(BorderGray);

            // Account Allocations Table (if lines exist)
            if (Lines.Count > 0)
            {
                column.Item().PaddingBottom(5).Text("توجيه الحسابات").Bold().FontColor(PrimaryBlue);
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(2); // Account
                        columns.RelativeColumn(3); // Description
                        columns.RelativeColumn(1); // Amount
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(HeaderCell).Text("الحساب").Bold();
                        header.Cell().Element(HeaderCell).Text("البيان").Bold();
                        header.Cell().Element(HeaderCell).Text("المبلغ").Bold();
                    });

                    foreach (var item in Lines)
                    {
                        table.Cell().Element(DataCell).Text(item.AccountName ?? "-");
                        table.Cell().Element(DataCell).Text(item.Description ?? "-");
                        table.Cell().Element(DataCell).Text($"{item.Amount:N2}");
                    }
                });
            }
        });
    }

    void ComposeFooter(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().Row(row => 
            {
                row.RelativeItem().AlignCenter().Text("المحاسب / Accountant").Bold();
                row.RelativeItem().AlignCenter().Text("المستلم / Receiver").Bold(); // Or Payee
                row.RelativeItem().AlignCenter().Text("المدير المالي / Financial Manager").Bold();
            });
            column.Item().PaddingTop(15).LineHorizontal(2).LineColor(PrimaryBlue);
            column.Item().PaddingTop(5).AlignCenter().Text("مستشفى آسيا - قسم الحسابات").FontSize(9).Italic();
        });
    }

    static IContainer HeaderCell(IContainer container) => container.Background(LightBlue).BorderBottom(1).BorderColor(PrimaryBlue).Padding(4).AlignCenter().DefaultTextStyle(x => x.FontColor(PrimaryBlue).Bold().FontSize(10));
    static IContainer DataCell(IContainer container) => container.BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(4).AlignCenter().DefaultTextStyle(x => x.FontSize(10));
}
