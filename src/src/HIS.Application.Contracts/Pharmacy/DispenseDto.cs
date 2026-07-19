using System;
using System.ComponentModel.DataAnnotations;

namespace HIS.Pharmacy;

public class DispenseDto
{
    [Required]
    public Guid MedicalOrderId { get; set; }
    
    [Required]
    public Guid WarehouseId { get; set; }
    
    public string? CounselingNotes { get; set; }
}
