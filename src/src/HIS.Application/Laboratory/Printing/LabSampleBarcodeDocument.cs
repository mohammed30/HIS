using System;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HIS.Laboratory.Printing;

public class LabSampleBarcodeDocument : IDocument
{
    public string PatientName { get; set; }
    public string SampleNumber { get; set; }
    public string TestName { get; set; }
    public DateTime RequestDate { get; set; }

    public DocumentMetadata GetMetadata() => new DocumentMetadata { Title = "Lab Sample Label" };

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            // Standard small label size: 50mm x 30mm
            page.Size(5, 3, Unit.Centimetre);
            page.Margin(0.2f, Unit.Centimetre);
            page.PageColor(Colors.White);
            page.DefaultTextStyle(x => x.FontSize(8).FontColor(Colors.Black));
            page.ContentFromRightToLeft();

            page.Content().Column(col =>
            {
                col.Spacing(2);
                
                col.Item().AlignCenter().Text("مستشفى آسيا - مختبر").FontSize(7).Bold();
                
                col.Item().BorderBottom(1).PaddingBottom(2).Row(row =>
                {
                    row.RelativeItem().Text(PatientName).Bold().FontSize(9);
                });

                col.Item().Row(row =>
                {
                    row.RelativeItem().Text($"عينة: {TestName}").FontSize(7);
                    row.RelativeItem().AlignLeft().Text(RequestDate.ToString("yyyy/MM/dd")).FontSize(7);
                });

                // Simulated Barcode Area
                col.Item().PaddingVertical(2).AlignCenter().Height(0.8f, Unit.Centimetre).Element(DrawSimulatedBarcode);
                
                col.Item().AlignCenter().Text(SampleNumber).FontSize(10).SemiBold();
            });
        });
    }

    void DrawSimulatedBarcode(IContainer container)
    {
        // For now, we draw a series of vertical lines to represent a barcode
        // In a production environment, we would use a barcode generation library
        container.Row(row =>
        {
            var random = new Random(SampleNumber.GetHashCode());
            for (int i = 0; i < 40; i++)
            {
                row.RelativeItem(random.Next(1, 4)).Background(Colors.Black).Height(0.8f, Unit.Centimetre);
                row.RelativeItem(random.Next(1, 3)).Background(Colors.White).Height(0.8f, Unit.Centimetre);
            }
        });
    }
}
