using System.ComponentModel.DataAnnotations;

namespace HIS.Pharmacy.Dtos;

public class CreateUpdateDrugDto
{
    [Required]
    [StringLength(128)]
    public string Barcode { get; set; }

    [Required]
    [StringLength(128)]
    public string BrandName { get; set; }

    [Required]
    [StringLength(128)]
    public string ScientificName { get; set; }

    [StringLength(64)]
    public string? Strength { get; set; }

    [StringLength(64)]
    public string? Form { get; set; }

    [StringLength(128)]
    public string? Manufacturer { get; set; }
    
    [StringLength(32)]
    public string? BatchNumberPrefix { get; set; }

    public int MinimumStockLevel { get; set; }
    public int ReorderLevel { get; set; }
    
    [StringLength(64)]
    public string? BinLocation { get; set; }

    public decimal Price { get; set; } // To set on ServiceItem
}
