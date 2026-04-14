using System;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HIS.Radiology.Printing;

public class RadiologyResultDocument : IDocument
{
    private static readonly string PrimaryBlue = "#003366";
    private static readonly string LightBlue = "#E6F2FF";
    private static readonly string AccentRed = "#DC3545";
    private static readonly string TextDark = "#333333";
    private static readonly string TextLight = "#FFFFFF";
    private static readonly string BorderGray = "#CCCCCC";

    public string PatientName { get; set; }
    public string PatientId { get; set; }
    public DateTime RequestDate { get; set; }
    public DateTime? ReportDate { get; set; }
    public string CustomRequestNumber { get; set; }
    public string RadiologyItemName { get; set; }
    public string DoctorName { get; set; }
    public string RadiologistName { get; set; }
    public string ReportBody { get; set; }
    public string TechnicianNotes { get; set; }
    public byte[] LogoBytes { get; set; }

    public DocumentMetadata GetMetadata() => new DocumentMetadata
    {
        Title = "Radiology Report",
        Author = "Asia Hospital"
    };

    public void Compose(IDocumentContainer container)
    {
        container
            .Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11).FontColor(TextDark));
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
                    col.Item().PaddingTop(2).Text("نظام معلومات المستشفى")
                        .FontSize(9).FontColor(LightBlue);
                });

                row.ConstantItem(40);
            });

            column.Item().Background(AccentRed).Padding(4).AlignCenter()
                .Text("تقرير أشعة")
                .FontSize(14).Bold().FontColor(TextLight);
        });
    }

    void ComposeContent(IContainer container)
    {
        container.PaddingVertical(15).Column(column =>
        {
            // Patient Info
            column.Item().Border(1).BorderColor(BorderGray).Padding(10).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                table.Cell().Text(text => { text.Span("المريض: ").Bold(); text.Span(PatientName); });
                table.Cell().Text(text => { text.Span("رقم الملف: ").Bold(); text.Span(PatientId); });
                table.Cell().Text(text => { text.Span("رقم الطلب: ").Bold(); text.Span(CustomRequestNumber); });
                table.Cell().Text(text => { text.Span("تاريخ الفحص: ").Bold(); text.Span(RequestDate.ToString("yyyy/MM/dd")); });
                table.Cell().Text(text => { text.Span("نوع الأشعة: ").Bold(); text.Span(RadiologyItemName); });
                table.Cell().Text(text => { text.Span("الطبيب المعالج: ").Bold(); text.Span(DoctorName); });
            });

            column.Item().Height(20);

            // Report Body
            column.Item().PaddingBottom(5).Text("نص التقرير الطبي").FontSize(13).Bold().Underline();
            column.Item().PaddingLeft(10).Text(ReportBody ?? "لا يوجد نص للتقرير").LineHeight(1.5f);

            if (!string.IsNullOrEmpty(TechnicianNotes))
            {
                column.Item().PaddingTop(20).Text("ملاحظات الفني:").FontSize(10).Bold();
                column.Item().PaddingLeft(10).Text(TechnicianNotes).FontSize(10).Italic();
            }

            column.Item().PaddingTop(40).Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().AlignCenter().Text("طبيب الأشعة المسؤول").Bold();
                    col.Item().AlignCenter().PaddingTop(10).Text(RadiologistName ?? "............................");
                    col.Item().AlignCenter().Text(ReportDate?.ToString("yyyy/MM/dd") ?? "");
                });

                row.RelativeItem().AlignCenter().Column(col =>
                {
                    col.Item().AlignCenter().Text("ختم القسم").Bold();
                    col.Item().Height(60);
                });
            });
        });
    }

    void ComposeFooter(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().LineHorizontal(1).LineColor(PrimaryBlue);
            column.Item().PaddingTop(5).Row(row =>
            {
                row.RelativeItem().Text("مستشفى آسيا - قسم الأشعة").FontSize(8).FontColor(Colors.Grey.Medium);
                row.RelativeItem().AlignCenter().Text(text =>
                {
                    text.Span("صفحة ").FontSize(8);
                    text.CurrentPageNumber().FontSize(8);
                    text.Span(" من ").FontSize(8);
                    text.TotalPages().FontSize(8);
                });
                row.RelativeItem().AlignLeft().Text(DateTime.Now.ToString("yyyy/MM/dd HH:mm")).FontSize(8);
            });
        });
    }
}
