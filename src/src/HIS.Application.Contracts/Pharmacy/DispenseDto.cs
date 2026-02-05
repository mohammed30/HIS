using System;
using System.ComponentModel.DataAnnotations;

namespace HIS.Pharmacy;

public class DispenseDto
{
    [Required]
    public Guid MedicalOrderId { get; set; }
    
    // Optional: If the user manually overrides which batch to pick (advanced requirement)
    // If null/empty, system uses LIFO.
    // public Guid? SpecifiedBatchId { get; set; } 
    // Keeping it simple as per plan first.
}
