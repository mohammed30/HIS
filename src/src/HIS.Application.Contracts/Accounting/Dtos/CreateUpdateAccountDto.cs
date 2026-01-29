using System;
using System.ComponentModel.DataAnnotations;

namespace HIS.Accounting.Dtos;

public class CreateUpdateAccountDto
{
    [Required]
    [StringLength(32)]
    public string Code { get; set; }

    [Required]
    [StringLength(128)]
    public string Name { get; set; }

    [StringLength(128)]
    public string NameAr { get; set; }

    public AccountType Type { get; set; }
    public Guid? ParentId { get; set; }
}
