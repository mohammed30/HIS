using System;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HIS.Operations.Printing;

public class OperationTicketDocument : IDocument
{
    // Primary brand colors
    private static readonly string PrimaryBlue = "#003366";
    private static readonly string LightBlue = "#E6F2FF";
    private static readonly string TextDark = "#333333";
    private static readonly string BorderGray = "#CCCCCC";

    public string TicketNumber { get; set; }
    public DateTime Date { get; set; }
    public string PatientName { get; set; }
    public string PatientFileNumber { get; set; }
    public string OperationName { get; set; }
    public string DoctorName { get; set; }
    public string AnesthesiaType { get; set; }
    public decimal Amount { get; set; }
    public string UserName { get; set; }
    public byte[] LogoBytes { get; set; }

    public DocumentMetadata GetMetadata() => new DocumentMetadata
    {
        Title = $"Operation Ticket {TicketNumber}",
        Author = "Asia Hospital",
        Subject = "Surgery Admission Ticket",
        Keywords = "Surgery, Ticket, Admission",
        Creator = "HIS System",
        Producer = "HIS System",
        CreationDate = DateTime.Now,
        ModificationDate = DateTime.Now
    };

    public void Compose(IDocumentContainer container)
    {
        container
            .Page(page =>
            {
                page.Size(PageSizes.A5); // A5 is better for surgery tickets (more details)
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10).FontColor(TextDark).FontFamily("Arial"));
                page.ContentFromRightToLeft(); // RTL
                
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

            row.RelativeItem().AlignCenter().Column(col =>
            {
                col.Item().Text("مستشفى آسيا").FontSize(18).Bold().FontColor(PrimaryBlue);
                col.Item().Text("ASIA HOSPITAL").FontSize(10).FontColor(PrimaryBlue);
                col.Item().Text("تذكرة دخول عمليات / SURGERY TICKET").FontSize(12).Bold().Underline();
            });
            
            row.ConstantItem(60).AlignMiddle().Column(col => 
            {
                col.Item().Text(DateTime.Now.ToString("dd/MM/yyyy")).FontSize(8);
                col.Item().Text(DateTime.Now.ToString("HH:mm")).FontSize(8);
            });
        });
    }

    void ComposeContent(IContainer container)
    {
        container.PaddingVertical(10).Column(column =>
        {
            column.Item().PaddingVertical(5).LineHorizontal(1).LineColor(BorderGray);

            // Ticket Number Box
            column.Item().AlignCenter().PaddingBottom(10).Column(c =>
            {
                c.Item().Text("رقم العملية / OPERATION NO").FontSize(8).FontColor(Colors.Grey.Medium);
                c.Item().Container().Padding(5).Background(LightBlue).CornerRadius(5).AlignCenter().Text(TicketNumber).FontSize(24).Bold().FontColor(PrimaryBlue);
            });

            // Patient & Operation Details
            column.Item().Border(0.5f).BorderColor(BorderGray).Padding(10).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(100);
                    columns.RelativeColumn();
                });

                void AddRow(string label, string value, bool isBold = false)
                {
                    table.Cell().Element(LabelCell).Text(label);
                    table.Cell().Element(ValueCell).Text(t => 
                    {
                        var txt = t.Span(value ?? "-");
                        if (isBold) txt.Bold().FontSize(12);
                    });
                }

                AddRow("اسم المريض:", PatientName, true);
                AddRow("رقم الملف:", PatientFileNumber);
                AddRow("تاريخ العملية:", Date.ToString("yyyy/MM/dd HH:mm"));
                AddRow("نوع العملية:", OperationName, true);
                AddRow("الجراح:", DoctorName);
                AddRow("التخدير:", AnesthesiaType);
                
                table.Cell().ColumnSpan(2).PaddingVertical(10).LineHorizontal(0.5f).LineColor(BorderGray);
                
                AddRow("المبلغ الإجمالي:", $"{Amount:N2} ج.م", true);
            });
            
            // Instructions
            column.Item().PaddingTop(20).Column(c => 
            {
                c.Item().Text("تعليمات / Instructions:").Bold().Underline();
                c.Item().Text("1. يرجى الحضور قبل موعد العملية بساعتين.").FontSize(9);
                c.Item().Text("2. الصيام لمدة 8 ساعات قبل العملية.").FontSize(9);
                c.Item().Text("3. إحضار جميع الفحوصات والأشعة السابقة.").FontSize(9);
            });
        });
    }

    void ComposeFooter(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().PaddingTop(10).LineHorizontal(0.5f).LineColor(BorderGray);
            column.Item().PaddingVertical(5).Row(row =>
            {
                row.RelativeItem().Text(t => 
                { 
                    t.Span("حرر بواسطة: ").FontSize(8); 
                    t.Span(UserName).FontSize(8).Bold(); 
                });
                row.RelativeItem().AlignLeft().Text(t =>
                {
                    t.Span("Page ").FontSize(8);
                    t.CurrentPageNumber().FontSize(8);
                    t.Span(" of ").FontSize(8);
                    t.TotalPages().FontSize(8);
                });
            });
        });
    }

    static IContainer LabelCell(IContainer container) => container.PaddingVertical(4).AlignLeft().DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Grey.Darken2).SemiBold());
    static IContainer ValueCell(IContainer container) => container.PaddingVertical(4).AlignRight().DefaultTextStyle(x => x.FontSize(10));
}
