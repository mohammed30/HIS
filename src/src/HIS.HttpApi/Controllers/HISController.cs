using HIS.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace HIS.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class HISController : AbpControllerBase
{
    protected HISController()
    {
        LocalizationResource = typeof(HISResource);
    }
}
