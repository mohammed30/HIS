namespace HIS.Notifications;

/// <summary>
/// Defines the available notification type constants.
/// </summary>
public static class NotificationTypes
{
    public const string Appointment = "appointment";
    public const string Lab         = "lab";
    public const string Pharmacy    = "pharmacy";
    public const string Radiology   = "radiology";
    public const string Inventory   = "inventory";
    public const string Billing     = "billing";
    public const string Emergency   = "emergency";
    public const string System      = "system";
    public const string Nursing     = "nursing";
    public const string Patients    = "patients";
    public const string Reception   = "reception";
    public const string Insurance   = "insurance";

    public static readonly string[] All =
    [
        Appointment,
        Lab,
        Pharmacy,
        Radiology,
        Inventory,
        Billing,
        Emergency,
        System,
        Nursing,
        Patients,
        Reception,
        Insurance
    ];
}
