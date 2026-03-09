using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HIS.HR.Printing;

public class PaySlipDocument : IDocument
{
    // Primary brand colors
    private static readonly string PrimaryBlue = "#003366";
    private static readonly string LightBlue = "#E6F2FF";
    private static readonly string AccentRed = "#DC3545";
    private static readonly string TextDark = "#333333";
    private static readonly string TextLight = "#FFFFFF";
    private static readonly string BorderGray = "#CCCCCC";

    public PaySlipDto Data { get; set; }
    public byte[] LogoBytes { get; set; }

    public DocumentMetadata GetMetadata() => new DocumentMetadata
    {
        Title = $"PaySlip - {Data?.EmployeeName}",
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
                    col.Item().PaddingTop(2).Text("قسيمة الراتب / PAY SLIP")
                        .FontSize(10)
                        .FontColor(LightBlue);
                });

                row.ConstantItem(40);
            });

            column.Item().Background(AccentRed).Padding(8).AlignCenter()
                .Text($"قسيمة الراتب - شهر {Data?.PeriodStart:MMMM yyyy}")
                .FontSize(14)
                .Bold()
                .FontColor(TextLight);
        });
    }

    void ComposeContent(IContainer container)
    {
        container.PaddingVertical(15).Column(column =>
        {
            // Employee Info Section
            column.Item().Border(1).BorderColor(BorderGray).Row(row =>
            {
                row.RelativeItem().Padding(10).Column(col =>
                {
                    col.Item().Row(r => { r.RelativeItem().Text("اسم الموظف:").Bold(); r.RelativeItem().Text(Data?.EmployeeName); });
                    col.Item().Row(r => { r.RelativeItem().Text("رقم الموظف:").Bold(); r.RelativeItem().Text(Data?.EmployeeNumber); });
                });
                
                row.RelativeItem().Padding(10).Column(col =>
                {
                    col.Item().Row(r => { r.RelativeItem().Text("الإدارة:").Bold(); r.RelativeItem().Text(Data?.DepartmentName ?? "-"); });
                    col.Item().Row(r => { r.RelativeItem().Text("المسمى الوظيفي:").Bold(); r.RelativeItem().Text(Data?.JobTitle ?? "-"); });
                });
            });

            column.Item().Height(15);

            // Earnings and Deductions Tables
            column.Item().Row(row =>
            {
                // Earnings Table
                row.RelativeItem().Column(col =>
                {
                    col.Item().Background(LightBlue).Padding(5).AlignCenter().Text("الاستحقاقات").Bold().FontColor(PrimaryBlue);
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(1);
                        });

                        foreach (var earning in Data?.Earnings ?? new())
                        {
                            table.Cell().Element(DataCell).Text(earning.ItemName);
                            table.Cell().Element(DataCell).AlignRight().Text($"{earning.Amount:N2}");
                        }

                        table.Cell().Element(FooterCell).Text("إجمالي الاستحقاقات").Bold();
                        table.Cell().Element(FooterCell).AlignRight().Text($"{Data?.TotalEarnings:N2}").Bold();
                    });
                });

                row.ConstantItem(20);

                // Deductions Table
                row.RelativeItem().Column(col =>
                {
                    col.Item().Background(Colors.Red.Lighten5).Padding(5).AlignCenter().Text("الاستقطاعات").Bold().FontColor(AccentRed);
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(1);
                        });

                        foreach (var deduction in Data?.Deductions ?? new())
                        {
                            table.Cell().Element(DataCell).Text(deduction.ItemName);
                            table.Cell().Element(DataCell).AlignRight().Text($"{deduction.Amount:N2}");
                        }

                        table.Cell().Element(FooterCell).Text("إجمالي الاستقطاعات").Bold();
                        table.Cell().Element(FooterCell).AlignRight().Text($"{Data?.TotalDeductions:N2}").Bold();
                    });
                });
            });

            column.Item().Height(20);

            // Net Salary Section
            column.Item().Background(PrimaryBlue).Padding(10).Row(row =>
            {
                row.RelativeItem().AlignMiddle().Text("صافي الراتب / NET SALARY").Bold().FontColor(TextLight).FontSize(14);
                row.RelativeItem().AlignRight().Text($"{Data?.NetSalary:N2} ج.م").Bold().FontColor(TextLight).FontSize(16);
            });
        });
    }

    void ComposeFooter(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().LineHorizontal(1).LineColor(BorderGray);
            column.Item().PaddingTop(5).AlignCenter().Text(text =>
            {
                text.Span("تم إنشاؤه بواسطة نظام مستشفى آسيا للموارد البشرية - ").FontSize(9).Italic();
                text.Span(DateTime.Now.ToString("yyyy/MM/dd HH:mm")).FontSize(9).Italic();
            });
        });
    }

    static IContainer DataCell(IContainer container) => container.BorderBottom(1).BorderColor(Colors.Grey.Lighten3).PaddingVertical(5).PaddingHorizontal(5);
    static IContainer FooterCell(IContainer container) => container.Background(Colors.Grey.Lighten4).PaddingVertical(5).PaddingHorizontal(5);
}
