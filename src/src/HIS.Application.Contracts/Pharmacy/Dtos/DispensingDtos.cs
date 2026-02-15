using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace HIS.Pharmacy.Dtos;

public class DispensingDto : FullAuditedEntityDto<Guid>
{
    public Guid MedicalOrderId { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; }
    public string? CounselingNotes { get; set; }
    public List<DispensedItemDto> Items { get; set; }
}

public class DispensedItemDto
{
    public Guid InventoryItemId { get; set; }
    public string ProductName { get; set; }
    public decimal Quantity { get; set; }
    public string BatchNumber { get; set; }
    public decimal UnitCost { get; set; }
}

public class DispensingLabelDto
{
    public string PatientName { get; set; }
    public string MRN { get; set; }
    public string DrugName { get; set; }
    public string DosageInstructions { get; set; }
    public string DispensedDate { get; set; }
    public string ExpiryDate { get; set; }
    public string PharmacistName { get; set; }
}

public class CreateDispensingDto
{
    public Guid MedicalOrderId { get; set; }
    public Guid PatientId { get; set; }
    public string? CounselingNotes { get; set; }
    public List<CreateDispensedItemDto> Items { get; set; }
}

public class CreateDispensedItemDto
{
    public Guid InventoryItemId { get; set; }
    public Guid InventoryBatchId { get; set; }
    public decimal Quantity { get; set; }
}
