using System;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HIS.Laboratory.Printing;

public class LabRequestDocument : IDocument
{
    private static readonly string PrimaryBlue = "#003366";
    private static readonly string LightBlue = "#E6F2FF";
    private static readonly string TextDark = "#333333";
    private static readonly string TextLight = "#FFFFFF";
    private static readonly string BorderGray = "#CCCCCC";

    public string PatientName { get; set; }
    public string PatientId { get; set; }
    public string DoctorName { get; set; }
    public DateTime RequestDate { get; set; }
    public string TestName { get; set; }
    public string TestCode { get; set; }
    public string Status { get; set; }
    public byte[] LogoBytes { get; set; }

    public DocumentMetadata GetMetadata() => new DocumentMetadata { Title = "Lab Order Slip", Author = "Asia Hospital" };

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A5);
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
            col.Item().Background(PrimaryBlue).Padding(10).Row(row =>
            {
                if (LogoBytes != null) row.ConstantItem(60).Image(LogoBytes).FitArea();
                
                row.RelativeItem().AlignCenter().Column(c =>
                {
                    c.Item().Text("مستشفى آسيا").FontSize(18).Bold().FontColor(TextLight);
                    c.Item().Text("طلب تحليل مخبري / Lab Order").FontSize(12).FontColor(LightBlue);
                });
            });
        });
    }

    void ComposeContent(IContainer container)
    {
        container.PaddingVertical(10).Column(col =>
        {
            col.Item().Row(r =>
            {
                r.RelativeItem().Element(c => ComposeSection(c, "المريض", x =>
                {
                    x.Item().Text(PatientName).Bold();
                    x.Item().Text($"#{PatientId}");
                }));
                
                r.ConstantItem(10);
                
                r.RelativeItem().Element(c => ComposeSection(c, "الطبيب الطالب", x =>
                {
                    x.Item().Text(DoctorName).Bold();
                    x.Item().Text($"{RequestDate:yyyy/MM/dd}");
                }));
            });

            col.Item().Height(10);

            col.Item().Element(c => ComposeSection(c, "التحاليل المطلوبة", x =>
            {
                x.Item().Table(table =>
                {
                    table.ColumnsDefinition(cd =>
                    {
                        cd.RelativeColumn(3);
                        cd.RelativeColumn(1);
                    });
                    
                    table.Header(h =>
                    {
                        h.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("الاسم").Bold();
                        h.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("الكود").Bold();
                    });
                    
                    table.Cell().BorderBottom(1).BorderColor(BorderGray).Padding(5).Text(TestName);
                    table.Cell().BorderBottom(1).BorderColor(BorderGray).Padding(5).Text(TestCode);
                });
            }));
            
            col.Item().Height(20);
            
            // Space for technician notes
            col.Item().Border(1).BorderColor(BorderGray).Background(Colors.Grey.Lighten5).Padding(10).Height(2, Unit.Centimetre).Text("ملاحظات الفني / النتائج الأولية:").FontSize(10).FontColor(Colors.Grey.Medium);
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
        container.PaddingTop(10).Row(row =>
        {
             row.RelativeItem().Text($"Status: {Status}").FontSize(9);
             row.RelativeItem().AlignLeft().Text($"Printed: {DateTime.Now:yyyy/MM/dd HH:mm}").FontSize(9);
        });
    }
}
