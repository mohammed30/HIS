using Microsoft.Extensions.Localization;
using HIS.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace HIS;

[Dependency(ReplaceServices = true)]
public class HISBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<HISResource> _localizer;

    public HISBrandingProvider(IStringLocalizer<HISResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
