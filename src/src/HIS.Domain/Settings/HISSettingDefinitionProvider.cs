using Volo.Abp.Settings;

namespace HIS.Settings;

public class HISSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(HISSettings.MySetting1));
    }
}
