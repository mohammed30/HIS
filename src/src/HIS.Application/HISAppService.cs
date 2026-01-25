using HIS.Localization;
using Volo.Abp.Application.Services;

namespace HIS;

/* Inherit your application services from this class.
 */
public abstract class HISAppService : ApplicationService
{
    protected HISAppService()
    {
        LocalizationResource = typeof(HISResource);
    }
}
