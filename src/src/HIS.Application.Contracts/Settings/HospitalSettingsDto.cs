using System.ComponentModel.DataAnnotations;

namespace HIS.Settings;

public class HospitalSettingsDto
{
    [Required]
    public string HospitalName { get; set; } = string.Empty;

    public string? HospitalAddress { get; set; }

    public string? HospitalPhone { get; set; }

    [EmailAddress]
    public string? HospitalEmail { get; set; }

    public string? HospitalLogo { get; set; }

    public string? HospitalTaxNumber { get; set; }
}
