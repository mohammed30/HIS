using System;
using Volo.Abp.Application.Dtos;

namespace HIS.Pharmacy.Dtos;

public class DrugDto : AuditedEntityDto<Guid>
{
    public string Barcode { get; set; }
    public string BrandName { get; set; }
    public string ScientificName { get; set; }
    public string Strength { get; set; }
    public string Form { get; set; }
    public string Manufacturer { get; set; }
    public string BatchNumberPrefix { get; set; }
    public Guid? ServiceItemId { get; set; }
    public string ServiceItemName { get; set; }
}
