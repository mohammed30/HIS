using System;
using System.Collections.Generic;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HIS.Operations.Printing;

public class OperationsReportDocument : IDocument
{
    private static readonly string PrimaryBlue = "#003366";
    private static readonly string LightBlue = "#E6F2FF";
    private static readonly string TextDark = "#333333";
    private static readonly string BorderGray = "#CCCCCC";

    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public List<SurgicalOperationDto> Operations { get; set; } = new();
    public byte[] LogoBytes { get; set; }
    public string UserName { get; set; }

    public DocumentMetadata GetMetadata() => new DocumentMetadata
    {
        Title = "Operations Report",
        Author = "Asia Hospital",
        Creator = "HIS System"
    };

    public void Compose(IDocumentContainer container)
    {
        container
            .Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(9).FontColor(TextDark).FontFamily("Arial"));
                page.ContentFromRightToLeft(); // RTL for Arabic

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });
    }

    void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            if (LogoBytes != null && LogoBytes.Length > 0)
            {
                row.ConstantItem(60).AlignMiddle().Image(LogoBytes).FitArea();
            }
            else
            {
                row.ConstantItem(60);
            }

            row.RelativeItem().AlignCenter().Column(col =>
            {
                col.Item().Text("مستشفى آسيا - ASIA HOSPITAL").FontSize(16).Bold().FontColor(PrimaryBlue);
                col.Item().Text("تقرير العمليات الجراحية / SURGICAL OPERATIONS REPORT").FontSize(12).Bold();
                
                if (FromDate.HasValue || ToDate.HasValue)
                {
                    var period = "";
                    if (FromDate.HasValue) period += $"من: {FromDate.Value:yyyy/MM/dd} ";
                    if (ToDate.HasValue) period += $"إلى: {ToDate.Value:yyyy/MM/dd}";
                    col.Item().Text(period).FontSize(10);
                }
            });

            row.ConstantItem(80).AlignRight().Column(col =>
            {
                col.Item().Text($"تاريخ الاستخراج: {DateTime.Now:yyyy/MM/dd}").FontSize(8);
                col.Item().Text($"الوقت: {DateTime.Now:HH:mm}").FontSize(8);
            });
        });
    }

    void ComposeContent(IContainer container)
    {
        container.PaddingVertical(10).Column(column =>
        {
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(25);  // #
                    columns.ConstantColumn(80);  // Date
                    columns.RelativeColumn(2);   // Patient
                    columns.RelativeColumn(2);   // Operation
                    columns.RelativeColumn(1.5f);// Doctor
                    columns.RelativeColumn(1.5f);// Specialty
                    columns.ConstantColumn(70);  // Amount
                });

                // Header
                table.Header(header =>
                {
                    header.Cell().Element(HeaderStyle).Text("#");
                    header.Cell().Element(HeaderStyle).Text("التاريخ");
                    header.Cell().Element(HeaderStyle).Text("المريض");
                    header.Cell().Element(HeaderStyle).Text("العملية");
                    header.Cell().Element(HeaderStyle).Text("الطبيب");
                    header.Cell().Element(HeaderStyle).Text("التخصص");
                    header.Cell().Element(HeaderStyle).Text("المبلغ");

                    static IContainer HeaderStyle(IContainer c) => c.Background(PrimaryBlue).Padding(5).AlignCenter().DefaultTextStyle(x => x.FontColor(Colors.White).Bold());
                });

                // Rows
                for (int i = 0; i < Operations.Count; i++)
                {
                    var op = Operations[i];
                    table.Cell().Element(RowStyle).AlignCenter().Text((i + 1).ToString());
                    table.Cell().Element(RowStyle).Text(op.OperationDate.ToString("yyyy/MM/dd HH:mm"));
                    table.Cell().Element(RowStyle).Text(op.PatientName ?? "-");
                    table.Cell().Element(RowStyle).Text(op.OperationName);
                    table.Cell().Element(RowStyle).Text(op.DoctorName ?? "-");
                    table.Cell().Element(RowStyle).Text(op.SpecialtyName ?? "-");
                    table.Cell().Element(RowStyle).AlignRight().Text($"{op.TotalAmount:N2}");
                }

                static IContainer RowStyle(IContainer c) => c.BorderBottom(0.5f).BorderColor(BorderGray).Padding(5).AlignMiddle();
            });

            // Summary
            var totalAmount = 0m;
            foreach (var op in Operations) totalAmount += op.TotalAmount;

            column.Item().PaddingTop(10).AlignRight().Text(t =>
            {
                t.Span("إجمالي العمليات: ").Bold();
                t.Span($"{totalAmount:N2} ج.م").Bold().FontColor(PrimaryBlue);
                t.Span($"  ({Operations.Count} عملية)").FontSize(8);
            });
        });
    }

    void ComposeFooter(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Text(t =>
            {
                t.Span("المستخدم: ").FontSize(8);
                t.Span(UserName).FontSize(8).Bold();
            });
            row.RelativeItem().AlignRight().Text(t =>
            {
                t.Span("صفحة ").FontSize(8);
                t.CurrentPageNumber().FontSize(8);
                t.Span(" من ").FontSize(8);
                t.TotalPages().FontSize(8);
            });
        });
    }
}
