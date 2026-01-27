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
            new SettingDefinition(HISSettings.HospitalTaxNumber, "")
        );
    }
}
