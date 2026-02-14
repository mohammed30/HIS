using System;
using System.ComponentModel.DataAnnotations;

namespace HIS.Inventory.Dtos;

public class CreateUpdateSupplierDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [StringLength(100)]
    public string ContactPerson { get; set; }

    [StringLength(20)]
    public string Phone { get; set; }

    [StringLength(100)]
    public string Email { get; set; }

    [StringLength(200)]
    public string Address { get; set; }

    [StringLength(50)]
    public string TaxId { get; set; }
}
