using System;
using System.IO;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HIS.Laboratory.Printing;

public class LabResultDocument : IDocument
{
    // Primary brand colors
    private static readonly string PrimaryBlue = "#003366";
    private static readonly string LightBlue = "#E6F2FF";
    private static readonly string AccentRed = "#DC3545";
    private static readonly string TextDark = "#333333";
    private static readonly string TextLight = "#FFFFFF";
    private static readonly string BorderGray = "#CCCCCC";

    public string PatientName { get; set; }
    public string PatientId { get; set; }
    public DateTime RequestDate { get; set; }
    public string TestName { get; set; }
    public string TestCode { get; set; }
    public string DoctorName { get; set; }
    public string Result { get; set; }
    public string ReferenceRange { get; set; }
    public string TestUnit { get; set; }
    public string Notes { get; set; }
    public string TechnicianName { get; set; }
    public byte[] LogoBytes { get; set; }

    public DocumentMetadata GetMetadata() => new DocumentMetadata
    {
        Title = "Lab Result Report",
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
                    col.Item().PaddingTop(2).Text("نظام معلومات المستشفى")
                        .FontSize(9).FontColor(LightBlue);
                });

                row.ConstantItem(40); // Spacer for balance
            });

            // Report title bar
            column.Item().Background(AccentRed).Padding(4).AlignCenter()
                .Text("تقرير نتائج التحليل المخبري")
                .FontSize(14).Bold().FontColor(TextLight);
        });
    }

    void ComposeContent(IContainer container)
    {
        container.PaddingVertical(15).Column(column =>
        {
            // Patient Information Section
            column.Item().Element(c => ComposeSection(c, "معلومات المريض", comp =>
            {
                comp.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    table.Cell().Element(InfoCell).Text(text =>
                    {
                        text.Span("اسم المريض: ").Bold();
                        text.Span(PatientName ?? "-");
                    });
                    table.Cell().Element(InfoCell).Text(text =>
                    {
                        text.Span("رقم المريض: ").Bold();
                        text.Span(PatientId ?? "-");
                    });
                    table.Cell().Element(InfoCell).Text(text =>
                    {
                        text.Span("الطبيب المعالج: ").Bold();
                        text.Span(DoctorName ?? "-");
                    });
                    table.Cell().Element(InfoCell).Text(text =>
                    {
                        text.Span("تاريخ الطلب: ").Bold();
                        text.Span(RequestDate.ToString("yyyy/MM/dd"));
                    });
                });
            }));

            column.Item().Height(10);

            // Test Results Section
            column.Item().Element(c => ComposeSection(c, "نتائج التحليل", comp =>
            {
                comp.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(2); // Test Name
                        columns.RelativeColumn(1); // Code
                        columns.RelativeColumn(2); // Result
                        columns.RelativeColumn(1); // Unit
                        columns.RelativeColumn(2); // Reference Range
                    });

                    // Header Row
                    table.Header(header =>
                    {
                        header.Cell().Element(HeaderCell).Text("اسم التحليل").Bold();
                        header.Cell().Element(HeaderCell).Text("الكود").Bold();
                        header.Cell().Element(HeaderCell).Text("النتيجة").Bold();
                        header.Cell().Element(HeaderCell).Text("الوحدة").Bold();
                        header.Cell().Element(HeaderCell).Text("المعدل الطبيعي").Bold();
                    });

                    // Data Row
                    bool isStructured = !string.IsNullOrWhiteSpace(ReferenceRange) && ReferenceRange.TrimStart().StartsWith("[");
                    System.Collections.Generic.List<ReferenceRangeModel> ranges = null;
                    if (isStructured)
                    {
                        try
                        {
                            ranges = System.Text.Json.JsonSerializer.Deserialize<System.Collections.Generic.List<ReferenceRangeModel>>(ReferenceRange, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        }
                        catch { isStructured = false; }
                    }

                    if (isStructured && ranges != null && ranges.Count > 0)
                    {
                        var resultLines = Result?.Split('\n') ?? Array.Empty<string>();
                        var resultMap = new System.Collections.Generic.Dictionary<string, string>();
                        foreach (var line in resultLines)
                        {
                            int colonIdx = line.IndexOf(':');
                            if (colonIdx >= 0)
                            {
                                resultMap[line.Substring(0, colonIdx).Trim()] = line.Substring(colonIdx + 1).Trim();
                            }
                        }

                        bool isFirst = true;
                        foreach (var r in ranges)
                        {
                            if (isFirst)
                            {
                                table.Cell().RowSpan((uint)ranges.Count).Element(DataCell).Text(TestName ?? "-");
                                table.Cell().RowSpan((uint)ranges.Count).Element(DataCell).Text(TestCode ?? "-");
                                isFirst = false;
                            }

                            string resStr = resultMap.ContainsKey(r.Label) ? resultMap[r.Label] : "-";
                            decimal? numVal = null;
                            if (resStr != "-")
                            {
                                var match = System.Text.RegularExpressions.Regex.Match(resStr, @"^[\d\.]+");
                                if (match.Success && decimal.TryParse(match.Value, out var n))
                                    numVal = n;
                            }

                            string color = TextDark;
                            if (numVal.HasValue)
                            {
                                decimal min = r.Min ?? decimal.MinValue;
                                decimal max = r.Max ?? decimal.MaxValue;
                                decimal cMin = r.CriticalMin ?? decimal.MinValue;
                                decimal cMax = r.CriticalMax ?? decimal.MaxValue;

                                if (cMax <= max) cMax = decimal.MaxValue;
                                if (cMin >= min) cMin = decimal.MinValue;

                                if (numVal < cMin || numVal > cMax) color = AccentRed;
                                else if (numVal < min || numVal > max) color = "#FF8C00";
                                else color = "#28A745";
                            }

                            table.Cell().Element(DataCellHighlight).Text(text =>
                            {
                                text.Span(r.Label + "\n").FontSize(9).FontColor(Colors.Grey.Darken2);
                                text.Span(resStr).Bold().FontColor(color).DirectionFromLeftToRight();
                            });

                            table.Cell().Element(DataCell).Text(r.Unit ?? TestUnit ?? "-").DirectionFromLeftToRight();

                            string refText = "";
                            if (r.Min.HasValue && r.Max.HasValue) refText = $"{r.Min} - {r.Max}";
                            else refText = "-";
                            table.Cell().Element(DataCell).Text(refText).DirectionFromLeftToRight();
                        }
                    }
                    else
                    {
                        table.Cell().Element(DataCell).Text(TestName ?? "-");
                        table.Cell().Element(DataCell).Text(TestCode ?? "-");
                        table.Cell().Element(DataCellHighlight).Text(Result ?? "-").Bold().DirectionFromLeftToRight();
                        table.Cell().Element(DataCell).Text(TestUnit ?? "-").DirectionFromLeftToRight();
                        table.Cell().Element(DataCell).Text(ReferenceRange ?? "-").DirectionFromLeftToRight();
                    }
                });
            }));

            column.Item().Height(10);

            // Notes Section (if available)
            if (!string.IsNullOrEmpty(Notes))
            {
                column.Item().Element(c => ComposeSection(c, "ملاحظات", comp =>
                {
                    comp.Item().Background(LightBlue).Padding(10).Text(Notes);
                }));
            }

            column.Item().Height(20);

            // Signatures Section
            column.Item().Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().BorderBottom(1).BorderColor(BorderGray).Height(40);
                    col.Item().PaddingTop(5).AlignCenter().Text("توقيع الفني المختبر").FontSize(10);
                    col.Item().AlignCenter().Text(TechnicianName ?? "").FontSize(9).FontColor(Colors.Grey.Medium);
                });
                
                row.ConstantItem(50);

                row.RelativeItem().Column(col =>
                {
                    col.Item().BorderBottom(1).BorderColor(BorderGray).Height(40);
                    col.Item().PaddingTop(5).AlignCenter().Text("ختم المختبر").FontSize(10);
                });
                
                row.ConstantItem(50);

                row.RelativeItem().Column(col =>
                {
                    col.Item().BorderBottom(1).BorderColor(BorderGray).Height(40);
                    col.Item().PaddingTop(5).AlignCenter().Text("توقيع الطبيب المسؤول").FontSize(10);
                });
            });
        });
    }

    void ComposeSection(IContainer container, string title, Action<ColumnDescriptor> contentAction)
    {
        container.Border(1).BorderColor(BorderGray).Column(column =>
        {
            // Section Header
            column.Item().Background(PrimaryBlue).Padding(8)
                .Text(title).FontSize(12).Bold().FontColor(TextLight);
            
            // Section Content
            column.Item().Padding(10).Column(contentAction);
        });
    }

    void ComposeFooter(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().LineHorizontal(2).LineColor(PrimaryBlue);
            
            column.Item().PaddingTop(10).Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("مستشفى آسيا - قسم المختبر").FontSize(9).FontColor(Colors.Grey.Medium);
                    col.Item().Text("هاتف: +966 XX XXX XXXX").FontSize(8).FontColor(Colors.Grey.Medium);
                });
                
                row.RelativeItem().AlignCenter().Text(text =>
                {
                    text.Span("تاريخ الطباعة: ").FontSize(8).FontColor(Colors.Grey.Medium);
                    text.Span(DateTime.Now.ToString("yyyy/MM/dd HH:mm")).FontSize(8).FontColor(Colors.Grey.Medium);
                });
                
                row.RelativeItem().AlignLeft().Text(text =>
                {
                    text.Span("صفحة ").FontSize(8).FontColor(Colors.Grey.Medium);
                    text.CurrentPageNumber().FontSize(8).FontColor(Colors.Grey.Medium);
                    text.Span(" من ").FontSize(8).FontColor(Colors.Grey.Medium);
                    text.TotalPages().FontSize(8).FontColor(Colors.Grey.Medium);
                });
            });
        });
    }

    // Cell styling methods
    static IContainer HeaderCell(IContainer container)
    {
        return container
            .Background(PrimaryBlue)
            .Padding(8)
            .AlignCenter()
            .DefaultTextStyle(x => x.FontColor(TextLight).FontSize(10));
    }

    static IContainer DataCell(IContainer container)
    {
        return container
            .Background(LightBlue)
            .Border(1)
            .BorderColor(BorderGray)
            .Padding(8)
            .AlignCenter();
    }

    static IContainer DataCellHighlight(IContainer container)
    {
        return container
            .Background(Colors.Yellow.Lighten4)
            .Border(1)
            .BorderColor(BorderGray)
            .Padding(8)
            .AlignCenter();
    }

    static IContainer InfoCell(IContainer container)
    {
        return container.PaddingBottom(8).PaddingRight(15);
    }
}

public class ReferenceRangeModel
{
    public string Label { get; set; }
    public decimal? Min { get; set; }
    public decimal? Max { get; set; }
    public decimal? CriticalMin { get; set; }
    public decimal? CriticalMax { get; set; }
    public string Unit { get; set; }
}
