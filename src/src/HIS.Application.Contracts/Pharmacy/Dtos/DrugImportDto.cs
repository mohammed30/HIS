using System.ComponentModel.DataAnnotations;

namespace HIS.Pharmacy.Dtos;

public class DrugImportDto
{
    public string Barcode { get; set; }
    public string BrandName { get; set; }
    public string ScientificName { get; set; }
    public string Strength { get; set; }
    public string Form { get; set; }
    public string Manufacturer { get; set; }
    public string BatchNumberPrefix { get; set; }
    public int MinimumStockLevel { get; set; }
    public int ReorderLevel { get; set; }
    public string BinLocation { get; set; }
    public string IsControlled { get; set; } // Yes/No
    public string LegalCategory { get; set; }
    public decimal Price { get; set; }
}
