using System;
using System.ComponentModel.DataAnnotations;
using HIS.Financials;

namespace HIS.Financials.Dtos;

public class CreateUpdateAccountDto
{
    [Required]
    public string NameAr { get; set; }
    
    public string NameEn { get; set; }
    
    public Guid? ParentId { get; set; }
    
    [Required]
    public AccountType Type { get; set; }
}
