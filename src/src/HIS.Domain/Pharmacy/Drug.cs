using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace HIS.Pharmacy;

public class Drug : FullAuditedAggregateRoot<Guid>
{
    public string Barcode { get; set; }
    public string BrandName { get; set; }
    public string ScientificName { get; set; }
    public string Strength { get; set; }      // e.g. 500mg
    public string Form { get; set; }          // e.g. Tablet, Syrup
    public string Manufacturer { get; set; }
    public string? BatchNumberPrefix { get; set; }
    
    // Link to ServiceItem for Billing/Ordering
    public Guid? ServiceItemId { get; set; }

    protected Drug() { }

    public Drug(Guid id, string barcode, string brandName, string scientificName, string strength, string form, string manufacturer) 
        : base(id)
    {
        Barcode = barcode;
        BrandName = brandName;
        ScientificName = scientificName;
        Strength = strength;
        Form = form;
        Manufacturer = manufacturer;
    }
}
