using Volo.Abp.Modularity;

namespace HIS;

/* Inherit from this class for your domain layer tests. */
public abstract class HISDomainTestBase<TStartupModule> : HISTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
