using Volo.Abp.Settings;

namespace HIS.Settings;

public class HISSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        context.Add(
            new SettingDefinition(HISSettings.HospitalName, "My Hospital"),
            new SettingDefinition(HISSettings.HospitalAddress, ""),
            new SettingDefinition(HISSettings.HospitalPhone, ""),
            new SettingDefinition(HISSettings.HospitalEmail, ""),
            new SettingDefinition(HISSettings.HospitalLogo, ""),
            new SettingDefinition(HISSettings.HospitalTaxNumber, ""),
            new SettingDefinition(HISSettings.Pharmacy.AllowNegativeStock, "false"),
            new SettingDefinition("HIS.Inventory.MainWarehouseId", ""),
            new SettingDefinition("HIS.Inventory.PharmacyWarehouseId", ""),

            // Notifications module subscribers
            new SettingDefinition("Notifications.Subscribers.Appointments", ""),
            new SettingDefinition("Notifications.Subscribers.Radiology", ""),
            new SettingDefinition("Notifications.Subscribers.Pharmacy", ""),
            new SettingDefinition("Notifications.Subscribers.Emergency", ""),
            new SettingDefinition("Notifications.Subscribers.Operations", ""),
            new SettingDefinition("Notifications.Subscribers.Billing", ""),
            new SettingDefinition("Notifications.Subscribers.Inventory", ""),
            new SettingDefinition("Notifications.Subscribers.Laboratory", ""),
            new SettingDefinition("Notifications.Subscribers.Inpatient", ""),
            new SettingDefinition("Notifications.Subscribers.Accounting", ""),
            new SettingDefinition("Notifications.Subscribers.HR", ""),
            new SettingDefinition("Notifications.Subscribers.Reception", ""),
            new SettingDefinition("Notifications.Subscribers.Payments", ""),
            new SettingDefinition("Notifications.Subscribers.Nursing", ""),
            new SettingDefinition("Notifications.Subscribers.Insurance", ""),
            new SettingDefinition("Notifications.Subscribers.Patients", "")
        );
    }
}
