using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HIS.Accounting.Dtos;

public class CreateUpdateJournalEntryDto
{
    [Required]
    public DateTime Date { get; set; }

    [StringLength(64)]
    public string ReferenceNumber { get; set; }

    [Required]
    [StringLength(512)]
    public string Description { get; set; }

    [Required]
    [MinLength(2)]
    public List<CreateUpdateJournalEntryLineDto> Lines { get; set; } = new();
}

public class CreateUpdateJournalEntryLineDto
{
    [Required]
    public Guid AccountId { get; set; }

    public decimal Debit { get; set; }

    public decimal Credit { get; set; }
}
