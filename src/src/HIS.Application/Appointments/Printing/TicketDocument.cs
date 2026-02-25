using System;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HIS.Appointments.Printing;

public class TicketDocument : IDocument
{
    // Primary brand colors
    private static readonly string PrimaryBlue = "#003366";
    private static readonly string LightBlue = "#E6F2FF";
    private static readonly string TextDark = "#333333";
    private static readonly string TextLight = "#FFFFFF";
    private static readonly string BorderGray = "#CCCCCC";

    public string TicketNumber { get; set; }
    public DateTime Date { get; set; }
    public string PatientName { get; set; }
    public string PatientFileNumber { get; set; }
    public string ClinicName { get; set; }
    public string DoctorName { get; set; }
    public string ServiceName { get; set; }
    public decimal Amount { get; set; }
    public string UserName { get; set; }
    public byte[] LogoBytes { get; set; }

    public DocumentMetadata GetMetadata() => new DocumentMetadata
    {
        Title = $"Ticket {TicketNumber}",
        Author = "Asia Hospital"
    };

    public void Compose(IDocumentContainer container)
    {
        container
            .Page(page =>
            {
                page.Size(PageSizes.A6); // Standard thermal/receipt size
                page.Margin(0.5f, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10).FontColor(TextDark).FontFamily("Arial"));
                page.ContentFromRightToLeft(); // RTL
                
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });
    }

    void ComposeContent(IContainer container)
    {
        container.PaddingVertical(5).Column(column =>
        {
            // Header with Logo
            column.Item().Row(row =>
            {
                if (LogoBytes != null && LogoBytes.Length > 0)
                {
                    row.ConstantItem(40).AlignMiddle().Image(LogoBytes).FitArea();
                }

                row.RelativeItem().AlignCenter().Column(col =>
                {
                    col.Item().Text(text =>
                    {
                        text.Span("مستشفى آسيا  ").FontSize(14).Bold().FontColor(PrimaryBlue);
                        text.Span("ASIA HOSPITAL").FontSize(8).FontColor(PrimaryBlue);
                    });
                });
                
                row.ConstantItem(40);
            });

            column.Item().PaddingVertical(5).LineHorizontal(1).LineColor(BorderGray);

            // Ticket Number Box
            column.Item().AlignCenter().PaddingTop(10).Column(c =>
            {
                c.Item().Text("رقم التذكرة / TICKET NUMBER").FontSize(8).FontColor(Colors.Grey.Medium);
                c.Item().Container().Padding(5).Background(LightBlue).AlignCenter().Text(TicketNumber).FontSize(24).Bold().FontColor(PrimaryBlue);
            });

            // Ticket Details
            column.Item().PaddingTop(15).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(80);
                    columns.RelativeColumn();
                });

                void AddRow(string label, string value, bool isBold = false)
                {
                    table.Cell().Element(LabelCell).Text(label);
                    table.Cell().Element(ValueCell).Text(t => 
                    {
                        var txt = t.Span(value ?? "-");
                        if (isBold) txt.Bold().FontSize(11);
                    });
                }

                AddRow("المريض:", PatientName, true);
                AddRow("رقم الملف:", PatientFileNumber);
                AddRow("التاريخ:", Date.ToString("yyyy/MM/dd HH:mm"));
                AddRow("العيادة:", ClinicName);
                AddRow("الطبيب:", DoctorName);
                AddRow("الخدمة:", ServiceName);
                
                table.Cell().ColumnSpan(2).PaddingVertical(10).LineHorizontal(0.5f).LineColor(BorderGray);
                
                AddRow("المبلغ:", $"{Amount:N2} ج.م", true);
            });
            
            // Helpful message
            column.Item().PaddingTop(10).AlignCenter().Text("يرجى الاحتفاظ بهذه التذكرة للمراجعة").FontSize(8).Italic().FontColor(Colors.Grey.Medium);
        });
    }

    void ComposeFooter(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().PaddingTop(5).LineHorizontal(0.5f).LineColor(BorderGray);
            column.Item().PaddingVertical(2).Row(row =>
            {
                row.RelativeItem().Text(t => 
                { 
                    t.Span("المستخدم: ").FontSize(7); 
                    t.Span(UserName).FontSize(7).Bold(); 
                });
                row.RelativeItem().AlignLeft().Text(DateTime.Now.ToString("yyyy/MM/dd HH:mm")).FontSize(7);
            });
        });
    }

    static IContainer LabelCell(IContainer container) => container.PaddingVertical(2).AlignLeft().DefaultTextStyle(x => x.FontSize(9).FontColor(Colors.Grey.Darken2));
    static IContainer ValueCell(IContainer container) => container.PaddingVertical(2).AlignRight().DefaultTextStyle(x => x.FontSize(10));
}
