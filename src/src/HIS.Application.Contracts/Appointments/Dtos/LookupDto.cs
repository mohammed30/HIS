using System;

namespace HIS.Appointments.Dtos;

public class LookupDto<TKey>
{
    public TKey Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
