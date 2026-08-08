using System;
using System.Linq;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HIS.Settings.Printing;

public class DoctorRevenueReportDocument : IDocument
{
    public DoctorRevenueReportDto ReportData { get; set; }
    public string HospitalName { get; set; } = "مستشفى آسيا";
    public bool IsHospitalReport { get; set; }

    public DocumentMetadata GetMetadata() => new DocumentMetadata
    {
        Title = IsHospitalReport ? "كشف حساب حق المستشفى" : "كشف حساب حق الطبيب",
        Author = HospitalName
    };

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(1.5f, Unit.Centimetre);
            page.PageColor(Colors.White);
            page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Segoe UI"));
            page.ContentFromRightToLeft();

            page.Header().Element(ComposeHeader);
            page.Content().Element(ComposeContent);
            page.Footer().Element(ComposeFooter);
        });
    }

    void ComposeHeader(IContainer container)
    {
        var logoPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot", "images", "logo", "leptonxlite", "logo-dark.png");

        container.Column(headerCol => 
        {
            headerCol.Item().PaddingBottom(15).Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text(HospitalName).FontSize(22).Bold().FontColor(Colors.Blue.Darken2);
                    column.Item().Text(IsHospitalReport ? "كشف حساب حق المستشفى" : "كشف حساب حق الطبيب").FontSize(16).SemiBold().FontColor(Colors.Grey.Darken3);
                    column.Item().PaddingTop(5).Text($"الفترة من: {ReportData.FromDate:yyyy/MM/dd} إلى: {ReportData.ToDate:yyyy/MM/dd}").FontSize(12).FontColor(Colors.Grey.Medium);
                });

                if (System.IO.File.Exists(logoPath))
                {
                    row.ConstantItem(120).AlignRight().Image(logoPath);
                }
                else
                {
                    // Fallback if logo not found
                    var fallbackPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "images", "logo", "leptonxlite", "logo-dark.png");
                    if (System.IO.File.Exists(fallbackPath))
                    {
                        row.ConstantItem(120).AlignRight().Image(fallbackPath);
                    }
                }
            });
            
            headerCol.Item().PaddingBottom(15).LineHorizontal(2).LineColor(Colors.Blue.Lighten2);
        });
    }

    void ComposeContent(IContainer container)
    {
        container.Column(column =>
        {
            if (IsHospitalReport)
            {
                ComposeHospitalSummary(column);
            }
            else
            {
                ComposeDoctorDetails(column);
            }
        });
    }

    void ComposeHospitalSummary(ColumnDescriptor column)
    {
        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(3); // Doctor Name
                columns.RelativeColumn(1); // Revenue
                columns.RelativeColumn(1); // Hosp %
                columns.RelativeColumn(1); // Hosp Amount
            });

            table.Header(header =>
            {
                header.Cell().Element(HeaderCell).Text("اسم الطبيب").FontColor(Colors.White).Bold();
                header.Cell().Element(HeaderCell).Text("إجمالي الإيرادات").FontColor(Colors.White).Bold();
                header.Cell().Element(HeaderCell).Text("نسبة المستشفى").FontColor(Colors.White).Bold();
                header.Cell().Element(HeaderCell).Text("حق المستشفى").FontColor(Colors.White).Bold();
            });

            foreach (var line in ReportData.Lines)
            {
                table.Cell().Element(DataCell).Text(line.DoctorName);
                table.Cell().Element(DataCell).Text($"{line.TotalRevenue:N2}");
                table.Cell().Element(DataCell).Text($"{line.HospitalPercentage}%");
                table.Cell().Element(DataCell).Text($"{line.HospitalAmount:N2}");
            }

            // Totals
            table.Cell().ColumnSpan(1).Element(TotalCell).Text("الإجماليات").Bold();
            table.Cell().Element(TotalCell).Text($"{ReportData.TotalRevenue:N2}").Bold();
            table.Cell().Element(TotalCell).Text("-");
            table.Cell().Element(TotalCell).Text($"{ReportData.TotalHospitalAmount:N2}").Bold();
        });
    }

    void ComposeDoctorDetails(ColumnDescriptor column)
    {
        foreach (var doctor in ReportData.Lines)
        {
            column.Item().PaddingTop(10).Text($"الطبيب: {doctor.DoctorName}").FontSize(14).Bold().FontColor(Colors.Blue.Darken2);
            column.Item().PaddingBottom(10).Text($"نسبة الطبيب: {doctor.DoctorPercentage}%").FontSize(10).FontColor(Colors.Grey.Darken2);

            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(1); // Date
                    columns.RelativeColumn(1); // Invoice Number
                    columns.RelativeColumn(2); // Patient Name
                    columns.RelativeColumn(2); // Service Name
                    columns.RelativeColumn(1); // Service Price
                    columns.RelativeColumn(1); // Doctor Amount
                });

                table.Header(header =>
                {
                    header.Cell().Element(HeaderCell).Text("التاريخ").FontColor(Colors.White).Bold();
                    header.Cell().Element(HeaderCell).Text("رقم الفاتورة").FontColor(Colors.White).Bold();
                    header.Cell().Element(HeaderCell).Text("اسم المريض").FontColor(Colors.White).Bold();
                    header.Cell().Element(HeaderCell).Text("الخدمة").FontColor(Colors.White).Bold();
                    header.Cell().Element(HeaderCell).Text("سعر الخدمة").FontColor(Colors.White).Bold();
                    header.Cell().Element(HeaderCell).Text("حق الطبيب").FontColor(Colors.White).Bold();
                });

                foreach (var detail in doctor.Details)
                {
                    table.Cell().Element(DataCell).Text($"{detail.Date:yyyy/MM/dd}");
                    table.Cell().Element(DataCell).Text(detail.InvoiceNumber);
                    table.Cell().Element(DataCell).Text(detail.PatientName);
                    table.Cell().Element(DataCell).Text(detail.ServiceName);
                    table.Cell().Element(DataCell).Text($"{detail.ServicePrice:N2}");
                    table.Cell().Element(DataCell).Text($"{detail.DoctorAmount:N2}");
                }

                // SubTotals for Doctor
                table.Cell().ColumnSpan(4).Element(TotalCell).Text("الإجمالي للطبيب").Bold();
                table.Cell().Element(TotalCell).Text($"{doctor.TotalRevenue:N2}").Bold();
                table.Cell().Element(TotalCell).Text($"{doctor.DoctorAmount:N2}").Bold();
            });
            
            column.Item().PaddingBottom(20); // space between doctors
        }

        // Grand Totals
        column.Item().PaddingTop(10).Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(4);
                columns.RelativeColumn(1);
                columns.RelativeColumn(1);
            });

            table.Cell().Element(TotalCell).Text("الإجمالي العام").Bold();
            table.Cell().Element(TotalCell).Text($"{ReportData.TotalRevenue:N2}").Bold();
            table.Cell().Element(TotalCell).Text($"{ReportData.TotalDoctorAmount:N2}").Bold();
        });
    }

    void ComposeFooter(IContainer container)
    {
        container.AlignCenter().Text(x =>
        {
            x.Span("الصفحة ");
            x.CurrentPageNumber();
            x.Span(" من ");
            x.TotalPages();
        });
    }

    static IContainer HeaderCell(IContainer container) => container.Background(Colors.Blue.Darken2).Border(1).BorderColor(Colors.White).Padding(8).AlignCenter();
    static IContainer DataCell(IContainer container) => container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(8).AlignCenter();
    static IContainer TotalCell(IContainer container) => container.Background(Colors.Blue.Lighten5).BorderTop(2).BorderColor(Colors.Blue.Darken2).Padding(8).AlignCenter();
}
