using System;
using System.IO;
using System.Linq;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HIS.Reports.Printing
{
    public class InsuranceClaimReportDocument : IDocument
    {
        public InsuranceClaimPrintDataDto ReportData { get; set; }
        
        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public DocumentSettings GetSettings() => DocumentSettings.Default;

        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4.Landscape());
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontFamily("Tahoma").FontSize(10));
                    page.ContentFromRightToLeft();

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    page.Footer().Element(ComposeFooter);
                });
        }

        private void ComposeHeader(IContainer container)
        {
            container.PaddingBottom(15).Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text("تقرير مطالبات شركات التأمين")
                        .FontSize(26).SemiBold().FontColor(Colors.Blue.Darken4);

                    if (!string.IsNullOrEmpty(ReportData.InsuranceCompanyName))
                    {
                        column.Item().Text($"الشركة: {ReportData.InsuranceCompanyName}")
                            .FontSize(15).FontColor(Colors.Grey.Darken3);
                    }
                    else
                    {
                        column.Item().Text("الشركة: جميع الشركات")
                            .FontSize(15).FontColor(Colors.Grey.Darken3);
                    }

                    var period = $"الفترة: {ReportData.StartDate:dd/MM/yyyy} - {ReportData.EndDate:dd/MM/yyyy}";
                    column.Item().Text(period).FontSize(13).FontColor(Colors.Grey.Darken2);
                });

                var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logo", "asia-logo-light.png");
                if (File.Exists(logoPath))
                {
                    row.ConstantItem(140).AlignRight().Image(logoPath);
                }
                else
                {
                    row.ConstantItem(100).Height(80).Placeholder(); // Logo Placeholder
                }
            });
        }

        private void ComposeContent(IContainer container)
        {
            container.Column(column =>
            {
                // Financial Summary
                column.Item().PaddingBottom(20).Element(ComposeSummary);

                // Claims Table
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(1.2f); // التاريخ
                        columns.RelativeColumn(2); // المريض
                        columns.RelativeColumn(1); // رقم التأمين
                        columns.RelativeColumn(1.8f); // الخدمة
                        columns.RelativeColumn(0.8f); // الإجمالي
                        columns.RelativeColumn(0.8f); // تحمل المريض
                        columns.RelativeColumn(0.8f); // تغطية التأمين
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(HeaderStyle).Text("تاريخ الفاتورة");
                        header.Cell().Element(HeaderStyle).Text("اسم المريض");
                        header.Cell().Element(HeaderStyle).Text("رقم البوليصة");
                        header.Cell().Element(HeaderStyle).Text("القسم والخدمات");
                        header.Cell().Element(HeaderStyle).AlignRight().Text("الإجمالي");
                        header.Cell().Element(HeaderStyle).AlignRight().Text("تحمل المريض");
                        header.Cell().Element(HeaderStyle).AlignRight().Text("التأمين");
                    });

                    var claimsByCompany = ReportData.Claims.GroupBy(x => x.InsuranceCompanyName).ToList();

                    foreach (var companyGroup in claimsByCompany)
                    {
                        // Company Group Header
                        table.Cell().ColumnSpan(7).Element(CompanyGroupStyle).Text($"شركة التأمين: {companyGroup.Key}");

                        int rowIndex = 0;
                        foreach (var claim in companyGroup)
                        {
                            bool isFirstItem = true;
                            string bgColor = rowIndex % 2 == 0 ? Colors.White : Colors.Grey.Lighten4;
                            
                            foreach (var item in claim.Items)
                            {
                                table.Cell().Element(c => CellStyle(c, bgColor)).Text(isFirstItem ? claim.InvoiceDate.ToString("dd/MM/yyyy") + $"\n(فاتورة: {claim.InvoiceNumber})" : "");
                                table.Cell().Element(c => CellStyle(c, bgColor)).Text(isFirstItem ? claim.PatientName + $"\n(ملف: {claim.PatientFileNumber})" : "");
                                table.Cell().Element(c => CellStyle(c, bgColor)).Text(isFirstItem ? claim.PolicyNumber : "");
                                table.Cell().Element(c => CellStyle(c, bgColor)).Text($"{item.DepartmentName} - {item.ServiceName}\n(كود: {item.ServiceCode})").FontSize(9);
                                table.Cell().Element(c => CellStyle(c, bgColor)).AlignRight().Text(item.TotalPrice.ToString("N2"));
                                table.Cell().Element(c => CellStyle(c, bgColor)).AlignRight().Text(item.PatientCoPay.ToString("N2")).FontColor(Colors.Orange.Darken2);
                                table.Cell().Element(c => CellStyle(c, bgColor)).AlignRight().Text(item.InsuranceCoverage.ToString("N2")).FontColor(Colors.Green.Darken2);
                                isFirstItem = false;
                            }
                            rowIndex++;
                            
                            // Invoice Total Row
                            table.Cell().ColumnSpan(4).Element(TotalStyle).AlignRight().Text("إجمالي الفاتورة:");
                            table.Cell().Element(TotalStyle).AlignRight().Text(claim.TotalInvoiceAmount.ToString("N2"));
                            table.Cell().Element(TotalStyle).AlignRight().Text(claim.TotalPatientAmount.ToString("N2")).FontColor(Colors.Orange.Darken2);
                            table.Cell().Element(TotalStyle).AlignRight().Text(claim.TotalInsuranceAmount.ToString("N2")).FontColor(Colors.Green.Darken2);
                        }
                        
                        // Company Total Row
                        table.Cell().ColumnSpan(4).Element(CompanyTotalStyle).AlignRight().Text($"إجمالي {companyGroup.Key}:");
                        table.Cell().Element(CompanyTotalStyle).AlignRight().Text(companyGroup.Sum(x => x.TotalInvoiceAmount).ToString("N2"));
                        table.Cell().Element(CompanyTotalStyle).AlignRight().Text(companyGroup.Sum(x => x.TotalPatientAmount).ToString("N2"));
                        table.Cell().Element(CompanyTotalStyle).AlignRight().Text(companyGroup.Sum(x => x.TotalInsuranceAmount).ToString("N2"));
                    }
                });
            });
        }

        private void ComposeSummary(IContainer container)
        {
            var grandTotalInvoice = ReportData.Claims.Sum(x => x.TotalInvoiceAmount);
            var grandTotalPatient = ReportData.Claims.Sum(x => x.TotalPatientAmount);
            var grandTotalInsurance = ReportData.Claims.Sum(x => x.TotalInsuranceAmount);

            container.Background(Colors.Blue.Lighten5)
                .BorderLeft(4).BorderColor(Colors.Blue.Darken2)
                .Padding(15).Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("إجمالي المطالبات").FontSize(11).FontColor(Colors.Grey.Darken3);
                    c.Item().Text(ReportData.Claims.Count.ToString()).FontSize(18).SemiBold().FontColor(Colors.Blue.Darken3);
                });
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("المبلغ الإجمالي (قبل التحمل)").FontSize(11).FontColor(Colors.Grey.Darken3);
                    c.Item().Text(grandTotalInvoice.ToString("N2")).FontSize(18).SemiBold().FontColor(Colors.Black);
                });
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("إجمالي تحمل المرضى").FontSize(11).FontColor(Colors.Grey.Darken3);
                    c.Item().Text(grandTotalPatient.ToString("N2")).FontSize(18).SemiBold().FontColor(Colors.Orange.Darken3);
                });
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("إجمالي المستحق من التأمين").FontSize(11).FontColor(Colors.Grey.Darken3);
                    c.Item().Text(grandTotalInsurance.ToString("N2")).FontSize(18).SemiBold().FontColor(Colors.Green.Darken3);
                });
            });
        }

        private void ComposeFooter(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().PaddingTop(20).Row(row =>
                {
                    row.RelativeItem().AlignCenter().Text("توقيع المحاسب").SemiBold();
                    row.RelativeItem().AlignCenter().Text("توقيع وإعتماد المدير الطبي").SemiBold();
                    row.RelativeItem().AlignCenter().Text("الختم الرسمي للمستشفى").SemiBold();
                });
                
                column.Item().PaddingTop(30).Row(row => 
                {
                    row.RelativeItem().Text(x =>
                    {
                        x.Span("تاريخ الطباعة: ");
                        x.Span(ReportData.PrintDate.ToString("dd/MM/yyyy hh:mm tt"));
                    });
                    row.RelativeItem().AlignRight().Text(x => 
                    {
                        x.Span("صفحة ");
                        x.CurrentPageNumber();
                        x.Span(" من ");
                        x.TotalPages();
                    });
                });
            });
        }

        private static IContainer HeaderStyle(IContainer container)
        {
            return container
                .Background(Colors.Blue.Darken3)
                .BorderBottom(2).BorderColor(Colors.Blue.Darken4)
                .PaddingVertical(10).PaddingHorizontal(8)
                .DefaultTextStyle(x => x.SemiBold().FontColor(Colors.White));
        }

        private static IContainer CellStyle(IContainer container, string backgroundColor)
        {
            return container
                .BorderBottom(1).BorderColor(Colors.Grey.Lighten3)
                .Background(backgroundColor)
                .PaddingVertical(8).PaddingHorizontal(8);
        }

        private static IContainer TotalStyle(IContainer container)
        {
            return container
                .Background(Colors.Grey.Lighten4)
                .PaddingVertical(6).PaddingHorizontal(8)
                .DefaultTextStyle(x => x.SemiBold());
        }
        
        private static IContainer CompanyGroupStyle(IContainer container)
        {
            return container
                .Background(Colors.Blue.Lighten4)
                .BorderBottom(2).BorderColor(Colors.Blue.Lighten2)
                .PaddingVertical(10).PaddingHorizontal(8)
                .DefaultTextStyle(x => x.SemiBold().FontSize(13).FontColor(Colors.Blue.Darken4));
        }

        private static IContainer CompanyTotalStyle(IContainer container)
        {
            return container
                .Background(Colors.Grey.Darken3)
                .PaddingVertical(12).PaddingHorizontal(8)
                .DefaultTextStyle(x => x.SemiBold().FontSize(12).FontColor(Colors.White));
        }
    }
}
