using System;
using System.ComponentModel.DataAnnotations;

namespace HIS.Clinical;

public class CreateUpdateMedicalOrderDto
{
    [Required]
    public Guid PatientId { get; set; }
    
    [Required]
    public Guid ServiceItemId { get; set; }
    
    public OrderType Type { get; set; }
    
    public string ClinicalNotes { get; set; }
    
    // Optional overrides
    public string Details { get; set; }
}
