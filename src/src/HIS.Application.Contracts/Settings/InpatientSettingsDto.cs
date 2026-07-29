namespace HIS.Settings;

public class InpatientSettingsDto
{
    public decimal AdmissionDepositAmount { get; set; } = 1000m;
    public bool RequireAdvancePayment { get; set; } = false;
}
