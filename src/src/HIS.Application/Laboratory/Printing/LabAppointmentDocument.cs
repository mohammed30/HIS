using System;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HIS.Laboratory.Printing;

public class LabAppointmentDocument : IDocument
{
    private static readonly string PrimaryBlue = "#003366";
    private static readonly string LightBlue = "#E6F2FF";
    private static readonly string TextDark = "#333333";
    private static readonly string TextLight = "#FFFFFF";
    private static readonly string BorderGray = "#CCCCCC";

    public string PatientName { get; set; }
    public string PatientId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string PreferredTime { get; set; }
    public string TestName { get; set; }
    public string TestCode { get; set; }
    public string PreparationInstructions { get; set; }
    public bool IsFasting { get; set; }
    public byte[] LogoBytes { get; set; }

    public DocumentMetadata GetMetadata() => new DocumentMetadata { Title = "Lab Appointment", Author = "Asia Hospital" };

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A5); // A5 is sufficient for appointment slips
            page.Margin(1, Unit.Centimetre);
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
        container.Column(col =>
        {
            col.Item().Background(PrimaryBlue).PaddingVertical(6).PaddingHorizontal(12).Row(row =>
            {
                if (LogoBytes != null && LogoBytes.Length > 0)
                    row.ConstantItem(40).AlignMiddle().Image(LogoBytes).FitArea();
                else
                    row.ConstantItem(40);
                
                row.RelativeItem().AlignCenter().Column(c =>
                {
                    c.Item().Text(text =>
                    {
                        text.Span("مستشفى آسيا  ").FontSize(16).Bold().FontColor(TextLight);
                        text.Span("ASIA HOSPITAL").FontSize(10).FontColor(TextLight);
                    });
                    c.Item().Text("تأكيد حجز موعد مختبر").FontSize(10).FontColor(LightBlue);
                });

                row.ConstantItem(40);
            });
        });
    }

    void ComposeContent(IContainer container)
    {
        container.PaddingVertical(10).Column(col =>
        {
            col.Item().Element(c => ComposeSection(c, "بيانات الحجز", x =>
            {
                x.Item().Text($"اسم المريض: {PatientName}").Bold();
                x.Item().Text($"رقم الملف: {PatientId}");
                x.Item().Text($"تاريخ الموعد: {AppointmentDate:yyyy/MM/dd}");
                if (!string.IsNullOrEmpty(PreferredTime))
                    x.Item().Text($"الوقت المفضل: {PreferredTime}");
            }));

            col.Item().Height(10);

            col.Item().Element(c => ComposeSection(c, "تفاصيل التحليل", x =>
            {
                x.Item().Text($"اسم التحليل: {TestName}");
                if (!string.IsNullOrEmpty(TestCode)) x.Item().Text($"الكود: {TestCode}");
                
                if (IsFasting)
                    x.Item().PaddingTop(5).Text("⚠ يتطلب صيام").FontColor(Colors.Red.Medium).Bold();
                
                if (!string.IsNullOrEmpty(PreparationInstructions))
                {
                    x.Item().PaddingTop(5).Text("تعليمات التحضير:").Bold();
                    x.Item().Text(PreparationInstructions).FontSize(10);
                }
            }));
        });
    }

    void ComposeSection(IContainer container, string title, Action<ColumnDescriptor> content)
    {
        container.Border(1).BorderColor(BorderGray).Column(c =>
        {
            c.Item().Background(LightBlue).Padding(5).Text(title).Bold().FontColor(PrimaryBlue);
            c.Item().Padding(10).Column(content);
        });
    }

    void ComposeFooter(IContainer container)
    {
        container.PaddingTop(10).AlignCenter().Text("الرجاء الحضور قبل الموعد بـ 15 دقيقة").FontSize(9);
    }
}
