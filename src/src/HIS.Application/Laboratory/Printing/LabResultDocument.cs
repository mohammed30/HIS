using System;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HIS.Laboratory.Printing;

public class LabResultDocument : IDocument
{
    public string PatientName { get; set; }
    public DateTime RequestDate { get; set; }
    public string TestName { get; set; }
    public string TestCode { get; set; }
    public string DoctorName { get; set; }
    public string Result { get; set; }
    public string Notes { get; set; }

    public DocumentMetadata GetMetadata() => new DocumentMetadata();

    public void Compose(IDocumentContainer container)
    {
        container
            .Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));
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
            row.RelativeItem().Column(column =>
            {
                column.Item().Text("مستشفى آسيا").FontSize(20).SemiBold().AlignCenter();
                column.Item().Text("تقرير نتيجة التحليل المخبري").FontSize(16).AlignCenter();
            });
        });
    }

    void ComposeContent(IContainer container)
    {
        container.PaddingVertical(20).Column(column =>
        {
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                table.Cell().Element(Block).Text($"اسم المريض: {PatientName}");
                table.Cell().Element(Block).Text($"التاريخ: {RequestDate:yyyy/MM/dd}");
                
                table.Cell().Element(Block).Text($"التحليل: {TestName}");
                table.Cell().Element(Block).Text($"الكود: {TestCode}");
                
                table.Cell().Element(Block).Text($"الطبيب: {DoctorName}");
                table.Cell().Element(Block).Text("");
            });

            column.Item().PaddingTop(20).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

            column.Item().PaddingTop(20).Text("النتيجة:").Bold();
            column.Item().Background(Colors.Grey.Lighten4).Padding(10).Text(Result ?? "-");

            if (!string.IsNullOrEmpty(Notes))
            {
                column.Item().PaddingTop(10).Text("ملاحظات:").Bold();
                column.Item().Text(Notes);
            }
        });
    }

    void ComposeFooter(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().PaddingTop(20).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
            
            column.Item().PaddingTop(10).Row(row =>
            {
               row.RelativeItem().AlignCenter().Text("توقيع الفني");
               row.RelativeItem().AlignCenter().Text("ختم المختبر");
            });
        });
    }
    
    static IContainer Block(IContainer container)
    {
        return container.PaddingBottom(5).PaddingRight(5); // Padding for table cells
    }
}
